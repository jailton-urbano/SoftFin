using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RamoSeguroController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }


        private static string ExtraiString(Dictionary<string, string> parameters, string key)
        {
            try
            {
                return parameters[key];
            }
            catch
            {
                return null;
            }

        }

        private List<RamoSeguro> Organiza(JqGridRequest request, List<RamoSeguro> objs)
        {

            return new SoftFin.Utils.UtilSoftFin.GenericSorter<RamoSeguro>().Sort(objs.AsQueryable(), request.SortingName, request.SortingOrder).ToList();
        }


        //[OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Listas(JqGridRequest request)
        {
            Dictionary<string, string> parameters = HttpContext.Request.QueryString.Keys.Cast<string>()
                .ToDictionary(k => k, v => HttpContext.Request.QueryString[v]);

            int totalRecords = 0;
            var objs = new RamoSeguro().ObterTodos();

            if (!String.IsNullOrEmpty(ExtraiString(parameters, "descr")))
            {
                var auxstring = parameters["descricao"];
                objs = objs.Where(p => p.descricao == auxstring).ToList();
            }

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
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.descricao
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public JsonResult Salvar(RamoSeguro entidade)
        {
            try
            {
                var validacao = entidade.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                if (entidade.ObterPorId(entidade.id) == null)
                {
                    entidade.Incluir(_paramBase);
                }
                else
                {
                    entidade.Alterar(entidade,_paramBase);
                }
                return Json(new
                {
                    CDStatus = "OK",
                    apolice = (entidade == null) ? null : new
                    {
                        descricao = entidade.descricao
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Excluir(RamoSeguro entidade)
        {
            try
            {
                if (entidade.ObterPorId(entidade.id) != null)
                {
                    string auxErro = "";
                    if (!entidade.Excluir(entidade.id, ref auxErro, _paramBase))
                    {
                        return Json(new { CDStatus = "NOK", Exception = auxErro }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { CDStatus = "NOK", Exception = "Registro não encontrado" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    CDStatus = "OK",
                    apolice = (entidade == null) ? null : new
                    {
                        descricao = entidade.descricao
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObterId(int id)
        {
            try
            {
                var obj = new RamoSeguro().ObterPorId(id);
                return Json(new
                {
                    CDStatus = "OK",
                    entidade = (obj == null) ? new RamoSeguro() : obj
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
