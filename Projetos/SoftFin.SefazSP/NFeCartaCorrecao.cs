using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SoftFin.Sefaz
{

    public class NFeCartaCorrecao
    {
        string _UrlWebservice = "";
        public DTORetornoNFe Execute(DTOLogEvento dTOEvento,
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string caminhoXSD,
            string urlServico) 
        {

            _UrlWebservice = urlServico;
            return Processar(dTOEvento, cert, caminhoXSD);
        }


        private DTORetornoNFe Processar(DTOLogEvento dTOEvento, 
                        System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
                        string caminhoXSD
        )
        {
            XmlDocument nfes = GeraXMLNFSe(dTOEvento, cert);

            DTORetornoNFe retornofinal = new DTORetornoNFe();

            nfes.Schemas.Add("http://www.portalfiscal.inf.br/nfe", caminhoXSD);
            retornofinal.xml = nfes;
            retornofinal.Sucesso = true;
            retornofinal.Erros = new List<DTOErro>();
            retornofinal.Alertas = new List<DTOErro>();

            nfes.Validate((sender, args) =>
            {
                var exception = (args.Exception as XmlSchemaValidationException);

                if (exception != null)
                {
                    retornofinal.Sucesso = false;
                    retornofinal.Erros.Add(new DTOErro { codigo = "XSDVALIDACAO", descricao = exception.Message });
                }
            });

            if (retornofinal.Sucesso)
            {
                var retorno = EnviarPeloWebServico(cert, nfes);
                retornofinal = ProcessaRetornoXML(retorno, retornofinal);
            }
            return retornofinal;
        }

        private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            var servico = new SoftFin.Sefaz.srwsNFeRecepcaoEvento.NFeRecepcaoEvento4Soap12Client("NFeRecepcaoEvento4Soap12", _UrlWebservice);
            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;

            //var cabec = new SoftFin.Sefaz.srwsNFeRecepcaoEvento.nfeCabecMsg();
            //cabec.cUF = "35";
            //cabec.versaoDados = "1.00";

            //nfes.PreserveWhitespace = true;

            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(nfes.InnerXml.ToString());

            var retorno = servico.nfeRecepcaoEvento(xmld);

            return retorno.InnerXml;

        }

        private DTORetornoNFe ProcessaRetornoXML(string retornoEnvioLoteRPS, DTORetornoNFe dTORetornoNFe)
        {
            XmlDocument xmld = new XmlDocument();
            retornoEnvioLoteRPS = "<retorno>" + retornoEnvioLoteRPS + "</retorno>";
            xmld.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));
            XmlNodeList procEventoNFe = xmld.GetElementsByTagName("infRec");
            var xMotivo = xmld.GetElementsByTagName("xMotivo")[0].InnerText;

            dTORetornoNFe.Sucesso = (xMotivo == "Lote de Evento Processado") ? true : false;
            dTORetornoNFe.xmlRetorno = retornoEnvioLoteRPS;
            
            if (dTORetornoNFe.Sucesso)
            {
                XmlNodeList infEventos = xmld.GetElementsByTagName("infEvento");
            
                foreach (XmlElement item in infEventos)
                {   
                    var cstat2 = item.GetElementsByTagName("cStat")[0].InnerText;
                    var xMotivo2 = item.GetElementsByTagName("xMotivo")[0].InnerText;
                    if (cstat2 == "135")
                    {
                        dTORetornoNFe.Alertas.Add(new DTOErro { codigo = cstat2, descricao = xMotivo2});
                        dTORetornoNFe.Sucesso = true;
                    }
                    else
                    {
                        dTORetornoNFe.Erros.Add(new DTOErro { codigo = cstat2, descricao = xMotivo2});
                        dTORetornoNFe.Sucesso = false;
                    }
                }
            }

            return dTORetornoNFe;
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

        private XmlDocument GeraXMLNFSe(DTOLogEvento dTOEvento, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            List<int> nfs = new List<int>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;

            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);

            XmlNode nodePrincipal = xmlDocument.CreateElement("envEvento");
            xmlDocument.AppendChild(nodePrincipal);

            XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
            attributev.Value = "1.00";
            nodePrincipal.Attributes.Append(attributev);

            XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns");
            attribute.Value = "http://www.portalfiscal.inf.br/nfe";
            nodePrincipal.Attributes.Append(attribute);

            AdicionaNovoNode(xmlDocument, nodePrincipal, "idLote", "1");

            XmlNode nodeevento = xmlDocument.CreateElement("evento");
            nodePrincipal.AppendChild(nodeevento);

            XmlAttribute attributev2 = xmlDocument.CreateAttribute("versao");
            attributev2.Value = "1.00";
            nodeevento.Attributes.Append(attributev2);

            XmlNode nodeinfEvento = xmlDocument.CreateElement("infEvento");
            nodeevento.AppendChild(nodeinfEvento);



            AdicionaNovoAtributo(xmlDocument, nodeinfEvento, "Id", "ID110110" + dTOEvento.chNFe + "0" + dTOEvento.nSeqEvento);
            AdicionaNovoNode(xmlDocument, nodeinfEvento, "cOrgao", dTOEvento.cOrgao);
            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                AdicionaNovoNode(xmlDocument, nodeinfEvento, "tpAmb", "1");
            }
            else
            {
                AdicionaNovoNode(xmlDocument, nodeinfEvento, "tpAmb", "2");
            }
            if (!string.IsNullOrEmpty(dTOEvento.cnpj))
            {
                AdicionaNovoNode(xmlDocument, nodeinfEvento, "CNPJ", dTOEvento.cnpj);
            }
            else
            {
                AdicionaNovoNode(xmlDocument, nodeinfEvento, "CPF", dTOEvento.cpf);
            }
            AdicionaNovoNode(xmlDocument, nodeinfEvento, "chNFe", dTOEvento.chNFe);
            AdicionaNovoNode(xmlDocument, nodeinfEvento, "dhEvento", dTOEvento.dhEvento);
            AdicionaNovoNode(xmlDocument, nodeinfEvento, "tpEvento", "110110");
            AdicionaNovoNode(xmlDocument, nodeinfEvento, "nSeqEvento", dTOEvento.nSeqEvento);
            AdicionaNovoNode(xmlDocument, nodeinfEvento, "verEvento", "1.00");


            XmlNode nodedetEvento = xmlDocument.CreateElement("detEvento");
            nodeinfEvento.AppendChild(nodedetEvento);

            XmlAttribute attributev3 = xmlDocument.CreateAttribute("versao");
            attributev3.Value = "1.00";
            nodedetEvento.Attributes.Append(attributev3);

            AdicionaNovoNode(xmlDocument, nodedetEvento, "descEvento", "Carta de Correção");
            AdicionaNovoNode(xmlDocument, nodedetEvento, "xCorrecao", dTOEvento.xCorrecao);
            AdicionaNovoNode(xmlDocument, nodedetEvento, "xCondUso", "A Carta de Correção é disciplinada pelo § 1º-A do art. 7º do Convênio S/N, de 15 de dezembro de 1970 e pode ser utilizada para regularização de erro ocorrido na emissão de documento fiscal, desde que o erro não esteja relacionado com: I - as variáveis que determinam o valor do imposto tais como: base de cálculo, alíquota, diferença de preço, quantidade, valor da operação ou da prestação; II - a correção de dados cadastrais que implique mudança do remetente ou do destinatário; III - a data de emissão ou de saída.");


            xmlDocument = AplicaAssinatura(xmlDocument, cert);


            return xmlDocument;
        }

        public XmlDocument AplicaAssinatura(XmlDocument docXML, X509Certificate2 cert)
        {
            try
            {
                XmlDocument docRequest = new XmlDocument();
                docRequest.PreserveWhitespace = true;
                docRequest.LoadXml(docXML.InnerXml.ToString());


                XmlNodeList ListNFe = docRequest.GetElementsByTagName("evento");

                foreach (XmlElement itemNFe in ListNFe)
                {

                    Reference reference = new Reference();
                    var infEvento = itemNFe.GetElementsByTagName("infEvento")[0];
                    reference.Uri = "#" + infEvento.Attributes.GetNamedItem("Id").InnerText;
                    // Create a SignedXml object.
                    SignedXml signedXml = new SignedXml(docRequest);
                    // Add the key to the SignedXml document
                    signedXml.SigningKey = cert.PrivateKey;

                    // Add an enveloped transformation to the reference.
                    XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                    reference.AddTransform(env);
                    XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                    reference.AddTransform(c14);
                    // Add the reference to the SignedXml object.
                    signedXml.AddReference(reference);
                    // Create a new KeyInfo object
                    KeyInfo keyInfo = new KeyInfo();
                    // Load the certificate into a KeyInfoX509Data object
                    // and add it to the KeyInfo object.
                    keyInfo.AddClause(new KeyInfoX509Data(cert));
                    // Add the KeyInfo object to the SignedXml object.
                    signedXml.KeyInfo = keyInfo;
                    signedXml.ComputeSignature();
                    // Get the XML representation of the signature and save
                    // it to an XmlElement object.
                    XmlElement xmlDigitalSignature = signedXml.GetXml();





                    //var evento = docRequest.GetElementsByTagName("evento");
                    itemNFe.AppendChild(xmlDigitalSignature);

                }
                return docRequest;

            }
            catch (Exception erro) { throw erro; }
        }


        public void AdicionaNovoNode(XmlDocument xmlDocument, XmlNode nodeDestino, string nomeNovoNode, string valorNovoNode)
        {
            var novoNode = xmlDocument.CreateElement(nomeNovoNode);
            novoNode.InnerText = valorNovoNode;
            nodeDestino.AppendChild(novoNode);
        }

        public void AdicionaNovoAtributo(XmlDocument xmlDocument, XmlNode nodeDestino, string nomeNovoAtributo, string valorNovoAtributo)
        {
            var attribute = xmlDocument.CreateAttribute(nomeNovoAtributo);
            attribute.Value = valorNovoAtributo;
            nodeDestino.Attributes.Append(attribute);
        }

    }
}
