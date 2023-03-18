using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SoftFin.Migrate.NFSe.DTO;
using SoftFin.Migrate.NFSe.WebService;

namespace SoftFin.Migrate.NFSe.Business
{
    public class ConsultaNFServico
    {
        public DTO.RetornoConsulta.Envelope Execute(Consulta consulta, string empresaPK, string estabHash)
        {
            var retorno = new DTO.RetornoConsulta.Envelope();
            var cy = new ClienteInvoiCy();
            if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
            {
                consulta.tpAmb = "1";
                cy.UrlWs = "https://web.invoicy.com.br/arecepcao.aspx";
            }
            else
            {
                consulta.tpAmb = "2";
                cy.UrlWs = "https://homolog.invoicy.com.br/arecepcao.aspx";
            }
            var xml = ToXML(consulta);
            var xmllimpo = xml.Replace("\r\n","");
            
            xmllimpo = xmllimpo.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            xmllimpo = xmllimpo.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            xmllimpo = xmllimpo.Replace(" ", "");
            var hash = estabHash;
            var hashGerado = Utils.UtilSoftFin.GetMd5Hash(hash + xmllimpo);
            cy.Soap = cy.EscreveSoap(xmllimpo, empresaPK, hashGerado);
            cy.ExecutaWS();
            retorno = LoadFromXMLString(cy.SoapRet);
            retorno.xml = xml;
            return retorno;
        }

        public string ToXML(Consulta envio)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(Consulta));
            serializer.Serialize(stringwriter, envio);
            return stringwriter.ToString();
        }

        public static DTO.RetornoConsulta.Envelope LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(DTO.RetornoConsulta.Envelope));
            return serializer.Deserialize(stringReader) as DTO.RetornoConsulta.Envelope;
        }

    }
}
