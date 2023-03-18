using SoftFin.NFe.DTO;
using SoftFin.Utils;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class NFImportacaoController : BaseController
    {
        // GET: /NFSeImportacao/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult obtemData()
        {
            var filtro = new { instituicao = 1, dataInicial = DateTime.Now.AddDays(-30).ToString("o"), dataFinal = DateTime.Now.ToString("o") };
            return Json(filtro, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ListaNotas()
        {
            try
            {
                var uploadPath = Server.MapPath("~/CertTMP/");
                Directory.CreateDirectory(uploadPath);
                var nomearquivonovo = Guid.NewGuid().ToString();
                string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
                var cert = Utils.UtilSoftFin.BuscaCert(_estab, _estabobj.senhaCertificado, caminhoArquivo, _estabobj.CNPJ);

                var regra = new SoftFin.Sefaz.NFeDistribuicaoDFe();
                var dto = new DTONfe();

                DbControle db = new DbControle();

                var url = db.UrlSefazUF.Where(p => p.UF == _estabobj.UF);

                var urlServico = "";
                if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
                {
                    urlServico = url.Where(p => p.codigo == "NFeConsultaDest").First().UrlSefazPrincipal.url;
                }
                else
                {
                    urlServico = url.Where(p => p.codigo == "NFeConsultaDest").First().UrlSefazPrincipal.urlHomologacao;
                }

                

                var result = regra.Execute(_estabobj.CNPJ, cert, urlServico);

                if (!result.CMDAceito)
                    return Json(new { CDStatus = "NOK", DSMessage = "Comando não aceito no Sefaz, verifique os erros", Alertas = result.Alertas, Erros = result.Erros }, JsonRequestBehavior.AllowGet);

                if (result.Objs == null)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não retornaram notas" }, JsonRequestBehavior.AllowGet);
                }

                var retorno = result.Objs.Select(p => new
                {
                    p.chNFe,
                    p.CNPJ,
                    p.cSitNFe,
                    p.dhEmi,
                    p.dhRecbto,
                    p.digVal,
                    p.IE,
                    p.nProt,
                    p.tpNF,
                    p.vNF,
                    p.xNome,
                    p.xJust,
                    p.xEvento,
                    p.xmlCompleto,

                    ExisteCPAG = (new DocumentoPagarParcela().ObterPorCodigoVerificacao(p.chNFe, _paramBase) != null),
                    ExistePessoas = (new Pessoa().ObterPorCNPJ(p.CNPJ, _paramBase) != null),
                    ExisteNF = (new NotaFiscal().ObterPorCodigoVerificacao(0, p.chNFe, _paramBase) != null),
                });

                return Json(new { CDStatus = "OK", NFSe = retorno }, JsonRequestBehavior.AllowGet);
 

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //[HttpPost]
        //public JsonResult ObtemNota(string chNFe)
        //{
        //    try
        //    {
        //        var uploadPath = Server.MapPath("~/CertTMP/");
        //        Directory.CreateDirectory(uploadPath);
        //        var nomearquivonovo = Guid.NewGuid().ToString();
        //        string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
        //        var cert = Utils.UtilSoftFin.BuscaCert(_estab, _estabobj.senhaCertificado, caminhoArquivo, _estabobj.CNPJ);

        //        var regra = new SoftFin.Sefaz.NFeDownload();
        //        var dto = new DTONfe();

        //        var result = regra.Execute(chNFe,_estabobj.CNPJ, cert,"", "");

        //        if (!result.CMDAceito)
        //            return Json(new { CDStatus = "NOK", DSMessage = "Comando não aceito no Sefaz, verifique os erros", Alertas = result.Alertas, Erros = result.Erros }, JsonRequestBehavior.AllowGet);

        //        var retorno = result.Objs.Select(p => new
        //        {
        //            p.chNFe,
        //            p.CNPJ,
        //            p.cSitNFe,
        //            p.dhEmi,
        //            p.dhRecbto,
        //            p.digVal,
        //            p.IE,
        //            p.nProt,
        //            p.tpNF,
        //            p.vNF,
        //            p.xNome,

        //            ExisteCPAG = (new DocumentoPagarParcela().ObterPorCodigoVerificacao(p.chNFe, _paramBase) != null),
        //            ExistePessoas = (new Pessoa().ObterPorCNPJ(p.CNPJ, _paramBase) != null),
        //            ExisteNF = (new NotaFiscal().ObterPorCodigoVerificacao(0, p.chNFe, _paramBase) != null),
        //        });

        //        return Json(new { CDStatus = "OK", NFSe = retorno }, JsonRequestBehavior.AllowGet);


        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}



        private dynamic TrataRetornoNota(int id, bool copiar)
        {

            var nf = new NotaFiscal();
            var ov = new OrdemVenda();

            var notaFiscalNFEItem = new NotaFiscalNFEItem();
            var notaFiscalNFEItems = new List<NotaFiscalNFEItem>();
            var nfe = new NotaFiscalNFE();
            var tomador = new NotaFiscalPessoa();
            var transp = new NotaFiscalNFETransportadora();
            var retirada = new NotaFiscalNFERetirada();
            var entrega = new NotaFiscalNFEEntrega();
            var listaNFReferenciada = new List<NotaFiscalNFEReferenciada>();
            var listaDuplicatas = new List<NotaFiscalNFEDuplicata>();
            var listaVolumes = new List<NotaFiscalNFEVolume>();
            var listaReboques = new List<NotaFiscalNFEReboque>();
            var listaFormaPagamento = new List<NotaFiscalNFEFormaPagamento>();


            ov.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
            ov.data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
            ov.descricao = "NFe Importada Sefaz";
            ov.estabelecimento_id = _estab;
            ov.itemProdutoServico_ID = new ItemProdutoServico().ObterTodos(_paramBase).First().id;
            ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
            ov.tabelaPreco_ID = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase).First().id;
            ov.usuarioinclusaoid = _usuarioobj.id;
            ov.usuarioAutorizador_id = _usuarioobj.id;
            nf.codigoServico = "NFe";
            nf.discriminacaoServico = "NFe";
            nf.aliquotaINSS = 0;
            nf.valorINSS = 0;
            nf.dataEmissaoRps = SoftFin.Utils.UtilSoftFin.DateBrasilia();
            nf.dataEmissaoNfse = SoftFin.Utils.UtilSoftFin.DateBrasilia();
            nf.dataVencimentoNfse = SoftFin.Utils.UtilSoftFin.DateBrasilia();
            nf.entradaSaida = "S";
            nf.estabelecimento_id = _estab;
            nf.municipio_id = _estabobj.Municipio_id;
            nf.numeroRps = 0;
            nf.operacao_id = new Operacao().ObterTodos(_paramBase).First().id;
            nf.situacaoPrefeitura_id = Models.NotaFiscal.NFGERADAENVIADA;
            nf.situacaoRps = "1";
            nf.tipoRps = 1;
            nf.serieRps = "K";
            nf.usuarioinclusaoid = _usuarioobj.id;
            nfe.dataHoraSaida = SoftFin.Utils.UtilSoftFin.DateBrasilia();
            nfe.situacao = 1;
            nfe.CFOP = "5102";
            nf.tipoNfe = "1";
            nf.banco_id = new Banco().ObterPrincipal(_paramBase).id;
            nf.serieNfe = 1;
            nf.numeroNfe = new NotaFiscal().ObterTodosUltimaNFe();
            nf.loteNfe = nf.numeroNfe;
            transp.indicadorCnpjCpf = 2;
                

            return new
            {
                CDStatus = "OK",
                ov = new
                {
                    ov.id,
                    ov.descricao,
                    ov.valor,
                    data = ov.data.ToString("o"),
                    ov.parcelaContrato_ID,
                    ov.statusParcela_ID,
                    ov.unidadeNegocio_ID,
                    ov.pessoas_ID,
                    ov.estabelecimento_id,
                    ov.usuarioAutorizador_id,
                    dataAutorizacao = (ov.dataAutorizacao == null) ? null : ov.dataAutorizacao.Value.ToString("o"),
                    ov.Numero,
                    ov.itemProdutoServico_ID,
                    ov.tabelaPreco_ID,
                    ov.usuarioinclusaoid,
                    ov.usuarioalteracaoid,
                    dataInclusao = (nf.dataInclusao == null) ? null : nf.dataInclusao.Value.ToString("o"),
                    dataAlteracao = (nf.dataAlteracao == null) ? null : nf.dataAlteracao.Value.ToString("o")
                },
                nf = new
                {
                    nf.id,
                    nf.situacaoPrefeitura_id,
                    nf.estabelecimento_id,
                    nf.ordemVenda_id,
                    banco_id = nf.banco_id.ToString(),
                    nf.operacao_id,
                    nf.tipoRps,
                    nf.serieRps,
                    nf.numeroRps,
                    dataEmissaoRps = nf.dataEmissaoRps.ToString("o"),
                    nf.situacaoRps,
                    nf.numeroNfse,
                    dataEmissaoNfse = nf.dataEmissaoNfse.ToString("o"),
                    dataVencimentoNfse = nf.dataVencimentoNfse.ToString("o"),
                    nf.codigoVerificacao,
                    nf.valorNfse,
                    nf.valorDeducoes,
                    nf.basedeCalculo,
                    nf.aliquotaISS,
                    nf.valorISS,
                    nf.creditoImposto,
                    nf.discriminacaoServico,
                    nf.irrf,
                    nf.pisRetido,
                    nf.cofinsRetida,
                    nf.csllRetida,
                    nf.valorLiquido,
                    nf.aliquotaIrrf,
                    nf.SituacaoRecebimento,
                    nf.entradaSaida,
                    nf.municipio_id,
                    nf.aliquotaINSS,
                    nf.valorINSS,
                    nf.codigoServico,
                    nf.usuarioinclusaoid,
                    nf.usuarioalteracaoid,
                    dataInclusao = (nf.dataInclusao == null) ? null : nf.dataInclusao.Value.ToString("o"),
                    dataAlteracao = (nf.dataAlteracao == null) ? null : nf.dataAlteracao.Value.ToString("o"),
                    nf.notaFiscalTomador_id,
                    nf.notaFiscalPrestador_id,
                    nf.NotaFiscalNFE_id,
                    nf.serieNfe,
                    nf.numeroNfe,
                    nf.loteNfe,
                    nf.tipoNfe,
                },
                nfe = new
                {
                    nfe.id,
                    dataHoraSaida = nfe.dataHoraSaida.ToString("o"),
                    nfe.finalidadeEmissao,
                    nfe.chaveAcesso,
                    nfe.faturaFormaPgto,
                    nfe.faturaNumero,
                    nfe.faturaValorOriginal,
                    nfe.faturaValorDesconto,
                    nfe.faturaValorLiquido,
                    nfe.informacaoComplementar,
                    nfe.informacaoComplementarFisco,
                    nfe.indicadorPresencaComprador,
                    nfe.emailDestinatario,
                    nfe.localEmbarqueExportacao,
                    nfe.ufEmbarqueExportacao,
                    nfe.identificacaoCompradorExtrangeiro,
                    nfe.informacaoPedidoCompra,
                    nfe.informacaoContato,
                    nfe.informacaoNotaEmpenhoCompras,
                    nfe.NotaFiscalNFEEntrega_id,
                    nfe.NotaFiscalNFERetensao_id,
                    nfe.NotaFiscalNFERetirada_id,
                    nfe.NotaFiscalNFETransportadora_id,
                    nfe.situacao,
                    nfe.CFOP,
                    nfe.valor,
                    nfe.baseICMS,
                    nfe.valorICMS,
                    nfe.valorICMSDesonerado,
                    nfe.baseICMSST,
                    nfe.valorICMSST,
                    nfe.valorProduto,
                    nfe.valorFrete,
                    nfe.valorSeguro,
                    nfe.valorDesconto,
                    nfe.valorII,
                    nfe.valorIPI,
                    nfe.valorPIS,
                    nfe.valorCONFINS,
                    nfe.valorOutro,
                    nfe.valorCSLL,

                },
                NotaFiscalNFEItem = new
                {
                    notaFiscalNFEItem.id,
                    notaFiscalNFEItem.produto,
                    notaFiscalNFEItem.operacao,
                    notaFiscalNFEItem.notaFiscal_id,
                    notaFiscalNFEItem.idProduto,
                    notaFiscalNFEItem.operacao_id,
                    notaFiscalNFEItem.quantidade,
                    notaFiscalNFEItem.item,
                    notaFiscalNFEItem.valor,
                    notaFiscalNFEItem.desconto,
                    notaFiscalNFEItem.valorICMS,
                    notaFiscalNFEItem.valorIPI,
                    notaFiscalNFEItem.NCM,
                    notaFiscalNFEItem.CFOP,
                    notaFiscalNFEItem.CSOSN,
                    notaFiscalNFEItem.valorICMSST,
                    notaFiscalNFEItem.valorISS,
                    notaFiscalNFEItem.valorIRRF,
                    notaFiscalNFEItem.valorINSS,
                    notaFiscalNFEItem.valorPIS,
                    notaFiscalNFEItem.valorCOFINS,
                    notaFiscalNFEItem.valorCSLL,
                    notaFiscalNFEItem.aliquotaISS,
                    notaFiscalNFEItem.aliquotaINSS,
                    notaFiscalNFEItem.aliquotaCOFINS,
                    notaFiscalNFEItem.PISRetido,
                    notaFiscalNFEItem.COFINSRetida,
                    notaFiscalNFEItem.CSLLRetida,
                    notaFiscalNFEItem.ICMSSTRetida,
                    notaFiscalNFEItem.ICMSRetida,
                    notaFiscalNFEItem.nomeProduto,
                    notaFiscalNFEItem.codigoProduto,
                    notaFiscalNFEItem.unidadeMedida,
                    notaFiscalNFEItem.EAN,
                    notaFiscalNFEItem.valorUnitario,
                    notaFiscalNFEItem.aliquotaIPI,
                    notaFiscalNFEItem.TabelaPrecoItemProdutoServico_id,
                    notaFiscalNFEItem.origem,
                    notaFiscalNFEItem.CEST,
                    notaFiscalNFEItem.infAdProd,
                    notaFiscalNFEItem.aliquotaPIS,
                    notaFiscalNFEItem.basePIS,
                    notaFiscalNFEItem.baseCOFINS,
                    notaFiscalNFEItem.PISCST,
                    notaFiscalNFEItem.COFINSCST,
                    notaFiscalNFEItem.valorTributos
                },
                NotaFiscalNFEItems = notaFiscalNFEItems.Select(p => new {
                    p.id,
                    p.notaFiscal_id,
                    p.idProduto,
                    p.operacao,
                    p.operacao_id,
                    p.quantidade,
                    p.item,
                    p.valor,
                    p.desconto,
                    p.valorICMS,
                    p.valorIPI,
                    p.NCM,
                    p.CFOP,
                    p.CSOSN,
                    p.valorICMSST,
                    p.valorISS,
                    p.valorIRRF,
                    p.valorINSS,
                    p.valorPIS,
                    p.valorCOFINS,
                    p.valorCSLL,
                    p.aliquotaISS,
                    p.aliquotaINSS,
                    p.PISRetido,
                    p.COFINSRetida,
                    p.CSLLRetida,
                    p.ICMSSTRetida,
                    p.ICMSRetida,
                    p.nomeProduto,
                    p.codigoProduto,
                    p.unidadeMedida,
                    p.EAN,
                    p.aliquotaIPI,
                    p.TabelaPrecoItemProdutoServico_id,
                    p.origem,
                    p.valorUnitario,
                    p.pRedBC,
                    p.CEST,
                    p.infAdProd,
                    p.valorTributos,
                    p.PISCST,
                    p.aliquotaPIS,
                    p.basePIS,
                    p.COFINSCST,
                    p.aliquotaCOFINS,
                    p.baseCOFINS
                }),
                retirada = new
                {
                    retirada.id,
                    retirada.notaFiscal_id,
                    retirada.cnpjCPF,
                    retirada.endereco,
                    retirada.numero,
                    retirada.complemento,
                    retirada.codMunicipio,
                    retirada.indicadorCnpjCpf,
                    retirada.cidade

                },
                entrega = new
                {
                    entrega.id,
                    entrega.cnpjCPF,
                    entrega.endereco,
                    entrega.numero,
                    entrega.complemento,
                    entrega.codMunicipio,
                    entrega.bairro,
                    entrega.indicadorCnpjCpf,
                    entrega.cidade

                },
                transp = new
                {
                    transp.id,
                    transp.nomeRazao,
                    transp.cnpjCPF,
                    transp.cidade,
                    transp.uf,
                    transp.placa,
                    transp.ufplaca,
                    transp.RNTC,
                    transp.baseCalculo,
                    transp.aliquota,
                    transp.valorServico,
                    transp.ICMSRetido,
                    transp.CFOP,
                    transp.modalidadeFrete,
                    transp.IE,
                    transp.EnderecoCompleto,
                    transp.codigoMunicipioOcorrencia,
                    transp.indicadorCnpjCpf

                },
                NotaFiscalPessoaTomador = new
                {
                    tomador.id,
                    tomador.razao,
                    tomador.indicadorCnpjCpf,
                    tomador.cnpjCpf,
                    tomador.inscricaoMunicipal,
                    tomador.inscricaoEstadual,
                    tomador.tipoEndereco,
                    tomador.endereco,
                    tomador.numero,
                    tomador.complemento,
                    tomador.bairro,
                    tomador.cidade,
                    tomador.uf,
                    tomador.cep,
                    tomador.email,
                    tomador.notaFiscalOriginal,
                    tomador.fone
                },
                listaNFReferenciada = listaNFReferenciada.Select(
                    p => new
                    {
                        p.id,
                        p.notaFiscal_id,
                        p.NFe,
                        p.CTe,
                        p.nfserie,
                        p.nfnumero,
                        p.nfmodelo,
                        p.nfuf,
                        p.nfanoMesEmissao,
                        p.nfcnpj,
                        p.nfprodserie,
                        p.nfprodnumero,
                        p.nfprodmodelo,
                        p.nfproduf,
                        p.nfprodanoMesEmissao,
                        p.nfprodcnpjCpf,
                        p.nfprodIE,
                        p.ECF,
                        p.numeroCOO,
                        p.modelo
                    }
                ),
                listaDuplicatas = listaDuplicatas.Select(
                    p => new
                    {
                        p.id,
                        p.notaFiscal_id,
                        p.numero,
                        vencto = p.vencto.ToString("o"),
                        p.valor
                    }
                ),
                listaVolumes = listaVolumes.Select(
                    p => new
                    {
                        p.id,
                        p.notaFiscal_id,
                        p.qtde,
                        p.especie,
                        p.marca,
                        p.numeracao,
                        p.pesoLiquido,
                        p.pesoBruto,
                        p.lacres
                    }
                ),
                listaReboques = listaReboques.Select(
                    p => new
                    {
                        p.id,
                        p.notaFiscal_id,
                        p.placa,
                        p.ufplaca,
                        p.RNTC
                    }
                ),
                listaFormaPagamento = listaFormaPagamento.Select(
                    P => new
                    {
                        P.cAut,
                        P.CNPJ,
                        P.id,
                        P.indPag,
                        P.notaFiscal_id,
                        P.tBand,
                        P.tPag,
                        P.tpIntegra,
                        P.vPag,
                        P.vTroco
                    }
                )


            };

        }

        public JsonResult ListaPessoas()
        {
            var data = new SelectList(new TipoPessoa().ObterTodos(_paramBase), "id", "Descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaCategorias()
        {
            var data = new SelectList(new CategoriaPessoa().ObterTodos(_paramBase), "id", "Descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaUnidades()
        {
            var data = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaTipoEnderecos()
        {
            var data = new SelectList(new TipoEndereco().ObterTodos(_paramBase), "id", "Descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPessoaPorCNPJ(string id)
        {
            var data = new Pessoa().ObterPorCNPJ(id, _paramBase);

            if (data == null)
                data = new Pessoa();


            return Json(new
            {
                data.agenciaConta,
                data.agenciaDigito,
                data.bairro,
                data.bancoConta,
                CategoriaPessoa_ID = (data.CategoriaPessoa_ID == 0) ? "" : data.CategoriaPessoa_ID.ToString(),
                data.ccm,
                data.Celular,
                cep = (data.cep == null) ? "" : data.cep.Replace("-", ""),
                data.cidade,
                cnpj = (data.cnpj == null) ? "" : data.cnpj.Replace(".", "").Replace("/", "").Replace("-", ""),
                data.codigo,
                data.complemento,
                data.contaBancaria,
                data.digitoContaBancaria,
                data.eMail,
                data.empresa_id,
                data.endereco,
                data.id,
                data.inscricao,
                data.nome,
                data.numero,
                data.razao,
                data.TelefoneFixo,
                TipoEndereco_ID = (data.TipoEndereco_ID == 0) ? "" : data.TipoEndereco_ID.ToString(),
                TipoPessoa_ID = (data.TipoPessoa_ID == 0) ? "" : data.TipoPessoa_ID.ToString(),
                data.uf,
                UnidadeNegocio_ID = (data.UnidadeNegocio_ID == 0) ? "" : data.UnidadeNegocio_ID.ToString()

            }
                    , JsonRequestBehavior.AllowGet);
        }


        //public JsonResult ObterCPAGCodigoVerificacao(string Identificador)
        //{
        //    var data = new DocumentoPagarMestre().ObterPorCodigoVerificacao(Identificador);

        //    if (data == null)
        //    {
        //        data = new DocumentoPagarMestre();
        //        data.dataLancamento = DateTime.Now;
        //        data.dataCompetencia = DateTime.Now.ToString("MM/yyyy");
        //        data.dataDocumento = DateTime.Now;
        //        data.tipolancamento = "P";
        //        var bancoid = new Banco().ObterPrincipal();
        //        if (bancoid != null)
        //            data.banco_id = bancoid.id;

        //    }

        //    return Json(new { data.banco_id ,
        //                      data.dataCompetencia,
        //                      dataLancamento = data.dataLancamento.ToString("o"),
        //                      dataDocumento = data.dataDocumento.ToString("o"),
        //                      data.tipolancamento,
        //                      data.CodigoVerificacao,
        //                      data.dataVencimento,
        //                      data.dataVencimentoOriginal,
        //                      data.documentopagaraprovacao_id,
        //                      data.estabelecimento_id,
        //                      data.id,
        //                      data.LinhaDigitavel,
        //                      data.lotePagamentoBanco,
        //                      data.numeroDocumento,
        //                      data.pessoa_id,
        //                      data.planoDeConta_id,
        //                      data.tipodocumento_id,
        //                      data.usuarioAutorizador_id,
        //                      data.valorBruto
        //        }
        //        , JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ObterCPAGCodigoVerificacao(string Identificador)

        {
            DocumentoPagarMestre obj = new DocumentoPagarMestre();
            obj.dataLancamento = DateTime.Now;
            obj.dataCompetencia = DateTime.Now.ToString("MM/yyyy");
            obj.estabelecimento_id = _estab;
            obj.tipolancamento = "P";

            var itens = new List<DocumentoPagarDetalhe>();

            if (Identificador != "")
            {
                var data = new DocumentoPagarParcela().ObterPorCodigoVerificacao(Identificador, _paramBase);
                if (data != null)
                {
                    itens = new DocumentoPagarDetalhe().ObterPorCPAG(data.id);
                }
            }

            return Json(new
            {
                CPAG = new
                {
                    banco_id = obj.banco_id.ToString(),
                    obj.codigoPagamento,
                    obj.CodigoVerificacao,
                    dataAlteracao = (obj.dataAlteracao == null) ? "" : obj.dataAlteracao.Value.ToString("o"),
                    obj.dataCompetencia,
                    dataDocumento = obj.dataDocumento.ToString("o"),
                    dataInclusao = (obj.dataInclusao == null) ? "" : obj.dataInclusao.Value.ToString("o"),
                    dataLancamento = obj.dataLancamento.ToString("o"),
                    //dataPagamanto = (obj.dataPagamanto == null) ? "" : obj.dataPagamanto.Value.ToString("o"),
                    //dataVencimento = obj.dataVencimento.ToString("o"),
                    //dataVencimentoOriginal = obj.dataVencimentoOriginal.ToString("o"),
                    obj.documentopagaraprovacao_id,
                    obj.estabelecimento_id,
                    obj.id,
                    obj.LinhaDigitavel,
                    //obj.lotePagamentoBanco,
                    obj.numeroDocumento,
                    obj.pessoa_id,
                    planoDeConta_id = obj.planoDeConta_id.ToString(),
                    obj.situacaoPagamento,
                    obj.StatusPagamento,
                    tipodocumento_id = obj.tipodocumento_id.ToString(),
                    obj.tipolancamento,
                    obj.usuarioalteracaoid,
                    //obj.usuarioAutorizador_id,
                    obj.usuarioinclusaoid,
                    obj.valorBruto,
                    pessoa_desc = (obj.pessoa_id == 0) ? "" : obj.Pessoa.nome + ", " + obj.Pessoa.cnpj
                },
                CPAGPARCELA = new
                {
                    id = 0,
                    vencimento = DateTime.Now.ToString("o"),
                    vencimentoPrevisto = DateTime.Now.ToString("o"),
                    parcelas = 1,
                    historico = "",
                    sobra = "U"
                },
                CPAGITENS = itens.Select
                (
                    p => new
                    {
                        p.historico,
                        p.estabelecimento_id,
                        p.id,
                        p.unidadenegocio_id,
                        p.valor,
                        UnidadeNegocio_desc = p.UnidadeNegocio.unidade
                    }
                )
            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult ListaTipoLancamento()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "P", Text = "Provisório", Selected = true });
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaContaContabil()
        {
            var pc = new PlanoDeConta().ObterTodosTipoA();
            return Json(pc, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaBanco()
        {
            var con1 = new Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return Json(listret, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaTipoDocumento()
        {
            var data = new SelectList(new TipoDocumento().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorCodigoVerificacao(string Identificador)
        {
            if (Identificador == null)
                return Json(new NotaFiscal(), JsonRequestBehavior.AllowGet);

            var estab = _estabobj;


            var data = new NotaFiscal().ObterPorCodigoVerificacao(estab.Municipio_id, Identificador, _paramBase);
            if (data == null)
            {
                data = new NotaFiscal();
                var estab1 = _estabobj;
                data.tipoRps = 1;
                data.municipio_id = estab1.Municipio_id;
                data.situacaoPrefeitura_id = 3;

                data.NotaFiscalPessoaPrestador = new NotaFiscalPessoa();
                data.NotaFiscalPessoaPrestador.inscricaoEstadual = "000";


                data.tipoRps = 1;
                data.situacaoRps = "1";
                data.entradaSaida = "E";
                data.dataEmissaoNfse = DateTime.Now;
                data.dataEmissaoRps = DateTime.Now;
                data.dataVencimentoNfse = DateTime.Now;
            }


            var returno = new
            {
                data.aliquotaINSS,
                data.aliquotaIrrf,
                data.aliquotaISS,
                banco_id = data.banco_id.ToString(),
                data.basedeCalculo,

                data.codigoServico,
                data.codigoVerificacao,
                data.cofinsRetida,
                data.creditoImposto,
                data.csllRetida,
                dataEmissaoNfse = data.dataEmissaoNfse.ToString("o"),
                dataEmissaoRps = data.dataEmissaoRps.ToString("o"),
                dataVencimentoNfse = data.dataVencimentoNfse.ToString("o"),
                data.discriminacaoServico,
                data.entradaSaida,
                estabelecimento_id = data.estabelecimento_id.ToString(),
                data.id,
                data.irrf,
                municipio_id = data.municipio_id.ToString(),
                data.numeroNfse,
                data.numeroRps,

                data.operacao_id,
                data.ordemVenda_id,
                data.pisRetido,
                data.Recebimentos,
                data.serieRps,
                situacaoPrefeitura_id = data.situacaoPrefeitura_id.ToString(),
                data.SituacaoRecebimento,
                data.situacaoRps,

                data.tipoRps,

                data.valorDeducoes,
                data.valorINSS,
                data.valorISS,
                data.valorLiquido,
                data.valorNfse,
                NotaFiscalPessoaPrestador = new
                {
                    data.NotaFiscalPessoaPrestador.razao,
                    data.NotaFiscalPessoaPrestador.bairro,
                    data.NotaFiscalPessoaPrestador.cep,
                    data.NotaFiscalPessoaPrestador.cidade,
                    data.NotaFiscalPessoaPrestador.cnpjCpf,
                    data.NotaFiscalPessoaPrestador.complemento,
                    data.NotaFiscalPessoaPrestador.indicadorCnpjCpf,
                    data.NotaFiscalPessoaPrestador.inscricaoEstadual,
                    data.NotaFiscalPessoaPrestador.inscricaoMunicipal,
                    data.NotaFiscalPessoaPrestador.email,
                    data.NotaFiscalPessoaPrestador.endereco,
                    data.NotaFiscalPessoaPrestador.numero,
                    data.NotaFiscalPessoaPrestador.tipoEndereco,
                    data.NotaFiscalPessoaPrestador.uf,
                    data.NotaFiscalPessoaPrestador.id
                }

            };

            return Json(returno, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ListaCodigoServico()
        //{
        //    var con1 = new CodigoServicoMunicipio().ObterTodos();
        //    var items = new List<SelectListItem>();
        //    foreach (var item in con1)
        //    {
        //        items.Add(new SelectListItem() { Value = item.codigo.ToString(), Text = String.Format("{0} - {1} ", item.codigo, item.descricao) });
        //    }
        //    var listret = new SelectList(items, "Value", "Text");
        //    return Json(listret, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ListaOperacao()
        {
            var data = new SelectList(new Operacao().ObterTodos(_paramBase), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CalculaNota(int idOperacao, decimal valorNota)
        {
            var nota = new NotaFiscal();

            //Detalhes de Nota
            nota.valorNfse = valorNota;
            var aliquotas = new calculoImposto().ObterTodos(_paramBase).Where(p => p.operacao_id == idOperacao);

            //Cálculo do IRRF
            var aliquotaIRRF = aliquotas.Where(p => p.imposto.codigo.Equals("IRRF")).First().aliquota;
            nota.irrf = valorNota * aliquotaIRRF / 100;
            nota.basedeCalculo = valorNota;
            //Carrega variável quando flag retido estiver marcado
            if (aliquotas.Where(p => p.imposto.codigo.Equals("IRRF")).First().retido == true)
            {
                //Verifica se o valor do IRRF é maior do que R$ 10,00 senão não retem
                if (nota.irrf > 10)
                {
                    var IRRFretido = nota.irrf;
                    nota.valorDeducoes = nota.valorDeducoes + nota.irrf;
                }
                else
                {
                    nota.irrf = 0;
                }
            }

            //Cálculo do ISS
            var aliquotaISS = aliquotas.Where(p => p.imposto.codigo.Equals("ISS")).First().aliquota;
            nota.aliquotaISS = aliquotaISS;
            nota.valorISS = nota.basedeCalculo * aliquotaISS / 100;
            //Carrega variável quando flag retido estiver marcado
            if (aliquotas.Where(p => p.imposto.codigo.Equals("ISS")).First().retido == true)
            {
                var ISSretido = nota.valorISS;
                nota.valorDeducoes = nota.valorDeducoes + nota.valorISS;

            }

            //Cálculo do PIS
            var aliquotaPIS = aliquotas.Where(p => p.imposto.codigo.Equals("PIS")).First().aliquota;
            nota.pisRetido = nota.basedeCalculo * aliquotaPIS / 100;
            //Carrega variável quando flag retido estiver marcado
            if (aliquotas.Where(p => p.imposto.codigo.Equals("PIS")).First().retido == true)
            {
                var PISretido = nota.pisRetido;
                nota.valorDeducoes = nota.valorDeducoes + nota.pisRetido;

            }

            //Cálculo da COFINS
            var aliquotaCONFINS = aliquotas.Where(p => p.imposto.codigo.Equals("COFINS")).First().aliquota;
            nota.cofinsRetida = nota.basedeCalculo * aliquotaCONFINS / 100;
            //Carrega variável quando flag retido estiver marcado
            if (aliquotas.Where(p => p.imposto.codigo.Equals("COFINS")).First().retido == true)
            {
                var COFINSretido = nota.cofinsRetida;
                nota.valorDeducoes = nota.valorDeducoes + nota.cofinsRetida;
            }


            //Cálculo da CSLL
            var aliquotaCSLL = aliquotas.Where(p => p.imposto.codigo.Equals("CSLL")).FirstOrDefault();

            if (aliquotaCSLL != null)
            {
                if (aliquotaCSLL.aliquota != 0)
                {
                    nota.csllRetida = nota.basedeCalculo * aliquotaCSLL.aliquota / 100;
                    //Carrega variável quando flag retido estiver marcado
                    if (aliquotas.Where(p => p.imposto.codigo.Equals("CSLL")).First().retido == true)
                    {
                        var CSLLretido = nota.csllRetida;
                        nota.valorDeducoes = nota.valorDeducoes + nota.csllRetida;
                    }
                }
                else
                {
                    nota.csllRetida = 0;
                }
            }
            else
            {
                nota.csllRetida = 0;
            }

            //Cálculo da CSLL
            var aliquotaINNS = aliquotas.Where(p => p.imposto.codigo.Equals("INSS")).FirstOrDefault();

            if (aliquotaINNS != null)
            {
                nota.aliquotaINSS = aliquotaINNS.aliquota;
                if (aliquotaINNS.aliquota != 0)
                {
                    nota.valorINSS = nota.basedeCalculo * aliquotaINNS.aliquota / 100;
                    //Carrega variável quando flag retido estiver marcado
                    if (aliquotas.Where(p => p.imposto.codigo.Equals("INSS")).First().retido == true)
                    {
                        var INSSretido = nota.valorINSS;
                        nota.valorDeducoes = nota.valorDeducoes + nota.valorINSS.Value;
                    }
                }
                else
                {
                    nota.valorINSS = 0;
                    nota.aliquotaINSS = 0;
                }
            }
            else
            {
                nota.valorINSS = 0;
                nota.aliquotaINSS = 0;
            }


            nota.creditoImposto = 0;

            //Atualiza valor líquido da nota com base nas retenções
            nota.valorLiquido = nota.valorNfse - nota.valorDeducoes;
            nota.aliquotaIrrf = aliquotaIRRF;

            return Json(nota, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Salvar(Pessoa pessoa, NotaFiscal notafiscal)
        {
            try
            {
                var erroscpag = new List<string>();

                var erros = pessoa.Validar(ModelState);

                if (erros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Campos Inválidos", Erros = erros }, JsonRequestBehavior.AllowGet);
                }

                DbControle db = new DbControle();


                var ov = new OrdemVenda();

                ov.data = notafiscal.dataVencimentoNfse;
                ov.dataInclusao = DateTime.Now;
                ov.descricao = "Importação de Nota Fiscal";
                ov.estabelecimento_id = _estab;
                ov.itemProdutoServico_ID = new ItemProdutoServico().ObterTodos(_paramBase).First().id;

                ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
                ov.tabelaPreco_ID = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase).First().id;
                ov.UsuarioInclusao = notafiscal.UsuarioInclusao;
                ov.valor = notafiscal.valorNfse;
                if (pessoa.UnidadeNegocio_ID == null)
                {
                    ov.unidadeNegocio_ID = new UnidadeNegocio().ObterTodos(_paramBase).First().id;
                }
                else
                {
                    ov.unidadeNegocio_ID = pessoa.UnidadeNegocio_ID.Value;
                }

                // Inicio Lançamento Contabil
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

                if (idCredito != 0 && idDebito != 0)
                {
                    ccLC.data = notafiscal.dataVencimentoNfse;
                    ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                    ccLC.estabelecimento_id = _paramBase.estab_id;
                    ccLC.historico = ov.descricao;
                    ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                    ccLC.origemmovimento_id = new OrigemMovimento().TipoFaturamento(_paramBase);
                    ccLC.UnidadeNegocio_ID = ov.unidadeNegocio_ID;

                    ccDebito.contaContabil_id = idDebito;
                    ccDebito.DebitoCredito = "D";
                    ccDebito.valor = ov.valor;

                    ccCredito.contaContabil_id = idCredito;
                    ccCredito.DebitoCredito = "C";
                    ccCredito.valor = ov.valor;
                }
                //Fim Lançamento Contabil



                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    pessoa.empresa_id = _empresa;
                    if (pessoa.id == 0)
                        pessoa.Incluir(_paramBase, db);
                    else
                        pessoa.Alterar(_paramBase, db);

                    var estab1 = _estabobj;



                    int numeroov = 0;
                    ov.pessoas_ID = pessoa.id;
                    ov.Incluir(ov, ref numeroov, _paramBase, db);
                    notafiscal.ordemVenda_id = ov.id;


                    if (notafiscal.id == 0)
                    {
                        notafiscal.estabelecimento_id = _estab;
                        notafiscal.dataEmissaoRps = DateTime.Now;
                        notafiscal.Incluir(_paramBase, db);

                        //Inicio Lançamento Contabil
                        if (idCredito != 0 && idDebito != 0)
                        {
                            var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                            ccLC.codigoLancamento = numeroLcto;
                            ccLC.notafiscal_id = notafiscal.id;

                            ccLC.Incluir(_paramBase, db);
                            ccDebito.lancamentoContabil_id = ccLC.id;
                            ccDebito.Incluir(_paramBase, db);
                            ccCredito.lancamentoContabil_id = ccLC.id;
                            ccCredito.Incluir(_paramBase, db);
                        }
                        //Fim Lançamento Contabil
                    }
                    else
                    {
                        var nfbanco = new NotaFiscal().ObterPorId(notafiscal.id, db);
                        nfbanco.dataVencimentoNfse = notafiscal.dataVencimentoNfse;
                        nfbanco.valorDeducoes = notafiscal.valorDeducoes;
                        nfbanco.valorINSS = notafiscal.valorINSS;
                        nfbanco.valorISS = notafiscal.valorISS;
                        nfbanco.valorLiquido = notafiscal.valorLiquido;
                        nfbanco.valorNfse = notafiscal.valorNfse;
                        nfbanco.aliquotaINSS = notafiscal.aliquotaINSS;
                        nfbanco.aliquotaIrrf = notafiscal.aliquotaIrrf;
                        nfbanco.aliquotaISS = notafiscal.aliquotaISS;
                        nfbanco.banco_id = notafiscal.banco_id;

                        nfbanco.Alterar(_paramBase, db);
                    }
                    //if (documentoPagarMestre == null)
                    //{
                    //    documentoPagarMestre.pessoa_id = pessoa.id;
                    //    documentoPagarMestre.Incluir(documentoPagarMestre, documentoPagarDetalhes, banco);
                    //    dbcxtransaction.Commit();
                    //}
                    dbcxtransaction.Commit();
                }
                return Json(new { CDStatus = "OK", DSMessage = "Nota Importada com sucesso" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult SalvarComCPAG(Pessoa pessoa, NotaFiscal notafiscal, List<DocumentoPagarParcela> cpagParcelas, DocumentoPagarMestre cpag, List<DocumentoPagarDetalhe> cpagItems)
        {
            try
            {
                if (cpag.estabelecimento_id != _estab)
                    return Json(new { CDStatus = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                if (cpagItems == null)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Erro! Informe o detalhe do lançamento" }, JsonRequestBehavior.AllowGet);
                }


                if (cpagItems.Count() == 0)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Erro! Informe o detalhe do lançamento" }, JsonRequestBehavior.AllowGet);
                }
                if (cpag.valorBruto != cpagItems.Sum(p => p.valor))
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Erro! valor bruto diferente da somatoria das parcelas" }, JsonRequestBehavior.AllowGet);
                }

                if (cpagParcelas == null)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe as parcelas" }, JsonRequestBehavior.AllowGet);
                }


                if (cpagParcelas.Count() == 0)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe as parcelas" }, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in cpagParcelas)
                {
                    if (item.vencimentoPrevisto.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de Vencimento Prevista inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }

                    if (item.vencimento.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de Vencimento inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }
                }


                if (cpagParcelas.Sum(p => p.valor) != cpag.valorBruto)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Valor Bruto diferente das parcelas" }, JsonRequestBehavior.AllowGet);
                }

                if (cpag.dataDocumento.Year < 2000)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Data do Documento inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                }

                var documento = new DocumentoPagarMestre();
                if (cpag.id != 0)
                {
                    documento = new DocumentoPagarMestre().ObterPorId(cpag.id, _paramBase);
                    if (documento == null)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Documento não encontrado para a alteração" }, JsonRequestBehavior.AllowGet);
                    }
                    //if (documento.dataPagamanto != null)
                    //{
                    //    return Json(new { CDStatus = "NOK", DSMessage = "Documento não pode sofrer alterações por já estar baixado" }, JsonRequestBehavior.AllowGet);
                    //}
                    if (documento.StatusPagamento == 3)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Documento não pode sofrer alterações por já estar totalmente pago" }, JsonRequestBehavior.AllowGet);
                    }
                }


                var CPAGItens = new List<DocumentoPagarDetalhe>();
                foreach (var item in cpagItems)
                {

                    var unidadeaux = new UnidadeNegocio().ObterTodos(_paramBase).Where(p => p.unidade == item.UnidadeNegocio_desc);
                    if (unidadeaux.Count() == 0)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Unidade não encontrado" }, JsonRequestBehavior.AllowGet);
                    }

                    item.unidadenegocio_id = unidadeaux.First().id;
                    item.estabelecimento_id = _estab;
                }


                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    pessoa.empresa_id = _empresa;
                    if (pessoa.id == 0)
                        pessoa.Incluir(_paramBase, db);
                    else
                        pessoa.Alterar(_paramBase, db);
                    var estab1 = _estabobj;


                    //var ldataVencimento = cpag.dataVencimento;
                    //var ldataVencimentoOriginal = cpag.dataVencimentoOriginal;
                    var ldataDocumento = cpag.dataDocumento;
                    var lnumeroDocumento = cpag.numeroDocumento + 1;
                    DateTime ldataCompetencia;

                    if (cpag.dataCompetencia.Length == 7)
                        ldataCompetencia = DateTime.Parse("01/" + cpag.dataCompetencia);
                    else
                        ldataCompetencia = DateTime.Parse(cpag.dataCompetencia);


                    var ov = new OrdemVenda();

                    ov.data = notafiscal.dataVencimentoNfse;
                    ov.dataInclusao = DateTime.Now;
                    ov.descricao = "Importação de Nota Fiscal Com CPAG";
                    ov.estabelecimento_id = _estab;
                    ov.itemProdutoServico_ID = new ItemProdutoServico().ObterTodos(_paramBase).First().id;
                    ov.pessoas_ID = pessoa.id;
                    ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
                    ov.tabelaPreco_ID = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase).First().id;
                    ov.UsuarioInclusao = notafiscal.UsuarioInclusao;
                    ov.valor = notafiscal.valorNfse;
                    if (pessoa.UnidadeNegocio_ID == null)
                    {
                        ov.unidadeNegocio_ID = new UnidadeNegocio().ObterTodos(_paramBase).First().id;
                    }
                    else
                    {
                        ov.unidadeNegocio_ID = pessoa.UnidadeNegocio_ID.Value;
                    }
                    int numeroov = 0;
                    ov.Incluir(ov, ref numeroov, _paramBase, db);
                    notafiscal.ordemVenda_id = ov.id;

                    // Inicio Lançamento Contabil
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

                    if (idCredito != 0 && idDebito != 0)
                    {
                        ccLC.data = notafiscal.dataVencimentoNfse;
                        ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                        ccLC.estabelecimento_id = _paramBase.estab_id;
                        ccLC.historico = ov.descricao;
                        ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                        ccLC.origemmovimento_id = new OrigemMovimento().TipoFaturamento(_paramBase);
                        ccLC.UnidadeNegocio_ID = ov.unidadeNegocio_ID;

                        ccDebito.contaContabil_id = idDebito;
                        ccDebito.DebitoCredito = "D";
                        ccDebito.valor = ov.valor;

                        ccCredito.contaContabil_id = idCredito;
                        ccCredito.DebitoCredito = "C";
                        ccCredito.valor = ov.valor;
                    }
                    //Fim Lançamento Contabil


                    if (notafiscal.id == 0)
                    {
                        notafiscal.estabelecimento_id = _estab;
                        notafiscal.dataEmissaoRps = DateTime.Now;
                        notafiscal.Incluir(_paramBase, db);

                        //Inicio Lançamento Contabil
                        if (idCredito != 0 && idDebito != 0)
                        {
                            var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                            ccLC.codigoLancamento = numeroLcto;
                            ccLC.notafiscal_id = notafiscal.id;

                            ccLC.Incluir(_paramBase, db);

                            ccDebito.lancamentoContabil_id = ccLC.id;
                            ccCredito.lancamentoContabil_id = ccLC.id;
                            ccDebito.Incluir(_paramBase, db);
                            ccCredito.Incluir(_paramBase, db);
                        }
                        //Fim Lançamento Contabil
                    }
                    else
                    {
                        var nfbanco = new NotaFiscal().ObterPorId(notafiscal.id, db);
                        nfbanco.dataVencimentoNfse = notafiscal.dataVencimentoNfse;
                        nfbanco.valorDeducoes = notafiscal.valorDeducoes;
                        nfbanco.valorINSS = notafiscal.valorINSS;
                        nfbanco.valorISS = notafiscal.valorISS;
                        nfbanco.valorLiquido = notafiscal.valorLiquido;
                        nfbanco.valorNfse = notafiscal.valorNfse;
                        nfbanco.aliquotaINSS = notafiscal.aliquotaINSS;
                        nfbanco.aliquotaIrrf = notafiscal.aliquotaIrrf;
                        nfbanco.aliquotaISS = notafiscal.aliquotaISS;
                        nfbanco.banco_id = notafiscal.banco_id;

                        nfbanco.Alterar(_paramBase, db);
                    }

                    if (cpag.id == 0)
                    {
                        var DocumentoPagarMestreInclusao = new DocumentoPagarMestre();
                        new CPAGController().ConvertDocumento(cpag, DocumentoPagarMestreInclusao, pessoa);
                        DocumentoPagarMestreInclusao.StatusPagamento = 1;
                        DocumentoPagarMestreInclusao.situacaoPagamento = "A";
                        //DocumentoPagarMestreInclusao.dataVencimento = ldataVencimento;
                        //DocumentoPagarMestreInclusao.dataVencimentoOriginal = ldataVencimentoOriginal;
                        DocumentoPagarMestreInclusao.dataDocumento = ldataDocumento;
                        DocumentoPagarMestreInclusao.numeroDocumento = lnumeroDocumento;
                        DocumentoPagarMestreInclusao.id = 0;
                        DocumentoPagarMestreInclusao.dataCompetencia = ldataCompetencia.ToString("MM/yyyy");
                        DocumentoPagarMestreInclusao.qtdParcelas = cpagParcelas.Count();
                        new CPAGController().InclurCPAG(DocumentoPagarMestreInclusao, cpagItems, cpagParcelas, null, null, null,null, db);
                    }

                    dbcxtransaction.Commit();
                }
                return Json(new { CDStatus = "OK", DSMessage = "Nota Importada com sucesso" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
