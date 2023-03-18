using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.POC.AnaliseAmostra.Models
{
    public class BAS_TEMPERATURA
    {
        [Key]
        public int BPR_ID { get; set; }

        [MaxLength(400)]
        public string BPR_PROCEDIMENTO { get; set; }
    }
}