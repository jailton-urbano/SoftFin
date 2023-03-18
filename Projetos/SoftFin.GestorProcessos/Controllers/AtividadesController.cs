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
    public class AtividadesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: Atividades
        public ActionResult Index()
        {
            var atividade = db.Atividade.Include(a => a.AtividadeTipo);
            return View(atividade.ToList());
        }

        // GET: Atividades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atividade atividade = db.Atividade.Find(id);
            if (atividade == null)
            {
                return HttpNotFound();
            }
            return View(atividade);
        }

        // GET: Atividades/Create
        public ActionResult Create()
        {
            ViewBag.IdAtividadeTipo = new SelectList(db.AtividadeTipo, "Id", "Descricao");
            return View();
        }

        // POST: Atividades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,Codigo,Ativo,CodigoEmpresa,CodigoEstabelecimento,Url,IdAtividadeTipo")] Atividade atividade)
        {
            if (ModelState.IsValid)
            {
                db.Atividade.Add(atividade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAtividadeTipo = new SelectList(db.AtividadeTipo, "Id", "Descricao", atividade.IdAtividadeTipo);
            return View(atividade);
        }

        // GET: Atividades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atividade atividade = db.Atividade.Find(id);
            if (atividade == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAtividadeTipo = new SelectList(db.AtividadeTipo, "Id", "Descricao", atividade.IdAtividadeTipo);
            return View(atividade);
        }

        // POST: Atividades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao,Codigo,Ativo,CodigoEmpresa,CodigoEstabelecimento,Url,IdAtividadeTipo")] Atividade atividade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atividade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAtividadeTipo = new SelectList(db.AtividadeTipo, "Id", "Descricao", atividade.IdAtividadeTipo);
            return View(atividade);
        }

        // GET: Atividades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atividade atividade = db.Atividade.Find(id);
            if (atividade == null)
            {
                return HttpNotFound();
            }
            return View(atividade);
        }

        // POST: Atividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Atividade atividade = db.Atividade.Find(id);
            db.Atividade.Remove(atividade);
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
