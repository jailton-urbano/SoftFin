using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class DocumentoPagarMestreVlw
    {
        [Display(Name = "Pessoa")]
        public int pessoa_id { get; set; }

        [Display(Name = "Data Lançamento Inicial:"), DataType(DataType.Date)]
        public DateTime? dataLancamentoIni { get; set; }

        [Display(Name = "Data Lançamento Final"), DataType(DataType.Date)]
        public DateTime? dataLancamentoFim { get; set; }
        
        [Display(Name = "Data Competencia"),  StringLength(7)]
        public string dataCompetencia { get; set; }

        [Display(Name = "Data Vencimento Inicial"), DataType(DataType.Date)]
        public DateTime? dataVencimentoIni { get; set; }

        [Display(Name = "Data Vencimento Final"), DataType(DataType.Date)]
        public DateTime? dataVencimentoFim { get; set; }
        
        [Display(Name = "Valor Bruto Inicial")]
        public decimal? valorBrutoIni { get; set; }

        [Display(Name = "Valor Bruto Final")]
        public decimal? valorBrutoFim { get; set; }

        [Display(Name = "Tipo Documento")]
        public int tipodocumento_id { get; set; }

        [Display(Name = "Tipo Lançamento")]
        public string tipolancamento { get; set; }

        [Display(Name = "Número Documento")]
        public int? numeroDocumento { get; set; }

        [Display(Name = "Data Documento Inicial"), DataType(DataType.Date)]
        public DateTime? dataDocumentoIni { get; set; }

        [Display(Name = "Data Documento Final"), DataType(DataType.Date)]
        public DateTime? dataDocumentoFim { get; set; }

        [Display(Name = "Situação Pagamento")]
        public string situacaoPagamento { get; set; }

        [Display(Name = "Data Pagamento Inicial"), DataType(DataType.Date)]
        public DateTime? dataPagamentoIni { get; set; }

        [Display(Name = "Data Pagamento Final"), DataType(DataType.Date)]
        public DateTime? dataPagamentoFim { get; set; }

        [Display(Name = "Codigo Pagamento"),]
        public int? codigoPagamento { get; set; }

        [Display(Name = "Lote Pagamento Banco"), StringLength(15)]
        public string lotePagamentoBanco { get; set; }

        [Display(Name = "Documento Pagar Aprovacao")]
        public int? documentopagaraprovacao_id { get; set; }


    }
}
