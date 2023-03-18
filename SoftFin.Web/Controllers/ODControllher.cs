using SoftFin.Web.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SoftFin.Web.Classes;

namespace SoftFin.Web.Controllers
{
    public class ODController : BaseController
    {
        #region Public
        //[HttpPost]
        //public override JsonResult TotalizadorDash(int? id)
        //{
        //    base.TotalizadorDash(id);
        //    var soma = new BancoMovimento().ObterEntreData(DataInicial, DataFinal).Sum(x => x.valor).ToString("n");
        //    return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new NotaFiscal().ObterEntreDataOD(DateTime.Now.AddMonths(-3), DateTime.Now, _paramBase);

            return Json(
                            objs.Select(p => new
                            {
                                dataEmissaoNfse = (p.dataEmissaoNfse == null) ? "" : p.dataEmissaoNfse.ToString("o"),
                                DataVencimentoOriginal = (p.DataVencimentoOriginal == null) ? "" : p.DataVencimentoOriginal.ToString("o"),
                                dataVencimentoNfse = (p.dataVencimentoNfse == null) ? "" : p.dataVencimentoNfse.ToString("o"),
                                razao = (p.NotaFiscalPessoaTomador == null) ? ((p.NotaFiscalPessoaPrestador == null) ? "" : p.NotaFiscalPessoaPrestador.razao) : p.NotaFiscalPessoaTomador.razao,
                                p.valorNfse,
                                p.id,
                                p.estabelecimento_id,
                                p.entradaSaida,
                                p.OrdemVenda.Numero,
                                p.SituacaoRecebimento
                            }
                            )
                            , JsonRequestBehavior.AllowGet
                        )
                 ;
        }
       
        public JsonResult ObterPorId(int id)
        {
            var nf = new NotaFiscal().ObterODPorId(id,_paramBase);
            if (nf == null)
            {
                nf = new NotaFiscal();
                nf.dataEmissaoNfse = DateTime.Now;
                nf.dataEmissaoRps = DateTime.Now;
                nf.situacaoPrefeitura_id = NotaFiscal.NFGERADAENVIADA;
                nf.estabelecimento_id = _estab;
                nf.entradaSaida = "E";
            }

            DbControle db = new DbControle();
            var outros = new NotaFiscalOutroItem().ObterPorNF(nf.id, db, _paramBase);


            return Json(new
            {
                Itens = outros.Select(p => new { p.Codigo, p.Descricao, p.Id, p.Valor, p.unidadenegocio_id, unidadedesc = p.UnidadeNegocio.unidade }),
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
                    nf.valorNfse
                },

                OV = new
                {
                    nf.OrdemVenda.id,
                    nf.OrdemVenda.Numero,
                    nf.OrdemVenda.unidadeNegocio_ID,
                    nf.OrdemVenda.descricao,
                    nf.OrdemVenda.valor,
                    data = nf.OrdemVenda.data.ToString("o"),
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

        public JsonResult ObterPorOV(int id)
        {
            var ov = new OrdemVenda().ObterPorId(id);

            var nf = new NotaFiscal().ObterPorOV(id);

            var listDet = new List<NotaFiscalOutroItem>();
            if (nf == null)
            {
                nf = new NotaFiscal();
                nf.dataEmissaoNfse = DateTime.Now;
                nf.dataEmissaoRps = DateTime.Now;
                nf.situacaoPrefeitura_id = NotaFiscal.NFGERADAENVIADA;
                nf.estabelecimento_id = _estab;
                nf.dataVencimentoNfse = DateTime.Now;
                nf.DataVencimentoOriginal = DateTime.Now;
                nf.ordemVenda_id = ov.id;
                nf.serieRps = "OD";
                nf.situacaoRps = "1";
                nf.discriminacaoServico = "OD";
                nf.dataVencimentoNfse = DateTime.Now;
                nf.DataVencimentoOriginal = DateTime.Now;
                nf.entradaSaida = "E";

                if (ov.parcelaContrato_ID != null)
                {
                    var cnt = new ParcelaContrato().ObterPorId(ov.parcelaContrato_ID.Value, _paramBase);

                    if (cnt.DataVencimento != null)
                    {
                        nf.dataVencimentoNfse = cnt.DataVencimento.Value;
                        nf.DataVencimentoOriginal = cnt.DataVencimento.Value;

                    }

                    var itens = new ContratoItemPedido().ObterPorParcela(ov.parcelaContrato_ID.Value, _paramBase);

                    foreach (var item in itens)
                    {
                        listDet.Add(new NotaFiscalOutroItem {  Codigo = item.Pedido, Descricao = item.Descricao, Valor = item.Valor, UnidadeNegocio = item.UnidadeNegocio, unidadenegocio_id = item.unidadenegocio_id});
                    }

                }

            }



            return Json(new
            {
                Itens = listDet.Select(p => new { p.Codigo, p.Descricao, p.Id, p.Valor, p.unidadenegocio_id, unidadedesc = p.UnidadeNegocio.unidade }),
                NF = new
                {
                    nf.situacaoPrefeitura_id,
                    nf.estabelecimento_id,
                    dataVencimentoNfse = nf.dataVencimentoNfse.ToString("O"),
                    DataVencimentoOriginal = nf.DataVencimentoOriginal.ToString("o"),
                    nf.ordemVenda_id,
                    nf.id,
                    nf.serieRps,
                    nf.situacaoRps,
                    nf.discriminacaoServico,
                    dataEmissaoNfse = nf.dataEmissaoNfse.ToString("o"),
                    dataEmissaoRps = nf.dataEmissaoRps.ToString("o")
                },
                OV = new
                {
                    ov.id,
                    ov.Numero,
                    ov.unidadeNegocio_ID,
                    ov.descricao,
                    ov.valor,
                    data = ov.data.ToString("o"),
                },


                NotaFiscalPessoaTomador = (ov.Pessoa == null) ? null : new
                {
                    ov.Pessoa.razao,
                    ov.Pessoa.numero,
                    ov.Pessoa.bairro,
                    ov.Pessoa.cep,
                    ov.Pessoa.cidade,
                    cnpjCpf = ov.Pessoa.cnpj.Replace(".", "").Replace("/", "").Replace("-", ""),
                    ov.Pessoa.complemento,
                    email = ov.Pessoa.eMail,
                    ov.Pessoa.endereco,
                    indicadorCnpjCpf = ((ov.Pessoa.cnpj != null) ? ((ov.Pessoa.cnpj.Length == 11)? 2 : 1) : 0),
                    tipoEndereco = ov.Pessoa.TipoEndereco.Descricao,
                    ov.Pessoa.uf,
                    id = 0
                }
            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult Salvar(List<NotaFiscalOutroItem> outroItems, OrdemVenda ov, NotaFiscalPessoa nfp, NotaFiscal nf )
        {
            try
            {
                var validacao = ov.Validar(ModelState);
                var plano = new PlanoDeConta().ObterTodos().Where(p => p.codigo.Equals("01.01")).FirstOrDefault();

                if (plano == null)
                {
                    validacao.Add("Plano de contas 01 não configurado");
                    return Json(new { CDMessage = "NOK", DSMessage = "Plano de contas 01 não configurado", Erros = validacao }, JsonRequestBehavior.AllowGet);
                }

                if (outroItems == null)
                {
                    validacao.Add("Informe os items.");
                }

                if (nf.banco_id == 0)
                {
                    validacao.Add("Informe o banco.");
                }


                if (nf.banco_id == null)
                {
                    validacao.Add("Informe o banco..");
                }
                if (ov.unidadeNegocio_ID == 0)
                {
                    validacao.Add("Informe a unidade.");
                }
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                validacao = nfp.Validar(ModelState);

                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                var tipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
                var tipoNotaPromissoria = new TipoDocumento().TipoNotaPromissoria();
                var tipoFaturamento = new OrigemMovimento().TipoFaturamento(_paramBase);
                
                var idCredito = 0;
                var idDebito = 0;
                var ccLC = new LancamentoContabil();
                var ccDebito = new LancamentoContabilDetalhe();
                var ccCredito = new LancamentoContabilDetalhe();

                var db = new DbControle();
                var pcf = new PessoaContaContabil().ObterPorPessoa(ov.id, db);

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
                    idCredito = ecf.ContaContabilOutro_id;
                    if (idDebito == 0)
                        idDebito = ecf.ContaContabilRecebimento_id;
                }



                
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var nfaux = new NotaFiscal().ObterODPorId(nf.id, _paramBase, db);
                    var ovid = 0;
                    if (ov.id == 0)
                    {
                        int numref = 0;
                        ov.Incluir(ov, ref numref, _paramBase, db);
                        ovid = ov.id;
                    }
                    else
                    {
                        var ovOriginal = ov.ObterPorId(ov.id,  db);
                        ovOriginal.descricao = ov.descricao;
                        ovOriginal.data = ov.data;
                        ovOriginal.valor = ov.valor;
                        ovOriginal.unidadeNegocio_ID = ov.unidadeNegocio_ID;
                        ovOriginal.Alterar(ovOriginal, _paramBase, db);
                        ovid = ovOriginal.id;

                        if (ovOriginal.parcelaContrato_ID != null)
                        {
                            var parcelaitem = new ParcelaContrato().ObterPorId(ovOriginal.parcelaContrato_ID.Value, db, _paramBase);
                            if (parcelaitem != null)
                            {
                                parcelaitem.statusParcela_ID = StatusParcela.SituacaoEmitida();
                                parcelaitem.Alterar(_paramBase, db);
                            }
                        }
                    }


                    if (nfp.id == 0)
                    {
                        nfp.Incluir(_paramBase, db);
                        nf.NotaFiscalPessoaTomador = nfp;
                    }
                    else
                    {


                        var nfpOriginal = new NotaFiscalPessoa().ObterPorId(nfaux.notaFiscalTomador_id.Value, db);

                        nfpOriginal.razao = nfp.razao;
                        nfpOriginal.numero = nfp.numero;
                        nfpOriginal.bairro = nfp.bairro;
                        nfpOriginal.cep = nfp.cep;
                        nfpOriginal.cidade = nfp.cidade;
                        nfpOriginal.cnpjCpf = nfp.cnpjCpf;
                        nfpOriginal.complemento = nfp.complemento;
                        nfpOriginal.email = nfp.email;
                        nfpOriginal.endereco = nfp.endereco;
                        nfpOriginal.indicadorCnpjCpf = nfp.indicadorCnpjCpf;
                        nfpOriginal.tipoEndereco = nfp.tipoEndereco;
                        nfpOriginal.uf = nfp.uf;
                        nfpOriginal.id = nfp.id;
                        nfpOriginal.Alterar(_paramBase, db);
                    }

                    nf.ordemVenda_id = ov.id;
                    nf.valorNfse = ov.valor;
                    nf.valorLiquido = ov.valor;
                    nf.entradaSaida = "E";

                    if (nf.id == 0)
                    {
                        nf.ordemVenda_id = ovid;
                        nf.TipoFaturamento = 2; //Outros
                        nf.situacaoPrefeitura_id = NotaFiscal.NFGERADAENVIADA;
                        nf.Incluir(_paramBase, db);
                    }
                    else
                    {
                        nfaux.dataEmissaoNfse = nf.dataEmissaoNfse;
                        nfaux.dataEmissaoRps = nf.dataEmissaoRps;
                        nfaux.dataVencimentoNfse = nf.dataVencimentoNfse;
                        nfaux.DataVencimentoOriginal = nf.DataVencimentoOriginal;
                        nfaux.banco_id = nf.banco_id;
                        nfaux.ordemVenda_id = ovid;
                        nfaux.TipoFaturamento = 2; //Outros
                        nfaux.situacaoPrefeitura_id = NotaFiscal.NFGERADAENVIADA;
                        nfaux.valorNfse = ov.valor;
                        nfaux.valorLiquido = ov.valor;

                        nfaux.Alterar(_paramBase, db);
                    }


                    var outrosdelete = new NotaFiscalOutroItem().ObterPorNF(nf.id, db, _paramBase);

                    foreach (var item in outrosdelete)
                    {
                        item.Excluir(item.Id, db, _paramBase);
                    }

                    foreach (var item in outroItems)
                    {
                        item.notafical_id = nf.id;
                        item.Incluir(item, db, _paramBase);
                    }


                    var bm = new BancoMovimento().ObterPorNFPlano(nf.id, plano.id, db, _paramBase) ;

                    if (bm == null)
                        bm = new BancoMovimento();

                    bm.banco_id = nf.banco_id.Value;
                    bm.data = nf.dataVencimentoNfse;
                    bm.historico = ov.descricao;
                    bm.valor = nf.valorNfse;
                    bm.origemmovimento_id = tipoFaturamento;
                    bm.tipoDeMovimento_id = tipoEntrada;
                    bm.tipoDeDocumento_id = tipoNotaPromissoria;
                    bm.planoDeConta_id = plano.id;
                    bm.usuarioinclusaoid = _usuarioobj.id;
                    bm.dataInclusao = DateTime.Now;
                    bm.notafiscal_id = nf.NotaFiscalNFE_id;
                    bm.UnidadeNegocio_id = ov.unidadeNegocio_ID;
                    bm.notafiscal_id = nf.id;

                    if (bm.id == 0)
                        bm.Incluir(_paramBase, db);
                    else
                        bm.Alterar(_paramBase, db);


                    // Inicio Lançamento Contabil
                    if (idCredito != 0 && idDebito != 0)
                    {
                        ccLC = ccLC.ObterPorNotaFiscal(nf.id, _paramBase, db).FirstOrDefault();
                        
                        if (ccLC == null)
                        {
                            ccLC = new LancamentoContabil();
                        }
                        else
                        {
                            var ccaux = ccDebito.ObterPorNotaFiscal(nf.id, _paramBase, db);
                            ccCredito = ccaux.Where(p => p.DebitoCredito == "C").FirstOrDefault();
                            ccDebito = ccaux.Where(p => p.DebitoCredito == "D").FirstOrDefault();
                        }


                        ccLC.data = nf.dataVencimentoNfse;
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
                            ccLC.notafiscal_id = nf.id;
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



                    

                    dbcxtransaction.Commit();
                }
                return Json(new { CDStatus = "OK", DSMessage = "Salvo com sucesso."  }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }



        }

        public JsonResult Excluir(int id)
        {
            try
            {
                var db = new DbControle();
                var nf = new NotaFiscal().ObterPorId(id, db);
                var ovid = nf.ordemVenda_id;
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    
                    string erro = "";
                    //new OrdemVenda().Excluir(nf.ordemVenda_id.Value, ref erro, _paramBase, db);
                    //if (erro != "")
                    //    return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);

                    var outrosdelete = new NotaFiscalOutroItem().ObterPorNF(nf.id, db, _paramBase);

                    foreach (var item in outrosdelete)
                    {
                        item.Excluir(item.Id, db, _paramBase);
                        if (erro != "")
                            return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);
                    }

                    var ccaux = new LancamentoContabilDetalhe().ObterPorNotaFiscal(nf.id, _paramBase, db);
                    foreach (var item in ccaux)
                    {
                        item.Excluir(item.id, ref erro, _paramBase, db);
                        if (erro != "")
                            return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);
                    }

                    var ccLCs = new LancamentoContabil().ObterPorNotaFiscal(nf.id, _paramBase, db);
                    foreach (var item in ccLCs)
                    {

                        item.Excluir(item.id,ref erro, _paramBase,db);
                        if (erro != "")
                            return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);
                    }

                    new NotaFiscalPessoa().Excluir(nf.NotaFiscalPessoaTomador.id, ref erro, _paramBase, db);
                    if (erro != "")
                        return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);

                    var ccBMs = new BancoMovimento().ObterPorNFES(nf.id, db, _paramBase);

                    foreach (var item in ccBMs)
                    {
                        new BancoMovimento().Excluir(item.id, ref erro, _paramBase, db);
                        if (erro != "")
                            return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);
                    }
                    new NotaFiscal().Excluir(nf.id, ref erro, _paramBase, db);
                    if (erro != "")
                        return Json(new { CDStatus = "NOK", Exception = erro }, JsonRequestBehavior.AllowGet);

                    if (ovid != null)
                    {
                        var ov = new OrdemVenda().ObterPorId(ovid.Value, db);

                        if (ov.parcelaContrato_ID != null)
                        {
                            var parcelaitem = new ParcelaContrato().ObterPorId(ov.parcelaContrato_ID.Value, db, _paramBase);
                            if (parcelaitem != null)
                            {
                                parcelaitem.statusParcela_ID = StatusParcela.SituacaoLiberada();
                                parcelaitem.Alterar(_paramBase, db);
                            }
                        }
                    }


                    dbcxtransaction.Commit();
                }
                return Json(new { CDStatus = "OK", DSMessage = "Excluido com sucesso." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }



        public JsonResult ListaOV()
        {
            var ov = new OrdemVenda().ObterTodosAutorizado(DateTime.Now.AddMonths(-3), DateTime.Now,_paramBase).Where(p => p.NotasFiscais.Count() == 0);

            return Json(

                ov.Select(p => new {
                    p.id,
                    p.Numero,
                    p.descricao,
                    p.valor,
                    data = p.data.ToString("o"),
                    aprovadado = p.usuarioAutorizador_id != null ? "SIM" : "NÂO",
                    p.Pessoa.cnpj,
                    p.Pessoa.nome,
                    p.Pessoa.razao,
                    p.Pessoa.cidade,
                    p.Pessoa.endereco,
                    p.Pessoa.uf,
                    p.Pessoa.complemento,
                    p.Pessoa.cep,
                    p.Pessoa.eMail,
                    p.Pessoa.TelefoneFixo,
                    p.Pessoa.Celular,
                    p.Pessoa.bancoConta,
                    p.Pessoa.agenciaConta,
                    p.Pessoa.agenciaDigito,
                    p.Pessoa.digitoContaBancaria
                })
                
            , JsonRequestBehavior.AllowGet);


        }


        #endregion
    }
}
