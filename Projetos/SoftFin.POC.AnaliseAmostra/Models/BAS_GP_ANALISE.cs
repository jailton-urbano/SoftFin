using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.POC.AnaliseAmostra.Models
{
    public class BAS_GP_ANALISE
    {
        [Key]
        public int BGA_ID { get; set; }

        [MaxLength(400)]
        public string BGA_GRUPO { get; set; }
    }
}