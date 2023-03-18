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
//    public class FornecedorAcordoController : BaseController
//    {

//        public override JsonResult TotalizadorDash(int? id)
//        {
//            //base.TotalizadorDash(id);
//            var soma = new FornecedorAcordo().ObterTodos(_paramBase).Sum(p => p.valor).ToString("n");
//            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

//        }

//        public ActionResult Excel()
//        {
//            var obj = new FornecedorAcordo();
//            var lista = obj.ObterTodos(_paramBase);
//            CsvExport myExport = new CsvExport();
//            foreach (var item in lista)
//            {
//                myExport.AddRow();
//                myExport["Data"] = item.Data;
//                myExport["descricao"] = item.descricao;
//                myExport["valor"] = item.valor;
//                myExport["Fornecedor"] = item.Fornecedor.Pessoa.nome;
//            }
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();
//            return File(myCsvData, "application/vnd.ms-excel", "FornecedorAcordo.csv");
//        }
//        public ActionResult Index()
//        {
//            CarregaViewData();
//            var obj = new FornecedorAcordo();
//            return View(obj);
//        }
//        public ActionResult Listas(JqGridRequest request)
//        {
//            string Valornome = Request.QueryString["nome"]; 
//            int totalRecords = 0;
//            FornecedorAcordo obj = new FornecedorAcordo();
//            var objs = new FornecedorAcordo().ObterTodos(_paramBase);
//            if (!String.IsNullOrEmpty(Valornome))
//            {
//                objs = objs.Where(p => p.Fornecedor.Pessoa.nome.ToUpper().Contains(Valornome.ToUpper())).ToList();
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
//                var data = "";
//                if (item.Data != null)
//                    data = item.Data.ToShortDateString();
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.Fornecedor.Pessoa.nome,
//                    item.Fornecedor.Pessoa.cnpj,
//                    data,
//                    item.descricao,
//                    item.valor.ToString("n")

//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }
//        private static List<FornecedorAcordo> Organiza(JqGridRequest request, List<FornecedorAcordo> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "nome":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Fornecedor.Pessoa.nome).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Fornecedor.Pessoa.nome).ToList();
//                    break;
//                case "cpf":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Fornecedor.Pessoa.cnpj).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Fornecedor.Pessoa.cnpj).ToList();
//                    break;
//                case "data":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Data).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Data).ToList();
//                    break;
//                case "valor":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.valor).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.valor).ToList();
//                    break;
//            }
//             return objs;
//          }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(FornecedorAcordo obj)
//        {
//            try
//            {
//            CarregaViewData();
//            //int estab = Acesso.EstabLogado();
//            //obj.estabelecimento_id = estab;
//            //int idempresa = Acesso.EmpresaLogado();
//            //obj.empresa_id = idempresa;
//            if (ModelState.IsValid)
//            {
//                if (obj.Incluir(_paramBase))
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
//            CarregaViewData();
//            var obj = new FornecedorAcordo();
//            obj.Data = DateTime.Now;
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
//        public ActionResult Edit(FornecedorAcordo obj)
//        {
//            try
//            {
//            CarregaViewData();
//            if (ModelState.IsValid)
//            {
//            //int estab = Acesso.EstabLogado();
//            //obj.estabelecimento_id = estab;
//            //int idempresa = Acesso.EmpresaLogado();
//            //obj.empresa_id = idempresa;
//                if (obj.Alterar(_paramBase))
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
//            FornecedorAcordo obj = new FornecedorAcordo();
//            obj = obj.ObterPorId(ID,_paramBase);
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
//        public ActionResult Delete(FornecedorAcordo obj)
//        {
//            try
//            {
//            CarregaViewData();
//            string erro = "";
//            if (obj.Excluir(ref erro,_paramBase))
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
//            FornecedorAcordo obj = new FornecedorAcordo();
//            obj = obj.ObterPorId(ID,_paramBase);
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
//            ViewData["fornecedor"] = new Fornecedor().ObterListaTodos(_paramBase);
//        }
//        public ActionResult Detail(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                FornecedorAcordo obj = new FornecedorAcordo();
//                obj = obj.ObterPorId(ID,_paramBase);
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
