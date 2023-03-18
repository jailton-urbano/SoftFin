using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using BoletoNet;

using System.Text;

namespace SoftFin.Web.Controllers
{
    public class PGController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var AReceber = new Pagamento().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valorPagamento).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + AReceber }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult obtemFiltro()
        {
            var filtro = new
            {
                vencimentoIni = DateTime.Parse(DateTime.Now.ToShortDateString()).AddDays(-30).ToString("o"),
                vencimentoFim = DateTime.Parse(DateTime.Now.ToShortDateString()).ToString("o"),
                valorIni = 0,
                valorFim = 999999999.99,
                emAberto = false
            };
            return Json(filtro, JsonRequestBehavior.AllowGet);
        }

        public class dtoFiltro
        {
            public DateTime? vencimentoIni { get; set; }
            public DateTime? vencimentoFim { get; set; }
            public Decimal? valorIni { get; set; }
            public Decimal? valorFim { get; set; }
            public bool emAberto { get; set; }
        }

        private class dtoAux
        {
            public string numeroDocumento { get; set; }
            public string dataLancamento { get; set; }
            public string nome { get; set; }
            public string vencimento { get; set; }
            public string situacao { get; set; }
            public string aprovado { get; set; }
            public decimal valor { get; set; }
            public int parcela { get; set; }
            public decimal valorPagamento { get; set; }
            public int id { get; set; }

            public string historico { get; set; }
            public int qtdImagens { get; set; }
            public int DocumentoPagarMestre_id { get; set; }
        }

        [HttpPost]
        public JsonResult ObtemEntidade(int id)
        {
            var obj = new DocumentoPagarParcela().ObterPorId(id, _paramBase);
            if (obj == null)
                obj = new DocumentoPagarParcela();

            var retorno = (new
            {
                obj.DocumentoPagarMestre_id,
                obj.historico,
                nome = obj.DocumentoPagarMestre.Pessoa.nome,
                obj.id,
                obj.DocumentoPagarMestre.estabelecimento_id,
                obj.lotePagamentoBanco,
                obj.parcela,
                obj.statusPagamento,
                obj.usuarioAutorizador_id,
                obj.valor,
                

                vencimento = obj.vencimento.ToString("o"),
                vencimentoPrevisto = obj.vencimentoPrevisto.ToString("o"),
                Pagamentos = new Pagamento().ObterTodosPorDocumentoPagarParcelaId(obj.id, _paramBase).
                        Select(p => new
                        {
                            p.banco_ID,
                            banco_nome = p.banco.codigoBanco + " " + p.banco.nomeBanco + " " + p.banco.agencia + " " + p.banco.contaCorrente + "-" + p.banco.agenciaDigito,
                            dataPagamento = p.dataPagamento.ToString("o"),
                            p.DocumentoPagarParcela_ID,
                            p.estabelecimento_id,
                            p.historico,
                            p.valorPagamento,
                            p.id,
                            p.multa,
                            p.juros,
                            p.contaContabilCredito_id,
                            contaContabilCredito_desc = (p.contaContabilCredito_id == null) ? "": string.Format("{0} {1}", p.ContaContabilCredito.codigo, p.ContaContabilCredito.descricao)
                        }
                        )
            });

            return Json(new { CDStatus = "OK", DSMessage = ViewBag.msg, objs = retorno }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult ObtemTodos(dtoFiltro data)
        {


            var objs = new DocumentoPagarParcela().ObterTodos(_paramBase);

            if (data.vencimentoIni != null)
            {

                data.vencimentoIni = new DateTime(
                    data.vencimentoIni.Value.Year,
                    data.vencimentoIni.Value.Month,
                    data.vencimentoIni.Value.Day);

                objs = objs.Where(p => p.vencimentoPrevisto >= data.vencimentoIni).ToList();
            }

            if (data.vencimentoFim != null)
            {

                data.vencimentoFim = new DateTime(
                    data.vencimentoFim.Value.Year,
                    data.vencimentoFim.Value.Month,
                    data.vencimentoFim.Value.Day, 23, 59, 0);

                objs = objs.Where(p => p.vencimentoPrevisto <= data.vencimentoFim).ToList();
            }

            if (data.valorIni != null)
            {
                objs = objs.Where(p => p.valor >= data.valorIni.Value).ToList();
            }

            if (data.valorFim != null)
            {
                objs = objs.Where(p => p.valor <= data.valorFim.Value).ToList();
            }


            if (data.emAberto)
            {
                objs = objs.Where(p => p.statusPagamento != 3).ToList();
            }
            else
            {
                objs = objs.Where(p => p.statusPagamento == 3).ToList();
            }



            var retorno = new List<dtoAux>();

            foreach (var item in objs)
            {
                var objret = new dtoAux();

                if (item.usuarioAutorizador_id == null)
                    objret.aprovado = "Não";
                else
                    objret.aprovado = "Sim";


                objret.situacao = "1 - Em Aberto";

                switch (item.statusPagamento)
                {
                    case DocumentoPagarMestre.DOCEMABERTO:
                        break;
                    case DocumentoPagarMestre.DOCPAGOTOTAL:
                        objret.situacao = "3 - Pago Total";
                        break;
                    case DocumentoPagarMestre.DOCPAGOPARC:
                        objret.situacao = "2 - Pago Parcialmente";
                        break;
                    default:
                        objret.situacao = "Outro";
                        break;
                }

                objret.nome = item.DocumentoPagarMestre.Pessoa.nome;
                objret.valor = item.valor;
                objret.dataLancamento = item.DocumentoPagarMestre.dataLancamento.ToString("o");
                objret.id = item.id;
                objret.numeroDocumento = item.DocumentoPagarMestre.numeroDocumento.ToString();
                objret.vencimento = item.vencimentoPrevisto.ToString("o");
                objret.parcela = item.parcela;
                objret.valorPagamento = new Pagamento().ObterTodosPorDocumentoPagarParcelaId(item.id, _paramBase).Sum(p => p.valorPagamento);
                objret.historico = item.historico;
                objret.qtdImagens = new DocumentoPagarArquivo().ObterPorCPAG(item.DocumentoPagarMestre_id, _paramBase).Count();
                objret.DocumentoPagarMestre_id = item.DocumentoPagarMestre_id;
                retorno.Add(objret);
            }


            return Json(

                retorno.Select(p => new
                {
                    p.id,
                    p.aprovado,
                    p.dataLancamento,
                    p.nome,
                    p.numeroDocumento,
                    p.parcela,
                    p.situacao,
                    p.valor,
                    p.valorPagamento,
                    p.vencimento,
                    p.historico,
                    p.qtdImagens,
                    p.DocumentoPagarMestre_id
                })
                , JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaBanco()
        {
            var con1 = new SoftFin.Web.Models.Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return Json(listret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Salvar(Pagamento obj)
        {
            try
            {
                Pagamento pagamento = new Pagamento();
                DbControle db = new DbControle();
                var TipoManual = new OrigemMovimento().TipoManual(_paramBase);
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    if (pagamento.Incluir(obj, _paramBase, db))
                    {

                        //Atualiza data/valor/origem de recebimento no Banco Movimento
                        var bms = new BancoMovimento().ObterPorCPAGParcela(obj.DocumentoPagarParcela_ID, _paramBase, db);
                        var doc = new DocumentoPagarParcela().ObterPorId(obj.DocumentoPagarParcela_ID, db, _paramBase);

                        var pg = pagamento.ObterTodosPorDocumentoPagarParcelaId(obj.DocumentoPagarParcela_ID, _paramBase, db);
                        //var pgDet = new DocumentoPagarDetalhe().ObterPorCPAG(obj.documentoPagarMestre_ID).FirstOrDefault();
                        var valorPago = pg.Sum(p => p.valorPagamento);

                        //Inclui no banco movimento lançamento referente ao pagamento feito

                        int? idDocPag = doc.id;
                        BancoMovimento bancoMovimento = new BancoMovimento().ObterTodos(_paramBase).Where(p => p.DocumentoPagarParcela_id == idDocPag).FirstOrDefault();
                        OrigemMovimento origemMovimento = new OrigemMovimento().ObterTodos(_paramBase).Where(x => x.Modulo == "Pagamento").FirstOrDefault();
                        TipoMovimento tipoMov = new TipoMovimento().ObterTodos(_paramBase).Where(x => x.Codigo == "SAI").FirstOrDefault();

                        BancoMovimento bancoMovimentoNew = new BancoMovimento();
                        bancoMovimentoNew.banco_id = obj.banco_ID;
                        bancoMovimentoNew.data = obj.dataPagamento;
                        bancoMovimentoNew.valor = obj.valorPagamento;
                        bancoMovimentoNew.planoDeConta_id = doc.DocumentoPagarMestre.planoDeConta_id;
                        bancoMovimentoNew.tipoDeMovimento_id = new TipoMovimento().TipoSaida(_paramBase);
                        bancoMovimentoNew.tipoDeDocumento_id = new TipoDocumento().TipoCreditoConta();
                        
                        bancoMovimentoNew.origemmovimento_id = origemMovimento.id;
                        bancoMovimentoNew.historico = "CPAG Documento PG Nº " + doc.DocumentoPagarMestre.numeroDocumento.ToString();
                        bancoMovimentoNew.notafiscal_id = null;
                        bancoMovimentoNew.DocumentoPagarParcela_id = obj.DocumentoPagarParcela_ID;
                        bancoMovimentoNew.pagamento_id = obj.id;
                        bancoMovimentoNew.usuarioinclusaoid = _usuarioobj.id;
                        bancoMovimentoNew.dataInclusao = DateTime.Now;
                        //Inclui novo banco movimento
                        bancoMovimentoNew.Incluir(bancoMovimentoNew, _paramBase, db);

                        // Inicio Lançamento Contabil
                        var idCredito = 0;
                        var idDebito = 0;

                        var pcf = new PessoaContaContabil().ObterPorPessoa(doc.DocumentoPagarMestre.pessoa_id);

                        if (pcf == null)
                        {
                            var ecf = new EmpresaContaContabil().ObterPorEmpresa(_paramBase, db);
                            if (ecf != null)
                            {
                                idDebito = ecf.ContaContabilPagamento_id;
                            }
                        }
                        else
                        {
                            if (pcf.contaContabilDespesaPadrao_id != null)
                            {
                                idDebito = pcf.contaContabilPagarPadrao_id.Value;
                            }
                            else
                            {
                                var ecf = new EmpresaContaContabil().ObterPorEmpresa(_paramBase, db);
                                if (ecf != null)
                                {
                                    idDebito = ecf.ContaContabilPagamento_id;
                                }
                            }
                        }

                        if (obj.contaContabilCredito_id != null)
                        {
                            if (obj.contaContabilCredito_id != 0)
                            {
                                idCredito = obj.contaContabilCredito_id.Value;

                                if (idCredito != 0 && idDebito != 0)
                                {
                                    var unds = new DocumentoPagarDetalhe().ObterPorCPAG(doc.DocumentoPagarMestre_id);

                                    foreach (var unidade in unds)
                                    {
                                        var ccLC = new LancamentoContabil();
                                        ccLC.data = obj.dataPagamento;
                                        ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                                        ccLC.estabelecimento_id = _paramBase.estab_id;
                                        ccLC.historico = bancoMovimentoNew.historico;
                                        ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                                        ccLC.origemmovimento_id = origemMovimento.id;
                                        ccLC.DocumentoPagarParcela_id = doc.id;
                                        ccLC.pagamento_id = obj.id;
                                        ccLC.UnidadeNegocio_ID = unidade.unidadenegocio_id;

                                        var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                                        ccLC.codigoLancamento = numeroLcto;
                                        ccLC.Incluir(_paramBase, db);

                                        var ccDebito = new LancamentoContabilDetalhe();
                                        ccDebito.lancamentoContabil_id = ccLC.id;
                                        ccDebito.contaContabil_id = idDebito;
                                        ccDebito.DebitoCredito = "D";
                                        
                                        decimal porc = (doc.valor * 100) / unidade.valor;
                                        decimal mult = (obj.valorPagamento / 100) * porc;
                                        mult = decimal.Round(mult, 2);
                                        ccDebito.valor = mult;

                                        ccDebito.Incluir(_paramBase, db);

                                        var ccCredito = new LancamentoContabilDetalhe();
                                        ccCredito.lancamentoContabil_id = ccLC.id;
                                        ccCredito.contaContabil_id = idCredito;
                                        ccCredito.DebitoCredito = "C";
                                        ccCredito.valor = mult;
                                        ccCredito.Incluir(_paramBase, db);
                                    }
                                }
                            }
                        }
                        //Fim Lançamento Contabil



                        //Atualiza Banco Movimento na Inclusão do Pagamento
                        if (valorPago < doc.valor || valorPago == 0)
                        {
                            //Valor Pago ainda é menor que o Valor Bruto do Contas a Pagar
                            //Atualizar no banco movimento apenas o saldo a pagar
                            var saldoPagar = doc.valor - valorPago;
                            foreach (var item in bms)
                            {
                                item.valor = saldoPagar;
                                item.usuarioalteracaoid = _usuarioobj.id;
                                item.dataAlteracao = DateTime.Now;
                                item.Alterar(_paramBase, db);
                            }
                        }
                        else
                        {
                            //Exclui do banco movimento
                            //quando não houver saldo a pagar
                            foreach (var item in bms)
                            {
                                //item.valor = 0;
                                string Erro = "";
                                if (!item.Excluir(item.id, ref Erro, _paramBase))
                                {
                                    ViewBag.msg = Erro;
                                }
                            }
                        }

                        if (pg.Count() == 0)
                            doc.statusPagamento = 1;
                        else
                        {
                            if (valorPago >= doc.valor)
                                doc.statusPagamento = 3;
                            else
                                doc.statusPagamento = 2;
                        }

                        doc.Alterar(doc, db, _paramBase);


                        var parcela = new DocumentoPagarParcela().ObterPorCPAG(doc.DocumentoPagarMestre_id, db);

                        if (
                            parcela.Where(p => p.statusPagamento == 3).Count() ==
                            doc.DocumentoPagarMestre.qtdParcelas)
                        {
                            doc.DocumentoPagarMestre.StatusPagamento = 3;
                        }
                        else if (
                        parcela.Where(p => p.statusPagamento == 1).Count() ==
                        doc.DocumentoPagarMestre.qtdParcelas)
                        {
                            doc.DocumentoPagarMestre.StatusPagamento = 1;
                        }
                        else
                        {
                            doc.DocumentoPagarMestre.StatusPagamento = 2;
                        }

                        db.SaveChanges();


                        ViewBag.msg = "Pagamento atualizado com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "Pagamento já efetuado";
                    }
                    dbcxtransaction.Commit();

                }
                //DocumentoPagarMestre doc2 = new DocumentoPagarMestre();
                //obj.DocumentoPagarParcela = doc2.ObterPorId(obj.DocumentoPagarParcela.DocumentoPagarMestre);
                //return View(obj);

                return ObtemEntidade(obj.DocumentoPagarParcela_ID);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpPost]
        public JsonResult Excluir(int id)
        {
            try
            {
                string Erro = "";
                Pagamento obj = new Pagamento().ObterPorId(id, null, _paramBase);
                var TipoManual = new OrigemMovimento().TipoManual(_paramBase);
                var db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    obj = obj.ObterPorId(obj.id, db, _paramBase);

                    //Exclui do banco movimento registro relacionado ao pagamento
                    //que está sendo excluído
                    var bmr = new BancoMovimento().ObterPorPagamento(obj.id, db, _paramBase);
                    foreach (var item in bmr)
                    {
                        if (!item.Excluir(item.id, ref Erro, _paramBase, db))
                        {
                            return Json(new { CDStatus = "NOK", DSMessage = Erro }, JsonRequestBehavior.AllowGet);
                        }
                    }


                    //  Exclusão LancamentoContabil Inicio

                    var objLcs = new LancamentoContabil().ObterPorCPAGParcelaPagamento(obj.id, _paramBase, db);

                    foreach (var itemLC in objLcs)
                    {
                        var errolc = "";
                        itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                        if (errolc != "")
                            throw new Exception(errolc);
                    }

                    
                    //Fim

                    //Exclui Pagamento
                    if (!obj.Excluir(obj.id, ref Erro, _paramBase, db))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = Erro }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var doc = new DocumentoPagarParcela().ObterPorId(obj.DocumentoPagarParcela_ID, db, _paramBase);
                        BancoMovimento banco = db.BancoMovimento.Where(x => x.DocumentoPagarParcela_id == doc.id).FirstOrDefault();
                        Pagamento pagamento = new Pagamento();
                        var pg = pagamento.ObterTodosPorDocumentoPagarParcelaId(obj.DocumentoPagarParcela_ID, _paramBase, db);
                        var valorPago = pg.Sum(p => p.valorPagamento);

                        var bms = new BancoMovimento().ObterPorCPAGSemPagamento(obj.DocumentoPagarParcela_ID, _paramBase, db);

                        //Atualiza Banco Movimento na exclusão do recebimento registro
                        //da nota fiscal original
                        if (valorPago < doc.valor || valorPago == 0)
                        {
                            //Valor Pago ainda é menor que o líquido da Contas a Pagar
                            //Atualizar no banco movimento apenas o saldo a pagar
                            var saldoPagar = doc.valor - valorPago;
                            if (bms.Count() == 0)
                            {
                                var bm = new BancoMovimento();
                                bm.banco_id = doc.DocumentoPagarMestre.banco_id;
                                bm.data = doc.vencimentoPrevisto;
                                bm.valor = saldoPagar;
                                bm.planoDeConta_id = doc.DocumentoPagarMestre.planoDeConta_id;
                                bm.tipoDeMovimento_id = new TipoMovimento().TipoSaida(_paramBase);
                                bm.tipoDeDocumento_id = doc.DocumentoPagarMestre.tipodocumento_id;
                                bm.origemmovimento_id = new OrigemMovimento().TipoCPAG(_paramBase);
                                bm.historico = doc.historico;
                                bm.DocumentoPagarParcela_id = doc.id;
                                bm.recebimento_id = null;
                                bm.Incluir(_paramBase, db);
                            }
                            else
                            {
                                foreach (var item in bms)
                                {
                                    item.valor = saldoPagar;
                                    item.Alterar(_paramBase, db);
                                }
                            }
                        }
                        else
                        {
                            //Exclui do banco movimento
                            //quando não houver saldo a receber
                            foreach (var item in bms)
                            {
                                string Erro2 = "";
                                if (!item.Excluir(item.id, ref Erro2, _paramBase))
                                {
                                    ViewBag.msg = Erro;
                                }
                            }
                        }

                        //Atualiza status do DocumentoPagarMestre
                        if (pg.Count() == 0)
                        {
                            doc.statusPagamento = 1;
                        }
                        else
                        {
                            if (valorPago >= doc.valor)
                                doc.statusPagamento = 3;
                            else
                                doc.statusPagamento = 2;
                        }
                        doc.Alterar(doc, db, _paramBase);

                        var parcela = new DocumentoPagarParcela().ObterPorCPAG(doc.DocumentoPagarMestre_id, db);

                        if (
                            parcela.Where(p => p.statusPagamento == 3).Count() ==
                            doc.DocumentoPagarMestre.qtdParcelas)
                        {
                            doc.DocumentoPagarMestre.StatusPagamento = 3;
                        }
                        else if (
                        parcela.Where(p => p.statusPagamento == 1).Count() ==
                        doc.DocumentoPagarMestre.qtdParcelas)
                        {
                            doc.DocumentoPagarMestre.StatusPagamento = 1;
                        }
                        else
                        {
                            doc.DocumentoPagarMestre.StatusPagamento = 2;
                        }

                        db.SaveChanges();
                        ViewBag.msg = "Pagamento Excluído com sucesso";
                    }
                    dbcxtransaction.Commit();
                }
                return ObtemEntidade(obj.DocumentoPagarParcela_ID);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
