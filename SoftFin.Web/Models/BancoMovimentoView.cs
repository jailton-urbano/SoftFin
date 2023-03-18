using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SoftFin.Web.Models
{
    public class BancoMovimentoView
    {
        [Display(Name = "Banco")]
        public int? banco_id { get; set; }

        [Display(Name = "Plano de Contas")]
        public int? planoDeConta_id { get; set; }
        
        [Display(Name = "Origem Movimento")]
        public int? origemmovimento_id { get; set; }

        [Display(Name = "Tipo de Movimento")]
        public int? tipoDeMovimento_id { get; set; }

        [Display(Name = "Tipo de Documento")]
        public int? tipoDeDocumento_id { get; set; }
        
        [Display(Name = "Unidade de Negócio")]
        public int? unidadeDeNegocio_id { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? dataIni { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? dataFim { get; set; }

        [Display(Name = "Historico")]
        public string historico { get; set; }

        [Display(Name = "Valor Inicial")]
        public decimal? valorIni { get; set; }

        [Display(Name = "Valor Final")]
        public decimal? valorFim { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string notafiscal { get; set; }

        [Display(Name = "Nº Contas Pagar")]
        public string DocumentoPagarMestre { get; set; }

    }
}
