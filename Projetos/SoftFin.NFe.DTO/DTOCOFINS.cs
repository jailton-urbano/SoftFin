using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class    DTOCOFINS
    {
        public DTOCOFINS()
        {
            COFINSNT = new DTOCOFINSNT();
        }
        public DTOCOFINSNT COFINSNT { get; set; }

        public string CST { get; set; }

        public string vBC { get; set; }

        public string pCOFINS { get; set; }

        public string vCOFINS { get; set; }
    }
}
