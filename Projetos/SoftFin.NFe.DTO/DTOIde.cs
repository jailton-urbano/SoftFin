using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOIde
    {
        public DTOIde()
        {
            NFref = new List<DTONFref>();
        }

        public List<DTONFref> NFref { get; set; }

        public string cUF { get; set; }
        public string cNF { get; set; }
        public string natOp { get; set; }
        public string indPag { get; set; }
        public string mod { get; set; }
        public string serie { get; set; }
        public string nNF { get; set; }
        public string dhEmi { get; set; }
        public string dhSaiEnt { get; set; }
        public string tpNF { get; set; }
        public string idDest { get; set; }

        public string cMunFG { get; set; }
        public string tpImp { get; set; }
        public string tpEmis { get; set; }
        public string cDV { get; set; }
        public string tpAmb { get; set; }

        public string finNFe { get; set; }
        public string indFinal { get; set; }
        public string indPres { get; set; }
        public string procEmi { get; set; }
        public string verProc { get; set; }
        public string chavenota { get; set; }
        public string nProt { get; set; }
        //Campo somente usado para retorno da nota
        public string digVal { get; set; }
        public string dhRecbto { get; set; }
        public string cSitNFe { get; set; }
    }
}
