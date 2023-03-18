//using Lib.Web.Mvc.JQuery.JqGrid;
//using SoftFin.Web.Classes;
//using SoftFin.Web.Models;
//using SoftFin.Web.Negocios;
//using SoftFin.Web.Regras;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace SoftFin.Web.Controllers
//{
//    public class BancoController : BaseController
//    {
//        public ActionResult TESTE()
//        {
//            return View();
//        }

//        //Bancos
//        public ActionResult Index()
//        {
//            _eventos.Info("Banco Index");
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            _eventos.Info("Banco Lista");

//            try
//            {

//                Banco obj = new Banco();
//                var db = new DbControle();
//                var totalRecords = obj.ObterTodos().Count();
                
//                JqGridResponse response = new JqGridResponse()
//                {
//                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                    PageIndex = request.PageIndex,
//                    TotalRecordsCount = totalRecords
//                };

//                foreach (var item in obj.ObterTodos().ToList())
//                {
//                    string principal = "";
//                    if (item.principal == true)
//                        principal = "Sim";
//                    else
//                        principal = "Não";
                    
//                    response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                    {
//                        item.codigo,
//                        item.nomeBanco,
//                        item.codigoBanco,
//                        item.agencia,
//                        item.contaCorrente,
//                        principal
//                    }));
//                }

//                return new JqGridJsonResult() { Data = response };
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        [HttpPost]
//        public ActionResult Create(Banco obj)
//        {
//            _eventos.Info("Banco Create Post");
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    int estab = _paramBase.estab_id;
//                    obj.estabelecimento_id = estab;
//                    Banco banco = new Banco();
//                    if (banco.Incluir(obj) == true)
//                    {
//                        _eventos.Info("Banco Create Post Json", obj);
//                        ViewBag.msg = "Banco incluído com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Banco já cadastrado";
//                    }
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
//        public ActionResult Create()
//        {
//            _eventos.Info("Banco Create Post");
//            var obj = new Banco();
//            return View(obj);
//        }

//        [HttpPost]
//        public ActionResult Edit(Banco obj)
//        {
//            _eventos.Info("Banco Edit Post");
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    Banco banco = new Banco();
//                    int estab = _paramBase.estab_id;
//                    obj.estabelecimento_id = estab;
//                    if (banco.Alterar(obj))
//                    {
//                        _eventos.Info("Banco Edit Post Json", obj);
//                        ViewBag.msg = "Banco alterado com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Não foi possivel concluir a operação";
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                }
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Edit(int ID)
//        {
//            _eventos.Info("Banco Edit Get");
//            Banco obj = new Banco();
//            try
//            {
//                Banco banco = obj.ObterPorId(ID);

//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(banco);


//                return View(banco);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        [HttpPost]
//        public ActionResult Delete(Banco obj)
//        {
//            _eventos.Info("Banco Delete Post");
//            try
//            {
//                DbControle banco = new DbControle();
//                Banco bc = new Banco();
//                Banco obj2 = obj.ObterPorId(obj.id);
//                bc.Excluir(obj.id);
//                _eventos.Info("Banco Delete Post Json",obj2);
//                ViewBag.msg = "Banco excluída com sucesso";
//                return RedirectToAction("/Index");
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Delete(int ID)
//        {
//            _eventos.Info("Banco Delete Get");
//            Banco obj = new Banco();
//            try
//            {
//                Banco banco = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(banco);


//                return View(banco);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        public ActionResult Principal(int ID)
//        {
//            _eventos.Info("Banco Principal Get");
//            Banco obj = new Banco();
//            try
//            {
//                obj.DefineBancoPrincipal(ID);
//                return RedirectToAction("/Index");
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//    }
//}
