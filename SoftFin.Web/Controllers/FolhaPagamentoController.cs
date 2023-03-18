//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using Sistema1.Negocios;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//namespace Sistema1.Controllers
//{
//    public class FolhaPagamentoController : BaseController
//    {
//        public ActionResult Excel()
//        {
//            var obj = new FolhaPagamento();
//            var lista = obj.ObterTodos();
//            CsvExport myExport = new CsvExport();
//            foreach (var item in lista)
//            {
//                myExport.AddRow();
//                myExport["id"] = item.id;
//                myExport["DataPagamento"] = item.DataPagamento;
//                myExport["dataBase"] = item.dataBase;
//                myExport["valor"] = item.valor;
//                myExport["UnidadeNegocio"] = item.UnidadeNegocio.unidade;
//                myExport["FolhaPagamentoTipo"] = item.FolhaPagamentoTipo.descricao;

//            }
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();
//            return File(myCsvData, "application/vnd.ms-excel", "FolhaPagamento.csv");
//        }
//        public ActionResult Index()
//        {
//            CarregaViewData();
//            var obj = new FolhaPagamento();
//            return View(obj);
//        }
//        public ActionResult Listas(JqGridRequest request)
//        {
//            string Valortxtdatabase = Request.QueryString["txtdatabase"]; 

//            int totalRecords = 0;
//            FolhaPagamento obj = new FolhaPagamento();
//            var objs = new FolhaPagamento().ObterTodos();
//            if (!String.IsNullOrEmpty(Valortxtdatabase)) 
//            {
//                objs = objs.Where(p => p.dataBase.ToUpper().Contains(Valortxtdatabase.ToUpper())).ToList();
//            }

//            objs = Organiza(request, objs);
//            totalRecords = objs.Count();
//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
//            foreach (var item in objs)
//            {
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.Funcionario.Pessoa.nome,
//                    item.dataBase,
//                    item.DataPagamento.ToShortDateString(),
//                    item.valor.ToString("n"),
//                    item.UnidadeNegocio.unidade,
//                    item.FolhaPagamentoTipo.descricao
//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }
//        private static List<FolhaPagamento> Organiza(JqGridRequest request, List<FolhaPagamento> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "datapagamento":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.DataPagamento).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.DataPagamento).ToList();
//                   break;
//                case "database":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.dataBase).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.dataBase).ToList();
//                   break;
//                case "valor":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.valor).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.valor).ToList();
//                   break;
//                case "unidadenegocio_id":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.unidadenegocio_id).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.unidadenegocio_id).ToList();
//                   break;
//                case "unidade":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.UnidadeNegocio.unidade).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.UnidadeNegocio.unidade).ToList();
//                   break;
//                case "folhapagamentotipo_id":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.folhapagamentotipo_id).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.folhapagamentotipo_id).ToList();
//                   break;
//                case "folhatipo":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.FolhaPagamentoTipo.descricao).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.FolhaPagamentoTipo.descricao).ToList();
//                   break;
//                case "estabelecimento_id":
//                   if (request.SortingOrder == JqGridSortingOrders.Desc)
//                       objs = objs.OrderByDescending(p => p.estabelecimento_id).ToList();
//                   else
//                       objs = objs.OrderBy(p => p.estabelecimento_id).ToList();
//                   break;

//             }
//             return objs;
//          }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(FolhaPagamento obj)
//        {
//            try
//            {
//            CarregaViewData();
//            int estab = Acesso.EstabLogado();
//            obj.estabelecimento_id = estab;
//            //int idempresa = Acesso.EmpresaLogado();
//            //obj.empresa_id = idempresa;
//            if (ModelState.IsValid)
//            {
//                if (obj.Incluir())
//                {
//                    ViewBag.msg = "Incluído com sucesso";
//                    return View(obj);
//                }
//                else
//                {
//                    ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
//                    return View(obj);
//                }
//            }
//            else
//            {
//                ModelState.AddModelError("", "Dados Invalidos");
//                return View(obj);
//            }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();
//                var obj = new FolhaPagamento();
//                obj.DataPagamento = DateTime.Now;
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(FolhaPagamento obj)
//        {
//            try
//            {
//            CarregaViewData();
//            if (ModelState.IsValid)
//            {
//                int estab = Acesso.EstabLogado();
//                obj.estabelecimento_id = estab;
//                if (obj.Alterar())
//                {
//                    ViewBag.msg = "Alterado com sucesso";
//                    return View(obj);
//                }
//                else
//                {
//                    ViewBag.msg = "Impossivel alterar, registro excluído";
//                    return View(obj);
//                }
//            }
//            else
//            {
//                ModelState.AddModelError("", "Dados Invalidos");
//                return View(obj);
//            }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        public ActionResult Edit(int ID)
//        {
//            try
//            {
//            CarregaViewData();
//            FolhaPagamento obj = new FolhaPagamento();
//            obj = obj.ObterPorId(ID);
//            SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//            return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(FolhaPagamento obj)
//        {
//            try
//            {
//            CarregaViewData();
//            string erro = "";
//            if (obj.Excluir(ref erro))
//            {
//                ViewBag.msg = "Excluido com sucesso";
//                return RedirectToAction("/Index");
//            }
//            else
//            {
//                ViewBag.msg = erro;
//                return View(obj);
//            }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        public ActionResult Delete(int ID)
//        {
//            try
//            {
//            CarregaViewData();
//            FolhaPagamento obj = new FolhaPagamento();
//            obj = obj.ObterPorId(ID);
//            SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//            return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        private void CarregaViewData()
//        {
//            ViewData["funcionario"] = new Funcionario().ObterListaTodos(_paramBase);
//            ViewData["unidadenegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(), "id", "unidade");
//            ViewData["folhapagamentotipo"] = new SelectList(new FolhaPagamentoTipo().ObterTodos(), "id", "descricao");
//        }
//        public ActionResult Detail(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                FolhaPagamento obj = new FolhaPagamento();
//                obj = obj.ObterPorId(ID);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//    }
//}

