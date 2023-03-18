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
    public class AtendimentoTipoHistoricoController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new AtendimentoTipoHistorico();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["descricao"] = item.descricao;

            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "AtendimentoTipoHistorico.csv");
        }
        public ActionResult Index()
        {
            //CarregaViewData();
            //var obj = new AtendimentoTipoHistorico();
            return View();
        }
        public ActionResult Listas(JqGridRequest request)
        {
              string Valorid = Request.QueryString["id"]; 
              string Valorempresa_id = Request.QueryString["empresa_id"]; 
              string ValorEmpresa = Request.QueryString["Empresa"]; 
              string Valordescricao = Request.QueryString["descricao"]; 
            int totalRecords = 0;
            AtendimentoTipoHistorico obj = new AtendimentoTipoHistorico();
            var objs = new AtendimentoTipoHistorico().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valorid)) 
            {
              int aux ;
              int.TryParse(Valorid, out aux);
              objs = objs.Where(p => p.id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valordescricao)) 
            {
              objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
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
                    item.id,
                    item.descricao
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AtendimentoTipoHistorico obj)
        {
            try
            {
                //CarregaViewData();
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
                var obj = new AtendimentoTipoHistorico();
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
        public ActionResult Edit(AtendimentoTipoHistorico obj)
        {
            try
            {
            CarregaViewData();
            if (ModelState.IsValid)
            {

                if (obj.Alterar(obj, _paramBase))
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
            AtendimentoTipoHistorico obj = new AtendimentoTipoHistorico();
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
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AtendimentoTipoHistorico obj)
        {
            try
            {
            CarregaViewData();
            if (obj.Excluir(obj.id, _paramBase))
            {
                ViewBag.msg = "Excluido com sucesso";
                return RedirectToAction("/Index");
            }
            else
            {
                ViewBag.msg = "Não foi possivel excluir";
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
                AtendimentoTipoHistorico obj = new AtendimentoTipoHistorico();
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
            ViewData["empresa"] = new SelectList(new Empresa().ObterTodos(_paramBase), "id", "apelido");
        }
    }
}
