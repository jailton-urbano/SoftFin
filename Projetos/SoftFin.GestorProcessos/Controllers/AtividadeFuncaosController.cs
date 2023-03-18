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
    public class AtividadeFuncaosController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: AtividadeFuncaos
        public ActionResult Index()
        {
            var atividadeFuncao = db.AtividadeFuncao.Include(a => a.Atividade).Include(a => a.Funcao);
            return View(atividadeFuncao.ToList());
        }

        // GET: AtividadeFuncaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeFuncao atividadeFuncao = db.AtividadeFuncao.Find(id);
            if (atividadeFuncao == null)
            {
                return HttpNotFound();
            }
            return View(atividadeFuncao);
        }

        // GET: AtividadeFuncaos/Create
        public ActionResult Create()
        {
            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao");
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao");
            return View();
        }

        // POST: AtividadeFuncaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdAtividade,IdFuncao")] AtividadeFuncao atividadeFuncao)
        {
            if (ModelState.IsValid)
            {
                db.AtividadeFuncao.Add(atividadeFuncao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao", atividadeFuncao.IdAtividade);
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao", atividadeFuncao.IdFuncao);
            return View(atividadeFuncao);
        }

        // GET: AtividadeFuncaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeFuncao atividadeFuncao = db.AtividadeFuncao.Find(id);
            if (atividadeFuncao == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao", atividadeFuncao.IdAtividade);
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao", atividadeFuncao.IdFuncao);
            return View(atividadeFuncao);
        }

        // POST: AtividadeFuncaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdAtividade,IdFuncao")] AtividadeFuncao atividadeFuncao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atividadeFuncao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAtividade = new SelectList(db.Atividade, "Id", "Descricao", atividadeFuncao.IdAtividade);
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao", atividadeFuncao.IdFuncao);
            return View(atividadeFuncao);
        }

        // GET: AtividadeFuncaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadeFuncao atividadeFuncao = db.AtividadeFuncao.Find(id);
            if (atividadeFuncao == null)
            {
                return HttpNotFound();
            }
            return View(atividadeFuncao);
        }

        // POST: AtividadeFuncaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AtividadeFuncao atividadeFuncao = db.AtividadeFuncao.Find(id);
            db.AtividadeFuncao.Remove(atividadeFuncao);
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
