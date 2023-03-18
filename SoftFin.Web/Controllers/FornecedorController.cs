//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//namespace Sistema1.Controllers
//{
//    public class FornecedorController : BaseController
//    {
//        public override JsonResult TotalizadorDash(int? id)
//        {
//            //base.TotalizadorDash(id);
//            var soma = new Fornecedor(_paramBase).ObterTodos().Count();
//            return Json(new { CDStatus = "OK", Result = soma }, JsonRequestBehavior.AllowGet);

//        }
//        public ActionResult Excel()
//        {
//            var obj = new Fornecedor();
//            var lista = obj.ObterTodos();
//            CsvExport myExport = new CsvExport();
//            foreach (var item in lista)
//            {
//                myExport.AddRow();
//                myExport["id"] = item.id;
//                myExport["Pessoa"] = item.Pessoa.nome;
//                myExport["Responsavel"] = item.Responsavel.nome;

//                if (item.dataContratada != null)
//                    myExport["dataContratada"] = item.dataContratada.Value.ToShortDateString();
    
//                myExport["UnidadeNegocio"] = item.UnidadeNegocio.unidade;

//                if (item.dataSaida != null)
//                    myExport["dataSaida"] = item.dataSaida.Value.ToShortDateString();
//                myExport["NomeEmpresa"] = item.NomeEmpresa;
//            }
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();
//            return File(myCsvData, "application/vnd.ms-excel", "Fornecedor.csv");
//        }
//        public ActionResult Index()
//        {
//            CarregaViewData();
//            var obj = new Fornecedor();
//            return View(obj);
//        }

//        public ActionResult Listas(JqGridRequest request)
//        {
//            string ValorNome = Request.QueryString["nome"];
//            string ValorUnidade = Request.QueryString["ddlUnidade"];


//            int totalRecords = 0;
//            Fornecedor obj = new Fornecedor();
//            var objs = new Fornecedor().ObterTodos();

//            if (!String.IsNullOrEmpty(ValorNome))
//            {
//                objs = objs.Where(p => p.Pessoa.nome.Contains(ValorNome)).ToList();
//            }

//            if (!String.IsNullOrEmpty(ValorUnidade))
//            {
//                int aux;
//                int.TryParse(ValorUnidade, out aux);
//                objs = objs.Where(p => p.unidadeNegocio_id == aux).ToList();
//            }



//            objs = Organiza(request, objs);
//            totalRecords = objs.Count();
//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
//            foreach (var item in objs)
//            {
//                var data = "";
//                var nomeempresa = "";
//                var funcao = "";
//                var datasaida = "";
//                var unidade = "";
                

//                nomeempresa = item.NomeEmpresa;

//                if (item.UnidadeNegocio != null)
//                    unidade = item.UnidadeNegocio.unidade;

//                if (item.dataSaida != null)
//                    datasaida = item.dataSaida.Value.ToShortDateString();


//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.Pessoa.nome,
//                    item.Pessoa.cnpj,
//                    nomeempresa,
//                    unidade,
//                    datasaida

//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }

//        private static List<Fornecedor> Organiza(JqGridRequest request, List<Fornecedor> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "nome":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Pessoa.nome).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Pessoa.nome).ToList();
//                    break;
//                case "cnpj":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Pessoa.cnpj).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Pessoa.cnpj).ToList();
//                    break;
//                case "txtNomeEmpresa":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.NomeEmpresa).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.NomeEmpresa).ToList();
//                    break;
//                case "unidadeNegocio_id":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.UnidadeNegocio.unidade).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.UnidadeNegocio.unidade).ToList();
//                    break;

//                case "dataSaida":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.dataSaida).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.dataSaida).ToList();
//                    break;
//            }
//            return objs;
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Fornecedor obj)
//        {
//            try
//            {
//                CarregaViewData();
//                //int estab = Acesso.EstabLogado();
//                //obj.estabelecimento_id = estab;
//                //int idempresa = Acesso.EmpresaLogado();
//                //obj.empresa_id = idempresa;
//                if (ModelState.IsValid)
//                {
//                    if (obj.Incluir())
//                    {
//                        ViewBag.msg = "Incluído com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
//                        return View(obj);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();

//                var obj = new Fornecedor();
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(Fornecedor obj)
//        {
//            try
//            {
//                CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    //int estab = Acesso.EstabLogado();
//                    //obj.estabelecimento_id = estab;
//                    //int idempresa = Acesso.EmpresaLogado();
//                    //obj.empresa_id = idempresa;
//                    if (obj.Alterar())
//                    {
//                        ViewBag.msg = "Alterado com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Impossivel alterar, registro excluído";
//                        return View(obj);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        public ActionResult Edit(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                Fornecedor obj = new Fornecedor();
//                obj = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(Fornecedor obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string erro = "";
//                if (obj.Excluir(ref erro))
//                {
//                    ViewBag.msg = "Excluido com sucesso";
//                    return RedirectToAction("/Index");
//                }
//                else
//                {
//                    ViewBag.msg = erro;
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        public ActionResult Delete(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                Fornecedor obj = new Fornecedor();
//                obj = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }

//        public ActionResult Detail(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                Fornecedor obj = new Fornecedor();
//                obj = obj.ObterPorId(ID);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("Index", "Erros");
//            }
//        }
//        private void CarregaViewData()
//        {
//            ViewData["pessoa"] = new SelectList(new Pessoa().ObterFornecedores(_paramBase), "id", "nome");
//            ViewData["responsavel"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
//            ViewData["unidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(), "id", "unidade");

//        }
//    }
//}
