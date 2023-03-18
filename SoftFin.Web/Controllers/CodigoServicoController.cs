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
//    public class CodigoServicoController : BaseController
//    {
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {
//            var obj = new CodigoServicoEstabelecimento();
//            var totalRecords = obj.ObterTodos(_paramBase).Count();

//            var response = new JqGridResponse()
//            { TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };
//            foreach (var item in
//                obj.ObterTodos().ToList())
//            {
//                //string tp = "";
//                //if (item.operacao_id != null)
//                //    tp = item.Operacao.descricao;

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.CodigoServicoMunicipio.codigo,
//                    item.CodigoServicoMunicipio.descricao,
//                    item.CodigoServicoMunicipio.aliquota.ToString("0.00")
//                }));
//            }


//            return new JqGridJsonResult() { Data = response };
//        }



//        [HttpPost]
//        public ActionResult Create(CodigoServicoEstabelecimento obj)
//        {
//            try
//            {
//                CarregaViewData();

//                if (ModelState.IsValid)
//                {
//                    obj.estabelecimento_id = _estab;
//                    var codigo = new CodigoServicoEstabelecimento();


//                    if (codigo.Incluir(obj) == true)
//                    {
//                        ViewBag.msg = "Código de Serviço incluído com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Código de Serviço já cadastrado";
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
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();
//                var obj = new CodigoServicoEstabelecimento();
//                obj.estabelecimento_id = _estab;
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost]
//        public ActionResult Delete(CodigoServicoEstabelecimento obj)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";
//                if (!obj.Excluir(obj.id, ref Erro))
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
//                var obj = new CodigoServicoEstabelecimento();
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
//        public ActionResult Edit(CodigoServicoEstabelecimento obj)
//        {
//            try
//            {
//                CarregaViewData();
//                var codigo = new CodigoServicoEstabelecimento();
//                obj.estabelecimento_id = _estab;
//                codigo.Alterar(obj);

//                ViewBag.msg = "Código de Serviço alterado com sucesso";
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
//                CarregaViewData();
//                var obj = new CodigoServicoEstabelecimento();
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
//        private void CarregaViewData()
//        {
//            ViewData["CodigoServico"] = new CodigoServicoMunicipio().CarregaCombo();
//        }
//    }
//}
