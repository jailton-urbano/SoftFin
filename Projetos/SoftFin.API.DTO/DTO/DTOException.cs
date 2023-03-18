using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOException
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }
    }
}