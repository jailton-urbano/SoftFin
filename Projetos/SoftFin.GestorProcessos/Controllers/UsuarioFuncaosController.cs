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
    public class UsuarioFuncaosController : BaseController
    {
        private DBGPControle db = new DBGPControle();

        // GET: UsuarioFuncaos
        public ActionResult Index()
        {
            var usuarioFuncao = db.UsuarioFuncao.Include(u => u.Funcao).Include(u => u.Usuario);
            return View(usuarioFuncao.ToList());
        }

        // GET: UsuarioFuncaos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioFuncao usuarioFuncao = db.UsuarioFuncao.Find(id);
            if (usuarioFuncao == null)
            {
                return HttpNotFound();
            }
            return View(usuarioFuncao);
        }

        // GET: UsuarioFuncaos/Create
        public ActionResult Create()
        {
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao");
            ViewBag.IdUsuario = new SelectList(db.Usuario, "Id", "Login");
            return View();
        }

        // POST: UsuarioFuncaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdUsuario,IdFuncao")] UsuarioFuncao usuarioFuncao)
        {
            if (ModelState.IsValid)
            {
                db.UsuarioFuncao.Add(usuarioFuncao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao", usuarioFuncao.IdFuncao);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "Id", "Login", usuarioFuncao.IdUsuario);
            return View(usuarioFuncao);
        }

        // GET: UsuarioFuncaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioFuncao usuarioFuncao = db.UsuarioFuncao.Find(id);
            if (usuarioFuncao == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao", usuarioFuncao.IdFuncao);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "Id", "Login", usuarioFuncao.IdUsuario);
            return View(usuarioFuncao);
        }

        // POST: UsuarioFuncaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdUsuario,IdFuncao")] UsuarioFuncao usuarioFuncao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioFuncao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdFuncao = new SelectList(db.Funcao, "Id", "Descricao", usuarioFuncao.IdFuncao);
            ViewBag.IdUsuario = new SelectList(db.Usuario, "Id", "Login", usuarioFuncao.IdUsuario);
            return View(usuarioFuncao);
        }

        // GET: UsuarioFuncaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioFuncao usuarioFuncao = db.UsuarioFuncao.Find(id);
            if (usuarioFuncao == null)
            {
                return HttpNotFound();
            }
            return View(usuarioFuncao);
        }

        // POST: UsuarioFuncaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioFuncao usuarioFuncao = db.UsuarioFuncao.Find(id);
            db.UsuarioFuncao.Remove(usuarioFuncao);
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
