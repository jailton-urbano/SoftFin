using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FuncionalidadeController : BaseController
    {
        private DbControle db = new DbControle();

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            string ValorDescricao = Request.QueryString["Descricao"];
            string ValorAction = Request.QueryString["NomeAction"];
            string ValorController = Request.QueryString["NomeController"];

            var db = new DbControle();
            int totalRecords = db.Funcionalidade.Count();

            var objs = db.Funcionalidade.OrderBy(p => p.idPai & p.Ordem).ToList();

            if (!String.IsNullOrEmpty(ValorDescricao))
            {
                objs = objs.Where(p => p.Descricao.Contains(ValorDescricao)).ToList();
            }

            if (!String.IsNullOrEmpty(ValorAction))
            {
                objs = objs.Where(p => p.NomeAction.Contains(ValorAction)).ToList();
            }

            if (!String.IsNullOrEmpty(ValorController))
            {
                objs = objs.Where(p => p.NomeController.Contains(ValorController)).ToList();
            }

            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

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
                objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.id,
                    item.Descricao,
                    item.NomeController,
                    item.NomeAction,
                    item.Ordem,
                    (item.Ativo.Value) ? "Sim" : "Não",
                    (item.Jarvis.Value) ? "Sim" : "Não",
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Funcionalidade funcionalidade)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Funcionalidade.Add(funcionalidade);
                    db.SaveChanges();
                    return RedirectToAction("/Index");
                }

                return View(funcionalidade);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

                
        }

        public ActionResult Edit(int id = 0)
        {
            try
            {
                Funcionalidade Funcionalidade = db.Funcionalidade.Find(id);

                return View(Funcionalidade);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Funcionalidade funcionalidade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(funcionalidade).State = EntityState.Modified;   
                db.SaveChanges();
                return RedirectToAction("/Index");
            }
            return View(funcionalidade);
        }

        public ActionResult Delete(int id = 0)
        {
            Funcionalidade Funcionalidade = db.Funcionalidade.Find(id);
            if (Funcionalidade == null)
            {
                return HttpNotFound();
            }
            return View(Funcionalidade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Funcionalidade Funcionalidade = db.Funcionalidade.Find(id);
            db.Funcionalidade.Remove(Funcionalidade);
            db.SaveChanges();
            return RedirectToAction("/Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
