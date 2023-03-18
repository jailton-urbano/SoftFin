using SoftFin.NFSe.DTO;
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

namespace SoftFin.NFSe.Guarulhos.Bussiness
{
    public class CancelaNFe
    {
        public DTORetornoNFEs Execute(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador, string arquivoxml)
        {
            return GeraCancelamento(redidoEnvioLoteRPS, certificadoPrestador, arquivoxml);
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

        private DTORetornoNFEs GeraCancelamento(DTONotaFiscal redidoEnvioLoteRPS, X509Certificate2 certificadoPrestador, string arquivoxml)
        {
            XmlDocument nfes = GeraXMLNFSe(redidoEnvioLoteRPS, certificadoPrestador);
            XmlDocument retornoAssinado = AplicaAssinatura2(
                                                        nfes, "p2:CancelarNfseEnvio", certificadoPrestador);



            //if (!string.IsNullOrEmpty(arquivoxml))
            //{
            //    nfes.Save(arquivoxml);
            //}

            //nfes.Save(@"c:\lixo\nfguacanc.xml");

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

            var retornoEnvioLoteRPS = servico.CancelarNfseV3(cabecalho, nfes.OuterXml);
            return retornoEnvioLoteRPS;


            //var loteNFe = new LoteNFe();
            //loteNFe.ClientCertificates.Add(certificadoPrestador);
            //var retornoEnvioLoteRPS = loteNFe.EnvioLoteRPS(1, nfes.OuterXml);
            //return retornoEnvioLoteRPS;
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
        public XmlDocument AplicaAssinatura2(XmlDocument docXML, string uri, X509Certificate2 cert)
        {
            try
            {
                string id = "";

                XmlAttributeCollection _Uri = docXML.GetElementsByTagName("tipos:InfPedidoCancelamento").Item(0).Attributes;
                foreach (XmlAttribute _atributo in _Uri)
                {
                    if (_atributo.Name == "Id")
                    {
                        //http://reference.Uri = "#" + _atributo.InnerText; Comentada para realizar a integração.
                        id = "#" + _atributo.InnerText;
                    }
                }

                var signedXml = new SignedXml(docXML);
                signedXml.SigningKey = cert.PrivateKey;

                Reference reference = new Reference(id);
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigC14NTransform());
                signedXml.AddReference(reference);

                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(cert));

                signedXml.KeyInfo = keyInfo;

                signedXml.ComputeSignature();

                XmlElement xmlSignature = docXML.CreateElement("dsig", "Signature", "http://www.w3.org/2000/09/xmldsig#");
                XmlElement xmlSignedInfo = signedXml.SignedInfo.GetXml();
                XmlElement xmlKeyInfo = signedXml.KeyInfo.GetXml();

                XmlElement xmlSignatureValue = docXML.CreateElement("dsig", "SignatureValue", xmlSignature.NamespaceURI);
                string signBase64 = Convert.ToBase64String(signedXml.Signature.SignatureValue);
                XmlText text = docXML.CreateTextNode(signBase64);
                xmlSignatureValue.AppendChild(text);

                xmlSignature.AppendChild(docXML.ImportNode(xmlSignedInfo, true));
                xmlSignature.AppendChild(xmlSignatureValue);
                xmlSignature.AppendChild(docXML.ImportNode(xmlKeyInfo, true));

                var evento = docXML.GetElementsByTagName(uri);
                evento[0].AppendChild(xmlSignature);
                //infNFe.AppendChild(xmlSignature);


                return docXML;
            }
            catch (Exception erro) { throw erro; }
        }

        private XmlDocument GeraXMLNFSe(DTONotaFiscal dTONotaFiscal, X509Certificate2 certificadoPrestador)
        {
            List<int> nfs = new List<int>();


            var listaRps = new tpNFSe();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = false;


            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);
            //XmlNode nodePrincipal = xmlDocument.CreateElement("p1", "ConsultarNfseEnvio", "http://www.prefeitura.sp.gov.br/nfe");
            XmlNode nodePrincipal = xmlDocument.CreateElement("p2", "CancelarNfseEnvio", "http://www.ginfes.com.br/servico_cancelar_nfse_envio_v03.xsd");
            xmlDocument.AppendChild(nodePrincipal);

            XmlAttribute attribute3 = xmlDocument.CreateAttribute("xmlns:p2");
            attribute3.Value = "http://www.ginfes.com.br/servico_cancelar_nfse_envio_v03.xsd";
            nodePrincipal.Attributes.Append(attribute3);

            XmlAttribute attribute2 = xmlDocument.CreateAttribute("xmlns:n2");
            attribute2.Value = "http://www.altova.com/samplexml/other-namespace";
            nodePrincipal.Attributes.Append(attribute2);

            XmlAttribute attribute5 = xmlDocument.CreateAttribute("xmlns:tipos");
            attribute5.Value = "http://www.ginfes.com.br/tipos_v03.xsd";
            nodePrincipal.Attributes.Append(attribute5);

            XmlAttribute attribute4 = xmlDocument.CreateAttribute("xmlns:dsig");
            attribute4.Value = "http://www.w3.org/2000/09/xmldsig#";
            nodePrincipal.Attributes.Append(attribute4);

            XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns:xsi");
            attribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            nodePrincipal.Attributes.Append(attribute);



            var nodePedido = xmlDocument.CreateElement("Pedido");
            nodePrincipal.AppendChild(nodePedido);



            var nodeInfPedidoCancelamento = xmlDocument.CreateElement("tipos", "InfPedidoCancelamento", "http://www.ginfes.com.br/tipos_v03.xsd");
            nodePedido.AppendChild(nodeInfPedidoCancelamento);


            XmlAttribute RPSid = xmlDocument.CreateAttribute("Id");
            RPSid.Value = "NFEsCanc" + dTONotaFiscal.NFSe.First().ChaveRPS.NumeroRPS;
            nodeInfPedidoCancelamento.Attributes.Append(RPSid);

            var nodeIdentificacaoNfse = xmlDocument.CreateElement("tipos", "IdentificacaoNfse", "http://www.ginfes.com.br/tipos_v03.xsd");
            nodeInfPedidoCancelamento.AppendChild(nodeIdentificacaoNfse);


            if ((!string.IsNullOrEmpty(dTONotaFiscal.Cabecalho.CPFCNPJRemetente.CNPJ)) ||
                     (!string.IsNullOrEmpty(dTONotaFiscal.Cabecalho.CPFCNPJRemetente.CPF)))
            {
                PreecheNode(xmlDocument, nodeIdentificacaoNfse, "Numero", dTONotaFiscal.NFSe.First().ChaveNFe.NumeroNFe, "tipos");

                if (!string.IsNullOrEmpty(dTONotaFiscal.Cabecalho.CPFCNPJRemetente.CNPJ))
                {
                    PreecheNode(xmlDocument, nodeIdentificacaoNfse, "Cnpj", dTONotaFiscal.Cabecalho.CPFCNPJRemetente.CNPJ, "tipos");
                }
                else
                {
                    PreecheNode(xmlDocument, nodeIdentificacaoNfse, "CPF", dTONotaFiscal.Cabecalho.CPFCNPJRemetente.CNPJ, "tipos");
                }
                //PreecheNode(xmlDocument, nodeIdentificacaoNfse, "InscricaoMunicipal", dTONotaFiscal.Cabecalho.CPFCNPJRemetente.InscricaoMunicipal, "tipos");
                PreecheNode(xmlDocument, nodeIdentificacaoNfse, "CodigoMunicipio", dTONotaFiscal.NFSe.First().CodigoMunicipioPrestador, "tipos");
            }
            PreecheNode(xmlDocument, nodeInfPedidoCancelamento, "CodigoCancelamento", "E137", "tipos");

            return xmlDocument;
        }

        private XmlElement PreecheNode(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value, string prefix)
        {
            var node = xmlDocument.CreateElement(prefix, nameElement, "http://www.ginfes.com.br/tipos_v03.xsd");
            if (value != null)
                node.InnerText = value;

            nodeRPS.AppendChild(node);
            return node;
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
