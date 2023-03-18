using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOVol
    {
        public DTOVol()
        {
            lacres = new List<DTOLacres>();
        }
        public List<DTOLacres> lacres { get; set; }

        public string qVol { get; set; }

        public string esp { get; set; }

        public string marca { get; set; }

        public string nVol { get; set; }

        public string pesoL { get; set; }

        public string pesoB { get; set; }
    }
}
