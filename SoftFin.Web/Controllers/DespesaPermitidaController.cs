using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class DespesaPermitidaController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new DespesaPermitida();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["Projeto"] = item.Projeto.nomeProjeto;
                myExport["descricao"] = item.descricao;
                myExport["sequencia"] = item.Aprovador;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "DespesaPermitida.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData(0);
            var obj = new DespesaPermitida();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string Valordescricao = Request.QueryString["descricao"];
            string Valorprojeto_id = Request.QueryString["projeto_id"];

            int totalRecords = 0;
            DespesaPermitida obj = new DespesaPermitida();
            var objs = new DespesaPermitida().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valordescricao))
            {
                objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorprojeto_id))
            {
                int aux;
                int.TryParse(Valorprojeto_id, out aux);
                objs = objs.Where(p => p.projeto_id == aux).ToList();
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
                string tipoDespesa = "";
                if (item.tipodespesa_id == 0)
                {
                    tipoDespesa = new TipoDespesa().ObterPorId(item.tipodespesa_id, _paramBase).descricao;
                }

                string aprovador = "";
                if (item.aprovador_id == 0)
                {
                    aprovador = new Usuario().ObterPorId(item.aprovador_id).nome;
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    tipoDespesa,
                    aprovador,
                    item.valorLimite,
                    item.cobravel,
                    item.reembolsavel,
                    item.usarpadrao,
                    item.descricao,
                    item.valorPadrao
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        private static List<DespesaPermitida> Organiza(JqGridRequest request, List<DespesaPermitida> objs)
        {
            switch (request.SortingName)
            {
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
                case "descricao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.descricao).ToList();
                    break;
                case "projeto_id":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Projeto.nomeProjeto).ToList();
                    else
                        objs = objs.OrderBy(p => p.Projeto.nomeProjeto).ToList();
                    break;
             }
             return objs;
          }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DespesaPermitida obj)
        {
            try
            {
                CarregaViewData(obj.projeto_id); 

                obj.estabelecimento_id = _estab;

                if (ModelState.IsValid)
                {
                    var db = new DbControle();
                    var DespesaPermitida = new DespesaPermitida();

                    var estab = _paramBase.estab_id;

                    DespesaPermitida.estabelecimento_id = estab;
                    DespesaPermitida.aprovador_id = obj.aprovador_id;
                    DespesaPermitida.descricao = obj.descricao;
                    DespesaPermitida.projeto_id = obj.projeto_id;
                    DespesaPermitida.cobravel = obj.cobravel;
                    DespesaPermitida.reembolsavel = obj.reembolsavel;
                    DespesaPermitida.tipodespesa_id = obj.tipodespesa_id;
                    DespesaPermitida.usarpadrao = obj.usarpadrao;
                    DespesaPermitida.valorLimite = obj.valorLimite;
                    DespesaPermitida.valorPadrao = obj.valorPadrao;

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        if (DespesaPermitida.Incluir(_paramBase))
                        {
                            dbcxtransaction.Commit();
                            CarregaViewData(obj.projeto_id); 

                            ViewBag.msg = "Incluído com sucesso";
                            return View(obj);
                        }
                        else
                        {
                            dbcxtransaction.Rollback();
                            ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                            return View(obj);
                        }
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
        public ActionResult Create(int id)
        {
            try
            {
                var obj = new DespesaPermitida();
                obj.estabelecimento_id = _estab;
                obj.Projeto = new Projeto().ObterPorId(id, _paramBase);
                obj.projeto_id = obj.Projeto.id;
                CarregaViewData(obj.projeto_id);

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
        public ActionResult Edit(DespesaPermitida obj)
        {
            try
            {
                CarregaViewData(obj.projeto_id); 
                if (ModelState.IsValid)
                {
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
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
                DespesaPermitida obj = new DespesaPermitida();
                obj = obj.ObterPorId(ID, _paramBase);
                CarregaViewData(obj.projeto_id);
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
        public ActionResult Delete(DespesaPermitida obj)
        {
            try
            {
            string erro = "";
            if (obj.Excluir(obj.id,ref erro, _paramBase))
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
                DespesaPermitida obj = new DespesaPermitida();
                obj = obj.ObterPorId(ID, _paramBase);
                CarregaViewData(obj.projeto_id);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private void CarregaViewData(int projetoid)
        {
            if (projetoid.Equals(0))
            {
                ViewData["projeto"] = new Projeto().CarregaProjeto(_paramBase);
            }
            else
            {
                ViewData["projeto"] = new Projeto().CarregaProjetoID(projetoid, _paramBase);
            }
                ViewData["tipoDespesa"] = new SelectList(new TipoDespesa().ObterTodos(_paramBase), "id", "descricao");
                ViewData["aprovador"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase).Where(p => p.usuarioBloqueado == false), "id", "nome");
        }
        
    }
}
