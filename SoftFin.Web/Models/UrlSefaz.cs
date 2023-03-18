using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class UrlSefaz
    {
        [Key]
        public int id { get; set; }
        [MaxLength(50)]
        public string descricao { get; set; }
        [MaxLength(250)]
        public string url { get; set; }
        [MaxLength(250)]
        public string urlHomologacao { get; set; }
    }
}