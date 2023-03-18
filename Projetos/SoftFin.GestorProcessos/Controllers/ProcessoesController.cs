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
    public class ProcessoesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: Processoes
        public ActionResult Index()
        {
            var processo = db.Processo; ;
            return View(processo.ToList());
        }

        // GET: Processoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Processo processo = db.Processo.Find(id);
            if (processo == null)
            {
                return HttpNotFound();
            }
            return View(processo);
        }

        // GET: Processoes/Create
        public ActionResult Create()
        {
            ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao");
            return View();
        }

        // POST: Processoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Codigo,CodigoProcessoTemplate,Descricao,Ativo,CodigoEmpresa,VersaoId,Contador")] Processo processo)
        {
            if (ModelState.IsValid)
            {
                db.Processo.Add(processo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao", processo.VersaoId);
            return View(processo);
        }

        // GET: Processoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Processo processo = db.Processo.Find(id);
            if (processo == null)
            {
                return HttpNotFound();
            }
            //ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao", processo.VersaoId);
            return View(processo);
        }

        // POST: Processoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Codigo,CodigoProcessoTemplate,Descricao,Ativo,CodigoEmpresa,VersaoId,Contador")] Processo processo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(processo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao", processo.VersaoId);
            return View(processo);
        }

        // GET: Processoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Processo processo = db.Processo.Find(id);
            if (processo == null)
            {
                return HttpNotFound();
            }
            return View(processo);
        }

        // POST: Processoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Processo processo = db.Processo.Find(id);
            db.Processo.Remove(processo);
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
