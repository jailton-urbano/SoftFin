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
    public class LoteReembolsoController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new LoteDespesa();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["lote"] = item.codigo;
                myExport["Data"] = item.Data.ToString();
                myExport["Histórico"] = item.Historico;
                myExport["notadeDebito_id"] = item.nd_id;
                myExport["NotadeDebito"] = item.NotadeDebito.numero.ToString();
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "LoteReembolso.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new LoteDespesa();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
              string Valorid = Request.QueryString["id"]; 
              string Valorestabelecimento_id = Request.QueryString["estabelecimento_id"]; 
              string ValorEstabelecimento = Request.QueryString["Estabelecimento"]; 
              string Valorlote = Request.QueryString["lote"]; 
              string ValorData = Request.QueryString["Data"]; 
              string ValorStatus = Request.QueryString["Status"]; 
              string ValorEmitida = Request.QueryString["Emitida"]; 
              string ValornotadeDebito_id = Request.QueryString["notadeDebito_id"]; 
              string ValorNotadeDebito = Request.QueryString["NotadeDebito"]; 
            int totalRecords = 0;
            LoteDespesa obj = new LoteDespesa();
            var objs = new LoteDespesa().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valorid)) 
            {
              int aux ;
              int.TryParse(Valorid, out aux);
              objs = objs.Where(p => p.id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorestabelecimento_id)) 
            {
              int aux ;
              int.TryParse(Valorestabelecimento_id, out aux);
              objs = objs.Where(p => p.estabelecimento_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorStatus)) 
            {
              objs = objs.Where(p => p.Historico.Contains(ValorStatus)).ToList();
            }
            if (!String.IsNullOrEmpty(ValornotadeDebito_id)) 
            {
              int aux ;
              int.TryParse(ValornotadeDebito_id, out aux);
              objs = objs.Where(p => p.nd_id.Equals(aux)).ToList();
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
                    item.Data,
                    item.codigo,
                    item.NotadeDebito.numero
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoteDespesa obj)
        {
            try
            {
                CarregaViewData();
                int estab = _paramBase.estab_id;
                obj.estabelecimento_id = estab;

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
                var obj = new LoteDespesa();
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
        public ActionResult Edit(LoteDespesa obj)
        {
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
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
            LoteDespesa obj = new LoteDespesa();
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
        public ActionResult Delete(LoteDespesa obj)
        {
            try
            {
                CarregaViewData();
                string erro = "";
                if (obj.Excluir(obj.id,ref erro,_paramBase))
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
                LoteDespesa obj = new LoteDespesa();
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
            ViewData["estabelecimento"] = new SelectList(new Estabelecimento().ObterTodos(_paramBase), "id", "Apelido");
            ViewData["notadeDebito"] = new SelectList(new NotadeDebito().ObterTodos(_paramBase), "id", "numero");
        }
    }
}
