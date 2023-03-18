using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SoftFin.Web.Controllers
{
    public class FuncionarioSalarioController : BaseController
    {
        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            //base.TotalizadorDash(id);
            var soma = new FuncionarioSalario().ObterTodos(_paramBase).Sum(p=>p.valorBruto).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Excel()
        {
            var obj = new FuncionarioSalario();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["Funcionario"] = item.Funcionario;
                myExport["funcionario_id"] = item.funcionario_id;
                myExport["dataInicial"] = item.dataInicial;
                myExport["valorBruto"] = item.valorBruto;
                myExport["valorAdiantamento"] = item.valorAdiantamento;
                myExport["valorComplemento"] = item.valorComplemento;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "FuncionarioSalario.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new FuncionarioSalario();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string Valornome = Request.QueryString["nome"]; 

            int totalRecords = 0;
            FuncionarioSalario obj = new FuncionarioSalario();
            var objs = new FuncionarioSalario().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valornome)) 
            {
                objs = objs.Where(p => p.Funcionario.Pessoa.nome.ToUpper().Contains(Valornome.ToUpper())).ToList();
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
                var data = "";
                if (item.dataInicial != null)
                    data = item.dataInicial.Value.ToShortDateString();

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Funcionario.Pessoa.nome,
                    item.Funcionario.Pessoa.cnpj,
                    data
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        private static List<FuncionarioSalario> Organiza(JqGridRequest request, List<FuncionarioSalario> objs)
        {
            switch (request.SortingName)
            {
                case "nome":
                   if (request.SortingOrder == JqGridSortingOrders.Desc)
                       objs = objs.OrderByDescending(p => p.Funcionario.Pessoa.nome).ToList();
                   else
                       objs = objs.OrderBy(p => p.Funcionario.Pessoa.nome).ToList();
                   break;
                case "cpf":
                   if (request.SortingOrder == JqGridSortingOrders.Desc)
                       objs = objs.OrderByDescending(p => p.Funcionario.Pessoa.cnpj).ToList();
                   else
                       objs = objs.OrderBy(p => p.Funcionario.Pessoa.cnpj).ToList();
                   break;
                case "data":
                   if (request.SortingOrder == JqGridSortingOrders.Desc)
                       objs = objs.OrderByDescending(p => p.dataInicial).ToList();
                   else
                       objs = objs.OrderBy(p => p.dataInicial).ToList();
                   break;
            }
             return objs;
          }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FuncionarioSalario obj)
        {
            try
            {
            CarregaViewData();
            //int estab = pb.estab_id;
            //obj.estabelecimento_id = estab;
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
            var obj = new FuncionarioSalario();
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
        public ActionResult Edit(FuncionarioSalario obj)
        {
            try
            {
            CarregaViewData();
            if (ModelState.IsValid)
            {
                //int estab = pb.estab_id;
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
            FuncionarioSalario obj = new FuncionarioSalario();
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
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FuncionarioSalario obj)
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
            FuncionarioSalario obj = new FuncionarioSalario();
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
            ViewData["funcionario"] = new Funcionario().ObterListaTodos(_paramBase);
        }
        public ActionResult Detail(int ID)
        {
            try
            {
                CarregaViewData();
                FuncionarioSalario obj = new FuncionarioSalario();
                obj = obj.ObterPorId(ID, _paramBase);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
    }
}