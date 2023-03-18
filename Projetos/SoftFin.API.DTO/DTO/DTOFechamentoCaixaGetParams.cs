using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOFechamentoCaixaGetParams
    {
        public string codigo_estab { get; set; }
        public string codigo_loja { get; set; }
        public DateTime? data_fechamento_ini { get; set; }
        public DateTime? data_fechamento_fim { get; set; }
        
    }
}