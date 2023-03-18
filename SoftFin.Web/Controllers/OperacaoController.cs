//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Sistema1.Models;
//using System.Web.Helpers;
//using System.Data.Entity;
//using Sistema1.Classes;
//using Sistema1.Negocios;
//using Lib.Web.Mvc.JQuery.JqGrid;

//namespace Sistema1.Controllers
//{
//    public class OperacaoController : BaseController
//    {
//        DbControle _banco = new DbControle();

//        public ActionResult Index()
//        {
//            return View();
//        }
//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {

            
//            var db = new DbControle();
//            var regs = new Operacao().ObterTodos();

//            int totalRecords = regs.Count();

//            //Fix for grouping, because it adds column name instead of index to SortingName
//            //string sortingName = "cdg_perfil";

//            //Prepare JqGridData instance
//            JqGridResponse response = new JqGridResponse()
//            {
//                //Total pages count
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                //Page number
//                PageIndex = request.PageIndex,
//                //Total records count
//                TotalRecordsCount = totalRecords
//            };

//            regs = regs.OrderBy(p => p.descricao).Skip(12 * request.PageIndex).Take(12).ToList();

//            //Table with rows data
//            foreach (var item in
//                regs)
//            {
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.codigo,
//                    item.situacaoTributariaNota.codigo,
//                    item.descricao,
//                    item.descricaoNota
//                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }

//        [HttpPost]
//        public ActionResult Create(Operacao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    Operacao operacao = new Operacao();

//                    int idempresa = Acesso.EmpresaLogado();
//                    obj.empresa_id = idempresa;
//                    if (operacao.Incluir(obj) == true)
//                    {
//                        ViewBag.msg = "Operação incluído com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Operação já cadastrado";
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
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();
//                return View(new Operacao());
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Edit(Operacao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    Operacao operacao = new Operacao();
//                    operacao.Alterar(obj);

//                    ViewBag.msg = "Operação alterado com sucesso";
//                    return View(obj);

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
//                Operacao obj = _banco.Operacao.Where(p => p.id == ID && p.empresa_id == _empresa ).FirstOrDefault();
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Delete(Operacao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";
//                if (!obj.Excluir(obj.id, ref Erro))
//                {
//                    ViewBag.msg = Erro;

//                    obj = obj.ObterPorId(obj.id );
//                    return View(obj);
//                }
//                else
//                {
//                    return RedirectToAction("/Index");
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
//                Operacao obj = _banco.Operacao.Where(p => p.id == ID && p.empresa_id == _empresa).FirstOrDefault();
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
//            var calculoImpostos = from c in _banco.calculoImposto.Where(p => p.empresa_id == _empresa) select c;
//            ViewData["calculoImposto"] = new SelectList(calculoImpostos, "id", "imposto");
//            ViewData["Palavras"] = "'#TesteA', '#TesteB', '#TesteC'";
//        }
//    }
//}
