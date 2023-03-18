//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using Sistema1.Models;
//using Sistema1.Classes;
//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Negocios;
//using System.Data.Entity;
//using BoletoNet;
//using System.Transactions;
//using System.Text;

//namespace Sistema1.Controllers
//{
//    public class PagamentoController : BaseController
//    {
//        public override JsonResult TotalizadorDash(int? id)
//        {
//            base.TotalizadorDash(id);
//            var AReceber = new Pagamento().ObterEntreData(DataInicial, DataFinal).Sum(x => x.valorPagamento).ToString("n");
//            return Json(new { CDStatus = "OK", Result = "R$ " + AReceber }, JsonRequestBehavior.AllowGet);

//        }

//        public ActionResult Decide(int ID)
//        {
//            try
//            {
//                Pagamento pagamento = new Pagamento().ObterPorDocumentoPagarMestreId(ID, null);

//                return RedirectToAction("Create", new { @id = ID });
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }

//        [HttpPost]
//        public ActionResult Create(Pagamento obj)
//        {
//            try
//            {
//                CarregaViewData();
//                Pagamento pagamento = new Pagamento();
//                int estab = Acesso.EstabLogado();
//                obj.estabelecimento_id = estab;

//                DbControle db = new DbControle();
//                var TipoManual = new OrigemMovimento().TipoManual();
//                using (var dbcxtransaction = db.Database.BeginTransaction())
//                {
//                    if (pagamento.Incluir(obj, db))
//                    {

//                        //Atualiza data/valor/origem de recebimento no Banco Movimento
//                        var bms = new BancoMovimento().ObterPorCPAGPrincipal(obj.documentoPagarMestre_ID, db);
//                        var doc = new DocumentoPagarMestre().ObterPorId(obj.documentoPagarMestre_ID, db);

//                        var pg = pagamento.ObterTodosPorDocumentoPagarMestreId(obj.documentoPagarMestre_ID, db);
//                        var pgDet = new DocumentoPagarDetalhe().ObterPorCPAG(obj.documentoPagarMestre_ID).FirstOrDefault();
//                        var valorPago = pg.Sum(p => p.valorPagamento);

//                        //Inclui no banco movimento lançamento referente ao pagamento feito
//                        BancoMovimentoConfig bancoMovimentoConfig = new BancoMovimento().Obter();
//                        int? idDocPag = doc.id;
//                        BancoMovimento bancoMovimento = new BancoMovimento().ObterTodos().Where(p => p.DocumentoPagarMestre_id == idDocPag).FirstOrDefault();
//                        OrigemMovimento origemMovimento = new OrigemMovimento().ObterTodos().Where(x => x.Modulo == "Pagamento").FirstOrDefault();
//                        TipoMovimento tipoMov = new TipoMovimento().ObterTodos().Where(x=> x.Codigo=="SAI").FirstOrDefault();

//                        BancoMovimento bancoMovimentoNew = new BancoMovimento();
//                        bancoMovimentoNew.banco_id = obj.banco_ID;
//                        bancoMovimentoNew.data = obj.dataPagamento;
//                        bancoMovimentoNew.valor = obj.valorPagamento;
//                        bancoMovimentoNew.planoDeConta_id = doc.planoDeConta_id;
//                        bancoMovimentoNew.tipoDeMovimento_id = tipoMov.id;
//                        bancoMovimentoNew.tipoDeDocumento_id = doc.tipodocumento_id;
//                        //Grava no banco movimento Origem="Contas a Pagar"
//                        bancoMovimentoNew.origemmovimento_id = origemMovimento.id;
//                        bancoMovimentoNew.historico = "CPAG Documento Nº " + doc.numeroDocumento.ToString();
//                        bancoMovimentoNew.notafiscal_id = null;
//                        bancoMovimentoNew.DocumentoPagarMestre_id = null;
//                        bancoMovimentoNew.pagamento_id = obj.id;

//                        //Inclui novo banco movimento
//                        bancoMovimentoNew.Incluir(bancoMovimentoNew, db);


//                        //Atualiza Banco Movimento na Inclusão do Pagamento
//                        if (valorPago < doc.valorBruto || valorPago == 0)
//                        {
//                            //Valor Pago ainda é menor que o Valor Bruto do Contas a Pagar
//                            //Atualizar no banco movimento apenas o saldo a pagar
//                            var saldoPagar = doc.valorBruto - valorPago;
//                            foreach (var item in bms)
//                            {
//                                item.valor = saldoPagar;
//                                item.Alterar(db);
//                            }
//                        }
//                        else
//                        {
//                            //Exclui do banco movimento
//                            //quando não houver saldo a pagar
//                            foreach (var item in bms)
//                            {
//                                //item.valor = 0;
//                                string Erro = "";
//                                if (!item.Excluir(item.id, ref Erro))
//                                {
//                                    ViewBag.msg = Erro;
//                                }
//                            }
//                        }

//                        if (pg.Count() == 0)
//                            doc.StatusPagamento = 1;
//                        else
//                        {
//                            if (valorPago >= doc.valorBruto)
//                                doc.StatusPagamento = 3;
//                            else
//                                doc.StatusPagamento = 2;
//                        }
//                        doc.Alterar(doc,db);

//                        ViewBag.msg = "Pagamento atualizado com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Pagamento já efetuado";
//                    }
//                    dbcxtransaction.Commit();

//                }
//                DocumentoPagarMestre doc2 = new DocumentoPagarMestre();
//                obj.DocumentoPagarParcela = doc2.ObterPorId(obj.DocumentoPagarParcela.DocumentoPagarMestre);
//                return View(obj);
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
//                DocumentoPagarMestre doc = new DocumentoPagarMestre();
//                doc = doc.ObterPorId(id);

//                Pagamento obj = new Pagamento();

//                obj.dataPagamento = DateTime.Now.Date;
//                obj.documentoPagarMestre = doc;
//                obj.documentoPagarMestre_ID = doc.id;

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }

//        [HttpPost]
//        public ActionResult Edit(Pagamento obj)
//        {
//            try
//            {
//                CarregaViewData();
//                Pagamento pagamento = new Pagamento();
//                int estab = Acesso.EstabLogado();
//                obj.estabelecimento_id = estab;

//                DbControle db = new DbControle();
//                var TipoManual = new OrigemMovimento().TipoManual();
//                using (var dbcxtransaction = db.Database.BeginTransaction())
//                {
//                    if (pagamento.Alterar(obj, db))
//                    {
//                        //Atualiza data/valor/origem de recebimento no Banco Movimento
//                        var bms = new BancoMovimento().ObterPorCPAG(obj.documentoPagarMestre_ID, db);

//                        foreach (var item in bms)
//                        {
//                            item.data = obj.dataPagamento;
//                            item.valor = obj.valorPagamento;
//                            item.origemmovimento_id = TipoManual;
//                            item.Alterar(db);
//                        }

//                        ViewBag.msg = "Contas a Pagar atualizado com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Contas a Pagar já pago";
//                    }
//                    dbcxtransaction.Commit();
//                }


//                ViewBag.msg = "Contas a Pagar alterado com sucesso";
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

//                Pagamento obj = new Pagamento();

//                obj = obj.ObterPorId(ID);

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
//        [HttpPost, ActionName("Delete")]
//        public ActionResult DeleteConfirmacao(int id)
//        {
//            try
//            {
//                CarregaViewData();
//                string Erro = "";
//                Pagamento obj = new Pagamento().ObterPorId(id, null);
//                var TipoManual = new OrigemMovimento().TipoManual();
//                var db = new DbControle();

//                using (var dbcxtransaction = db.Database.BeginTransaction())
//                {
//                    obj = obj.ObterPorId(obj.id, db);

//                    //Exclui do banco movimento registro relacionado ao pagamento
//                    //que está sendo excluído
//                    var bmr = new BancoMovimento().ObterPorPagamento(obj.id, db);
//                    foreach (var item in bmr)
//                    {
//                        if (!item.Excluir(item.id, ref Erro, db))
//                        {
//                            ViewBag.msg = Erro;
//                            return View(obj);
//                        }
//                    }

//                    //Exclui Pagamento
//                    if (!obj.Excluir(obj.id, ref Erro, db))
//                    {
//                        ViewBag.msg = Erro;
//                        return View(obj);
//                    }
//                    else
//                    {
//                        var doc = new DocumentoPagarMestre().ObterPorId(obj.documentoPagarMestre_ID, db);
//                        BancoMovimento banco = db.BancoMovimento.Where(x=> x.DocumentoPagarMestre_id==doc.id).FirstOrDefault();
//                        Pagamento pagamento = new Pagamento();
//                        var pg = pagamento.ObterTodosPorDocumentoPagarMestreId(obj.documentoPagarMestre_ID, db);
//                        var valorPago = pg.Sum(p => p.valorPagamento);

//                        var bms = new BancoMovimento().ObterPorCPAG(obj.documentoPagarMestre_ID, db);

//                        //Atualiza Banco Movimento na exclusão do recebimento registro
//                        //da nota fiscal original
//                        if (valorPago < doc.valorBruto || valorPago == 0)
//                        {
//                            //Valor Pago ainda é menor que o líquido da Contas a Pagar
//                            //Atualizar no banco movimento apenas o saldo a pagar
//                            var saldoPagar = doc.valorBruto - valorPago;
//                            foreach (var item in bms)
//                            {
//                                item.valor = saldoPagar;
//                                item.Alterar(db);
//                            }
//                        }
//                        else
//                        {
//                            //Exclui do banco movimento
//                            //quando não houver saldo a receber
//                            foreach (var item in bms)
//                            {
//                                string Erro2 = "";
//                                if (!item.Excluir(item.id, ref Erro2))
//                                {
//                                    ViewBag.msg = Erro;
//                                }
//                            }
//                        }

//                        //Atualiza status do DocumentoPagarMestre
//                        if (pg.Count() == 0)
//                        {
//                            doc.StatusPagamento = 1;
//                        }
//                        else
//                        {
//                            if (valorPago >= doc.valorBruto)
//                                doc.StatusPagamento = 3;
//                            else
//                                doc.StatusPagamento = 2;
//                        }
//                        doc.Alterar(db);

//                        ViewBag.msg = "Pagamento Excluído com sucesso";
//                    }
//                    dbcxtransaction.Commit();
//                }
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
//            try
//            {
//                CarregaViewData();

//                Pagamento obj = new Pagamento().ObterPorDocumentoPagarMestreId(ID, null);
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
//            ViewData["notafiscal"] = new SelectList(new NotaFiscal().ObterTodos(), "id", "numeroNfse");
//            ViewData["banco"] = new Sistema1.Models.Banco().CarregaBancoGeral();
//            ViewData["Pessoas"] = new SelectList(new Pessoa().ObterTodos(), "id", "nome");
//        }

//        public ActionResult Index()
//        {
//            try
//            {
//                var hoje = DateTime.Now;
//                ViewData["DataInicial"] = new DateTime(hoje.Year, hoje.Month, 1).ToShortDateString();
//                ViewData["DataFinal"] = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();
//                ViewData["Pessoas"] = new SelectList(new Pessoa().ObterTodos(), "id", "nome");
//                return View(new Sistema1.Models.PagamentoFiltro());
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }

//        public ActionResult Listas(JqGridRequest request)
//        {
//            string Valorpessoa_id = Request.QueryString["pessoa_id"];
//            string emaberto = Request.QueryString["emaberto"];
//            string ValorrazaoFornecedor = Request.QueryString["razaoFornecedor"];
//            string Valorcnpj = Request.QueryString["cnpj"];
//            string Valorcpf = Request.QueryString["cpf"];
//            string ValornumeroDoc = Request.QueryString["numeroDoc"];
//            string ValordataEmissaoDocIni = Request.QueryString["dataEmissaoDocIni"];
//            string ValordataEmissaoDocFim = Request.QueryString["dataEmissaoDocFim"];
//            string ValordataVencimentoDocIni = Request.QueryString["dataVencimentoDocIni"];
//            string ValordataVencimentoDocFim = Request.QueryString["dataVencimentoDocFim"];
//            string Valorhistorico = Request.QueryString["historico"];
//            string Valorbanco_id = Request.QueryString["banco_id"];
//            string ValorValorTotalIni = Request.QueryString["ValorTotalIni"];
//            string ValorValorTotalFim = Request.QueryString["ValorTotalFim"];
//            string ValorValorPagamentoIni = Request.QueryString["ValorPagamentoIni"];
//            string ValorValorPagamentoFim = Request.QueryString["ValorPagamentoFim"];

//            var ret = new List<DocumentoPagarParcela>();
//            if (emaberto == "true")
//            {
//                ret = new DocumentoPagarParcela().ObterTodosEmAberto();
//            }
//            else
//            {
//                ret = new DocumentoPagarParcela().ObterTodosPagos();
//            }

//            int Valorpessoa_idAux;
//            if (int.TryParse(Valorpessoa_id, out Valorpessoa_idAux))
//                ret = ret.Where(p => p.DocumentoPagarMestre.pessoa_id == Valorpessoa_idAux).ToList();

//            int ValornumeroDocAux;
//            if (int.TryParse(ValornumeroDoc, out ValornumeroDocAux))
//                ret = ret.Where(p => p.DocumentoPagarMestre.numeroDocumento == ValornumeroDocAux).ToList();

//            DateTime ValordataEmissaoDocIniAux;
//            if (DateTime.TryParse(ValordataEmissaoDocIni, out ValordataEmissaoDocIniAux))
//                ret = ret.Where(p => p.DocumentoPagarMestre.dataDocumento.Date >= ValordataEmissaoDocIniAux.Date).ToList();

//            DateTime ValordataEmissaoDocFimAux;
//            if (DateTime.TryParse(ValordataEmissaoDocFim, out ValordataEmissaoDocFimAux))
//                ret = ret.Where(p => p.DocumentoPagarMestre.dataDocumento.Date <= ValordataEmissaoDocFimAux.Date).ToList();

//            DateTime ValordataVencimentoDocIniAux;
//            if (DateTime.TryParse(ValordataVencimentoDocIni, out ValordataVencimentoDocIniAux))
//                ret = ret.Where(p => p.vencimento.Date >= ValordataVencimentoDocIniAux.Date).ToList();

//            DateTime ValordataVencimentoDocFimAux;
//            if (DateTime.TryParse(ValordataVencimentoDocFim, out ValordataVencimentoDocFimAux))
//                ret = ret.Where(p => p.vencimento.Date <= ValordataVencimentoDocFimAux.Date).ToList();

//            var grid = new List<DocumentoPagarMestreEmAbertoView>();
//            foreach (var item in ret)
//            {
//                string auxsituacao = "1 - Em Aberto";

//                switch (item.DocumentoPagarMestre.StatusPagamento)
//                {
//                    case DocumentoPagarMestre.DOCEMABERTO:
//                        break;
//                    case DocumentoPagarMestre.DOCPAGOTOTAL:
//                        auxsituacao = "3 - Pago Total";
//                        break;
//                    case DocumentoPagarMestre.DOCPAGOPARC:
//                        auxsituacao = "2 - Pago Parcialmente";
//                        break;
//                    default:
//                        break;
//                }

//                var novo = new DocumentoPagarMestreEmAbertoView()
//                {
//                    id = item.id.ToString(),
//                    numeroDocumento = item.DocumentoPagarMestre.numeroDocumento,
//                    razaoFornecedor = item.DocumentoPagarMestre.Pessoa.nome,
//                    dataEmissaoDoc = item.DocumentoPagarMestre.dataDocumento,
//                    dataVencimentoDoc = item.vencimento,
//                    valorTotal = item.valor,
//                    status = auxsituacao,
//                    valorPagamento = new Pagamento().ObterTodosPorDocumentoPagarMestreId(item.id).Sum(p => p.valorPagamento)
//                };

//                grid.Add(novo);
//            }






//            int totalRecords = 0;



//            totalRecords = ret.Count();

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
//            //Table with rows data



//            var lista = grid.OrderBy(p => p.dataEmissaoDoc).Skip(12 * request.PageIndex).Take(12).ToList();

//            foreach (var item in lista)
//            {
//                string dta1 = "";
//                string dta2 = "";

//                if (item.dataVencimentoDoc != null)
//                    dta1 = item.dataVencimentoDoc.Value.ToShortDateString();

//                if (item.dataPagamento != null)
//                    dta2 = item.dataPagamento.Value.ToShortDateString();

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.numeroDocumento,
//                    item.razaoFornecedor,
//                    item.dataEmissaoDoc.ToShortDateString(),
//                    dta1,
//                    item.valorTotal,
//                    item.valorPagamento,
//                    item.status                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }

//        public JsonResult pagamentos(int id)
//        {
//            var rets = new Pagamento().ObterTodosPorDocumentoPagarMestreId(id);
//            StringBuilder table = new StringBuilder();

//            table.AppendLine("<div class='row' class='form-group'>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("Banco");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("Conta Corrente");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("Data Pagamento");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("Valor Pago");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("Histórico");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("Opção");
//            table.Append("</div>");
//            table.Append("</div>");

//            foreach (var item in rets)
//            {
//                table.AppendLine("<div class='row' class='form-group'>");
//                table.Append("<div class='col-md-2'>");
//                table.Append(item.banco.codigoBanco);
//                table.Append("</div>");
//                table.Append("<div class='col-md-2'>");
//                table.Append(item.banco.contaCorrente);
//                table.Append("</div>");
//                table.Append("<div class='col-md-2'>");
//                table.Append(item.dataPagamento.ToShortDateString());
//                table.Append("</div>");
//                table.Append("<div class='col-md-2'>");
//                table.Append(item.valorPagamento.ToString("n"));
//                table.Append("</div>");
//                table.Append("<div class='col-md-2'>");
//                table.Append(item.historico);
//                table.Append("</div>");
//                table.Append("<div class='col-md-2'>");
//                table.Append("<input type='button' class='btn-danger' onClick='Remove(" + item.id + ");' value='Excluir' />");
//                table.Append("</div>");
//                table.Append("</div>");
//            }

//            table.AppendLine("<div class='row' class='form-group'>");

//            table.Append("<div class='col-md-2'>");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>Total");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append(rets.Sum(p => p.valorPagamento).ToString("n"));
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("</div>");
//            table.Append("<div class='col-md-2'>");
//            table.Append("</div>");

//            table.Append("</div>");

//            var situacao = "Em Aberto";
//            var docs = rets.FirstOrDefault();

//            if (docs != null)

//                if (docs.documentoPagarMestre.StatusPagamento.Equals(2))
//                {
//                    situacao = "Pago Parcialmente";
//                }
//                else
//                {
//                    situacao = "Pago Total";
//                }

//            return Json(new { obj = table.ToString(), situacao = situacao }, JsonRequestBehavior.AllowGet);

//        }




//    }
//}
