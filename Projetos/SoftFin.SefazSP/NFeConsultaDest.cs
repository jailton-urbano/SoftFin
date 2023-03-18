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
    public class NFeConsultaDestNovo
    {
        string _UrlWebservice = "";
        string _Uf = "";
        string _Cpfcnpj = "";

        public DTORetornoCadastro Execute(string cpfcnpj,
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string urlWebservice = "", 
            string uf = "SP")
        {
            _UrlWebservice = urlWebservice;
            _Uf = uf;
            _Cpfcnpj = cpfcnpj;
            return Processar(cpfcnpj, cert);
        }


        private DTORetornoCadastro Processar(string cpfcnpj, 
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            var retorno = EnviarPeloWebServico(cert);

            var retornofinal = ProcessaRetornoXML(retorno);
            return retornofinal;
        }

        private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador)
        {
            var servico = new SoftFin.Sefaz.srwsNFeConsultaDestinatario.CadConsultaCadastro4Soap12Client("CadConsultaCadastro4Soap12", _UrlWebservice) ;

            servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;


            var sbNF = new StringBuilder();

            var path = HostingEnvironment.MapPath("~/Template/TXT/");
            var arquivohmtl = Path.Combine(path, "consultaCadastro2.txt");
            string readText = System.IO.File.ReadAllText(arquivohmtl);

            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                readText = readText.Replace("[tpAmb]", "1");
            }
            else
            {
                readText = readText.Replace("[tpAmb]", "2");
            }

            readText = readText.Replace("[UF]", _Uf);
            readText = readText.Replace("[CNPJ]", _Cpfcnpj);

            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(readText);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            //var cabec = new SoftFin.Sefaz.srwsNFeConsultaAntigo.nfeCabecMsg();
            //cabec.cUF = "33";
            //cabec.versaoDados = "2.00";

            

            var retorno = servico.consultaCadastro(xmld);

            return retorno.InnerXml.ToString();

        }

        private DTORetornoCadastro ProcessaRetornoXML(string retornoXML)
        {

            //TODO RICARDO - Colocar log

            var resultingMessage = new DTORetornoCadastro();

            if (retornoXML.Contains("Nenhum documento localizado"))
            {
                resultingMessage.CDStatus = "NOK";
                resultingMessage.DSStatus = "CNPJ não encontrado";
                return resultingMessage;
            }
            if (retornoXML.Contains("Rejeição:"))
            {
                resultingMessage.CDStatus = "NOK";
                resultingMessage.DSStatus = retornoXML.Substring(retornoXML.IndexOf("Rejeição"), retornoXML.IndexOf("</xMotivo>") - retornoXML.IndexOf("Rejeição"));  ;
                return resultingMessage;
            }
            resultingMessage.CDStatus = "OK";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoXML));

            resultingMessage.InfConsObj.InfCad = new DTORetornoCadastro.InfCad();
            resultingMessage.InfConsObj.InfCad.Ender = new DTORetornoCadastro.Ender();

            resultingMessage.InfConsObj.XNome = xmlDocument.GetElementsByTagName("xNome")[0].InnerText;
            resultingMessage.InfConsObj.CNPJ = xmlDocument.GetElementsByTagName("CNPJ")[0].InnerText;
            resultingMessage.InfConsObj.CStat = xmlDocument.GetElementsByTagName("cStat")[0].InnerText;
            resultingMessage.InfConsObj.CUF = xmlDocument.GetElementsByTagName("cUF")[0].InnerText;
           // resultingMessage.InfConsObj.DhCons = xmlDocument.GetElementsByTagName("DhCons")[0].InnerText;
            resultingMessage.InfConsObj.InfCad.Ender.CEP = xmlDocument.GetElementsByTagName("CEP")[0].InnerText;
            resultingMessage.InfConsObj.InfCad.Ender.Nro = xmlDocument.GetElementsByTagName("nro")[0].InnerText;
            resultingMessage.InfConsObj.InfCad.Ender.XBairro = xmlDocument.GetElementsByTagName("xBairro")[0].InnerText;
            resultingMessage.InfConsObj.InfCad.Ender.XLgr = xmlDocument.GetElementsByTagName("xLgr")[0].InnerText;
            resultingMessage.InfConsObj.InfCad.Ender.XMun = xmlDocument.GetElementsByTagName("xMun")[0].InnerText;
            resultingMessage.InfConsObj.InfCad.IE = xmlDocument.GetElementsByTagName("IE")[0].InnerText;


            
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
