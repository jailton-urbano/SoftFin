using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Models;

namespace SoftFin.GestorProcessos.Controllers
{
    public class AtividadeTipoesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: AtividadeTipoes
        public ActionResult Index()
        {
            return View(db.AtividadeTipo.ToList());
        }

        // GET: AtividadeTipoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeTipo atividadeTipo = db.AtividadeTipo.Find(id);
            if (atividadeTipo == null)
            {
                return HttpNotFound();
            }
            return View(atividadeTipo);
        }

        // GET: AtividadeTipoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AtividadeTipoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,Ativo,Codigo")] AtividadeTipo atividadeTipo)
        {
            if (ModelState.IsValid)
            {
                db.AtividadeTipo.Add(atividadeTipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(atividadeTipo);
        }

        // GET: AtividadeTipoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeTipo atividadeTipo = db.AtividadeTipo.Find(id);
            if (atividadeTipo == null)
            {
                return HttpNotFound();
            }
            return View(atividadeTipo);
        }

        // POST: AtividadeTipoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao,Ativo,Codigo")] AtividadeTipo atividadeTipo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atividadeTipo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(atividadeTipo);
        }

        // GET: AtividadeTipoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeTipo atividadeTipo = db.AtividadeTipo.Find(id);
            if (atividadeTipo == null)
            {
                return HttpNotFound();
            }
            return View(atividadeTipo);
        }

        // POST: AtividadeTipoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AtividadeTipo atividadeTipo = db.AtividadeTipo.Find(id);
            db.AtividadeTipo.Remove(atividadeTipo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
