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
    public class TipoContratoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listas(JqGridRequest request)
        {

            int totalRecords = 0;



            totalRecords = new TipoContrato().ObterTodos(_paramBase).Count();

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

            List<TipoContrato> lista = new List<TipoContrato>();

            lista = new TipoContrato().ObterTodos(_paramBase).OrderBy(p => p.tipo).Skip(12 * request.PageIndex).Take(12).ToList();

            foreach (var item in lista)
            {


                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.tipo
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(TipoContrato obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoContrato tipo = new TipoContrato();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    if (tipo.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Tipo do Contrato incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Tipo do Contrato já cadastrado";
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
                var obj = new TipoContrato();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(TipoContrato obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoContrato tipo = new TipoContrato();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    tipo.Alterar(obj,_paramBase);

                    ViewBag.msg = "Tipo do Contrato alterado com sucesso";
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
                TipoContrato obj = new TipoContrato();
                TipoContrato tc = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(tc);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(TipoContrato obj)
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
                TipoContrato obj = new TipoContrato();
                TipoContrato tc = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(tc);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

    }
}
