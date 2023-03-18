using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTONFref
    {
        public DTONFref()
        {
            refNF = new DTORefNF();
            refNFP = new DTORefNFP();
            refECF = new DTORefECF();
        }

        public string refNFe { get; set; }
        public DTORefNF refNF { get; set; }
        public DTORefNFP refNFP { get; set; }
        public DTORefECF refECF { get; set; }
    }
}
