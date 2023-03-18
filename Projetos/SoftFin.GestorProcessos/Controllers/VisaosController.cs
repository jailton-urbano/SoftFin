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
    public class VisaosController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: Visaos
        public ActionResult Index()
        {
            var visao = db.Visao.Include(v => v.Tabela);
            return View(visao.ToList());
        }

        // GET: Visaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visao visao = db.Visao.Find(id);
            if (visao == null)
            {
                return HttpNotFound();
            }
            return View(visao);
        }

        // GET: Visaos/Create
        public ActionResult Create()
        {
            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome");
            return View();
        }

        // POST: Visaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,Ativo,CodigoEmpresa,IdTabela,TipoVisao")] Visao visao)
        {
            if (ModelState.IsValid)
            {
                db.Visao.Add(visao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome", visao.IdTabela);
            return View(visao);
        }

        // GET: Visaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visao visao = db.Visao.Find(id);
            if (visao == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome", visao.IdTabela);
            return View(visao);
        }

        // POST: Visaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao,Ativo,CodigoEmpresa,IdTabela,TipoVisao")] Visao visao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(visao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome", visao.IdTabela);
            return View(visao);
        }

        // GET: Visaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visao visao = db.Visao.Find(id);
            if (visao == null)
            {
                return HttpNotFound();
            }
            return View(visao);
        }

        // POST: Visaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Visao visao = db.Visao.Find(id);
            db.Visao.Remove(visao);
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
