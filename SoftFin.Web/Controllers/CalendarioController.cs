using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Web.Regras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CalendarioController : BaseController
    {

        public ActionResult Index()
        {
            _eventos.Info("Calendario Index");
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            _eventos.Info("Calendario Lista");

            try
            {

                Calendario obj = new Calendario();
                var db = new DbControle();
                var totalRecords = obj.ObterTodos(_paramBase).Count();

                JqGridResponse response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                foreach (var item in obj.ObterTodos(_paramBase).ToList())
                {

                    response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                    {
                        item.data.ToString("dd/MM/yyyy"),
                        item.descricao,
                        item.tipoDataCalendario.tipo
                    }));
                }

                return new JqGridJsonResult() { Data = response };
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Create(Calendario obj)
        {
            _eventos.Info("Calendario Create Post");
            try
            {
                CarregaViewData();
                DateTime dat = new DateTime();

                if (DateTime.TryParse(obj.data.ToString(), out dat) == false)
                {
                    ModelState.AddModelError("", "Data Inválida");
                    return View(obj);
                } 
                
                if (ModelState.IsValid)
                {
                    int empresa  = _paramBase.empresa_id;
                    obj.empresa_id = empresa;
                    
                    if (obj.Incluir(_paramBase) == true)
                    {
                        _eventos.Info("Calendario Create Post Json", obj);
                        ViewBag.msg = "Calendario incluído com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "Calendario já cadastrado";
                    }
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
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();
                var obj = new Calendario();
                obj.data = DateTime.Now;
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(Calendario obj)
        {
            _eventos.Info("Calendario Edit Post");
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    Calendario calendario = new Calendario();
                    int empresa  = _paramBase.empresa_id;
                    obj.empresa_id = empresa;
                    if (calendario.Altera(obj))
                    {
                        _eventos.Info("Calendario Edit Post Json", obj);
                        ViewBag.msg = "Calendario alterado com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "Não foi possivel concluir a operação";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            _eventos.Info("Calendario Edit Get");
            Calendario obj = new Calendario();
            try
            {
                CarregaViewData();
                Calendario calendario = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(calendario);
                return View(calendario);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(Calendario obj)
        {
            CarregaViewData();
            try
            {
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    ViewBag.msg = Erro;
                    obj = obj.ObterPorId(obj.id,_paramBase);
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
                Calendario obj = new Calendario();
                Calendario cd = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(cd);
                return View(cd);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private void CarregaViewData()
        {
            ViewData["TipoData"] = new SelectList(new TipoDataCalendario().ObterTodos(), "id", "tipo");

        }
    }
}
