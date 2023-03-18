using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;

namespace SoftFin.Sefaz
{
    public class NFeDistribuicaoDFe
    {
        string _UrlWebservice = "";
        public DTORetorno<DTORetornoNotaEntrada> Execute(string cpfCnpj,
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string urlWebservice = "https://www1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx")
        {
            _UrlWebservice = urlWebservice;
            return Processar(cpfCnpj, cert);
        }


        private DTORetorno<DTORetornoNotaEntrada> Processar(string cpfCnpj, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            //XmlDocument nfes = GeraXMLNFSe(dTONfe, cert);

            var retorno = EnviarPeloWebServico(cert, cpfCnpj.Replace("-","").Replace("/", "").Replace(".", ""));

            var retornofinal = ProcessaRetornoXML(retorno);
            return retornofinal;
        }

        private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, string cnpj)
        {

            ////var servico = new SoftFin.Sefaz.srwsNFeConsultaDestinatario.NFeConsultaDestSoapClient("NFeConsultaDestSoap", _UrlWebservice);
            var servico = new SoftFin.Sefaz.srvsNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient("NFeDistribuicaoDFeSoap", _UrlWebservice);
            servico.ClientCredentials.ServiceCertificate.DefaultCertificate = certificadoPrestador;
            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;


            var sbNF = new StringBuilder();

            var path = HostingEnvironment.MapPath("~/Template/TXT/");
            var arquivohmtl = Path.Combine(path, "NFeDistribuicaoDFe.txt");
            string readText = System.IO.File.ReadAllText(arquivohmtl);
            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                readText = readText.Replace("[tpAmb]", "1");
            }
            else
            {
                readText = readText.Replace("[tpAmb]", "1");
            }

            readText = readText.Replace("[UF]", "35");
            readText = readText.Replace("[CNPJ]", cnpj);
            XElement xmlDist = XElement.Parse(readText);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            var retorno = servico.nfeDistDFeInteresse(xmlDist);

            return retorno.ToString();

        }

        private DTORetorno<DTORetornoNotaEntrada> ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {

            //TODO RICARDO - Colocar log

            DTORetorno<DTORetornoNotaEntrada> resultingMessage = new DTORetorno<DTORetornoNotaEntrada>();


            if (retornoEnvioLoteRPS.Contains("Nenhum documento localizado"))
            {
                resultingMessage.CDStatus = "NOK";
                resultingMessage.CMDAceito = true;
                resultingMessage.Alertas = new List<DTOErro>();
                resultingMessage.Alertas.Add(new DTOErro { codigo = "137", descricao = "Nenhum documento localizado para o destinatário" });
                return resultingMessage;
            }
            if (retornoEnvioLoteRPS.Contains("Rejeição:"))
            {
                resultingMessage.CDStatus = "NOK";
                resultingMessage.CMDAceito = false;
                resultingMessage.DSStatus = retornoEnvioLoteRPS.Substring(retornoEnvioLoteRPS.IndexOf("Rejeição"), retornoEnvioLoteRPS.IndexOf("</xMotivo>") - retornoEnvioLoteRPS.IndexOf("Rejeição")); ;
                return resultingMessage;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));

            XmlNodeList infProt = xmlDocument.GetElementsByTagName("docZip");
            resultingMessage.Objs = new List<DTORetornoNotaEntrada>();
            foreach (XmlElement item in infProt)
            {
                var cStat2 = item.InnerText;
                byte[] buffer = Convert.FromBase64String(cStat2);
                byte[] xmlret = Decompress(buffer);

                string result = System.Text.Encoding.UTF8.GetString(xmlret);

                XmlDocument xmlNota = new XmlDocument();
                xmlNota.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(result));

                DTORetornoNotaEntrada dTOInfNFe = new DTORetornoNotaEntrada();

                dTOInfNFe.CNPJ = xmlNota.GetElementsByTagName("CNPJ")[0].InnerText;
                dTOInfNFe.chNFe = xmlNota.GetElementsByTagName("chNFe")[0].InnerText;
                dTOInfNFe.nProt = xmlNota.GetElementsByTagName("nProt")[0].InnerText;
                

                if (result.Contains("xEvento"))
                    dTOInfNFe.xEvento = xmlNota.GetElementsByTagName("xEvento")[0].InnerText;

                if (result.Contains("xNome"))
                    dTOInfNFe.xNome = xmlNota.GetElementsByTagName("xNome")[0].InnerText;

                if (result.Contains("<IE>"))
                    dTOInfNFe.IE = xmlNota.GetElementsByTagName("IE")[0].InnerText;

                if (result.Contains("dhEmi"))
                    dTOInfNFe.dhEmi = xmlNota.GetElementsByTagName("dhEmi")[0].InnerText;

                if (result.Contains("tpNF"))
                    dTOInfNFe.tpNF = xmlNota.GetElementsByTagName("tpNF")[0].InnerText;

                if (result.Contains("vNF"))
                    dTOInfNFe.vNF = xmlNota.GetElementsByTagName("vNF")[0].InnerText;

                if (result.Contains("digVal"))
                    dTOInfNFe.digVal = xmlNota.GetElementsByTagName("digVal")[0].InnerText;

                if (result.Contains("cSitNFe"))
                    dTOInfNFe.cSitNFe = xmlNota.GetElementsByTagName("cSitNFe")[0].InnerText;


                if (result.Contains("xJust"))
                    dTOInfNFe.xJust = xmlNota.GetElementsByTagName("xJust")[0].InnerText;



                dTOInfNFe.xmlCompleto = result;

                resultingMessage.Objs.Add(dTOInfNFe);
            }
            resultingMessage.CDStatus = "OK";
            resultingMessage.CMDAceito = true;
            return resultingMessage;
        }

        public static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
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

        //private XmlDocument GeraXMLNFSe(DTONfe dTONfe, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        //{
        //    List<int> nfs = new List<int>();

        //    XmlDocument xmlDocument = new XmlDocument();
        //    xmlDocument.PreserveWhitespace = true;

        //    XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        //    XmlElement document_element = xmlDocument.DocumentElement;
        //    xmlDocument.InsertBefore(xml_declaration, document_element);

        //    XmlNode nodePrincipal = xmlDocument.CreateElement("consNFeDest");
        //    xmlDocument.AppendChild(nodePrincipal);

        //    XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
        //    attributev.Value = "1.01";
        //    nodePrincipal.Attributes.Append(attributev);

        //    XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns");
        //    attribute.Value = "http://www.portalfiscal.inf.br/nfe";
        //    nodePrincipal.Attributes.Append(attribute);

        //    AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "1");
        //    AdicionaNovoNode(xmlDocument, nodePrincipal, "xServ", "CONSULTAR NFE DEST");
        //    AdicionaNovoNode(xmlDocument, nodePrincipal, "CNPJ", dTONfe.InfNFe.Dest.CNPJ);
        //    AdicionaNovoNode(xmlDocument, nodePrincipal, "indNFe", "0");
        //    AdicionaNovoNode(xmlDocument, nodePrincipal, "indEmi", "0");
        //    AdicionaNovoNode(xmlDocument, nodePrincipal, "ultNSU", "0");


        //    return xmlDocument;
        //}

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
