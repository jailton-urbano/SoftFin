using SoftFin.Migrate.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftFin.Migrate.NFSe.Business
{
    public class EnvioNF
    {
        public Documento Execute(Envio envio, string empresaPK, string estabHash)
        {
            var retorno = new Documento();
            var cy = new WebService.ClienteInvoiCy();
            

            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                envio.ide.tpAmb = "1";
                cy.UrlWs = "https://www.invoicy.com.br/arecepcao.aspx";
            }
            else
            {
                envio.ide.tpAmb = "2";
                cy.UrlWs = "https://homolog.invoicy.com.br/arecepcao.aspx";
            }


            var xml = ToXML(envio);
            var xmllimpo = xml.Replace("\r\n","");

            xmllimpo = xmllimpo.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            xmllimpo = xmllimpo.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            var hashGerado = Utils.UtilSoftFin.GetMd5Hash(estabHash + xmllimpo);

            cy.Soap = cy.EscreveSoap(xmllimpo, empresaPK, hashGerado);
            cy.ExecutaWS();
            retorno = LoadFromXMLString(cy.SoapRet);
            retorno.xml = xml;
            return retorno;
        }
        public string ToXML(Envio envio)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(Envio));
            serializer.Serialize(stringwriter, envio);
            return stringwriter.ToString();
        }

        public static  Documento LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Documento));
            return serializer.Deserialize(stringReader) as Documento;
        }

    }
}
