using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotasEmAbertoView
    {
        public string id { get; set; }
        public int? numeroNfse { get; set; }
        public DateTime dataEmissaoNfse { get; set; }
        public DateTime? dataVencimentoNfse { get; set; }
        public DateTime? dataRecebimento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorRecebimento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorLiquido { get; set; }
        
        public string historico { get; set; }
        public string banco { get; set; }
        public string situacao { get; set; }

    }
}