using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOIPI
    {
        public DTOIPI()
        {
            IPITrib = new DTOIPITrib();
        }
        public DTOIPITrib IPITrib { get; set; }

        public string cEnq { get; set; }
    }
}
