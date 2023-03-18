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
    public class AtividadePlanoesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: AtividadePlanoes
        public ActionResult Index()
        {
            var atividadePlano = db.AtividadePlano.Include(a => a.Atividade).Include(a => a.Processo);
            return View(atividadePlano.ToList());
        }

        // GET: AtividadePlanoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadePlano atividadePlano = db.AtividadePlano.Find(id);
            if (atividadePlano == null)
            {
                return HttpNotFound();
            }
            return View(atividadePlano);
        }

        // GET: AtividadePlanoes/Create
        public ActionResult Create()
        {
            ViewBag.AtividadeId = new SelectList(db.Atividade, "Id", "Descricao");
            ViewBag.ProcessoId = new SelectList(db.Processo, "Id", "Codigo");
            ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao");
            return View();
        }

        // POST: AtividadePlanoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProcessoId,VersaoId,AtividadeId,AtividadeIdEntrada,CondicaoEntrada")] AtividadePlano atividadePlano)
        {
            if (ModelState.IsValid)
            {
                db.AtividadePlano.Add(atividadePlano);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AtividadeId = new SelectList(db.Atividade, "Id", "Descricao", atividadePlano.AtividadeId);
            ViewBag.ProcessoId = new SelectList(db.Processo, "Id", "Codigo", atividadePlano.ProcessoId);
            //ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao", atividadePlano.VersaoId);
            return View(atividadePlano);
        }

        // GET: AtividadePlanoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadePlano atividadePlano = db.AtividadePlano.Find(id);
            if (atividadePlano == null)
            {
                return HttpNotFound();
            }
            ViewBag.AtividadeId = new SelectList(db.Atividade, "Id", "Descricao", atividadePlano.AtividadeId);
            ViewBag.ProcessoId = new SelectList(db.Processo, "Id", "Codigo", atividadePlano.ProcessoId);
            //ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao", atividadePlano.VersaoId);
            return View(atividadePlano);
        }

        // POST: AtividadePlanoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProcessoId,VersaoId,AtividadeId,AtividadeIdEntrada,CondicaoEntrada")] AtividadePlano atividadePlano)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atividadePlano).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AtividadeId = new SelectList(db.Atividade, "Id", "Descricao", atividadePlano.AtividadeId);
            ViewBag.ProcessoId = new SelectList(db.Processo, "Id", "Codigo", atividadePlano.ProcessoId);
            //ViewBag.VersaoId = new SelectList(db.Versao, "Id", "Descricao", atividadePlano.VersaoId);
            return View(atividadePlano);
        }

        // GET: AtividadePlanoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtividadePlano atividadePlano = db.AtividadePlano.Find(id);
            if (atividadePlano == null)
            {
                return HttpNotFound();
            }
            return View(atividadePlano);
        }

        // POST: AtividadePlanoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AtividadePlano atividadePlano = db.AtividadePlano.Find(id);
            db.AtividadePlano.Remove(atividadePlano);
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
