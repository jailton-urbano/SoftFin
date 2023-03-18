using SoftFin.NFSe.DTO;
using SoftFin.NFSe.SaoPaulo.InterfaceService;
using SoftFin.NFSe.SaoPaulo.Utils;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SoftFin.NFSe.SaoPaulo.Bussiness
{
    public class EnviarLoteRPS
    {

        public DTORetornoNFEs Executar(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador)
        {
            return GeraArquivoLote(redidoEnvioLoteRPS, certificadoPrestador);
        }


        private DTORetornoNFEs GeraArquivoLote(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador)
        {
            XmlDocument nfes = GeraXMLNFSe(redidoEnvioLoteRPS, certificadoPrestador);
            XmlDocument retornoAssinado = new SoftFin.NFSe.SaoPaulo.Utils.AssinaturaDigital().AplicaAssinatura(
                                                        nfes, "PedidoEnvioLoteRPS", certificadoPrestador);
            //nfes.Save(@"C:\lixo\NfEnvio.xml");
            var retornoEnvioLoteRPS = EnviarPeloWebServico(certificadoPrestador, nfes);
            
            var retorno =  ProcessaRetornoXML(retornoEnvioLoteRPS);
            retorno.xml = retornoAssinado.InnerXml;

            return retorno;
       }

        private static string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            var loteNFe = new LoteNFe();
            loteNFe.ClientCertificates.Add(certificadoPrestador);

            string retornoEnvioLoteRPS = "";
            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                retornoEnvioLoteRPS = loteNFe.EnvioLoteRPS(1, nfes.OuterXml);
            }
            else
            {
                retornoEnvioLoteRPS = loteNFe.TesteEnvioLoteRPS(1, nfes.OuterXml);
            }
            return retornoEnvioLoteRPS;
        }

        private DTORetornoNFEs ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(GenerateStreamFromString(retornoEnvioLoteRPS));
            XmlNodeList procEventoNFe = xmlDocument.GetElementsByTagName("RetornoEnvioLoteRPS");
            XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Cabecalho");
            XmlNodeList erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Erro");
            XmlNodeList alertas = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Alerta");

            var resultingMessage = new DTORetornoNFEs();

            resultingMessage.Cabecalho.Sucesso = cabecalho[0].ChildNodes[0].InnerText;

            foreach (XmlElement item in erros)
            {
                resultingMessage.Erro.Add(new TPErro
                {
                    Codigo = item.ChildNodes[0].InnerText,
                    Descricao = item.ChildNodes[1].InnerText
                });
            }
            foreach (XmlElement item in alertas)
            {
                resultingMessage.Alerta.Add(new tpAlerta
                {
                    Codigo = item.ChildNodes[0].InnerText,
                    Descricao = item.ChildNodes[1].InnerText
                });
            }
            return resultingMessage;
        }

        private XmlDocument GeraXMLNFSe(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador)
        {
            List<int> nfs = new List<int>();

            var pedidoEnvioLoteRPS = new DTONotaFiscal();
            var listaRps = new tpNFSe();

            XmlDocument nfes = new XmlDocument();
            XmlDeclaration xml_declaration = nfes.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = nfes.DocumentElement;
            nfes.InsertBefore(xml_declaration, document_element);
            XmlNode nodePedidoEnvioLoteRPS = nfes.CreateElement("PedidoEnvioLoteRPS");

            XmlAttribute attribute = nfes.CreateAttribute("xmlns:xsi");
            attribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute);

            XmlAttribute attribute2 = nfes.CreateAttribute("xmlns:xsd");
            attribute2.Value = "http://www.w3.org/2001/XMLSchema";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute2);

            XmlAttribute attribute3 = nfes.CreateAttribute("xmlns");
            attribute3.Value = "http://www.prefeitura.sp.gov.br/nfe";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute3);

            var nodeCabecalho = nfes.CreateElement("Cabecalho");
            nodePedidoEnvioLoteRPS.AppendChild(nodeCabecalho);
            nfes.AppendChild(nodePedidoEnvioLoteRPS);


            foreach (var itemNF in redidoEnvioLoteRPS.NFSe)
            {
                XmlNode nodeRPS = nfes.CreateElement("RPS");

                XmlAttribute RPSxmlnsattribute = nfes.CreateAttribute("xmlns");
                RPSxmlnsattribute.Value = "";
                nodeRPS.Attributes.Append(RPSxmlnsattribute);


                var nodeAssinatura = nfes.CreateElement("Assinatura");
                nodeRPS.AppendChild(nodeAssinatura);
                var nodeChaveRPS = nfes.CreateElement("ChaveRPS");

                var nodeInscricaoPrestador = nfes.CreateElement("InscricaoPrestador");
                nodeInscricaoPrestador.InnerText = itemNF.ChaveRPS.InscricaoPrestador;
                nodeChaveRPS.AppendChild(nodeInscricaoPrestador);

                var nodeSerieRPS = nfes.CreateElement("SerieRPS");
                nodeSerieRPS.InnerText = itemNF.ChaveRPS.SerieRPS;
                nodeChaveRPS.AppendChild(nodeSerieRPS);

                var nodeNumeroRPS = nfes.CreateElement("NumeroRPS");
                nodeNumeroRPS.InnerText = itemNF.ChaveRPS.NumeroRPS;
                nodeChaveRPS.AppendChild(nodeNumeroRPS);

                nodeRPS.AppendChild(nodeChaveRPS);

                PreecheNode(nfes, nodeRPS, "TipoRPS", itemNF.TipoRPS);
                PreecheNode(nfes, nodeRPS, "DataEmissao", itemNF.DataEmissaoRPS);
                PreecheNode(nfes, nodeRPS, "StatusRPS", itemNF.StatusRPS);
                PreecheNode(nfes, nodeRPS, "TributacaoRPS", itemNF.TributacaoRPS);
                PreecheNode(nfes, nodeRPS, "ValorServicos", itemNF.ValorServicos);
                PreecheNode(nfes, nodeRPS, "ValorDeducoes",itemNF.ValorDeducoes);
                PreecheNode(nfes, nodeRPS, "ValorPIS", itemNF.ValorPIS);
                PreecheNode(nfes, nodeRPS, "ValorCOFINS", itemNF.ValorCOFINS);
                PreecheNode(nfes, nodeRPS, "ValorINSS", itemNF.ValorINSS);
                PreecheNode(nfes, nodeRPS, "ValorIR", itemNF.ValorIR);
                PreecheNode(nfes, nodeRPS, "ValorCSLL", itemNF.ValorCSLL);
                PreecheNode(nfes, nodeRPS, "CodigoServico", itemNF.CodigoServicos);
                PreecheNode(nfes, nodeRPS, "AliquotaServicos", itemNF.AliquotaServicos);
                PreecheNode(nfes, nodeRPS, "ISSRetido", itemNF.ISSRetido);

                var nodeCPFCNPJTomador = nfes.CreateElement("CPFCNPJTomador");
                if (!string.IsNullOrEmpty(itemNF.CPFCNPJTomador.CNPJ))
                {
                    PreecheNode(nfes, nodeCPFCNPJTomador, "CNPJ", itemNF.CPFCNPJTomador.CNPJ);
                }
                else
                {
                    PreecheNode(nfes, nodeCPFCNPJTomador, "CPF", itemNF.CPFCNPJTomador.CPF);
                }
                nodeRPS.AppendChild(nodeCPFCNPJTomador);

                PreecheNode(nfes, nodeRPS, "RazaoSocialTomador", itemNF.RazaoSocialTomador);
                var nodeEnderecoTomador = nfes.CreateElement("EnderecoTomador");
                PreecheNode(nfes, nodeEnderecoTomador, "TipoLogradouro", itemNF.EnderecoTomador.TipoLogradouro);
                PreecheNode(nfes, nodeEnderecoTomador, "Logradouro", itemNF.EnderecoTomador.Logradouro);
                PreecheNode(nfes, nodeEnderecoTomador, "NumeroEndereco", itemNF.EnderecoTomador.NumeroEndereco);
                PreecheNode(nfes, nodeEnderecoTomador, "ComplementoEndereco", itemNF.EnderecoTomador.ComplementoEndereco);
                PreecheNode(nfes, nodeEnderecoTomador, "Bairro", itemNF.EnderecoTomador.Bairro);
                PreecheNode(nfes, nodeEnderecoTomador, "Cidade", itemNF.EnderecoTomador.CodigoMunicipio);
                PreecheNode(nfes, nodeEnderecoTomador, "UF", itemNF.EnderecoTomador.UF);
                PreecheNode(nfes, nodeEnderecoTomador, "CEP", itemNF.EnderecoTomador.CEP);
                nodeRPS.AppendChild(nodeEnderecoTomador);
                PreecheNode(nfes, nodeRPS, "EmailTomador", itemNF.EmailTomador);
                PreecheNode(nfes, nodeRPS, "Discriminacao", itemNF.Discriminacao.Replace("\n", "|").Replace("\r", ""));

                if (!string.IsNullOrEmpty(itemNF.ValorCargaTributaria))
                {
                    PreecheNode(nfes, nodeRPS, "ValorCargaTributaria", itemNF.ValorCargaTributaria);
                }

                if (!string.IsNullOrEmpty(itemNF.PercentualCargaTributaria))
                {
                    PreecheNode(nfes, nodeRPS, "PercentualCargaTributaria", itemNF.PercentualCargaTributaria);
                }

                if (!string.IsNullOrEmpty(itemNF.FonteCargaTributaria))
                {
                    PreecheNode(nfes, nodeRPS, "FonteCargaTributaria", itemNF.FonteCargaTributaria);
                }

                if (!string.IsNullOrEmpty(itemNF.CodigoCEI))
                {
                    PreecheNode(nfes, nodeRPS, "CodigoCEI", itemNF.CodigoCEI);
                }
                if (!string.IsNullOrEmpty(itemNF.MatriculaObra))
                {
                    PreecheNode(nfes, nodeRPS, "MatriculaObra", itemNF.MatriculaObra);
                }


                nodePedidoEnvioLoteRPS.AppendChild(nodeRPS);

                var assinatura = GeraAssinaturaRPS(certificadoPrestador, itemNF);

                nodeAssinatura.InnerText = assinatura;
                nodePedidoEnvioLoteRPS.AppendChild(nodeRPS);
            }

            //PedidoEnvioLoteRPS.xsd

            var nodeCPFCNPJRemetente = nfes.CreateElement("CPFCNPJRemetente");
            if (!string.IsNullOrEmpty(redidoEnvioLoteRPS.Cabecalho.CPFCNPJRemetente.CNPJ))
            {
                PreecheNode(nfes, nodeCPFCNPJRemetente, "CNPJ", UtilSoftFin.Limpastrings(redidoEnvioLoteRPS.Cabecalho.CPFCNPJRemetente.CNPJ));
            }
            else
            {
                PreecheNode(nfes, nodeCPFCNPJRemetente, "CPF", UtilSoftFin.Limpastrings(redidoEnvioLoteRPS.Cabecalho.CPFCNPJRemetente.CPF));
            }
            nodeCabecalho.AppendChild(nodeCPFCNPJRemetente);
            PreecheNode(nfes, nodeCabecalho, "transacao", redidoEnvioLoteRPS.Cabecalho.transacao);
            PreecheNode(nfes, nodeCabecalho, "dtInicio", redidoEnvioLoteRPS.Cabecalho.dtInicio);
            PreecheNode(nfes, nodeCabecalho, "dtFim", redidoEnvioLoteRPS.Cabecalho.dtFim);
            PreecheNode(nfes, nodeCabecalho, "QtdRPS", redidoEnvioLoteRPS.NFSe.Count().ToString());
            PreecheNode(nfes, nodeCabecalho, "ValorTotalServicos", redidoEnvioLoteRPS.Cabecalho.ValorTotalServicos);
            PreecheNode(nfes, nodeCabecalho, "ValorTotalDeducoes", redidoEnvioLoteRPS.Cabecalho.ValorTotalDeducoes);
            PreecheAtributo(nfes, nodeCabecalho, "Versao", "1");
            PreecheAtributo(nfes, nodeCabecalho, "xmlns", "");



            return nfes;
        }

        private string GeraAssinaturaRPS(X509Certificate2 certificadoPrestador, tpNFSe itemNF)
        {
            StringBuilder hashAssinatura = new StringBuilder();
            hashAssinatura.Append(itemNF.ChaveRPS.InscricaoPrestador.PadRight(8, '0')); //1- Inscrição Municipal do Prestador
            hashAssinatura.Append(itemNF.ChaveRPS.SerieRPS.PadRight(5, ' ')); // 2= Série do RPS
            hashAssinatura.Append(itemNF.ChaveRPS.NumeroRPS.PadLeft(12, '0')); //3- Número do RPS
            hashAssinatura.Append(UtilSoftFin.Limpastrings(itemNF.DataEmissao)); //4 - Data de Emissão do RPS
            hashAssinatura.Append(itemNF.TributacaoRPS);//5 - Tipo de Tributação do RPS
            hashAssinatura.Append(itemNF.StatusRPS); //6 - Status do RPS
            if (itemNF.ISSRetido.ToLower().Equals("false"))
                hashAssinatura.Append("N"); //7 - ISS Retido
            else
                hashAssinatura.Append("S"); //7 - ISS Retido

            hashAssinatura.Append(itemNF.ValorServicos.Replace(".","").PadLeft(15, '0'));//8 - Valor dos Serviços
            hashAssinatura.Append(itemNF.ValorDeducoes.Replace(".", "").PadLeft(15, '0'));//9 - Valor das Deduções
            hashAssinatura.Append(itemNF.CodigoServicos);//10 - Código do Serviço Prestado 
            if (!string.IsNullOrEmpty(itemNF.CPFCNPJTomador.CNPJ))
            {
                hashAssinatura.Append("2");//11 - Indicador de CPF/CNPJ do Tomador 
                hashAssinatura.Append(itemNF.CPFCNPJTomador.CNPJ);//12 - CPF/CNPJ do Tomador
            }
            else{
                hashAssinatura.Append("1");//11 - Indicador de CPF/CNPJ do Tomador 
                hashAssinatura.Append("000" + itemNF.CPFCNPJTomador.CPF);//12 - CPF/CNPJ do Tomador
            }
              


            //hashAssinatura.Append("2");//13 - Indicador de CPF/CNPJ do Intermediário 
            //hashAssinatura.Append(dadosEstab.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""));//14 - CPF/CNPJ do Intermediário CPF 
            //hashAssinatura.Append("N");//15 - SS Retido Intermediário 

            var assinatura = new Assinador().AssinarRPSSP(certificadoPrestador, hashAssinatura.ToString());
            return assinatura;
        }


        #region Métodos Auxiliares
        private string FormataNumero(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", ".");
        }
        private string FormataNumero2(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", "");
        }
        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private static void PreecheNode(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value)
        {
            var node = xmlDocument.CreateElement(nameElement);
            node.InnerText = value;
            nodeRPS.AppendChild(node);
        }
        private static void PreecheAtributo(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value)
        {
            var attribute = xmlDocument.CreateAttribute(nameElement);
            attribute.Value = value;
            nodeRPS.Attributes.Append(attribute);
        }
        #endregion
    }
}
