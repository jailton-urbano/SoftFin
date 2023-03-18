using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoftFin.Sefaz
{
    
    public class NFeInutilizacao
    {
        string _UrlWebservice = "";
        public DTORetornoNFe Execute(DTONfe dTONfe, 
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string urlWebservice = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeinutilizacao2.asmx") 
        {
            _UrlWebservice = urlWebservice;
            return Processar(dTONfe, cert);
        }


        private DTORetornoNFe Processar(DTONfe dTONfe, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            XmlDocument nfes = GeraXMLNFSe(dTONfe, cert);

            var retorno = EnviarPeloWebServico(cert, nfes);

            var retornofinal = ProcessaRetornoXML(retorno);
            return retornofinal;
        }

        private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            var servico = new SoftFin.Sefaz.srwsNFeInutilizacao.NFeInutilizacao4Soap12Client("NFeInutilizacao4Soap12", _UrlWebservice);
            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;



            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(nfes.InnerXml.ToString());

            var retorno = servico.nfeInutilizacaoNF(xmld);

            return retorno.InnerXml;

        }

        private DTORetornoNFe ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var resultingMessage = new DTORetornoNFe();

            if (retornoEnvioLoteRPS.Contains("Nenhum documento localizado para o destinatario"))
            {
                resultingMessage.Sucesso = true;
                resultingMessage.Alertas.Add(new DTOErro { codigo = "137", descricao = "Nenhum documento localizado para o destinatário" });
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

        private XmlDocument GeraXMLNFSe(DTONfe dTONfe, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            List<int> nfs = new List<int>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;

            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);

            XmlNode nodePrincipal = xmlDocument.CreateElement("consNFeDest");
            xmlDocument.AppendChild(nodePrincipal);

            XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
            attributev.Value = "1.01";
            nodePrincipal.Attributes.Append(attributev);

            XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns");
            attribute.Value = "http://www.portalfiscal.inf.br/nfe";
            nodePrincipal.Attributes.Append(attribute);

            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "1");
            }
            else
            {
                AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "2");
            }
            AdicionaNovoNode(xmlDocument, nodePrincipal, "xServ", "CONSULTAR NFE DEST");
            AdicionaNovoNode(xmlDocument, nodePrincipal, "CNPJ", dTONfe.InfNFe.Dest.CNPJ);
            AdicionaNovoNode(xmlDocument, nodePrincipal, "indNFe", "0");
            AdicionaNovoNode(xmlDocument, nodePrincipal, "indEmi", "0");
            AdicionaNovoNode(xmlDocument, nodePrincipal, "ultNSU", "0");


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
