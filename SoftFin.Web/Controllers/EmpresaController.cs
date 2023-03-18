//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Sistema1.Models;
//using Sistema1.Classes;
//using Lib.Web.Mvc.JQuery.JqGrid;

//namespace Sistema1.Controllers
//{
//    public class EmpresaController : BaseController
//    {

//        //
//        // GET: /Empresa/

//        public ActionResult Index()
//        {
//            return View(new Empresa().ObterTodos());
//        }


//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {

//            int totalRecords = new Empresa().ObterTodos().Count();

//            //Fix for grouping, because it adds column name instead of index to SortingName
//            //string sortingName = "cdg_perfil";

//            //Prepare JqGridData instance
//            JqGridResponse response = new JqGridResponse()
//            {
//                //Total pages count
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                //Page number
//                PageIndex = request.PageIndex,
//                //Total records count
//                TotalRecordsCount = totalRecords
//            };
//            //Table with rows data
//            foreach (var item in
//                new Empresa().ObterTodos())
//            {
//                var auxhold = "";
//                if (item.Holding != null)
//                    auxhold = item.Holding.nome;

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.id,
//                    item.apelido,
//                    item.codigo,
//                    item.nome,
//                    auxhold
//                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }

//        //
//        // GET: /Empresa/Details/5

//        public ActionResult Details(int id = 0)
//        {
//            try
//            {
//                Empresa empresa = new Empresa().ObterPorId(id);
//                if (empresa == null)
//                {
//                    return HttpNotFound();
//                }
//                return View(empresa);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        //
//        // GET: /Empresa/Create

//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();
//                return View(new Empresa());
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        private void CarregaViewData()
//        {
//            ViewData["Holding"] = new SelectList(new Holding().ObterTodos(), "id", "nome");
//        }

//        //
//        // POST: /Empresa/Create

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Empresa obj)        
//        {
//            try
//            {
//                CarregaViewData();
//                //CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    if (obj.Incluir())
//                    {
//                        ViewBag.msg = "Incluir com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Não foi possivel incluir o registro";
//                    }
//                    return View(obj);
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        //
//        // GET: /Empresa/Edit/5

//        public ActionResult Edit(int id = 0)
//        {
//            try
//            {
//                CarregaViewData();
//                Empresa empresa = new Empresa().ObterPorId(id);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(empresa);
//                return View(empresa);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        //
//        // POST: /Empresa/Edit/5

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(Empresa obj)
//        {
//            try
//            {
//              CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    if (obj.Alterar() == true)
//                    {
//                        ViewBag.msg = "Alterado com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Não foi possivel atualizar o registro";
//                    }
//                    return View(obj);
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        //
//        // GET: /Empresa/Delete/5

//        public ActionResult Delete(int id = 0)
//        {
//            try
//            {
//                CarregaViewData();
//                Empresa empresa = new Empresa().ObterPorId(id);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(empresa);
//                return View(empresa);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        //
//        // POST: /Empresa/Delete/5

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(Empresa obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";
//                if (!obj.Excluir(obj.id, ref Erro))
//                {
//                    ViewBag.msg = Erro;

//                    obj = obj.ObterPorId(obj.id);
//                    return View(obj);
//                }
//                else
//                {
//                    return RedirectToAction("/Index");
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }


//    }
//}