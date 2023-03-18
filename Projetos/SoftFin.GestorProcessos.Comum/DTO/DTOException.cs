using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class DTOException
    {
        public int Td { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}