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
    public class AtividadeVisaosController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: AtividadeVisaos
        public ActionResult Index()
        {
            var atividadeVisao = db.AtividadeVisao.Include(a => a.Atividade).Include(a => a.Visao);
            return View(atividadeVisao.ToList());
        }

        // GET: AtividadeVisaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeVisao atividadeVisao = db.AtividadeVisao.Find(id);
            if (atividadeVisao == null)
            {
                return HttpNotFound();
            }
            return View(atividadeVisao);
        }

        // GET: AtividadeVisaos/Create
        public ActionResult Create()
        {
            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao");
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao");
            return View();
        }

        // POST: AtividadeVisaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ordem,IdAtividade,IdVisao,Titulo")] AtividadeVisao atividadeVisao)
        {
            if (ModelState.IsValid)
            {
                db.AtividadeVisao.Add(atividadeVisao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao", atividadeVisao.IdAtividade);
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao", atividadeVisao.IdVisao);
            return View(atividadeVisao);
        }

        // GET: AtividadeVisaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeVisao atividadeVisao = db.AtividadeVisao.Find(id);
            if (atividadeVisao == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao", atividadeVisao.IdAtividade);
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao", atividadeVisao.IdVisao);
            return View(atividadeVisao);
        }

        // POST: AtividadeVisaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ordem,IdAtividade,IdVisao,Titulo")] AtividadeVisao atividadeVisao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atividadeVisao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao", atividadeVisao.IdAtividade);
            ViewBag.IdVisao = new SelectList(db.Visao, "Id", "Descricao", atividadeVisao.IdVisao);
            return View(atividadeVisao);
        }

        // GET: AtividadeVisaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeVisao atividadeVisao = db.AtividadeVisao.Find(id);
            if (atividadeVisao == null)
            {
                return HttpNotFound();
            }
            return View(atividadeVisao);
        }

        // POST: AtividadeVisaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AtividadeVisao atividadeVisao = db.AtividadeVisao.Find(id);
            db.AtividadeVisao.Remove(atividadeVisao);
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
