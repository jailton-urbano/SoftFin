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
    public class TipoEnderecoController : BaseController
    {
        public ActionResult Index()
        {

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            
            TipoEndereco obj = new TipoEndereco();
            //lista = obj.listaCategoriaPessoa()();
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
                    item.Descricao
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        public ActionResult Create(TipoEndereco obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoEndereco te = new TipoEndereco();

                    if (te.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Tipo Endereço incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Tipo Endereço já cadastrado";
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
                var obj = new TipoEndereco();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(TipoEndereco obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoEndereco te = new TipoEndereco();
                    te.Alterar(obj, _paramBase);
                    ViewBag.msg = "Tipo Endereço alterado com sucesso";
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
                TipoEndereco obj = new TipoEndereco();
                TipoEndereco tipo = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(tipo);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        [HttpPost]
        public ActionResult Delete(TipoEndereco obj)
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
                TipoEndereco obj = new TipoEndereco();
                TipoEndereco tipo = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(tipo);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }


    }
}
