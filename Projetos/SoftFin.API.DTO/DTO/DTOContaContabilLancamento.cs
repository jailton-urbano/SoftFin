using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{

    public class DTOContaContabilLancamento
    {
        public DTOContaContabilLancamento()
        {
            DTOContaContabilLancamentoDetalhes = new List<DTOContaContabilLancamentoDetalhe>();
        }
        public string codigo_estab { get; set; }

        public string data_lancamento { get; set; }
        
        public string historico_lancamento { get; set; }

        public string origem_lancamento { get; set; }

        public int codigo_lancamento { get;  set; }

        public List<DTOContaContabilLancamentoDetalhe> DTOContaContabilLancamentoDetalhes { get; set; }
    }

    public class DTOContaContabilLancamentoDetalhe
    {
        public string codigo_conta_contabil { get; set; }

        public string descricao_conta_contabil { get; set; }

        public decimal valor_lancamento { get; set; }

        public string codigo_unidade_negocio_centro_custos { get; set; }
        public string flag_debito_credito { get;  set; }
    }



}