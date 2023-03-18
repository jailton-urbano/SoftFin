using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOImposto
    {
        public DTOImposto()
        {
            PIS = new DTOPIS();
            COFINS = new DTOCOFINS();
            IPI = new DTOIPI();
        }
        public string vTotTrib { get; set; }


        public DTOPIS PIS { get; set; }
        public DTOCOFINS COFINS { get; set; }

        public DTOIPI IPI { get; set; }


    }
}
