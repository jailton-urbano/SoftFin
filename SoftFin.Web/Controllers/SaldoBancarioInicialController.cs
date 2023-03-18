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
    public class SaldoBancarioInicialController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new SaldoBancarioInicial();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["Banco"] = item.Banco.contaCorrente;
                myExport["Ano"] = item.Ano;
                myExport["saldoInicial"] = item.saldoInicial;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "SaldoBancarioInicial.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new SaldoBancarioInicial();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
              string Valorid = Request.QueryString["id"]; 
              string Valorempresa_id = Request.QueryString["empresa_id"]; 
              string ValorEmpresa = Request.QueryString["Empresa"]; 
              string Valorbanco_id = Request.QueryString["banco_id"]; 
              string ValorBanco = Request.QueryString["Banco"]; 
              string ValorAno = Request.QueryString["Ano"]; 
              string ValorsaldoInicial = Request.QueryString["saldoInicial"]; 
            int totalRecords = 0;
            SaldoBancarioInicial obj = new SaldoBancarioInicial();
            var objs = new SaldoBancarioInicial().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valorid)) 
            {
              int aux ;
              int.TryParse(Valorid, out aux);
              objs = objs.Where(p => p.id.Equals(aux)).ToList();
            }
            //if (!String.IsNullOrEmpty(Valorempresa_id))
            //{
            //    int aux;
            //    int.TryParse(Valorempresa_id, out aux);
            //    objs = objs.Where(p => p.Banco.empresa_id.Equals(aux)).ToList();
            //}
            if (!String.IsNullOrEmpty(Valorbanco_id)) 
            {
              int aux ;
              int.TryParse(Valorbanco_id, out aux);
              objs = objs.Where(p => p.banco_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorAno)) 
            {
              objs = objs.Where(p => p.Ano.Contains(ValorAno)).ToList();
            }
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
                    item.Ano,
                    item.saldoInicial
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SaldoBancarioInicial obj)
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
            var obj = new SaldoBancarioInicial();
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
        public ActionResult Edit(SaldoBancarioInicial obj)
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
            SaldoBancarioInicial obj = new SaldoBancarioInicial();
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

        public ActionResult Delete(SaldoBancarioInicial obj)
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
            SaldoBancarioInicial obj = new SaldoBancarioInicial();
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
