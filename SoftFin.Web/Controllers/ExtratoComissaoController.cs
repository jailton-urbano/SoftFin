using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ExtratoComissaoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            var obj = new ExtratoComissao();

            var objs = obj.ObterTodos(_paramBase);

            var totalRecords = objs.Count();

            var response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            objs = Organiza(request, objs);

            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

            foreach (var item in
                objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.id,
                    item.Seguradora,
                    item.Extrato.ToString(),
                    item.DataExtrato.Value.ToShortDateString(),
                    item.DataCredito.Value.ToShortDateString(),
                    item.DataApropriacao.Value.ToShortDateString(),
                    item.ComissaoBruta.ToString("N2"),
                    item.IRRF.ToString("N2"),
                    item.ISS.ToString("N2"),
                    item.ComissaoLiquida.ToString("N2"),
                    item.Observacao
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        private static List<ExtratoComissao> Organiza(JqGridRequest request, List<ExtratoComissao> objs)
        {
            switch (request.SortingName)
            {
                case "Seguradora":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Seguradora).ToList();
                    else
                        objs = objs.OrderBy(p => p.Seguradora).ToList();
                    break;
                case "Extrato":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Extrato).ToList();
                    else
                        objs = objs.OrderBy(p => p.Extrato).ToList();
                    break;
                case "DataExtrato":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.DataExtrato).ToList();
                    else
                        objs = objs.OrderBy(p => p.DataExtrato).ToList();
                    break;
                case "DataCredito":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.DataCredito).ToList();
                    else
                        objs = objs.OrderBy(p => p.DataCredito).ToList();
                    break;
                case "DataApropriacao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.DataApropriacao).ToList();
                    else
                        objs = objs.OrderBy(p => p.DataApropriacao).ToList();
                    break;
                case "ComissaoBruta":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.ComissaoBruta).ToList();
                    else
                        objs = objs.OrderBy(p => p.ComissaoBruta).ToList();
                    break;
                case "IRRF":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.IRRF).ToList();
                    else
                        objs = objs.OrderBy(p => p.IRRF).ToList();
                    break;
                case "ISS":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.ISS).ToList();
                    else
                        objs = objs.OrderBy(p => p.ISS).ToList();
                    break;
                case "ComissaoLiquida":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.ComissaoLiquida).ToList();
                    else
                        objs = objs.OrderBy(p => p.ComissaoLiquida).ToList();
                    break;
                case "Observacao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Observacao).ToList();
                    else
                        objs = objs.OrderBy(p => p.Observacao).ToList();
                    break;
            }
            return objs;
        }


        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            int estab = _paramBase.estab_id;

            var arquivo = Request.Files[0];

            if (arquivo != null && arquivo.ContentLength > 0)
            {
                var fileName = Path.GetFileName(arquivo.FileName);


                if (fileName.ToUpper().IndexOf(".XLS") == -1)
                {
                    ViewBag.msg = "Arquivo invalido.";
                    return View();
                }

                var path = Path.Combine(Server.MapPath("~/TXTTemp/"), fileName);
                arquivo.SaveAs(path);

                new ExtratoComissao().ImportaExcel(path, _paramBase);

                ViewBag.msg = "Importação Finalizada com sucesso";
            }


            return View();
        }

        [HttpPost]
        public ActionResult Create(ExtratoComissao obj)
        {
            try
            {
                CarregaViewData();

                if (ModelState.IsValid)
                {
                    var modelo = new ExtratoComissao();
                    int idempresa = _paramBase.estab_id;
                    obj.estabelecimento_id = idempresa;
                    if (modelo.Incluir(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Tipo Documento incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Tipo Documento já cadastrado";
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
            
            ViewData["ExtratoComissao"] = new SelectList(new ExtratoComissao().ObterTodos(_paramBase), "id", "Descricao");
        }
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();

                var obj = new ExtratoComissao();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }
        [HttpPost]
        public ActionResult Delete(ExtratoComissao obj)
        {
            try
            {
                CarregaViewData();
                string Erro = "";
                int idempresa = _paramBase.estab_id;
                obj.estabelecimento_id = idempresa;
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
                var obj = new ExtratoComissao();
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
        public ActionResult Edit(ExtratoComissao obj)
        {
            try
            {
                CarregaViewData();
                var codigo = new ExtratoComissao();
                codigo.Alterar(obj, _paramBase);

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
                var obj = new ExtratoComissao();
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
