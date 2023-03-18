using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class ExtratoSeguradora
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Data Lancto"), Required(ErrorMessage = "*")]
        public DateTime dataLancto { get; set; }

        [Display(Name = "Valor"), Required(ErrorMessage = "*")]
        public decimal valor { get; set; }


        [Display(Name = "Histórico"),
        Required(ErrorMessage = "*"),
        StringLength(35)]
        public string historico { get; set; }

    }
}