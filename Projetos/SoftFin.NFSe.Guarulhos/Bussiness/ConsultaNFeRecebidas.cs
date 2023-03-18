using SoftFin.NFSe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoftFin.NFSe.Guarulhos.Bussiness  
{
    public class ConsultaNFeRecebidas
    {
        public DTORetornoNFEs Execute(DTONotaFiscal pedidoConsultaNFe, X509Certificate2 certificadoPrestador)
        {
            return Processar(pedidoConsultaNFe, certificadoPrestador);
        }


        public XmlDocument AplicaAssinatura2(XmlDocument docXML, string uri, X509Certificate2 cert)
        {
            try
            {
                string id = "";

                XmlAttributeCollection _Uri = docXML.GetElementsByTagName("ConsultarNfseEnvio").Item(0).Attributes;
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

        private DTORetornoNFEs Processar(DTONotaFiscal pedidoConsultaNFe, X509Certificate2 certificadoPrestador)
        {
            XmlDocument nfes = GeraXMLNFSe(pedidoConsultaNFe, certificadoPrestador);

            XmlDocument retornoAssinado = nfes;
            //new SoftFin.NFSe.Guarulhos.Utils.AssinaturaDigital().AplicaAssinatura(
                                               //                    nfes, "ConsultarNfseEnvio", "ConsultarNfseEnvio", certificadoPrestador);

            retornoAssinado = AplicaAssinatura2(retornoAssinado, "ConsultarNfseEnvio", certificadoPrestador);
            
            //retornoAssinado.Save("c:\\lixo\\nfConsulta.xml");

            var retorno = EnviarPeloWebServico(certificadoPrestador, retornoAssinado);

            var retornofinal = ProcessaRetornoXML(retorno);
            retornofinal.xml = retornoAssinado.InnerXml;
            return retornofinal;
        }

        public XmlDocument AplicaAssinatura(XmlDocument docXML, string uri, X509Certificate2 cert)
        {
            try
            {
                // Obtem o certificado
                X509Certificate2 X509Cert = cert;
                // Cria um documento XML para carregar o XML
                //XmlDocument docXML = new XmlDocument();
                //docXML.PreserveWhitespace = true;
                //xml = xml.Replace("\r", "").Replace("\n", "");
                // Carrega o documento XML
                //docXML.LoadXml(xml);


                // Cria o objeto XML assinado
                SignedXml signedXml = new SignedXml(docXML);
                // Assina com a chave privada
                signedXml.SigningKey = X509Cert.PrivateKey;
                // Atribui o método de canonização
                signedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
                // Atribui o método para assinatura
                signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                // Cria a referencia
                Reference reference = new Reference("");
                // Pega a URI para ser assinada

                //reference.Uri = "#" + ((XmlElement)docXML.GetElementsByTagName("tipos:Cnpj")[0]).InnerText;
                XmlAttributeCollection _Uri = docXML.GetElementsByTagName(uri).Item(0).Attributes;
                foreach (XmlAttribute _atributo in _Uri)
                {
                    if (_atributo.Name == "Id")
                        reference.Uri = "#" + _atributo.InnerText;
                }
                // Adiciona o envelope à referência
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                // Atribui o método do Hash
                reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                // Adiciona a referencia ao XML assinado
                signedXml.AddReference(reference);
                // Cria o objeto keyInfo
                KeyInfo keyInfo = new KeyInfo();
                // Carrega a informação da KeyInfo
                KeyInfoClause rsaKeyVal = new RSAKeyValue((RSA)X509Cert.PrivateKey);
                KeyInfoX509Data x509Data = new KeyInfoX509Data(X509Cert);
                //x509Data.AddSubjectName(X509Cert.SubjectName.Name.ToString());
                keyInfo.AddClause(x509Data);
                //keyInfo.AddClause(rsaKeyVal);
                // Adiciona a KeyInfo
                signedXml.KeyInfo = keyInfo;
                // Atribui uma ID à assinatura
                //signedXml.Signature.Id = "#" + uri;
                // Efetiva a assinatura
                signedXml.ComputeSignature();
                bool signed = signedXml.CheckSignature(cert, true);
                
                
                // Obtem o XML assinado
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

                //var evento = docRequest.GetElementsByTagName(uri);
                //evento[0].AppendChild(xmlSignature);
                // Adiciona o elemento assinado ao XML
                docXML.DocumentElement.AppendChild(docXML.ImportNode(xmlSignature, true));

                // Retorna o XML
                return docXML;
            }
            catch (Exception erro) { throw erro; }
        }

        private static string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            System.ServiceModel.BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            EndpointAddress endPoint = new EndpointAddress("https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
            NFEsGinfes.ServiceGinfesImplClient servico = new NFEsGinfes.ServiceGinfesImplClient(binding, endPoint);
            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;


            var cabecalho = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            cabecalho += "<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\">";
            cabecalho += "<versaoDados>3</versaoDados>";
            cabecalho += "</ns2:cabecalho>";

            var retornoEnvioLoteRPS = servico.ConsultarNfseV3(cabecalho, nfes.OuterXml);
            return retornoEnvioLoteRPS;
        }

        private DTORetornoNFEs ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));
            XmlNodeList procEventoNFe = xmlDocument.GetElementsByTagName("ListaMensagemRetorno");
            //XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Cabecalho");
            //XmlNodeList NFes = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("NFe");
            XmlNodeList erros = null;

            if (procEventoNFe.Count == 0)
            {
                procEventoNFe = xmlDocument.GetElementsByTagName("ConsultarNfseResposta");
                if (procEventoNFe.Count != 0)
                {
                    erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("MensagemRetorno");
                }
            }
            else
                erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("ns4:MensagemRetorno");


            //XmlNodeList alertas = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Alerta");

            var resultingMessage = new DTORetornoNFEs();

            resultingMessage.Cabecalho.Sucesso = "False";

            //foreach (XmlElement item in NFes)
            //{
            //    var nFe = new tpNFSe();

            //    var chaveNFe = new tpChaveNFe();
            //    var chaveNFeNode = BuscaElementoXML(item, "ChaveNFe");
            //    chaveNFe.CodigoVerificacao = BuscaElementoXML(chaveNFeNode, "CodigoVerificacao").InnerText;
            //    chaveNFe.InscricaoPrestador = BuscaElementoXML(chaveNFeNode, "InscricaoPrestador").InnerText;
            //    chaveNFe.NumeroNFe = BuscaElementoXML(chaveNFeNode, "NumeroNFe").InnerText;
            //    nFe.ChaveNFe = chaveNFe;


            //    var chaveRPS = new tpChaveRPS();
            //    var chaveRPSNode = BuscaElementoXML(item, "ChaveRPS");
            //    chaveRPS.InscricaoPrestador = BuscaElementoXMLToString(chaveRPSNode, "InscricaoPrestador");
            //    chaveRPS.NumeroRPS = BuscaElementoXMLToString(chaveRPSNode, "NumeroRPS");
            //    chaveRPS.SerieRPS = BuscaElementoXMLToString(chaveRPSNode, "SerieRPS");
            //    nFe.ChaveRPS = chaveRPS;


            //    var enderecoTomador = new tpEndereco();
            //    var enderecoTomadorNode = BuscaElementoXML(item, "EnderecoTomador");
            //    enderecoTomador.Bairro = BuscaElementoXMLToString(enderecoTomadorNode, "Bairro");
            //    enderecoTomador.CEP = BuscaElementoXMLToString(enderecoTomadorNode, "CEP");
            //    enderecoTomador.Cidade = BuscaElementoXMLToString(enderecoTomadorNode, "Cidade");
            //    enderecoTomador.ComplementoEndereco = BuscaElementoXMLToString(enderecoTomadorNode, "ComplementoEndereco");
            //    enderecoTomador.Logradouro = BuscaElementoXMLToString(enderecoTomadorNode, "Logradouro");
            //    enderecoTomador.NumeroEndereco = BuscaElementoXMLToString(enderecoTomadorNode, "NumeroEndereco");
            //    enderecoTomador.TipoLogradouro = BuscaElementoXMLToString(enderecoTomadorNode, "TipoLogradouro");
            //    enderecoTomador.UF = BuscaElementoXMLToString(enderecoTomadorNode, "UF");
            //    nFe.EnderecoTomador = enderecoTomador;


            //    var enderecoPrestador = new tpEndereco();
            //    var enderecoPrestadorNode = BuscaElementoXML(item, "EnderecoPrestador");
            //    enderecoPrestador.Bairro = BuscaElementoXMLToString(enderecoPrestadorNode, "Bairro");
            //    enderecoPrestador.CEP = BuscaElementoXMLToString(enderecoPrestadorNode, "CEP");
            //    enderecoPrestador.Cidade = BuscaElementoXMLToString(enderecoPrestadorNode, "Cidade");
            //    enderecoPrestador.ComplementoEndereco = BuscaElementoXMLToString(enderecoPrestadorNode, "ComplementoEndereco");
            //    enderecoPrestador.Logradouro = BuscaElementoXMLToString(enderecoPrestadorNode, "Logradouro");
            //    enderecoPrestador.NumeroEndereco = BuscaElementoXMLToString(enderecoPrestadorNode, "NumeroEndereco");
            //    enderecoPrestador.TipoLogradouro = BuscaElementoXMLToString(enderecoPrestadorNode, "TipoLogradouro");
            //    enderecoPrestador.UF = BuscaElementoXMLToString(enderecoPrestadorNode, "UF");
            //    nFe.EnderecoPrestador = enderecoPrestador;


            //    var cpfcnpjPrestador = new tpCPFCNPJ();
            //    var cpfcnpjPrestadorNode = BuscaElementoXML(item, "CPFCNPJPrestador");
            //    cpfcnpjPrestador.CNPJ = BuscaElementoXMLToString(cpfcnpjPrestadorNode, "CNPJ");
            //    cpfcnpjPrestador.CPF = BuscaElementoXMLToString(cpfcnpjPrestadorNode, "CPF");
            //    nFe.CPFCNPJPrestador = cpfcnpjPrestador;

            //    var cpfcnpjTomador = new tpCPFCNPJ();
            //    var cpfcnpjTomadorNode = BuscaElementoXML(item, "CPFCNPJTomador");
            //    cpfcnpjTomador.CNPJ = BuscaElementoXMLToString(cpfcnpjTomadorNode, "CNPJ");
            //    cpfcnpjTomador.CPF = BuscaElementoXMLToString(cpfcnpjTomadorNode, "CPF");
            //    nFe.CPFCNPJTomador = cpfcnpjTomador;

            //    nFe.Assinatura = BuscaElementoXMLToString(item, "Assinatura");
            //    nFe.DataEmissaoNFe = BuscaElementoXMLToString(item, "DataEmissaoNFe");
            //    nFe.NumeroLote = BuscaElementoXMLToString(item, "NumeroLote");
            //    nFe.TipoRPS = BuscaElementoXMLToString(item, "TipoRPS");
            //    nFe.DataEmissaoRPS = BuscaElementoXMLToString(item, "DataEmissaoRPS");
            //    nFe.RazaoSocialPrestador = BuscaElementoXMLToString(item, "RazaoSocialPrestador");
            //    nFe.StatusNFe = BuscaElementoXMLToString(item, "StatusNFe");
            //    nFe.TributacaoNFe = BuscaElementoXMLToString(item, "TributacaoNFe");
            //    nFe.OpcaoSimples = BuscaElementoXMLToString(item, "OpcaoSimples");
            //    nFe.ValorServicos = BuscaElementoXMLToString(item, "ValorServicos");
            //    nFe.ValorDeducoes = BuscaElementoXMLToString(item, "ValorDeducoes");
            //    nFe.ValorPIS = BuscaElementoXMLToString(item, "ValorPIS");
            //    nFe.ValorCOFINS = BuscaElementoXMLToString(item, "ValorCOFINS");
            //    nFe.ValorINSS = BuscaElementoXMLToString(item, "ValorINSS");
            //    nFe.ValorIR = BuscaElementoXMLToString(item, "ValorIR");
            //    nFe.ValorCSLL = BuscaElementoXMLToString(item, "ValorCSLL");
            //    nFe.CodigoServico = BuscaElementoXMLToString(item, "CodigoServico");
            //    nFe.AliquotaServicos = BuscaElementoXMLToString(item, "AliquotaServicos");
            //    nFe.ValorISS = BuscaElementoXMLToString(item, "ValorISS");
            //    nFe.ValorCredito = BuscaElementoXMLToString(item, "ValorCredito");
            //    nFe.ISSRetido = BuscaElementoXMLToString(item, "ISSRetido");
            //    nFe.EmailTomador = BuscaElementoXMLToString(item, "EmailTomador");
            //    nFe.Discriminacao = BuscaElementoXMLToString(item, "Discriminacao");
            //    nFe.FonteCargaTributaria = BuscaElementoXMLToString(item, "FonteCargaTributaria");

                
            //    resultingMessage.NFe.Add(nFe);
            //}

            if (erros != null)
            {
                foreach (XmlElement item in erros)
                {
                    resultingMessage.Erro.Add(new TPErro
                    {
                        Codigo = item.ChildNodes[0].InnerText,
                        Descricao = item.ChildNodes[1].InnerText + "  " + item.ChildNodes[2].InnerText,
                    });
                }
            }

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

        public string BuscaElementoXMLToString(XmlElement nodes, String nomeNome)
        {
            foreach (XmlElement item in nodes)
            {
                if (item.Name == nomeNome)
                    return item.InnerText.ToString();
            }
            return "";
        }

        public XmlElement BuscaElementoXML(XmlElement nodes, String nomeNome)
        {
            foreach (XmlElement item in nodes)
            {
                if (item.Name == nomeNome)
                    return item;
            }
            return null;
        }

        private XmlDocument GeraXMLNFSe(DTONotaFiscal pedidoConsultaNFe, X509Certificate2 certificadoPrestador)
        {
            List<int> nfs = new List<int>();

            var envio = pedidoConsultaNFe;

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = false;
            

            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);
            //XmlNode nodePrincipal = xmlDocument.CreateElement("p1", "ConsultarNfseEnvio", "http://www.prefeitura.sp.gov.br/nfe");
            XmlNode nodePrincipal = xmlDocument.CreateElement( "ConsultarNfseEnvio");
            xmlDocument.AppendChild(nodePrincipal);

            XmlAttribute RPSid = xmlDocument.CreateAttribute("Id");
            RPSid.Value = "NFEsConsulta";
            nodePrincipal.Attributes.Append(RPSid);

            XmlAttribute attribute3 = xmlDocument.CreateAttribute("xmlns");
            attribute3.Value = "http://www.ginfes.com.br/servico_consultar_nfse_envio_v03.xsd";
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

            //XmlAttribute RPSxmlnsattribute = xmlDocument.CreateAttribute("Id");
            //RPSxmlnsattribute.Value = "NFES" + pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ;
            //nodePrincipal.Attributes.Append(RPSxmlnsattribute);


            var nodePrestador = xmlDocument.CreateElement("Prestador");
            nodePrincipal.AppendChild(nodePrestador);

            

            if ((!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ)) ||
                (!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CPF)))
            {

                if (!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ))
                {
                    PreecheNode(xmlDocument, nodePrestador, "Cnpj", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ, "tipos");
                   // PreecheNode(xmlDocument, nodePrestador, "InscricaoMunicipal", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.InscricaoMunicipal, "tipos");
                }
                else
                {
                    PreecheNode(xmlDocument, nodePrestador, "CPF", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ, "tipos");
                    PreecheNode(xmlDocument, nodePrestador, "InscricaoMunicipal", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.InscricaoMunicipal, "tipos");
                }





            
            }
            var nodePeriodoEmissao = xmlDocument.CreateElement("PeriodoEmissao");
            nodePrincipal.AppendChild(nodePeriodoEmissao);

            SoftFin.NFSe.Guarulhos.Utils.Util.AdicionaNovoNode(xmlDocument, nodePeriodoEmissao, "DataInicial", pedidoConsultaNFe.Cabecalho.dtInicio);
            SoftFin.NFSe.Guarulhos.Utils.Util.AdicionaNovoNode(xmlDocument, nodePeriodoEmissao, "DataFinal", pedidoConsultaNFe.Cabecalho.dtFim);

            //var nodeSignature = xmlDocument.CreateElement("dsig", "Signature", "http://www.w3.org/2000/09/xmldsig#");
            //nodePrincipal.AppendChild(nodeSignature);

            return xmlDocument;
        }


        private XmlElement PreecheNode(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value,string prefix)
        {
            var node = xmlDocument.CreateElement(prefix, nameElement, "http://www.ginfes.com.br/tipos_v03.xsd");
            if (value != null)
                node.InnerText = value;
            
            nodeRPS.AppendChild(node);
            return node;
        }


    }
}
