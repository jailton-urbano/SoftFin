using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using System.Web.Helpers;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace SoftFin.Web.Controllers
{
    public class ImpostosController : BaseController
    {
        DbControle _banco = new DbControle();

        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            var db = new DbControle();
            var regs = new Imposto().ObterTodos();
            var totalRecords = regs.Count();

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

            regs = regs.OrderBy(p => p.descricao).Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                regs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.codigo,
                    item.descricao
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(Imposto obj)
        {
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    Imposto Imposto = new Imposto();


                    if (Imposto.Incluir(obj,_paramBase) == true)
                    {
                        ViewBag.msg = "Imposto incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Imposto já cadastrado";
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
            CarregaViewData();
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Imposto obj)
        {
            CarregaViewData();
            if (ModelState.IsValid)
            {
                Imposto Imposto = new Imposto();


                Imposto.Alterar(obj, _paramBase);

                ViewBag.msg = "Imposto alterado com sucesso";
                return View(obj);

            }
            else
            {
                ModelState.AddModelError("", "Dados Invalidos");
                return View(obj);

            }
        }
        public ActionResult Edit(int ID)
        {
            CarregaViewData();
            Imposto obj = _banco.Imposto.Where(p => p.id == ID).FirstOrDefault();
            return View(obj);
        }
        [HttpPost]
        public ActionResult Delete(Imposto obj)
        {
            try
            {
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    ViewBag.msg = Erro;
                    obj = obj.ObterPorId(obj.id);
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
            CarregaViewData();
            Imposto obj = _banco.Imposto.Where(p => p.id == ID).FirstOrDefault();
            return View(obj);
        }
        private void CarregaViewData()
        {
            var calculoImpostos = from c in _banco.calculoImposto select c;
            ViewData["calculoImposto"] = new SelectList(calculoImpostos, "id", "imposto");
        }
    }
}
