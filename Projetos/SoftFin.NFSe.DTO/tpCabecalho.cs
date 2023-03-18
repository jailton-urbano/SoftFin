using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SoftFin.NFSe.DTO
{
    public class tpCabecalho
    {
        public tpCabecalho()
        {
            CPFCNPJRemetente = new tpCPFCNPJ();
        }

        [XmlAttribute]
        public String Versao { get; set; }
        public tpCPFCNPJ CPFCNPJRemetente { get; set; }
        public String transacao { get; set; }
        public String dtInicio { get; set; }
        public String dtFim { get; set; }
        public String QtdRPS { get; set; }
        public String ValorTotalServicos { get; set; }
        public String ValorTotalDeducoes { get; set; }

        public string Sucesso { get; set; }

        public string Inscricao { get; set; }
    }
}