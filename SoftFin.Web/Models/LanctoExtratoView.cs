using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class LanctoExtratoView
    {
        [Display(Name = "Data Inicial")]
        public DateTime dataIni { get; set; }

        [Display(Name = "Data Final")]
        public DateTime dataFim { get; set; }

        [Display(Name = "Identificação Banco"), Required(ErrorMessage = "*"), MaxLength(20)]
        public string idLancto { get; set; }
        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(120)]
        public string descricao { get; set; }
        [Display(Name = "Tipo"), Required(ErrorMessage = "*"), MaxLength(1)]
        public string Tipo { get; set; }
        [Display(Name = "Valor Inicial"), Required(ErrorMessage = "*")]
        public decimal? ValorIni { get; set; }

        [Display(Name = "Valor Final"), Required(ErrorMessage = "*")]
        public decimal? ValorFim { get; set; }

        [Display(Name = "Banco"), Required(ErrorMessage = "*")]
        public int banco_id { get; set; }

    }
}