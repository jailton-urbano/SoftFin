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
    public class RelacionamentoPessoaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            var obj = new RelacionamentoPessoa();
            var totalRecords = obj.ObterTodos(_paramBase).Count();

            var response = new JqGridResponse()
            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in
                obj.ObterTodos(_paramBase).ToList())
            {

                
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.id,
                    item.descricao,
                    item.TipoRelacionamento.descricao,
                    item.PessoaMestre.nome,
                    item.Pessoa.nome,
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }



        [HttpPost]
        public ActionResult Create(RelacionamentoPessoa obj)
        {
            try
            {
                CarregaViewData();

                if (ModelState.IsValid)
                {
                    var modelo = new RelacionamentoPessoa();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    if (modelo.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Já cadastrado";
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

            ViewData["pessoa"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["pessoamestre"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["tiporelacionamento"] = new SelectList(new TipoRelacionamento().ObterTodos(_paramBase), "id", "Descricao");
        }
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();

                var obj = new RelacionamentoPessoa();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Delete(RelacionamentoPessoa obj)
        {
            try
            {
                CarregaViewData();
                string Erro = "";
                int idempresa  = _paramBase.empresa_id;
                obj.empresa_id = idempresa;
                if (!obj.Excluir(obj.id, ref Erro,_paramBase))
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
                var obj = new RelacionamentoPessoa();
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
        public ActionResult Edit(RelacionamentoPessoa obj)
        {
            try
            {
                CarregaViewData();
                var codigo = new RelacionamentoPessoa();
                codigo.Alterar(obj,_paramBase);

                ViewBag.msg = "Tipo Documento alterado com sucesso";
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
                var banco = new DbControle();
                var obj = new RelacionamentoPessoa();
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
