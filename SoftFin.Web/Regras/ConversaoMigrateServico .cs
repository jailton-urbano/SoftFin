using SoftFin.Web.Models;
using SoftFin.NFSe.DTO;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Migrate.NFSe.DTO;
using System.Xml;
using SoftFin.Migrate.NFSe.DTO.RetornoConsulta;
using System.Xml.Serialization;

namespace SoftFin.Web.Regras
{
    public class ConversaoMigrateServico
    {
        internal static DTORetornoNFEs RetornoMigrateToRetornoConsultaDto(Migrate.NFSe.DTO.RetornoConsulta.Envelope resultadoMigrate)
        {
            var dTORetornoNFEs = new DTORetornoNFEs();
            dTORetornoNFEs.tipo = "Envio";
            dTORetornoNFEs.xml = resultadoMigrate.xml;


            var codigoResultado = resultadoMigrate.Body.recepcaoExecuteResponse.Invoicyretorno.Mensagem.MensagemItem.Codigo;
            if (codigoResultado.Equals("100"))
            {
                dTORetornoNFEs.Cabecalho.Sucesso = "True";

                var xmlretornoPrefeitura = resultadoMigrate.Body.recepcaoExecuteResponse.Invoicyretorno.Mensagem.MensagemItem.Documentos.DocumentosItem.Documento.ToString();
                var xmlretornoPrefeituraConvertido = LoadFromXMLString(xmlretornoPrefeitura);

                
                dTORetornoNFEs.NFe.Add(new tpNFSe { DataEmissao = DateTime.Now.ToString() });
                dTORetornoNFEs.NFe.First().ChaveNFe.NumeroNFe = xmlretornoPrefeituraConvertido.NFSe.NFSeNumero;
                dTORetornoNFEs.NFe.First().ChaveNFe.CodigoVerificacao = xmlretornoPrefeituraConvertido.NFSe.NFSeCodVerificacao;


                if (xmlretornoPrefeituraConvertido.NFSe.NFSeNumero == "0")
                {


                    var documentoRetorno = LoadFromXMLStringReturnEvents(xmlretornoPrefeitura);

                    

                    dTORetornoNFEs.Cabecalho.Sucesso = "False";

                    dTORetornoNFEs.Erro.Add(new TPErro
                    {
                        Codigo = documentoRetorno.DocSitCodigo,
                        Descricao = documentoRetorno.DocSitDescricao
                    });
                }

            }

            else
            {
                dTORetornoNFEs.Cabecalho.Sucesso = "False";
                dTORetornoNFEs.Erro.Add(new TPErro
                {
                    Codigo = codigoResultado.ToString(),
                    Descricao = resultadoMigrate.Body.recepcaoExecuteResponse.Invoicyretorno.Mensagem.MensagemItem.Descricao
                });

            }




            return dTORetornoNFEs;

        }

        public static Migrate.NFSe.DTO.RetornoValidacao.Documento LoadFromXMLStringReturnEvents(string xmlText)
        {
            RetornoValidacao.Documento documento = new RetornoValidacao.Documento();

            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Migrate.NFSe.DTO.RetornoValidacao.Documento));
            return serializer.Deserialize(stringReader) as Migrate.NFSe.DTO.RetornoValidacao.Documento;
        }

        internal Consulta ConverterDTOMigrateConsulta(Estabelecimento estabelecimento, NotaFiscal objNF)
        {
            var consulta = new Migrate.NFSe.DTO.Consulta();
            consulta.ModeloDocumento = "NFSE";
            consulta.Versao = "1.0";
            consulta.tpAmb = "2";
            consulta.CnpjEmissor = SoftFin.Utils.UtilSoftFin.Limpastrings(estabelecimento.CNPJ);
            consulta.NumeroInicial = objNF.numeroRps.ToString();
            consulta.NumeroFinal = objNF.numeroRps.ToString();
            consulta.Serie = objNF.serieRps;
            return consulta;

        }

        public SoftFin.Migrate.NFSe.DTO.Envio ConverterDTOMigrateEnvio(
            Estabelecimento objEstab,
            NotaFiscal objNF, 
            ParamBase pb)
        {
            SoftFin.Migrate.NFSe.DTO.Envio envioMigrate = new Migrate.NFSe.DTO.Envio();
            envioMigrate.ModeloDocumento = "NFSe";
            envioMigrate.Versao = "1.0";

            // RPS
            envioMigrate.RPS = new EnvioRPS();
            envioMigrate.RPS.RPSNumero = objNF.numeroRps.ToString();
            envioMigrate.RPS.RPSSerie = objNF.serieRps;
            envioMigrate.RPS.RPSTipo = "1";//byte.Parse(objNF.Operacao.tipoRPS.codigo);
            envioMigrate.RPS.dEmis = objNF.dataEmissaoRps.ToString("o");
            envioMigrate.RPS.dCompetencia = objNF.dataEmissaoRps.ToString("o");

            if (objNF.LocalPrestServ == null)
                throw new Exception("Local de Prestação de serviço não encontrado.");

            envioMigrate.RPS.LocalPrestServ = objNF.LocalPrestServ;// "1";//objNF.Operacao.situacaoTributariaNota.CodeMigrate;

            string codigoServico = null;
            string codigoServicoComplemento = null;

            if  (string.IsNullOrWhiteSpace(objNF.codigoServico))
                throw new Exception("Código de serviço não encontrado.");

            if (objNF.codigoServico.Contains("/"))
            {
                codigoServico = objNF.codigoServico.Split('/')[0].Trim();
                codigoServicoComplemento = objNF.codigoServico.Split('/')[1].Trim();
            }
            else
            {
                codigoServico = objNF.codigoServico;
            }

            envioMigrate.RPS.natOp = objNF.Operacao.situacaoTributariaNota.CodeMigrate;
            envioMigrate.RPS.Operacao = null;
            envioMigrate.RPS.NumProcesso = null;
            envioMigrate.RPS.RegEspTrib = objEstab.opcaoTributariaSimples.codigo;

            var temsimples = objEstab.opcaoTributariaSimples.descricao;

            if (temsimples.Contains("Não-Optante"))
                envioMigrate.RPS.OptSN = "2";
            else
                envioMigrate.RPS.OptSN = "1";

            envioMigrate.RPS.IncCult = null;
            envioMigrate.RPS.Status = "1";
            envioMigrate.RPS.cVerificaRPS = null;
            envioMigrate.RPS.RPSSubs = null;
            envioMigrate.RPS.RPSCanhoto = null;
            envioMigrate.RPS.Arquivo = null;
            envioMigrate.RPS.EmpreitadaGlobal = null;
            envioMigrate.RPS.tpAmb = "2";
          
            


            //Transpórtadora
            envioMigrate.RPS.Prestador = new EnvioRPSPrestador();
            envioMigrate.RPS.Transportadora = new EnvioRPSTransportadora();
            envioMigrate.RPS.Transportadora.TraCPFCNPJ = null;
            envioMigrate.RPS.Transportadora.TraEnd = null;
            envioMigrate.RPS.Transportadora.TraIE = null;
            envioMigrate.RPS.Transportadora.TraMun = null;
            envioMigrate.RPS.Transportadora.TraNome = null;
            envioMigrate.RPS.Transportadora.TraPais = null;
            envioMigrate.RPS.Transportadora.TraPlaca = null;
            envioMigrate.RPS.Transportadora.TraTipoFrete = null;
            envioMigrate.RPS.Transportadora.TraUF = null;

            //Prestador
            envioMigrate.RPS.Prestador.CMC = objEstab.InscricaoMunicipal.ToString();
            var cpfcnpf = UtilSoftFin.Limpastrings(objEstab.CNPJ);

            if (cpfcnpf.Length == 14)
            {
                envioMigrate.RPS.Prestador.CNPJ_prest = cpfcnpf;
            }
            else
            {
                envioMigrate.RPS.Prestador.CPF_prest = cpfcnpf;
            }
            envioMigrate.RPS.Prestador.enderPrest = new EnvioRPSPrestadorEnderPrest();
            envioMigrate.RPS.Prestador.enderPrest.CEP = objEstab.CEP.Replace("-", "");
            envioMigrate.RPS.Prestador.enderPrest.cMun = objEstab.Municipio.codigoIBGE.ToString();
            envioMigrate.RPS.Prestador.enderPrest.Email = objEstab.emailNotificacoes;
            envioMigrate.RPS.Prestador.enderPrest.fone = objEstab.Fone;
            envioMigrate.RPS.Prestador.enderPrest.nro = objEstab.NumeroLogradouro.ToString();
            envioMigrate.RPS.Prestador.enderPrest.TPEnd = "Comercial";
            envioMigrate.RPS.Prestador.enderPrest.UF = objEstab.UF;
            envioMigrate.RPS.Prestador.enderPrest.xBairro = objEstab.BAIRRO;
            envioMigrate.RPS.Prestador.enderPrest.xCpl = objEstab.Complemento;
            envioMigrate.RPS.Prestador.enderPrest.xLgr = objEstab.Logradouro;
            envioMigrate.RPS.Prestador.IE = objEstab.InscricaoEstadual.ToString();
            envioMigrate.RPS.Prestador.IM = objEstab.InscricaoMunicipal.ToString();
            envioMigrate.RPS.Prestador.xFant = objEstab.NomeCompleto;
            envioMigrate.RPS.Prestador.xNome = objEstab.NomeCompleto;


            //Tomador
            envioMigrate.RPS.Tomador = new EnvioRPSTomador();
            envioMigrate.RPS.Tomador.DocTomadorEstrangeiro = null;
            envioMigrate.RPS.Tomador.TomaBairro = objNF.NotaFiscalPessoaTomador.bairro;
            envioMigrate.RPS.Tomador.TomaCEP = objNF.NotaFiscalPessoaTomador.cep.Replace("-", "");


            if (objNF.NotaFiscalPessoaTomador.cnpjCpf.Length == 14)
                envioMigrate.RPS.Tomador.TomaCNPJ = objNF.NotaFiscalPessoaTomador.cnpjCpf;
            else
                envioMigrate.RPS.Tomador.TomaCPF = objNF.NotaFiscalPessoaTomador.cnpjCpf;

            var xmunicPrestacao = "";
            var cmunicPrestacao = "";
            switch (envioMigrate.RPS.LocalPrestServ)
            {
                case "1":
                case "2":
                    {
                        var ibgemunic = new Municipio().ObterPorNome(objNF.NotaFiscalPessoaTomador.cidade, null).FirstOrDefault();
                        if (ibgemunic != null)
                            envioMigrate.RPS.Tomador.TomacMun = ibgemunic.codigoIBGE;
                        else
                            throw new Exception("Municipío Tomador e código IBGE não encontrados");

                        cmunicPrestacao = objEstab.Municipio.codigoIBGE;
                        xmunicPrestacao = objEstab.Municipio.DESC_MUNICIPIO;
                    }
                    break;


                case "3":
                case "4":
                case "5":
                    {
                        var ibgemunic = new Municipio().ObterPorNome(objNF.NotaFiscalPessoaTomador.cidade, null).FirstOrDefault();
                        if (ibgemunic != null)
                            envioMigrate.RPS.Tomador.TomacMun = ibgemunic.codigoIBGE;
                        else
                            throw new Exception("Municipío Tomador e código IBGE não encontrados");

                        cmunicPrestacao = ibgemunic.codigoIBGE;
                        xmunicPrestacao = ibgemunic.DESC_MUNICIPIO;
                    }
                    break;
                default:
                    break;
            }
            
            envioMigrate.RPS.Tomador.TomaComplemento = objNF.NotaFiscalPessoaTomador.complemento;
            envioMigrate.RPS.Tomador.TomaEmail = objNF.NotaFiscalPessoaTomador.email;
            envioMigrate.RPS.Tomador.TomaEndereco = objNF.NotaFiscalPessoaTomador.endereco;
            envioMigrate.RPS.Tomador.TomaIE = objNF.NotaFiscalPessoaTomador.inscricaoEstadual;
            envioMigrate.RPS.Tomador.TomaIM = objNF.NotaFiscalPessoaTomador.inscricaoMunicipal;
            envioMigrate.RPS.Tomador.TomaIME = null;
            envioMigrate.RPS.Tomador.TomaNumero = objNF.NotaFiscalPessoaTomador.numero;
            envioMigrate.RPS.Tomador.TomaPais = "Brasil";
            envioMigrate.RPS.Tomador.TomaRazaoSocial = objNF.NotaFiscalPessoaTomador.razao.Replace("&","E");
            envioMigrate.RPS.Tomador.TomaSite = null;
            envioMigrate.RPS.Tomador.TomaSituacaoEspecial = null;
            if (objNF.NotaFiscalPessoaTomador.fone == null)
                envioMigrate.RPS.Tomador.TomaTelefone = "1139001054";
            else
                envioMigrate.RPS.Tomador.TomaTelefone = objNF.NotaFiscalPessoaTomador.fone;
            envioMigrate.RPS.Tomador.TomaTipoTelefone = null;
            envioMigrate.RPS.Tomador.TomatpLgr = null;
            envioMigrate.RPS.Tomador.TomaUF = objNF.NotaFiscalPessoaTomador.uf;
            envioMigrate.RPS.Tomador.TomaxMun = objNF.NotaFiscalPessoaTomador.cidade;
            

            // Serviço
            envioMigrate.RPS.Servico = new EnvioRPSServico();
            envioMigrate.RPS.Servico.TributMunicipio = codigoServico;
            envioMigrate.RPS.IncCult = "2";
            envioMigrate.RPS.Servico.cMun = cmunicPrestacao;
            envioMigrate.RPS.Servico.cMunIncidencia = cmunicPrestacao;
            envioMigrate.RPS.Servico.Cnae = null;
            envioMigrate.RPS.Servico.CodigoAtividadeEconomica = null;
            envioMigrate.RPS.Servico.Discriminacao = objNF.discriminacaoServico.Replace("\n", "|").Replace("\r", "");
            envioMigrate.RPS.Servico.dVencimento = objNF.DataVencimentoOriginal.ToString("o") ;
            envioMigrate.RPS.Servico.fPagamento = null;

            
            envioMigrate.RPS.Servico.IteListServico = codigoServicoComplemento;

            envioMigrate.RPS.Servico.Valores = new EnvioRPSServicoValores();
            envioMigrate.RPS.Servico.Valores.ValServicos = FormataNumero(objNF.valorNfse);
            envioMigrate.RPS.Servico.Valores.ValDeducoes = FormataNumero(objNF.valorDeducoes);
            envioMigrate.RPS.Servico.Valores.ValPIS = FormataNumero(objNF.pisRetido);
            envioMigrate.RPS.Servico.Valores.ValAliqPIS = FormataNumero(objNF.ValAliqPIS);
            envioMigrate.RPS.Servico.Valores.ValAliqCSLL = FormataNumero(objNF.ValAliqCSLL);
            envioMigrate.RPS.Servico.Valores.ValAliqCOFINS = FormataNumero(objNF.ValAliqCOFINS);
            envioMigrate.RPS.Servico.Valores.ValCOFINS = FormataNumero(objNF.cofinsRetida);
            envioMigrate.RPS.Servico.Valores.ValINSS = FormataNumero(objNF.valorINSS);
            envioMigrate.RPS.Servico.Valores.ValCSLL = FormataNumero(objNF.csllRetida);
            envioMigrate.RPS.Servico.Valores.ValISS = FormataNumero(objNF.valorISS);
            envioMigrate.RPS.Servico.Valores.ValIR = FormataNumero(objNF.irrf);
            envioMigrate.RPS.Servico.Valores.ValAliqIR = FormataNumero(objNF.aliquotaIrrf);
            envioMigrate.RPS.Servico.Valores.ISSRetido = (objNF.ValISSRetido != 0) ? "1" : "2";
            if (objNF.ValISSRetido != 0)
            {
                
                envioMigrate.RPS.Servico.Valores.ValISSRetido = FormataNumero(objNF.ValISSRetido);
                envioMigrate.RPS.Servico.Valores.RespRetencao = objNF.RespRetencao;
                envioMigrate.RPS.Servico.Valores.ValAliqISSRetido = FormataNumero(objNF.ValAliqISSRetido);
                
            }

            envioMigrate.RPS.Servico.Valores.ValTotal = FormataNumero(objNF.valorNfse - objNF.valorDeducoes);
            envioMigrate.RPS.Servico.Valores.ValBaseCalculo = FormataNumero(objNF.basedeCalculo); 
            envioMigrate.RPS.Servico.Valores.ValAliqISS = FormataNumeroTresCasas(objNF.aliquotaISS / 100);
            envioMigrate.RPS.Servico.Valores.ValLiquido = FormataNumero(objNF.valorNfse - 
                objNF.pisRetido - 
                objNF.cofinsRetida - 
                objNF.valorINSS.Value - 
                objNF.irrf - 
                objNF.csllRetida - 
                objNF.valorISS - 
                objNF.ValISSRetido);

            //Local Prestação
            envioMigrate.RPS.Servico.LocalPrestacao = new EnvioRPSServicoLocalPrestacao();
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndBairro = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndCep = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndcMun = cmunicPrestacao;
            
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndComplemento = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndLgr = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndNumero = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndSiglaUF = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndTpLgr = null;
            envioMigrate.RPS.Servico.LocalPrestacao.SerEndxMun = xmunicPrestacao; ;

            //Intermediario de serviço
            envioMigrate.RPS.IntermServico = new EnvioRPSIntermServico();
            if (objNF.NotaFiscalIntermediario != null)
            {
                envioMigrate.RPS.IntermServico.IntermBairro = objNF.NotaFiscalIntermediario.bairro;
                envioMigrate.RPS.IntermServico.IntermCep = objNF.NotaFiscalIntermediario.cep.Replace("-", "");
                envioMigrate.RPS.IntermServico.IntermCmun = objNF.municipio.codigoIBGE;
                envioMigrate.RPS.Tomador.TomacMun = objNF.municipio.codigoIBGE;
                if (objNF.NotaFiscalPessoaTomador.cnpjCpf.Length == 14)
                    envioMigrate.RPS.IntermServico.IntermCNPJ = objNF.NotaFiscalIntermediario.cnpjCpf;
                else
                    envioMigrate.RPS.IntermServico.IntermCPF = objNF.NotaFiscalIntermediario.cnpjCpf;


                envioMigrate.RPS.IntermServico.IntermComplemento = objNF.NotaFiscalIntermediario.complemento;
                envioMigrate.RPS.IntermServico.IntermEmail = objNF.NotaFiscalIntermediario.email;
                envioMigrate.RPS.IntermServico.IntermEndereco = objNF.NotaFiscalIntermediario.endereco;
                envioMigrate.RPS.IntermServico.IntermFone = objNF.NotaFiscalIntermediario.fone;
                envioMigrate.RPS.IntermServico.IntermIM = objNF.NotaFiscalIntermediario.inscricaoMunicipal;
                envioMigrate.RPS.IntermServico.IntermNumero = objNF.NotaFiscalIntermediario.numero;
                envioMigrate.RPS.IntermServico.IntermRazaoSocial = objNF.NotaFiscalIntermediario.razao;
                envioMigrate.RPS.IntermServico.IntermXmun = null;
                envioMigrate.RPS.IntermServico.ItermIE = objNF.NotaFiscalIntermediario.inscricaoEstadual;
            }

            //Lista Decucao
            envioMigrate.RPS.ListaDed = new EnvioRPSListaDed();
            envioMigrate.RPS.ListaDed.Ded = new EnvioRPSListaDedDed();  
            envioMigrate.RPS.ListaDed.Ded.DedCNPJRef = null;
            envioMigrate.RPS.ListaDed.Ded.DedCPFRef = null;
            envioMigrate.RPS.ListaDed.Ded.DedDescricao = null;
            envioMigrate.RPS.ListaDed.Ded.DednNFRef = null;
            envioMigrate.RPS.ListaDed.Ded.DedPer = null;
            envioMigrate.RPS.ListaDed.Ded.DedQtde = null;
            envioMigrate.RPS.ListaDed.Ded.DedSeq = null;
            envioMigrate.RPS.ListaDed.Ded.DedTipo = "";
            envioMigrate.RPS.ListaDed.Ded.DedValor = null;
            envioMigrate.RPS.ListaDed.Ded.DedValPer = "";
            envioMigrate.RPS.ListaDed.Ded.DedValUnit = null;
            envioMigrate.RPS.ListaDed.Ded.DedvlTotRef = null;


            //Item
            envioMigrate.RPS.ListaItens = new EnvioRPSListaItens();
            envioMigrate.RPS.ListaItens.Item = new EnvioRPSListaItensItem();
            envioMigrate.RPS.ListaItens.Item.ItemSeq = "1";
            envioMigrate.RPS.ListaItens.Item.ItemAliquota = null;
            envioMigrate.RPS.ListaItens.Item.ItemBaseCalculo = null;
            envioMigrate.RPS.ListaItens.Item.ItemBCRetido = null;
            envioMigrate.RPS.ListaItens.Item.ItemcCnae = null;
            envioMigrate.RPS.ListaItens.Item.ItemcMunIncidencia = cmunicPrestacao;
            
            envioMigrate.RPS.ListaItens.Item.ItemCod = null;
            envioMigrate.RPS.ListaItens.Item.ItemDedCNPJRef = null;
            envioMigrate.RPS.ListaItens.Item.ItemDedCPFRef = null;
            envioMigrate.RPS.ListaItens.Item.ItemDedPer = null;
            envioMigrate.RPS.ListaItens.Item.ItemDedTipo = null;
            envioMigrate.RPS.ListaItens.Item.ItemDedvlTotRef = null;
            envioMigrate.RPS.ListaItens.Item.ItemDesc = null;
            envioMigrate.RPS.ListaItens.Item.ItemExigibilidadeISS = null;
            envioMigrate.RPS.ListaItens.Item.ItemIssRetido = null;
            envioMigrate.RPS.ListaItens.Item.ItemIteListServico = null;
            envioMigrate.RPS.ListaItens.Item.ItemJustDed = null;
            envioMigrate.RPS.ListaItens.Item.ItemnAlvara = null;
            envioMigrate.RPS.ListaItens.Item.ItemNumProcesso = null;
            envioMigrate.RPS.ListaItens.Item.ItemPaisImpDevido = null;
            envioMigrate.RPS.ListaItens.Item.ItemQtde = null;
            envioMigrate.RPS.ListaItens.Item.ItemRedBCRetido = null;
            envioMigrate.RPS.ListaItens.Item.ItemTributavel = "S";
            envioMigrate.RPS.ListaItens.Item.ItemTributMunicipio = codigoServico;
            envioMigrate.RPS.ListaItens.Item.ItemuMed = null;

            envioMigrate.RPS.ListaItens.Item.ItemValAliqCOFINS = FormataNumero(objNF.ValAliqPIS);
            envioMigrate.RPS.ListaItens.Item.ItemValAliqCSLL = FormataNumero(objNF.ValAliqCSLL);
            envioMigrate.RPS.ListaItens.Item.ItemValAliqINSS = FormataNumero(objNF.aliquotaINSS);
            envioMigrate.RPS.ListaItens.Item.ItemValAliqIR = FormataNumero(objNF.aliquotaIrrf);
            envioMigrate.RPS.ListaItens.Item.ItemValAliqISSRetido = FormataNumero(objNF.ValAliqISSRetido);
            envioMigrate.RPS.ListaItens.Item.ItemValAliqPIS = FormataNumero(objNF.ValAliqPIS); ;

            envioMigrate.RPS.ListaItens.Item.ItemValCOFINS = FormataNumero(objNF.cofinsRetida);
            envioMigrate.RPS.ListaItens.Item.ItemValCSLL = FormataNumero(objNF.csllRetida);
            envioMigrate.RPS.ListaItens.Item.ItemValINSS = FormataNumero(objNF.valorINSS);
            envioMigrate.RPS.ListaItens.Item.ItemValIR = FormataNumero(objNF.irrf);
            envioMigrate.RPS.ListaItens.Item.ItemValPIS = FormataNumero(objNF.pisRetido);

            envioMigrate.RPS.ListaItens.Item.ItemvDesconto = FormataNumero(0);
            envioMigrate.RPS.ListaItens.Item.ItemvIss = FormataNumero(objNF.aliquotaISS);
            envioMigrate.RPS.ListaItens.Item.ItemvlDed = FormataNumero(objNF.valorDeducoes);
            envioMigrate.RPS.ListaItens.Item.ItemvlrISSRetido = FormataNumero(objNF.ValISSRetido);
            envioMigrate.RPS.ListaItens.Item.ItemVlrLiquido = FormataNumero(objNF.valorNfse - 
                objNF.pisRetido - 
                objNF.cofinsRetida - 
                objNF.valorINSS.Value - 
                objNF.irrf - 
                objNF.csllRetida - 
                objNF.valorISS);

            envioMigrate.RPS.ListaItens.Item.ItemVlrTotal = FormataNumero(objNF.valorNfse);
            envioMigrate.RPS.ListaItens.Item.ItemvUnit = null;

            //Parcela
            envioMigrate.RPS.ListaParcelas = new EnvioRPSListaParcelas();
            envioMigrate.RPS.ListaParcelas.Parcela = new EnvioRPSListaParcelasParcela();
            envioMigrate.RPS.ListaParcelas.Parcela.PrcDscTipVenc = null;
            envioMigrate.RPS.ListaParcelas.Parcela.PrcDtaVencimento = null;
            envioMigrate.RPS.ListaParcelas.Parcela.PrcNroFatura = null;
            envioMigrate.RPS.ListaParcelas.Parcela.PrcSequencial = null;
            envioMigrate.RPS.ListaParcelas.Parcela.PrcTipVenc = null;
            envioMigrate.RPS.ListaParcelas.Parcela.PrcValor = null;


            //Contrucao Civil
            envioMigrate.RPS.ConstCivil = new EnvioRPSConstCivil();
            envioMigrate.RPS.ConstCivil.ObraCEI = objNF.codigoCEI;
            envioMigrate.RPS.ConstCivil.ObraNumEncapsulamento = objNF.ObraNumEncapsulamento;
            envioMigrate.RPS.ConstCivil.Art = objNF.matriculaObra;
            envioMigrate.RPS.ConstCivil.CodObra = null;
            envioMigrate.RPS.ConstCivil.ObraBairro = null;
            envioMigrate.RPS.ConstCivil.ObraCEP = null;
            envioMigrate.RPS.ConstCivil.ObraCompl = null;
            envioMigrate.RPS.ConstCivil.ObraLog = null;
            envioMigrate.RPS.ConstCivil.ObraMatricula = objNF.matriculaObra;
            envioMigrate.RPS.ConstCivil.ObraMun = null;
            envioMigrate.RPS.ConstCivil.ObraNumero = null;
            envioMigrate.RPS.ConstCivil.ObraPais = null;
            envioMigrate.RPS.ConstCivil.ObraTipo = null;
            envioMigrate.RPS.ConstCivil.ObraUF = null;
            envioMigrate.RPS.ConstCivil.ObraValRedBC = null;

            return envioMigrate;
        }

        public SoftFin.Migrate.NFSe.DTO.EnvioEvento ConverterDTOMigrateCancelamento(
            Estabelecimento objEstab,
            NotaFiscal objNF)
        {
            var envioEvento = new Migrate.NFSe.DTO.EnvioEvento();
            envioEvento.ModeloDocumento = "NFSe";
            envioEvento.Versao = "1.0";

            envioEvento.Evento = new EnvioEventoEvento();
            envioEvento.Evento.CNPJ = objNF.NotaFiscalPessoaTomador.cnpjCpf;
            envioEvento.Evento.NFSeNumero = objNF.numeroNfse.ToString();
            envioEvento.Evento.RPSNumero = objNF.numeroRps.ToString();
            envioEvento.Evento.RPSSerie = objNF.serieRps;
            envioEvento.Evento.EveTp = "110111";
            envioEvento.Evento.tpAmb = "2";

            return envioEvento;
        }


        private string FormataNumero(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", ".");
        }
        private string FormataNumero(decimal? valor)
        {
            if (valor == null)
                return "0.00";
            return valor.Value.ToString("n").Replace(".", "").Replace(",", ".");
        }
        private string FormataNumero3(decimal valor)
        {
            return valor.ToString("0.0000").Replace(".", "").Replace(",", ".");
        }
        private string FormataNumero2(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", "");
        }
        private string FormataNumeroTresCasas(decimal valor)
        {
            return valor.ToString("0.000").Replace(".", "").Replace(",", ".");
        }

        public void ConverteRetornoGravaLog(DTORetornoNFEs resultado, int notaid, string usuario)
        {
            var log = new LogNFXMLPrincipal();

            log.notaFiscal_id = notaid;
            log.usuarioInsert = usuario;
            log.dataInsert = UtilSoftFin.DateTimeBrasilia();
            log.aceito = (resultado.Cabecalho.Sucesso.ToUpper() == "TRUE");
            log.xml = resultado.xml;
            log.tipo = resultado.tipo;


            foreach (var item in resultado.Erro)
            {
                log.logNFXMLErros.Add(new LogNFXMLErro { codigo = item.Codigo, descricao = item.Descricao });
            }

            foreach (var item in resultado.Alerta)
            {
                log.logNFXMLAlertas.Add(new LogNFXMLAlerta { codigo = item.Codigo, descricao = item.Descricao });
            }

            log.Inclui(log);
        }

  
        internal static DTORetornoNFEs RetornoMigrateToRetornoDto(Migrate.NFSe.DTO.RetornoComum.Envelope resultadoMigrate)
        {
            var dTORetornoNFEs = new DTORetornoNFEs();
            dTORetornoNFEs.tipo = "Envio";
            dTORetornoNFEs.xml = resultadoMigrate.xml;

            
            var codigoResultado = resultadoMigrate.Body.recepcaoExecuteResponse.Invoicyretorno.Mensagem.MensagemItem.Codigo;
            if (codigoResultado.Equals("100"))
            {
                dTORetornoNFEs.Cabecalho.Sucesso = "True";
                var xmlretornoPrefeitura = resultadoMigrate.Body.recepcaoExecuteResponse.Invoicyretorno.Mensagem.MensagemItem.Documentos.DocumentosItem.Documento.ToString();
                var xmlretornoPrefeituraConvertido = LoadFromXMLString(xmlretornoPrefeitura);

                if (xmlretornoPrefeituraConvertido.Situacao.SitCodigo == "999")
                {
                    dTORetornoNFEs.Cabecalho.Sucesso = "False";
                    dTORetornoNFEs.Erro.Add(new TPErro
                    {
                        Codigo = xmlretornoPrefeituraConvertido.Situacao.SitCodigo,
                        Descricao = xmlretornoPrefeituraConvertido.Situacao.SitDescricao
                    });
                }


            }
            else
            {
                dTORetornoNFEs.Cabecalho.Sucesso = "False";
                dTORetornoNFEs.Erro.Add(new TPErro
                {
                    Codigo = codigoResultado.ToString(),
                    Descricao = resultadoMigrate.Body.recepcaoExecuteResponse.Invoicyretorno.Mensagem.MensagemItem.Descricao
                });

            }

            return dTORetornoNFEs;
        }
        public static Migrate.NFSe.DTO.RetornoComum.Documento LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Migrate.NFSe.DTO.RetornoComum.Documento));
            return serializer.Deserialize(stringReader) as Migrate.NFSe.DTO.RetornoComum.Documento;
        }



    }

}