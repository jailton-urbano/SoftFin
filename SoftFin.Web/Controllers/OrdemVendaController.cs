//using Lib.Web.Mvc.JQuery.JqGrid;
//using SoftFin.Web.Classes;
//using SoftFin.Web.Models;
//using SoftFin.Web.Negocios;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace SoftFin.Web.Controllers
//{
//    public class OrdemVendaController : BaseController
//    {


//        [HttpPost]
//        public override JsonResult TotalizadorDash(int? id)
//        {
//            base.TotalizadorDash(id);
//            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal).Sum(x => x.valor).ToString("n");
//            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

//        }

//        public ActionResult Excel()
//        {
//            var obj = new OrdemVenda();
//            var lista = obj.ObterTodos();
//            CsvExport myExport = new CsvExport();
//            foreach (var item in lista)
//            {
//                myExport.AddRow();
//                myExport["id"] = item.id;
//                myExport["descricao"] = item.descricao;
//                myExport["valor"] = item.valor;
//                myExport["data"] = item.data;
//                myExport["parcelaContrato_ID"] = item.parcelaContrato_ID;
//                myExport["statusParcela_ID"] = item.statusParcela_ID;
//                myExport["unidadeNegocio_ID"] = item.unidadeNegocio_ID;
//                myExport["ParcelaContrato"] = item.ParcelaContrato;
//                myExport["statusParcela"] = item.statusParcela;
//                myExport["UnidadeNegocio"] = item.UnidadeNegocio;
//                myExport["pessoas_ID"] = item.pessoas_ID;
//                myExport["Pessoa"] = item.Pessoa;
//                myExport["estabelecimento_id"] = item.estabelecimento_id;
//                myExport["Estabelecimento"] = item.Estabelecimento;
//                myExport["usuarioAutorizador_id"] = item.usuarioAutorizador_id;
//                myExport["usuarioAutorizador"] = item.usuarioAutorizador;
//                myExport["dataAutorizacao"] = item.dataAutorizacao;
//            }
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();
//            return File(myCsvData, "application/vnd.ms-excel", "OrdemVenda.csv");
//        }
//        public ActionResult Index()
//        {
//            CarregaViewData(false);
//            var obj = new OrdemVenda();
//            return View(obj);
//        }

//        private void CarregaViewData(bool ispendente)
//        {
//            var status = new StatusParcela();
//            var lista = new List<StatusParcela>();
//            if (ispendente)
//                lista = status.ObterTodos().Where(p => p.status == "Pendente").ToList();
//            else
//                lista = status.ObterTodos().ToList();

//            ViewData["parcelaContrato"] = new SelectList(new ParcelaContrato().ObterTodos(), "id", "descricao");
//            ViewData["statusParcela"] = new SelectList(lista, "id", "status");
//            ViewData["unidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(), "id", "unidade");
//            ViewData["pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
//            ViewData["item"] = new SelectList(new ItemProdutoServico().ObterTodos(), "id", "descricao");
//            ViewData["tabelaPreco"] = new SelectList(new TabelaPrecoItemProdutoServico().ObterTodos(), "id", "descricao");
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            string Valorpessoa_id = Request.QueryString["pessoas_ID"];
//            string ValorNumero = Request.QueryString["Numero"];
//            string ValordataInicial = Request.QueryString["dataInicial"];
//            string ValordataFinal = Request.QueryString["dataFinal"];

//            var obj = new OrdemVenda();
//            var objs = obj.ObterTodos();

//            if (!String.IsNullOrEmpty(Valorpessoa_id))
//            {
//                int aux;
//                int.TryParse(Valorpessoa_id, out aux);
//                objs = objs.Where(p => p.pessoas_ID == aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorNumero))
//            {
//                int aux;
//                int.TryParse(ValorNumero, out aux);
//                objs = objs.Where(p => p.Numero == aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValordataInicial))
//            {
//                DateTime aux;
//                DateTime.TryParse(ValordataInicial, out aux);
//                objs = objs.Where(p => p.data >= aux).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValordataFinal))
//            {
//                DateTime aux;
//                DateTime.TryParse(ValordataFinal, out aux);
//                objs = objs.Where(p => p.data <= aux).ToList();
//            }


//            //lista = obj.listaCategoriaPessoa()();

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
//            objs = Organiza(request, objs);
//            objs = objs.OrderBy(x=> x.data).Skip(12 * request.PageIndex).Take(12).ToList();

//            foreach (var item in
//                objs)
//            {
//                string statuslib = "";
//                string statusparc = "";
//                string usuario = "";
//                string data = "";
//                string unidade = "";
//                string pessoa = "";
//                if (item.usuarioAutorizador != null)
//                {
//                    statuslib = "Autorizado";
//                    usuario = item.usuarioAutorizador.nome;
//                    data = item.dataAutorizacao.Value.ToString("dd/MM/yyyy");
//                }
//                else
//                {
//                    statuslib = "Pendente Autorização";
//                }

//                if (item.statusParcela != null)
//                {
//                    statusparc = item.statusParcela.status;
//                }

//                if (item.UnidadeNegocio != null)
//                {
//                    unidade = item.UnidadeNegocio.unidade;
//                }
//                if (item.Pessoa != null)
//                {
//                    pessoa = item.Pessoa.nome;
//                }                
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.Numero.ToString(),
//                    item.data.ToString("dd/MM/yyyy"),
//                    item.descricao,
//                    pessoa,
//                    unidade,
//                    item.valor,
//                    statusparc,
//                    statuslib,
//                    usuario,
//                    data
//                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }


//        private static List<OrdemVenda> Organiza(JqGridRequest request, List<OrdemVenda> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "numero":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Numero).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Numero).ToList();
//                    break;
//                case "Data":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.data).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.data).ToList();
//                    break;
//                case "Cliente":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Pessoa.nome).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Pessoa.nome).ToList();
//                    break;
//                case "Unidade":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.UnidadeNegocio.unidade).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.UnidadeNegocio.unidade).ToList();
//                    break;
//                case "Valor":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.valor).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.valor).ToList();
//                    break;
//                case "Situacao":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.statusParcela).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.statusParcela).ToList();
//                    break;
//                case "Statuios":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.usuarioAutorizador).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.usuarioAutorizador).ToList();
//                    break;

//                case "Usuar":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.usuarioAutorizador.nome).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.usuarioAutorizador.nome).ToList();
//                    break;
//            }
//            return objs;
//        }


//        [HttpPost]
//        public ActionResult Create(OrdemVenda obj)
//        {
//            try
//            {
//                CarregaViewData(true);
//                if (ModelState.IsValid)
//                {
//                    OrdemVenda tp = new OrdemVenda();
//                    int estab = pb.estab_id;
//                    obj.estabelecimento_id = estab;
//                    obj.parcelaContrato_ID = null;
//                    int ov = 0;
//                    if (tp.Incluir(obj,ref ov) == true)
//                    {
//                        TabelaPrecoItemProdutoServico tabela = new TabelaPrecoItemProdutoServico();
//                        ViewData["tabela"] = tabela.ObterTodos().Where(x => x.id == obj.tabelaPreco_ID).FirstOrDefault().descricao;
//                        ViewBag.msg = "Ordem Venda número " + ov.ToString() +  " incluído com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Ordem Venda já cadastrado";
//                        return View(obj);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);

//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData(true);
//                ViewData["tabela"] = "";
//                var obj = new OrdemVenda();
//                obj.data = DateTime.Now;
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        [HttpPost]
//        public ActionResult Edit(OrdemVenda obj)
//        {
//            try
//            {
//                CarregaViewData(true);
//                if (ModelState.IsValid)
//                {
//                    OrdemVenda tp = new OrdemVenda();
//                    int estab = pb.estab_id;
//                    obj.estabelecimento_id = estab;
//                    tp.Alterar(obj);
//                    ViewBag.msg = "Ordem Venda alterado com sucesso";
//                    return View(obj);

//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);

//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Edit(int ID)
//        {
//            try
//            {
                
//                CarregaViewData(true);
//                OrdemVenda obj = new OrdemVenda().ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//                var status = new StatusParcela();
//                if (obj.ParcelaContrato != null)
//                    return RedirectToAction("Detail", new { id=ID});

//                obj.statusParcela_ID = status.ObterTodos().Where(p => p.status == "Pendente").FirstOrDefault().id;
//                obj.statusParcela = status.ObterTodos().Where(p => p.status == "Pendente").FirstOrDefault();

                

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
        
//        [HttpPost]
//        public ActionResult Delete(OrdemVenda obj)
//        {

//            try
//            {
//                CarregaViewData(false);
//                string Erro = "";
//                if (!obj.Excluir(obj.id, ref Erro))
//                {
//                    ViewBag.msg = Erro;
//                    obj = obj.ObterPorId(obj.id);
//                    return View(obj);
//                }
//                else
//                {
//                    return RedirectToAction("/Index");
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        public ActionResult Delete(int ID)
//        {
//            try
//            {
//                CarregaViewData(false);
//                OrdemVenda obj = new OrdemVenda();
//                OrdemVenda pessoa = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(pessoa);
//                return View(pessoa);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        public ActionResult Detail(int ID)
//        {
//            try
//            {
//                CarregaViewData(false);
//                OrdemVenda obj = new OrdemVenda();
//                OrdemVenda pessoa = obj.ObterPorId(ID);
//                return View(pessoa);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        [HttpPost]
//        public JsonResult atualizaPreco(int item, int tabela)
//        {
//            precoCalculado preco = new precoCalculado();

//            var banco = new DbControle();
//            var user = Acesso.UsuarioLogado();
//            int estab = pb.estab_id;

//            PrecoItemProdutoServico precoItem = new PrecoItemProdutoServico();
//            preco.valor = precoItem.ObterTodos().Where(x => x.ItemProdutoServico_ID == item && x.TabelaPrecoItemProdutoServico_ID == tabela).FirstOrDefault().preco;

//            var data = Newtonsoft.Json.JsonConvert.SerializeObject(preco);

//            return Json(data, JsonRequestBehavior.AllowGet);
//        }


//        public class precoCalculado {
//            public Decimal valor;
//        }

//        [HttpGet]
//        public JsonResult getList(int item_id)
//        {
//            PrecoItemProdutoServico precoItem = new PrecoItemProdutoServico();
//            var retornobjs = precoItem.ObterTodos().Where(p => p.ItemProdutoServico_ID == item_id).ToList();
//            return Json(retornobjs.Select(p => new { id = p.id, descricao = p.TabelaPrecoItemProdutoServico.descricao, preco = p.preco.ToString("n"), tabela_id = p.TabelaPrecoItemProdutoServico_ID }), JsonRequestBehavior.AllowGet);
//        }    
        
        
//    }
//}
