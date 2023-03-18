using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class PerfilController : BaseController
    {
        DbControle db = new DbControle();
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            try
            {
                int totalRecords = new Perfil().ObterTodos(_paramBase).Count();

                //Fix for grouping, because it adds column name instead of index to SortingName
                //string sortingName = "cdg_perfil";

                //Prepare JqGridData instance
                JqGridResponse response = new JqGridResponse()
                {
                    //Total pages count
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    //Page number
                    PageIndex = request.PageIndex,
                    //Total records count
                    TotalRecordsCount = totalRecords
                };
                //Table with rows data
                foreach (var item in
                    new Perfil().ObterTodos(_paramBase) )
                {
                    response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                    {
                        item.id,
                        item.Descricao
                    }));
                }


                //Return data as json
                return new JqGridJsonResult() { Data = response };

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index","Erros");
            }
        }
        //
        // GET: /Perfil/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Perfil/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Perfil/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var novoPerfil = new Perfil();
            try
            {
                
                var descricao = collection["descricao"].ToString();

                novoPerfil.Descricao = descricao;
                novoPerfil.empresa_id  = _paramBase.empresa_id;
                db.Perfil.Add(novoPerfil);

                if (ModelState.IsValid)
                    db.SaveChanges();
                else
                    return View(novoPerfil);

                foreach (var item in collection)
                {
                    if (!item.Equals("txtPerfil"))
                    {
                        string valor = collection[item.ToString()];

                        if (valor.Equals("T") || valor.Equals("C"))
                        {
                            int idFuncionalidade ; 
                            var y = item.ToString().Split('$');
                            idFuncionalidade = int.Parse(y[1]);
                            var novoPerfilFunc = new PerfilFuncionalidade();
                            novoPerfilFunc.idFuncionalidade = idFuncionalidade;
                            novoPerfilFunc.flgTipoAcesso = valor;
                            novoPerfilFunc.idPerfil = novoPerfil.id;
                            db.PerfilFuncionalidade.Add(novoPerfilFunc);
                        }

                    }
                }
                db.SaveChanges();
                // TODO: Add insert logic here

                return RedirectToAction("/Index");
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        //
        // GET: /Perfil/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                return View(db.Perfil.Where(p => p.id == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        //
        // POST: /Perfil/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {

                var edit = db.Perfil.Where(p => p.id == id).FirstOrDefault();

                var descricao = collection["descricao"].ToString();

                edit.Descricao = descricao;
                db.SaveChanges();

                foreach (var item in collection)
                {
                    if (!item.Equals("descricao"))
                    {
                        string valor = collection[item.ToString()];

                        int idFuncionalidade;
                        var y = item.ToString().Split('$');

                        if (y.Count() == 2)
                        {
                            idFuncionalidade = int.Parse(y[1]);

                            var editPerfil = db.PerfilFuncionalidade.Where(p => p.idPerfil == id && p.idFuncionalidade == idFuncionalidade).FirstOrDefault();

                            if (editPerfil == null)
                            {
                                if (valor.Equals("T") || valor.Equals("C"))
                                {
                                    var novoPerfilFunc = new PerfilFuncionalidade();
                                    novoPerfilFunc.idFuncionalidade = idFuncionalidade;
                                    novoPerfilFunc.flgTipoAcesso = valor;
                                    novoPerfilFunc.idPerfil = id;
                                    db.PerfilFuncionalidade.Add(novoPerfilFunc);
                                }
                            }
                            else
                            {
                                if (valor.Equals("T") || valor.Equals("C"))
                                {
                                    editPerfil.idFuncionalidade = idFuncionalidade;
                                    editPerfil.flgTipoAcesso = valor;
                                    editPerfil.idPerfil = id;
                                }
                                else
                                {
                                    db.PerfilFuncionalidade.Remove(editPerfil);
                                }
                            }
                            db.SaveChanges();
                        }

                    }
                }
                
                // TODO: Add insert logic here

                return RedirectToAction("/Index");
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index","Erros");
            }
        }

        //
        // GET: /Perfil/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                return View(db.Perfil.Where(p => p.id == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        //
        // POST: /Perfil/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var y = db.PerfilFuncionalidade.Where(p => p.Funcionalidade.id == id);

                foreach (var item in y)
	            {
                    db.PerfilFuncionalidade.Remove(item);
	            }

                var x = db.Perfil.Where(p => p.id == id).FirstOrDefault();
                db.Perfil.Remove(x);

                db.SaveChanges();
                return RedirectToAction("/Index");
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
    }
}
