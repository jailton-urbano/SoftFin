using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class Deducao
    {
        public int DeducaoPor { get; set; }
        public int? TipoDeducao { get; set; }
        public int? CPFCNPJReferencia { get; set; }
        public int? ValorTotalReferencia { get; set; }
        public int? ValorDeduzir { get; set; }
        public int? PercentualDeduzir { get; set; }
        public int? NumeroNFReferencia { get; set; }
        public String JustificativaDeducao { get; set; }

    }
}