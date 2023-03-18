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
    public class FuncionarioController : BaseController
    {
        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            //base.TotalizadorDash(id);
            var soma = new Funcionario().ObterTodos(_paramBase).Count().ToString();
            return Json(new { CDStatus = "OK", Result = soma }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Excel()
        {
            var obj = new Funcionario();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["Pessoa"] = item.Pessoa.nome;
                myExport["Responsavel"] = item.Responsavel.nome;

                if (item.dataAdmissao != null)
                    myExport["dataAdmissao"] = item.dataAdmissao.Value.ToShortDateString();
    
                myExport["UnidadeNegocio"] = item.UnidadeNegocio.unidade;

                if (item.dataSaida != null)
                    myExport["dataSaida"] = item.dataSaida.Value.ToShortDateString();
                
                if (item.dataNascimento != null)
                    myExport["dataNascimento"] = item.dataNascimento.Value.ToShortDateString();
                
                if (item.Funcao != null)
                    myExport["Funcao"] = item.Funcao.nome;

                myExport["profissao"] = item.profissao;
                myExport["estadocivil"] = item.estadocivil;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "Funcionario.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new Funcionario();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string ValorNome = Request.QueryString["nome"];
            string ValorUnidade = Request.QueryString["ddlUnidade"];
            string ValorFuncionarioFuncao = Request.QueryString["ddlFuncionarioFuncao"]; 

            
            int totalRecords = 0;
            Funcionario obj = new Funcionario();
            var objs = new Funcionario().ObterTodos(_paramBase);

            if (!String.IsNullOrEmpty(ValorNome))
            {
                objs = objs.Where(p => p.Pessoa.nome.Contains(ValorNome)).ToList();
            }

            if (!String.IsNullOrEmpty(ValorUnidade))
            {
                int aux;
                int.TryParse(ValorUnidade, out aux);
                objs = objs.Where(p => p.unidadeNegocio_id == aux).ToList();
            }

            if (!String.IsNullOrEmpty(ValorFuncionarioFuncao))
            {
                int aux;
                int.TryParse(ValorFuncionarioFuncao, out aux);
                objs = objs.Where(p => p.funcao_id == aux).ToList();
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
                var unidade = "";
                var funcao = "";
                var datasaida = "";



                if (item.dataAdmissao != null)
                    data = item.dataAdmissao.Value.ToShortDateString();
                unidade = item.UnidadeNegocio.unidade;
                
                if (item.Funcao != null)
                    funcao = item.Funcao.nome;

                if (item.dataSaida != null)
                    datasaida = item.dataSaida.Value.ToShortDateString();


                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Pessoa.nome,
                    item.Pessoa.cnpj,
                    data, 
                    unidade,
                    funcao,
                    datasaida

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        private static List<Funcionario> Organiza(JqGridRequest request, List<Funcionario> objs)
        {
            switch (request.SortingName)
            {
                case "nome":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Pessoa.nome).ToList();
                    else
                        objs = objs.OrderBy(p => p.Pessoa.nome).ToList();
                    break;
                case "cpf":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Pessoa.cnpj).ToList();
                    else
                        objs = objs.OrderBy(p => p.Pessoa.cnpj).ToList();
                    break;
                case "dataAdmissao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.dataAdmissao).ToList();
                    else
                        objs = objs.OrderBy(p => p.dataAdmissao).ToList();
                    break;
                case "unidadeNegocio_id":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.UnidadeNegocio.unidade).ToList();
                    else
                        objs = objs.OrderBy(p => p.UnidadeNegocio.unidade).ToList();
                    break;
                case "funcao_id":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Funcao.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.Funcao.descricao).ToList();
                    break;
                case "dataSaida":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.dataSaida).ToList();
                    else
                        objs = objs.OrderBy(p => p.dataSaida).ToList();
                    break;
            }
            return objs;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Funcionario obj)
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

                var obj = new Funcionario();
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
        public ActionResult Edit(Funcionario obj)
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
            Funcionario obj = new Funcionario();
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
        public ActionResult Delete(Funcionario obj)
        {
            try
            {
                CarregaViewData();
                string erro = "";
                if (obj.Excluir(_paramBase,ref erro))
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
                Funcionario obj = new Funcionario();
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

        public ActionResult Detail(int ID)
        {
            try
            {
                CarregaViewData();
                Funcionario obj = new Funcionario();
                obj = obj.ObterPorId(ID, _paramBase);
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
            ViewData["pessoa"] = new SelectList(new Pessoa().ObterFuncionarios(_paramBase), "id", "nome");
            ViewData["responsavel"] = new SelectList(new Pessoa().ObterFuncionarios(_paramBase), "id", "nome");
            ViewData["unidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
            ViewData["FuncionarioFuncao"] = new SelectList(new FuncionarioFuncao().ObterTodos(_paramBase), "id", "nome");
        }
    }
}
