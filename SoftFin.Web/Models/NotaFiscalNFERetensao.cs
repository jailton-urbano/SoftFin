using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalNFERetensao
    {
        public int id { get; set; }
        public decimal PISRetido { get; set; }

        public decimal IRRFBaseRetido { get; set; }

        public decimal PrevidenciaBaseRetido { get; set; }
        public decimal CSLLRetido { get; set; }
        public decimal CONFINSRetido { get; set; }

        public decimal IRRFRetido { get; set; }
        public decimal PrevidenciaRetido { get; set; }

    }
}