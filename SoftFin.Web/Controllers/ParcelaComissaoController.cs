//using Lib.Web.Mvc.JQuery.JqGrid;
//using SoftFin.Web.Classes;
//using SoftFin.Web.Models;
//using SoftFin.Web.Negocios;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace SoftFin.Web.Controllers
//{
//    public class ParcelaComissaoController : BaseController
//    {
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            var obj = new ParcelaComissao();

//            var objs = obj.ObterTodos();

//            var totalRecords = objs.Count();

//            var response = new JqGridResponse()
//            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };

//            objs = Organiza(request, objs);
    
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

//            foreach (var item in
//                objs)
//            {

                
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.id,
//                    item.en1.ToString(),
//                    item.en2.ToString(),
//                    item.en3.ToString(),
//                    item.seguradora,
//                    item.familia,
//                    item.produto,
//                    item.tipo,
//                    item.extrato,
//                    item.data_extrato.Value.ToString("dd/MM/yyyy"),
//                    item.data_credito.Value.ToString("dd/MM/yyyy"),
//                    item.parcela.ToString(),
//                    item.comissao_bruta.ToString("N2")
//                }));
//            }

//            return new JqGridJsonResult() { Data = response };
//        }

//        private static List<ParcelaComissao> Organiza(JqGridRequest request, List<ParcelaComissao> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "EN1":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.en1).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.en1).ToList();
//                    break;
//                case "EN2":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.en2).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.en2).ToList();
//                    break;
//                case "EN3":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.en3).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.en3).ToList();
//                    break;
//                case "Seguradora":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.seguradora).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.seguradora).ToList();
//                    break;
//                case "Familia":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.familia).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.familia).ToList();
//                    break;
//                case "Produto":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.produto).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.produto).ToList();
//                    break;
//                case "Tipo":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.tipo).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.tipo).ToList();
//                    break;
//                case "Extrato":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.extrato).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.extrato).ToList();
//                    break;
//                case "DataExtrato":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.data_extrato).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.data_extrato).ToList();
//                    break;
//                case "DataCredito":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.data_credito).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.data_credito).ToList();
//                    break;
//                case "Parcela":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.parcela).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.parcela).ToList();
//                    break;
//                case "ComissaoBruta":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.comissao_bruta).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.comissao_bruta).ToList();
//                    break;

//            }
//            return objs;
//        }



//        [HttpPost]
//        public ActionResult Create(ParcelaComissao obj)
//        {
//            try
//            {
//                CarregaViewData();

//                if (ModelState.IsValid)
//                {
//                    var modelo = new ParcelaComissao();
//                    int idempresa = pb.estab_id;
//                    obj.estabelecimento_id = idempresa;
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
            
//            ViewData["ParcelaComissao"] = new SelectList(new ParcelaComissao().ObterTodos(), "id", "Descricao");
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();

//                var obj = new ParcelaComissao();
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Delete(ParcelaComissao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";
//                int idempresa = pb.estab_id;
//                obj.estabelecimento_id = idempresa;
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
//                CarregaViewData();
//                var obj = new ParcelaComissao();
//                var cs = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(cs);
//                return View(cs);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Edit(ParcelaComissao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                var codigo = new ParcelaComissao();
//                codigo.Alterar(obj);

//                ViewBag.msg = "Tipo Documento alterado com sucesso";
//                return RedirectToAction("/Index");
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
//                CarregaViewData();
//                var banco = new DbControle();
//                var obj = new ParcelaComissao();
//                obj = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }


//        public ActionResult Import()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Import(HttpPostedFileBase file)
//        {
//            int estab = pb.estab_id;

//            var arquivo = Request.Files[0];

//            if (arquivo != null && arquivo.ContentLength > 0)
//            {
//                var fileName = Path.GetFileName(arquivo.FileName);


//                if (fileName.ToUpper().IndexOf(".XLS") == -1)
//                {
//                    ViewBag.msg = "Arquivo invalido.";
//                    return View();
//                }

//                var path = Path.Combine(Server.MapPath("~/TXTTemp/"), fileName);
//                arquivo.SaveAs(path);

//                new ParcelaComissao().ImportaExcel(path);

//                ViewBag.msg = "Importação Finalizada com sucesso";
//            }


//            return View();
//        }
//    }
//}
