using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class VisaoHashcode
    {
        [Key]
        public int Id { get; set; }
        public string Valor { get; set; }
        public bool UsarPadrao { get; set; }

        public int IdHashcode { get; set; }

        [JsonIgnore, ForeignKey("IdHashcode")]
        public virtual Hashcode Hashcode { get; set; }

    }
}