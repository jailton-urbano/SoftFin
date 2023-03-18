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
    public class GPSController : BaseController
    {


        //GPS
        public ActionResult Index()
        {
            _eventos.Info("GPS Index");
            return View(new GPSVLW());
        }

        public ActionResult Excel()
        {
            var obj = new GPS();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["codigoPagamento"] = item.codigoPagamento;
                myExport["competencia"] = item.competencia;
                myExport["identificador"] = item.identificador;
                myExport["valorTributo"] = item.valorTributo;
                myExport["valorOutrasEntidades"] = item.valorOutrasEntidades;
                myExport["valorAtualizacaoMonetaria"] = item.valorAtualizacaoMonetaria;
                myExport["valorArrecadado"] = item.valorArrecadado;
                myExport["dataArrecadacao"] = item.dataArrecadacao;
                myExport["informacoesComplementares"] = item.informacoesComplementares;
                myExport["nomeContribuinte"] = item.nomeContribuinte;

            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "GPS.csv");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            string ValorcodigoPagamento = Request.QueryString["codigoPagamento"];
            string Valorcompetencia = Request.QueryString["competencia"];
            string Valoridentificador = Request.QueryString["identificador"];
            string ValorvalorTributoInicial = Request.QueryString["valorTributoInicial"];
            string ValorvalorOutrasEntidadesInicial = Request.QueryString["valorOutrasEntidadesInicial"];
            string ValorvalorAtualizacaoMonetariaInicial = Request.QueryString["valorAtualizacaoMonetariaInicial"];
            string ValorvalorArrecadadoInicial = Request.QueryString["valorArrecadadoInicial"];
            string ValordataArrecadacaoInicial = Request.QueryString["dataArrecadacaoInicial"];
            string ValorvalorTributoFinal = Request.QueryString["valorTributoFinal"];
            string ValorvalorOutrasEntidadesFinal = Request.QueryString["valorOutrasEntidadesFinal"];
            string ValorvalorAtualizacaoMonetariaFinal = Request.QueryString["valorAtualizacaoMonetariaFinal"];
            string ValorvalorArrecadadoFinal = Request.QueryString["valorArrecadadoFinal"];
            string ValordataArrecadacaoFinal = Request.QueryString["dataArrecadacaoFinal"];
            string ValorinformacoesComplementares = Request.QueryString["informacoesComplementares"];
            string ValornomeContribuinte = Request.QueryString["nomeContribuinte"]; 

 
            GPS obj = new GPS();
            var db = new DbControle();

            var objs = obj.ObterTodos(_paramBase);


            if (!String.IsNullOrEmpty(ValorcodigoPagamento))
            {
                int aux;
                int.TryParse(ValorcodigoPagamento, out aux);
                objs = objs.Where(p => p.codigoPagamento == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valorcompetencia))
            {
                objs = objs.Where(p => p.competencia.Contains(Valorcompetencia)).ToList();
            }
            if (!String.IsNullOrEmpty(Valoridentificador))
            {
                objs = objs.Where(p => p.identificador == Valoridentificador).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorTributoInicial))
            {
                decimal aux;
                decimal.TryParse(ValorvalorTributoInicial, out aux);
                objs = objs.Where(p => p.valorTributo >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorOutrasEntidadesInicial))
            {
                decimal aux;
                decimal.TryParse(ValorvalorOutrasEntidadesInicial, out aux);
                objs = objs.Where(p => p.valorOutrasEntidades >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorAtualizacaoMonetariaInicial))
            {
                decimal aux;
                decimal.TryParse(ValorvalorAtualizacaoMonetariaInicial, out aux);
                objs = objs.Where(p => p.valorAtualizacaoMonetaria >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorArrecadadoInicial))
            {
                decimal aux;
                decimal.TryParse(ValorvalorArrecadadoInicial, out aux);
                objs = objs.Where(p => p.valorArrecadado >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataArrecadacaoInicial))
            {
                DateTime aux;
                DateTime.TryParse(ValordataArrecadacaoInicial, out aux);
                objs = objs.Where(p => p.dataArrecadacao >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorTributoFinal))
            {
                decimal aux;
                decimal.TryParse(ValorvalorTributoFinal, out aux);
                objs = objs.Where(p => p.valorTributo <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorOutrasEntidadesFinal))
            {
                decimal aux;
                decimal.TryParse(ValorvalorOutrasEntidadesFinal, out aux);
                objs = objs.Where(p => p.valorOutrasEntidades <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorAtualizacaoMonetariaFinal))
            {
                decimal aux;
                decimal.TryParse(ValorvalorAtualizacaoMonetariaFinal, out aux);
                objs = objs.Where(p => p.valorAtualizacaoMonetaria <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorArrecadadoFinal))
            {
                decimal aux;
                decimal.TryParse(ValorvalorArrecadadoFinal, out aux);
                objs = objs.Where(p => p.valorArrecadado <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataArrecadacaoFinal))
            {
                DateTime aux;
                DateTime.TryParse(ValordataArrecadacaoFinal, out aux);
                objs = objs.Where(p => p.dataArrecadacao <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorinformacoesComplementares))
            {
                objs = objs.Where(p => p.informacoesComplementares.Contains(ValorinformacoesComplementares)).ToList();
            }
            if (!String.IsNullOrEmpty(ValornomeContribuinte))
            {
                objs = objs.Where(p => p.nomeContribuinte.Contains(ValornomeContribuinte)).ToList();
            }

            if (!String.IsNullOrEmpty(ValorcodigoPagamento))
            {
                int aux;
                int.TryParse(ValorcodigoPagamento, out aux);
                objs = objs.Where(p => p.codigoPagamento.Equals(aux)).ToList();
            }

            objs = Organiza(request, objs);
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

            var totalRecords = objs.Count();

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
            foreach (var item in objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.codigoPagamento,
                    item.competencia,
                    item.identificador,
                    item.valorTributo.ToString("n"),
                    item.valorOutrasEntidades.ToString("n"),
                    item.valorAtualizacaoMonetaria.ToString("n"),
                    item.valorArrecadado.ToString("n"),
                    item.dataArrecadacao.ToString("dd/MM/yyyy"),
                    item.informacoesComplementares,
                    item.nomeContribuinte
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }


        private static List<GPS> Organiza(JqGridRequest request, List<GPS> objs)
        {
            switch (request.SortingName)
            {

            case "CodigodePagamento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.codigoPagamento).ToList();
               else
                   objs = objs.OrderBy(p => p.codigoPagamento).ToList();
               break;
            case "Competencia":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.competencia).ToList();
               else
                   objs = objs.OrderBy(p => p.competencia).ToList();
               break;
            case "Identificador":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.identificador).ToList();
               else
                   objs = objs.OrderBy(p => p.identificador).ToList();
               break;
            case "ValordoTributo":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.valorTributo).ToList();
               else
                   objs = objs.OrderBy(p => p.valorTributo).ToList();
               break;
            case "ValorOutrasEntidades":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.valorOutrasEntidades).ToList();
               else
                   objs = objs.OrderBy(p => p.valorOutrasEntidades).ToList();
               break;
            case "ValorAtualizacaoMonetaria":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.valorAtualizacaoMonetaria).ToList();
               else
                   objs = objs.OrderBy(p => p.valorAtualizacaoMonetaria).ToList();
               break;
            case "ValorArrecadado":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.valorArrecadado).ToList();
               else
                   objs = objs.OrderBy(p => p.valorArrecadado).ToList();
               break;
            case "DatadaArrecadacao":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.dataArrecadacao).ToList();
               else
                   objs = objs.OrderBy(p => p.dataArrecadacao).ToList();
               break;
            case "InformacoesComplementares":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.informacoesComplementares).ToList();
               else
                   objs = objs.OrderBy(p => p.informacoesComplementares).ToList();
               break;
            case "NomedoContribuinte":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.nomeContribuinte).ToList();
               else
                   objs = objs.OrderBy(p => p.nomeContribuinte).ToList();
               break;

             }
             return objs;
          }

        [HttpPost]
        public ActionResult Create(GPS obj)
        {
            _eventos.Info("GPS Create Post");
            try
            {
                if (ModelState.IsValid)
                {
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
                    GPS banco = new GPS();
                    if (banco.Incluir(obj, _paramBase) == true)
                    {
                        _eventos.Info("GPS Create Post Json", obj);
                        ViewBag.msg = "GPS incluída com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "GPS já cadastrado";
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
            _eventos.Info("GPS Create Post");
            var obj = new GPS();
            obj.dataArrecadacao = DateTime.Now;
            return View(obj);
        }

        [HttpPost]
        public ActionResult Edit(GPS obj, ParamBase pb)
        {
            _eventos.Info("GPS Edit Post");
            try
            {
                if (ModelState.IsValid)
                {
                    GPS banco = new GPS();
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
                    if (banco.Alterar(obj, pb))
                    {
                        _eventos.Info("GPS Edit Post Json", obj);
                        ViewBag.msg = "GPS alterada com sucesso";
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
        public ActionResult Edit(int ID, ParamBase pb)
        {
            _eventos.Info("GPS Edit Get");
            GPS obj = new GPS();
            try
            {
                GPS banco = obj.ObterPorId(ID, pb);
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
        public ActionResult Delete(GPS obj)
        {
            _eventos.Info("GPS Delete Post");
            try
            {
                GPS obj2 = obj.ObterPorId(obj.id,_paramBase);

                DbControle banco = new DbControle();
                GPS bc = new GPS();
                bc.Excluir(obj.id, _paramBase);
                _eventos.Info("GPS Delete Post Json", obj2);
                ViewBag.msg = "GPS excluída com sucesso";
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
            _eventos.Info("GPS Delete Get");
            GPS obj = new GPS();
            try
            {
                GPS gps = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(gps);
                if (gps == null)
                    return RedirectToAction("/Index", "Erros");

                return View(gps);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Detail(int ID)
        {
            _eventos.Info("Detail Delete Get");
            GPS obj = new GPS();
            try
            {
                GPS gps = obj.ObterPorId(ID, _paramBase);
                if (gps == null)
                    return RedirectToAction("/Index", "Erros");

                return View(gps);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
    }
}
