using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.POC.ConforLab.Models
{
    public class BAS_PROCEDIMENTO
    {
        [Key]
        public int BGA_ID  { get; set; }

        [MaxLength(400)]
        public string BGA_GRUPO { get; set; }
    }
}