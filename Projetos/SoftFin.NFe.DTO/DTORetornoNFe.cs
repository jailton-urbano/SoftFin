
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoftFin.NFe.DTO
{
    public class DTORetornoNFe
    {
        public XmlDocument xml { get; set; }

        public string tipo { get; set; }
        public bool Sucesso { get; set; }

        public List<DTOErro> Erros { get; set; }
        public List<DTOErro> Alertas { get; set; }

        public List<DTOInfNFe> NFe { get; set; }

        public string nRec { get; set; }

        public string xmlRetorno { get; set; }

        public string chaveAcesso { get; set; }

        public string protocoloAutorizacao { get; set; }
    }
}
