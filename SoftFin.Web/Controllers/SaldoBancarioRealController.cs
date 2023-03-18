using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SoftFin.Web.Controllers
{
    public class SaldoBancarioRealController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new SaldoBancarioReal();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["Banco"] = item.Banco.contaCorrente;
                myExport["dataSaldo"] = item.dataSaldo.ToShortDateString();
                myExport["saldoFinal"] = item.saldoFinal.ToString("n");
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "SaldoBancarioReal.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new SaldoBancarioReal();
            obj.dataSaldo = DateTime.Now;
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
              string Valorbanco_id = Request.QueryString["banco"];
              string ValordataSaldo = Request.QueryString["datainicial"];
              string ValorsaldoFinal = Request.QueryString["datafinal"]; 
            int totalRecords = 0;
            SaldoBancarioReal obj = new SaldoBancarioReal();
            var objs = new SaldoBancarioReal().ObterTodos(_paramBase);
            
            if (!String.IsNullOrEmpty(Valorbanco_id))
            {
                int aux;
                int.TryParse(Valorbanco_id, out aux);
                objs = objs.Where(p => p.banco_id.Equals(aux)).ToList();
            }

            if (!String.IsNullOrEmpty(ValordataSaldo))
            {
                DateTime aux;
                DateTime.TryParse(ValordataSaldo, out aux);

                objs = objs.Where(p => p.dataSaldo >= aux).ToList();
            }

            if (!String.IsNullOrEmpty(ValorsaldoFinal))
            {
                DateTime aux;
                DateTime.TryParse(ValorsaldoFinal, out aux);

                objs = objs.Where(p => p.dataSaldo <= aux).ToList();
            }

            objs = Organiza(request, objs);
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
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    String.Format("{0} - {1} - {2} - {3}", item.Banco.codigoBanco, item.Banco.nomeBanco, item.Banco.agencia, item.Banco.contaCorrente)
                    ,
                    item.dataSaldo.ToShortDateString(),
                    item.saldoFinal.ToString("n")
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        private static List<SaldoBancarioReal> Organiza(JqGridRequest request, List<SaldoBancarioReal> objs)
        {
            switch (request.SortingName)
            {

                case "Banco":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Banco.nomeBanco).ToList();
                    else
                        objs = objs.OrderBy(p => p.Banco.nomeBanco).ToList();
                    break;
                case "dataSaldo":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.dataSaldo).ToList();
                    else
                        objs = objs.OrderBy(p => p.dataSaldo).ToList();
                    break;
                case "Saldo":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.saldoFinal).ToList();
                    else
                        objs = objs.OrderBy(p => p.saldoFinal).ToList();
                    break;
            }
            return objs;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SaldoBancarioReal obj)
        {
            try
            {
            CarregaViewData();

            //int idempresa  = pb.empresa_id;
            //obj.empresa_id = idempresa;
            if (ModelState.IsValid)
            {
                if (obj.Incluir(_paramBase))
                {
                    ViewBag.msg = "Incluído com sucesso";
                    return View(obj);
                }
                else
                {
                    ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
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
            try
            {
            CarregaViewData();
            var obj = new SaldoBancarioReal();
            obj.dataSaldo = DateTime.Now;
            return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SaldoBancarioReal obj)
        {
            try
            {
            CarregaViewData();
            if (ModelState.IsValid)
            {

            //int idempresa  = pb.empresa_id;
            //obj.empresa_id = idempresa;
                if (obj.Alterar(_paramBase))
                {
                    ViewBag.msg = "Alterado com sucesso";
                    return View(obj);
                }
                else
                {
                    ViewBag.msg = "Impossivel alterar, registro excluído";
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
        public ActionResult Edit(int ID)
        {
            try
            {
            CarregaViewData();
            SaldoBancarioReal obj = new SaldoBancarioReal();
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
        [HttpPost]

        public ActionResult Delete(SaldoBancarioReal obj)
        {
            try
            {
            CarregaViewData();
            string erro = "";
            if (obj.Excluir(ref erro, _paramBase))
            {
                ViewBag.msg = "Excluido com sucesso";
                return RedirectToAction("/Index");
            }
            else
            {
                ViewBag.msg = erro;
                return View(obj);
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
            SaldoBancarioReal obj = new SaldoBancarioReal();
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
        private void CarregaViewData()
        {
            CarregaBanco();
        }

        private void CarregaBanco()
        {

            var con1 = new Banco().ObterTodos( _paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            ViewData["banco"] = listret;
        }
    }
}
