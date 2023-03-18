using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class BaseDTORetorno
    {
        public BaseDTORetorno()
        {
            Exceptions = new List<DTOException>();
        }
        public string status { get; set; }
        public List<DTOException> Exceptions { get; set; }

    }
}
