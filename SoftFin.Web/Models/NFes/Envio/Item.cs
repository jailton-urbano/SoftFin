using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class Item
    {
        public String ItemListaServico { get; set; }
        public int CodigoCnae { get; set; }
        public String DiscriminacaoServico { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public int ServicoTributavel { get; set; }
        public String CodigoTributacaoMunicipio { get; set; }
        public decimal VlrAliquota { get; set; }
    }
}