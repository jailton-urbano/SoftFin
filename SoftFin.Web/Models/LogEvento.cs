using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class LogEvento
    {

        [Key]
        public int id { get; set; }
        public DateTime data { get; set; }
        public string thread { get; set; }
        public string nivel { get; set; }
        public string logger { get; set; }
        public string mensagem { get; set; }
        public string excessao { get; set; }
        public string usuario { get; set; }
        public string json { get; set; }

        [MaxLength(80)]
        public string IP { get; set; }


    }
}