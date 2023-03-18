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
    public class TabelaCampoesController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: TabelaCampoes
        public ActionResult Index()
        {
            var tabelaCampo = db.TabelaCampo.Include(t => t.Tabela).Include(t => t.TipoCampo);
            return View(tabelaCampo.ToList());
        }

        // GET: TabelaCampoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TabelaCampo tabelaCampo = db.TabelaCampo.Find(id);
            if (tabelaCampo == null)
            {
                return HttpNotFound();
            }
            return View(tabelaCampo);
        }

        // GET: TabelaCampoes/Create
        [ValidateInput(false)]
        public ActionResult Create()
        {
            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome");
            ViewBag.IdTipoCampo = new SelectList(db.TipoCampo, "Id", "Descricao");
            return View();
        }

        // POST: TabelaCampoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Campo,Descricao,Ativo,IdTabela,IdTipoCampo,TamanhoColuna,TamanhoCampo,Precisao,FK,FKUrl,FKRetorno,FKParam,Ordem,Obrigatorio,SQLDefault")] TabelaCampo tabelaCampo)
        {
            if (ModelState.IsValid)
            {
                db.TabelaCampo.Add(tabelaCampo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTipoCampo = new SelectList(db.TipoCampo, "Id", "Descricao", tabelaCampo.IdTipoCampo);
            return View(tabelaCampo);
        }

        // GET: TabelaCampoes/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TabelaCampo tabelaCampo = db.TabelaCampo.Find(id);
            if (tabelaCampo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome", tabelaCampo.Tabela_Id);
            ViewBag.IdTipoCampo = new SelectList(db.TipoCampo, "Id", "Descricao", tabelaCampo.IdTipoCampo);
            return View(tabelaCampo);
        }

        // POST: TabelaCampoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Campo,Descricao,Ativo,IdTabela,IdTipoCampo,TamanhoColuna,TamanhoCampo,Precisao,FK,FKUrl,FKRetorno,FKParam,Ordem,Obrigatorio,SQLDefault")] TabelaCampo tabelaCampo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tabelaCampo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTabela = new SelectList(db.Tabela, "Id", "Nome", tabelaCampo.Tabela_Id);
            ViewBag.IdTipoCampo = new SelectList(db.TipoCampo, "Id", "Descricao", tabelaCampo.IdTipoCampo);
            return View(tabelaCampo);
        }

        // GET: TabelaCampoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TabelaCampo tabelaCampo = db.TabelaCampo.Find(id);
            if (tabelaCampo == null)
            {
                return HttpNotFound();
            }
            return View(tabelaCampo);
        }

        // POST: TabelaCampoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TabelaCampo tabelaCampo = db.TabelaCampo.Find(id);
            db.TabelaCampo.Remove(tabelaCampo);
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
