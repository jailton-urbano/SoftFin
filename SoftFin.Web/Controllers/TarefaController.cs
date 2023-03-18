//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using Sistema1.Negocios;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Sistema1.Controllers
//{
//    public class TarefaController : BaseController
//    {
//        public JsonResult GetAtividades(int idprojeto) 
//        {
//            var objs = new Atividade().ObterTodosPorIdProjeto(idprojeto);

//            return Json(new SelectList(objs, "id", "descricao"),JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult Excel()
//        {
//            var obj = new Tarefa();
//            var lista = obj.ObterTodos();
//            CsvExport myExport = new CsvExport();

//            foreach (var item in lista)
//            {
//                myExport.AddRow();
//                myExport["id"] = item.id;
//                myExport["Usuario"] = item.Usuario.nome;
//                myExport["data"] = item.data;
//                myExport["qtdHoras"] = item.qtdHoras;
//                myExport["historico"] = item.historico;
//                myExport["aprovador"] = item.aprovadorusu.nome;
//                myExport["Atividade"] = item.atividade.descricao;
//            }
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();
//            return File(myCsvData, "application/vnd.ms-excel", "Apontamento.csv");
//        }
//        public ActionResult Index()
//        {
//            CarregaViewData();
//            var obj = new Tarefa();
//            return View(obj);
//        }
//        public ActionResult Listas(JqGridRequest request)
//        {
//            string Valoratividade_id = Request.QueryString["atividade_id"];
//            int totalRecords = 0;
//            Tarefa obj = new Tarefa();

//            var objs = obj.ObterTodos().ToList();

//            if (!String.IsNullOrEmpty(Valoratividade_id))
//            {
//                int aux;
//                int.TryParse(Valoratividade_id, out aux);
//                objs = objs.Where(p => p.atividade.id == aux).ToList();
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
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.atividade.Projeto.codigoProjeto + " - " + item.atividade.Projeto.nomeProjeto,
//                    item.atividade.descricao,
//                    item.historico,
//                    item.qtdHoras.ToString("n"),
//                    item.Usuario.nome
//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }


//        private static List<Tarefa> Organiza(JqGridRequest request, List<Tarefa> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "nomeProjeto":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.atividade.Projeto.nomeProjeto).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.atividade.Projeto.nomeProjeto).ToList();
//                    break;
//                case "atividade":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.atividade.descricao).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.atividade.descricao).ToList();
//                    break;
//                case "historico":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.historico).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.historico).ToList();
//                    break;
//                case "qtdHoras":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.qtdHoras).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.qtdHoras).ToList();
//                    break;
//                case "usuario":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.Usuario.nome).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.Usuario.nome).ToList();
//                    break;
//             }
//             return objs;
//          }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Tarefa obj)
//        {
//            try
//            {
//                CarregaViewData();
//                int estab = Acesso.EstabLogado();
//                obj.estabelecimento_id = _estab;

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
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Create(int id)
//        {
//            try
//            {
//                CarregaViewData();
//                var obj = new Tarefa();
//                obj.estabelecimento_id = _estab;
//                obj.atividade = new Atividade().ObterPorId(id);
//                obj.atividade_id = obj.atividade.id;
                
//                obj.data = DateTime.Now;
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(Tarefa obj)
//        {
//            try
//            {
//                CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    int estab = Acesso.EstabLogado();
//                    obj.estabelecimento_id = estab;

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
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Edit(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                Tarefa obj = new Tarefa();
//                obj = obj.ObterPorId(ID);
//                Seguranca.validaNulo(obj);

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(Tarefa obj)
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
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Delete(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                Tarefa obj = new Tarefa();
//                obj = obj.ObterPorId(ID);
//                Seguranca.validaNulo(obj);

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        private void CarregaViewData()
//        {
//            ViewData["projeto"] = new Projeto().CarregaProjeto();
//            ViewData["atividade"] = new SelectList(new Atividade().ObterTodos(), "id", "descricao");
//            ViewData["usuario"] = new SelectList(new Usuario().ObterTodos(), "id", "nome");
//        }

        
//    }
//}
