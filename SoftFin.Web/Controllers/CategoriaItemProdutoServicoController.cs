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
    public class CategoriaItemProdutoServicoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            CategoriaItemProdutoServico obj = new CategoriaItemProdutoServico();
            int totalRecords = obj.ObterTodos(_paramBase).Count();

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
                    item.descricao
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(CategoriaItemProdutoServico obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CategoriaItemProdutoServico cp = new CategoriaItemProdutoServico();

                    if (cp.Incluir(obj,_paramBase) == true)
                    {
                        ViewBag.msg = "Categoria incluída com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Categoria já cadastrada";
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
                var obj = new CategoriaItemProdutoServico();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(CategoriaItemProdutoServico obj)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    CategoriaItemProdutoServico cp = new CategoriaItemProdutoServico();

                    cp.Alterar(obj, _paramBase);
                    ViewBag.msg = "Categoria alterada com sucesso.";
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
                CategoriaItemProdutoServico obj = new CategoriaItemProdutoServico();
                CategoriaItemProdutoServico categoria = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(categoria);
                return View(categoria);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(CategoriaItemProdutoServico obj)
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
                CategoriaItemProdutoServico obj = new CategoriaItemProdutoServico();
                CategoriaItemProdutoServico categoria = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(categoria);
                return View(categoria);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

    }
}

