using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.NFe.DTO;
using SoftFin.NFSe.DTO;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class NFSeImportacaoController : BaseController
    {
        //
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
        public JsonResult ListaNotas(string instituicao, DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                var uploadPath = Server.MapPath("~/CertTMP/");
                Directory.CreateDirectory(uploadPath);
                var nomearquivonovo = Guid.NewGuid().ToString();
                string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
                var cert = UtilSoftFin.BuscaCert(_estab, _estabobj.senhaCertificado, caminhoArquivo, _estabobj.CNPJ);


             
                    var regra = new SoftFin.NFSe.Business.SFConsultaNFSeRecebidas();
                    var result = regra.Execute(new DTONotaFiscal
                    {
                        municipio_desc = _estabobj.Municipio.DESC_MUNICIPIO,
                        Cabecalho = new tpCabecalho
                        {
                            CPFCNPJRemetente = new tpCPFCNPJ { CNPJ = UtilSoftFin.Limpastrings(_estabobj.CNPJ) },
                            Inscricao = _estabobj.InscricaoMunicipal.ToString(),
                            dtInicio = dataInicial.ToString("yyyy-MM-dd"),
                            dtFim = dataFinal.ToString("yyyy-MM-dd"),
                        }
                    }, cert);

                    if (result.Cabecalho.Sucesso.ToLower() == "false")
                        return Json(new { CDStatus = "NOK", DSMessage = "Comando não aceito na prefeitura, verifique os erros", Alertas = result.Alerta, Erros = result.Erro, NFes = result.NFe }, JsonRequestBehavior.AllowGet);


                    var retorno = result.NFe.Select(p => new
                    {
                        p.AliquotaServicos,
                        p.Assinatura,
                        p.ChaveNFe,
                        p.ChaveRPS,
                        p.CPFCNPJPrestador,
                        p.CPFCNPJTomador,
                        DataEmissaoNFe = (string.IsNullOrWhiteSpace(p.DataEmissaoNFe)) ? DateTime.Now.ToString("o") : DateTime.Parse(p.DataEmissaoNFe).ToString("o"),
                        DataEmissaoRPS = DateTime.Now.ToString("o"),
                        p.Discriminacao,
                        p.EmailTomador,
                        p.EnderecoPrestador,
                        p.EnderecoTomador,
                        p.FonteCargaTributaria,
                        p.ISSRetido,
                        p.NumeroLote,
                        p.OpcaoSimples,
                        p.RazaoSocialPrestador,
                        p.StatusNFe,
                        p.TipoRPS,
                        p.TributacaoNFe,
                        p.ValorCOFINS,
                        p.ValorCredito,
                        p.ValorCSLL,
                        p.ValorDeducoes,
                        p.ValorINSS,
                        p.ValorIR,
                        p.ValorISS,
                        p.ValorPIS,
                        p.CodigoServico,
                        ValorNFe = "0",
                        ValorServicos = decimal.Parse(p.ValorServicos.Replace(".", ",")),
                        ExisteCPAG = (new DocumentoPagarParcela().ObterPorCodigoVerificacao(p.ChaveNFe.CodigoVerificacao, _paramBase) != null),
                        ExistePessoas = (new Pessoa().ObterPorCNPJ(p.CPFCNPJPrestador.CNPJ, _paramBase) != null),
                        ExisteNF = (new NotaFiscal().ObterPorCodigoVerificacao(_estabobj.Municipio_id, p.ChaveNFe.CodigoVerificacao, _paramBase) != null),
                    });

                    return Json(new { CDStatus = "OK", NFSe = retorno }, JsonRequestBehavior.AllowGet);
          
                
                
                
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
            

            return Json(new  {   data.agenciaConta,
                                data.agenciaDigito,
                                data.bairro,
                                data.bancoConta,
                                 CategoriaPessoa_ID = (data.CategoriaPessoa_ID == 0) ? "" : data.CategoriaPessoa_ID.ToString(),
                                data.ccm,
                                data.Celular,
                                 cep = (data.cep == null)? "" : data.cep.Replace("-", ""),
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
                
                if (erros.Count() > 0 )
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Campos Inválidos", Erros = erros}, JsonRequestBehavior.AllowGet);
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
                        pessoa.Incluir(_paramBase,db);
                    else
                        pessoa.Alterar(_paramBase,db);

                    var estab1 = _estabobj;



                    int numeroov = 0;
                    ov.pessoas_ID = pessoa.id;
                    ov.Incluir(ov, ref numeroov, _paramBase, db);
                    notafiscal.ordemVenda_id = ov.id;


                    if (notafiscal.id == 0)
                    {
                        notafiscal.estabelecimento_id = _estab;
                        notafiscal.dataEmissaoRps = DateTime.Now;
                        notafiscal.DataVencimentoOriginal = notafiscal.dataVencimentoNfse;
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
            catch(Exception ex)
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


                    var pcf = new PessoaContaContabil().ObterPorPessoa(pessoa.id,db);

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
                        notafiscal.DataVencimentoOriginal = notafiscal.dataVencimentoNfse;
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
