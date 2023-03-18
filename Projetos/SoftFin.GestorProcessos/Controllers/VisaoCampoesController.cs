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
    public class VisaoCampoesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: VisaoCampoes
        public ActionResult Index()
        {
            var visaoCampo = db.VisaoCampo.Include(v => v.TabelaCampo).Include(v => v.Visao);
            return View(visaoCampo.ToList());
        }

        // GET: VisaoCampoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VisaoCampo visaoCampo = db.VisaoCampo.Find(id);
            if (visaoCampo == null)
            {
                return HttpNotFound();
            }
            return View(visaoCampo);
        }

        // GET: VisaoCampoes/Create
        public ActionResult Create()
        {
            ViewBag.IdTabelaCampo = new SelectList(db.TabelaCampo, "Id", "Campo");
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao");
            return View();
        }

        // POST: VisaoCampoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdTabelaCampo,Ativo,Visivel,Transferivel,IdVisao,PadraoSalva,ReferenciaNgModel,ReferenciaNgChange")] VisaoCampo visaoCampo)
        {
            if (ModelState.IsValid)
            {
                db.VisaoCampo.Add(visaoCampo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTabelaCampo = new SelectList(db.TabelaCampo, "Id", "Campo", visaoCampo.IdTabelaCampo);
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao", visaoCampo.IdVisao);
            return View(visaoCampo);
        }

        // GET: VisaoCampoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VisaoCampo visaoCampo = db.VisaoCampo.Find(id);
            if (visaoCampo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTabelaCampo = new SelectList(db.TabelaCampo, "Id", "Campo", visaoCampo.IdTabelaCampo);
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao", visaoCampo.IdVisao);
            return View(visaoCampo);
        }

        // POST: VisaoCampoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdTabelaCampo,Ativo,Visivel,Transferivel,IdVisao,PadraoSalva,ReferenciaNgModel,ReferenciaNgChange")] VisaoCampo visaoCampo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(visaoCampo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTabelaCampo = new SelectList(db.TabelaCampo, "Id", "Campo", visaoCampo.IdTabelaCampo);
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao", visaoCampo.IdVisao);
            return View(visaoCampo);
        }

        // GET: VisaoCampoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VisaoCampo visaoCampo = db.VisaoCampo.Find(id);
            if (visaoCampo == null)
            {
                return HttpNotFound();
            }
            return View(visaoCampo);
        }

        // POST: VisaoCampoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VisaoCampo visaoCampo = db.VisaoCampo.Find(id);
            db.VisaoCampo.Remove(visaoCampo);
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
