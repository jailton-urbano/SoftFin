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
    public class ConversaoMigrateMercadoria
    {
        public void ConverterNFeMigrate(SoftFin.Migrate.NFe.DTO.Envio dTONfe, Estabelecimento _estabobj, List<NotaFiscal> listaNF)
        {
            foreach (var nf in listaNF)
            {
                dTONfe.ModeloDocumento = "NFe";
                dTONfe.Versao = "4.00";

                dTONfe.ide = new Migrate.NFe.DTO.EnvioIde();

                dTONfe.ide.cNF = nf.loteNfe.ToString();
                dTONfe.ide.cUF = CodigoEstado(_estabobj.UF);
                dTONfe.ide.natOp = BuscaNatureza(nf.NotaFiscalNFE.CFOP);
                dTONfe.ide.mod = "55";
                if (nf.serieNfe == 0)
                {
                    dTONfe.ide.serie = "000";
                }
                else
                {
                    dTONfe.ide.serie = nf.serieNfe.ToString();
                }
                dTONfe.ide.cNF = nf.numeroNfe.ToString().PadLeft(8, '0');
                dTONfe.ide.dhEmi = nf.dataEmissaoNfse.ToString("yyyy-MM-ddTHH:mm:ss") ;
                dTONfe.ide.fusoHorario = "-03:00";
                dTONfe.ide.dhSaiEnt = nf.NotaFiscalNFE.dataHoraSaida.ToString("yyyy-MM-ddTHH:mm:ss");
                dTONfe.ide.tpNf = nf.NotaFiscalNFE.TipoOperacao.ToString(); //"0-entrada 1 - saída"; //TODO RICARDO ACERTAR GERACAO  DE NOTA
                dTONfe.ide.idDest = nf.NotaFiscalNFE.IndicadorDestino;//
                dTONfe.ide.indFinal = nf.NotaFiscalNFE.IndicadorFinal;
                dTONfe.ide.indPres = nf.NotaFiscalNFE.IndicadorPresencial;
                dTONfe.ide.cMunFg = nf.municipio.codigoIBGE.ToString();
                dTONfe.ide.tpImp = "1";
                dTONfe.ide.tpEmis = "1";
                dTONfe.ide.tpAmb = "2";
                dTONfe.ide.xJust = "";
                dTONfe.ide.dhCont = "";
                dTONfe.ide.finNFe = nf.tipoNfe;
                dTONfe.ide.EmailArquivos = _estabobj.emailNotificacoes + ";" + "andre.dias@softfin.com.br";
                dTONfe.ide.NumeroPedido = nf.loteNfe.ToString();

                if (nf.NotaFiscalNFE.NotaFiscalNFEReferenciadas.Count() > 0)
                {
                    dTONfe.ide.NFRef = new List<Migrate.NFe.DTO.NFRefItem>();

                    foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEReferenciadas)
                    {
                        var aux = new Migrate.NFe.DTO.NFRefItem();
                        aux.refNFe = nf.NotaFiscalNFE.chaveAcesso;
                        aux.cUF_refNFE = CodigoEstado(_estabobj.UF);
                        aux.AAMM = item.nfanoMesEmissao;
                        aux.CNPJ = item.nfcnpj;
                        //aux.CPF = item.cp
                        aux.mod_refNFE = item.modelo;
                        aux.serie_refNFE = item.nfserie;
                        aux.nNF_refNFE = item.nfnumero;
                        aux.IE_refNFP = item.nfprodIE;
                        aux.RefCte = item.CTe;
                        aux.mod_refECF = item.modelo;
                        aux.nECF_refECF = item.ECF;
                        aux.nCOO_refECF = item.numeroCOO;
                        dTONfe.ide.NFRef.Add(aux);
                    }
                }

                dTONfe.emit = new Migrate.NFe.DTO.EnvioEmit();

                dTONfe.emit.CNPJ_emit = _estabobj.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "");

                dTONfe.emit.xNome = _estabobj.NomeCompleto;
                dTONfe.emit.xFant = _estabobj.Apelido;

                if (!string.IsNullOrEmpty(_estabobj.CNAE))
                {
                    dTONfe.emit.IM = _estabobj.InscricaoMunicipal;
                    dTONfe.emit.CNAE = _estabobj.CNAE;
                }
                               
                if (_estabobj.InscricaoEstadual != null)
                {
                    if (_estabobj.InscricaoEstadual.ToUpper() == "ISENTA")
                    {
                        dTONfe.emit.IE = "ISENTO";
                    }
                    else
                    {
                        dTONfe.emit.IE = _estabobj.InscricaoEstadual.ToUpper();

                    }
                }
                
                dTONfe.emit.IEST = "";
                dTONfe.emit.CRT = _estabobj.opcaoTributariaSimples.codigo;

                dTONfe.emit.enderEmit = new Migrate.NFe.DTO.EnvioEmitEnderEmit();
                dTONfe.emit.enderEmit.xLgr = _estabobj.Logradouro;
                dTONfe.emit.enderEmit.nro = _estabobj.NumeroLogradouro.ToString();
                dTONfe.emit.enderEmit.xBairro = _estabobj.BAIRRO;
                dTONfe.emit.enderEmit.cMun = nf.municipio.codigoIBGE.ToString();
                dTONfe.emit.enderEmit.xMun = nf.municipio.DESC_MUNICIPIO;
                dTONfe.emit.enderEmit.UF = _estabobj.UF;
                dTONfe.emit.enderEmit.CEP = _estabobj.CEP.Replace("-", "");
                dTONfe.emit.enderEmit.cPais = "1058";
                dTONfe.emit.enderEmit.xPais = "BRASIL";
                dTONfe.emit.enderEmit.fone = SoftFin.Utils.UtilSoftFin.Limpastrings(_estabobj.Fone);
                dTONfe.emit.enderEmit.Email = _estabobj.emailNotificacoes;

                dTONfe.dest = new Migrate.NFe.DTO.EnvioDest();
                if (nf.NotaFiscalPessoaTomador.indicadorCnpjCpf == 1)
                {
                    dTONfe.dest.CPF_dest = SoftFin.Utils.UtilSoftFin.Limpastrings(nf.NotaFiscalPessoaTomador.cnpjCpf);
                }
                else
                {
                    dTONfe.dest.CNPJ_dest = SoftFin.Utils.UtilSoftFin.Limpastrings(nf.NotaFiscalPessoaTomador.cnpjCpf);
                }

                //dTONfe.dest.idEstrangeiro = nf.NotaFiscalPessoaTomador.razao; // Não Preparado
                dTONfe.dest.xNome_dest = nf.NotaFiscalPessoaTomador.razao;
                
                if (nf.NotaFiscalPessoaTomador.inscricaoEstadual.ToUpper().Equals("ISENTO"))
                    dTONfe.dest.indIEDest = "2";
                else if (nf.NotaFiscalPessoaTomador.inscricaoEstadual == "NÃO CONTRIBUINTE")
                {
                    dTONfe.dest.indIEDest = "9";
                }
                else
                {
                    dTONfe.dest.indIEDest = "1";
                    dTONfe.dest.IE_dest = nf.NotaFiscalPessoaTomador.inscricaoEstadual;
                }
                dTONfe.dest.enderDest = new Migrate.NFe.DTO.EnvioDestEnderDest();
                dTONfe.dest.enderDest.xLgr_dest = nf.NotaFiscalPessoaTomador.endereco;
                dTONfe.dest.enderDest.nro_dest = nf.NotaFiscalPessoaTomador.numero;
                dTONfe.dest.enderDest.xBairro_dest = nf.NotaFiscalPessoaTomador.bairro;
                dTONfe.dest.enderDest.cMun_dest = CodigoMunicipio(nf.NotaFiscalPessoaTomador.cidade).ToString();
                dTONfe.dest.enderDest.UF_dest = nf.NotaFiscalPessoaTomador.uf;
                dTONfe.dest.enderDest.CEP_dest = nf.NotaFiscalPessoaTomador.cep.Replace("-", "");
                dTONfe.dest.enderDest.cPais_dest = "1058";
                dTONfe.dest.enderDest.xPais_dest = "BRASIL";
                dTONfe.dest.enderDest.fone_dest = (nf.NotaFiscalPessoaTomador.fone == null) ? "" : nf.NotaFiscalPessoaTomador.fone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                                


                if (nf.NotaFiscalNFE.NotaFiscalNFERetirada != null)
                {
                    if (nf.NotaFiscalNFE.NotaFiscalNFERetirada.cnpjCPF != null)
                    {

                        dTONfe.retirada = new Migrate.NFe.DTO.retirada();

                        var cnpjCPFAux = UtilSoftFin.Limpastrings(nf.NotaFiscalNFE.NotaFiscalNFERetirada.cnpjCPF);

                        if (cnpjCPFAux.Length != 14)
                            dTONfe.retirada.CPF_ret = cnpjCPFAux;
                        else
                            dTONfe.retirada.CNPJ_ret = cnpjCPFAux;

                        dTONfe.retirada.cMun_ret = nf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio;
                        dTONfe.retirada.xMun_ret = ExtraiNomeMunicipio(nf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio);
                        dTONfe.retirada.UF_ret = ExtraiUF(nf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio);
                        dTONfe.retirada.nro_ret = nf.NotaFiscalNFE.NotaFiscalNFERetirada.numero;
                        dTONfe.retirada.xBairro_ret = nf.NotaFiscalNFE.NotaFiscalNFERetirada.bairro;
                        dTONfe.retirada.xCpl_ret = nf.NotaFiscalNFE.NotaFiscalNFERetirada.complemento;
                        dTONfe.retirada.xLgr_ret = nf.NotaFiscalNFE.NotaFiscalNFERetirada.endereco;
                    }
                }

                if (nf.NotaFiscalNFE.NotaFiscalNFEEntrega != null)
                {
                    if (nf.NotaFiscalNFE.NotaFiscalNFEEntrega.cnpjCPF != null)
                    {
                        dTONfe.entrega = new Migrate.NFe.DTO.entrega();

                        var cnpjCPFAux = UtilSoftFin.Limpastrings(nf.NotaFiscalNFE.NotaFiscalNFEEntrega.cnpjCPF);

                        if (cnpjCPFAux.Length != 14)
                            dTONfe.entrega.CPF_entr = cnpjCPFAux;
                        else
                            dTONfe.entrega.CNPJ_entr = cnpjCPFAux;

                        dTONfe.entrega.cMun_entr = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio;
                        dTONfe.entrega.xMun_entr = ExtraiNomeMunicipio(nf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio);
                        dTONfe.entrega.UF_entr = ExtraiUF(nf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio);
                        dTONfe.entrega.nro_entr = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.numero;
                        dTONfe.entrega.xBairro_entr = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.bairro;
                        dTONfe.entrega.xCpl_entr = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.complemento;
                        dTONfe.entrega.xLgr_entr = nf.NotaFiscalNFE.NotaFiscalNFEEntrega.endereco;
                    }
                }



                decimal valorTributos = 0;
                var nfsitem = new NotaFiscalNFEItem().ObterPorNf(nf.NotaFiscalNFE.id);
                var contador = 1;

                dTONfe.det = new Migrate.NFe.DTO.EnvioDet();
                dTONfe.det.detItem = new List<Migrate.NFe.DTO.EnvioDetDetItem>();
                
                foreach (var item in nfsitem)
                {
                    var det = new SoftFin.Migrate.NFe.DTO.EnvioDetDetItem();
                    det.prod = new Migrate.NFe.DTO.EnvioDetDetItemProd();

                    //det.nItem = contador.ToString();
                    det.prod.cProd = item.codigoProduto;
                    det.prod.cEAN = item.EAN;
                    det.prod.xProd = item.nomeProduto;
                    det.prod.NCM = item.NCM;
                    det.prod.CFOP = item.CFOP;
                    det.prod.uCOM = item.unidadeMedida;
                    det.prod.qCOM = item.quantidade.ToString();
                    det.prod.vUnCom = item.valorUnitario.ToString().Replace(",", ".");
                    det.prod.vProd = item.valor.ToString().Replace(",", ".");
                    det.prod.cEANTrib = item.EAN;
                    det.prod.uTrib = item.unidadeMedida;
                    det.prod.qTrib = item.quantidade.ToString();
                    det.prod.vDesc = item.desconto.ToString().Replace(",", ".");
                    det.prod.vUnTrib = item.valorUnitario.ToString().Replace(",", ".");
                    det.prod.vFrete = item.valorFrete.ToString().Replace(",", ".");
                    det.prod.vSeg = item.valorSeguro.ToString().Replace(",", ".");
                    det.prod.EXTIPI = item.EXTIPI.ToString().Replace(",", ".");
                    det.prod.indTot = "1";
                    det.prod.nItemPed = item.nItemPed;
                    det.prod.dProd = item.dProd;
                    det.prod.nRECOPI = item.nRECOPI;
                    det.prod.CEST = item.CEST;
                    det.prod.indEscala = item.indEscala;
                    det.prod.CNPJFab = item.CNPJFab;
                    det.prod.cBenef = item.cBenef;

                    det.imposto = new Migrate.NFe.DTO.EnvioDetDetItemImposto();
                    det.imposto.ICMS = new Migrate.NFe.DTO.EnvioDetDetItemImpostoICMS();



                    if (item.valorTributos != 0)
                        det.imposto.vTotTrib = item.valorTributos.ToString().Replace(",", ".");

                    det.imposto.ICMS.CST = item.CST;
                    det.imposto.ICMS.orig = item.origem;
                    det.imposto.ICMS.modBC = item.modBC;
                    det.imposto.ICMS.vBC = item.pRedBC.ToString().Replace(",", ".");
                    det.imposto.ICMS.pICMS = item.aliquotaICMS.Replace(",", "."); ;
                    //det.imposto.ICMS.vICMS_icms =item.valorICMS.ToString().Replace(",", ".");
                    //det.imposto.ICMS.modBCST = null;
                    //det.imposto.ICMS.pMVAST = "";
                    //det.imposto.ICMS.pRedBCST
                    //det.imposto.ICMS.vBCST
                    //det.imposto.ICMS.vBCSTRet
                    //det.imposto.ICMS.pICMSST
                    //det.imposto.ICMS.vICMSST_icms
                    //det.imposto.ICMS.vICMSSTRet
                    det.imposto.ICMS.pRedBC = item.pRedBC.ToString().Replace(",", ".");
                    //det.imposto.ICMS.motDesICMS = item.motDesICMS;
                    //det.imposto.ICMS.vICMSDeson = item.vICMSDeson;
                    //det.imposto.ICMS.vICMSOp
                    //det.imposto.ICMS.pDif
                    //det.imposto.ICMS.vICMSDif
                    //det.imposto.ICMS.pBCOp
                    //det.imposto.ICMS.UFST
                    //det.imposto.ICMS.vBCSTDest
                    //det.imposto.ICMS.vICMSSTDest_icms
                    //det.imposto.ICMS.pCredSN
                    //det.imposto.ICMS.vCredICMSSN
                    //det.imposto.ICMS.pFCP
                    //det.imposto.ICMS.vFCP
                    //det.imposto.ICMS.vBCFCP
                    //det.imposto.ICMS.vBCFCPST
                    //det.imposto.ICMS.pFCPST
                    //det.imposto.ICMS.vFCPST
                    //det.imposto.ICMS.pST
                    // det.imposto.ICMS.vBCFCPSTRet
                    // det.imposto.ICMS.pFCPSTRet
                    // det.imposto.ICMS.vFCPSTRet
                    //det.imposto.ICMS.GerarICMSST
                    // det.imposto.ICMS.pRedBCEfet
                    // det.imposto.ICMS.vBCEfet
                    // det.imposto.ICMS.pICMSEfet
                    // det.imposto.ICMS.vICMSEfet

                    det.imposto.PIS = new Migrate.NFe.DTO.EnvioDetDetItemImpostoPIS();
                    det.imposto.PIS.CST_pis = item.PISCST;
                    //det.Imposto.PIS.pPIS = item.aliquotaPIS.ToString().Replace(",", ".");
                    //det.Imposto.PIS.vBC = item.basePIS.ToString().Replace(",", ".");
                    //det.Imposto.PIS.vPIS = item.valorPIS.ToString().Replace(",", ".");

                    det.imposto.COFINS = new Migrate.NFe.DTO.EnvioDetDetItemImpostoCOFINS();
                    det.imposto.COFINS.CST_cofins = item.COFINSCST;
                    //det.Imposto.COFINS.pCOFINS = item.aliquotaCOFINS.ToString().Replace(",", ".");
                    //det.Imposto.COFINS.vBC = item.baseCOFINS.ToString().Replace(",", ".");
                    //det.Imposto.COFINS.vCOFINS = item.valorCOFINS.ToString().Replace(",", ".");

                    //if (item.valorTributos != 0)
                    //    det.Imposto.vTotTrib = item.valorTributos.ToString().Replace(",", ".");

                    //valorTributos += item.valorTributos;

                    //if (item.valorTributos != 0)
                    //    det.Prod.infAdProd += "Total aproximado de tributos: R$" + item.valorTributos.ToString() + " Fonte: IBPT";

                    //dTONfe.InfNFe.Det.Add(det);
                    dTONfe.det.detItem.Add(det);
                    //contador++;
                }

                dTONfe.total = new Migrate.NFe.DTO.EnvioTotal();
                dTONfe.total.ICMStot = new Migrate.NFe.DTO.EnvioTotalICMStot();

                dTONfe.total.ICMStot.vBC_ttlnfe = nf.NotaFiscalNFE.baseICMS.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vICMS_ttlnfe = nf.NotaFiscalNFE.valorICMS.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vICMS_ttlnfe = nf.NotaFiscalNFE.valorICMSDesonerado.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vBCST_ttlnfe = nf.NotaFiscalNFE.baseICMSST.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vST_ttlnfe = nf.NotaFiscalNFE.valorICMSST.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vProd_ttlnfe = nf.NotaFiscalNFE.valorProduto.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vFCP_ttlnfe = nf.NotaFiscalNFE.valorFrete.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vSeg_ttlnfe = nf.NotaFiscalNFE.valorSeguro.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vDesc_ttlnfe = nf.NotaFiscalNFE.valorDesconto.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vII_ttlnfe = nf.NotaFiscalNFE.valorII.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vIPI_ttlnfe = nf.NotaFiscalNFE.valorIPI.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vPIS_ttlnfe = nf.NotaFiscalNFE.valorPIS.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vCOFINS_ttlnfe = nf.NotaFiscalNFE.valorCONFINS.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vOutro = nf.NotaFiscalNFE.valorOutro.ToString().Replace(",", ".");
                dTONfe.total.ICMStot.vNF = nf.NotaFiscalNFE.valor.ToString().Replace(",", ".");
                //dTONfe.total.ICMStot.vTot = valorTributos.ToString().Replace(",", ".");

                //dTONfe.total.ICMStot.vFCP = (0).ToString().Replace(",", ".");// v4.0
                //dTONfe.total.ICMStot.vFCPST = (0).ToString().Replace(",", ".");// v4.0
                //dTONfe.total.ICMStot.vFCPSTRet = (0).ToString().Replace(",", ".");// v4.0
                //dTONfe.total.ICMStot.vIPIDevol = (0).ToString().Replace(",", ".");// v4.0



                dTONfe.transp = new Migrate.NFe.DTO.EnvioTransp();
                

                nf.NotaFiscalNFE.NotaFiscalNFETransportadora = new NotaFiscalNFETransportadora().ObterPorId(nf.NotaFiscalNFE.NotaFiscalNFETransportadora_id.Value);

                dTONfe.transp.modFrete = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.modalidadeFrete;



                //if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF != null)
                //{
                //    if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length == 14)
                //    {
                //        dTONfe.InfNFe.Transp.Transporta.CNPJ = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
                //    }
                //    if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length == 11)
                //    {
                //        dTONfe.InfNFe.Transp.Transporta.CPF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
                //    }
                //    dTONfe.InfNFe.Transp.Transporta.CPF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
                //    dTONfe.InfNFe.Transp.Transporta.xNome = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.nomeRazao;
                //    if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE != null)
                //    {
                //        if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE.ToUpper() == "ISENTA")
                //        {
                //            nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE = "ISENTO";
                //        }
                //        else
                //        {
                //            nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE.ToString();

                //        }
                //    }

                //    dTONfe.InfNFe.Transp.Transporta.xEnder = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.EnderecoCompleto;
                //    dTONfe.InfNFe.Transp.Transporta.xMun = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.cidade;
                //    dTONfe.InfNFe.Transp.Transporta.UF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.uf;
                //}

                //if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora.ICMSRetido != 0)
                //{
                //    dTONfe.InfNFe.Transp.RetTransp.vServ = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.valorServico.ToString().Replace(",", ".");
                //    dTONfe.InfNFe.Transp.RetTransp.vBCRet = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.baseCalculo.ToString().Replace(",", ".");
                //    dTONfe.InfNFe.Transp.RetTransp.pICMSRet = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.aliquota.ToString().Replace(",", ".");
                //    dTONfe.InfNFe.Transp.RetTransp.vICMSRet = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.ICMSRetido.ToString().Replace(",", ".");
                //    dTONfe.InfNFe.Transp.RetTransp.CFOP = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.CFOP;
                //    dTONfe.InfNFe.Transp.RetTransp.cMunFG = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.codigoMunicipioOcorrencia;
                //}

                //dTONfe.InfNFe.Transp.VeicTransp.placa = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.placa;
                //dTONfe.InfNFe.Transp.VeicTransp.RNTC = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.RNTC;
                //dTONfe.InfNFe.Transp.VeicTransp.UF = nf.NotaFiscalNFE.NotaFiscalNFETransportadora.ufplaca;

                //nf.NotaFiscalNFE.NotaFiscalNFEVolume = new NotaFiscalNFEVolume().ObterPorNf(nf.NotaFiscalNFE.id);
                //foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEVolume)
                //{
                //    var itemVolume = new SoftFin.NFe.DTO.DTOVol
                //    {
                //        esp = item.especie,
                //        marca = item.marca,
                //        nVol = item.numeracao,
                //        pesoB = item.pesoBruto.ToString().Replace(",", "."),
                //        pesoL = item.pesoLiquido.ToString().Replace(",", "."),
                //        qVol = item.qtde.ToString()

                //    };

                //    var lks = item.lacres.Split(';');

                //    foreach (var itemLks in lks)
                //    {
                //        itemVolume.lacres.Add(new SoftFin.NFe.DTO.DTOLacres { nLacre = itemLks });
                //    }
                //    dTONfe.InfNFe.Transp.Vol.Add(itemVolume);
                //}

                //dTONfe.InfNFe.Cobr.Fat.nFat = nf.NotaFiscalNFE.faturaNumero;
                //dTONfe.InfNFe.Cobr.Fat.vDesc = nf.NotaFiscalNFE.ToString().Replace(",", ".");
                //dTONfe.InfNFe.Cobr.Fat.vLiq = nf.NotaFiscalNFE.ToString().Replace(",", ".");
                //dTONfe.InfNFe.Cobr.Fat.vOrig = nf.NotaFiscalNFE.ToString().Replace(",", ".");

                //nf.NotaFiscalNFE.NotaFiscalNFEDuplicatas = new NotaFiscalNFEDuplicata().ObterPorNf(nf.NotaFiscalNFE.id);

                //foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEDuplicatas)
                //{
                //    dTONfe.InfNFe.Cobr.Dup.Add(new SoftFin.NFe.DTO.DTODup
                //    {
                //        dVenc = item.vencto.ToString("yyyy-MM-yy"),
                //        nDup = item.numero.ToString(),
                //        vDup = item.valor.ToString().Replace(",", ".")
                //    });
                //}

                
                dTONfe.pag = new Migrate.NFe.DTO.EnvioPag();
                foreach (var item in nf.NotaFiscalNFE.NotaFiscalNFEFormaPagamentos)
                {
                    var dtoPG = new Migrate.NFe.DTO.EnvioPagPagItem();

                    if (item.indPag != null)
                        dtoPG.indPag_pag = item.indPag.ToString();

                    if (item.tPag != null)
                        dtoPG.tPag = item.tPag.Value.ToString("00");

                    if (item.vPag != 0)
                        dtoPG.vPag = item.vPag.ToString().Replace(",", ".");

                    if (item.vTroco != null)
                        dtoPG.vTroco = item.vTroco.Value.ToString().Replace(",", ".");

                    dTONfe.pag.pagItem.Add(dtoPG);
                }

                dTONfe.infAdic = new Migrate.NFe.DTO.EnvioInfAdic();

                dTONfe.infAdic.infCpl = nf.NotaFiscalNFE.informacaoComplementar;
                dTONfe.infAdic.infAdFisco = nf.NotaFiscalNFE.informacaoComplementarFisco;
                
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


    }

}