using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class DocumentoPagarAprovacaoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            var obj = new DocumentoPagarAprovacao();
            var totalRecords = obj.ObterTodos(_paramBase).Count();

            var response = new JqGridResponse()
            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in
                obj.ObterTodos(_paramBase))
            {

                
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Pessoa.nome,
                    item.Pessoa.nome
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }



        [HttpPost]
        public ActionResult Create(DocumentoPagarAprovacao obj)
        {
            try
            {
                CarregaViewData();

                if (ModelState.IsValid)
                {
                    var modelo = new DocumentoPagarAprovacao();
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
                    if (modelo.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Lançamento já cadastrado";
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
            ViewData["Pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
        }

        public ActionResult Create()
        {
            try
            {
                CarregaViewData();

                var obj = new DocumentoPagarAprovacao();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Delete(DocumentoPagarAprovacao obj)
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
            try
            {
                CarregaViewData();
                var obj = new DocumentoPagarAprovacao();
                var cs = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(cs);
                return View(cs);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Edit(DocumentoPagarAprovacao obj)
        {
            try
            {
                CarregaViewData();
                var codigo = new DocumentoPagarAprovacao();
                int estab = _paramBase.estab_id;
                obj.estabelecimento_id = estab;

                codigo.Alterar(obj, _paramBase);

                ViewBag.msg = "Alterado com sucesso";
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
            try
            {
                CarregaViewData();
                var obj = new DocumentoPagarAprovacao();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
    }
}
