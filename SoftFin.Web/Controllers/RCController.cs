using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RCController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public class paramGrid
        {
            public bool emaberto { get; set; }
            public DateTime? dataInicial { get; set; }
            public DateTime? dataFinal { get; set; }
            public decimal? valorInicial { get; set; }
            public decimal? valorFinal { get; set; }
        }

        public JsonResult ObterTodos(paramGrid pg)
        {
            var ret = new List<NotaFiscal>();
            if (pg.emaberto)
            {
                ret = new NotaFiscal().ObterTodosEmAberto(_paramBase);
            }
            else
            {
                ret = new NotaFiscal().ObterTodosRecebidos(_paramBase);
            }

            if (pg.dataInicial != null)
            {
                var dat = SoftFin.Utils.UtilSoftFin.TiraHora(pg.dataInicial.Value);
                ret = ret.Where(p => p.dataVencimentoNfse >= dat).ToList(); 
            }

            if (pg.dataFinal != null)
            {
                var dat = SoftFin.Utils.UtilSoftFin.TiraHora(pg.dataFinal.Value).AddDays(1);
                ret = ret.Where(p => p.dataVencimentoNfse < dat).ToList(); 
            }


            if (pg.valorInicial != null)
            {
                ret = ret.Where(p => p.valorNfse >= pg.valorInicial).ToList(); 
            }

            if (pg.valorInicial != null)
            {
                ret = ret.Where(p => p.valorNfse <= pg.valorFinal).ToList(); 
            }

            return Json(ret.Select(obj => new
            {
                obj.id,
                obj.numeroNfse,
                dataEmissaoNfse = obj.dataEmissaoNfse.ToString("o"),
                dataVencimentoNfse = obj.dataVencimentoNfse.ToString("o"),
                obj.valorLiquido,
                obj.valorNfse,
                valorRecebido = new Recebimento().ObterTodosPorNFSeId(obj.id, _paramBase).Sum(p => p.valorRecebimento),
                situacao = VerSituacao(obj.SituacaoRecebimento),
                obj.SituacaoRecebimento,
                obj.NotaFiscalPessoaTomador.cnpjCpf, 
                obj.NotaFiscalPessoaTomador.razao,
                obj.TipoFaturamento,
                Numero = (obj.OrdemVenda == null) ? "" :obj.OrdemVenda.Numero.ToString()
            }), JsonRequestBehavior.AllowGet);
        }

        public string VerSituacao(int situacaoRecebimento)
        {
            string auxsituacao = "1 - Em Aberto";

            switch (situacaoRecebimento)
            {
                case NotaFiscal.NFEMABERTO:
                    break;
                case NotaFiscal.NFRECEBIDATOTAL:
                    auxsituacao = "3 - Recebida Total";
                    break;
                case NotaFiscal.NFRECEBIDAPARC:
                    auxsituacao = "2 - Recebida Parcialmente";
                    break;
                default:
                    break;
            }
            return auxsituacao;
        }



        public JsonResult ObterPorId(int id)
        {

            NotaFiscal nf = new NotaFiscal().ObterPorId(id);
            Recebimento obj = new Recebimento();
            obj.dataRecebimento = SoftFin.Utils.UtilSoftFin.DateBrasilia();
            obj.notaFiscal_ID = id;
            obj.banco_ID = new Banco().ObterPrincipal(_paramBase).id;
            obj.historico = "Recebimento de Nota Fiscal " + nf.numeroNfse  + " - " + nf.NotaFiscalPessoaTomador.cnpjCpf + " - " + nf.NotaFiscalPessoaTomador.razao ;
            obj.estabelecimento_id = _estab;
            
             var rec = new Recebimento().ObterTodosPorNFSeId(id,_paramBase);

            return Json(new {
                nf.TipoFaturamento,
                Numero = (nf.OrdemVenda == null) ? "" : nf.OrdemVenda.Numero.ToString(),
                obj.banco_ID, 
                dataRecebimento = obj.dataRecebimento.ToString("o"),
                obj.estabelecimento_id,
                obj.historico,
                obj.id,
                obj.notaFiscal_ID,
                obj.valorRecebimento,
                obj.tipodocumento_id,

                idnf = nf.id,
                nf.numeroNfse,
                cliente = nf.NotaFiscalPessoaTomador.razao,
                dataEmissaoNfse = nf.dataEmissaoNfse.ToString("o"),
                dataVencimentoNfse = nf.dataVencimentoNfse.ToString("o"),
                nf.valorLiquido,
                nf.valorNfse,
                valorRecebido = new Recebimento().ObterTodosPorNFSeId(obj.id, _paramBase).Sum(p => p.valorRecebimento),
                situacao = VerSituacao(nf.SituacaoRecebimento),
                nf.SituacaoRecebimento,
                nf.NotaFiscalPessoaTomador.cnpjCpf, 
                nf.NotaFiscalPessoaTomador.razao,
                recebimento = rec.Select(p => new
                {
                    p.id,
                    p.banco.codigoBanco,
                    p.banco.contaCorrente,
                    dataRecebimento = p.dataRecebimento.ToShortDateString(),
                    p.valorRecebimento,
                    p.historico,
                    p.estabelecimento_id,
                    tipodocumento = p.TipoDocumento == null? "": p.TipoDocumento.descricao,
                    p.tipodocumento_id,
                    valorRecebido = new Recebimento().ObterTodosPorNFSeId(p.id, _paramBase).Sum(pi => pi.valorRecebimento),
                    situacao = VerSituacao(p.notaFiscal.SituacaoRecebimento),
                    p.notaFiscal.SituacaoRecebimento
                })
            }, JsonRequestBehavior.AllowGet);

        }
        //public JsonResult ObterRecebimentosPorId(int id)
        //{
        //    var item = new Recebimento().ObterTodosPorNFSeId(id);

        //    return Json(item.Select(obj => new
        //    {
        //        obj.banco.codigoBanco,
        //        obj.banco.contaCorrente,
        //        dataRecebimento = obj.dataRecebimento.ToShortDateString(),
        //        obj.valorRecebimento,
        //        obj.historico,
        //        valorRecebido = new Recebimento().ObterTodosPorNFSeId(obj.id).Sum(p => p.valorRecebimento),
        //        situacao = VerSituacao(obj.notaFiscal.SituacaoRecebimento),
        //        obj.notaFiscal.SituacaoRecebimento
        //    }), JsonRequestBehavior.AllowGet);

        //}

        public ActionResult Salvar(Recebimento obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                if (obj.banco_ID == 0)
                {
                    objErros.Add("Informe o banco.");
                }
                
                if (obj.tipodocumento_id == null)
                {
                    objErros.Add("Informe o Tipo de documento.");
                }
                
                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }



                DbControle db = new DbControle();
                var TipoManual = new OrigemMovimento().TipoManual(_paramBase);
                int idorigemMovimento = new OrigemMovimento().ObterTodos(_paramBase).Where(x => x.Modulo == "Recebimento").FirstOrDefault().id;

                using (var dbcxtransaction = db.Database.BeginTransaction())
                { 
                    if (obj.Incluir(_paramBase, db))
                    {
                        BancoMovimento bancoMovimentoNew = new BancoMovimento();
                        var nf = new NotaFiscal().ObterPorId(obj.notaFiscal_ID, db);

                        nf.banco_id = obj.banco_ID;
                        bancoMovimentoNew.banco_id = nf.banco_id.Value;
                        bancoMovimentoNew.data = obj.dataRecebimento;
                        bancoMovimentoNew.valor = obj.valorRecebimento;
                        bancoMovimentoNew.tipoDeMovimento_id = new TipoMovimento().TipoEntrada(db, _paramBase);
                        bancoMovimentoNew.tipoDeDocumento_id = obj.tipodocumento_id.Value;
                        bancoMovimentoNew.origemmovimento_id = idorigemMovimento;
                        bancoMovimentoNew.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("01.01", db).id;
                        bancoMovimentoNew.historico = "NF Nº" + nf.numeroNfse.ToString();
                        bancoMovimentoNew.notafiscal_id = null;
                        bancoMovimentoNew.recebimento_id = obj.id;
                        bancoMovimentoNew.Incluir(bancoMovimentoNew, _paramBase, db);



                        // Inicio Lançamento Contabil
                        var idCredito = 0;
                        var idDebito = 0;
                        var ccLC = new LancamentoContabil();
                        var ccDebito = new LancamentoContabilDetalhe();
                        var ccCredito = new LancamentoContabilDetalhe();


                        var pcf = new PessoaContaContabil().ObterPorPessoa(nf.OrdemVenda.pessoas_ID, db);

                        if (pcf != null)
                        {
                            if (pcf.contaContabilReceberPadrao_id != null)
                            {
                                idCredito = pcf.contaContabilReceberPadrao_id.Value;
                            }
                        }

                        var ecf = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);
                        if (ecf != null)
                        {
                            if (ecf.ContaContabilRecebimento_id != 0)
                            {
                                idDebito = ecf.ContaContabilRecebimento_id;
                            }
                        }

                        if (idCredito != 0 && idDebito != 0)
                        {
                            ccLC.data = obj.dataRecebimento;
                            ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                            ccLC.estabelecimento_id = _paramBase.estab_id;
                            ccLC.historico = bancoMovimentoNew.historico;
                            ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                            ccLC.origemmovimento_id = idorigemMovimento;
                            ccLC.UnidadeNegocio_ID = nf.OrdemVenda.unidadeNegocio_ID;

                            ccDebito.contaContabil_id = idDebito;
                            ccDebito.DebitoCredito = "D";
                            ccDebito.valor = obj.valorRecebimento;

                            ccCredito.contaContabil_id = idCredito;
                            ccCredito.DebitoCredito = "C";
                            ccCredito.valor = obj.valorRecebimento;
                        }
                        //Fim Lançamento Contabil




                        //Atualiza data/valor/origem de recebimento no Banco Movimento
                        var bms = new BancoMovimento().ObterPorNFES(obj.notaFiscal_ID, db,_paramBase).FirstOrDefault();
                        var rc = new Recebimento().ObterTodosPorNFSeId(obj.notaFiscal_ID, _paramBase , db);
                        var valorRec = rc.Sum(p => p.valorRecebimento);

                        //Inclui no banco movimento lançamento referente ao recebimento feito
                        //BancoMovimentoConfig bancoMovimentoConfig = new BancoMovimento().Obter();
                        int? idNf = nf.id;
                        //BancoMovimento bancoMovimento = new BancoMovimento().ObterTodos(_paramBase).Where(p => p.notafiscal_id == idNf).FirstOrDefault();
                                   

                        //Atualiza Banco Movimento na Inclusão do Recebimento
                        if (valorRec < nf.valorLiquido)
                        {
                            //Valor Recebido ainda é menor que o líquido da nota
                            //Atualizar no banco movimento apenas o saldo a receber
                            nf.SituacaoRecebimento = 2;

                            var saldoReceber = nf.valorLiquido - valorRec;
                            if (bms == null)
                            {
                                bms = new BancoMovimento();
                                bms.banco_id = nf.banco_id.Value;
                                bms.data = nf.dataVencimentoNfse;
                                bms.valor = saldoReceber;
                                bms.planoDeConta_id = 1;
                                bms.tipoDeMovimento_id = new TipoMovimento().TipoEntrada(db, _paramBase);
                                bms.tipoDeDocumento_id = new TipoDocumento().TipoNotaPromissoria();
                                bms.origemmovimento_id = idorigemMovimento;
                                bms.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("01.01", db).id;
                                bms.historico = "NF Nº" + nf.numeroNfse.ToString();
                                bms.notafiscal_id = nf.id;
                                bms.recebimento_id = null;
                                bms.Incluir(_paramBase, db);
                            }
                            else
                            {
                                bms.valor = saldoReceber;
                                bms.banco_id = obj.banco_ID;
                                bms.Alterar(_paramBase, db);
                            }
                        }
                        else
                        {
                            string Erro = "";
                            nf.SituacaoRecebimento = 3;

                            if (bms != null)
                            {
                                if (!bms.Excluir(bms.id, ref Erro, _paramBase))
                                {
                                    if (Erro != "")
                                    {
                                        dbcxtransaction.Rollback();
                                        throw new Exception("Falha na salva recalculo de BM: " + Erro);
                                    }
                                }
                            }
                        }

                        //Inicio Lançamento Contabil
                        if (idCredito != 0 && idDebito != 0)
                        {
                            var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                            ccLC.codigoLancamento = numeroLcto;
                            ccLC.notafiscal_id = nf.id;
                            ccLC.recebimento_id = obj.id;
                            ccLC.Incluir(_paramBase, db);
                            ccDebito.lancamentoContabil_id = ccLC.id;
                            ccCredito.lancamentoContabil_id = ccLC.id;
                            ccDebito.Incluir(_paramBase, db);
                            ccCredito.Incluir(_paramBase, db);
                        }
                        //Fim Lançamento Contabil

                        nf.Alterar(nf, _paramBase, db);
                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        throw new Exception("Erro ao salvar o recebimento");
                    }
                    dbcxtransaction.Commit();
                }
                
                var ret = new Recebimento().ObterTodosPorNFSeId(obj.notaFiscal_ID, _paramBase);

                return Json(
                    new { CDStatus = "OK", 
                            DSMessage = "Registro salvo com sucesso"
                        }
                        , JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(Recebimento obj)
        {

            try
            {

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var TipoManual = new OrigemMovimento().TipoManual(_paramBase);
                var db = new DbControle();
                var erro = "";
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    obj = obj.ObterPorId(obj.id, db,_paramBase);

                    //Exclui do banco movimento registro relacionado ao recebimento
                    //que está sendo excluído
                    var bmr = new BancoMovimento().ObterPorRecebimento(obj.id, db,_paramBase);
                    foreach (var item in bmr)
                    {

                        if (!item.Excluir(item.id, ref erro,_paramBase, db))
                        {
                            if (erro != "")
                            {
                                dbcxtransaction.Rollback();
                                throw new Exception("Erro ao excluir BM") ;
                            }
                        }
                    }


                    //  Exclusão LancamentoContabil Inicio
                    var objLcs = new LancamentoContabil().ObterPorNotaFiscalRecebimento(obj.id, _paramBase, db);

                    foreach (var itemLC in objLcs)
                    {
                        var errolc = "";
                        itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                        if (errolc != "")
                            throw new Exception(errolc);
                    }
                    //Fim


                    //Exclui Recebimento
                    if (obj.Excluir(obj.id, ref erro,_paramBase, db))
                    {
                        if (erro != "")
                        {
                            dbcxtransaction.Rollback();
                            throw new Exception("Erro ao excluir BM") ;
                        }
                    }

                    //Atualiza data/valor/origem de recebimento no Banco Movimento
                    var bms = new BancoMovimento().ObterPorNFES(obj.notaFiscal_ID, db,_paramBase).FirstOrDefault();
                    var rc = new Recebimento().ObterTodosPorNFSeId(obj.notaFiscal_ID, _paramBase, db);
                    var valorRec = rc.Sum(p => p.valorRecebimento);

                    var nf = new NotaFiscal().ObterPorId(obj.notaFiscal_ID, db);
                    int idorigemMovimento = new OrigemMovimento().ObterTodos(_paramBase).Where(x => x.Modulo == "Recebimento").FirstOrDefault().id;

                    //Atualiza Banco Movimento na Inclusão do Recebimento
                    if (valorRec < nf.valorLiquido)
                    {
                        //Valor Recebido ainda é menor que o líquido da nota
                        //Atualizar no banco movimento apenas o saldo a receber
                        if (valorRec == 0)
                        {
                            nf.SituacaoRecebimento = 1;
                        }
                        else
                        {
                            nf.SituacaoRecebimento = 2;
                        }
                        var saldoReceber = nf.valorLiquido - valorRec;
                        if (bms == null)
                        {
                            bms = new BancoMovimento();
                            bms.banco_id = nf.banco_id.Value;
                            bms.data = nf.dataVencimentoNfse;
                            bms.valor = saldoReceber;
                            bms.planoDeConta_id = 1;
                            bms.tipoDeMovimento_id = new TipoMovimento().TipoEntrada(db, _paramBase);
                            bms.tipoDeDocumento_id = new TipoDocumento().TipoNotaPromissoria();
                            bms.origemmovimento_id = idorigemMovimento;
                            bms.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("01.01", db).id;
                            bms.historico = "NF Nº" + nf.numeroNfse.ToString();
                            bms.notafiscal_id = nf.id;
                            bms.recebimento_id = null;
                            bms.Incluir(_paramBase,db);
                        }
                        else
                        {
                            bms.valor = saldoReceber;
                            bms.banco_id = obj.banco_ID;
                            bms.Alterar(_paramBase, db);
                        }
                    }
                    else
                    {
                        string Erro = "";
                        nf.SituacaoRecebimento = 3;
                        if (bms != null)
                            if (!bms.Excluir(bms.id, ref Erro, _paramBase))
                            {
                                if (Erro != "")
                                {
                                    dbcxtransaction.Rollback();
                                    throw new Exception("Falha na salva recalculo de BM: " + Erro);
                                }
                            }
                    }

                    nf.Alterar(nf, _paramBase, db);

                   
                    dbcxtransaction.Commit();
                }
    
                var ret = new Recebimento().ObterTodosPorNFSeId(obj.notaFiscal_ID,_paramBase);

                return Json(
                    new { CDStatus = "OK", 
                            DSMessage = "Registro excluido com sucesso",
                            objs = ret.Select(p => new
                            {
                                p.banco.codigoBanco,
                                p.banco.contaCorrente,
                                dataRecebimento = p.dataRecebimento.ToShortDateString(),
                                p.valorRecebimento,
                                p.tipodocumento_id,
                                p.historico,
                                valorRecebido = new Recebimento().ObterTodosPorNFSeId(p.id,_paramBase).Sum(pi => pi.valorRecebimento),
                                situacao = VerSituacao(p.notaFiscal.SituacaoRecebimento),
                                p.notaFiscal.SituacaoRecebimento
                            })}, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ObterTipoDocumento()
        {
            var objs = new TipoDocumento().ObterTodos();
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = int.Parse(p.Value),
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
