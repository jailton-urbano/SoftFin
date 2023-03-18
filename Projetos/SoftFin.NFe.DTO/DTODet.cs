using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTODet
    {
        public DTODet()
        {
            Prod = new DTOProd();
            Imposto = new DTOImposto();
            
        }
        public string nItem { get; set; }
        public DTOProd Prod { get; set; }
        public DTOImposto Imposto { get; set; }
        
    }
}
