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
    public class FolhaPagamentoTipoController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new FolhaPagamentoTipo();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();

                myExport["descricao"] = item.descricao;

            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "FolhaPagamentoTipo.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new FolhaPagamentoTipo();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {

            int totalRecords = 0;
            FolhaPagamentoTipo obj = new FolhaPagamentoTipo();
            var objs = new FolhaPagamentoTipo().ObterTodos(_paramBase);

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
                    item.descricao
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FolhaPagamentoTipo obj)
        {
            try
            {
            CarregaViewData();
            //int estab = pb.estab_id;
            //obj.estabelecimento_id = estab;

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
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Create()
        {
            try
            {
            CarregaViewData();
            var obj = new FolhaPagamentoTipo();
            return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FolhaPagamentoTipo obj)
        {
            try
            {
            CarregaViewData();
            if (ModelState.IsValid)
            {
            //int estab = pb.estab_id;
            //obj.estabelecimento_id = estab;

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
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            try
            {
            CarregaViewData();
            FolhaPagamentoTipo obj = new FolhaPagamentoTipo();
            obj = obj.ObterPorId(ID, _paramBase);
            SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
            return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        [HttpPost]
        public ActionResult Delete(FolhaPagamentoTipo obj)
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
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            try
            {
            CarregaViewData();
            FolhaPagamentoTipo obj = new FolhaPagamentoTipo();
            obj = obj.ObterPorId(ID, _paramBase);
            SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
            return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        private void CarregaViewData()
        {
            //ViewData["estabelecimento"] = new SelectList(new estabelecimento().ObterTodos(), "id", "descricao");
        }
    }
}
