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

    public class NFeDownload
    {
        string _UrlWebservice = "";
        public DTORetornoNFe Execute(
            string chaveAcesso,
            string cnpjDestinatario, 
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string caminhoXSD,
            string urlServico) 
        {

            _UrlWebservice = urlServico;
            return Processar(chaveAcesso, cnpjDestinatario, cert, caminhoXSD);
        }


        private DTORetornoNFe Processar(
                        string chaveAcesso,
                        string cnpjDestinatario, 
                        System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
                        string caminhoXSD
        )
        {
            XmlDocument nfes = GeraXMLNFSe(chaveAcesso, cnpjDestinatario, cert);

            DTORetornoNFe retornofinal = new DTORetornoNFe();

            //nfes.Schemas.Add("http://www.portalfiscal.inf.br/nfe", caminhoXSD);
            //retornofinal.xml = nfes;
            //retornofinal.Sucesso = true;
            //retornofinal.Erros = new List<DTOErro>();
            //retornofinal.Alertas = new List<DTOErro>();

            //nfes.Validate((sender, args) =>
            //{
            //    var exception = (args.Exception as XmlSchemaValidationException);

            //    if (exception != null)
            //    {
            //        retornofinal.Sucesso = false;
            //        retornofinal.Erros.Add(new DTOErro { codigo = "XSDVALIDACAO", descricao = exception.Message });
            //    }
            //});

            if (retornofinal.Sucesso)
            {
                var retorno = EnviarPeloWebServico(cert, nfes);
                retornofinal = ProcessaRetornoXML(retorno, retornofinal);
            }
            return retornofinal;
        }

        private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            var servico = new SoftFin.Sefaz.srwsNFeRecepcaoEvento.NFeRecepcaoEvento4Soap12Client("NFeRecepcaoEvento4Soap12");
            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;



            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(nfes.InnerXml.ToString());

            var retorno = servico.nfeRecepcaoEvento( xmld);

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
                XmlNodeList infEventos = xmld.GetElementsByTagName("procNFe");
            
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

        private XmlDocument GeraXMLNFSe(
            string chaveAcesso, 
            string cnpjDestinatario, 
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            List<int> nfs = new List<int>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;

            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);

            XmlNode nodePrincipal = xmlDocument.CreateElement("downloadNFe");
            xmlDocument.AppendChild(nodePrincipal);

            XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
            attributev.Value = "1.00";
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

            AdicionaNovoNode(xmlDocument, nodePrincipal, "xServ", "DOWNLOAD NFE");
            AdicionaNovoNode(xmlDocument, nodePrincipal, "CNPJ", cnpjDestinatario);
            AdicionaNovoNode(xmlDocument, nodePrincipal, "chNFe", chaveAcesso);


            

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
