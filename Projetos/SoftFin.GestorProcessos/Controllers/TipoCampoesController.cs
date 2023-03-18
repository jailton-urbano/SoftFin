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
    public class TipoCampoesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: TipoCampoes
        public ActionResult Index()
        {
            return View(db.TipoCampo.ToList());
        }

        // GET: TipoCampoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoCampo tipoCampo = db.TipoCampo.Find(id);
            if (tipoCampo == null)
            {
                return HttpNotFound();
            }
            return View(tipoCampo);
        }

        // GET: TipoCampoes/Create
        [ValidateInput(false)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoCampoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Descricao,Ativo,TemplateHTML,TemplateHTMLModal,TemplateJSControl,TemplateJSScript,TipoBancoDados,SQLDefault")] TipoCampo tipoCampo)
        {
            if (ModelState.IsValid)
            {
                db.TipoCampo.Add(tipoCampo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoCampo);
        }

        // GET: TipoCampoes/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoCampo tipoCampo = db.TipoCampo.Find(id);
            if (tipoCampo == null)
            {
                return HttpNotFound();
            }
            return View(tipoCampo);
        }

        // POST: TipoCampoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Descricao,Ativo,TemplateHTML,TemplateHTMLModal,TemplateJSControl,TemplateJSScript,TipoBancoDados,SQLDefault")] TipoCampo tipoCampo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoCampo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoCampo);
        }

        // GET: TipoCampoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoCampo tipoCampo = db.TipoCampo.Find(id);
            if (tipoCampo == null)
            {
                return HttpNotFound();
            }
            return View(tipoCampo);
        }

        // POST: TipoCampoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoCampo tipoCampo = db.TipoCampo.Find(id);
            db.TipoCampo.Remove(tipoCampo);
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
