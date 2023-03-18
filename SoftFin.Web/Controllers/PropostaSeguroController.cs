using Lib.Web.Mvc.JQuery.JqGrid;
using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace SoftFin.Web.Controllers
{
    public class PropostaSeguroController : BaseController
    {
        //
        // GET: /Proposta/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SalvaProposta(PropostaApolice entidade)
        {
            try
            {
                this.ValidaEstabUsuarioException(entidade.estabelecimento_id);


                entidade.isValidoRegrasNegocio(_paramBase);

                var apoliceentidade = new PropostaApolice();

                var nomePessoa = entidade.Segurado_Principal_Nome.Split(',')[0];
                var cnpjPessoa = entidade.Segurado_Principal_Nome.Split(',')[1];
                entidade.Segurado_Principal_id = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa, _paramBase).id;

                if (!string.IsNullOrEmpty(entidade.seguradora_descr))
                {
                    var nomePessoa2 = entidade.seguradora_descr.Split(',')[0];
                    var cnpjPessoa2 = entidade.seguradora_descr.Split(',')[1];
                    entidade.seguradora_id = new Pessoa().ObterPorNomeCNPJ(nomePessoa2, cnpjPessoa2, _paramBase).id;
                }
                else{
                    entidade.seguradora_id = null;
                }
                

                var validacao = entidade.Validar(ModelState);
                
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                if (entidade.id == 0)
                {
                    entidade.Incluir(_paramBase);
                }
                else
                {
                    entidade.Alterar(_paramBase);
                }

                return ConvertMVVM(entidade);

            }
            catch(Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AdicionarParcela(ParcelaSeguro entidade)
        {
            try
            {
                
                var validacao = entidade.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                var historicoaux = entidade.historico;
                for (int i = 1; i <= entidade.numeroParcelas; i++)
                {
                    entidade.historico = historicoaux; 
                    entidade.historico = entidade.historico.Replace("{NU}", entidade.numero.ToString());
                    entidade.historico = entidade.historico.Replace("{NP}", entidade.numeroParcelas.ToString());
                    entidade.historico = entidade.historico.Replace("{DV}", entidade.dataOriginalVencto.ToShortDateString());
                    entidade.PropostaApolice_id = entidade.PropostaApolice_id;

                    entidade.Incluir(_paramBase);
                    entidade.numero += 1;
                    entidade.dataOriginalVencto = entidade.dataOriginalVencto.AddMonths(1);
                }
                
                var objs = entidade.ObterTodosPorIDApolice(entidade.PropostaApolice_id);

                return Json(new
                {
                    CDStatus = "OK",
                    parcelas = (entidade == null) ? null : objs.Select(p => new
                    {
                        p.id, 
                        p.historico, 
                        p.numero,
                        p.PropostaApolice_id, 
                        p.dataConciliado, 
                        dataOriginalVencto = p.dataOriginalVencto.ToString("o"), 
                        p.valorParcelaRepresentante, 
                        valorReceber = (p.valorReceber == null)? null: p.valorReceber.Value.ToString("n"),
                        valorPagar = (p.valorPagar == null)? null: p.valorPagar.Value.ToString("n")}).ToList()

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ExcluirParcela(int id)
        {
            var erro = "";
            try
            {
                var entidade = new ParcelaSeguro().ObterPorId(id);

                entidade.Excluir(entidade.id, ref erro,_paramBase);

                var objs = entidade.ObterTodosPorIDApolice(entidade.PropostaApolice_id);

                return Json(new
                {
                    CDStatus = "OK",
                    parcelas = (entidade == null) ? null : objs.Select(p => new
                    {
                        p.id,
                        p.historico,
                        p.numero,
                        p.dataConciliado,
                        dataOriginalVencto = p.dataOriginalVencto.ToString("o"),
                        p.valorParcelaRepresentante,
                        valorReceber = (p.valorReceber == null) ? null : p.valorReceber.Value.ToString("n"),
                        valorPagar = (p.valorPagar == null) ? null : p.valorPagar.Value.ToString("n")
                    }).ToList()

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Sinistro
        [HttpPost]
        public JsonResult AdicionarSinistro(Sinistro entidade)
        {
            try
            {
                var validacao = entidade.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);
                if (entidade.dataAviso == DateTime.MinValue)
                    entidade.dataAviso = null;

                if (entidade.dataLiquidacao == DateTime.MinValue)
                    entidade.dataLiquidacao = null;

                if (entidade.dataVistoria == DateTime.MinValue)
                    entidade.dataVistoria = null;


                if (entidade.id == 0)
                    entidade.Incluir(_paramBase);
                else
                {
                    var strHistorico = "";
                    var entidadeAntes = new Sinistro().ObterPorId(entidade.id);

                    var usuario = _usuario;
                    if (entidadeAntes.dataAviso != entidade.dataAviso)
                    {
                        if (entidadeAntes.dataAviso == null)
                            strHistorico += "[Data Aviso Mudou de Vazio para " + entidade.dataAviso.Value.ToShortDateString() + "] ";
                        else if (entidade.dataAviso == null)
                            strHistorico += "[Data Aviso Mudou de " + entidadeAntes.dataAviso.Value.ToShortDateString() + " para Vazio] ";  
                        else
                            strHistorico += "[Data Aviso Mudou de " + entidadeAntes.dataAviso.Value.ToShortDateString() + " para " + entidade.dataAviso.Value.ToShortDateString() + "] ";  
                    }

                    if (entidadeAntes.dataLiquidacao != entidade.dataLiquidacao)
                    {
                        if (entidadeAntes.dataLiquidacao == null)
                            strHistorico += "[Data Liquidação Mudou de Vazio para " + entidade.dataLiquidacao.Value.ToShortDateString() + "] ";
                        else if (entidade.dataLiquidacao == null)
                            strHistorico += "[Data Liquidação Mudou de " + entidadeAntes.dataLiquidacao.Value.ToShortDateString() + " para Vazio] ";
                        else
                            strHistorico += "[Data Liquidação Mudou de " + entidadeAntes.dataLiquidacao.Value.ToShortDateString() + " para " + entidade.dataLiquidacao.Value.ToShortDateString() + "] ";
                    }

                    if (entidadeAntes.dataOcorrencia != entidade.dataOcorrencia)
                    {
                        if (entidadeAntes.dataOcorrencia == null)
                            strHistorico += "[Data Ocorrência Mudou de Vazio para " + entidade.dataOcorrencia.ToShortDateString() + "] ";
                        else if (entidade.dataOcorrencia == null)
                            strHistorico += "[Data Ocorrência Mudou de " + entidadeAntes.dataOcorrencia.ToShortDateString() + " para Vazio] ";
                        else
                            strHistorico += "[Data Ocorrência Mudou de " + entidadeAntes.dataOcorrencia.ToShortDateString() + " para " + entidade.dataOcorrencia.ToShortDateString() + "] ";
                    }

                    if (entidadeAntes.dataVistoria != entidade.dataVistoria)
                    {
                        if (entidadeAntes.dataVistoria == null)
                            strHistorico += "[Data Vistoria Mudou de Vazio para " + entidade.dataVistoria.Value.ToShortDateString() + "] ";
                        else if (entidade.dataVistoria == null)
                            strHistorico += "[Data Vistoria Mudou de " + entidadeAntes.dataVistoria.Value.ToShortDateString() + " para Vazio] ";
                        else
                            strHistorico += "[Data Vistoria Mudou de " + entidadeAntes.dataVistoria.Value.ToShortDateString() + " para " + entidade.dataVistoria.Value.ToShortDateString() + "] ";
                    }

                    if (entidadeAntes.numeroProcesso != entidade.numeroProcesso)
                    {
                        strHistorico += "[Número de Processo Mudou de '" + entidadeAntes.numeroProcesso + "' para '" + entidade.numeroProcesso + "'] ";
                    }

                    if (entidadeAntes.valor != entidade.valor)
                    {
                        strHistorico += "[Valor Mudou de '" + entidadeAntes.valor.ToString("n") + "' para '" + entidade.valor.ToString("n") + "'] ";
                    }


                    if (entidadeAntes.historico != entidade.historico)
                    {
                        strHistorico += "[Histórico Mudou de '" + entidadeAntes.historico + "' para '" + entidade.historico + "'] ";
                    }


                    if (entidadeAntes.statusSeguro != entidade.statusSeguro)
                    {
                        strHistorico += "[Status Mudou de '" + entidadeAntes.statusSeguro + "' para '" + entidade.statusSeguro + "'] ";
                    }


                    if (entidadeAntes.tipodeSinistro != entidade.tipodeSinistro)
                    {
                        strHistorico += "[Status Mudou de '" + entidadeAntes.tipodeSinistro + "' para '" + entidade.tipodeSinistro + "'] ";
                    }
                    new SinistroHistorico().Incluir(new SinistroHistorico { usuario = usuario, DataHora = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(), historico = strHistorico, sinistro_id = entidade.id });

                    entidade.Alterar(entidade, _paramBase);
                }


                var objs = entidade.ObterTodosPorIDApolice(entidade.PropostaApolice_id);

                return ConvertMVVMSinistros(objs);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        private JsonResult ConvertMVVMSinistros( List<Sinistro> objs)
        {
            return Json(new
            {
                CDStatus = "OK",
                sinistros = (objs == null) ? null : objs.Select(
                p => new
                {
                    dataAviso = (p.dataAviso == null)? null: p.dataAviso.Value.ToString("o"),
                    dataLiquidacao = (p.dataLiquidacao == null) ? null : p.dataLiquidacao.Value.ToString("o"),
                    dataOcorrencia = p.dataOcorrencia.ToString("o"),
                    dataVistoria = (p.dataVistoria == null) ? null : p.dataVistoria.Value.ToString("o"),
                    descricao = p.descricao,
                    historico = p.historico,
                    numeroProcesso = p.numeroProcesso,
                    p.statusSeguro,
                    p.tipodeSinistro,
                    PropostaApolice_id = p.PropostaApolice_id.ToString(),
                    valor = p.valor,
                    id = p.id,
                    p.active
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExcluirSinistro(int id)
        {
            var erro = "";
            try
            {
                var entidade = new Sinistro().ObterPorId(id);

                entidade.Excluir(entidade.id, ref erro, _paramBase);

                var objs = entidade.ObterTodosPorIDApolice(entidade.PropostaApolice_id);

                return ConvertMVVMSinistros(objs);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObterSinistros(int id)
        {

            try
            {
                var objs = new Sinistro().ObterTodosPorIDApolice(id);
                return ConvertMVVMSinistros(objs);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult NovoSinistro()
        {
            try
            {
            
                var objs = new List<Sinistro>();
                objs.Add(new Sinistro{dataOcorrencia = DateTime.Now});
                return ConvertMVVMSinistros(objs);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult NovoEndosso()
        {
            try
            {

                var objs = new List<EndossoSeguro>();
                objs.Add(new EndossoSeguro { data = DateTime.Now });
                return ConvertMVVMEndossos(objs);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion


        #region Endosso
        [HttpPost]
        public JsonResult AdicionarEndosso(EndossoSeguro entidade)
        {
            try
            {
                var validacao = entidade.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                var historicoaux = entidade.descricao;

                entidade.numero = entidade.numero;
                entidade.descricao = historicoaux;
                entidade.descricao = entidade.descricao.Replace("{NU}", entidade.numero.ToString());
                entidade.descricao = entidade.descricao.Replace("{NP}", entidade.numeroParcelas.ToString());
                entidade.descricao = entidade.descricao.Replace("{DV}", entidade.data.ToShortDateString());
                entidade.PropostaApolice_id = entidade.PropostaApolice_id;


                if (entidade.id == 0)
                    entidade.Incluir(_paramBase);
                else
                    entidade.Alterar(entidade, _paramBase);

                var objs = entidade.ObterTodosPorIDApolice(entidade.PropostaApolice_id);

                return ConvertMVVMEndossos(objs);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        private JsonResult ConvertMVVMEndossos(List<EndossoSeguro> objs)
        {
            return Json(new
            {
                CDStatus = "OK",
                Endossos = (objs == null) ? null : objs.Select(
                p => new
                {
                    id = p.id,
                    numero = p.numero,
                    PropostaApolice_id = p.PropostaApolice_id,
                    valor = p.valor,
                    data = p.data.ToString("o"),
                    dataRegistro = p.dataRegistro.ToString("o"),
                    descricao = p.descricao,
                    p.status

                })
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ExcluirEndosso(int id)
        {
            var erro = "";
            try
            {
                var entidade = new EndossoSeguro().ObterPorId(id);

                entidade.Excluir(entidade.id, ref erro, _paramBase);

                var objs = entidade.ObterTodosPorIDApolice(entidade.PropostaApolice_id);

                return ConvertMVVMEndossos(objs);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ObterEndossos(int id)
        {

            try
            {
                var objs = new EndossoSeguro().ObterTodosPorIDApolice(id);
                return ConvertMVVMEndossos(objs);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        public JsonResult ObterParcelas(int id)
        {

            try
            {
                var objs = new ParcelaSeguro().ObterTodosPorIDApolice(id);
                return Json(new
                {
                    CDStatus = "OK",
                    parcelas = (objs == null) ? null : objs.Select(p => new { 
                        p.id, 
                        p.historico, 
                        p.numero,
                        p.PropostaApolice_id, 
                        p.dataConciliado, 
                        dataOriginalVencto = p.dataOriginalVencto.ToString("o"), 
                        p.valorParcelaRepresentante, 
                        valorReceber = (p.valorReceber == null)? null: p.valorReceber.Value.ToString("n"),
                        valorPagar = (p.valorPagar == null)? null: p.valorPagar.Value.ToString("n")}).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ObterPropostaPorId(int id)
        {
            try
            {
                PropostaApolice entidade;

                if (id == 0)
                {
                    entidade = new PropostaApolice();
                    entidade.estabelecimento_id = _estab;
                    entidade.dataProposta = DateTime.Now;
                    entidade.dataRegistro = DateTime.Now;
                    entidade.dataVigenciaInicio = DateTime.Now;
                    entidade.dataVigenciaFim = DateTime.Now;
                    entidade.tipoPropostaSeguro_id = 1;

                    entidade.observacoes = "Obs:";
                    entidade.tipoStatusPropostaSeguro_id = 1;
                    entidade.codigo = new AuxiliarContador().GeraNovoContador("PropostaSeguro", _estab).ToString();
                    entidade.estabelecimento_id = _estab;
                    
                }
                else
                {
                    entidade = new PropostaApolice().ObterPorId(id, _paramBase);
                }

                return ConvertMVVM(entidade);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        private JsonResult ConvertMVVM(PropostaApolice entidade)
        {
            var Segurado_Principal_Nome = "";
            if (entidade.Segurado_Principal_id != 0)
            {
                var x = new Pessoa().ObterPorId(entidade.Segurado_Principal_id, _paramBase);
                if (x != null)
                {
                    Segurado_Principal_Nome = x.nome + ", " + x.cnpj;
                }
            }

            var seguradora_descr = "";
            if (entidade.seguradora_id != null)
            {
                if (entidade.seguradora_id != 0)
                {
                    var x = new Pessoa().ObterPorId(entidade.seguradora_id.Value, _paramBase);
                    if (x != null)
                    {
                        seguradora_descr = x.nome + ", " + x.cnpj;
                    }
                }
            }

            return Json(
                new
                {
                    CDStatus = "OK",
                    entidade = new
                    {
                        id = entidade.id,
                        dataProposta = entidade.dataProposta.ToString("o"),
                        dataRegistro = entidade.dataRegistro.ToString("o"),
                        dataEfetivacaoProposta = (entidade.dataEfetivacaoProposta == null) ? null : entidade.dataEfetivacaoProposta.Value.ToString("o"),
                        dataVigenciaInicio = entidade.dataVigenciaInicio.ToString("o"),
                        dataVigenciaFim = entidade.dataVigenciaFim.ToString("o"),
                        tipoStatusPropostaSeguro_id = entidade.tipoStatusPropostaSeguro_id.ToString(),
                        tipoPropostaSeguro_id = entidade.tipoPropostaSeguro_id.ToString(),
                        observacoes = entidade.observacoes,
                        Segurado_Principal_Nome,
                        numeroApolice = entidade.numeroApolice,
                        numeroProposta = entidade.numeroProposta,
                        tipoCondicaoPagamentoPremio_id = entidade.tipoCondicaoPagamentoPremio_id.ToString(),
                        seguradora_id = entidade.seguradora_id.ToString(),
                        sucursal_id = entidade.sucursal_id.ToString(),
                        representante_id = entidade.representante_id.ToString(),
                        regiao_id = entidade.regiao_id.ToString(),
                        estabelecimento_id = entidade.estabelecimento_id.ToString(),
                        DataAtual = DateTime.Now.ToString("o"),
                        seguradora_descr = seguradora_descr,
                        codigo = entidade.codigo,
                        grupoRamoSeguro_id = entidade.grupoRamoSeguro_id.ToString(),
                        comissaoRepresentante = entidade.comissaoRepresentante,
                        comissaoCorretora = entidade.comissaoCorretora,
                        numeroOportunidade = entidade.numeroOportunidade,
                        PropostaApoliceRamoItems = (entidade.PropostaApoliceRamoItems == null) ? null : entidade.PropostaApoliceRamoItems.Select(p => new {p.id,p.NumeroItem,p.premio,p.PropostaApolice_id,p.ramoseguro_descr,p.ramoseguro_id })
                    }
                },JsonRequestBehavior.AllowGet);
                  
            }
        
        
        
        
        public JsonResult ListaStatuPropostaSeguro()
        {
            var data = new SelectList(new TipoStatusPropostaSeguro().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaTipoPropostaSeguro()
        {
            var data = new SelectList(new TipoPropostaSeguro().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaProdutoSeguradora()
        {
            var data = new SelectList(new ProdutoSeguradora().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaRegiao()
        {
            var data = new SelectList(new Regiao().ObterTodos(_estab), "id", "nome");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaSucursal()
        {
            var data = new SelectList(new Sucursal().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListaRepresentante()
        {
            var data = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaRamoSeguro(int id)
        {
            var objs = new RamoSeguro().ObterTodosByGrupo(id).OrderBy(p=> p.identificador);
            var itens = objs.Select(x => new SelectListItem() { Value = x.id.ToString(), Text = x.identificador + "-" + x.descricao });
            return Json(itens, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaComissaoSeguro()
        {
            var data = new SelectList(new ComissaoSeguro().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaTipoCondicaoPagamento()
        {
            var data = new SelectList(new TipoCondicaoPagamentoPremio().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaTipoCondicaoPagamentoPremio()
        {
            var data = new SelectList(new TipoCondicaoPagamentoPremio().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListaTipoSituacaoSinistro()
        {
            var data = new SelectList(new TipoSituacaoSinistro().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListaGrupoRamoSeguro()
        {

            try
            {
                var objs = new GrupoRamoSeguro().ObterTodos();
                var itens = objs.Select(x => new SelectListItem() { Value = x.id.ToString(), Text = x.codigo + "-" + x.descricao });
                return Json(itens, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        //[OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Listas(JqGridRequest request)
        {
            Dictionary<string, string> parameters = HttpContext.Request.QueryString.Keys.Cast<string>()
                .ToDictionary(k => k, v => HttpContext.Request.QueryString[v]);


            int totalRecords = 0;
            var objs = new PropostaApolice().ObterTodos(_paramBase);


            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$seguradoprincipal")))
            {
                var auxstring = parameters["param$seguradoprincipal"];
                var nomePessoa = auxstring.Split(',')[0].Trim();
                var cnpjPessoa = auxstring.Split(',')[1].Trim();

                objs = objs.Where(p => p.Segurado_Principal.nome == nomePessoa).ToList();
                objs = objs.Where(p => p.Segurado_Principal.cnpj == cnpjPessoa).ToList();
            }

            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$seguradora_id")))
            {
                var auxstring = parameters["param$seguradora_id"];
                var nomePessoa = auxstring.Split(',')[0].Trim();
                var cnpjPessoa = auxstring.Split(',')[1].Trim();

                objs = objs.Where(p => p.PessoaSeguradora.nome == nomePessoa).ToList();
                objs = objs.Where(p => p.PessoaSeguradora.cnpj == cnpjPessoa).ToList();
            }

            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$numero")))
            {
                var auxstring = parameters["param$numero"];
                objs = objs.Where(p => p.numeroApolice == auxstring).ToList();
            }


            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$regiao_id")))
            {
                var auxfield = 0;
                if (int.TryParse(parameters["param$regiao_id"].ToString(), out auxfield))
                {
                    objs = objs.Where(p => p.regiao_id == auxfield).ToList();
                }
            }

            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$sucursal_id")))
            {
                var auxfield = 0;
                if (int.TryParse(parameters["param$sucursal_id"].ToString(), out auxfield))
                {
                    objs = objs.Where(p => p.sucursal_id == auxfield).ToList();
                }
            }



            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$dataVigenciaInicio01")))
            {
                var auxfield = new DateTime();
                if (DateTime.TryParse(parameters["param$dataVigenciaInicio01"].ToString(), out auxfield))
                {
                    objs = objs.Where(p => p.dataVigenciaInicio >= auxfield ).ToList();
                }
            }
            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$dataVigenciaInicio02")))
            {
                var auxfield = new DateTime();
                if (DateTime.TryParse(parameters["param$dataVigenciaInicio02"].ToString(), out auxfield))
                {
                    objs = objs.Where(p => p.dataVigenciaInicio <= auxfield).ToList();
                }
            }

            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$dataVigenciaFim01")))
            {
                var auxfield = new DateTime();
                if (DateTime.TryParse(parameters["param$dataVigenciaFim01"].ToString(), out auxfield))
                {
                    objs = objs.Where(p => p.dataVigenciaFim >= auxfield).ToList();
                }
            }
            if (!String.IsNullOrEmpty(ExtraiString(parameters, "param$dataVigenciaFim02")))
            {
                var auxfield = new DateTime();
                if (DateTime.TryParse(parameters["param$dataVigenciaFim02"].ToString(), out auxfield))
                {
                    objs = objs.Where(p => p.dataVigenciaFim <= auxfield).ToList();
                }
            }
            totalRecords = objs.Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            objs = Organiza(request, objs);
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                var grdnumeroproposta = "";
                var grddataproposta = "";
                var grdcliente = "";
                var grdnumeroapolice = "";
                var grddatavigenciaIni = "";
                var grddatavigenciafim = "";
                var grdpremiototal = "";
                var grdseguradora = "";

                if (item.numeroProposta != null)
                    grdnumeroproposta = item.numeroProposta.ToString();
                
                grddataproposta = item.dataProposta.ToShortDateString();


                grdcliente = item.Segurado_Principal.nome;
                grdnumeroapolice = item.numeroApolice;
                if (item.PessoaSeguradora != null)
                    grdseguradora = item.PessoaSeguradora.nome;
                grddatavigenciaIni = item.dataVigenciaInicio.ToShortDateString();
                grddatavigenciafim = item.dataVigenciaFim.ToShortDateString();
                if (item.PropostaApoliceRamoItems != null)
                    grdpremiototal = item.PropostaApoliceRamoItems.Sum(p => p.premio).ToString("n");


                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    grdnumeroproposta,
                    grddataproposta,
                    grdcliente,
                    grdnumeroapolice,
                    grddatavigenciaIni,
                    grddatavigenciafim,
                    grdpremiototal,
                    grdseguradora
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        private static string ExtraiString(Dictionary<string, string> parameters, string key)
        {
            try
            {
                if (parameters.ContainsKey(key))
                    return parameters[key];
                else
                    return null;
            }
            catch
            {
                return null;
            }

        }

        private List<PropostaApolice> Organiza(JqGridRequest request, List<PropostaApolice> objs)
        {
            var quebra = request.SortingName.Split('$');
            

            switch (quebra.Length)
            {
                case 3:
                    //TODO
                    return objs;
                case 2:
                    return new SoftFin.Utils.UtilSoftFin.GenericSorter<PropostaApolice>().Sort(objs.AsQueryable(), quebra[1], request.SortingOrder).ToList();
                default:
                    throw new Exception("Ordenação não implementada");

            }
        }

        public JsonResult ListaSeguradoras()
        {
            var seguradoras = new Pessoa().ObterSeguradoras(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in seguradoras)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}, {1} ", item.nome, item.cnpj) });
            }
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ApoliceArquivo(string codigo, string descricao)
        {

            if (Request.Files.Count == 0)
                return Json(new { CDStatus = "NOK", DSMessage = "Selecione pelo menos 1 arquivo." }, JsonRequestBehavior.AllowGet);

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png", ".xls", ".xlsx" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Extenção não permitida (.doc, .docx, .pdf, .txt, .jpeg, .jpg, .png, .xls, .xlsx)" }, JsonRequestBehavior.AllowGet);
                    }

                    var db = new DbControle();

                    var sistemaArquivo = new SistemaArquivo();

                    if (sistemaArquivo.ObterPorNomeDeArquivo("Proposta", codigo, arquivo.FileName,_paramBase).Count() > 0)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Arquivo já incluido" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

            var pessoa = new Pessoa();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo = arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);

                    AzureStorage.UploadFile(caminhoArquivo,
                                "PropostaApoliceArquivo/" + _estab + "/" + codigo + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DbControle();

                    var sistemaArquivo = new SistemaArquivo();
                    sistemaArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "PropostaApoliceArquivo/" + _estab + "/" + codigo + "/" + nomearquivonovo;
                    sistemaArquivo.arquivoOriginal = nomearquivonovo;
                    sistemaArquivo.rotinaOwner = "Proposta";
                    sistemaArquivo.codigo = codigo;
                    sistemaArquivo.tamanho = arquivo.ContentLength;
                    sistemaArquivo.arquivoExtensao = nomearquivonovo.Substring(nomearquivonovo.IndexOf("."));
                    sistemaArquivo.estabelecimento_id = _estab;
                    sistemaArquivo.Descricao = descricao;
                    sistemaArquivo.Salvar(_paramBase);
                }
            }
            return Json(new { CDStatus = "OK", DSMessage = "Incluido com sucesso " }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ListaApoliceArquivo(string codigo)
        {
            var objs = new SistemaArquivo().ObterOwnerCodigo("Proposta", codigo,_paramBase);

            return Json(
                new {
                    CDStatus = "OK", 
                        ArquivosProposta = 
                        objs.Select(p => new { p.id, 
                                p.estabelecimento_id, 
                                p.arquivoReal,
                                DataInclucao = p.DataInclucao.ToShortDateString(), 
                                p.Descricao,
                                p.arquivoOriginal, 
                                p.arquivoExtensao }) 
                    }, 
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExcluirApoliceArquivo(string codigo,int id)
        {
            try
            {
                var arq = new SistemaArquivo().ObterPorId(id,_paramBase);
                AzureStorage.DeleteFile(arq.arquivoReal, "PropostaApoliceArquivo/"  + _estab + "/" + codigo + "/" + arq.arquivoOriginal,
                            ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                new SistemaArquivo().Excluir(id,_paramBase);

                return ListaApoliceArquivo(codigo);
            }
            catch(Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult EndossoArquivo(string codigo, string descricao)
        {

            if (Request.Files.Count == 0)
                return Json(new { CDStatus = "NOK", DSMessage = "Selecione pelo menos 1 arquivo." }, JsonRequestBehavior.AllowGet);

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png", ".xls", ".xlsx" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Extenção não permitida (.doc, .docx, .pdf, .txt, .jpeg, .jpg, .png, .xls, .xlsx)" }, JsonRequestBehavior.AllowGet);
                    }

                    var db = new DbControle();

                    var sistemaArquivo = new SistemaArquivo();

                    if (sistemaArquivo.ObterPorNomeDeArquivo("Endosso", codigo, arquivo.FileName,_paramBase).Count() > 0)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Arquivo já incluido" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

            var pessoa = new Pessoa();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo = arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);

                    AzureStorage.UploadFile(caminhoArquivo,
                                "Endosso/" + _estab + "/" + codigo + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DbControle();

                    var sistemaArquivo = new SistemaArquivo();
                    sistemaArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "Endosso/" + _estab + "/" + codigo + "/" + nomearquivonovo;
                    sistemaArquivo.arquivoOriginal = nomearquivonovo;
                    sistemaArquivo.rotinaOwner = "Endosso";
                    sistemaArquivo.codigo = codigo;
                    sistemaArquivo.tamanho = arquivo.ContentLength;
                    sistemaArquivo.arquivoExtensao = nomearquivonovo.Substring(nomearquivonovo.IndexOf("."));
                    sistemaArquivo.estabelecimento_id = _estab;
                    sistemaArquivo.Descricao = descricao;
                    sistemaArquivo.Salvar(_paramBase);
                }
            }
            return Json(new { CDStatus = "OK", DSMessage = "Incluido com sucesso " }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ListaEndossoArquivo(string codigo)
        {
            var objs = new SistemaArquivo().ObterOwnerCodigo("Endosso", codigo,_paramBase);

            return Json(
                new
                {
                    CDStatus = "OK",
                    ArquivosProposta =
                    objs.Select(p => new
                    {
                        p.id,
                        p.estabelecimento_id,
                        p.arquivoReal,
                        DataInclucao = p.DataInclucao.ToShortDateString(),
                        p.Descricao,
                        p.arquivoOriginal,
                        p.arquivoExtensao
                    })
                },
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExcluirEndossoArquivo(string codigo, int id)
        {
            try
            {
                var arq = new SistemaArquivo().ObterPorId(id,_paramBase);
                AzureStorage.DeleteFile(arq.arquivoReal, "Endosso/" +  _estab + "/" + codigo + "/" + arq.arquivoOriginal,
                            ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                new SistemaArquivo().Excluir(id,_paramBase);

                return ListaApoliceArquivo(codigo);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SinistroArquivo(string codigo, string descricao)
        {

            if (Request.Files.Count == 0)
                return Json(new { CDStatus = "NOK", DSMessage = "Selecione pelo menos 1 arquivo." }, JsonRequestBehavior.AllowGet);

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png", ".xls", ".xlsx" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Extenção não permitida (.doc, .docx, .pdf, .txt, .jpeg, .jpg, .png, .xls, .xlsx)" }, JsonRequestBehavior.AllowGet);
                    }

                    var db = new DbControle();

                    var sistemaArquivo = new SistemaArquivo();

                    if (sistemaArquivo.ObterPorNomeDeArquivo("Proposta", codigo, arquivo.FileName,_paramBase).Count() > 0)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Arquivo já incluido" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

            var pessoa = new Pessoa();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo = arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);

                    AzureStorage.UploadFile(caminhoArquivo,
                                "Sinistro/" + _estab + "/" + codigo + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DbControle();

                    var sistemaArquivo = new SistemaArquivo();
                    sistemaArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "Sinistro/" + _estab + "/" + codigo + "/" + nomearquivonovo;
                    sistemaArquivo.arquivoOriginal = nomearquivonovo;
                    sistemaArquivo.rotinaOwner = "Sinistro";
                    sistemaArquivo.codigo = codigo;
                    sistemaArquivo.tamanho = arquivo.ContentLength;
                    sistemaArquivo.arquivoExtensao = nomearquivonovo.Substring(nomearquivonovo.IndexOf("."));
                    sistemaArquivo.estabelecimento_id = _estab;
                    sistemaArquivo.Descricao = descricao;
                    sistemaArquivo.Salvar(_paramBase);
                }
            }
            return Json(new { CDStatus = "OK", DSMessage = "Incluido com sucesso " }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult ListaSinistroArquivo(string codigo)
        {
            var objs = new SistemaArquivo().ObterOwnerCodigo("Sinistro", codigo,_paramBase);

            return Json(
                new
                {
                    CDStatus = "OK",
                    ListaArquivosSinistro =
                    objs.Select(p => new
                    {
                        p.id,
                        p.estabelecimento_id,
                        p.arquivoReal,
                        DataInclucao = p.DataInclucao.ToShortDateString(),
                        p.Descricao,
                        p.arquivoOriginal,
                        p.arquivoExtensao
                    })
                },
                JsonRequestBehavior.AllowGet);
        }


        public JsonResult HistoricoSinistro(int id)
        {
            var objs = new SinistroHistorico().ObteTodosPorIdSinistro(id);

            return Json(
                new
                {
                    lista = objs.Select(p => new { p.id, p.historico, p.sinistro_id, DataHora = p.DataHora.ToShortDateString() + " " + p.DataHora.ToShortTimeString(), p.usuario})
                 },
                JsonRequestBehavior.AllowGet);
        }


        
    
    }
}
