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
    public class OrigemMovimentoController : BaseController
    {
        //

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            
            
            
            var objs = new OrigemMovimento().ObterTodos(_paramBase);






            var totalRecords = objs.Count();

            var response = new JqGridResponse()
            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in
                objs)
            {

                
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Modulo,
                    item.Tipo,
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }



        [HttpPost]
        public ActionResult Create(OrigemMovimento obj)
        {
            try
            {
                CarregaViewData();

                if (ModelState.IsValid)
                {
                    var modelo = new OrigemMovimento();


                    if (modelo.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Origem Movimento incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Origem Movimento já cadastrado";
                        return View(obj);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados Invalidos");
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        private void CarregaViewData()
        {
            DbControle banco = new DbControle();
            var con = from c in banco.OrigemMovimento select c;
            ViewData["OrigemMovimento"] = new SelectList(con, "id", "Descricao");
        }
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();

                var obj = new OrigemMovimento();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        public ActionResult Delete(OrigemMovimento obj)
        {
            
            try
            {
                CarregaViewData();
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    ViewBag.msg = Erro;
                    obj = obj.ObterPorId(obj.id, _paramBase);
                    return View(obj);
                }
                else
                {
                    return RedirectToAction("/Index");
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            try{
                CarregaViewData();
                var obj = new OrigemMovimento();
                var cs = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(cs);
                return View(cs);

            }            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Edit(OrigemMovimento obj)
        {
            try{
            CarregaViewData();
            var codigo = new OrigemMovimento();

            codigo.Alterar(obj, _paramBase);

            ViewBag.msg = "Origem Movimento alterado com sucesso";
            return RedirectToAction("/Index");
            }
                        catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        public ActionResult Edit(int ID)
        {
            CarregaViewData();
            var banco = new DbControle();
            var obj = new OrigemMovimento();
            obj = banco.OrigemMovimento.Where(x => x.id == ID).FirstOrDefault();
            return View(obj);
        }
    }
}
