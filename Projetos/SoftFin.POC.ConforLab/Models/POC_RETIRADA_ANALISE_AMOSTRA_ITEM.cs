using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.POC.ConforLab.Models
{
    public class POC_RETIRADA_ANALISE_AMOSTRA_ITEM
    {
        [Key]
        public int RAC_ID { get; set; }
        public int RAI_ID { get; set; }
        [MaxLength(2)]
        public string RAI_NUMERO { get; set; }
        [MaxLength(100)]
        public string RAI_PH { get; set; }
        [MaxLength(100)]
        public string RAI_TEMP { get; set; }
        [MaxLength(100)]
        public string RAI_CLORO { get; set; }
        [MaxLength(100)]
        public string RAI_LOCAL { get; set; }
        public DateTime RAI_HORA { get; set; }
    }
}