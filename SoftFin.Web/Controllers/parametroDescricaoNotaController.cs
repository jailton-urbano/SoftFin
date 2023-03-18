using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Negocios;

namespace SoftFin.Web.Controllers
{
    public class parametroDescricaoNotaController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            var regs = new parametroDescricaoNota().ObterTodos();

            int totalRecords = regs.Count();

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

            regs = regs.OrderBy(p => p.nome).Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                regs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.nome,
                    item.nomemodel,
                    item.campo,
                    item.hashtag
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(parametroDescricaoNota obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    parametroDescricaoNota parametroDescricao = new parametroDescricaoNota();

                    if (parametroDescricao.Incluir(obj,_paramBase) == true)
                    {
                        ViewBag.msg = "Operação incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Operação já cadastrado";
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
            return View();
        }
        [HttpPost]
        public ActionResult Edit(parametroDescricaoNota obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    parametroDescricaoNota parametroDescricaoNotaaux = new parametroDescricaoNota();

                    parametroDescricaoNotaaux.Alterar(obj,_paramBase);

                    ViewBag.msg = "Operação alterado com sucesso";
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
                var obj = new parametroDescricaoNota().ObterPorId(ID);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Delete(parametroDescricaoNota obj)
        {

            try
            {
                //CarregaViewData();
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro,_paramBase))
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
            try
            {
                var obj = new parametroDescricaoNota().ObterPorId(ID);
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
