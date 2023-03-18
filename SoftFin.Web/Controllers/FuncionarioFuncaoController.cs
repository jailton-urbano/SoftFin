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
    public class FuncionarioFuncaoController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new FuncionarioFuncao();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["nome"] = item.nome;
                myExport["descricao"] = item.descricao;
                myExport["valorpiso"] = item.valorpiso;

            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "FuncionarioFuncao.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new FuncionarioFuncao();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {

            int totalRecords = 0;
            FuncionarioFuncao obj = new FuncionarioFuncao();
            var objs = new FuncionarioFuncao().ObterTodos(_paramBase);

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
                    item.nome,
                    item.descricao,
                    item.valorpiso
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FuncionarioFuncao obj)
        {
            try
            {
            CarregaViewData();
            int estab = _paramBase.estab_id;
            obj.estabelecimento_id = estab;
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
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Create()
        {
            try
            {
            CarregaViewData();
            var obj = new FuncionarioFuncao();
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
        public ActionResult Edit(FuncionarioFuncao obj)
        {
            try
            {
            CarregaViewData();
            if (ModelState.IsValid)
            {
            //int estab = _paramBase.estab_id;
            //obj.estabelecimento_id = estab;
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
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            try
            {
            CarregaViewData();
            FuncionarioFuncao obj = new FuncionarioFuncao();
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
        public ActionResult Delete(FuncionarioFuncao obj)
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
            FuncionarioFuncao obj = new FuncionarioFuncao();
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
