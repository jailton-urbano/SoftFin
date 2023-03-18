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
    public class Conversao
    {
        public void ConverterNFEs(
        DTONotaFiscal obj,
        Estabelecimento objEstab,
        List<NotaFiscal> objNFs)
        {
            var FormatoData = "yyyy-MM-dd";

            decimal valorTotalDeducoes = 0;
            decimal valorTotalServicos = 0;

            obj.municipio_desc = objEstab.Municipio.DESC_MUNICIPIO;

            foreach (var objNF in objNFs)
            {
                var chaveRps = new tpChaveRPS();
                chaveRps.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();
                chaveRps.SerieRPS = objNF.serieRps;
                chaveRps.NumeroRPS = objNF.numeroRps.ToString();
                

                var chaveNFe= new tpChaveNFe();
                chaveNFe.CodigoVerificacao = objNF.codigoVerificacao;
                chaveNFe.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();
                chaveNFe.NumeroNFe = objNF.numeroNfse.ToString();

                
                var enderecoTomador = new tpEndereco();
                enderecoTomador.TipoLogradouro = objNF.NotaFiscalPessoaTomador.tipoEndereco.Substring(0, 1);
                enderecoTomador.Logradouro = objNF.NotaFiscalPessoaTomador.endereco;
                enderecoTomador.NumeroEndereco = objNF.NotaFiscalPessoaTomador.numero;
                enderecoTomador.ComplementoEndereco = objNF.NotaFiscalPessoaTomador.complemento;
                enderecoTomador.Bairro = objNF.NotaFiscalPessoaTomador.bairro;
                enderecoTomador.Cidade = objNF.municipio.DESC_MUNICIPIO;
                enderecoTomador.CodigoMunicipio = objNF.municipio.codigoIBGE;
                enderecoTomador.UF = objNF.NotaFiscalPessoaTomador.uf;
                enderecoTomador.CEP = objNF.NotaFiscalPessoaTomador.cep.Replace("-", "");


                var enderecoPrestador = new tpEndereco();
                enderecoPrestador.TipoLogradouro = objEstab.Logradouro.Substring(0, 1);
                enderecoPrestador.Logradouro = objEstab.Logradouro;
                enderecoPrestador.NumeroEndereco = objEstab.NumeroLogradouro.ToString();
                enderecoPrestador.ComplementoEndereco = objEstab.Complemento;
                enderecoPrestador.Bairro = objEstab.BAIRRO;
                enderecoPrestador.Cidade = objEstab.Municipio.DESC_MUNICIPIO;
                enderecoPrestador.CodigoMunicipio = objEstab.Municipio.codigoIBGE;
                enderecoPrestador.UF = objEstab.UF;
                enderecoPrestador.CEP = objEstab.CEP.Replace("-", "");

                
                var cpfcnpj = new tpCPFCNPJ();
                if (objNF.NotaFiscalPessoaTomador.cnpjCpf.Length == 14)
                {
                    cpfcnpj.CNPJ = UtilSoftFin.Limpastrings(objNF.NotaFiscalPessoaTomador.cnpjCpf);
                }
                else
                {
                    cpfcnpj.CPF = UtilSoftFin.Limpastrings(objNF.NotaFiscalPessoaTomador.cnpjCpf);

                }
                cpfcnpj.InscricaoMunicipal = objNF.NotaFiscalPessoaTomador.inscricaoMunicipal;

                var cpfcnpjPrestador = new tpCPFCNPJ();
                cpfcnpjPrestador.CNPJ = objEstab.CNPJ.Replace("-", "").Replace("/", "").Replace(".", ""); ;
                cpfcnpjPrestador.InscricaoMunicipal = objEstab.InscricaoMunicipal.ToString();


                var objcon = new tpNFSe();

                
                objcon.ChaveRPS = chaveRps;
                objcon.ChaveNFe = chaveNFe;
                objcon.EnderecoTomador = enderecoPrestador;
                objcon.EnderecoPrestador = enderecoPrestador;
                objcon.CPFCNPJTomador = cpfcnpj;
                objcon.CPFCNPJPrestador = cpfcnpjPrestador;

                objcon.CodigoServicos = objNF.codigoServico;
                objcon.TipoRPS = objNF.Operacao.tipoRPS.codigo;
                objcon.DataEmissao = objNF.dataEmissaoRps.ToString(FormatoData);
                objcon.DataEmissaoRPS = objNF.dataEmissaoRps.ToString(FormatoData);
                objcon.DataEmissaoNFe = objNF.dataEmissaoNfse.ToString(FormatoData);

                objcon.StatusRPS = "N";
                objcon.TributacaoRPS = objNF.Operacao.situacaoTributariaNota.codigo;
                objcon.ValorServicos = FormataNumero(objNF.valorNfse);
                objcon.ValorDeducoes = FormataNumero(objNF.valorDeducoes);
                objcon.ValorPIS = FormataNumero(objNF.pisRetido);
                objcon.ValorCOFINS = FormataNumero(objNF.cofinsRetida);
                objcon.ValorINSS = FormataNumero(objNF.valorINSS.Value);
                objcon.ValorIR = FormataNumero(objNF.irrf);
                objcon.ValorCSLL = FormataNumero(objNF.csllRetida);
                objcon.AliquotaServicos = FormataNumeroTresCasas(objNF.aliquotaISS /100);
                objcon.ValorIss = FormataNumero(objNF.valorISS);
                objcon.ValorIssRetido = FormataNumero(0);
                objcon.valorOutrasDeducoes = FormataNumero(0);
                objcon.BaseCalculo = FormataNumero(objNF.valorNfse - objNF.valorDeducoes);
                objcon.ValorLiquidoNfse = FormataNumero(objNF.valorNfse - objNF.pisRetido - objNF.cofinsRetida - objNF.valorINSS.Value - objNF.irrf - objNF.csllRetida - objNF.valorISS);
                objcon.ISSRetido = "false";
                objcon.EmailTomador = objNF.NotaFiscalPessoaTomador.email;
                objcon.ValorCargaTributaria = (objNF.valorCargaTributaria == null) ? "" : FormataNumero(objNF.valorCargaTributaria.Value);

                objcon.PercentualCargaTributaria = (objNF.percentualCargaTributaria == null) ? "": FormataNumero3(objNF.percentualCargaTributaria.Value / 100);
                if (objcon.PercentualCargaTributaria == "0.0000")
                    objcon.PercentualCargaTributaria = null;

                objcon.FonteCargaTributaria = objNF.fonteCargaTributaria;
                objcon.CodigoCEI = objNF.codigoCEI;
                objcon.MatriculaObra = objNF.matriculaObra;
                objcon.ObraNumEncapsulamento = objNF.ObraNumEncapsulamento;

                objcon.Discriminacao = objNF.discriminacaoServico.Replace("\n", "|").Replace("\r", "");

                objcon.RazaoSocialTomador = objNF.NotaFiscalPessoaTomador.razao;
                objcon.CodigoMunicipioPrestador = objNF.municipio.codigoIBGE;
                objcon.CnpjPrestador = objEstab.CNPJ.Replace("-", "").Replace("/", "").Replace(".", "");
                objcon.InscricaoMunicipalPretador = objEstab.InscricaoMunicipal.ToString();
                if (objNF.Operacao.CFOP != null)
                    objcon.NaturezaOperaco = objNF.Operacao.CFOP;
                objcon.OptanteSimplesNacional = objEstab.opcaoTributariaSimples.codigo;
                objcon.CodigoTributacaoMunicipio = objEstab.CodigoTributacaoMunicipio;
                obj.NFSe.Add(objcon);

                valorTotalDeducoes += objNF.valorDeducoes;
                valorTotalServicos += objNF.valorNfse;

            }

            obj.Cabecalho.CPFCNPJRemetente.CNPJ = UtilSoftFin.Limpastrings(objEstab.CNPJ);
            obj.Cabecalho.transacao = "true";
            obj.Cabecalho.dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(FormatoData);
            obj.Cabecalho.dtFim = DateTime.Now.ToString(FormatoData); ;
            obj.Cabecalho.QtdRPS = obj.NFSe.Count().ToString();
            obj.Cabecalho.ValorTotalServicos = FormataNumero(valorTotalServicos);
            obj.Cabecalho.ValorTotalDeducoes = FormataNumero(valorTotalDeducoes);
        }

        private string FormataNumero(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", ".");
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

        public void ConverteRetornoGravaLog(SoftFin.NFe.DTO.DTORetornoNFe resultado, int notaid, string usuario)
        {
            var log = new LogNFXMLPrincipal();

            log.notaFiscal_id = notaid;
            log.usuarioInsert = usuario;
            log.dataInsert = UtilSoftFin.DateTimeBrasilia();
            log.aceito = (resultado.Sucesso == true);
            log.xml = resultado.xml.InnerXml;
            log.tipo = resultado.tipo;
            log.xmlRetorno = resultado.xmlRetorno;


            foreach (var item in resultado.Erros)
            {

                log.logNFXMLErros.Add(new LogNFXMLErro { codigo = item.codigo, descricao = (item.descricao.Length < 499) ? item.descricao : item.descricao.Substring(0,498) });
            }

            if (resultado.Alertas != null) 
                foreach (var item in resultado.Alertas)
                {
                    log.logNFXMLAlertas.Add(new LogNFXMLAlerta { codigo = item.codigo, descricao = (item.descricao.Length < 499) ? item.descricao : item.descricao.Substring(0, 498) });
                }

            log.Inclui(log);
        }

        public void ConverterNFe(SoftFin.NFe.DTO.DTONfe dTONfe, Estabelecimento _estabobj, List<NotaFiscal> listaNF)
        {
            foreach (var nf in listaNF)
            {
                dTONfe.InfNFe.versao = "3.10";
                dTONfe.idLote = nf.loteNfe.ToString();
                dTONfe.InfNFe.Ide.cUF = CodigoEstado(_estabobj.UF);
                dTONfe.InfNFe.Ide.natOp = BuscaNatureza(nf.NotaFiscalNFE.CFOP);
                dTONfe.InfNFe.Ide.indPag = nf.NotaFiscalNFE.faturaFormaPgto.ToString();
                dTONfe.InfNFe.Ide.mod = "55";
                //55=NF-e emitida em substituição ao modelo 1 ou 1A;
                //65=NFC-e, utilizada nas operações de venda no varejo (a
                //critério da UF aceitar este modelo de documento).

                if (nf.serieNfe == 0)
                {
                    dTONfe.InfNFe.Ide.serie = "000";
                }
                else
                {
                    dTONfe.InfNFe.Ide.serie = nf.serieNfe.ToString();
                }
                dTONfe.InfNFe.Ide.nNF = nf.numeroNfe.ToString();
                dTONfe.InfNFe.Ide.cNF = nf.numeroNfe.ToString().PadLeft(8, '0');
                dTONfe.InfNFe.Ide.dhEmi = nf.dataEmissaoNfse.ToString("yyyy-MM-ddTHH:mm:ss") + "-03:00";
                dTONfe.InfNFe.Ide.dhSaiEnt = nf.NotaFiscalNFE.dataHoraSaida.ToString("yyyy-MM-ddTHH:mm:ss") + "-03:00";
                dTONfe.InfNFe.Ide.tpNF = "1";//0=Entrada;1=Saída
                dTONfe.InfNFe.Ide.idDest = "1";//1=Operação interna;2=Operação interestadual;3=Operação com exterior
                dTONfe.InfNFe.Ide.cMunFG = nf.municipio.codigoIBGE.ToString();
                dTONfe.InfNFe.Ide.tpImp = "1";
                dTONfe.InfNFe.Ide.tpEmis = "1";
                dTONfe.InfNFe.Ide.tpAmb = "2";
                dTONfe.InfNFe.Ide.finNFe = "1";
                dTONfe.InfNFe.Ide.indFinal = "0";
                dTONfe.InfNFe.Ide.indPres = "9";
                dTONfe.InfNFe.Ide.procEmi = "0";
                dTONfe.InfNFe.Ide.chavenota = nf.NotaFiscalNFE.chaveAcesso;
                dTONfe.InfNFe.Ide.nProt = nf.NotaFiscalNFE.protocoloAutorizacao;
                dTONfe.InfNFe.Ide.verProc = "SoftFin 2.0";
                


                dTONfe.InfNFe.Emi.CNPJ = _estabobj.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "");
                dTONfe.InfNFe.AutXML = new List<SoftFin.NFe.DTO.DTOAutXML>();
                //dTONfe.InfNFe.AutXML.Add(new SoftFin.NFe.DTO.DTOAutXML { CNPJ = dTONfe.InfNFe.Emi.CNPJ });
;
                dTONfe.InfNFe.Emi.xNome = _estabobj.NomeCompleto;
                dTONfe.InfNFe.Emi.xFant = _estabobj.Apelido;
                dTONfe.InfNFe.Emi.EnderEmit.xLgr = _estabobj.Logradouro;
                dTONfe.InfNFe.Emi.EnderEmit.nro = _estabobj.NumeroLogradouro.ToString();
                dTONfe.InfNFe.Emi.EnderEmit.xBairro = _estabobj.BAIRRO;
                dTONfe.InfNFe.Emi.EnderEmit.cMun = nf.municipio.codigoIBGE.ToString();
                dTONfe.InfNFe.Emi.EnderEmit.xMun = nf.municipio.DESC_MUNICIPIO;
                dTONfe.InfNFe.Emi.EnderEmit.UF = _estabobj.UF;
                dTONfe.InfNFe.Emi.EnderEmit.CEP = _estabobj.CEP.Replace("-", "");
                dTONfe.InfNFe.Emi.EnderEmit.cPais = "1058";
                dTONfe.InfNFe.Emi.EnderEmit.xPais = "BRASIL";
                if (_estabobj.Fone != null)
                {
                    dTONfe.InfNFe.Emi.EnderEmit.fone = _estabobj.Fone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""); ;
                }

                if (_estabobj.InscricaoEstadual != null)
                {
                    if (_estabobj.InscricaoEstadual.ToUpper() == "ISENTA")
                    {
                        dTONfe.InfNFe.Emi.IE = "ISENTO";
                    }
                    else
                    {
                        dTONfe.InfNFe.Emi.IE = _estabobj.InscricaoEstadual.ToUpper();

                    }
                }
                
                dTONfe.InfNFe.Emi.CRT = "1";// _estabobj.CRT;
                dTONfe.InfNFe.Ide.cDV = CalculoCDVNFE(dTONfe);

                if (nf.NotaFiscalPessoaTomador.indicadorCnpjCpf == 2)
                {
                    dTONfe.InfNFe.Dest.CNPJ = nf.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", "");
                    //dTONfe.InfNFe.AutXML.Add(new SoftFin.NFe.DTO.DTOAutXML { CNPJ = dTONfe.InfNFe.Dest.CNPJ });
                }
                else
                {
                    dTONfe.InfNFe.Dest.CPF = nf.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", "");
                    //dTONfe.InfNFe.AutXML.Add(new SoftFin.NFe.DTO.DTOAutXML { CNPJ = dTONfe.InfNFe.Dest.CPF });

                }
                
                dTONfe.InfNFe.Dest.xNome = nf.NotaFiscalPessoaTomador.razao;
                dTONfe.InfNFe.Dest.EnderDest.xLgr = nf.NotaFiscalPessoaTomador.endereco;
                dTONfe.InfNFe.Dest.EnderDest.nro = nf.NotaFiscalPessoaTomador.numero;
                dTONfe.InfNFe.Dest.EnderDest.xBairro = nf.NotaFiscalPessoaTomador.bairro;
                dTONfe.InfNFe.Dest.EnderDest.cMun = CodigoMunicipio(nf.NotaFiscalPessoaTomador.cidade).ToString();
                dTONfe.InfNFe.Dest.EnderDest.xMun = nf.NotaFiscalPessoaTomador.cidade;
                dTONfe.InfNFe.Dest.EnderDest.UF = nf.NotaFiscalPessoaTomador.uf;
                dTONfe.InfNFe.Dest.EnderDest.CEP = nf.NotaFiscalPessoaTomador.cep.Replace("-","");
                dTONfe.InfNFe.Dest.EnderDest.cPais = "1058";
                dTONfe.InfNFe.Dest.EnderDest.xPais = "BRASIL";
                dTONfe.InfNFe.Dest.EnderDest.fone = (nf.NotaFiscalPessoaTomador.fone == null) ? "" : nf.NotaFiscalPessoaTomador.fone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                if (nf.NotaFiscalPessoaTomador.inscricaoEstadual.ToUpper().Equals("ISENTO"))
                    dTONfe.InfNFe.Dest.indIEDest = "2";
                else if (nf.NotaFiscalPessoaTomador.inscricaoEstadual == "NÃO CONTRIBUINTE")
                {
                    dTONfe.InfNFe.Dest.indIEDest = "9";
                    dTONfe.InfNFe.Ide.indFinal = "1";
                }
                else
                {
                    

                    dTONfe.InfNFe.Dest.IE = nf.NotaFiscalPessoaTomador.inscricaoEstadual;
                    //if (int.TryParse(nf.NotaFiscalPessoaTomador.inscricaoEstadual , out ieTeste) == false)
                    //{
                    //    throw new Exception("Inscrição estadual do cliente pode ser ('ISENTO','NÃO CONTRIBUINTE' ou 'numero sem caracteres especiais')");
                    //}
                    dTONfe.InfNFe.Dest.indIEDest = "1";
                }


                if (nf.NotaFiscalNFE.NotaFiscalNFERetirada != null)
                {
                    if (nf.NotaFiscalNFE.NotaFiscalNFERetirada.cnpjCPF != null)
                    {
                        var cnpjCPFAux = UtilSoftFin.Limpastrings(nf.NotaFiscalNFE.NotaFiscalNFERetirada.cnpjCPF);

                        if (cnpjCPFAux.Length != 14)
                            dTONfe.InfNFe.Retirada.CPF = cnpjCPFAux;
                        else
                            dTONfe.InfNFe.Retirada.CNPJ = cnpjCPFAux;

                        dTONfe.InfNFe.Retirada.cMun = nf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio;
                        dTONfe.InfNFe.Retirada.xMun = ExtraiNomeMunicipio(nf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio);
                        dTONfe.InfNFe.Retirada.UF = ExtraiUF(nf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio);
                        dTONfe.InfNFe.Retirada.nro = nf.NotaFiscalNFE.NotaFiscalNFERetirada.numero;
                        dTONfe.InfNFe.Retirada.xBairro = nf.NotaFiscalNFE.NotaFiscalNFERetirada.bairro;
                        dTONfe.InfNFe.Retirada.xCpl = nf.NotaFiscalNFE.NotaFiscalNFERetirada.complemento;
                        dTONfe.InfNFe.Retirada.xLgr = nf.NotaFiscalNFE.NotaFiscalNFERetirada.endereco;
                    }
                }

                if (nf.NotaFiscalNFE.NotaFiscalNFEEntrega != null)
                {
                    if (nf.NotaFiscalNFE.NotaFiscalNFEEntrega.cnpjCPF != null)
                    {
                        var cnpjCPFAux = UtilSoftFin.Limpastrings(nf.NotaFiscalNFE.NotaFiscalNFEEntrega.cnpjCPF);

                        if (cnpjCPFAux.Length != 14)
                            dTONfe.InfNFe.Entrega.CPF = cnpjCPFAux;
                        else
                            dTONfe.InfNFe.Entrega.CNPJ = cnpjCPFAux;

                        dTONfe.InfNFe.Entrega.cMun = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio;
                        dTONfe.InfNFe.Entrega.xMun = ExtraiNomeMunicipio(nf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio);
                        dTONfe.InfNFe.Entrega.UF = ExtraiUF(nf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio);
                        dTONfe.InfNFe.Entrega.nro = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.numero;
                        dTONfe.InfNFe.Entrega.xBairro = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.bairro;
                        dTONfe.InfNFe.Entrega.xCpl = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.complemento;
                        dTONfe.InfNFe.Entrega.xLgr = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.endereco;
                    }
                }



                decimal valorTributos = 0;
                var nfsitem = new NotaFiscalNFEItem().ObterPorNf(nf.NotaFiscalNFE.id);
                var contador = 1;
                foreach (var item in nfsitem)
	            {
		            var det = new SoftFin.NFe.DTO.DTODet();

                    det.nItem = contador.ToString();
                    det.Prod.cProd = item.codigoProduto; 
                    det.Prod.cEAN = item.EAN;
                    det.Prod.xProd = item.nomeProduto;
                    det.Prod.NCM = item.NCM;
                    det.Prod.CFOP = item.CFOP;
                    det.Prod.uCom = item.unidadeMedida;
                    det.Prod.qCom = item.quantidade.ToString();
                    det.Prod.vUnCom = item.valorUnitario.ToString().Replace(",", ".");
                    det.Prod.vProd = item.valor.ToString().Replace(",", ".");
                    det.Prod.cEANTrib = item.EAN;
                    det.Prod.uTrib = item.unidadeMedida;
                    det.Prod.qTrib = item.quantidade.ToString();
                    det.Prod.vDesc = item.desconto.ToString().Replace(",", ".");
                    det.Prod.vUnTrib = item.valorUnitario.ToString().Replace(",", ".");
                    det.Prod.orig = item.origem;
                    det.Prod.CSOSN = item.CSOSN;
                    det.Prod.indTot = "1";
                    det.Prod.nItemPed = "000000";
                    det.Prod.CEST = item.CEST;
                    det.Prod.infAdProd = item.infAdProd ;
                    if (det.Prod.infAdProd == null)
                        det.Prod.infAdProd = "";

                    det.Imposto.PIS.CST = item.PISCST;
                    det.Imposto.PIS.pPIS = item.aliquotaPIS.ToString().Replace(",", ".");
                    det.Imposto.PIS.vBC = item.basePIS.ToString().Replace(",", ".");
                    det.Imposto.PIS.vPIS = item.valorPIS.ToString().Replace(",", ".");
                    
                    det.Imposto.COFINS.CST = item.COFINSCST;
                    det.Imposto.COFINS.pCOFINS = item.aliquotaCOFINS.ToString().Replace(",", ".");
                    det.Imposto.COFINS.vBC = item.baseCOFINS.ToString().Replace(",", ".");
                    det.Imposto.COFINS.vCOFINS = item.valorCOFINS.ToString().Replace(",", ".");

                    if (item.valorTributos != 0)
                        det.Imposto.vTotTrib = item.valorTributos.ToString().Replace(",", ".");

                    valorTributos += item.valorTributos; 

                    if (item.valorTributos != 0)
                        det.Prod.infAdProd += "Total aproximado de tributos: R$" + item.valorTributos.ToString() + " Fonte: IBPT";

                    dTONfe.InfNFe.Det.Add(det);
                    contador++;
	            }

                dTONfe.InfNFe.Total.ICMSTot.vBC = nf.NotaFiscalNFE.baseICMS.ToString().Replace(",",".");
                dTONfe.InfNFe.Total.ICMSTot.vICMS = nf.NotaFiscalNFE.valorICMS.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vICMSDeson = nf.NotaFiscalNFE.valorICMSDesonerado.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vBCST = nf.NotaFiscalNFE.baseICMSST.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vST = nf.NotaFiscalNFE.valorICMSST.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vProd = nf.NotaFiscalNFE.valorProduto.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vFrete = nf.NotaFiscalNFE.valorFrete.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vSeg = nf.NotaFiscalNFE.valorSeguro.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vDesc = nf.NotaFiscalNFE.valorDesconto.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vII = nf.NotaFiscalNFE.valorII.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vIPI = nf.NotaFiscalNFE.valorIPI.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vPIS = nf.NotaFiscalNFE.valorPIS.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vCOFINS = nf.NotaFiscalNFE.valorCONFINS.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vOutro = nf.NotaFiscalNFE.valorOutro.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vNF = nf.NotaFiscalNFE.valor.ToString().Replace(",", ".");
                dTONfe.InfNFe.Total.ICMSTot.vTotTrib = valorTributos.ToString().Replace(",", ".");

                dTONfe.InfNFe.Total.ICMSTot.vFCP = (0).ToString().Replace(",", ".");// v4.0
                dTONfe.InfNFe.Total.ICMSTot.vFCPST = (0).ToString().Replace(",", ".");// v4.0
                dTONfe.InfNFe.Total.ICMSTot.vFCPSTRet = (0).ToString().Replace(",", ".");// v4.0
                dTONfe.InfNFe.Total.ICMSTot.vIPIDevol = (0).ToString().Replace(",", ".");// v4.0


                nf.NotaFiscalNFE.NotaFiscalNFETransportadora = new NotaFiscalNFETransportadora().ObterPorId(nf.NotaFiscalNFE.NotaFiscalNFETransportadora_id.Value);

                dTONfe.InfNFe.Transp.modFrete = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.modalidadeFrete;

                if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF != null)
                {
                    if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length == 14)
                    {
                        dTONfe.InfNFe.Transp.Transporta.CNPJ = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
                    }
                    if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length == 11)
                    {
                        dTONfe.InfNFe.Transp.Transporta.CPF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
                    }
                    dTONfe.InfNFe.Transp.Transporta.CPF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
                    dTONfe.InfNFe.Transp.Transporta.xNome = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.nomeRazao;
                    if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE != null)
                    {
                        if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE.ToUpper() == "ISENTA")
                        {
                            nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE = "ISENTO";
                        }
                        else
                        {
                            nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE.ToString();

                        }
                    }
                    
                    dTONfe.InfNFe.Transp.Transporta.xEnder = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.EnderecoCompleto;
                    dTONfe.InfNFe.Transp.Transporta.xMun = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cidade;
                    dTONfe.InfNFe.Transp.Transporta.UF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.uf;
                }

                if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.ICMSRetido != 0)
                {
                    dTONfe.InfNFe.Transp.RetTransp.vServ = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.valorServico.ToString().Replace(",",".");
                    dTONfe.InfNFe.Transp.RetTransp.vBCRet = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.baseCalculo.ToString().Replace(",", ".");
                    dTONfe.InfNFe.Transp.RetTransp.pICMSRet = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.aliquota.ToString().Replace(",", ".");
                    dTONfe.InfNFe.Transp.RetTransp.vICMSRet = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.ICMSRetido.ToString().Replace(",", ".");
                    dTONfe.InfNFe.Transp.RetTransp.CFOP = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.CFOP;
                    dTONfe.InfNFe.Transp.RetTransp.cMunFG = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.codigoMunicipioOcorrencia;
                }

                dTONfe.InfNFe.Transp.VeicTransp.placa = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.placa;
                dTONfe.InfNFe.Transp.VeicTransp.RNTC = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.RNTC;
                dTONfe.InfNFe.Transp.VeicTransp.UF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.ufplaca;

                nf.NotaFiscalNFE.NotaFiscalNFEVolume = new NotaFiscalNFEVolume().ObterPorNf(nf.NotaFiscalNFE.id);
                foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEVolume)
                {
                    var itemVolume = new SoftFin.NFe.DTO.DTOVol
                    {
                        esp = item.especie,
                        marca = item.marca,
                        nVol = item.numeracao,
                        pesoB = item.pesoBruto.ToString().Replace(",", "."),
                        pesoL = item.pesoLiquido.ToString().Replace(",", "."),
                        qVol = item.qtde.ToString()

                    };

                    var lks = item.lacres.Split(';');

                    foreach (var itemLks in lks)
                    {
                        itemVolume.lacres.Add(new SoftFin.NFe.DTO.DTOLacres { nLacre = itemLks });
                    }
                    dTONfe.InfNFe.Transp.Vol.Add(itemVolume);
                }

                dTONfe.InfNFe.Cobr.Fat.nFat = nf.NotaFiscalNFE.faturaNumero;
                dTONfe.InfNFe.Cobr.Fat.vDesc = nf.NotaFiscalNFE.ToString().Replace(",", ".");
                dTONfe.InfNFe.Cobr.Fat.vLiq = nf.NotaFiscalNFE.ToString().Replace(",", ".");
                dTONfe.InfNFe.Cobr.Fat.vOrig = nf.NotaFiscalNFE.ToString().Replace(",", ".");

                nf.NotaFiscalNFE.NotaFiscalNFEDuplicatas = new NotaFiscalNFEDuplicata().ObterPorNf(nf.NotaFiscalNFE.id);

                foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEDuplicatas)
                {
                    dTONfe.InfNFe.Cobr.Dup.Add(new SoftFin.NFe.DTO.DTODup
                    {
                        dVenc = item.vencto.ToString("yyyy-MM-yy"),
                        nDup = item.numero.ToString(),
                        vDup = item.valor.ToString().Replace(",", ".")
                    });
                }

                dTONfe.InfNFe.InfAdic.infCpl = nf.NotaFiscalNFE.informacaoComplementar;
                dTONfe.InfNFe.InfAdic.infAdFisco = nf.NotaFiscalNFE.informacaoComplementarFisco;

                dTONfe.InfNFe.Pagamento = new List<NFe.DTO.DTOPagamento>();

                foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEFormaPagamentos)
                {
                    var dtoPG = new NFe.DTO.DTOPagamento();

                    if (item.indPag != null)
                        dtoPG.indPag = item.indPag.ToString();

                    if (item.tPag != null)
                        dtoPG.tPag = item.tPag.Value.ToString("00");

                    if (item.vPag != 0)
                        dtoPG.vPag = item.vPag.ToString().Replace(",", ".");

                    if (item.tpIntegra != null)
                        dtoPG.tpIntegra = item.tpIntegra.ToString();

                    if (item.tBand != null)
                        dtoPG.tBand = item.tBand.Value.ToString("00");

                    if (item.vTroco != null)
                        dtoPG.vTroco = item.vTroco.Value.ToString().Replace(",", ".");

                    if (item.cAut != null)
                        dtoPG.cAut = item.cAut;

                    if (item.indPag != null)
                        dtoPG.indPag = item.indPag.Value.ToString().Replace(",", ".");

                    dTONfe.InfNFe.Pagamento.Add(dtoPG);
                }
                
            }
        }

        private string ExtraiUF(string codigoIBGE)
        {
            var municipio = new Municipio().ObterPorCodigoIBGE(codigoIBGE);


            if (municipio == null)
                throw new Exception("Municipio não preparado");
            else
                return municipio.UF;
        }

        private string ExtraiNomeMunicipio(string codigoIBGE)
        {

            var municipio = new Municipio().ObterPorCodigoIBGE(codigoIBGE);


            if (municipio == null)
                throw new Exception("Municipio não preparado");
            else
                return municipio.DESC_MUNICIPIO;
        }

        public string CalculoCDVNFE(SoftFin.NFe.DTO.DTONfe dTONfe)
        {

            int AA = (DateTime.Parse(dTONfe.InfNFe.Ide.dhEmi).Year.ToString().Count() == 4) ? int.Parse(DateTime.Parse(dTONfe.InfNFe.Ide.dhEmi).Year.ToString().Substring(2)) : DateTime.Parse(dTONfe.InfNFe.Ide.dhEmi).Year;
            string MM = DateTime.Parse(dTONfe.InfNFe.Ide.dhEmi).Month.ToString().PadLeft(2, '0');


            string chave = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
               dTONfe.InfNFe.Emi.EnderEmit.cMun.Substring(0, 2),
               AA,
               MM,
               dTONfe.InfNFe.Emi.CNPJ,
               dTONfe.InfNFe.Ide.mod,
               dTONfe.InfNFe.Ide.serie.PadLeft(3, '0'),
               dTONfe.InfNFe.Ide.nNF.PadLeft(9, '0'),
               dTONfe.InfNFe.Ide.tpEmis,
               dTONfe.InfNFe.Ide.cNF);


            string chaveInvertida = ReverseString(chave);
            int[] t = { 2, 3, 4, 5, 6, 7, 8, 9 };
            int somatorio = 0;
            int posicaoParaCalculo = 0;
            foreach (var v in chaveInvertida)
            {

                somatorio = somatorio + (int.Parse(v.ToString()) * t[posicaoParaCalculo]);
                if (posicaoParaCalculo == 7)
                {
                    posicaoParaCalculo = 0;
                }
                else
                {
                    posicaoParaCalculo += 1;
                }
            }

            int resto = somatorio % 11;
            int dv;
            if (resto == 0 || resto == 1)
            {
                dv = 0;
            }
            else
            {
                dv = (11 - resto);
            }

            dTONfe.InfNFe.Id = "NFe" + chave + dv.ToString();
            return dv.ToString();

        }
        /// <summary>
        /// Metodo para inverter uma string
        /// </summary>
        /// <param name="prStringEntrada"></param>
        /// <returns></returns>
        public string ReverseString(string prStringEntrada)
        {
            char[] arrChar = prStringEntrada.ToCharArray();
            Array.Reverse(arrChar);
            string invertida = new String(arrChar);

            return invertida;
        }


        private string BuscaNatureza(string cfop)
        {
            return new CFOP().ObterPorCfop(cfop).descricao;
        }

        private string CodigoEstado(string p)
        {
            switch (p)
            {
                case "SP":
                    return "35";
                default:
                    throw new Exception("Estado não preparado");
            }
        }

        private String CodigoMunicipio(string nome)
        {
            var municipio = new Municipio().ObterPorNome(nome);


            if (municipio.Count() == 0)
                throw new Exception("Municipio não preparado");
            else
                return municipio.First().codigoIBGE.ToString();
        }

        public static Migrate.NFSe.DTO.RetornoComum.Documento LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Migrate.NFSe.DTO.RetornoComum.Documento));
            return serializer.Deserialize(stringReader) as Migrate.NFSe.DTO.RetornoComum.Documento;
        }
    }

}