using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOCobr
    {
        public DTOCobr()
        {
            Fat = new DTOFat();
            Dup = new List<DTODup>();
        }
        public DTOFat Fat { get; set; }

        public List<DTODup> Dup { get; set; }
    }
}
