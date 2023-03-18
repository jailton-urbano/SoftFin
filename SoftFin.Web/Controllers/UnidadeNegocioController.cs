using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class UnidadeNegocioController : BaseController
    {
        public override JsonResult TotalizadorDash(int? id)
        {
            //base.TotalizadorDash(id);
            var soma = new UnidadeNegocio().ObterTodos(_paramBase).Count().ToString();
            return Json(new { CDStatus = "OK", Result = soma }, JsonRequestBehavior.AllowGet);

        }

        //Unidades de Negócio
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            IEnumerable<UnidadeNegocio> lista;
            UnidadeNegocio obj = new UnidadeNegocio();
            lista = obj.ObterTodos(_paramBase);
            
            int totalRecords = obj.ObterTodos(_paramBase).Count();

            //Fix for grouping, because it adds column name instead of index to SortingName
            //string sortingName = "cdg_perfil";

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };
            //Table with rows data
            foreach (var item in
                obj.ObterTodos(_paramBase).ToList())
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.unidade
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(UnidadeNegocio obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UnidadeNegocio unidade = new UnidadeNegocio();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    if (unidade.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Unidade de Negócio incluída com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Unidade de Negócio já cadastrado";
                        return View(obj);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Create()
        {
            try
            {
                var obj = new UnidadeNegocio();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(UnidadeNegocio obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UnidadeNegocio unidade = new UnidadeNegocio();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    unidade.Alterar(obj,_paramBase);
                    ViewBag.msg = "Unidade de Negócio alterado";
                    return View(obj);
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            try
            {
                UnidadeNegocio obj = new UnidadeNegocio();
                UnidadeNegocio unidade = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(unidade);
                return View(unidade);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }


        [HttpPost]
        public ActionResult Delete(UnidadeNegocio obj)
        {
            try
            {
                //CarregaViewData();
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    ViewBag.msg = Erro;
                    obj = obj.ObterPorId(obj.id, _paramBase);
                    return View(obj);
                }
                else
                {
                    return RedirectToAction("/Index");
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            try
            {
                UnidadeNegocio obj = new UnidadeNegocio();
                UnidadeNegocio unidade = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(unidade);
                return View(unidade);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

    }
}
