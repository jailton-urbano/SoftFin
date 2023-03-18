using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.POC.ConforLab.Models
{
    public class BAS_CAD_TECNICO
    {
        [Key]
        public int BCT_ID { get; set; }

        [MaxLength(400)]
        public string BCT_NOME  { get; set; }
    }
}