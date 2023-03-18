using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class Valores
    {
        public decimal ValorServicos { get; set; }
        public decimal ValorDeducoes { get; set; }
        public decimal ValorPis { get; set; }
        public decimal ValorCofins { get; set; }
        public decimal ValorInss { get; set; }
        public decimal ValorIr { get; set; }
        public decimal ValorCsll { get; set; }
        public decimal? ValorIss { get; set; }
        public decimal? ValorIssRetido { get; set; }
        public decimal? OutrasRetencoes { get; set; }
        public decimal BaseCalculo { get; set; }
        public decimal Aliquota { get; set; }
        public decimal ValorLiquidoNfse { get; set; }
        public decimal? DescontoIncondicionado { get; set; }
        public decimal? DescontoCondicionado { get; set; }

    }
}