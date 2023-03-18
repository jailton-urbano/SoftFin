//using Lib.Web.Mvc.JQuery.JqGrid;
//using SoftFin.Web.Classes;
//using SoftFin.Web.Models;
//using SoftFin.Web.Negocios;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace SoftFin.Web.Controllers
//{
//    public class PerfilPagarAprovacaoController : BaseController
//    {
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            var obj = new PerfilPagarAprovacao();
//            var totalRecords = obj.ObterTodos().Count;

//            var response = new JqGridResponse()
//            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };

//            foreach (var item in
//                obj.ObterTodos())
//            {
//                var nome = "";
//                if (item.usuarioAutorizador != null)
//                {
//                    nome = item.usuarioAutorizador.nome;
//                }

//                var status = "Ativo";
//                if (item.Inativo)
//                {
//                    status = "Inativo";
//                }

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.id,
//                    nome ,
//                    item.valorLimiteCPAG.ToString("0.00"),
//                    item.valorLimiteNFSE.ToString("0.00"),
//                    status
//                }));
//            }

//            return new JqGridJsonResult() { Data = response };
//        }



//        [HttpPost]
//        public ActionResult Create(PerfilPagarAprovacao obj)
//        {
//            try
//            {
//                CarregaViewData();

//                if (ModelState.IsValid)
//                {
//                    var modelo = new PerfilPagarAprovacao();
//                    obj.estabelecimento_id = _estab; 
//                    if (modelo.Incluir(obj) == true)
//                    {
//                        ViewBag.msg = "Perfil pagar Aprovacao incluído com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Perfil pagar Aprovacao já cadastrado";
//                        return View(obj);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Dados Invalidos");
//                    return View(obj);
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }

//        private void CarregaViewData()
//        {

//            ViewData["pessoa"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");

//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();


//                var obj = new PerfilPagarAprovacao();
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Delete(PerfilPagarAprovacao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";

//                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
//                {
//                    ViewBag.msg = Erro;
//                    obj = obj.ObterPorId(obj.id);
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
//                var obj = new PerfilPagarAprovacao();
//                var cs = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(cs);
//                return View(cs);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Edit(PerfilPagarAprovacao obj)
//        {
//            try
//            {
//                CarregaViewData();
//                var codigo = new PerfilPagarAprovacao();
//                obj.estabelecimento_id = _estab; 
//                codigo.Alterar(obj);

//                ViewBag.msg = "Perfil Aprovação alterado com sucesso";
//                return RedirectToAction("/Index");
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
//                var obj = new PerfilPagarAprovacao();
//                obj = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//    }
//}
