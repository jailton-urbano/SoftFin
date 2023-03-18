//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using Sistema1.Negocios;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Sistema1.Controllers
//{
//    public class ConciliacaoBancariaController : BaseController
//    {

//        public ActionResult Excel()
//        {
//            var obj = new BancoMovimento();
//            var lista = obj.ObterTodos();
//            CsvExport myExport = new CsvExport();
//            foreach (var item in lista)
//            {
//                myExport.AddRow();


//                myExport["planoDeConta_id"] = item.planoDeConta_id;
//                myExport["PlanoDeConta"] = item.PlanoDeConta;
//                myExport["origemmovimento_id"] = item.origemmovimento_id;
//                myExport["OrigemMovimento"] = item.OrigemMovimento;
//                myExport["tipoDeMovimento_id"] = item.tipoDeMovimento_id;
//                myExport["TipoMovimento"] = item.TipoMovimento;
//                myExport["tipoDeDocumento_id"] = item.tipoDeDocumento_id;
//                myExport["TipoDocumento"] = item.TipoDocumento;

//                myExport["data"] = item.data;
//                myExport["historico"] = item.historico;
//                myExport["valor"] = item.valor;
//                myExport["notafiscal_id"] = item.notafiscal_id;
//                myExport["NotaFiscal"] = item.NotaFiscal;

//                myExport["LanctoExtrato_id"] = item.LanctoExtrato_id;
//                myExport["LanctoExtrato"] = item.LanctoExtrato;
//            }
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();
//            return File(myCsvData, "application/vnd.ms-excel", "BancoMovimento.csv");
//        }

//        public ActionResult Index()
//        {
//            ViewData["banco"] = new Banco().CarregaBanco();
//            return View(new LanctoExtratoView());
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            var obj = new LanctoExtrato();

//            var objs = obj.ObterTodos();

//            string ValordataIni = Request.QueryString["dataIni"];
//            string ValordataFim = Request.QueryString["dataFim"];
//            string ValoridLancto = Request.QueryString["idLancto"];
//            string Valordescricao = Request.QueryString["descricao"];
//            string ValorTipo = Request.QueryString["Tipo"];
//            string ValorValorIni = Request.QueryString["ValorIni"];
//            string ValorValorFim = Request.QueryString["ValorFim"];
//            string Valorbanco_id = Request.QueryString["banco_id"];
//            int totalRecords = 0;


//            if (!String.IsNullOrEmpty(ValordataIni))
//            {
//                DateTime aux;
//                DateTime.TryParse(ValordataIni, out aux);
//                objs = objs.Where(p => p.data >= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValordataFim))
//            {
//                DateTime aux;
//                DateTime.TryParse(ValordataFim, out aux);
//                objs = objs.Where(p => p.data <= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValoridLancto))
//            {
//                objs = objs.Where(p => p.idLancto.Contains(ValoridLancto)).ToList();
//            }
//            if (!String.IsNullOrEmpty(Valordescricao))
//            {
//                objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorTipo))
//            {
//                objs = objs.Where(p => p.Tipo.Contains(ValorTipo)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorValorIni))
//            {
//                decimal aux;
//                decimal.TryParse(ValorValorIni, out aux);
//                objs = objs.Where(p => p.Valor >= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorValorFim))
//            {
//                decimal aux;
//                decimal.TryParse(ValorValorFim, out aux);
//                objs = objs.Where(p => p.Valor == aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(Valorbanco_id))
//            {
//                int aux;
//                int.TryParse(Valorbanco_id, out aux);
//                objs = objs.Where(p => p.banco_id == aux).ToList();
//            }


//            objs = Organiza(request, objs);
//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

//            foreach (LanctoExtrato item in
//                objs)
//            {
//                string conc = (item.Conciliado) ? "SIM": "NÃO";
//                string tipo = (item.Tipo == "C") ? "Crédito" : "Débito";
//                string dat = (item.DataConciliado == null) ? "" : item.DataConciliado.Value.ToShortDateString();
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.id,
//                    item.data.ToShortDateString(),
//                    item.idLancto,
//                    item.descricao,
//                    tipo,
//                    item.Valor.ToString("N"),
//                    item.banco.nomeBanco,
//                    conc,
//                    dat,
//                    item.UsuConciliado

//                }));
//            }
//            //'Codigo', 'Data', 'Identificação', 'Descrição', 'Tipo', 'Valor', 'Banco', 'Conc.', 'Data Conc.', 'Usu. Conc.'
//            return new JqGridJsonResult() { Data = response };
//        }

 

//        private static List<LanctoExtrato> Organiza(JqGridRequest request, List<LanctoExtrato> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "data":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.data).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.data).ToList();
//                    break;
//                case "idLancto":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.idLancto).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.idLancto).ToList();
//                    break;
//                case "descricao":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.descricao).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.descricao).ToList();
//                    break;
//                case "Tipo":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Tipo).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Tipo).ToList();
//                    break;
//                case "Valor":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Valor).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Valor).ToList();
//                    break;
//                case "Banco":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.banco.nomeBanco).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.banco.nomeBanco).ToList();
//                    break;
//                case "Conc":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Conciliado).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Conciliado).ToList();
//                    break;
//            }
//            return objs;
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas2(JqGridRequest request)
//        {

//            string Valorbanco_id = Request.QueryString["banco_id"];
//            string ValorDataIni = Request.QueryString["DataIni"];
//            string ValorDataFim = Request.QueryString["DataFim"];
//            string ValorValorIni = Request.QueryString["ValorIni"];
//            string ValorValorFim = Request.QueryString["ValorFim"];
//            string ValorNumeroCPAG = Request.QueryString["NumeroCPAG"];
//            string ValorNumeroNFES = Request.QueryString["NumeroNFES"]; 

//            var objsTemp = new BancoMovimento().ObterTodos().OrderBy(p => p.valor).ThenBy(p => p.data);
//            var objs = new List<BancoMovimentoConciliacaoVLW>();

//            foreach (var item in objsTemp)
//            {
//                var itemAux = new BancoMovimentoConciliacaoVLW();
                
//                if (item.DocumentoPagarMestre_id != null)
//                {
//                    if (objs.Where(p => p.DocumentoPagarMestre_id == item.DocumentoPagarMestre_id).FirstOrDefault() == null)
//                    {
//                        itemAux.banco = item.Banco.nomeBanco + " - " + item.Banco.agencia + " - " + item.Banco.contaCorrente;
//                        itemAux.banco_id = item.banco_id;
//                        itemAux.Data = item.data;
//                        itemAux.Valor = item.DocumentoPagarMestre.valorBruto;
//                        itemAux.DocumentoPagarMestre_id = item.DocumentoPagarMestre_id;
//                        itemAux.NumeroCPAG = item.DocumentoPagarMestre.numeroDocumento.ToString();
//                        itemAux.id = "CPAG" + itemAux.DocumentoPagarMestre_id.ToString();
//                    }

//                }
//                else if (item.notafiscal_id != null)
//                {
//                    if (objs.Where(p => p.notafiscal_id == item.notafiscal_id).FirstOrDefault() == null)
//                    {
//                        itemAux.banco = item.Banco.nomeBanco + " - " + item.Banco.agencia + " - " + item.Banco.contaCorrente;
//                        itemAux.banco_id = item.banco_id; 
//                        itemAux.Data = item.data;
//                        itemAux.Valor = item.DocumentoPagarMestre.valorBruto;
//                        itemAux.notafiscal_id = item.notafiscal_id;
//                        itemAux.NumeroNFES = item.notafiscal_id.ToString();
//                        itemAux.id = "NFES" + itemAux.NumeroNFES;
//                    }

//                }
//                else
//                {
//                    itemAux.banco = item.Banco.nomeBanco + " - " + item.Banco.agencia + " - " + item.Banco.contaCorrente;
//                    itemAux.banco_id = item.banco_id; 
//                    itemAux.Data = item.data;
//                    itemAux.Valor = item.valor;
//                    itemAux.id = "CONC" + item.id.ToString();
//                }
//                objs.Add(itemAux);
//            }



//            if (!String.IsNullOrEmpty(Valorbanco_id))
//            {
//                int aux;
//                int.TryParse(Valorbanco_id, out aux);
//                objs = objs.Where(p => p.banco_id == aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorDataIni))
//            {
//                DateTime aux;
//                DateTime.TryParse(ValorDataIni, out aux);
//                objs = objs.Where(p => p.Data >= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorDataFim))
//            {
//                DateTime aux;
//                DateTime.TryParse(ValorDataFim, out aux);
//                objs = objs.Where(p => p.Data <= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorValorIni))
//            {
//                decimal aux;
//                decimal.TryParse(ValorValorIni, out aux);
//                objs = objs.Where(p => p.Valor >= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorValorFim))
//            {
//                decimal aux;
//                decimal.TryParse(ValorValorFim, out aux);
//                objs = objs.Where(p => p.Valor <= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorNumeroCPAG))
//            {
//                objs = objs.Where(p => p.NumeroCPAG.Contains(ValorNumeroCPAG)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorNumeroNFES))
//            {
//                objs = objs.Where(p => p.NumeroNFES.Contains(ValorNumeroNFES)).ToList();
//            }


//            int totalRecords = objs.Count();

//            //Fix for grouping, because it adds column name instead of index to SortingName
//            //string sortingName = "cdg_perfil";

//            //Prepare JqGridData instance
//            JqGridResponse response = new JqGridResponse()
//            {
//                //Total pages count
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                //Page number
//                PageIndex = request.PageIndex,
//                //Total records count
//                TotalRecordsCount = totalRecords
//            };

//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

//            //objs = Organiza2(request, objs);

//            //Table with rows data
//            foreach (var item in
//                objs)
//            {
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.Data.Value.ToShortDateString(),
//                    item.Valor.Value.ToString("n"),
//                    item.banco,
//                    item.NumeroCPAG,
//                    item.NumeroNFES,
//                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }



//        private static List<BancoMovimento> Organiza2(JqGridRequest request, List<BancoMovimento> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "Data":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.data).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.data).ToList();
//                    break;
//                case "Historico":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.historico).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.historico).ToList();
//                    break;
//                case "PlanoDeConta":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.PlanoDeConta.descricao).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.PlanoDeConta.descricao).ToList();
//                    break;
//                case "Valor":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.valor).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.valor).ToList();
//                    break;
//                case "Banco":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Banco.nomeBanco).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Banco.nomeBanco).ToList();
//                    break;
//                //case "Unidade":
//                //    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                //        objs = objs.OrderByDescending(p => p.UnidadeNegocio.unidade).ToList();
//                //    else
//                //        objs = objs.OrderBy(p => p.UnidadeNegocio.unidade).ToList();
//                //    break;
//                case "Origen":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.OrigemMovimento.Modulo).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.OrigemMovimento.Modulo).ToList();
//                    break;
//                case "Tipo":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.TipoDocumento.descricao).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.TipoDocumento.descricao).ToList();
//                    break;
//                case "NFEs":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.NotaFiscal.numeroNfse).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.NotaFiscal.numeroNfse).ToList();
//                    break;
//                case "CPAG":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.DocumentoPagarMestre.codigoPagamento).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.DocumentoPagarMestre.codigoPagamento).ToList();
//                    break;


//            }
//            return objs;
//        }




//        public ActionResult Import()
//        {
//            return View();
//        }



//        [HttpPost]
//        public ActionResult Create(LanctoExtrato obj)
//        {
//            try
//            {
//                CarregaViewData();

//                if (ModelState.IsValid)
//                {
//                    var modelo = new LanctoExtrato();
//                    int idempresa = Acesso.EmpresaLogado();
//                    if (modelo.Incluir(obj) == true)
//                    {
//                        ViewBag.msg = "Tipo Documento incluído com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Tipo Documento já cadastrado";
//                        return View(obj);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Dados Invalidos");
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }

//        private void CarregaViewData()
//        {
//            ViewData["banco"] = new Banco().CarregaBanco();
//            //ViewData["ConciliacaoBancaria"] = new SelectList(new ConciliacaoBancaria().ObterTodos(), "id", "Descricao");
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();

//                var obj = new LanctoExtrato();
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }

//        [HttpPost]
//        public ActionResult Conciliar(int id, string txtBancoMovimento)
//        {
//            try
//            {
//                CarregaViewData();
//                var lanctoExtrato = new LanctoExtrato();
//                lanctoExtrato.Conciliar(id, txtBancoMovimento);

//                ViewBag.msg = "Documento conciliado com sucesso";
//                return RedirectToAction("/Index");
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        public ActionResult Conciliar(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                var banco = new DbControle();
//                var obj = new LanctoExtrato();
//                obj = obj.ObterPorId(ID);

//                ViewData["DataIni"] = obj.data.AddDays(-1).ToShortDateString();
//                ViewData["DataFim"] = obj.data.AddDays(1).ToShortDateString();
//                ViewData["ValorIni"] = obj.Valor.ToString("n");
//                ViewData["ValorFim"] = obj.Valor.ToString("n");

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }


//        }
//    }
//}
