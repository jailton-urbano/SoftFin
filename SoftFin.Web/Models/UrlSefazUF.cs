using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class UrlSefazUF
    {
        [Key]
        public int id { get; set; }
        [MaxLength(50)]
        public string UF { get; set; }

        [Display(Name = "Url Principal")]
        public int UrlSefazPrincipal_id { get; set; }

        [JsonIgnore,ForeignKey("UrlSefazPrincipal_id")]
        public virtual UrlSefaz UrlSefazPrincipal { get; set; }

        [Display(Name = "Url Secundario")]
        public int? UrlSefazSecundario_id { get; set; }

        [JsonIgnore,ForeignKey("UrlSefazSecundario_id")]
        public virtual UrlSefaz UrlSefazSecundario { get; set; }

        [MaxLength(50)]
        public string codigo { get; set; }

    }
}