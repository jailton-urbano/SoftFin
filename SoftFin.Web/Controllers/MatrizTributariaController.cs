//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using Sistema1.Negocios;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;

//namespace Sistema1.Controllers
//{
//    public class MatrizTributariaController : BaseController
//    {

//        #region JsonResult
//        public JsonResult ListaImposto()
//        {
//            return Json(new SelectList(new Imposto().ObterTodos(), "id", "descricao"), JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult ObterCFOP()
//        {
//            var objs = new CFOP().ObterTodos();


//            return Json(objs.Select(p => new
//            {
//                Value = p.id.ToString(),
//                Text = p.descricao
//            }), JsonRequestBehavior.AllowGet);
//        }


//        public JsonResult ObtemEntidade(int id)
//        {
//            OperacaoView obj = new OperacaoView();

//            if (id != 0)
//            {
//                var data = new Operacao().ObterPorId(id);

//                obj.CalculoImpostoList = new List<CalculoImpostoView>();

//                if (data != null)
//                {
//                    obj.id = data.id;
//                    obj.codigo = data.codigo;
//                    obj.descricao = data.descricao;
//                    obj.descricaoNota = data.descricaoNota;
//                    obj.IncentivoFiscal = data.IncentivoFiscal;
//                    obj.NaturezaOperacao = data.CFOP;
//                    //obj.OptanteSimplesNacional      = data.OptanteSimplesNacional;
//                    obj.RegimeEspecialTributacao = data.RegimeEspecialTributacao;
//                    obj.SituacaoNF = data.SituacaoNF;
//                    obj.TributarMunicipio = data.TributarMunicipio;
//                    obj.TributarPrestador = data.TributarPrestador;
//                    obj.tipoRPS_id = data.tipoRPS_id.ToString();
//                    obj.situacaoTributariaNota_id = data.situacaoTributariaNota_id.ToString();
//                    obj.entradaSaida = data.entradaSaida;


//                    var dataCalculoImposto = new calculoImposto().ObterTodos().Where(x => x.operacao_id == id).ToList();
//                    foreach (var item in dataCalculoImposto)
//                    {
//                        var impostoaux = new Imposto().ObterTodos().Where(p => p.id == item.imposto.id);
//                        if (impostoaux.Count() == 0)
//                        {
//                            return Json(new { CDMessage = "NOK", DSErroReturn = "Imposto não encontrado" }, JsonRequestBehavior.AllowGet);
//                        }

//                        obj.CalculoImpostoList.Add(new CalculoImpostoView
//                        {
//                            id = data.id,
//                            aliquota = item.aliquota,
//                            arrecadador = item.arrecadador,
//                            retido = item.retido,
//                            imposto = impostoaux.First().descricao,
//                        });
//                    }
//                }
//            }
//            return Json(obj, JsonRequestBehavior.AllowGet);

//        }
//        public JsonResult ListaTipoRPS()
//        {
//            var data = new SelectList(new TipoRPS().ObterTodos(), "id", "descricao");
//            return Json(data, JsonRequestBehavior.AllowGet);
//        }

//        public JsonResult ListaSituacaoTributaria()
//        {
//            var con1 = new SituacaoTributariaNota().ObterTodos();
//            var items = new List<SelectListItem>();
//            foreach (var item in con1)
//            {
//                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1}", item.codigo, item.descricao) });
//            }
//            var listret = new SelectList(items, "Value", "Text");
//            return Json(listret, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public JsonResult Salvar(OperacaoView obj)
//        {
//            try
//            {
//                if (obj.CalculoImpostoList.Count() == 0)
//                {
//                    return Json(new { CDMessage = "NOK", DSErroReturn = "Erro! Informe o detalhe do lançamento" }, JsonRequestBehavior.AllowGet);
//                }

//                var db = new DbControle();


//                using (var dbcxtransaction = db.Database.BeginTransaction())
//                {
//                    var documento = new Operacao();
//                    if (obj.id != 0)
//                    {
//                        documento = new Operacao().ObterPorId(obj.id, db);
//                        if (documento == null)
//                        {
//                            return Json(new { CDMessage = "NOK", DSErroReturn = "Operação não encontrada para a alteração" }, JsonRequestBehavior.AllowGet);
//                        }

//                    }

//                    var CalculoImposto = new List<calculoImposto>();
//                    foreach (var item in obj.CalculoImpostoList)
//                    {
//                        var impostoaux = new Imposto().ObterTodos().Where(p => p.descricao == item.imposto);
//                        if (impostoaux.Count() == 0)
//                        {
//                            return Json(new { CDMessage = "NOK", DSErroReturn = "Imposto não encontrado" }, JsonRequestBehavior.AllowGet);
//                        }

//                        CalculoImposto.Add(
//                            new calculoImposto
//                            {
//                                id = item.id,
//                                aliquota = item.aliquota,
//                                arrecadador = item.arrecadador,
//                                retido = item.retido,
//                                imposto_id = impostoaux.First().id,
//                                operacao_id = obj.id
//                            });
//                    }

//                    var situacaoAux = new SituacaoTributariaNota().ObterPorId(int.Parse(obj.situacaoTributariaNota_id));

//                    documento.empresa_id = _empresa;
//                    documento.codigo = obj.codigo;
//                    documento.descricao = obj.descricao;
//                    documento.descricaoNota = obj.descricaoNota;
//                    documento.CFOP = obj.NaturezaOperacao.Value.ToString();
//                    documento.RegimeEspecialTributacao = obj.RegimeEspecialTributacao;
//                    //documento.OptanteSimplesNacional = obj.OptanteSimplesNacional;
//                    documento.IncentivoFiscal = obj.IncentivoFiscal;
//                    documento.TributarMunicipio = obj.TributarMunicipio;
//                    documento.TributarPrestador = obj.TributarPrestador;
//                    documento.SituacaoNF = situacaoAux.codigo;
//                    documento.tipoRPS_id = int.Parse(obj.tipoRPS_id);
//                    documento.situacaoTributariaNota_id = int.Parse(obj.situacaoTributariaNota_id);
//                    documento.entradaSaida = obj.entradaSaida;
//                    //documento.opcaoTributariaSimples_id = int.Parse(obj.opcaoTributariaSimples_id);


//                    if (obj.id != 0)
//                    {
//                        AlterarOperacao(documento, CalculoImposto, db);
//                    }
//                    else
//                    {
//                        IncluirOperacao(documento, CalculoImposto, db);
//                    }
//                    dbcxtransaction.Commit();
//                }

//                return Json(new { CDMessage = "OK", Success = obj }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return Json(new { CDMessage = "NOK", DSErroReturn = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        [HttpPost]
//        public JsonResult Excluir(int id)
//        {

//            try
//            {
//                var db = new DbControle();

//                var operacao = new Operacao().ObterPorId(id, db);
//                if (operacao == null)
//                {
//                    return Json(new { CDMessage = "NOK", DSErroReturn = "Operação não encontrada para exclusão" }, JsonRequestBehavior.AllowGet);
//                }

//                using (var dbcxtransaction = db.Database.BeginTransaction())
//                {

//                    var calculoImposto   = new calculoImposto().ObterTodos().Where(x=> x.operacao_id == operacao.id).ToList();

//                    foreach (var item in calculoImposto)
//                    {
//                        string erro = "";
//                        new calculoImposto().Excluir(item.id, ref erro, db);
//                        if (erro != "")
//                            throw new Exception(erro);
//                    }


//                    var cs = new Operacao();
//                    cs.Excluir(id, db);
//                    dbcxtransaction.Commit();
//                }

//                return Json(new { CDMessage = "OK" }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return Json(new { CDMessage = "NOK", DSErroReturn = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);

//            }
//        }


//        #endregion

//        public ActionResult Index()
//        {
//            var obj = new Operacao();
//            return View(obj);
//        }





//        public ActionResult Listas(JqGridRequest request)
//        {
//            string ValorCodigo = Request.QueryString["codigo"];
//            string ValorDescricao = Request.QueryString["descricao"];
//            string ValorNaturezaOperacao = Request.QueryString["naturezaOperacao"];
//            string ValorEntrada = Request.QueryString["entrada"];
//            string ValorSaida = Request.QueryString["saida"];

//            int totalRecords = 0;
//            Operacao obj = new Operacao();
//            var objs = new Operacao().ObterTodos();

//            if (!String.IsNullOrEmpty(ValorCodigo))
//            {
//                objs = objs.Where(p => p.codigo == ValorCodigo).ToList();
//            }

//            if (!String.IsNullOrEmpty(ValorDescricao))
//            {
//                objs = objs.Where(p => p.descricao == ValorDescricao).ToList();
//            }

//            if (ValorEntrada != null && ValorSaida != null)
//            {
//                if ((ValorEntrada.ToLower() == "false") || (ValorSaida.ToLower() == "false"))
//                {
//                    if (ValorEntrada.ToLower() == "true")
//                    {
//                        int aux;
//                        int.TryParse(ValorNaturezaOperacao, out aux);
//                        objs = objs.Where(p => p.entradaSaida == "E").ToList();
//                    }

//                    if (ValorSaida.ToLower() == "true")
//                    {
//                        int aux;
//                        int.TryParse(ValorNaturezaOperacao, out aux);
//                        objs = objs.Where(p => p.entradaSaida == "S").ToList();
//                    }
//                }
//            }
//            totalRecords = objs.Count();

//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };

//            objs = Organiza(request, objs);
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
//            foreach (var item in objs)
//            {

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.codigo,
//                    item.descricao,
//                    (item.entradaSaida == "E") ? "Entrada":  "Saída"
//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }
//        private static List<Operacao> Organiza(JqGridRequest request, List<Operacao> objs)
//        {
//            switch (request.SortingName)
//            {
//                case "codigo":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.codigo).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.codigo).ToList();
//                    break;
//                case "descricao":
//                    if (request.SortingOrder == JqGridSortingOrders.Desc)
//                        objs = objs.OrderByDescending(p => p.descricao).ToList();
//                    else
//                        objs = objs.OrderBy(p => p.descricao).ToList();
//                    break;
//            }
//            return objs;
//        }

//        #region Rotas
//        public ActionResult Manut(int? id, bool? delete, bool? detail)
//        {
//            ViewData["btnOK"] = "OK";
//            ViewData["delete"] = false;
//            ViewData["detail"] = false;

//            ViewData["Titulo"] = "Matriz Tributária - Novo";
//            if (id != null)
//            {
//                ViewData["Titulo"] = "Matriz Tributária - Editar";

//            }
//            if (delete != null)
//            {
//                ViewData["Titulo"] = "Matriz Tributária - Exclusão";
//                detail = true;
//                ViewData["delete"] = true;
//                ViewData["detail"] = true;
//            }
//            else
//            {
//                if (detail != null)
//                {
//                    ViewData["Titulo"] = "Matriz Tributária - Detalhe";
//                    ViewData["btnOK"] = "Detalhe";
//                    detail = true;
//                    ViewData["detail"] = true;
//                }
//            }

//            ViewData["id"] = id;


//            return View();
//        }

//        #endregion

//        #region Privates
//        private void IncluirOperacao(Operacao operacao, List<calculoImposto> calculoImpostos, DbControle db)
//        {
//            operacao.Incluir(operacao, calculoImpostos, db);
//        }
//        private void AlterarOperacao(Operacao operacao, List<calculoImposto> calculoImpostos, DbControle db)
//        {
//            operacao.Alterar(operacao, calculoImpostos, db);
//        }

//        #endregion
//    }
//}
