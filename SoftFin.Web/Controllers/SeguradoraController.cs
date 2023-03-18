//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Sistema1.Controllers
//{
//    public class SeguradoraController : BaseController
//    {

//        public ActionResult Index()
//        {
//            return View();
//        }


//        private static string ExtraiString(Dictionary<string, string> parameters, string key)
//        {
//            try
//            {
//                return parameters[key];
//            }
//            catch
//            {
//                return null;
//            }

//        }

//        private List<Seguradora> Organiza(JqGridRequest request, List<Seguradora> objs)
//        {
//            var quebra = request.SortingName.Split('$');


//            switch (quebra.Length)
//            {
//                case 3:
//                    //TODO
//                    return objs;
//                case 2:
//                    return new GenericSorter<Seguradora>().Sort(objs.AsQueryable(), quebra[1], request.SortingOrder).ToList();
//                default:
//                    throw new Exception("Ordenação não implementada");

//            }
//        }

//        private static string ExtraiString(Dictionary<string, string> parameters, string key)
//        {
//            try
//            {
//                return parameters[key];
//            }
//            catch
//            {
//                return null;
//            }

//        }

//        private List<Seguradora> Organiza(JqGridRequest request, List<Seguradora> objs)
//        {

//            return new GenericSorter<Seguradora>().Sort(objs.AsQueryable(), request.SortingName, request.SortingOrder).ToList();
//        }


//        [OutputCache(Duration = 0, VaryByParam = "none")]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            Dictionary<string, string> parameters = HttpContext.Request.QueryString.Keys.Cast<string>()
//                .ToDictionary(k => k, v => HttpContext.Request.QueryString[v]);

//            int totalRecords = 0;
//            var objs = new Seguradora().ObterTodos();

//            if (!String.IsNullOrEmpty(ExtraiString(parameters, "descr")))
//            {
//                var auxstring = parameters["descricao"];
//                objs = objs.Where(p => p.descricao == auxstring).ToList();
//            }

//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };

//            objs = Organiza(request, objs);
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
//            foreach (var item in objs)
//            {
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.descricao
//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }

//        [HttpPost]
//        public JsonResult Salvar(Seguradora entidade)
//        {
//            try
//            {
//                var validacao = entidade.Validar(ModelState);
//                if (validacao.Count() > 0)
//                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

//                if (entidade.ObterPorId(entidade.id) == null)
//                {
//                    entidade.Incluir();
//                }
//                else
//                {
//                    entidade.Alterar(entidade);
//                }
//                return Json(new
//                {
//                    CDStatus = "OK",
//                    apolice = (entidade == null) ? null : new
//                    {
//                        descricao = entidade.descricao
//                    }
//                }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        [HttpPost]
//        public JsonResult Excluir(Seguradora entidade)
//        {
//            try
//            {
//                if (entidade.ObterPorId(entidade.id) != null)
//                {
//                    string auxErro = "";
//                    if (!entidade.Excluir(entidade.id, ref auxErro))
//                    {
//                        return Json(new { CDStatus = "NOK", Exception = auxErro }, JsonRequestBehavior.AllowGet);
//                    }
//                }
//                else
//                {
//                    return Json(new { CDStatus = "NOK", Exception = "Registro não encontrado" }, JsonRequestBehavior.AllowGet);
//                }

//                return Json(new
//                {
//                    CDStatus = "OK",
//                    apolice = (entidade == null) ? null : new
//                    {
//                        descricao = entidade.descricao
//                    }
//                }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
//            }
//        }


//        public JsonResult ObterId(int id)
//        {
//            try
//            {
//                var obj = new Seguradora().ObterPorId(id);
//                return Json(new
//                {
//                    CDStatus = "OK",
//                    entidade = (obj == null) ? new Seguradora() : obj
//                }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
//            }
//        }
//    }
//}
