using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftFin.NFe.DTO
{
    public class DTORetornoCadastro
    {
        public DTORetornoCadastro()
        {
            InfConsObj = new InfCons();
        }
        public string CDStatus { get; set; }
        public string DSStatus { get; set; }

        public InfCons InfConsObj { get; set; }


        public class Ender
        {

            public string XLgr { get; set; }
            [XmlElement(ElementName = "nro", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string Nro { get; set; }
            [XmlElement(ElementName = "xBairro", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string XBairro { get; set; }
            [XmlElement(ElementName = "xMun", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string XMun { get; set; }
            [XmlElement(ElementName = "CEP", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CEP { get; set; }
        }

        [XmlRoot(ElementName = "infCad", Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public class InfCad
        {
            [XmlElement(ElementName = "IE", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string IE { get; set; }
            [XmlElement(ElementName = "CNPJ", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CNPJ { get; set; }
            [XmlElement(ElementName = "UF", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string UF { get; set; }
            [XmlElement(ElementName = "cSit", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CSit { get; set; }
            [XmlElement(ElementName = "indCredNFe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string IndCredNFe { get; set; }
            [XmlElement(ElementName = "indCredCTe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string IndCredCTe { get; set; }
            [XmlElement(ElementName = "xNome", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string XNome { get; set; }
            [XmlElement(ElementName = "xRegApur", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string XRegApur { get; set; }
            [XmlElement(ElementName = "CNAE", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CNAE { get; set; }
            [XmlElement(ElementName = "dIniAtiv", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string DIniAtiv { get; set; }
            [XmlElement(ElementName = "dUltSit", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string DUltSit { get; set; }
            [XmlElement(ElementName = "ender", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public Ender Ender { get; set; }
        }

        [XmlRoot(ElementName = "infCons", Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public class InfCons
        {
            [XmlElement(ElementName = "verAplic", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string VerAplic { get; set; }
            [XmlElement(ElementName = "cStat", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CStat { get; set; }
            [XmlElement(ElementName = "xMotivo", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string XMotivo { get; set; }
            [XmlElement(ElementName = "UF", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string UF { get; set; }
            [XmlElement(ElementName = "CNPJ", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CNPJ { get; set; }
            [XmlElement(ElementName = "dhCons", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string DhCons { get; set; }
            [XmlElement(ElementName = "cUF", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public string CUF { get; set; }
            [XmlElement(ElementName = "infCad", Namespace = "http://www.portalfiscal.inf.br/nfe")]
            public InfCad InfCad { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            public string XNome { get; set; }
        }
    }
}
