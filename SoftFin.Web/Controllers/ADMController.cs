using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using SoftFin.Vindi.callApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ADMController : BaseController
    {
        public class ListaDashs
        {
            public ListaDashs()
            {
                ListaFuncionalidades = new List<ObterPorHoldId>();
            }
            public int id { get; set; }
            public string Descricao { get; set; }
            public List<ObterPorHoldId> ListaFuncionalidades { get; set; }
        }

        public class ObterPorHoldId
        {
            public int id { get; set; }
            public string Descricao { get; set; }
            public string valor { get; set; }
        }

        public ActionResult Index()
        {
            
            ViewData["holding"] = "";
            if (!Acesso.UsuarioLogado().ToUpper().Equals("JARVIS"))
                ViewData["holding"] = new Holding().ObterTodos(_paramBase).First().id;

            SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

            return View();
        }


        //[OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Listas(JqGridRequest request)
        {
            Dictionary<string, string> parameters = HttpContext.Request.QueryString.Keys.Cast<string>()
                .ToDictionary(k => k, v => HttpContext.Request.QueryString[v]);


            int totalRecords = 0;
            List<Holding> objs = new List<Holding>();
            if (Acesso.UsuarioLogado().ToUpper().Equals("JARVIS"))
                objs = new Holding().ObterTodosSF();
            else
                objs = new Holding().ObterTodos(_paramBase);

            if (!String.IsNullOrEmpty(UtilSoftFin.ExtraiString(parameters, "param$holding")))
            {
                var auxstring = parameters["param$holding"];
                objs = objs.Where(p => p.nome.ToUpper().Contains( auxstring.ToUpper())).ToList();
            }

            totalRecords = objs.Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            objs = UtilSoftFin.Organiza<Holding>(objs, request);
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.nome,
                    item.codigo,
                    item.bloqueada,
                    item.motivobloqueada
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        public JsonResult ObterPorId(int id)
        {
            try
            {
                Holding hold;
                Empresa Empresa;
                List<Estabelecimento> Estabs;
                EmpresaContaContabil configConta;


                if (id == 0)
                {
                    hold = new Holding();
                    Empresa = new Empresa();
                    Estabs = new List<Estabelecimento>();
                    configConta = new EmpresaContaContabil();
                }
                else
                {
                    hold = new Holding().ObterPorId(id, _paramBase);
                  
                    if (hold == null)
                        Empresa = new Empresa();
                    else
                        Empresa = new Empresa().ObterPorHoldId(hold.id, _paramBase);

                    if (Empresa == null)
                        Estabs = new List<Estabelecimento>();
                    else
                        Estabs = new Estabelecimento().ObterPorEmpresaId(Empresa.id);


                    foreach (var item in Estabs)
	                {
                        item.MunicipioDesc =item.Municipio.DESC_MUNICIPIO; 
                    }

                    configConta = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);

                    if (configConta == null)
                        configConta = new EmpresaContaContabil();
                }

                configConta.empresa_id = _empresa;

                var estabs = Estabs.Select(p => new
                {
                    p.Apelido,
                    p.BAIRRO,
                    CEP = p.CEP.Replace("-", ""),
                    CNPJ = p.CNPJ.Replace(".", "").Replace("/", "").Replace("-", ""),
                    p.Codigo,
                    p.Complemento,
                    p.Empresa_id,
                    p.id,
                    p.InscricaoEstadual,
                    p.InscricaoMunicipal,
                    p.Logradouro,
                    p.Municipio_id,
                    p.MunicipioDesc,
                    p.NomeCompleto,
                    p.NumeroLogradouro,
                    p.UF,
                    p.Logo,
                    p.ativo,
                    senhaCertificado = string.IsNullOrEmpty(p.senhaCertificado) ? "Não" : "Sim",
                    opcaoTributariaSimples_id = p.opcaoTributariaSimples_id.ToString(),
                    p.CodigoTributacaoMunicipio,
                    p.autorizacaoCPAG,
                    p.autorizacaoFaturamento,
                    p.emailNotificacoes,
                    p.Fone,
                    p.CRT,
                    p.MigrateCode

                }).ToList();


                return Json(new
                {
                    CDStatus = "OK",
                    hold = new
                    {
                        hold.id,
                        hold.motivobloqueada,
                        hold.nome,
                        hold.bloqueada,
                        hold.codigo,
                        hold.codigoApiPagamento
                    },
                    Empresa = new
                    {
                        Empresa.apelido,
                        Empresa.codigo,
                        Empresa.Holding_id,
                        Empresa.id,
                        Empresa.nome
                    },
                    Estabs = estabs,
                    ConfigConta = new {
                        configConta.empresa_id,
                        configConta.ContaContabilTitulo_id,
                        configConta.ContaContabilPagamento_id,
                        configConta.ContaContabilRecebimento_id,
                        configConta.ContaContabilNFMercadoria_id,
                        configConta.ContaContabilNFServico_id,
                        configConta.ContaContabilOutro_id,
                        configConta.ContaContabilRecebimentoDC_id
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObterPerfilPorIdEmpresa(int id)
        {
            try
            {
                List<Perfil> Ps;

                if (id == 0)
                {
                    Ps = new List<Perfil>();
                }
                else
                {
                    Ps = new Perfil().ObterPorEmpresa(id);
                }
            
                return Json(new { CDStatus = "OK", Perfils = Ps.Select(p => new { p.id, p.Codigo, p.Descricao, p.empresa_id }) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObterPerfilPorId(int id)
        {
            try
            {
                Perfil perfils;

                if (id == 0)
                {
                    perfils = new Perfil();
                }
                else
                {
                    perfils = new Perfil().ObterPorId(id, _paramBase);
                }

                return Json(new { CDStatus = "OK", Perfil = perfils }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult ObterUsuarioPorIdEmpresa(int id)
        {
            try
            {
                List<Usuario> Usuarios;

                if (id == 0)
                {
                    Usuarios = new List<Usuario>();
                }
                else
                {
                    Usuarios = new Usuario().ObterPorEmpresa(id);
                }

                foreach (var item in Usuarios)
                {
                    item.senha = "";
                }


                return Json(new { Usuarios = Usuarios.Select(p => new
                {
                    p.alterarsenha,
                    p.codigo,
                    p.empresa_id,
                    p.id,
                    p.idPerfil,
                    p.logado,
                    p.nome,
                    p.tokenApi,
                    p.usuarioBloqueado
                    
                }) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ObterUsuarioPorId(int id)
        {
            try
            {
                Usuario usuarios;

                if (id == 0)
                {
                    usuarios = new Usuario();
                }
                else
                {
                    usuarios = new Usuario().ObterPorId(id);
                }

                usuarios.senha = "";

                return Json(new { Usuarios = new {
                    usuarios.alterarsenha,
                    usuarios.codigo,
                    usuarios.empresa_id,
                    usuarios.id,
                    usuarios.idPerfil,
                    usuarios.logado,
                    usuarios.nome,
                    usuarios.tokenApi,
                    usuarios.usuarioBloqueado
                } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObterFuncionalidades(int id)
        {
            try
            {
                List<SistemaDashBoard> sistemaDashBoard = new SistemaDashBoard().ObterTodos();





                return Json(new { sistemaDashBoard }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Salva(Holding holding, Empresa empresa)
        {
            try
            {
                var validacao = holding.Validar(ModelState);

                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                validacao = empresa.Validar(ModelState);

                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                var db = new DbControle();
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    if (holding.id == 0)
                    {
                        holding.Incluir(_paramBase, db);

                    }
                    else
                    {
                        holding.Alterar(_paramBase, holding, db);
                    }

                    empresa.Holding_id = holding.id;
                    if (empresa.id == 0)
                    {
                        empresa.Incluir(_paramBase, db);
                    }
                    else
                    {
                        empresa.Alterar(_paramBase, db);
                    }
                    dbcxtransaction.Commit();
                }
                SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

                return ObterPorId(holding.id);
               
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SalvaConfig(EmpresaContaContabil obj)
        {
            try
            {
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var validacao = obj.Validar(ModelState);

                if (obj.ContaContabilTitulo_id == 0)
                    validacao.Add("Informe a conta de despesa padrão");

                if (obj.ContaContabilPagamento_id == 0)
                    validacao.Add("Informe a conta de pagamento padrão");

                if (obj.ContaContabilRecebimento_id== 0)
                    validacao.Add("Informe a conta de recebimento padrão");

                if (obj.ContaContabilNFServico_id == 0)
                    validacao.Add("Informe a conta de NFSe padrão");

                if (obj.ContaContabilNFMercadoria_id == 0)
                    validacao.Add("Informe a conta de NFe padrão");


                if (obj.ContaContabilOutro_id == 0)
                    validacao.Add("Informe a conta de Outro padrão");

                if (obj.ContaContabilRecebimentoDC_id == 0)
                    validacao.Add("Informe a conta de Recebimento Débito Crédito");

                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao, DSMessage = "Campos inválidos" }, JsonRequestBehavior.AllowGet);
                var db = new DbControle();

                var empresaContaContabil = new EmpresaContaContabil().ObterPorEmpresa(_paramBase, db);
                if (empresaContaContabil == null)
                {   
                    obj.Incluir(_paramBase);
                    obj = empresaContaContabil;
                }
                else
                {
                    empresaContaContabil.ContaContabilTitulo_id = obj.ContaContabilTitulo_id;
                    empresaContaContabil.ContaContabilPagamento_id = obj.ContaContabilPagamento_id;
                    empresaContaContabil.ContaContabilRecebimento_id = obj.ContaContabilRecebimento_id;
                    empresaContaContabil.ContaContabilNFMercadoria_id = obj.ContaContabilNFMercadoria_id;
                    empresaContaContabil.ContaContabilNFServico_id = obj.ContaContabilNFServico_id;
                    empresaContaContabil.ContaContabilOutro_id = obj.ContaContabilOutro_id;
                    empresaContaContabil.ContaContabilRecebimentoDC_id = obj.ContaContabilRecebimentoDC_id;
                    empresaContaContabil.Alterar(_paramBase, db);
                    obj = empresaContaContabil;
                }

                var x = new
                {
                    obj.empresa_id,
                    obj.ContaContabilTitulo_id,
                    obj.ContaContabilPagamento_id,
                    obj.ContaContabilRecebimento_id,
                    obj.ContaContabilNFMercadoria_id,
                    obj.ContaContabilNFServico_id,
                    obj.ContaContabilOutro_id,
                    obj.ContaContabilRecebimentoDC_id
                };

                SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

                return Json(new { CDStatus = "OK", ConfigConta = x }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SalvaEstab(Estabelecimento estab)
        {
            try
            {
                var buscamunic = new Municipio().ObterPorNome(estab.MunicipioDesc);
                if ( buscamunic.Count() == 0)
                    return Json(new { CDStatus = "NOK", Erros = new List<string> { "Municipio Inválido" } }, JsonRequestBehavior.AllowGet);
                else
                    estab.Municipio_id =buscamunic.FirstOrDefault().ID_MUNICIPIO;

                var validacao = estab.Validar(ModelState);
                validacao = validacao.Where(p => p.Contains("O campo id é obrigatório.")).ToList();
                validacao = validacao.Where(p => p.Contains("Campo Municipio obrigatório, somente números")).ToList();
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                estab.CNPJ = long.Parse(estab.CNPJ).ToString(@"00\.000\.000\/0000\-00");
                estab.CEP = int.Parse(estab.CEP).ToString(@"00000\-000");
                if (estab.id == 0)
                    estab.Incluir(_paramBase);
                else
                {
                    var estabAux = new Estabelecimento().ObterPorId(estab.id, _paramBase);
                    estab.Logo = estabAux.Logo;
                    estab.senhaCertificado = estabAux.senhaCertificado;
                    estab.Alterar(_paramBase);
                }
                SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

                return ObterEstabsPorEmpresaID(estab.Empresa_id);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", Exception = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObterUsuarioDefault(int id)
        {
            try
            {
                Usuario usuario;

                List<PerfilPagarAprovacao> perfilPagarAprovacaos = new List<PerfilPagarAprovacao>();

                if (id == 0)
                    usuario = new Usuario();
                else
                {
                    usuario = new Usuario().ObterPorId(id);
                    var usuarioEstabelecimento = new UsuarioEstabelecimento().ObterTodosPorIdUsuario(id);
                    foreach (var item in usuarioEstabelecimento)
                    {
                        var perfilPagarAprovacao = new PerfilPagarAprovacao().ObterPorEstabUsuario(item.estabelecimento_id, usuario.id);
                        if (perfilPagarAprovacao == null)
                        {
                            perfilPagarAprovacao = new PerfilPagarAprovacao();
                            perfilPagarAprovacao.usuarioAutorizador_id = usuario.id;
                            perfilPagarAprovacao.estabelecimento_id = item.estabelecimento_id;
                            perfilPagarAprovacao.Inativo = false;
                        }
                        else
                        {
                            perfilPagarAprovacao.valor = true;
                            perfilPagarAprovacao.Inativo = !perfilPagarAprovacao.Inativo;
                        }
                        perfilPagarAprovacaos.Add(perfilPagarAprovacao);
                    }

                }
                usuario.senha = "";

                return Json(new { CDStatus = "OK",
                    usuario = new {
                        usuario.id,
                        usuario.alterarsenha,
                        usuario.codigo,
                        usuario.empresa_id,
                        usuario.idPerfil,
                        usuario.logado,
                        usuario.nome,
                        usuario.tokenApi,
                        usuario.usuarioBloqueado
                    }, estabs = perfilPagarAprovacaos.Select(p => new { p.estabelecimento_id, p.id, p.Inativo, p.usuarioAutorizador_id,p.valor, p.valorLimiteCPAG, p.valorLimiteNFSE })}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ObterPerfilDefault(int id)
        {

            var retorno = ConvertMVVMPertfil(id);
            Perfil perfil;

            if (id != 0)
                perfil = new Perfil().ObterPorId(id, _paramBase);
            else
                perfil = new Perfil();

            return Json(new { CDStatus = "OK", funcs = retorno, perfil = new { perfil.Codigo, perfil.Descricao, perfil.empresa_id, perfil.id } }, JsonRequestBehavior.AllowGet);
        }

        private static List<ListaDashs> ConvertMVVMPertfil(int id)
        {
            var retorno = GeraPerfilDefault();

            if (id != 0)
            {
                var rels = new PerfilFuncionalidade().ObterPorIdPerfil(id);
                foreach (var item in retorno)
                {
                    foreach (var item2 in item.ListaFuncionalidades)
                    {
                        var nivel = rels.Where(p => p.idFuncionalidade == item2.id);
                        if (nivel.Count() > 0)
                        {
                            item2.valor = nivel.First().flgTipoAcesso;
                        }
                    }
                }
            }
            return retorno;
        }

        private static List<ListaDashs> GeraPerfilDefault()
        {

            var retorno = new List<ListaDashs>();
            var sistemaDashBoard = new SistemaDashBoard();
            var sistemaDashBoardFuncionalidade = new SistemaDashBoardFuncionalidade();

            var objs = sistemaDashBoard.ObterTodos();

            foreach (var item in objs)
            {
                var itemobj = new ListaDashs { Descricao = item.Descricao, id = item.id };
                foreach (var itemFunc in sistemaDashBoardFuncionalidade.ObterPorIdDash(itemobj.id))
                {
                    if (!itemFunc.Funcionalidade.Jarvis.Value)
                    {
                        itemobj.ListaFuncionalidades.Add(
                            new ObterPorHoldId { id = itemFunc.Funcionalidade_id, Descricao = itemFunc.Descricao, valor = "N" }
                        );
                    }
                }
                retorno.Add(itemobj);
            }


            return retorno;
        }


        [HttpPost]
        public JsonResult SalvaUsuario(Usuario usuario, List<PerfilPagarAprovacao> estabelecimentos)
        {
            try
            {

                var validacao = usuario.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);


                int usuarioid = 0;

                if (usuario.id == 0)
                {
                    usuario.Incluir(_paramBase);
                    usuarioid = usuario.id;
                }
                else
                {
                    var usuarioBanco = new Usuario().ObterPorId(usuario.id);
                    var usu = new Usuario();
                    usu.alterarsenha = usuario.alterarsenha; 
                    if (usuario.alterarsenha == false)
                    {
                        usu.senha = usuarioBanco.senha;
                    }
                    else
                    {
                        usu.senha = usuario.senha;
                    }
                    usu.codigo = usuario.codigo;
                    usu.empresa_id = usuario.empresa_id;
                    usu.idPerfil = usuario.idPerfil;
                    usu.logado = usuarioBanco.logado;
                    usu.nome = usuario.nome;
                    usu.id = usuario.id;
                    usu.usuarioBloqueado = usuario.usuarioBloqueado;
                    new Usuario().Alterar(usu, _paramBase);
                    usuarioid = usu.id;
                }
                var rels = new UsuarioEstabelecimento().ObterTodosPorIdUsuario(usuarioid);
                foreach (var item in rels)
                {
                    item.Excluir(item.id, _paramBase);   
                }

                var rels2 = new PerfilPagarAprovacao().ObterTodosPorIdUsuario(usuarioid);
                foreach (var item in rels2)
                {
                    string aux = "";
                    item.Excluir(item.id,ref aux,_paramBase);
                }
                if (estabelecimentos != null)
                {
                    foreach (var item in estabelecimentos)
                    {
                        item.usuarioAutorizador_id = usuarioid;
                        item.Inativo = !item.Inativo;

                        new UsuarioEstabelecimento().Incluir(new UsuarioEstabelecimento { estabelecimento_id = item.estabelecimento_id, usuario_id = usuarioid }, _paramBase);
                        new PerfilPagarAprovacao().Incluir(item, _paramBase);
                    }
                }
                



                return Json(new { CDStatus = "OK"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SalvaPerfil(Perfil perfil, List<ListaDashs> dash)
        {
            try
            {

                var validacao = perfil.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);


                if (perfil.id == 0)
                    perfil.Incluir(_paramBase);
                else
                    perfil.Alterar(perfil,_paramBase);

                var rels = new PerfilFuncionalidade().ObterPorIdPerfil(perfil.id);
                foreach (var item in rels)
                {
                    string erro = "";
                    item.Excluir(item.id, ref erro, _paramBase);   
                }

                foreach (var item in dash)
                {
                    foreach (var item2 in item.ListaFuncionalidades)
                    {
                        if (item2.valor != "N")
                        {
                            var obj = new PerfilFuncionalidade();
                            obj.idPerfil = perfil.id;
                            obj.idFuncionalidade = item2.id;
                            obj.flgTipoAcesso = item2.valor;
                            obj.Incluir(_paramBase);
                        }
                    }
                }



                SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

                return Json(new { CDStatus = "OK"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public JsonResult ExcluirPerfil(int id)
        {
            try
            {
                string texto = "";

                var objs = new PerfilFuncionalidade().ObterPorIdPerfil(id);

                foreach (var item in objs)
                {
                    var erro = "";
                    new PerfilFuncionalidade().Excluir(item.id, ref erro, _paramBase);
                }

                new Perfil().Excluir(id,ref texto, _paramBase);

                if (texto == "")
                {
                    return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        private JsonResult ObterEstabsPorEmpresaID(int id)
        {
            try
            {

                List<Estabelecimento> Estabs;

                if (id == 0)
                {
                    Estabs = new List<Estabelecimento>();
                }
                else
                {
                    Estabs = new Estabelecimento().ObterPorEmpresaId(id);
                }

                foreach (var item in Estabs)
                {
                    item.MunicipioDesc =item.Municipio.DESC_MUNICIPIO; 
                }


                return Json(new
                {
                    CDStatus = "OK",
                    Estabs = Estabs.Select(p => new
                    {
                        p.Apelido,
                        p.BAIRRO,
                        CEP = p.CEP.Replace("-", ""),
                        CNPJ = p.CNPJ.Replace(".", "").Replace("/", "").Replace("-", ""),
                        p.Codigo,
                        p.Complemento,
                        p.Empresa_id,
                        p.id,
                        p.InscricaoEstadual,
                        p.InscricaoMunicipal,
                        p.Logradouro,
                        p.Municipio_id,
                        p.MunicipioDesc,
                        p.NomeCompleto,
                        p.NumeroLogradouro,
                        p.UF,
                        p.Logo,
                        p.ativo,
                        senhaCertificado = string.IsNullOrEmpty(p.senhaCertificado) ? "Não" : "Sim" ,
                        opcaoTributariaSimples_id = p.opcaoTributariaSimples_id.ToString(),
                        p.CodigoTributacaoMunicipio,
                        p.autorizacaoCPAG,
                        p.autorizacaoFaturamento
                        ,
                        p.emailNotificacoes,
                        p.Fone,
                        p.CRT,
                        p.MigrateCode

                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
 
        }

        public JsonResult ListaMunicipios()
        {
            var municipios = new Municipio().ObterTodos();
            var items = new List<SelectListItem>();
            foreach (var item in municipios)
            {
                items.Add(new SelectListItem() { Value = item.DESC_MUNICIPIO.ToString(), Text = item.DESC_MUNICIPIO.ToString() });
            }
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }
        
        public JsonResult ListaHoldings()
        {
            var holding = new Holding().ObterTodosSF();
            var items = new List<SelectListItem>();
            foreach (var item in holding)
            {
                items.Add(new SelectListItem() { Value = item.nome.ToString(), Text = item.nome.ToString() });
            }
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult UploadLogo(int id)
        {
            if (Request.Files.Count == 0)
            {
                var db = new DbControle();

                var estabelecimento = new Estabelecimento().ObterPorId(id, db, _paramBase);
                estabelecimento.Logo = "";
                estabelecimento.Alterar(_paramBase, db);
            }
            else
            {

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase arquivo = Request.Files[i];
                    string[] extensionarquivos = new string[] { ".jpg", ".jpeg", ".png" };
                    if (arquivo.FileName != "")
                    {
                        if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                        {
                            return Json(new { CDStatus = "OK", DSMessage = "Impossivel salvar, extenção não permitida" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                var estabelecimento = new Estabelecimento();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase arquivo = Request.Files[i];

                    if (arquivo.ContentLength > 0)
                    {
                        var uploadPath = Server.MapPath("~/Logo/");
                        Directory.CreateDirectory(uploadPath);

                        var nomearquivonovo = "Logo_" + id.ToString() + Path.GetExtension(arquivo.FileName);

                        string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                        arquivo.SaveAs(caminhoArquivo);

                        AzureStorage.UploadFile(caminhoArquivo,
                                    "Logo/" + nomearquivonovo,
                                    ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                        var db = new DbControle();

                        estabelecimento = new Estabelecimento().ObterPorId(id, db, _paramBase);
                        estabelecimento.Logo = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                    "Logo/" + nomearquivonovo;
                        estabelecimento.Alterar(_paramBase, db);

                    }
                }
            }
            SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

            return Json(new { CDStatus = "OK", DSMessage = "Incluido com sucesso " }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult UploadCert(int id,string senha)
        {
            try
            {
                if (string.IsNullOrEmpty(senha))
                {
                    throw new Exception("Favor informar a senha");
                }

                if (Request.Files.Count == 0)
                {
                    throw new Exception("Selecione um arquivo para upload.");
                }
                else
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase arquivo = Request.Files[i];
                        string[] extensionarquivos = new string[] { ".pfx" };
                        if (arquivo.FileName != "")
                        {
                            if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                            {
                                throw new Exception("Impossivel salvar, extenção não permitida"); 
                            }
                        }
                    }
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase arquivo = Request.Files[i];

                        if (arquivo.ContentLength > 0)
                        {
                            var uploadPath = Server.MapPath("~/TXTTemp/");
                            Directory.CreateDirectory(uploadPath);

                            var nomearquivonovo = "Cert_" + id.ToString() + ".txt";

                            string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                            arquivo.SaveAs(caminhoArquivo);

                            ValidaSenha(senha, caminhoArquivo);

                            AzureStorage.UploadFile(caminhoArquivo,
                                        "Certificados/" + id.ToString() + "/cert.pfx",
                                        ConfigurationManager.AppSettings["StorageAtendimento"].ToString()); 
                            



                        }


                        var db = new DbControle();
                        var estabelecimento = new Estabelecimento().ObterPorId(id, db, _paramBase);
                        estabelecimento.senhaCertificado = senha;
                        estabelecimento.Alterar(_paramBase, db);
                    }
                }
                SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

                return Json(new { CDStatus = "OK", DSMessage = "Incluido com sucesso " }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        private static void ValidaSenha(string senha, string caminhoArquivo)
        {
            try
            {
                // Create a collection object and populate it using the PFX file
                X509Certificate2Collection collection = new X509Certificate2Collection();
                var y = StreamFile(caminhoArquivo);
                collection.Import(y, senha, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }
            catch(Exception ex)
            {
                
                throw new Exception("Senha do certificado inválida. Mensagem: " + ex.Message + " Caminho : " + caminhoArquivo);
            }
        }

        
        private static byte[] StreamFile(string filename)
        {
            try
            {

                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

                // Create a byte array of file stream length
                byte[] ImageData = new byte[fs.Length];

                //Read block of bytes from stream into the byte array
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

                //Close the File Stream
                fs.Close();
                return ImageData; //return the byte data
            }
            catch (Exception ex)
            {
                throw new Exception(" StreamFile - " + ex.Message);
            }
        }

        [HttpPost]
        public JsonResult AlterarUnidade(UnidadeNegocio unidade)
        {
            try
            {

                var validacao = unidade.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);


                if (unidade.id == 0)
                    unidade.Incluir(_paramBase);
                else
                    unidade.Alterar(unidade, _paramBase);

                return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ExcluirUnidade(int id)
        {
            try
            {
                string texto = "";

                new UnidadeNegocio().Excluir(id, ref texto, _paramBase);

                if (texto == "")
                {
                    return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = texto}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private JsonResult ObterUnidadePorId(int id)
        {
            try
            {

                UnidadeNegocio unidade;

                if (id == 0)
                {
                    unidade = new UnidadeNegocio();
                }
                else
                {
                    unidade = new UnidadeNegocio().ObterPorId(id, _paramBase);
                }


                return Json(new
                {
                    CDStatus = "OK",
                    unidade = new
                    { unidade.empresa_id,
                        unidade.id,
                        unidade.unidade
                    }
                    
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ListaUnidades(int id)
        {
            var objs = new UnidadeNegocio().ObterTodosporIdEmpresa(id);
            
            return Json(new
            {
                objs = objs.Select(p => new
                {
                    p.unidade,
                    p.empresa_id,
                    p.id
                })
            
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListaPagamentos(int id)
        {
            string codigoApi = null;

            if (_usuario.ToUpper().Equals("JARVIS"))
            {
                codigoApi = new Empresa().ObterPorId(id).Holding.codigoApiPagamento;

            }
            else
            {
                codigoApi = _estabobj.Empresa.Holding.codigoApiPagamento;
            }

            if (codigoApi == null)
                return Json(null, JsonRequestBehavior.AllowGet);


            var objsx = new Fatura().getList(codigoApi);

            return Json(new { objs = objsx }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaOpcaoSimples()
        {
            var data = new SelectList(new OpcaoTributariaSimples().ObterTodos(_paramBase), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPessoaPorCPFCNPJ(int id)
        {
            var data = new Pessoa().ObterPorCNPJ(id.ToString(),_paramBase);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SalvarHome(int idPerfil, 
            string calendarioJson, 
            string saldosJson,
            string notasJson,
            string parcelasJson)
        {
            ExcluirTodosSistemaPrincipalPerfil(idPerfil);
            SalvaPermissao(idPerfil, calendarioJson, "Calendario");
            SalvaPermissao(idPerfil, saldosJson, "Saldos");
            SalvaPermissao(idPerfil, notasJson, "Notas");
            SalvaPermissao(idPerfil, parcelasJson, "Parcelas");

            return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        private void ExcluirTodosSistemaPrincipalPerfil(int idPerfil)
        {
            var perfils = new SistemaPrincipalPerfil().ObterPorIdPerfil(idPerfil);

            foreach (var item in perfils)
            {
                string erro = "";

                item.Excluir(ref erro, _paramBase);
                if (erro != "")
                {
                    throw new Exception("Erro ao excluir a tabela SistemaPrincipalPerfil => " + erro);
                }
            }
        }

        private void SalvaPermissao(int idPerfil, string json, string codigo)
        {
            var permissaoCPAG = new SistemaPrincipalPerfil();
            permissaoCPAG.perfil_id = idPerfil;
            permissaoCPAG.sistemaprincipal_id = new SistemaPrincipal().ObterPorCodigo(codigo).id;
            permissaoCPAG.Json = json;
            permissaoCPAG.Incluir(_paramBase);
            SoftFin.Utils.UtilSoftFin.CacheSF.Reset();

        }
        private string ObterPermissao(int idPerfil, string codigo)
        {
            var sistemaprincipal_id = new SistemaPrincipal().ObterPorCodigo(codigo).id;
            var permissao = new SistemaPrincipalPerfil().ObterPorIdPerfilSistemaPrincipal(idPerfil, sistemaprincipal_id);
            if (permissao != null)
            {
                return permissao.Json;
            }

            if (codigo == "Calendario")
            {
                return "{\"contasapagar\":false,\"faturamento\":false,\"importacaonfMunicipal\":false,\"classificacao\":false,\"processo\":false}";
            }
            else
            {
                return "{\"sim\":false}";
            }


        }

        public JsonResult ObterHome(int idPerfil)
        {
            var perfil = new Perfil().ObterPorId(idPerfil, _paramBase);
            return Json(new {
                    CalendarioJson = ObterPermissao(idPerfil, "Calendario"),
                SaldosJson = ObterPermissao(idPerfil, "Saldos"),
                NotasJson = ObterPermissao(idPerfil, "Notas"),
                ParcelasJson = ObterPermissao(idPerfil, "Parcelas")
            } , JsonRequestBehavior.AllowGet);
        }

    }
}
