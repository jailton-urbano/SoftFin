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
    public class TabelasController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: Tabelas
        public ActionResult Index()
        {
            return View(db.Tabela.ToList());
        }

        // GET: Tabelas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tabela tabela = db.Tabela.Find(id);
            if (tabela == null)
            {
                return HttpNotFound();
            }
            return View(tabela);
        }

        // GET: Tabelas/Create
        [ValidateInput(false)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tabelas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Nome,Descricao,Ativo,CodigoEmpresa,CadastroAuxiliar,SQLCadastroAuxiliar")] Tabela tabela)
        {
            if (ModelState.IsValid)
            {
                db.Tabela.Add(tabela);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tabela);
        }

        // GET: Tabelas/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tabela tabela = db.Tabela.Find(id);
            if (tabela == null)
            {
                return HttpNotFound();
            }
            return View(tabela);
        }

        // POST: Tabelas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Nome,Descricao,Ativo,CodigoEmpresa,CadastroAuxiliar,SQLCadastroAuxiliar")] Tabela tabela)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tabela).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tabela);
        }

        // GET: Tabelas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tabela tabela = db.Tabela.Find(id);
            if (tabela == null)
            {
                return HttpNotFound();
            }
            return View(tabela);
        }

        // POST: Tabelas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tabela tabela = db.Tabela.Find(id);
            db.Tabela.Remove(tabela);
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
