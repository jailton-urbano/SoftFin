using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{

    public class DTOFechamentoCaixa
    {
        public string codigo_estab { get; set; }

        public DateTime data_fechamento { get; set; }

        public int sequencia { get; set; }

        public string descricao { get; set; }

        public string codigo_loja { get; set; }

        public string codigo_operador { get; set; }

        public string codigo_caixa { get; set; }

        public decimal saldo_inicial { get; set; }

        public decimal saldo_final { get; set; }

        public decimal valor_bruto { get; set; }

        public decimal valor_liquido { get; set; }

        public decimal valor_taxas { get; set; }

        public List<DTOFechamentoCaixaDetalhe> fechamento_caixa_detalhes { get; set; }
        
    }
}