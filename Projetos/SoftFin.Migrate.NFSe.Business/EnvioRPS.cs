using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftFin.Migrate.NFSe.Business
{
    public class EnvioRPS
    {
        public DTO.RetornoComum.Envelope Execute(DTO.Envio envio, string empresaPK, string estabHash)
        {
            var retorno = new DTO.RetornoComum.Envelope();
            var cy = new WebService.ClienteInvoiCy();
            

            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                envio.RPS.tpAmb = "1";
                cy.UrlWs = "https://web.invoicy.com.br/arecepcao.aspx";
            }
            else
            {
                envio.RPS.tpAmb = "2";
                cy.UrlWs = "https://homolog.invoicy.com.br/arecepcao.aspx";
            }


            var xml = ToXML(envio);
            var xmllimpo = xml.Replace("\r\n","").Replace("&", "E");
            xmllimpo = xmllimpo.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            xmllimpo = xmllimpo.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            while (xmllimpo.IndexOf("> ") > 0)
            {
                xmllimpo = xmllimpo.Replace("> ", ">");
            }
            //while (xmllimpo.IndexOf(" />") > 0)
            //{
            //    xmllimpo = xmllimpo.Replace(" />", "/>");
            //}


           var hashGerado = Utils.UtilSoftFin.GetMd5Hash(estabHash + xmllimpo);

            cy.Soap = cy.EscreveSoap(xmllimpo, empresaPK, hashGerado);
            cy.ExecutaWS();
            retorno = LoadFromXMLString(cy.SoapRet);
            retorno.xml = xml;
            return retorno;
        }
        public string ToXML(DTO.Envio envio)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(DTO.Envio));
            serializer.Serialize(stringwriter, envio);
            return stringwriter.ToString();
        }

        public static DTO.RetornoComum.Envelope LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(DTO.RetornoComum.Envelope));
            return serializer.Deserialize(stringReader) as DTO.RetornoComum.Envelope;
        }

    }
}
