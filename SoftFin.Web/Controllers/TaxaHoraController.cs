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
    public class TaxaHoraController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            IEnumerable<TaxaHora> lista;
            TaxaHora obj = new TaxaHora();
            lista = obj.ObterTodos(_paramBase);

            int totalRecords = obj.ObterTodos(_paramBase).Count();

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
                obj.ObterTodos(_paramBase).ToList())
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.categoriaProfissional.categoria,
                    item.descricao,
                    item.dataValidade.ToShortDateString(),
                    item.taxaHoraVenda.ToString("n"),
                    item.taxaHoraCusto.ToString("n")
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(TaxaHora obj, string dataValidade)
        {
            CarregaViewData();
            try
            {
                if (ModelState.IsValid)
                {
                    TaxaHora taxa = new TaxaHora();

                    DateTime DataValidade = new DateTime();
                    DataValidade = DateTime.Parse(dataValidade);
                    obj.dataValidade = DataValidade;
                    
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;

                    if (taxa.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Taxa Hora incluída com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Taxa Hora já cadastrada";
                        return View(obj);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Create()
        {
            CarregaViewData();
            try
            {
                var obj = new TaxaHora();
                obj.dataValidade = DateTime.Now.AddYears(1);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(TaxaHora obj)
        {
            CarregaViewData();
            try
            {
                if (ModelState.IsValid)
                {
                    TaxaHora taxa = new TaxaHora();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    taxa.Alterar(obj, _paramBase);
                    ViewBag.msg = "Taxa Hora alterada";
                    return View(obj);
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);

                }
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
            try
            {
                TaxaHora obj = new TaxaHora();
                TaxaHora taxa = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(taxa);
                return View(taxa);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }


        [HttpPost]
        public ActionResult Delete(TaxaHora obj)
        {
            CarregaViewData();
            try
            {
                //CarregaViewData();
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
            CarregaViewData();
            try
            {
                TaxaHora obj = new TaxaHora();
                TaxaHora taxa = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(taxa);
                return View(taxa);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private void CarregaViewData()
        {
            CategoriaProfissional cp = new CategoriaProfissional();
            ViewData["categoria"] = new SelectList(cp.ObterTodos(_paramBase), "id", "categoria");
        }
    }
}
