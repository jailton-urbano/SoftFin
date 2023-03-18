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
    public class CategoriaPessoaController : BaseController
    {
        //Categoria Pessoa
        public ActionResult Index()
        {
            //IEnumerable<CategoriaPessoa> lista;
            //CategoriaPessoa obj = new CategoriaPessoa();
            //lista = obj.listaCategoriaPessoa();
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            CategoriaPessoa obj = new CategoriaPessoa();
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
        public ActionResult Create(CategoriaPessoa obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CategoriaPessoa cp = new CategoriaPessoa();

                    if (cp.Incluir(obj,_paramBase) == true)
                    {
                        ViewBag.msg = "Categoria Pessoa incluída com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Categoria Pessoa já cadastrada";
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
                var obj = new CategoriaPessoa();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(CategoriaPessoa obj)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    CategoriaPessoa cp = new CategoriaPessoa();

                    cp.Alterar(obj, _paramBase);
                    ViewBag.msg = "Categoria Pessoa alterada com sucesso.";
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
                CategoriaPessoa obj = new CategoriaPessoa();
                CategoriaPessoa pessoa = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(pessoa);
                return View(pessoa);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(CategoriaPessoa obj)
        {
            try
            {
                //CarregaViewData();
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro,_paramBase))
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
                CategoriaPessoa obj = new CategoriaPessoa();
                CategoriaPessoa pessoa = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(pessoa);

                return View(pessoa);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

    }
}
