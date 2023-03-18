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
    public class PlanoDeContaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            
            var regs = new PlanoDeConta().ObterTodos();


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

            regs = regs.OrderBy(p => p.codigo).Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                regs)
            {
                string ObjPAi = "";

                if (item.PlanoDeContasnivelSuperior != null)
                    ObjPAi = item.PlanoDeContasnivelSuperior.descricao;
                
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.codigo,
                    item.descricao,
                    ObjPAi
                }));
            }
            


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }



        [HttpPost]
        public ActionResult Create(PlanoDeConta obj)
        {
            try
            {
                CarregaViewData();

                if (ModelState.IsValid)
                {
                    var modelo = new PlanoDeConta();


                    if (modelo.Incluir(obj) == true)
                    {
                        ViewBag.msg = "Plano de Contas incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Plano de Contas já cadastrado";
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

            
            ViewData["PlanoDeConta"] = new SelectList(new PlanoDeConta().ObterTodos(), "id", "Descricao");
        }
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();

                var obj = new PlanoDeConta();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Delete(PlanoDeConta obj)
        {
            try
            {
                CarregaViewData();

                var cs = new PlanoDeConta();

                cs.Excluir(obj.id);
                ViewBag.msg = "Plano de Contas excluído com sucesso";
                return RedirectToAction("/Index");
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
                var obj = new PlanoDeConta();
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
        public ActionResult Edit(PlanoDeConta obj)
        {
            try
            {
                CarregaViewData();
                var codigo = new PlanoDeConta();
                codigo.Alterar(obj);

                ViewBag.msg = "Plano de Contas alterado com sucesso";
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
                var obj = new PlanoDeConta();
                obj = new PlanoDeConta().ObterPorId(ID, _paramBase);
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
