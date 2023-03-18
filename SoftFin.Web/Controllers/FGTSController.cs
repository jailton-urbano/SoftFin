using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Web.Regras;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FGTSController : BaseController
    {

        public ActionResult Excel()
        {
            var obj = new FGTS();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["codigoReceita"] = item.codigoReceita;
                myExport["tipoInscricao"] = item.tipoInscricao;
                myExport["cnpj"] = item.cnpj;
                myExport["codigoBarras"] = item.codigoBarras;
                myExport["identificadorFgts"] = item.identificadorFgts;
                myExport["lacreConectividadeSocial"] = item.lacreConectividadeSocial;
                myExport["digitoLacre"] = item.digitoLacre;
                myExport["nomeContribuinte"] = item.nomeContribuinte;
                myExport["dataPagamento"] = item.dataPagamento;
                myExport["valorPagamento"] = item.valorPagamento;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "FGTSVLW.csv");
        }
        public ActionResult Index()
        {
            //CarregaViewData();
            var obj = new FGTSVLW();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string ValorcodigoReceita = Request.QueryString["codigoReceita"];
            string ValortipoInscricao = Request.QueryString["tipoInscricao"];
            string Valorcnpj = Request.QueryString["cnpj"];
            string ValorcodigoBarras = Request.QueryString["codigoBarras"];
            string ValoridentificadorFgts = Request.QueryString["identificadorFgts"];
            string ValorlacreConectividadeSocial = Request.QueryString["lacreConectividadeSocial"];
            string ValordigitoLacre = Request.QueryString["digitoLacre"];
            string ValornomeContribuinte = Request.QueryString["nomeContribuinte"];
            string ValordataPagamentoInicial = Request.QueryString["dataPagamentoInicial"];
            string ValorvalorPagamentoInicial = Request.QueryString["valorPagamentoInicial"];
            string ValordataPagamentoFinal = Request.QueryString["dataPagamentoFinal"];
            string ValorvalorPagamentoFinal = Request.QueryString["valorPagamentoFinal"];
            int totalRecords = 0;
            var obj = new FGTS();
            var objs = new FGTS().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(ValorcodigoReceita))
            {
                int aux;
                int.TryParse(ValorcodigoReceita, out aux);
                objs = objs.Where(p => p.codigoReceita == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValortipoInscricao))
            {
                int aux;
                int.TryParse(ValortipoInscricao, out aux);
                objs = objs.Where(p => p.tipoInscricao == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valorcnpj))
            {
                objs = objs.Where(p => p.cnpj.Contains(Valorcnpj)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorcodigoBarras))
            {
                objs = objs.Where(p => p.codigoBarras.Contains(ValorcodigoBarras)).ToList();
            }
            if (!String.IsNullOrEmpty(ValoridentificadorFgts))
            {
                objs = objs.Where(p => p.identificadorFgts.Contains(ValoridentificadorFgts)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorlacreConectividadeSocial))
            {
                int aux;
                int.TryParse(ValorlacreConectividadeSocial, out aux);
                objs = objs.Where(p => p.lacreConectividadeSocial == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordigitoLacre))
            {
                int aux;
                int.TryParse(ValordigitoLacre, out aux);
                objs = objs.Where(p => p.digitoLacre == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValornomeContribuinte))
            {
                objs = objs.Where(p => p.nomeContribuinte.Contains(ValornomeContribuinte)).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataPagamentoInicial))
            {
                DateTime aux;
                DateTime.TryParse(ValordataPagamentoInicial, out aux);
                objs = objs.Where(p => p.dataPagamento >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorPagamentoInicial))
            {
                decimal aux;
                decimal.TryParse(ValorvalorPagamentoInicial, out aux);
                objs = objs.Where(p => p.valorPagamento >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataPagamentoFinal))
            {
                DateTime aux;
                DateTime.TryParse(ValordataPagamentoFinal, out aux);
                objs = objs.Where(p => p.dataPagamento <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorPagamentoFinal))
            {
                decimal aux;
                decimal.TryParse(ValorvalorPagamentoFinal, out aux);
                objs = objs.Where(p => p.valorPagamento <= aux).ToList();
            }
            Organiza(request, objs);
            totalRecords = objs.Count();
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList(); 
            
            foreach (var item in objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.codigoReceita,
                    item.tipoInscricao,
                    item.cnpj,
                    item.codigoBarras,
                    item.identificadorFgts,
                    item.lacreConectividadeSocial,
                    item.digitoLacre,
                    item.nomeContribuinte,
                    item.dataPagamento.ToShortDateString(),
                    item.valorPagamento.ToString("n")
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }


        private static List<FGTS> Organiza(JqGridRequest request, List<FGTS> objs)
        {
            switch (request.SortingName)
            {
            case "ID":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.ID).ToList();
               else
                   objs = objs.OrderBy(p => p.ID).ToList();
               break;
            case "codigoReceita":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.codigoReceita).ToList();
               else
                   objs = objs.OrderBy(p => p.codigoReceita).ToList();
               break;
            case "tipoInscricao":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.tipoInscricao).ToList();
               else
                   objs = objs.OrderBy(p => p.tipoInscricao).ToList();
               break;
            case "cnpj":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.cnpj).ToList();
               else
                   objs = objs.OrderBy(p => p.cnpj).ToList();
               break;
            case "codigoBarras":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.codigoBarras).ToList();
               else
                   objs = objs.OrderBy(p => p.codigoBarras).ToList();
               break;
            case "identificadorFgts":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.identificadorFgts).ToList();
               else
                   objs = objs.OrderBy(p => p.identificadorFgts).ToList();
               break;
            case "lacreConectividadeSocial":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.lacreConectividadeSocial).ToList();
               else
                   objs = objs.OrderBy(p => p.lacreConectividadeSocial).ToList();
               break;
            case "digitoLacre":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.digitoLacre).ToList();
               else
                   objs = objs.OrderBy(p => p.digitoLacre).ToList();
               break;
            case "nomeContribuinte":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.nomeContribuinte).ToList();
               else
                   objs = objs.OrderBy(p => p.nomeContribuinte).ToList();
               break;
            case "dataPagamento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.dataPagamento).ToList();
               else
                   objs = objs.OrderBy(p => p.dataPagamento).ToList();
               break;
            case "valorPagamento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.valorPagamento).ToList();
               else
                   objs = objs.OrderBy(p => p.valorPagamento).ToList();
               break;
            case "estabelecimento_id":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.estabelecimento_id).ToList();
               else
                   objs = objs.OrderBy(p => p.estabelecimento_id).ToList();
               break;
            case "Estabelecimento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.Estabelecimento).ToList();
               else
                   objs = objs.OrderBy(p => p.Estabelecimento).ToList();
               break;
             }
             return objs;
          }

        [HttpPost]
        public ActionResult Create(FGTS obj)
        {
            _eventos.Info("FGTS Create Post");
            try
            {
                if (ModelState.IsValid)
                {
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
                    FGTS banco = new FGTS();
                    if (banco.Incluir(obj, _paramBase) == true)
                    {
                        _eventos.Info("FGTS Create Post Json", obj);
                        ViewBag.msg = "FGTS incluída com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "FGTS já cadastrado";
                    }
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
        public ActionResult Create()
        {
            _eventos.Info("FGTS Create Post");
            var obj = new FGTS();
            return View(obj);
        }

        [HttpPost]
        public ActionResult Edit(FGTS obj)
        {
            _eventos.Info("FGTS Edit Post");
            try
            {
                if (ModelState.IsValid)
                {
                    FGTS banco = new FGTS();
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
                    if (banco.Alterar(obj, _paramBase))
                    {
                        _eventos.Info("FGTS Edit Post Json", obj);
                        ViewBag.msg = "FGTS alterada com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "Não foi possivel concluir a operação";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            _eventos.Info("FGTS Edit Get");
            FGTS obj = new FGTS();
            try
            {
                FGTS banco = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(banco);
                if (banco == null)
                    return RedirectToAction("/Index", "Erros");

                return View(banco);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(FGTS obj)
        {
            _eventos.Info("DARF Delete Post");
            try
            {
                FGTS obj2 = obj.ObterPorId(obj.ID, _paramBase);

                DbControle banco = new DbControle();
                FGTS bc = new FGTS();
                bc.Excluir(obj.ID, _paramBase);
                _eventos.Info("FGTS Delete Post Json", obj2);
                ViewBag.msg = "FGTS excluída com sucesso";
                return RedirectToAction("/Index");
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            _eventos.Info("FGTS Delete Get");
            FGTS obj = new FGTS();
            try
            {
                FGTS darf = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(darf);
                if (darf == null)
                    return RedirectToAction("/Index", "Erros");

                return View(darf);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Detail(int ID)
        {
            _eventos.Info("FGTS Delete Get");
            FGTS obj = new FGTS();
            try
            {
                FGTS darf = obj.ObterPorId(ID, _paramBase);
                if (darf == null)
                    return RedirectToAction("/Index", "Erros");

                return View(darf);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
    }
}
