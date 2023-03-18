using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftFin.NFe.DTO
{
    public class DTORetornoNotaEntrada
    {
        public string CNPJ { get; set; }
        public string chNFe { get; set; }
        public string xNome { get; set; }
        public string IE { get; set; }
        public string dhEmi { get; set; }
        public string vNF { get; set; }
        public string digVal { get; set; }
        public string dhRecbto { get; set; }
        public string nProt { get; set; }

        public string cSitNFe { get; set; }
        public string tpNF { get; set; }
        public string xmlCompleto { get; set; }
        public string xJust { get; set; }
        public string xEvento { get; set; }
    }
}
