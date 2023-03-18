using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SoftFin.Sefaz
{
    public class NFeStatus
    {
        string _UrlWebservice = "";
        public DTORetornoNFe Execute(string codigoUF, 
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string urlWebservice = "") 
        {
            _UrlWebservice = urlWebservice;
            return Processar(codigoUF, cert);
        }


        private DTORetornoNFe Processar(string codigoUF, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            XmlDocument nfes = GeraXMLNFSe(codigoUF, cert);

            var retorno = EnviarPeloWebServico(cert, nfes);

            var retornofinal = ProcessaRetornoXML(retorno);
            return retornofinal;
        }

        private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {

            var servico = new SoftFin.Sefaz.srwsNFeStatusServico.NFeStatusServico4Soap12Client("NFeStatusServico4Soap12", _UrlWebservice);
            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(nfes.InnerXml.ToString());


            var retorno = servico.nfeStatusServicoNF(xmld);

            return retorno.InnerXml;

        }


        private DTORetornoNFe ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var resultingMessage = new DTORetornoNFe();

            if (retornoEnvioLoteRPS.Contains("Serviço em Operação"))
            {
                resultingMessage.Sucesso = true;
                resultingMessage.Alertas = new List<DTOErro>();
                resultingMessage.Alertas.Add(new DTOErro { codigo = "107", descricao = "Serviço em Operação" });
            }

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

        private XmlDocument GeraXMLNFSe(string codigoUF, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            List<int> nfs = new List<int>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            

             XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);

            XmlNode nodePrincipal = xmlDocument.CreateElement("consStatServ");
            xmlDocument.AppendChild(nodePrincipal);


            //XmlSchema schema = new XmlSchema();
            //schema.Namespaces.Add("xmlns", "http://www.portalfiscal.inf.br/nfe/wsdl/NFeStatusServico4");
            //xmlDocument.Schemas.Add(schema);
            

            XmlAttribute attributename= xmlDocument.CreateAttribute("xmlns");
            attributename.Value = "http://www.portalfiscal.inf.br/nfe";
            nodePrincipal.Attributes.Append(attributename);

            XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
            attributev.Value = "4.00";
            nodePrincipal.Attributes.Append(attributev);



            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "1");
            }
            else
            {
                AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "2");
            }

            AdicionaNovoNode(xmlDocument, nodePrincipal, "cUF", codigoUF);
            AdicionaNovoNode(xmlDocument, nodePrincipal, "xServ", "STATUS");

            

            return xmlDocument;
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
