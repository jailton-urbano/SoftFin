using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class NCM 
    {
        [Key]
        public int id { get; set; }
        
        [MaxLength(10)]
        public string codigo { get; set; }

        [MaxLength(2)]
        public String EX { get; set; }

        [MaxLength(10)]
        public String CEST { get; set; }

        [MaxLength(10)]
        public String descricao { get; set; }

        [MaxLength(30)]
        public String segmento { get; set; }
        [MaxLength(130)]
        public String item { get; set; }
        public decimal? aliquotaNacional { get; set; }
        public decimal? aliquotaImportados { get; set; }
        public decimal? aliquotaEstadual { get; set; }
        public decimal? aliquotaMunicipal { get; set; }
    }
}
