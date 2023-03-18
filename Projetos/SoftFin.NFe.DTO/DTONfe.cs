using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTONfe
    {
        public DTONfe()
        {
            InfNFe = new DTOInfNFe();
            Signature = new DTOSignature();
        }

        public DTOInfNFe InfNFe { get; set; }
        public DTOSignature Signature { get; set; }



        public string idLote { get; set; }
    }
}
