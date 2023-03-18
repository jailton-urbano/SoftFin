using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class DocumentoPagarMestreEmAbertoView
    {
        public string id { get; set; }
        public int? numeroDocumento { get; set; }
        public string razaoFornecedor { get; set; }
        public DateTime dataEmissaoDoc { get; set; }
        public DateTime? dataVencimentoDoc { get; set; }
        public DateTime? dataPagamento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorPagamento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorTotal { get; set; }
        public string historico { get; set; }
        public string banco { get; set; }
        public string status { get; set; }

    }
}