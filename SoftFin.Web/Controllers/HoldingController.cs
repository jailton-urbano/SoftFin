using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;
using Lib.Web.Mvc.JQuery.JqGrid;


namespace SoftFin.Web.Controllers
{
    public class HoldingController : BaseController
    {
        private DbControle db = new DbControle();

        //
        // GET: /Holding/

        public ActionResult Index()
        {
            return View(db.Holding.ToList());
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            var db = new DbControle();
            int totalRecords = new Holding().ObterTodos(_paramBase).Count();

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
                new Holding().ObterTodos(_paramBase))
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.id,
                    item.codigo,
                    item.nome
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        //
        // GET: /Holding/Details/5

        public ActionResult Details(int id = 0)
        {
            try
            {

                Holding holding = db.Holding.Find(id);
                if (holding == null)
                {
                    return HttpNotFound();
                }
                return View(holding);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        //
        // GET: /Holding/Create

        public ActionResult Create()
        {
            return View(new Holding());
        }

        //
        // POST: /Holding/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Holding holding)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Holding.Add(holding);
                    db.SaveChanges();
                    return RedirectToAction("/Index");
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

            return View(holding);
        }

        //
        // GET: /Holding/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Holding holding = db.Holding.Find(id);
            if (holding == null)
            {
                return HttpNotFound();
            }
            return View(holding);
        }

        //
        // POST: /Holding/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Holding holding)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(holding).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("/Index");
                }
                return View(holding);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        //
        // GET: /Holding/Delete/5

        public ActionResult Delete(int id = 0)
        {
            try
            {
                Holding holding = db.Holding.Find(id);
                if (holding == null)
                {
                    return HttpNotFound();
                }
                return View(holding);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        //
        // POST: /Holding/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Holding holding = db.Holding.Find(id);
                db.Holding.Remove(holding);
                db.SaveChanges();
                return RedirectToAction("/Index");
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
       }
    }
}