using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LojaFechamentoController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new LojaFechamento().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.ativo,
                dataFechamento = obj.dataFechamento.ToString("o"),
                obj.descricao,
                obj.Loja_id,
                obj.LojaCaixa_id,
                obj.LojaOperador_id,
                obj.saldoFinal,
                obj.sequencia,
                obj.valorLiquido,
                obj.valorBruto,
                obj.valorTaxas,
                obj.saldoInicial,
                obj.flgSituacao,
                loja = obj.LojaCaixa.Loja.descricao,
                operador = obj.LojaOperador.descricao,
                caixa = obj.LojaCaixa.descricao
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Incluir(int idLoja, DateTime dataFechamento)
        {

            var obj = new LojaFechamento();
            obj.dataFechamento = SoftFin.Utils.UtilSoftFin.TiraHora(dataFechamento);
            var consultaSeq = new LojaFechamento().ObterTodosPorData(obj.dataFechamento, idLoja, _paramBase, null);

            if (consultaSeq.Count() == 0)
            {
                obj.sequencia = 1;
            }
            else
            {
                obj.sequencia = consultaSeq.Max(p => p.sequencia) + 1;
            }
            obj.Loja_id = idLoja;
            obj.flgSituacao = "L";


            return Json(new
            {
                obj.id,
                obj.ativo,
                dataFechamento = obj.dataFechamento.ToString("o"),
                obj.descricao,
                obj.Loja_id,
                obj.LojaCaixa_id,
                obj.LojaOperador_id,
                obj.saldoFinal,
                obj.sequencia,
                obj.valorLiquido,
                obj.valorBruto,
                obj.saldoInicial,
                obj.valorTaxas,
                obj.flgSituacao
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new LojaFechamento().ObterPorId(id, _paramBase);

            return Json(new
            {
                obj.id,
                obj.ativo,
                dataFechamento = obj.dataFechamento.ToString("o"),
                obj.descricao,
                obj.Loja_id,
                obj.LojaCaixa_id,
                obj.LojaOperador_id,
                obj.saldoFinal,
                obj.sequencia,
                obj.valorLiquido,
                obj.valorBruto,
                obj.saldoInicial,
                obj.valorTaxas,
                obj.flgSituacao
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(LojaFechamento obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);


                if (obj.LojaCaixa_id == 0)
                {
                    objErros.Add("informe o caixa");
                }

                if (obj.LojaOperador_id == 0)
                {
                    objErros.Add("informe o operador");
                }


                if (string.IsNullOrEmpty( obj.flgSituacao) )
                {
                    objErros.Add("informe a situação");
                }

                if (string.IsNullOrWhiteSpace(obj.descricao))
                {
                    objErros.Add("informe o código da caixa");
                }

                foreach (var item in obj.LojaFechamentoCCs)
                {
                    if (item.dataPagamentoPrevisto.Year < DateTime.Now.AddYears(-3).Year)
                    {
                        objErros.Add("Verifique as datas de previsão de pagamento informadas.");
                        break;
                    }
                    
                }


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.id == 0)
                {
                    var db = new DbControle();
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        obj.dataFechamento = SoftFin.Utils.UtilSoftFin.TiraHora(obj.dataFechamento);
                        var consultaSeq = new LojaFechamento().ObterTodosPorData(obj.dataFechamento, obj.Loja_id, _paramBase, db);
                        if (consultaSeq.Count() == 0)
                        {
                            obj.sequencia = 1;
                        }
                        else
                        {
                            obj.sequencia = consultaSeq.Max(p => p.sequencia) + 1;
                        }
                        bool correto = FecharCaixa(obj, db, dbcxtransaction);
                        if (correto)
                        {
                            return Json(new { CDStatus = "OK", DSMessage = "Caixa Fechado com sucesso " + obj.dataFechamento.ToShortDateString() + " Sequencial " + obj.sequencia }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    var db = new DbControle();
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        var erro = "";
                        obj.dataFechamento = SoftFin.Utils.UtilSoftFin.TiraHora(obj.dataFechamento);

                        if (CancelaFechamento(obj, db, dbcxtransaction, ref erro))
                        {
                            bool correto = FecharCaixa(obj, db, dbcxtransaction);
                            if (correto)
                            {
                                return Json(new { CDStatus = "OK", DSMessage = "Caixa Fechado com sucesso " + obj.dataFechamento.ToShortDateString() + " Sequencial " + obj.sequencia }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool FecharCaixa(LojaFechamento obj, DbControle db, System.Data.Entity.DbContextTransaction dbcxtransaction)
        {
            var correto = false;

            if (obj.Incluir(_paramBase, db) == true)
            {
                foreach (var item in obj.LojaFechamentoCCs)
                {
                    var tipoCaixa = new OrigemMovimento().TipoCaixa(_paramBase);
                    var tipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
                    var tipoSaida = new TipoMovimento().TipoSaida(_paramBase);
                    var tipoTipoFechamentoCaixa = new TipoDocumento().TipoFechamentoCaixa();

                    if (item.valorBruto != 0)
                    {
                        var bc1 = new BancoMovimento();
                        bc1.banco_id = item.banco_id;
                        bc1.data = item.dataPagamentoPrevisto;
                        bc1.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        bc1.estab = _estab;
                        bc1.historico = item.descricao;
                        bc1.LojaFechamentoCC_id = item.id;
                        bc1.origemmovimento_id = tipoCaixa; //TODO trocar para tipo 'Caixa'
                        bc1.tipoDeMovimento_id = tipoEntrada;
                        bc1.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("01.01", db).id;
                        bc1.tipoDeDocumento_id = tipoTipoFechamentoCaixa; //TODO Relatorio de Caixa
                        bc1.usuarioinclusaoid = _usuarioobj.id;
                        bc1.valor = item.valorBruto;
                        bc1.Incluir(_paramBase, db);
                    }

                    if (item.valorTaxa != 0)
                    {
                        var bc2 = new BancoMovimento();
                        bc2.banco_id = item.banco_id;
                        bc2.data = item.dataPagamentoPrevisto;
                        bc2.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        bc2.estab = _estab;
                        bc2.historico = item.descricao + "-Tx";
                        bc2.LojaFechamentoCC_id = item.id;
                        bc2.origemmovimento_id = tipoCaixa;
                        bc2.tipoDeMovimento_id = tipoSaida;
                        bc2.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("06.02", db).id;
                        bc2.tipoDeDocumento_id = tipoTipoFechamentoCaixa;
                        bc2.usuarioinclusaoid = _usuarioobj.id;
                        bc2.valor = item.valorTaxa;
                        bc2.Incluir(_paramBase, db);
                    }

                }
                dbcxtransaction.Commit();
                correto = true;
            }
            else
            {
                dbcxtransaction.Rollback();
                correto = false;
            }

            return correto;
        }

        public JsonResult Excluir(LojaFechamento obj)
        {

            try
            {
                var objLoja = new Loja().ObterPorId(obj.Loja_id, _paramBase);
                if (objLoja == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Loja não encontrada" }, JsonRequestBehavior.AllowGet);
                if (objLoja.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                var db = new DbControle();
                var erro = "";
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    if (CancelaFechamento(obj, db, dbcxtransaction, ref erro))
                    {
                        dbcxtransaction.Commit();
                        return Json(new { CDStatus = "OK", DSMessage = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }



        private bool CancelaFechamento(LojaFechamento obj, DbControle db, System.Data.Entity.DbContextTransaction dbcxtransaction, ref string messagemErro)
        {
            var objscc = new LojaFechamentoCC().ObterTodos(obj.id, _paramBase, db);
            
            string erro = "";

            foreach (var item in objscc)
            {
                var objs = new BancoMovimento().ObterPorFechamentoCC(item.id, _paramBase, db);

                foreach (var item2 in objs)
                {
                    item2.Excluir(item2.id, ref erro, _paramBase, db);
                    if (erro != "")
                    {
                        dbcxtransaction.Rollback();
                        messagemErro = "Não foi possivel excluir Registro - " + erro;
                        return false;
                    }
                }

                item.Excluir(item.id, ref erro, _paramBase, db);

                if (erro != "")
                {
                    dbcxtransaction.Rollback();
                    messagemErro = "Não foi possivel excluir Registro - " + erro;
                    return false;

                }
            }


            if (new LojaFechamento().Excluir(obj.id, ref erro, _paramBase, db))
            {
                if (erro != "")
                {
                    dbcxtransaction.Rollback();
                    messagemErro = "Não foi possivel excluir Registro - " + erro;
                    return false;
                }
                
                return true;

            }
            else
            {
                dbcxtransaction.Rollback();
                messagemErro = "Não foi possivel excluir Registro" ;
                return false;
            }
        }

        public JsonResult ObterLoja()
        {
            var objs = new Loja().ObterTodos(_paramBase).Where(p => p.ativo == true);

            var caixa = objs.Select(p => new
                {
                    Value = p.id,
                    Text = p.descricao
                }
            );
            return Json(caixa, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTipoRecebimentoCaixa(int id, int idLoja, DateTime dataRec)
        {
            var objsCC = new List<LojaFechamentoCC>();

            if (id == 0)
            {
                var objs = new LojaTipoRecebimentoCaixa().ObterTodosPorLoja(idLoja, _paramBase).Where(p => p.ativo == true);

                foreach (var item in objs)
                {
                    var objcc = new LojaFechamentoCC();

                    var prazoVigente = new LojaTipoRecebimentoCaixaVigencia().ObterTipoVigente(item.id, dataRec,_paramBase); 

                    objcc.descricao = "(" + item.codigo + ") " + item.descricao;
                    objcc.LojaFechamento_id = 0;
                    objcc.LojaTipoRecebimentoCaixa_id = item.id;
                    objcc.tipoMovimento = "E";
                    objcc.tipoVenda = (prazoVigente.prazoDias == 0) ? "A" : "P";
                    objcc.prazoDias = prazoVigente.prazoDias;
                    objcc.taxa = prazoVigente.taxa;
                    objcc.valorBruto = 0;
                    objcc.valorLiquido = 0;
                    objcc.valorTaxa = 0;

                    objcc.dataPagamentoPrevisto = dataRec.AddDays(prazoVigente.prazoDias);
                    objcc.banco_id = item.banco_id;
                    objsCC.Add(objcc);

                }

            }
            else
            {
                objsCC = new LojaFechamentoCC().ObterTodos(id,_paramBase);
                            
            }
            return Json(objsCC.Select(p => new {
                p.banco_id,
                dataPagamentoPrevisto = p.dataPagamentoPrevisto.ToString("o"),
                p.descricao,
                p.id,
                p.LojaFechamento_id,
                p.LojaTipoRecebimentoCaixa_id,
                p.prazoDias,
                p.taxa,
                p.tipoMovimento,
                p.tipoVenda,
                p.valorBruto,
                p.valorLiquido,
                p.valorTaxa}),
                JsonRequestBehavior.AllowGet);

            
        }


        public JsonResult ObterOperador(int id)
        {
            var objs = new LojaOperador().ObterTodosPorLoja(id, _paramBase).Where(p => p.ativo == true);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterCaixa(int id)
        {
            var objs = new LojaCaixa().ObterTodosPorLoja(id, _paramBase).Where(p => p.ativo == true);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
