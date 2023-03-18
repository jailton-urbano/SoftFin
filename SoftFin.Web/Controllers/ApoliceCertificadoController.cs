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
//    public class ApoliceCertificadoController : BaseController
//    {
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            var obj = new ApoliceCertificado();

//            var objs = obj.ObterTodos();

//            var totalRecords = objs.Count();

//            var response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
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
//                    item.PremioLiquido.ToString("N2"),
//                    item.Comissao.ToString(),
//                    item.Endosso,
//                    item.Segurado.ToString(),
//                    item.CPF_CNPJ,
//                    item.Atendimento,
//                    item.Login
//                }));
//            }

//            return new JqGridJsonResult() { Data = response };
//        }

//        private static List<ApoliceCertificado> Organiza(JqGridRequest request, List<ApoliceCertificado> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "PremioLiquido":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.PremioLiquido).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.PremioLiquido).ToList();
//                    break;
//                case "Comissao":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Comissao).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Comissao).ToList();
//                    break;
//                case "Endosso":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Endosso).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Endosso).ToList();
//                    break;
//                case "Segurado":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Segurado).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Segurado).ToList();
//                    break;
//                case "CPF_CNPJ":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.CPF_CNPJ).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.CPF_CNPJ).ToList();
//                    break;
//                case "Atendimento":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Atendimento).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Atendimento).ToList();
//                    break;
//                case "Login":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Login).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Login).ToList();
//                    break;
//            }
//            return objs;
//        }

//        public ActionResult Import()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Import(HttpPostedFileBase file)
//        {
//            int estab = _paramBase.estab_id;

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

//                new ApoliceCertificado().ImportaExcel(path);

//                ViewBag.msg = "Importação Finalizada com sucesso";
//            }


//            return View();
//        }

//        [HttpPost]
//        public ActionResult Create(ApoliceCertificado obj)
//        {
//            try
//            {
//                CarregaViewData();

//                if (ModelState.IsValid)
//                {
//                    var modelo = new ApoliceCertificado();
//                    int idempresa = _paramBase.estab_id;
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
            
//            ViewData["ApoliceCertificado"] = new SelectList(new ApoliceCertificado().ObterTodos(), "id", "Descricao");
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();

//                var obj = new ApoliceCertificado();
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Delete(ApoliceCertificado obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";
//                int idempresa = _paramBase.estab_id;
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
//                var obj = new ApoliceCertificado(); 
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
//        public ActionResult Edit(ApoliceCertificado obj)
//        {
//            try
//            {
//                CarregaViewData();
//                var codigo = new ApoliceCertificado();
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
//                var obj = new ApoliceCertificado();
//                obj = obj.ObterPorId(ID);
//                if (obj == null)
//                    throw new Exception("Acesso Negado.");
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
