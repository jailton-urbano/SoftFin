using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOTotal
    {
        public DTOTotal()
        {
            ICMSTot = new DTOICMSTot();
        }
        public DTOICMSTot ICMSTot { get; set; }

        public decimal valorTributos { get; set; }
        //Campo somente usado para retorno da nota
        public string vNF { get; set; }
    }
}
