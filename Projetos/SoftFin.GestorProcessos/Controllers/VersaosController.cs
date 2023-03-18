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
    public class VersaosController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: Versaos
        public ActionResult Index()
        {
            return View(db.Versao.ToList());
        }

        // GET: Versaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Versao versao = db.Versao.Find(id);
            if (versao == null)
            {
                return HttpNotFound();
            }
            return View(versao);
        }

        // GET: Versaos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Versaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,Ativo,CodigoEmpresa")] Versao versao)
        {
            if (ModelState.IsValid)
            {
                db.Versao.Add(versao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(versao);
        }

        // GET: Versaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Versao versao = db.Versao.Find(id);
            if (versao == null)
            {
                return HttpNotFound();
            }
            return View(versao);
        }

        // POST: Versaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao,Ativo,CodigoEmpresa")] Versao versao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(versao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(versao);
        }

        // GET: Versaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Versao versao = db.Versao.Find(id);
            if (versao == null)
            {
                return HttpNotFound();
            }
            return View(versao);
        }

        // POST: Versaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Versao versao = db.Versao.Find(id);
            db.Versao.Remove(versao);
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
