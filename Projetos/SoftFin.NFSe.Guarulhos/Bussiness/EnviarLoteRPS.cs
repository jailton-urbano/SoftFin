using SoftFin.NFSe.DTO;

using SoftFin.NFSe.Guarulhos.InterfaceService;
using SoftFin.NFSe.Guarulhos.Utils;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SoftFin.NFSe.Guarulhos.Bussiness
{
    public class EnviarLoteRPS
    {

        public DTORetornoNFEs Executar(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador, string arquivoxml)
        {
            return GeraArquivoLote(redidoEnvioLoteRPS, certificadoPrestador, arquivoxml);
        }


        public static XmlDocument AssinaLoteRpsDigitalmente(XmlDocument doc, X509Certificate2 Cert)
        {
            //Cria um novo ArquivoXml para manusea-lo
            //XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            //doc.LoadXml(xml);

            //Pega o certificado válido para usar na assinatura do xml

            string x = Cert.GetKeyAlgorithm();

            //Cria um objeto de assinatura do arquivo
            SignedXml signedXml = new SignedXml(doc);

            //Adiciona a chave do certificado no objeto de assinatura do arquivo
            signedXml.SigningKey = Cert.PrivateKey;

            //Cria um objeto de referencia para assinatura
            Reference reference = new Reference();
            //Pega os atributos da tag LoteRps
            XmlAttributeCollection _Uri = doc.GetElementsByTagName("LoteRps").Item(0).Attributes;
            foreach (XmlAttribute _atributo in _Uri)
            {
                if (_atributo.Name == "Id")
                {
                    //http://reference.Uri = "#" + _atributo.InnerText; Comentada para realizar a integração.
                    reference.Uri = "";
                }
            }

            //Adiciona o envelope de transformação da referencia
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
            reference.AddTransform(c14);

            //Adiciona a referencia no objeto de assinatura
            signedXml.AddReference(reference);

            //Cria um objeto KeyInfo
            KeyInfo keyInfo = new KeyInfo();

            //Adiciona o certificado no objeto KeyInfo
            keyInfo.AddClause(new KeyInfoX509Data(Cert));

            //Adiciona o objeto KeyInfo no objeto de assinatura do arquivo
            signedXml.KeyInfo = keyInfo;

            //"Computa" a assinatura
            signedXml.ComputeSignature();

            //Cria um elemento do arquivo xml com a assinatura do arquivo
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            //Adiciona a tag de assinatura no Documento Xml
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            return doc;

        }




        private DTORetornoNFEs GeraArquivoLote(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador, string arquivoxml)
        {
            XmlDocument nfes = GeraXMLNFSe(redidoEnvioLoteRPS, certificadoPrestador);


            
        

            XmlDocument retornoAssinado = new SoftFin.NFSe.Guarulhos.Utils.AssinaturaDigital().AplicaAssinatura(
                                                        nfes, "tipos:InfRps","tipos:Rps", certificadoPrestador);
            retornoAssinado = new SoftFin.NFSe.Guarulhos.Utils.AssinaturaDigital().AplicaAssinatura(
                                                        nfes, "LoteRps", "EnviarLoteRpsEnvio", certificadoPrestador);

            //retornoAssinado = AssinaLoteRpsDigitalmente(
            //                                retornoAssinado, certificadoPrestador);

            //if (!string.IsNullOrEmpty(arquivoxml))
            //{
            //    nfes.Save(arquivoxml);
            //}

            var retornoEnvioLoteRPS = EnviarPeloWebServico(certificadoPrestador, nfes);
            


            var retornofinal = ProcessaRetornoXML(retornoEnvioLoteRPS);
            retornofinal.xml = retornoAssinado.InnerXml;
            return retornofinal;

            //return new DTORetornoEnvioLoteRPS();
       }

        private static string EnviarPeloWebServico(X509Certificate2 cert, XmlDocument nfes)
        {
            System.ServiceModel.BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            EndpointAddress endPoint = new EndpointAddress("https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
            NFEsGinfes.ServiceGinfesImplClient servico = new NFEsGinfes.ServiceGinfesImplClient(binding, endPoint);
            servico.ClientCredentials.ClientCertificate.Certificate = cert;

            var cabecalho = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            cabecalho += "<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\">";
            cabecalho += "<versaoDados>3</versaoDados>";
            cabecalho += "</ns2:cabecalho>";





            var retornoEnvioLoteRPS = servico.RecepcionarLoteRpsV3(cabecalho, nfes.OuterXml);
            return retornoEnvioLoteRPS;
                
                
            //var loteNFe = new LoteNFe();
            //loteNFe.ClientCertificates.Add(certificadoPrestador);
            //var retornoEnvioLoteRPS = loteNFe.EnvioLoteRPS(1, nfes.OuterXml);
            //return retornoEnvioLoteRPS;
        }


        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    break;
            }

        }

        private DTORetornoNFEs ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(GenerateStreamFromString(retornoEnvioLoteRPS));
            XmlNodeList procEventoNFe = xmlDocument.GetElementsByTagName("ListaMensagemRetorno");
            //XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Cabecalho");
            XmlNodeList erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("ns2:MensagemRetorno");
            //XmlNodeList alertas = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Alerta");

            var resultingMessage = new DTORetornoNFEs();

            //resultingMessage.Cabecalho.Sucesso = cabecalho[0].ChildNodes[0].InnerText;

            foreach (XmlElement item in erros)
            {
                resultingMessage.Erro.Add(new TPErro
                {
                    Codigo = item.ChildNodes[0].InnerText,
                    Descricao = item.ChildNodes[1].InnerText + " " + item.ChildNodes[2].InnerText
                });
            }


            resultingMessage.Cabecalho.Sucesso = (resultingMessage.Erro.Count() == 0).ToString();

            //foreach (XmlElement item in alertas)
            //{
            //    resultingMessage.Alerta.Add(new tpAlerta
            //    {
            //        Codigo = item.ChildNodes[0].InnerText,
            //        Descricao = item.ChildNodes[1].InnerText
            //    });
            //}
            return resultingMessage;
        }

        private XmlDocument GeraXMLNFSe(DTONotaFiscal pedidoEnvioLoteRPS, X509Certificate2 certificadoPrestador)
        {
            List<int> nfs = new List<int>();


            var listaRps = new tpNFSe();

            XmlDocument nfes = new XmlDocument();
            XmlDeclaration xml_declaration = nfes.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = nfes.DocumentElement;
            nfes.InsertBefore(xml_declaration, document_element);
            XmlNode nodePedidoEnvioLoteRPS = nfes.CreateElement("EnviarLoteRpsEnvio");


            XmlAttribute attribute3 = nfes.CreateAttribute("xmlns");
            attribute3.Value = "http://www.ginfes.com.br/servico_enviar_lote_rps_envio_v03.xsd";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute3);


            XmlAttribute attribute2 = nfes.CreateAttribute("xmlns:n2");
            attribute2.Value = "http://www.altova.com/samplexml/other-namespace";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute2);

            XmlAttribute attribute5 = nfes.CreateAttribute("xmlns:tipos");
            attribute5.Value = "http://www.ginfes.com.br/tipos_v03.xsd";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute5);

            XmlAttribute attribute4 = nfes.CreateAttribute("xmlns:dsig");
            attribute4.Value = "http://www.w3.org/2000/09/xmldsig#";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute4);

            XmlAttribute attribute = nfes.CreateAttribute("xmlns:xsi");
            attribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute);

            var nodeCabecalho = nfes.CreateElement("LoteRps");
            nodePedidoEnvioLoteRPS.AppendChild(nodeCabecalho);
            nfes.AppendChild(nodePedidoEnvioLoteRPS);

            XmlAttribute RPSxmlnsattribute = nfes.CreateAttribute("Id");
            RPSxmlnsattribute.Value = "NFES" + pedidoEnvioLoteRPS.NFSe.First().ChaveRPS.NumeroRPS;
            nodeCabecalho.Attributes.Append(RPSxmlnsattribute);

            PreecheNode(nfes, nodeCabecalho, "NumeroLote", pedidoEnvioLoteRPS.NFSe.First().ChaveRPS.NumeroRPS);
            PreecheNode(nfes, nodeCabecalho, "Cnpj", pedidoEnvioLoteRPS.NFSe.First().CnpjPrestador.Replace("/", "").Replace(".", "").Replace("-", ""));
            PreecheNode(nfes, nodeCabecalho, "InscricaoMunicipal", pedidoEnvioLoteRPS.NFSe.First().CPFCNPJTomador.InscricaoMunicipal);
            PreecheNode(nfes, nodeCabecalho, "QuantidadeRps", pedidoEnvioLoteRPS.NFSe.Count().ToString());
            var nodeListaRps = PreecheNode2(nfes, nodeCabecalho, "ListaRps");


            foreach (var itemNF in pedidoEnvioLoteRPS.NFSe)
            {
                XmlNode nodeRPS = nfes.CreateElement("tipos", "Rps", "http://www.ginfes.com.br/tipos_v03.xsd");
                nodeListaRps.AppendChild(nodeRPS);

                var nodetiposInfRps = nfes.CreateElement("tipos", "InfRps", "http://www.ginfes.com.br/tipos_v03.xsd");
                nodeRPS.AppendChild(nodetiposInfRps);

                XmlAttribute RPSid = nfes.CreateAttribute("Id");
                RPSid.Value = "NFE" + pedidoEnvioLoteRPS.NFSe.First().ChaveRPS.NumeroRPS;
                nodetiposInfRps.Attributes.Append(RPSid);


                var nodetiposIdentificacaoRps = PreecheNode2(nfes, nodetiposInfRps, "IdentificacaoRps"); 

                PreecheNode(nfes, nodetiposIdentificacaoRps, "Numero", itemNF.ChaveRPS.NumeroRPS);
                PreecheNode(nfes, nodetiposIdentificacaoRps, "Serie", itemNF.ChaveRPS.SerieRPS);
                PreecheNode(nfes, nodetiposIdentificacaoRps, "Tipo", "1");

                PreecheNode(nfes, nodetiposInfRps, "DataEmissao", itemNF.DataEmissao + "T12:00:00");
                PreecheNode(nfes, nodetiposInfRps, "NaturezaOperacao", itemNF.NaturezaOperaco);
                PreecheNode(nfes, nodetiposInfRps, "OptanteSimplesNacional", ((itemNF.OptanteSimplesNacional == "0") ? "2" : "1")); //itemNF.OptanteSimplesNacional == 0 não
                PreecheNode(nfes, nodetiposInfRps, "IncentivadorCultural", "2");
                PreecheNode(nfes, nodetiposInfRps, "Status", "1"); //'Normal'

                var nodetiposServico = PreecheNode2(nfes, nodetiposInfRps, "Servico");
                var nodetiposValores = PreecheNode2(nfes, nodetiposServico, "Valores");
                PreecheNode(nfes, nodetiposValores, "ValorServicos", itemNF.ValorServicos);
                PreecheNode(nfes, nodetiposValores, "ValorDeducoes", itemNF.ValorDeducoes);
                PreecheNode(nfes, nodetiposValores, "ValorPis", itemNF.ValorPIS);
                PreecheNode(nfes, nodetiposValores, "ValorCofins", itemNF.ValorCOFINS);
                PreecheNode(nfes, nodetiposValores, "ValorInss", itemNF.ValorINSS);
                PreecheNode(nfes, nodetiposValores, "ValorIr", itemNF.ValorIR);
                PreecheNode(nfes, nodetiposValores, "ValorCsll", itemNF.ValorCSLL);
                PreecheNode(nfes, nodetiposValores, "IssRetido", (itemNF.ISSRetido.ToUpper() == "FALSE")? "2": "1" );
                PreecheNode(nfes, nodetiposValores, "ValorIss", itemNF.ValorIss);
                PreecheNode(nfes, nodetiposValores, "ValorIssRetido", itemNF.ValorIssRetido);
                PreecheNode(nfes, nodetiposValores, "OutrasRetencoes", itemNF.valorOutrasDeducoes);
                PreecheNode(nfes, nodetiposValores, "BaseCalculo", itemNF.BaseCalculo);
                PreecheNode(nfes, nodetiposValores, "Aliquota", itemNF.AliquotaServicos);
                PreecheNode(nfes, nodetiposValores, "ValorLiquidoNfse", itemNF.ValorLiquidoNfse);


                PreecheNode(nfes, nodetiposServico, "ItemListaServico", itemNF.CodigoServicos); //itemNF.ItemListaServico);
                PreecheNode(nfes, nodetiposServico, "CodigoTributacaoMunicipio", itemNF.CodigoTributacaoMunicipio);
                PreecheNode(nfes, nodetiposServico, "Discriminacao", itemNF.Discriminacao);
                PreecheNode(nfes, nodetiposServico, "CodigoMunicipio", itemNF.CodigoMunicipioPrestador);

                var nodetiposPrestador = PreecheNode2(nfes, nodetiposInfRps, "Prestador");

                PreecheNode(nfes, nodetiposPrestador, "Cnpj", itemNF.CnpjPrestador.Replace(".", "").Replace("/", "").Replace("-", ""));
                PreecheNode(nfes, nodetiposPrestador, "InscricaoMunicipal", itemNF.InscricaoMunicipalPretador);

                var nodeTomador = PreecheNode2(nfes, nodetiposInfRps, "Tomador");
                var nodeIdentificacaoTomador = PreecheNode2(nfes, nodeTomador, "IdentificacaoTomador");
                var nodeCpfCnpj = PreecheNode2(nfes, nodeIdentificacaoTomador, "CpfCnpj");
                if (string.IsNullOrEmpty(itemNF.CPFCNPJTomador.CPF))
                    PreecheNode(nfes, nodeCpfCnpj, "Cnpj", itemNF.CPFCNPJTomador.CNPJ.Replace(".", "").Replace("/", "").Replace("-", ""));
                else
                    PreecheNode(nfes, nodeCpfCnpj, "Cpf", itemNF.CPFCNPJTomador.CPF);
                PreecheNode(nfes, nodeTomador, "RazaoSocial", itemNF.CPFCNPJTomador.InscricaoMunicipal);
                var nodetiposEndereco = PreecheNode2(nfes, nodetiposInfRps, "Endereco");

                nodeTomador.AppendChild(nodetiposEndereco);
                PreecheNode(nfes, nodetiposEndereco, "Endereco", itemNF.EnderecoTomador.Logradouro);
                PreecheNode(nfes, nodetiposEndereco, "Numero", itemNF.EnderecoTomador.NumeroEndereco);
                PreecheNode(nfes, nodetiposEndereco, "Bairro", itemNF.EnderecoTomador.Bairro);
                PreecheNode(nfes, nodetiposEndereco, "CodigoMunicipio", itemNF.CodigoMunicipioPrestador);
                PreecheNode(nfes, nodetiposEndereco, "Uf", itemNF.EnderecoTomador.UF);
                PreecheNode(nfes, nodetiposEndereco, "Cep", itemNF.EnderecoTomador.CEP);
            }

            return nfes;
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
        private XmlElement PreecheNode(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value)
        {
            var node = xmlDocument.CreateElement("tipos", nameElement, "http://www.ginfes.com.br/tipos_v03.xsd");
            node.InnerText = value;
            nodeRPS.AppendChild(node);
            return node;
        }


        private XmlElement PreecheNode2(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement)
        {
            var node = xmlDocument.CreateElement("tipos", nameElement, "http://www.ginfes.com.br/tipos_v03.xsd");

            nodeRPS.AppendChild(node);
            return node;
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
