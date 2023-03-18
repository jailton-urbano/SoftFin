using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;

namespace SoftFin.Web.Models
{
    public class PagamentoFiltro
    {
        [Display(Name = "Pessoa")]
        public int pessoa_id { get; set; }

        [Display(Name = "Razão Fornecedor"), StringLength(50)]
        public string razaoFornecedor { get; set; }

        [Display(Name = "CNPJ"), StringLength(18)]
        public string cnpj { get; set; }

        [Display(Name = "CPF"), StringLength(15)]
        public string cpf { get; set; }

        [Display(Name = "Número Documento"), StringLength(15)]
        public string numeroDoc { get; set; }

        [Display(Name = "Emissão Doc. Inicial")]
        public DateTime? dataEmissaoDocIni { get; set; }

        [Display(Name = "Emissão Doc. Final")]
        public DateTime? dataEmissaoDocFim { get; set; }

        [Display(Name = "Vencimento Inicial")]
        [DataType(DataType.Date)]
        public DateTime? dataVencimentoDocIni { get; set; }

        [Display(Name = "Vencimento Final")]
        [DataType(DataType.Date)]
        public DateTime? dataVencimentoDocFim { get; set; }


        [Display(Name = "Data de Pagto Inicial")]
        [DataType(DataType.Date)]
        public DateTime? dataPagamentoIni { get; set; }

        [Display(Name = "Data de Pagto Final")]
        [DataType(DataType.Date)]
        public DateTime? dataPagamentoFim { get; set; }

        [Display(Name = "Histórico"), StringLength(30)]
        public string historico { get; set; }

        [Display(Name = "Banco"), StringLength(30)]
        public string banco_id { get; set; }

        [Display(Name = "Valor Documento Inicial")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorDocumentoIni { get; set; }

        [Display(Name = "Valor Documento Final")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorDocumentoFim { get; set; }

        [Display(Name = "Valor Pagamento Inicial")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorPagamentoIni { get; set; }

        [Display(Name = "Valor Pagamento Final")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorPagamentoFim { get; set; }

        [Display(Name = "Em aberto"), StringLength(15)]
        public bool emaberto { get; set; }

    }
}