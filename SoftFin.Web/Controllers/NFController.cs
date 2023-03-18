using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using System.Web.Helpers;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.Web.Mvc.Html;
using System.Globalization;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Reflection;
using System.IO;
using System.Text;

using SoftFin.Utils;
using SoftFin.NFSe.DTO;
using SoftFin.Web.Regras;
using System.Xml;
using System.Configuration;
using SoftFin.Migrate.NFSe.DTO;
using System.Xml.Serialization;

namespace SoftFin.Web.Controllers
{
    public class NFController : BaseController
    {
        #region Public
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult TotalizadorAutorizacao(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.usuarioAutorizador_id == null).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult AlterarVencimento(int id, int estab, DateTime vencimento)
        {

            try
            {
                if (estab != _estab)
                {


                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
                    });

                }


                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    var nf = new NotaFiscal().ObterPorId(id, db);
                    nf.dataVencimentoNfse = vencimento;

                    //Atualiza Banco Movimento
                    var banco = db.BancoMovimento.Where(x => x.notafiscal_id == id).FirstOrDefault();
                    if (banco != null)
                    {
                        banco.data = vencimento;
                    }

                    nf.usuarioalteracaoid = _usuarioobj.id;
                    nf.dataAlteracao = DateTime.Now;

                    if (nf.Alterar(_paramBase, db))
                    {
                        dbcxtransaction.Commit();
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Alterado com sucesso"

                        }); 


                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Impossivel alterar, registro excluído"

                        }); 
                    }

                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new
                {
                    CDStatus = "NOK",
                    DSMessage = ex.Message.ToString()
                }); 
            }
        }

        public JsonResult ListaPessoas()
        {
            return Json(new Pessoa().ObterTodosRacaoComCNPJ(_paramBase), JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new NotaFiscal().ObterEntreDataSomenteNFSe(DataInicial, DataFinal, _paramBase).Where(p => p.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA || p.situacaoPrefeitura_id == Models.NotaFiscal.NFGERADAENVIADA || p.situacaoPrefeitura_id == Models.NotaFiscal.NFBAIXA).Sum(x => x.valorNfse).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);
        }

        public class dtoFiltro
        {
            

            public DateTime? dataEmissaoRpsIni { get; set; }
            public DateTime? dataEmissaoRpsFim { get; set; }
            public DateTime? dataVencimentoNfseIni { get; set; }
            public DateTime? dataVencimentoNfseFim { get; set; }
            public decimal? valorBrutoIni { get; set; }
            public decimal? valorBrutoFim { get; set; }
            public int situacao { get; set; }
            public int Numero { get; set; }
        }


        [HttpPost]
        public JsonResult ObterNF(dtoFiltro data)
        {
            var Listas = PesquisaNFse();

            if (data.dataEmissaoRpsIni != null)
                Listas = Listas.Where(p => p.dataEmissaoRps >= data.dataEmissaoRpsIni).ToList();

            if (data.dataEmissaoRpsFim != null)
                Listas = Listas.Where(p => p.dataEmissaoRps <= data.dataEmissaoRpsFim.Value.AddDays(1)).ToList();


            if (data.dataVencimentoNfseIni != null)
                Listas = Listas.Where(p => p.dataVencimentoNfse >= data.dataVencimentoNfseIni).ToList();

            if (data.dataVencimentoNfseFim != null)
                Listas = Listas.Where(p => p.dataVencimentoNfse <= data.dataVencimentoNfseFim.Value.AddDays(1)).ToList();

            if (data.valorBrutoIni != null)
                Listas = Listas.Where(p => p.valorNfse >= data.valorBrutoIni).ToList();

            if (data.valorBrutoFim != null)
                Listas = Listas.Where(p => p.valorNfse <= data.valorBrutoFim).ToList();

            if (data.situacao != 0)
                Listas = Listas.Where(p => p.situacaoPrefeitura_id == data.situacao).ToList();

            if (data.Numero != 0)
                Listas = Listas.Where(p => p.OrdemVenda != null).Where(p => p.OrdemVenda.Numero == data.Numero).ToList();


            return Json(
                            Listas.Select(p => new
                                {
                                    p.tipoRps,
                                    p.serieRps,
                                    p.numeroRps,
                                    dataEmissaoRps = (p.dataEmissaoRps == null)? "": p.dataEmissaoRps.ToString("o"),
                                    p.numeroNfse,
                                    dataEmissaoNfse = (p.dataEmissaoNfse == null)? "": p.dataEmissaoNfse.ToString("o"),
                                    DataVencimentoOriginal = (p.DataVencimentoOriginal == null) ? "" : p.DataVencimentoOriginal.ToString("o"),
                                    dataVencimentoNfse = (p.dataVencimentoNfse == null) ? "" : p.dataVencimentoNfse.ToString("o"),
                                    razao = (p.NotaFiscalPessoaTomador == null) ? ((p.NotaFiscalPessoaPrestador== null) ? "" : p.NotaFiscalPessoaPrestador.razao) : p.NotaFiscalPessoaTomador.razao,
                                    p.valorNfse,
                                    p.codigoServico,
                                    p.codigoVerificacao,
                                    situacaoPrefeitura = ConsultaSituacao(p.situacaoPrefeitura_id),
                                    p.situacaoPrefeitura_id,
                                    p.id,
                                    linkpf = ConsultaLinkPF(p),
                                    p.estabelecimento_id,
                                    p.entradaSaida,
                                Numero = (p.OrdemVenda == null) ? 0 : p.OrdemVenda.Numero
                                }
                            )
                            , JsonRequestBehavior.AllowGet
                        )
                 ;

        }

        public JsonResult NFXMLConsulta(NotaFiscal obj)
        {
            try
            {

                if (obj.estabelecimento_id != _estab)
                {


                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = _mensagemTrocaAba,
                    });

                }
                if (_estabobj.senhaCertificado == null)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Certificado não configurado para esta estabelecimento."
                    }, JsonRequestBehavior.AllowGet);
                }


                DbControle db = new DbControle();
                NotaFiscal objNF;
                DTORetornoNFEs resultado;
                //Server.MapPath("~/CertTMP/");
                new NotaFiscalCalculos().AtualizaNFBussines(obj.id, _paramBase, null, db, out objNF, out resultado);

                if (resultado.Cabecalho.Sucesso.ToLower() == "false")
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Comando não aceito pela prefeitura, a seguir os erros:", 
                        Erros = resultado.Erro,
                        Alertas = resultado.Alerta
                    }); 
                }
                else
                {
                    new NotaFiscalCalculos().BaixaNF(objNF,
                        objNF.dataEmissaoRps,
                        resultado.NFe.First().ChaveNFe.CodigoVerificacao,
                        int.Parse(resultado.NFe.First().ChaveNFe.NumeroNFe),
                        resultado.NFe.First().StatusNFe,
                        db, _paramBase);
                    
                    if (resultado.Alerta.Count() == 0)
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Comando aceito pela prefeitura", 
                            Erros = resultado.Erro,
                            Alertas = resultado.Alerta
                        }); 
                    else
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Comando aceito pela prefeitura, a seguir os alertas:", 
                            Erros = resultado.Erro,
                            Alertas = resultado.Alerta
                        }); 

                }


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    CDStatus = "NOK",
                    DSMessage = ex.Message.ToString()
                }); 
            }
        }
        public JsonResult NFXMLEnvio(NotaFiscal obj)
        {


            try
            {
                if (obj.estabelecimento_id != _estab)
                {


                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
                    });

                }
                if (_estabobj.senhaCertificado == null)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Certificado não configurado para esta estabelecimento."
                    }, JsonRequestBehavior.AllowGet);
                }

                var resultado = EnviaNFBussiness(obj.id);

                if (resultado.Cabecalho.Sucesso.ToLower() == "false")
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Comando não aceito pela prefeitura, a seguir os erros:", 
                        Erros = resultado.Erro,
                        Alertas = resultado.Alerta
                    }); 
                }
                else
                {
                    if (resultado.Alerta.Count() == 0)
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Comando aceito pela prefeitura", 
                            Erros = resultado.Erro,
                            Alertas = resultado.Alerta
                        }); 
                    else
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Comando aceito pela prefeitura, a seguir os alertas:", 
                            Erros = resultado.Erro,
                            Alertas = resultado.Alerta
                        }); 
                }

            }
            catch (Exception ex)
            {
                
                _eventos.Error(ex);
                return Json(new
                {
                    CDStatus = "NOK",
                    DSMessage = ex.Message.ToString()
                }); 
            }
        }
        [HttpPost]
        public JsonResult Cancelamento(NotaFiscal obj)
        {
            try
            {
                if (obj.estabelecimento_id != _estab)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = _mensagemTrocaAba,
                    });

                }
                


                int? notaPrest = null;
                int? notaTomador = null;

                var db = new DbControle();
                var rc = new Recebimento().ObterTodosPorNFSeId(obj.id, _paramBase, db);
                if (rc.Count() != 0)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Existem pelo menos 1 recebimento relacionado a esta nota, não é possivel cancelar(Para cancelar exclua os recebimentos)",
                    });

                }

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var nf = new NotaFiscal().ObterPorId(obj.id, db);
                    if ((nf.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA) || (nf.situacaoPrefeitura_id == Models.NotaFiscal.NFAVULSA))
                    {

                        if (nf.ordemVenda_id != null)
                        {
                            string ovErro = "";
                            var ov = new OrdemVenda().ObterPorId(nf.ordemVenda_id.Value, db);

                            if (nf.situacaoPrefeitura_id == Models.NotaFiscal.NFAVULSA)
                            {
                                ov.Excluir(nf.ordemVenda_id.Value, ref ovErro, _paramBase,db);
                            }
                            else
                            {
                                ov.statusParcela_ID = Models.StatusParcela.SituacaoLiberada();

                                if (ov.ParcelaContrato != null)
                                {
                                    if (ov.parcelaContrato_ID != null)
                                    {
                                        var contratoParcela = new ParcelaContrato().ObterPorId(ov.parcelaContrato_ID.Value, db, _paramBase);
                                        contratoParcela.statusParcela_ID = Models.StatusParcela.SituacaoLiberada();
                                    }
                                }
                            }
                        }

                        var logsMestres = new LogNFXMLPrincipal().ObterTodosPorNota(nf.id, db);

                        foreach (var item in logsMestres)
                        {
                            var logsAlertas = new LogNFXMLAlerta().ObterTodosPorCapa(item.id, db);
                            foreach (var item2 in logsAlertas)
                            {
                                item2.Excluir(_paramBase, db);
                            }
                            var logsErros = new LogNFXMLErro().ObterTodosPorCapa(item.id, db);
                            ;
                            foreach (var item2 in logsErros)
                            {
                                item2.Excluir(_paramBase, db);
                            }
                            item.Excluir(db,_paramBase);
                        }

                        var bcoMovs = new BancoMovimento().ObterPorNFES(obj.id, db, _paramBase);
                        foreach (var item in bcoMovs)
                        {
                            string erro = "";
                            item.Excluir(item.id, ref erro,_paramBase, db);
                            if (erro != "")
                            {
                                dbcxtransaction.Rollback();
                                string strerro = "Erro ao excluir o banco movimento id (" + nf.id.ToString() + ") : " + erro;
                                throw new Exception(strerro);
                            }
                        }

                        //  Exclusão LancamentoContabil Inicio
                        

                        var objLcs = new LancamentoContabil().ObterPorNotaFiscal(obj.id, _paramBase, db);
                        foreach (var itemLC in objLcs)
                        {
                            var objDets = new LancamentoContabilDetalhe().ObterPorCapa(itemLC.id, _paramBase, db);
                            foreach (var itemDT in objDets)
                            {
                                var erroDet = "";
                                itemDT.Excluir(itemDT.id, ref erroDet, _paramBase, db);
                                if (erroDet != "")
                                    throw new Exception(erroDet);
                            }

                            var errolc = "";
                            itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                            if (errolc != "")
                                throw new Exception(errolc);
                        }
                        //  Exclusão LancamentoContabil Fim




                        string erro2 = "";
                        notaPrest = nf.notaFiscalPrestador_id ;
                        notaTomador = nf.notaFiscalTomador_id ;

                        nf.notaFiscalPrestador_id = null;
                        nf.notaFiscalTomador_id = null;
                        nf.Alterar(_paramBase, db);
                        nf.Excluir(ref erro2, _paramBase, db);

                        if (erro2 != "")
                        {
                            dbcxtransaction.Rollback();
                            string strerro = "Erro ao excluir a nota id (" + nf.id.ToString() + ") tabela NotaFiscal : " + erro2;
                            throw new Exception(strerro);
                        }
                        db.SaveChanges();
                        /*
                        if (notaTomador != null)
                        {
                            var Tomador = new NotaFiscalPessoa().ObterPorId(notaTomador.Value, db);
                            Tomador.ExcluirNS(db);
                        }

                        if (notaPrest != null)
                        {
                            var Prestador = new NotaFiscalPessoa().ObterPorId(notaPrest.Value, db);
                            Prestador.Excluir(db);
                        }


                        db.SaveChanges();
                        */
                    }
                    else if (nf.situacaoPrefeitura_id == Models.NotaFiscal.NFGERADAENVIADA)
                    {
                        

                        var bcoMovs = new BancoMovimento().ObterPorNFES(obj.id, db,_paramBase);
                        foreach (var item in bcoMovs)
                        {
                            string erro = "";
                            item.Excluir(item.id, ref erro, _paramBase, db);
                            if (erro != "")
                            {
                                dbcxtransaction.Rollback();
                                string strerro = "Erro ao excluir o banco movimento id (" + nf.id.ToString() + ") : " + erro;
                                throw new Exception(strerro);
                            }
                        }

                        //  Exclusão LancamentoContabil Inicio
                        var objLcs = new LancamentoContabil().ObterPorNotaFiscal(obj.id, _paramBase, db);
                        foreach (var itemLC in objLcs)
                        {
                            var errolc = "";
                            itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                            if (errolc != "")
                                throw new Exception(errolc);
                        }
                        //  Exclusão LancamentoContabil Fim

                        nf.situacaoPrefeitura_id = Models.NotaFiscal.NFCANCELADAEMCONF;
                        db.SaveChanges();
                    }


                    dbcxtransaction.Commit();
                    
                }

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    if (notaTomador != null)
                    {
                        var Tomador = new NotaFiscalPessoa().ObterPorId(notaTomador.Value, db);
                        Tomador.ExcluirNS(_paramBase, db);
                    }

                    if (notaPrest != null)
                    {
                        var Prestador = new NotaFiscalPessoa().ObterPorId(notaPrest.Value, db);
                        Prestador.ExcluirNS(_paramBase, db);
                    }


                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }


                return Json(new
                {
                    CDStatus = "OK",
                    DSMessage = "Cancelado com sucesso"

                });

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new
                {
                    CDStatus = "NOK",
                    DSMessage = ex.Message.ToString()
                });
            }
        }
        [HttpPost]
        public JsonResult BaixaPerda(NotaFiscal obj)
        {
            try
            {
                if (obj.estabelecimento_id != _estab)
                {


                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
                    });

                }

                var db = new DbControle();


                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var nfe = db.NotaFiscal.Where(nf => nf.id == obj.id && nf.estabelecimento_id == _estab).FirstOrDefault();

                    nfe.situacaoPrefeitura_id = Models.NotaFiscal.NFBAIXA;
                    nfe.SituacaoRecebimento = 4;

                    //Exclui Banco Movimento quando baixada a nota fiscal
                    var bcoMov = db.BancoMovimento.Where(nf => nf.notafiscal_id == nfe.id && nf.Banco.estabelecimento_id == _estab).FirstOrDefault();
                    if (bcoMov != null)
                    {
                        db.BancoMovimento.Remove(bcoMov);
                    }

                    //  Exclusão LancamentoContabil Inicio
                    var objLcs = new LancamentoContabil().ObterPorNotaFiscal(obj.id, _paramBase, db);

                    foreach (var itemLC in objLcs)
                    {
                        var errolc = "";
                        itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                        if (errolc != "")
                            throw new Exception(errolc);
                    }
                    //  Exclusão LancamentoContabil FIM


                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }

                return Json(new
                    {
                        CDStatus = "OK",
                        DSMessage = "Sucesso na Baixa"

                    }); 
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    CDStatus = "NOK",
                    DSMessage = ex.Message.ToString()
                }); 
            }
        }


        public JsonResult ObterUnidadeNegocios()
        {
            var objs = new UnidadeNegocio().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.unidade
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterItemProdutoServicos()
        {
            var objs = new ItemProdutoServico().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTabelaPrecoItemProdutoServicos()
        {
            var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObterOperacoes()
        {
            var objs = new Operacao().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id.ToString(),
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObterCodigoServicos()
        {
            var objs = new CodigoServicoEstabelecimento().ObterTodos(_paramBase).Where(p => p.CodigoServicoMunicipio.municipio_id == _estabobj.Municipio_id);


            return Json(objs.Select(p => new
            {
                Value = p.CodigoServicoMunicipio.codigo,
                Text = p.CodigoServicoMunicipio.codigo + " - " + p.CodigoServicoMunicipio.descricao
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObterNFPorId(int id)
        {
            var nf = new NotaFiscal().ObterPorId(id);
            
            nf.tipoRps = 1;
            nf.estabelecimento_id = _estab;

            
            var ov = new OrdemVenda();
            
            if (nf.ordemVenda_id!=null)
                ov = ov.ObterPorId(nf.ordemVenda_id.Value);
            
            return Json(
                new
                {
                    ov = new {
                        data = ov.data.ToString("o"),
                        dataAutorizacao = (ov.dataAutorizacao == null) ? "": ov.dataAutorizacao.Value.ToString("o"),
                        ov.estabelecimento_id,
                        ov.id,
                        ov.itemProdutoServico_ID,
                        ov.Numero,
                        ov.parcelaContrato_ID,
                        ov.pessoas_ID,
                        ov.statusParcela_ID,
                        ov.tabelaPreco_ID,
                        ov.unidadeNegocio_ID,
                        ov.usuarioAutorizador_id,
                        ov.valor
                    },
                    filtro = new
                    {
                        descricao = nf.OrdemVenda == null ? "" : nf.OrdemVenda.descricao, 
                        codigoServico = nf.codigoServico,
                        data = nf.dataVencimentoNfse.ToString("o"),
                        banco_id = nf.banco_id.ToString(),
                        operacao_id = nf.operacao_id.ToString(),
                        valor = nf.valorNfse,
                        numeroNfse = nf.numeroNfse,
                        codigoVerificacao = nf.codigoVerificacao,
                        itemProdutoServico_ID = nf.OrdemVenda == null ? 0 : nf.OrdemVenda.itemProdutoServico_ID,
                        tabelaPreco_ID = nf.OrdemVenda == null ? 0 : nf.OrdemVenda.tabelaPreco_ID,
                        unidadeNegocio_ID = nf.OrdemVenda == null ? 0 : nf.OrdemVenda.unidadeNegocio_ID,
                        ovid = nf.ordemVenda_id,
                        pessoastr = nf.OrdemVenda == null ? "" : nf.OrdemVenda.Pessoa.nome + ", " + nf.OrdemVenda.Pessoa.cnpj
                    },

                    nf = new
                    {
                        nf.aliquotaINSS,
                        nf.ordemVenda_id,
                        nf.aliquotaIrrf,
                        nf.aliquotaISS,
                        nf.banco_id,
                        nf.basedeCalculo,
                        nf.codigoServico,
                        nf.codigoVerificacao,
                        nf.cofinsRetida,
                        nf.creditoImposto,
                        nf.csllRetida,
                        dataEmissaoNfse = (nf.dataEmissaoNfse == null) ? "" : nf.dataEmissaoNfse.ToString("o"),
                        dataEmissaoRps = (nf.dataEmissaoRps == null) ? "" : nf.dataEmissaoRps.ToString("o"),
                        dataVencimentoNfse = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
                        DataVencimentoOriginal = (nf.DataVencimentoOriginal == null) ? "" : nf.DataVencimentoOriginal.ToString("o"),
                        nf.discriminacaoServico,
                        nf.entradaSaida,
                        nf.estabelecimento_id,
                        nf.id,
                        nf.irrf,
                        nf.municipio_id,
                        nf.numeroNfse,
                        nf.numeroRps,
                        nf.operacao_id,
                        nf.pisRetido,
                        nf.serieRps,
                        nf.situacaoPrefeitura_id,
                        nf.SituacaoRecebimento,
                        nf.situacaoRps,
                        nf.tipoRps,
                        nf.valorDeducoes,
                        nf.valorINSS,
                        nf.valorISS,
                        nf.valorLiquido,
                        nf.valorNfse,
                        nf.ValAliqPIS,
                        nf.ValAliqISSRetido,
                        nf.ValAliqCOFINS,
                        nf.ValAliqCSLL,
                        nf.ValISSRetido,
                        nf.TipoFaturamento,
                        nf.notaFiscalIntermediario_id,
                        nf.notaFiscalPrestador_id,
                        nf.notaFiscalTomador_id,
                        nf.LocalPrestServ,
                        NotaFiscalPessoaTomador = (nf.NotaFiscalPessoaTomador == null) ? null :new
                        {
                            nf.NotaFiscalPessoaTomador.razao,
                            nf.NotaFiscalPessoaTomador.numero,
                            nf.NotaFiscalPessoaTomador.bairro,
                            nf.NotaFiscalPessoaTomador.cep,
                            nf.NotaFiscalPessoaTomador.cidade,
                            cnpjCpf = nf.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
                            nf.NotaFiscalPessoaTomador.complemento,
                            nf.NotaFiscalPessoaTomador.email,
                            nf.NotaFiscalPessoaTomador.endereco,
                            nf.NotaFiscalPessoaTomador.indicadorCnpjCpf,
                            nf.NotaFiscalPessoaTomador.inscricaoEstadual,
                            nf.NotaFiscalPessoaTomador.inscricaoMunicipal,
                            nf.NotaFiscalPessoaTomador.tipoEndereco,
                            nf.NotaFiscalPessoaTomador.uf,
                            nf.NotaFiscalPessoaTomador.id
                        },

                        NotaFiscalPessoaPrestador = (nf.NotaFiscalPessoaPrestador == null) ? null : new
                        {
                            nf.NotaFiscalPessoaPrestador.razao,
                            nf.NotaFiscalPessoaPrestador.numero,
                            nf.NotaFiscalPessoaPrestador.bairro,
                            nf.NotaFiscalPessoaPrestador.cep,
                            nf.NotaFiscalPessoaPrestador.cidade,
                            cnpjCpf = nf.NotaFiscalPessoaPrestador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
                            nf.NotaFiscalPessoaPrestador.complemento,
                            nf.NotaFiscalPessoaPrestador.email,
                            nf.NotaFiscalPessoaPrestador.endereco,
                            nf.NotaFiscalPessoaPrestador.indicadorCnpjCpf,
                            nf.NotaFiscalPessoaPrestador.inscricaoEstadual,
                            nf.NotaFiscalPessoaPrestador.inscricaoMunicipal,
                            nf.NotaFiscalPessoaPrestador.tipoEndereco,
                            nf.NotaFiscalPessoaPrestador.uf,
                            nf.NotaFiscalPessoaPrestador.id
                        }


                    }
                }
                            , JsonRequestBehavior.AllowGet
                        )
                 ;

        }

        [HttpPost]
        public JsonResult ObterNFPorXML(HttpPostedFileBase arquivo)
        {
            try
            {
                if (!arquivo.FileName.ToLower().Contains(".xml"))
                {
                    throw new Exception("Extensão inválido");
                }

                string nomearquivo = arquivo.FileName;
                var uploadPath = Server.MapPath("~/TXTTemp/");
                Directory.CreateDirectory(uploadPath);
                nomearquivo = nomearquivo.ToLower();
                string caminhoArquivo = Path.Combine(@uploadPath, nomearquivo);
                arquivo.SaveAs(caminhoArquivo);

                var nf = new NotaFiscal();

                nf.tipoRps = 1;
                nf.estabelecimento_id = _estab;

                var ov = new OrdemVenda();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(caminhoArquivo);


                var numCodigoVerificacao = XmlToStringJson(xmlDocument, "ns3:CodigoVerificacao");
                var munic = new Municipio().ObterPorCodigoIBGE(
                                            XmlToStringJson(xmlDocument, "ns3:Cidade", 1)
                                            );
                var existeNota = new NotaFiscal().ObterPorCodigoVerificacao(munic.ID_MUNICIPIO,numCodigoVerificacao, _paramBase);

                if (existeNota != null)
                    throw new Exception("Nota já incluida");

                ov.estabelecimento_id = _estab;

                ov.Numero = 0;//Soma ao Salvar
                ov.pessoas_ID = 0;
                ov.statusParcela_ID = new StatusParcela().ObterStatusManual();


                var cnpj = XmlToStringJson(xmlDocument, "ns3:Cnpj", 1);
                var existePessoa = new Pessoa().ObterPorCNPJ(cnpj, _paramBase);

                if (existePessoa == null)
                {
                    existePessoa = new Pessoa();
                    existePessoa.empresa_id = _empresa;
                    existePessoa.razao = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:RazaoSocial", 0);
                    existePessoa.nome = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:RazaoSocial", 0);
                    existePessoa.numero = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Numero", 0);
                    existePessoa.bairro = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Bairro", 0);
                    existePessoa.cep = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Cep", 0);
                    existePessoa.cidade = munic.DESC_MUNICIPIO;

                    existePessoa.cnpj = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Cnpj", 0);
                    existePessoa.complemento = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Complemento", 0);
                    existePessoa.eMail = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Email", 0);
                    if (existePessoa.eMail == "")
                        existePessoa.eMail = null;
                    existePessoa.endereco = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Endereco", 1);

                    existePessoa.inscricao = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:inscricaoEstadual", 0);
                    existePessoa.ccm = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:InscricaoMunicipal", 0);
                    existePessoa.uf = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Estado", 0);
                    existePessoa.codigo = cnpj;
                    existePessoa.UnidadeNegocio_ID = new UnidadeNegocio().ObterTodos(_paramBase).First().id;
                    existePessoa.Incluir(_paramBase);
                }

                ov.pessoas_ID = existePessoa.id;



                var ovAux = new
                        {
                            data = XmlToDateJson(xmlDocument, "ns3:DataEmissao"),
                            dataAutorizacao = DateTime.Now.ToString("o"),
                            ov.estabelecimento_id,
                            ov.id,
                            ov.itemProdutoServico_ID,
                            ov.Numero,
                            ov.parcelaContrato_ID,
                            ov.pessoas_ID,
                            ov.statusParcela_ID,
                            ov.tabelaPreco_ID,
                            ov.unidadeNegocio_ID,
                            usuarioAutorizador_id = _usuarioobj.id,
                            valor = XmlToDecimalJson(xmlDocument, "ns3:ValorServicos")
                        };

                var filtroAux = new
                        {
                            descricao = "Importação de NF Emitida ",
                            codigoServico = "",
                            data = XmlToDateJson(xmlDocument, "ns3:DataEmissao"),
                            banco_id = new Banco().ObterPrincipal(_paramBase).id,
                            operacao_id = "",
                            valor = XmlToDecimalJson(xmlDocument, "ns3:ValorServicos"),
                            numeroNfse = XmlToStringJson(xmlDocument, "ns3:Numero"),
                            codigoVerificacao = XmlToStringJson(xmlDocument, "ns3:CodigoVerificacao"),
                            itemProdutoServico_ID = "",
                            tabelaPreco_ID = "",
                            unidadeNegocio_ID = "",
                            ovid = 0,
                            pessoastr = XmlToStringJson(xmlDocument, "ns3:RazaoSocial", 1) + ", " + XmlToStringJson(xmlDocument, "ns3:Cnpj", 1)
                        };

                var NotaFiscalPessoaTomadorAux = new
                            {
                                razao = XmlToStringJson(xmlDocument, "ns3:TomadorServico","ns3:RazaoSocial", 0),
                                numero = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Numero", 0),
                                bairro = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Bairro", 0),
                                cep = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Cep", 0),
                                cidade = new Municipio().ObterPorCodigoIBGE(
                                                    XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Cidade", 0)
                                                    ).DESC_MUNICIPIO,
                                cnpjCpf = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Cnpj", 0),
                                complemento = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Complemento", 0),
                                email = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Email", 0),
                                endereco = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Endereco", 1),
                                indicadorCnpjCpf = 2,
                                inscricaoEstadual = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:inscricaoEstadual", 0),
                                inscricaoMunicipal = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:InscricaoMunicipal", 0),
                                tipoEndereco = "",
                                uf = XmlToStringJson(xmlDocument, "ns3:TomadorServico", "ns3:Estado", 0),

                                id = 0
                            };

                var NotaFiscalPessoaPrestadorAux = new
                {
                    razao = XmlToStringJson(xmlDocument, "ns3:PrestadorServico","ns3:RazaoSocial", 0),
                    numero = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Numero", 0),
                    bairro = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Bairro", 0),
                    cep = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Cep", 0),
                    cidade = new Municipio().ObterPorCodigoIBGE(
                                        XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Cidade", 0)
                                        ).DESC_MUNICIPIO,
                    cnpjCpf = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Cnpj", 0),
                    complemento = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Complemento", 0),
                    email = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Email", 0),
                    endereco = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Endereco", 1),
                    indicadorCnpjCpf = 2,
                    inscricaoEstadual = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:inscricaoEstadual", 0),
                    inscricaoMunicipal = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:InscricaoMunicipal", 0),
                    tipoEndereco = "",
                    uf = XmlToStringJson(xmlDocument, "ns3:PrestadorServico", "ns3:Estado", 0),

                    id = 0
                };

                return Json(
                    new
                    {
                        CDStatus = "OK",
                        ov = ovAux,
                        filtro = filtroAux,

                        nf = new
                        {
                            aliquotaINSS = 0,
                            ordemVenda_id = 0,
                            aliquotaIrrf = 0,
                            aliquotaISS = 0,
                            banco_id = new Banco().ObterPrincipal(_paramBase).id,
                            basedeCalculo = XmlToDecimalJson(xmlDocument, "ns3:BaseCalculo"),
                            codigoServico = "",
                            codigoVerificacao = XmlToStringJson(xmlDocument, "ns3:CodigoVerificacao"),
                            cofinsRetida = XmlToDecimalJson(xmlDocument, "ns3:ValorCofins"),
                            creditoImposto = 0,
                            csllRetida = XmlToDecimalJson(xmlDocument, "ns3:ValorCsll"),
                            dataEmissaoNfse = XmlToDateJson(xmlDocument, "ns3:DataEmissao"),
                            dataEmissaoRps = XmlToDateJson(xmlDocument, "ns3:DataEmissao"),
                            dataVencimentoNfse = XmlToDateJson(xmlDocument, "ns3:DataEmissao"),
                            discriminacaoServico = XmlToStringJson(xmlDocument, "ns3:Discriminacao"),
                            entradaSaida = "S",
                            estabelecimento_id = _estab,
                            id = 0,
                            irrf = XmlToDecimalJson(xmlDocument, "ns3:ValorIr"),
                            municipio_id = new Municipio().ObterPorCodigoIBGE(
                                                    XmlToStringJson(xmlDocument, "ns3:Cidade", 1)
                                                    ).ID_MUNICIPIO,
                            numeroNfse = XmlToStringJson(xmlDocument, "ns3:Numero"),
                            numeroRps = 0,
                            operacao_id = 0,
                            pisRetido = XmlToDecimalJson(xmlDocument, "ns3:ValorPis"),
                            serieRps = "0",
                            situacaoPrefeitura_id = Models.NotaFiscal.NFGERADAENVIADA,
                            SituacaoRecebimento = Models.NotaFiscal.NFEMABERTO,
                            situacaoRps = 1,
                            tipoRps = 1,
                            valorDeducoes = 0,
                            valorINSS = 0,
                            valorISS = XmlToDecimalJson(xmlDocument, "ns3:ValorIss"),
                            valorLiquido = XmlToDecimalJson(xmlDocument, "ns3:ValorLiquidoNfse"),
                            valorNfse = XmlToDecimalJson(xmlDocument, "ns3:ValorServicos"),


                            NotaFiscalPessoaTomador = NotaFiscalPessoaTomadorAux,

                            NotaFiscalPessoaPrestador = NotaFiscalPessoaPrestadorAux


                        }
                    }
                                , JsonRequestBehavior.AllowGet
                            )
                     ;
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", CDMessage = ex.Message },JsonRequestBehavior.AllowGet);
            }

        }

        private static string XmlToDateJson(XmlDocument xmlDocument, string key)
        {
            try
            {
                var tagValor = xmlDocument.GetElementsByTagName(key)[0].InnerText;
                return DateTime.Parse(tagValor).ToString("o");
            }
            catch
            {
                return "";
            }
        }
        private static decimal XmlToDecimalJson(XmlDocument xmlDocument, string key)
        {
            try
            {
                var tagValor = xmlDocument.GetElementsByTagName(key)[0].InnerText.Replace(".",",");
                return decimal.Parse(tagValor);
            }
            catch
            {
                return 0;
            }
        }
        private static string XmlToStringJson(XmlDocument xmlDocument, string key, int pos=0)
        {
            try
            {
                if (xmlDocument.GetElementsByTagName(key).Count >= pos)
                {
                    var tagValor = xmlDocument.GetElementsByTagName(key)[pos].InnerText;
                    return tagValor;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        private static string XmlToStringJson(XmlDocument xmlDocument,string nodeselect, string key, int pos = 0)
        {
            try
            {
                XmlNodeList nodesx = xmlDocument.GetElementsByTagName(nodeselect);
                
                var tagValor = ((XmlElement)nodesx[0]).GetElementsByTagName(key)[pos].InnerText;
                return tagValor;
            }
            catch
            {
                return "";
            }
        }

        [HttpPost]
        //Calcula Nota
        public JsonResult CalculaNotaTela(
            string codigoServico,
            DateTime data,
            int banco_id,
            int operacao_id,
            decimal valor,
            int unidadeNegocio_ID,
            int ovid,
            string pessoastr,
            int nota_id,
            decimal valorDeducoes)
        {

            try
            {
                var nf = new NotaFiscal();
                nf.valorDeducoes = valorDeducoes;
                var nomePessoa = pessoastr.Split(',')[0];
                var cnpjPessoa = pessoastr.Split(',')[1].Replace(".", "").Replace("/", "").Replace("-", "");
                var pessoa = new Pessoa().ObterPorRazaoCNPJ(nomePessoa, "", _paramBase);

                if (pessoa == null)
                {
                    pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, "", _paramBase);
                    
                    if (pessoa == null)
                        throw new Exception("Pessoa não encontrada ou não informada.");
                }

                new NotaFiscalCalculos().Calcula(codigoServico,
                                                data,
                                                banco_id,
                                                operacao_id,
                                                valor,
                                                unidadeNegocio_ID,
                                                pessoa.id,
                                                ovid,
                                                _estabobj,
                                                nf, 
                                                _paramBase);

                var ov = new OrdemVenda();

                ov.data = data;
                ov.dataAutorizacao = null;
                ov.estabelecimento_id = _estab;
                ov.itemProdutoServico_ID = 0;
                ov.Numero = 0;
                ov.parcelaContrato_ID = null;
                ov.pessoas_ID = pessoa.id;
                ov.statusParcela_ID = 0;
                ov.tabelaPreco_ID = 0;
                ov.unidadeNegocio_ID = unidadeNegocio_ID;
                ov.usuarioAutorizador_id = null;
                ov.valor = valor;
                ov.id = ovid;
               
                nf.id = nota_id;



                return Json(
                    new
                    {
                        CDStatus = "OK",
                        ov = new
                        {
                            data = ov.data.ToString("o"),
                            dataAutorizacao = (ov.dataAutorizacao == null) ? "" : ov.dataAutorizacao.Value.ToString("o"),
                            ov.estabelecimento_id,
                            ov.id,
                            ov.itemProdutoServico_ID,
                            ov.Numero,
                            ov.parcelaContrato_ID,
                            ov.pessoas_ID,
                            ov.statusParcela_ID,
                            ov.tabelaPreco_ID,
                            ov.unidadeNegocio_ID,
                            ov.usuarioAutorizador_id,
                            ov.valor
                        },
                        obj = new
                        {
                            nf.aliquotaINSS,
                            nf.ordemVenda_id,
                            nf.aliquotaIrrf,
                            nf.aliquotaISS,
                            nf.banco_id,
                            nf.basedeCalculo,
                            nf.codigoServico,
                            nf.codigoVerificacao,
                            nf.cofinsRetida,
                            nf.creditoImposto,
                            nf.csllRetida,
                            dataEmissaoNfse = (nf.dataEmissaoNfse == null) ? "" : nf.dataEmissaoNfse.ToString("o"),
                            dataEmissaoRps = (nf.dataEmissaoRps == null) ? "" : nf.dataEmissaoRps.ToString("o"),
                            dataVencimentoNfse = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
                            nf.discriminacaoServico,
                            nf.entradaSaida,
                            nf.estabelecimento_id,
                            nf.id,
                            nf.irrf,
                            nf.municipio_id,
                            nf.numeroNfse,
                            nf.numeroRps,
                            nf.operacao_id,
                            nf.pisRetido,
                            nf.serieRps,
                            nf.situacaoPrefeitura_id,
                            nf.SituacaoRecebimento,
                            nf.situacaoRps,
                            nf.tipoRps,
                            nf.valorDeducoes,
                            nf.valorINSS,
                            nf.valorISS,
                            nf.valorLiquido,
                            nf.valorNfse,
                            nf.ValAliqPIS,
                            nf.ValAliqISSRetido,
                            nf.ValAliqCOFINS,
                            nf.ValAliqCSLL,
                            nf.ValISSRetido,
                            nf.TipoFaturamento,
                            nf.LocalPrestServ,
                        NotaFiscalPessoaTomador = (nf.NotaFiscalPessoaTomador == null) ? null :new
                        {
                            nf.NotaFiscalPessoaTomador.razao,
                            nf.NotaFiscalPessoaTomador.numero,
                            nf.NotaFiscalPessoaTomador.bairro,
                            nf.NotaFiscalPessoaTomador.cep,
                            nf.NotaFiscalPessoaTomador.cidade,
                            cnpjCpf = (nf.NotaFiscalPessoaTomador.cnpjCpf == null) ? "" : nf.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
                            nf.NotaFiscalPessoaTomador.complemento,
                            nf.NotaFiscalPessoaTomador.email,
                            nf.NotaFiscalPessoaTomador.endereco,
                            nf.NotaFiscalPessoaTomador.indicadorCnpjCpf,
                            nf.NotaFiscalPessoaTomador.inscricaoEstadual,
                            nf.NotaFiscalPessoaTomador.inscricaoMunicipal,
                            nf.NotaFiscalPessoaTomador.tipoEndereco,
                            nf.NotaFiscalPessoaTomador.uf,
                            nf.NotaFiscalPessoaTomador.id
                        },

                        NotaFiscalPessoaPrestador = (nf.NotaFiscalPessoaPrestador == null) ? null : new
                        {
                            nf.NotaFiscalPessoaPrestador.razao,
                            nf.NotaFiscalPessoaPrestador.numero,
                            nf.NotaFiscalPessoaPrestador.bairro,
                            nf.NotaFiscalPessoaPrestador.cep,
                            nf.NotaFiscalPessoaPrestador.cidade,
                            cnpjCpf = (nf.NotaFiscalPessoaPrestador.cnpjCpf == null) ? "" : nf.NotaFiscalPessoaPrestador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
                            nf.NotaFiscalPessoaPrestador.complemento,
                            nf.NotaFiscalPessoaPrestador.email,
                            nf.NotaFiscalPessoaPrestador.endereco,
                            nf.NotaFiscalPessoaPrestador.indicadorCnpjCpf,
                            nf.NotaFiscalPessoaPrestador.inscricaoEstadual,
                            nf.NotaFiscalPessoaPrestador.inscricaoMunicipal,
                            nf.NotaFiscalPessoaPrestador.tipoEndereco,
                            nf.NotaFiscalPessoaPrestador.uf,
                            nf.NotaFiscalPessoaPrestador.id
                        }
                        }

                    }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObterPorId(int id)
        {
            var nf = new NotaFiscal().ObterPorId(id);

            DbControle db = new DbControle();

            return Json(new
            {
                NF = new
                {
                    nf.aliquotaINSS,
                    nf.ordemVenda_id,
                    nf.aliquotaIrrf,
                    nf.aliquotaISS,
                    banco_id = nf.banco_id.ToString(),
                    nf.basedeCalculo,
                    nf.codigoServico,
                    nf.codigoVerificacao,
                    nf.cofinsRetida,
                    nf.creditoImposto,
                    nf.csllRetida,
                    dataEmissaoNfse = (nf.dataEmissaoNfse == null) ? "" : nf.dataEmissaoNfse.ToString("o"),
                    dataEmissaoRps = (nf.dataEmissaoRps == null) ? "" : nf.dataEmissaoRps.ToString("o"),
                    dataVencimentoNfse = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
                    DataVencimentoOriginal = (nf.DataVencimentoOriginal == null) ? "" : nf.DataVencimentoOriginal.ToString("o"),
                    nf.discriminacaoServico,
                    nf.entradaSaida,
                    nf.estabelecimento_id,
                    nf.id,
                    nf.irrf,
                    nf.municipio_id,
                    nf.numeroNfse,
                    nf.numeroRps,
                    nf.operacao_id,
                    nf.pisRetido,
                    nf.serieRps,
                    nf.situacaoPrefeitura_id,
                    nf.SituacaoRecebimento,
                    nf.situacaoRps,
                    nf.tipoRps,
                    nf.valorDeducoes,
                    nf.valorINSS,
                    nf.valorISS,
                    nf.valorLiquido,
                    nf.valorNfse,
                    nf.codigoCEI,
                    nf.percentualCargaTributaria,
                    nf.fonteCargaTributaria,
                    nf.matriculaObra,
                    nf.ValAliqPIS,
                    nf.ValAliqISSRetido,
                    nf.ValAliqCOFINS,
                    nf.ValAliqCSLL,
                    nf.ValISSRetido,
                    nf.TipoFaturamento,
                    nf.notaFiscalIntermediario_id,
                    nf.notaFiscalPrestador_id,
                    nf.notaFiscalTomador_id,
                    nf.RespRetencao,
                    nf.LocalPrestServ,
                    nf.ObraNumEncapsulamento

        },

                OV = new
                {
                    nf.OrdemVenda.id,
                    nf.OrdemVenda.Numero,
                    nf.OrdemVenda.unidadeNegocio_ID,
                    nf.OrdemVenda.descricao,
                    nf.OrdemVenda.valor,
                    nf.OrdemVenda.itemProdutoServico_ID,
                    nf.OrdemVenda.tabelaPreco_ID,
                    operacao_id = nf.operacao_id.ToString(),
                    banco_id = nf.banco_id.ToString(),
                    dataVencimentoNfse = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
                    DataVencimentoOriginal = (nf.DataVencimentoOriginal == null) ? "" : nf.DataVencimentoOriginal.ToString("o"),
                    data = nf.OrdemVenda.data.ToString("o"),
                    nf.codigoServico,
                    pessoanome = nf.NotaFiscalPessoaTomador.razao + ", " + nf.NotaFiscalPessoaTomador.cnpjCpf,
                    numeroNfse = nf.numeroNfe,
                    codigoVerificacao = nf.codigoVerificacao,
                    nf.situacaoPrefeitura_id,
                    nf.valorDeducoes,
                },

                NotaFiscalPessoaTomador = (nf.NotaFiscalPessoaTomador == null) ? null : new
                {
                    nf.NotaFiscalPessoaTomador.razao,
                    nf.NotaFiscalPessoaTomador.numero,
                    nf.NotaFiscalPessoaTomador.bairro,
                    nf.NotaFiscalPessoaTomador.cep,
                    nf.NotaFiscalPessoaTomador.cidade,
                    cnpjCpf = nf.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
                    nf.NotaFiscalPessoaTomador.complemento,
                    nf.NotaFiscalPessoaTomador.email,
                    nf.NotaFiscalPessoaTomador.endereco,
                    nf.NotaFiscalPessoaTomador.indicadorCnpjCpf,
                    nf.NotaFiscalPessoaTomador.inscricaoEstadual,
                    nf.NotaFiscalPessoaTomador.inscricaoMunicipal,
                    nf.NotaFiscalPessoaTomador.tipoEndereco,
                    nf.NotaFiscalPessoaTomador.uf,
                    nf.NotaFiscalPessoaTomador.id
                },

                NotaFiscalPessoaPrestador = (nf.NotaFiscalPessoaPrestador == null) ? null : new
                {
                    nf.NotaFiscalPessoaPrestador.razao,
                    nf.NotaFiscalPessoaPrestador.numero,
                    nf.NotaFiscalPessoaPrestador.bairro,
                    nf.NotaFiscalPessoaPrestador.cep,
                    nf.NotaFiscalPessoaPrestador.cidade,
                    cnpjCpf = nf.NotaFiscalPessoaPrestador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
                    nf.NotaFiscalPessoaPrestador.complemento,
                    nf.NotaFiscalPessoaPrestador.email,
                    nf.NotaFiscalPessoaPrestador.endereco,
                    nf.NotaFiscalPessoaPrestador.indicadorCnpjCpf,
                    nf.NotaFiscalPessoaPrestador.inscricaoEstadual,
                    nf.NotaFiscalPessoaPrestador.inscricaoMunicipal,
                    nf.NotaFiscalPessoaPrestador.tipoEndereco,
                    nf.NotaFiscalPessoaPrestador.uf,
                    nf.NotaFiscalPessoaPrestador.id
                }

            }, JsonRequestBehavior.AllowGet);
        }


        //Salva Nota
        [HttpPost]
        public JsonResult Salvar(NotaFiscal notafiscal, OrdemVenda ov)
        {
            try
            {
                var erroscpag = new List<string>();
                var titulo = new StringBuilder();
                var emailaviso = _estabobj.emailNotificacoes;
                var corpoemail = new StringBuilder();
                var erros = notafiscal.Validar(ModelState);
                var nfaux = notafiscal ;
                var ovaux = ov;

                if (erros.Count() > 0)
                {
                    return Json(new { CDMessage = "NOK", DSMessage = "Campos Inválidos", Erros = erros }, JsonRequestBehavior.AllowGet);
                }

                if (notafiscal.estabelecimento_id != _estab)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = _mensagemTrocaAba,
                    });

                }

                var plano = new PlanoDeConta().ObterTodos().Where(p => p.codigo.Equals("01.01")).FirstOrDefault();

                if (plano == null)
                {
                    return Json(new { CDMessage = "NOK", DSMessage = "Plano de contas 01.01 não configurado", Erros = erros }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(ov.pessoanome))
                {
                    return Json(new { CDMessage = "NOK", DSMessage = "Pessoa não encontrada ou não informada.", Erros = erros }, JsonRequestBehavior.AllowGet);
                }


                var nomePessoa = ov.pessoanome.Split(',')[0];
                var cnpjPessoa = ov.pessoanome.Split(',')[1].Replace("-","").Replace("/", "").Replace(".", "");

                var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa, _paramBase);

                if (pessoa == null)
                {
                    pessoa = new Pessoa().ObterPorRazaoCNPJ(nomePessoa, "", _paramBase);

                    if (pessoa == null)
                    {
                        return Json(new { CDMessage = "NOK", DSMessage = "Pessoa não encontrada ou não informada.", Erros = erros }, JsonRequestBehavior.AllowGet);
                    }
                }
                


                var tipoManual = new OrigemMovimento().TipoFaturamento(_paramBase);
                var tipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
                var tipoNotaPromissoria = new TipoDocumento().TipoNotaPromissoria();

                DbControle db = new DbControle();
                var bm = new BancoMovimento();

                if (notafiscal.id != 0)
                {
                    bm = new BancoMovimento().ObterPorNFES(notafiscal.id, db, _paramBase).FirstOrDefault();

                    if (bm == null)
                    {
                        bm = new BancoMovimento();
                    }
                }



                if (notafiscal.id != 0)
                {
                    notafiscal = new NotaFiscal().ObterPorId(notafiscal.id, db);

                    if (ov == null)
                    {
                        return Json(new { CDMessage = "NOK", DSMessage = "Ordem não encontrado.", Erros = erros }, JsonRequestBehavior.AllowGet);
                    }

                    notafiscal.aliquotaINSS = nfaux.aliquotaINSS;
                    notafiscal.aliquotaIrrf = nfaux.aliquotaIrrf;
                    notafiscal.aliquotaISS = nfaux.aliquotaISS;
                    notafiscal.banco_id = nfaux.banco_id;
                    notafiscal.basedeCalculo = nfaux.basedeCalculo;
                    notafiscal.codigoCEI = nfaux.codigoCEI;
                    notafiscal.ObraNumEncapsulamento = nfaux.ObraNumEncapsulamento;
                    notafiscal.codigoServico = nfaux.codigoServico;
                    notafiscal.codigoVerificacao = nfaux.codigoVerificacao;
                    notafiscal.cofinsRetida = nfaux.cofinsRetida;
                    notafiscal.cofinsRetida = nfaux.cofinsRetida;
                    notafiscal.csllRetida = nfaux.csllRetida;
                    notafiscal.dataAlteracao = DateTime.Now;
                    
                    notafiscal.usuarioalteracaoid = _paramBase.usuario_id;
                    notafiscal.dataVencimentoNfse = SoftFin.Utils.UtilSoftFin.TiraHora(nfaux.dataVencimentoNfse);
                    notafiscal.DataVencimentoOriginal = SoftFin.Utils.UtilSoftFin.TiraHora(nfaux.DataVencimentoOriginal);
                    notafiscal.discriminacaoServico = nfaux.discriminacaoServico;
                    notafiscal.entradaSaida = nfaux.entradaSaida;
                    notafiscal.fonteCargaTributaria = nfaux.fonteCargaTributaria;
                    notafiscal.irrf = nfaux.irrf;
                    notafiscal.matriculaObra = nfaux.matriculaObra;
                    notafiscal.municipio_id = nfaux.municipio_id;
                    notafiscal.operacao_id = nfaux.operacao_id;
                    notafiscal.ordemVenda_id = nfaux.ordemVenda_id;
                    notafiscal.percentualCargaTributaria = nfaux.percentualCargaTributaria;
                    notafiscal.pisRetido = nfaux.pisRetido;
                    notafiscal.serieRps = nfaux.serieRps;
                    notafiscal.valorCargaTributaria = nfaux.valorCargaTributaria;
                    notafiscal.valorDeducoes = nfaux.valorDeducoes;
                    notafiscal.valorINSS = nfaux.valorINSS;
                    notafiscal.valorISS = nfaux.valorISS;
                    notafiscal.valorLiquido = nfaux.valorLiquido;
                    notafiscal.valorNfse = nfaux.valorNfse;
                    notafiscal.ValAliqPIS = nfaux.ValAliqPIS;
                    notafiscal.ValAliqISSRetido = nfaux.ValAliqISSRetido;
                    notafiscal.ValAliqCOFINS = nfaux.ValAliqCOFINS;
                    notafiscal.ValAliqCSLL = nfaux.ValAliqCSLL;
                    notafiscal.ValISSRetido = nfaux.ValISSRetido;
                    notafiscal.TipoFaturamento = nfaux.TipoFaturamento;
                    notafiscal.notaFiscalIntermediario_id = nfaux.notaFiscalIntermediario_id;
                    notafiscal.notaFiscalPrestador_id = nfaux.notaFiscalPrestador_id;
                    //notafiscal.notaFiscalTomador_id = nfaux.notaFiscalTomador_id;
                    notafiscal.RespRetencao = nfaux.RespRetencao;
                    notafiscal.LocalPrestServ = nfaux.LocalPrestServ;

                    var nfTomador = new NotaFiscalPessoa().ObterPorId(notafiscal.notaFiscalTomador_id.Value, db);

                    nfTomador.bairro = nfaux.NotaFiscalPessoaTomador.bairro;
                    nfTomador.cep = nfaux.NotaFiscalPessoaTomador.cep;
                    nfTomador.cidade = nfaux.NotaFiscalPessoaTomador.cidade;
                    nfTomador.cnpjCpf = nfaux.NotaFiscalPessoaTomador.cnpjCpf;
                    nfTomador.complemento = nfaux.NotaFiscalPessoaTomador.complemento;
                    nfTomador.email = nfaux.NotaFiscalPessoaTomador.email;
                    nfTomador.endereco = nfaux.NotaFiscalPessoaTomador.endereco;
                    nfTomador.uf = nfaux.NotaFiscalPessoaTomador.uf;
                    nfTomador.tipoEndereco = nfaux.NotaFiscalPessoaTomador.tipoEndereco;
                    nfTomador.razao = nfaux.NotaFiscalPessoaTomador.razao;
                    nfTomador.inscricaoMunicipal = nfaux.NotaFiscalPessoaTomador.inscricaoMunicipal;
                    nfTomador.numero = nfaux.NotaFiscalPessoaTomador.numero;
                    nfTomador.inscricaoEstadual = nfaux.NotaFiscalPessoaTomador.inscricaoEstadual;
                    nfTomador.Alterar(_paramBase, db);

                }
                else
                {
                    notafiscal.usuarioinclusaoid = _usuarioobj.id;
                    notafiscal.dataInclusao = DateTime.Now;

                    bm.dataInclusao = DateTime.Now;
                    bm.usuarioinclusaoid = _usuarioobj.id;


                }

                bm.banco_id = notafiscal.banco_id.Value;
                bm.data = SoftFin.Utils.UtilSoftFin.TiraHora(notafiscal.dataVencimentoNfse);
                bm.historico = ov.descricao;
                bm.valor = notafiscal.valorNfse;
                bm.origemmovimento_id = tipoManual;
                bm.tipoDeMovimento_id = tipoEntrada;
                bm.tipoDeDocumento_id = tipoNotaPromissoria;
                bm.planoDeConta_id = plano.id;
                bm.usuarioinclusaoid = _usuarioobj.id;

                if (notafiscal.banco_id == null)
                {
                    ov = new OrdemVenda().ObterPorId(notafiscal.ordemVenda_id.Value, db);

                    if (ov == null)
                    {
                        return Json(new { CDMessage = "NOK", DSMessage = "Banco não informado não encontrado.", Erros = erros }, JsonRequestBehavior.AllowGet);
                    }

                    ov.usuarioalteracaoid = _usuarioobj.id;
                    ov.dataAlteracao = DateTime.Now;
                }
                else
                {
                    ov.usuarioinclusaoid = _usuarioobj.id;
                    ov.dataInclusao = DateTime.Now;
                    ov.statusParcela_ID = new StatusParcela().ObterTodos().Where(p => p.status == "Emitida").First().id;
                    ov.estabelecimento_id = _estab;
                    ov.pessoas_ID = pessoa.id;
                }

                // Inicio Lançamento Contabil
                var tipoFaturamento = new OrigemMovimento().TipoFaturamento(_paramBase);
                var idCredito = 0;
                var idDebito = 0;
                var ccLC = new LancamentoContabil();
                var ccDebito = new LancamentoContabilDetalhe();
                var ccCredito = new LancamentoContabilDetalhe();
                var pcf = new PessoaContaContabil().ObterPorPessoa(pessoa.id, db);

                if (pcf != null)
                {
                    if (pcf.contaContabilReceberPadrao_id != null)
                    {
                        idDebito = pcf.contaContabilReceberPadrao_id.Value;
                    }
                }

                var ecf = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);
                if (ecf != null)
                {
                    idCredito = ecf.ContaContabilNFServico_id;
                    if (idDebito == 0)
                        idDebito = ecf.ContaContabilRecebimento_id;
                }
                //Fim Lançamento Contabil

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    int numeroref = 0;

                    if (ov.id == 0)
                        ov.Incluir(ov, ref numeroref, _paramBase, db);
                    else
                        ov.Alterar(ov, _paramBase, db);

                    notafiscal.ordemVenda_id = ov.id;
                    if (notafiscal.id == 0)
                    {
                        notafiscal.dataEmissaoNfse = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                        notafiscal.dataEmissaoRps = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                        notafiscal.Incluir(_paramBase, db);
                    }
                    else
                        notafiscal.Alterar(_paramBase, db);

                    bm.notafiscal_id = notafiscal.id;

                    if (bm.id == 0)
                        bm.Incluir(bm, _paramBase, db);
                    else
                        bm.Alterar(bm, _paramBase, db);


                    // Inicio Lançamento Contabil
                    
                    if (idCredito != 0 && idDebito != 0)
                    {
                        ccLC = ccLC.ObterPorNotaFiscal(notafiscal.id, _paramBase, db).FirstOrDefault();

                        if (ccLC == null)
                        {
                            ccLC = new LancamentoContabil();
                        }
                        else
                        {
                            var ccaux = ccDebito.ObterPorNotaFiscal(notafiscal.id, _paramBase, db);
                            ccCredito = ccaux.Where(p => p.DebitoCredito == "C").FirstOrDefault();
                            ccDebito = ccaux.Where(p => p.DebitoCredito == "D").FirstOrDefault();
                        }


                        ccLC.data = notafiscal.dataVencimentoNfse;
                        ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                        ccLC.estabelecimento_id = _paramBase.estab_id;
                        ccLC.historico = ov.descricao;
                        ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                        ccLC.origemmovimento_id = tipoFaturamento;
                        ccLC.UnidadeNegocio_ID = ov.unidadeNegocio_ID;

                        ccDebito.contaContabil_id = idDebito;
                        ccDebito.DebitoCredito = "D";
                        
                        ccDebito.valor = ov.valor;
                        ccCredito.contaContabil_id = idCredito;
                        ccCredito.DebitoCredito = "C";
                        ccCredito.valor = ov.valor;



                        if (ccLC.id == 0)
                        {
                            ccLC.notafiscal_id = notafiscal.id;
                            var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                            ccLC.codigoLancamento = numeroLcto;
                            ccLC.Incluir(_paramBase, db);
                            ccDebito.lancamentoContabil_id = ccLC.id;
                            ccDebito.Incluir(_paramBase, db);
                            ccCredito.lancamentoContabil_id = ccLC.id;
                            ccCredito.Incluir(_paramBase, db);
                        }
                        else
                        {
                            ccLC.Alterar(_paramBase, db);
                            ccDebito.Alterar(_paramBase, db);
                            ccCredito.Alterar(_paramBase, db);
                        }

                    }
                    //Fim Lançamento Contabil
                    //Fim Lançamento Contabil

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                            
                    
                }
                DTORetornoNFEs resultado = new DTORetornoNFEs();

                resultado.Cabecalho = new tpCabecalho();

                if (emailaviso != null)
                {
                    EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, _estabobj);
                }


                return Json(new { CDStatus = "OK", DSMessage = "Nota salva com sucesso" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Historico(NotaFiscal notafiscal)
        {
            try
            {

                if (notafiscal.estabelecimento_id != _estab)
                {


                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
                    });

                }

                var objs = new LogNFXMLPrincipal().ObterTodosPorNota(notafiscal.id).OrderBy(p => p.dataInsert);

                return Json(new
                {
                    CDStatus = "OK",
                    objs = objs.Select(p => new
                    {
                        aceito = p.aceito ? "SIM": "NÃO",
                        dataInsert = p.dataInsert.ToString("o"),
                        p.tipo,
                        p.usuarioInsert,
                        p.id,
                        alertas = p.logNFXMLAlertas.Select( e=> new {e.codigo, e.descricao}),
                        erros = p.logNFXMLErros.Select( e=> new {e.codigo, e.descricao})
                    }
                                            )
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Private
        private string ConsultaLinkPF(NotaFiscal notaFiscal)
        {
            string url = "";
            if (notaFiscal.numeroNfse != null)
            {
                url = "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?inscricao={inscricao}&nf={numeronf}&verificacao={codigoverificacao}";
                url = url.Replace("{codigoverificacao}", notaFiscal.codigoVerificacao);
                url = url.Replace("{numeronf}", notaFiscal.numeroNfse.ToString());
                url = url.Replace("{inscricao}", _estabobj.InscricaoMunicipal.ToString().Replace("-", "").Replace(".", "").Replace("/", ""));

            }
            return url;
        }


        public const string RPSEMITIDA_TEXTO = "1 - RPS Emitido";
        public const string NFSEGERADA_TEXTO = "2 - NFS-e Gerada";
        public const string NFSECANCELADAEMCONF_TEXTO = "3 - NFS-e cancelada sem confirmação";
        public const string NFSECANCELADACCONF_TEXTO = "4 - NFS-e cancelada com confirmação";
        public const string NFSEBAIXADA_TEXTO = "5 - NFS-e baixada como perda";
        public const string NFSEAVULSA_TEXTO = "6 - NFS-e Avulsa";

        private string ConsultaSituacao(int situacao)
        {
            switch (situacao)
            {
                case 1:
                    return RPSEMITIDA_TEXTO;
                case 2:
                    return NFSEGERADA_TEXTO;
                case 3:
                    return NFSECANCELADAEMCONF_TEXTO;
                case 4:
                    return NFSECANCELADACCONF_TEXTO;
                case 5:
                    return NFSEBAIXADA_TEXTO;
                case 6:
                    return NFSEAVULSA_TEXTO;
                default:
                    return "Indefinido";
            }
        }
        private DTORetornoNFEs EnviaNFBussiness(int id)
        {
            var objPedidoEnvioLoteRPS = new DTONotaFiscal();

            var objEstab = _paramBase.estab_id;
            var objNF = new NotaFiscal().ObterPorId(id);
            var resultado = new DTORetornoNFEs();

            AtualizaNFServiceValidar(_estabobj, objNF);

            var listaNF = new List<NotaFiscal>();
            listaNF.Add(objNF);

            string caminhoArquivo;
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
            ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);

            if (objNF.situacaoPrefeitura_id == Models.NotaFiscal.NFCANCELADAEMCONF)
            {
                if (!string.IsNullOrEmpty(_estabobj.MigrateCode))
                {
                    var envio = new ConversaoMigrateServico().ConverterDTOMigrateCancelamento(_estabobj, listaNF.First());
                    var service = new SoftFin.Migrate.NFSe.Business.CancelamentoNFServico();
                    var codeEmpresa = ConfigurationManager.AppSettings["MigrateCode"].ToString();
                    var resultadoMigrate = service.Execute(envio, codeEmpresa, _estabobj.MigrateCode);

                    resultado = ConversaoMigrateServico.RetornoMigrateToRetornoDto(resultadoMigrate);
                    new Conversao().ConverteRetornoGravaLog(resultado, objNF.id, _usuario);
                }
                else
                {
                    var objCancelamentoNFe = new DTONotaFiscal();
                    new Conversao().ConverterNFEs(objCancelamentoNFe, _estabobj, listaNF);
                    var service = new SoftFin.NFSe.Business.SFCancelamentoNFSe();
                    resultado = service.Execute(objCancelamentoNFe, cert, "");
                    new Conversao().ConverteRetornoGravaLog(resultado, objNF.id, _usuario);
                }

            }
            else if (objNF.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA)
            {
                if (!string.IsNullOrEmpty(_estabobj.MigrateCode))
                {
                    var envio = new ConversaoMigrateServico().ConverterDTOMigrateEnvio(_estabobj, listaNF.First(), _paramBase);
                    var service = new SoftFin.Migrate.NFSe.Business.EnvioRPS();
                    var codeEmpresa = ConfigurationManager.AppSettings["MigrateCode"].ToString();
                    
                    var resultadoMigrate = service.Execute(envio, codeEmpresa, _estabobj.MigrateCode);
                    resultado = ConversaoMigrateServico.RetornoMigrateToRetornoDto(resultadoMigrate);
                    new Conversao().ConverteRetornoGravaLog(resultado, objNF.id, _usuario);
                }
                else
                {
                    new Conversao().ConverterNFEs(objPedidoEnvioLoteRPS, _estabobj, listaNF);
                    var arquivoxml = "";
                    var service = new SoftFin.NFSe.Business.SFEnvioLoteRPS();
                    resultado = service.Execute(objPedidoEnvioLoteRPS, cert, arquivoxml);
                    new Conversao().ConverteRetornoGravaLog(resultado, objNF.id, _usuario);
                }
            }
            else
            {
                throw new Exception("Situação de Nota Fiscal inválida.");
            }
            try
            {
                cert = null;
                System.IO.File.Delete(caminhoArquivo);
            }
            catch
            {
            }

            return resultado;
        }


        public JsonResult ObterOrdemVendaAberto()
        {
            var Listas = PesquisaNFse2();


            return Json(
                            Listas.Select(p => new
                            {
                                data = p.data.ToString("o"),
                                dataAutorizacao = (p.dataAutorizacao == null) ? "" : p.dataAutorizacao.Value.ToString("o"),
                                p.estabelecimento_id,
                                p.id,
                                p.itemProdutoServico_ID,
                                p.Numero,
                                p.valor,
                                codigoServico = (p.parcelaContrato_ID == null) ? "" : (
                                            p.ParcelaContrato.codigoServicoEstabelecimento_id == null ? "" :
                                            p.ParcelaContrato.CodigoServicoEstabelecimento.CodigoServicoMunicipio.codigo),

                                DataVencimentoOriginal = (p.ParcelaContrato != null) ?
                                        (
                                            (p.ParcelaContrato.DataVencimento != null) ? p.ParcelaContrato.DataVencimento.Value.ToString("o") : "") : "",
                                nome = p.Pessoa.nome,
                                pessoanome = p.Pessoa.nome + ", " + p.Pessoa.cnpj,
                                unidadeNegocio_ID = p.unidadeNegocio_ID,
                                unidade = p.UnidadeNegocio.unidade,
                                p.descricao,
                                descricaoparcela = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.descricao,
                                pedido = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.ContratoItem.pedido,
                                contrato = (p.parcelaContrato_ID == null) ? "" : (p.ParcelaContrato.ContratoItem.Contrato.contrato),
                                descricaocontrato = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.ContratoItem.Contrato.descricao,
                                numeroparcela = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.parcela.ToString(),

                                banco_id = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.banco_id == null) ? "" : p.ParcelaContrato.banco_id.ToString()),
                                banco = (p.parcelaContrato_ID == null) ? "" :
                                            (
                                                (p.ParcelaContrato.banco_id == null)
                                                ? "" : p.ParcelaContrato.banco.nomeBanco
                                                + " " +
                                                p.ParcelaContrato.banco.agencia
                                                + " " +
                                                p.ParcelaContrato.banco.contaCorrente
                                                + "-" +
                                                p.ParcelaContrato.banco.contaCorrenteDigito
                                            ),
                                operacao_id = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.operacao_id == null) ? "" : p.ParcelaContrato.operacao_id.ToString()),
                                operacao = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.operacao_id == null)
                                                ? "" : p.ParcelaContrato.Operacao.descricao),
                                pessoaid = p.pessoas_ID,
                                tabelaPreco_ID = (p.tabelaPreco_ID != null) ? p.tabelaPreco_ID.Value : 0


                            }
                            )
                            , JsonRequestBehavior.AllowGet
                        )
                 ;

        }

        private List<OrdemVenda> PesquisaNFse2()
        {
            var obj = new OrdemVenda();
            var date = DateTime.Now.AddMonths(1);
            IEnumerable<OrdemVenda> lista;

            if (_estabobj.autorizacaoFaturamento == null)
                _estabobj.autorizacaoFaturamento = false;

            if (_estabobj.autorizacaoFaturamento.Value)
                lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.data <= date);
            else
                lista = obj.ObterTodosPendentes(_paramBase).Where(x => x.data <= date);
            
            return lista.OrderBy(p => p.data).ToList();
        }


        private static void AtualizaNFServiceValidar(Estabelecimento objEstab, NotaFiscal objNF)
        {
            if (objNF == null)
                throw new Exception("Nota não encotrada.");

            if (objNF.estabelecimento_id != objEstab.id)
                throw new Exception("Estabelecimento não compativel.");

            if (objNF.numeroRps == 0)
                throw new Exception("RPS não emitido.");

            if (objNF.serieRps == null)
                throw new Exception("RPS não emitido.");
        }

        private List<NotaFiscal> PesquisaNFse()
        {
            var lista = new NotaFiscal().ObterTodos(_paramBase).Where(p => p.TipoFaturamento == 0);
            return lista.ToList();
        }
        private static void GeraEmailSemCertificado(NotaFiscal notafiscal, StringBuilder titulo, StringBuilder corpoemail, OrdemVenda ov, ParcelaContrato pc)
        {
            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>RPS</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>Data</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine("<b>Descrição</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine("<b>Tomador</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine("<b>Valor</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(notafiscal.numeroRps + "-" + notafiscal.serieRps);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(pc.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(pc.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(pc.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            titulo.Append("SoftFin - NFS-e Manual - Nota Gerada com sucesso!");
        }
        private void ObtemCertificadoX509(Estabelecimento objEstab, out string caminhoArquivo, out System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CertTMP/");
            Directory.CreateDirectory(uploadPath);

            var nomearquivonovo = Guid.NewGuid().ToString();

            caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

            cert = UtilSoftFin.BuscaCert(objEstab.id, objEstab.senhaCertificado, caminhoArquivo, objEstab.CNPJ);
        }
        private void EnviaEmail(string titulo, string corpo, string emailaviso, Estabelecimento estab)
        {
            var email = new Email();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            var arquivohmtl = Path.Combine(path, "Email02.html");
            string readText = System.IO.File.ReadAllText(arquivohmtl);
            readText = readText.Replace("{Titulo}", titulo);
            readText = readText.Replace("{Corpo}", corpo);
            readText = readText.Replace("{nomeestab}", estab.NomeCompleto);
            email.EnviarMensagem(emailaviso, titulo, readText, true);
        }
        private string CriaPastaeNomeXML(Estabelecimento estab)
        {
            var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/OFXTemp/");
            Directory.CreateDirectory(uploadPath);
            var arquivoxml = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/OFXTemp/"), "NFEnvioAutomatico" + estab.id + ".xml");

            try
            {
                System.IO.File.Delete(arquivoxml);
            }
            catch
            {
            }
            return arquivoxml;
        }
        #endregion
    }
}
