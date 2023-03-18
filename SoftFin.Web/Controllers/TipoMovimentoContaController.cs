using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class TipoMovimentoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            var obj = new TipoMovimento();
            var totalRecords = obj.ObterTodos(_paramBase).Count();

            var response = new JqGridResponse()
            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in
                obj.ObterTodos(_paramBase).ToList())
            {

                
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Codigo,
                    item.Descricao,
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }



        [HttpPost]
        public ActionResult Create(TipoMovimento obj)
        {
            try
            {
                CarregaViewData();

                if (ModelState.IsValid)
                {
                    var modelo = new TipoMovimento();

                    if (modelo.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Tipo Movimento incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Tipo Movimento já cadastrado";
                        return View(obj);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados Invalidos");
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private void CarregaViewData()
        {
            ViewData["TipoMovimento"] = new SelectList(new TipoMovimento().ObterTodos(_paramBase), "id", "Descricao");
        }
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();

                var obj = new TipoMovimento();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        public ActionResult Delete(TipoMovimento obj)
        {
            try
            {
                CarregaViewData();
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
                CarregaViewData();
                var obj = new TipoMovimento();
                var cs = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(cs);
                return View(cs);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        public ActionResult Edit(TipoMovimento obj)
        {
            try
            {
                CarregaViewData();
                var codigo = new TipoMovimento();

                codigo.Alterar(obj, _paramBase);

                ViewBag.msg = "Tipo Movimento alterado com sucesso";
                return RedirectToAction("/Index");
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
                CarregaViewData();
                var obj = new TipoMovimento();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
    }
}
