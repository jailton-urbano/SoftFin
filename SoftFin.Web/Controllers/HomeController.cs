using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using System.Web.Helpers;
using SoftFin.Web.Classes;
using System.IO;
using System.Xml.Serialization;
using System.Web.UI;

using SoftFin.Web.Negocios;
using SoftFin.Web.Regras;
using System.Net.Mail;
using SoftFin.NFSe.DTO;
using SoftFin.Utils;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace SoftFin.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        string _amarelo = "#F7CA18";
        string _azul = "#4B77BE";
        string _vermelho = "#D91E18";
        string _verde = "#1BBC9B";
        string _lilas = "#b200ff";

        public JsonResult ListaMenu()
        {

        var items = new List<SelectListItem>();
        foreach (var item in SoftFin.Web.Negocios.Acesso.RetornaFuncionalidadesPrincipais(SoftFin.Web.Negocios.Acesso.UsuarioLogado()).Where(x=> x.idPai != null))
        {
            items.Add(new SelectListItem() { Value = item.id.ToString(), Text = item.Descricao });
        }
        var listret = new SelectList(items, "Value", "Text");
        return Json(listret, JsonRequestBehavior.AllowGet);
        }

        public class dtoEventos
        {
            public string title { get; set; }
            public string start { get; set; }
            public string backgroundColor { get; set; }

            public string url { get; set; }
        }

        public class dtoSaldos
        {
            public string banco { get; set; }
            public decimal saldoCredito { get; set; }
            public decimal saldoDedito { get; set; }
            public decimal saldoInicial { get; set; }

            public decimal saldoFinal { get; set; }
        }



        public JsonResult getSaldos()
        {
            var bancos = new Banco().ObterTodos(_paramBase);
            var dtoSaldos = new List<dtoSaldos>();
            foreach (var itemBanco in bancos)
            {
                var dtoSaldo = new dtoSaldos();

                var dataInicial = new DateTime();
                dtoSaldo.saldoInicial = 0;
                var dataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,23,59,00);
                var saldoDiario = new SaldoBancarioReal().ObterUltimoSaldo(itemBanco.id, _paramBase);

                if (saldoDiario == null)
                {
                    dataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 23, 59, 00);
                }
                else
                { 
                    dataInicial = saldoDiario.dataSaldo;
                    dtoSaldo.saldoInicial = saldoDiario.saldoFinal;
                }

                var objCC = new BancoMovimento().ObterTodosDataBanco(dataInicial, dataFinal, itemBanco.id,_paramBase);

                dtoSaldo.banco = itemBanco.nomeBanco + " " + itemBanco.agencia + " " + itemBanco.contaCorrente + "-" + itemBanco.contaCorrenteDigito + " [" + dataInicial.ToShortDateString() + " - " + dataFinal.ToShortDateString() + "]" ;
                dtoSaldo.saldoCredito = objCC.Where(p => p.PlanoDeConta.DebitoCredito == "C").Sum(p => p.valor);
                dtoSaldo.saldoDedito = objCC.Where(p => p.PlanoDeConta.DebitoCredito == "D").Sum(p => p.valor);
                dtoSaldo.saldoFinal = (dtoSaldo.saldoCredito - dtoSaldo.saldoDedito) + dtoSaldo.saldoInicial;
                dtoSaldos.Add(dtoSaldo);
            }

            return Json(dtoSaldos.Select(p => new
            {
                p.banco,
                saldoInicial = p.saldoInicial.ToString("n"),
                carsaldoInicial = (p.saldoInicial >= 0) ? "green": "red",
                carsaldoFinal = (p.saldoFinal >= 0) ? "green" : "red",
                saldoFinal = p.saldoFinal.ToString("n"),
                saldoDedito = p.saldoDedito.ToString("n"),
                saldoCredito = p.saldoCredito.ToString("n"),
            }), JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetCalendar()
        {


            DateTime primeiroDia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-12);
            DateTime ultimoDia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).AddDays(12);
            DateTime ultimoDiaNF = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


            var cpags = new DocumentoPagarParcela().ObterEntreDataVencimento(primeiroDia, ultimoDia, _paramBase);
            var recebs = new ParcelaContrato().ObterEntreData(primeiroDia, ultimoDia, _paramBase);
            var nfs = BuscaNFMunicipal(primeiroDia, ultimoDiaNF);
            var ccs = new BancoMovimento().ObterTodosAConciliar(primeiroDia, ultimoDiaNF, _paramBase);
            var retorno = new List<dtoEventos>();


            var sPP = new SistemaPrincipalPerfil().ObterPorIdPerfil(_paramBase.perfil_id);
            if (sPP.Count() != 0)
            {
                var sP = sPP.Where(p => p.SistemaPrincipal.Codigo == "Calendario").FirstOrDefault();
                //var objs = JArray.Parse(sP.Json);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic objs = serializer.Deserialize<object>(sP.Json);
                //string objs = item["test"];

                //{ "{"contasapagar":true,"faturamento":true,"importacaonfMunicipal":true,"classificacao":true,"processo":true}":true,"faturamento":true,"importacaonfMunicipal":true,"classificacao":true,"processo":true}
                while (primeiroDia <= ultimoDia)
                {
                    if (objs["contasapagar"].ToString() == "True")
                        CalculaCPAG(primeiroDia, cpags, retorno);

                    if (objs["faturamento"].ToString() == "True")
                        CalculaRecebes(primeiroDia, recebs, retorno);

                    if (objs["importacaonfMunicipal"].ToString() == "True")
                        CalculaNFMunicipal(primeiroDia, nfs, retorno);

                    if (objs["classificacao"].ToString() == "True")
                        CalculaCCClassificar(primeiroDia, ccs, retorno);

                    primeiroDia = primeiroDia.AddDays(1);
                }

            }





            return Json(retorno,JsonRequestBehavior.AllowGet);

        }

        public DTORetornoNFEs BuscaNFMunicipal(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                if (string.IsNullOrEmpty(_estabobj.senhaCertificado))
                    return null;

                var uploadPath = Server.MapPath("~/CertTMP/");
                Directory.CreateDirectory(uploadPath);
                var nomearquivonovo = Guid.NewGuid().ToString();
                string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
                var cert = UtilSoftFin.BuscaCert(_estab, _estabobj.senhaCertificado, caminhoArquivo, _estabobj.CNPJ);
                var regra = new SoftFin.NFSe.Business.SFConsultaNFSeRecebidas();
                var result = regra.Execute(new DTONotaFiscal
                {
                    municipio_desc = _estabobj.Municipio.DESC_MUNICIPIO,
                    Cabecalho = new tpCabecalho
                    {
                        CPFCNPJRemetente = new tpCPFCNPJ { CNPJ = UtilSoftFin.Limpastrings(_estabobj.CNPJ) },
                        Inscricao = _estabobj.InscricaoMunicipal.ToString(),
                        dtInicio = dataInicial.ToString("yyyy-MM-dd"),
                        dtFim = dataFinal.ToString("yyyy-MM-dd"),
                    }
                },cert);
                if (result.Cabecalho.Sucesso.ToLower() == "false")
                {
                    return null;
                }
                return result;
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return null;
            }
        }

        private void CalculaNFMunicipal(DateTime primeiroDia, DTORetornoNFEs nfs, List<dtoEventos> retorno)
        {
            if (nfs != null)
            {
                var cpagsAux = nfs.NFe.Where(p => p.DataEmissaoNFe.Substring(0,10) == primeiroDia.ToString("yyyy-MM-dd"));
                if (cpagsAux.Count() > 0)
                {
                    var importados = 0;
                    var pendentes = 0;

                    foreach (var item in cpagsAux)
                    {
                        var nf = new NotaFiscal().ObterPorCodigoVerificacao(_estabobj.Municipio_id, item.ChaveNFe.CodigoVerificacao, _paramBase);

                        if (nf == null)
                            pendentes += 1;
                        else
                            importados += 1;
                    }

                    if (importados > 0)
                    {
                        retorno.Add(
                            new dtoEventos
                            {
                                backgroundColor = _verde,
                                start = primeiroDia.ToString("yyyy-MM-dd"),
                                title = "NF Municipal (" + importados.ToString() + ")",
                                url = "../../NFSeImportacao"
                            });
                    }
                    if (pendentes > 0)
                    {
                        retorno.Add(
                            new dtoEventos
                            {
                                backgroundColor = _azul,
                                start = primeiroDia.ToString("yyyy-MM-dd"),
                                title = "NF Municipal (" + pendentes.ToString() + ")",
                                url = "../../NFSeImportacao"
                            });
                    }
                }
            }
        }

        private void CalculaCPAG(DateTime primeiroDia, List<DocumentoPagarParcela> cpags, List<dtoEventos> retorno)
        {
            var cpagsAux = cpags.Where(p => p.vencimentoPrevisto >= primeiroDia && p.vencimentoPrevisto <= primeiroDia.AddHours(23).AddMinutes(59));
            var cor = _vermelho;
            if (primeiroDia < DateTime.Now)
            {
                var cpagsAuxCount = cpagsAux.Where(p => p.DocumentoPagarMestre.StatusPagamento == 3).Count();
                if (cpagsAuxCount >  0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _verde,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Contas a Pagar (" + cpagsAuxCount.ToString() + ")"
                        ,
                        url = "../../CPAG"
                    });
                }

                cpagsAuxCount = cpagsAux.Where(p => p.DocumentoPagarMestre.StatusPagamento == 2).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _amarelo,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Contas a Pagar (" + cpagsAuxCount.ToString() + ")",
                        url = "../../CPAG"

                    });
                }

                cpagsAuxCount = cpagsAux.Where(p => p.DocumentoPagarMestre.StatusPagamento == 1).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _vermelho,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Contas a Pagar (" + cpagsAuxCount.ToString() + ")",
                        url = "../../CPAG"

                    });
                }

            }
            else
            {
                var cpagsAuxCount = cpagsAux.Where(p => p.DocumentoPagarMestre.StatusPagamento == 3).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _verde,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Contas a Pagar (" + cpagsAuxCount.ToString() + ")",
                        url = "../../CPAG"

                    });
                }

                cpagsAuxCount = cpagsAux.Where(p => p.DocumentoPagarMestre.StatusPagamento == 2).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _amarelo,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Contas a Pagar (" + cpagsAuxCount.ToString() + ")",
                        url = "../../CPAG"

                    });
                }

                cpagsAuxCount = cpagsAux.Where(p => p.DocumentoPagarMestre.StatusPagamento == 1).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _azul,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Contas a Pagar (" + cpagsAuxCount.ToString() + ")",
                        url = "../../CPAG"

                    });
                }
            }
        }
        private void CalculaRecebes(DateTime primeiroDia, List<ParcelaContrato> recebs, List<dtoEventos> retorno)
        {
            var recebesAux = recebs.Where(p => p.data >= primeiroDia && p.data <= primeiroDia.AddHours(23).AddMinutes(59));
            var cor = _vermelho;
            if (primeiroDia < DateTime.Now)
            {
                var cpagsAuxCount = recebesAux.Where(p => p.statusParcela_ID == 1).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _amarelo,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Faturamentos (" + cpagsAuxCount.ToString() + ")",
                        url = "../../Faturar"

                    });
                }

                cpagsAuxCount = recebesAux.Where(p => p.statusParcela_ID == 2).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _verde,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Faturamentos (" + cpagsAuxCount.ToString() + ")",
                        url = "../../Faturar"
                    });
                }

                cpagsAuxCount = recebesAux.Where(p => p.statusParcela_ID == 4).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _vermelho,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Faturamentos (" + cpagsAuxCount.ToString() + ")",
                        url = "../../Faturar"
                    });
                }
            }
            else
            {
                var cpagsAuxCount = recebesAux.Where(p => p.statusParcela_ID == 1).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _amarelo,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Faturamentos (" + cpagsAuxCount.ToString() + ")",
                        url = "../../Faturar"
                    });
                }

                cpagsAuxCount = recebesAux.Where(p => p.statusParcela_ID == 2).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _verde,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Faturamentos (" + cpagsAuxCount.ToString() + ")",
                        url = "../../Faturar"
                    });
                }

                cpagsAuxCount = recebesAux.Where(p => p.statusParcela_ID == 4).Count();
                if (cpagsAuxCount > 0)
                {
                    retorno.Add(
                    new dtoEventos
                    {
                        backgroundColor = _azul,
                        start = primeiroDia.ToString("yyyy-MM-dd"),
                        title = "Faturamentos (" + cpagsAuxCount.ToString() + ")",
                        url = "../../Faturar"
                    });
                }
            }
        }

        private void CalculaCCClassificar(DateTime primeiroDia, List<BancoMovimento> ccs, List<dtoEventos> retorno)
        {
            if (ccs != null)
            {
                var ccsAux = ccs.Where(p => p.data >= primeiroDia && p.data < primeiroDia.AddDays(1));
                if (ccsAux.Count() > 0)
                {
                    var pendentes = ccsAux.Count();

                    retorno.Add(
                        new dtoEventos
                        {
                            backgroundColor = _lilas,
                            start = primeiroDia.ToString("yyyy-MM-dd"),
                            title = "Class. Pendente BM (" + pendentes.ToString() + ")",
                            url = "../../BM/Index?DataInicial=" + primeiroDia.ToString("yyyy-MM-dd") + "&Pesquisa=a classificar"
                        });
                    
                }
            }
        }


        public JsonResult GetParcelaContratos(int id)
        {
            List<object> data = new List<object>();
            DateTime DataInicial = new DateTime();
            DateTime DataFinal = new DateTime();
            var texto = "Mês";
            if (id == 1)
            {
                DataInicial = DateTime.Parse("01" + DateTime.Now.ToString("/MM/yyyy"));
                DataFinal = DataInicial.AddMonths(1);
                DataFinal = DataFinal.AddDays(-1);
            }
            if (id == 2)
            {
                texto = "Semana";
                int numeroMenor = 1, numeroMaior = 7;
                DataInicial = DateTime.Now.AddDays(numeroMenor - DateTime.Now.DayOfWeek.GetHashCode());
                DataFinal = DateTime.Now.AddDays(numeroMaior - DateTime.Now.DayOfWeek.GetHashCode());
            }
            if (id == 3)
            {
                texto = "Dia";
                DataInicial = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00");
                DataFinal = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59");
            }

            //Contratos Parcela
            var parcelas = new ParcelaContrato().ObterEntreData(DataInicial, DataFinal, _paramBase);

            var contaLiberada = parcelas.Where(p => p.statusParcela_ID == 1).Count();
            var contaPendente = parcelas.Where(p => p.statusParcela_ID == 4).Count();
            var contaEmtida = parcelas.Where(p => p.statusParcela_ID == 2).Count();
            var contaCancelada = parcelas.Where(p => p.statusParcela_ID == 5).Count();


            //Notas Fiscais
            var Notas = new NotaFiscal().ObterEntreData(DataInicial, DataFinal, _paramBase);
            var NotasTodas = new NotaFiscal().ObterTodos(_paramBase);
            var Recebimentos = new Recebimento().ObterEntreData(DataInicial, DataFinal, _paramBase).GroupBy(x=> x.notaFiscal_ID);

            var contaEmitidas = Notas.Count();
            var contaRecebidas = Recebimentos.Count();
            var contaNaoRecebidas = NotasTodas.Where(p => p.SituacaoRecebimento == 1).Count();
            var contaRecebidasParcialmente = Notas.Where(p => p.SituacaoRecebimento == 2).Count();

            //Contas a Pagar
            var CP = new DocumentoPagarParcela().ObterEntreData(DataInicial, DataFinal, _paramBase);
            var CPTodos = new DocumentoPagarMestre().ObterTodos(_paramBase);
            var Pagamentos = new Pagamento().ObterEntreData(DataInicial, DataFinal, _paramBase).GroupBy(x => x.DocumentoPagarParcela_ID);

            var contaPagamentos = CP.Count();
            var contaPagos = Pagamentos.Count();
            var contaNaoPagos = CPTodos.Where(p => p.StatusPagamento == 1).Count();
            var contaPagosParcialmente = CPTodos.Where(p => p.StatusPagamento == 2).Count();

            //Caixa
            var caixa = new BancoMovimento().ObterTodosData(DataInicial, DataFinal, _paramBase);
            var contaEntradas = caixa.Where(x => x.TipoMovimento.Codigo == "ENT").Count();
            var contaSaidas = caixa.Where(x => x.TipoMovimento.Codigo == "SAI").Count();


            var pessoas = new Pessoa().Quantidade(_paramBase).ToString();
            var contratos = new Contrato().Quantidade(_paramBase).ToString();
            var conciliacao = "0";
            var nfe = new ParcelaContrato().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.statusParcela_ID == 4).Count();

            var PagamentosEfetuados = new Pagamento().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valorPagamento).ToString("0.00");
            var AReceber = new Recebimento().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valorRecebimento).ToString("0.00");
            var APagar = new DocumentoPagarParcela().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x=> x.valor).ToString("0.00");
            var RecebimentosEmAtraso = new DocumentoPagarMestre().ObterTodos(_paramBase).Where(p => p.situacaoPagamento == "1").Sum(x => x.valorBruto).ToString("0.00");


            return Json(new { 
                Liberada = contaLiberada, 
                Pendente = contaPendente, 
                Emitida = contaEmtida, 
                Cancelada = contaCancelada,
                NotasEmitidas = contaEmitidas, 
                Texto = texto ,
                Recebidas = contaRecebidas,
                NaoRecebidas = contaNaoRecebidas,
                RecebidasParcialmente = contaRecebidasParcialmente,
                Pagamentos = contaPagamentos,
                Pagos = contaPagos,
                NaoPagos = contaNaoPagos,
                PagosParcialmente = contaPagosParcialmente,
                EntradaCaixa = contaEntradas,
                SaidaCaixa = contaSaidas,
                Pessoas = pessoas,
                Contratos = contratos,
                Conciliacao = conciliacao,
                Nfe = nfe,
                AReceber = AReceber,
                APagar = APagar,
                PagamentosEfetuados = PagamentosEfetuados,
                RecebimentosEmAtraso = RecebimentosEmAtraso
            }, JsonRequestBehavior.AllowGet);
        }

        // Página principal do site
        public ActionResult Index()
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            return View();
        }
        public ActionResult IndexNew()
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            return View();
        }
        public ActionResult Sobre()
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            return View();
        }



        [AllowAnonymousAttribute]
        [HttpPost]
        public ActionResult Login(Login login)
        {


            try
            {
                if (login.codigo == "reset" && login.senha == "reset")
                {
                    SoftFin.Utils.UtilSoftFin.CacheSF.Reset();
                }



                string logout = Request.Form["Logout"];
                if (logout != null)
                {
                    System.Web.Security.FormsAuthentication.SignOut();
                    return RedirectToAction("/Index", "Perfil");
                }
                else
                {

                    if (Acesso.isUsuarioValido(login.codigo, login.senha))
                    {

                            int usuario = Acesso.RetornaIdUsuario(login.codigo);
                            var acessos = new UsuarioEstabelecimento().ObterTodosRelacionados(_paramBase).Where(p => p.usuario_id == usuario);

                            if (acessos.Count() == 1)
                            {

                                Acesso.Logar(login.codigo, acessos.First().estabelecimento_id);
                                return RedirectToAction("/Index", "Home");
                            }
                            else
                            {
                                Acesso.Logar(login.codigo);
                                return RedirectToAction("SelecaoEstab", "UsuarioEstabelecimento");
                            }
                    }
                    else
                    {
                        if (login.remember != null)
                        {
                            Acesso.EnviarEmail(login.codigo);
                            ModelState.AddModelError("", "Senha enviado ao seu email.");
                            ViewData["ERRO"] = "Senha enviado ao seu email.";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Usuário ou senha inválido");
                            ViewData["ERRO"] = "Usuário ou senha inválido";
                        }
                    }
                }
                return View();
             }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AllowAnonymousAttribute]
        public ActionResult Login()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            var login = new Login();
            return View(login);
        }

        public ActionResult DatePicker()
        {
            return View();
        }


        public ActionResult Lida(int id)
        {
            DbControle db = new DbControle();
            var obj = db.UsuarioAviso.Find(id);
            obj.lido = true;
            obj.datalido = DateTime.Now;
            db.SaveChanges();

            return View();
        }


    }
}
