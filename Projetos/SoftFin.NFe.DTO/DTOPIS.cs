using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOPIS
    {
        public DTOPIS()
        {
            PISNT = new DTOPISNT(); 
        }
        public DTOPISNT PISNT { get; set; }

        public string CST { get; set; }

        public string vBC { get; set; }

        public string pPIS { get; set; }

        public string vPIS { get; set; }
    }
}
