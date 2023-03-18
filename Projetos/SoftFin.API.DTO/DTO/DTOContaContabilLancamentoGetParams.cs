using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    /// <summary>
    /// Paramentros de pesquisa de Lançamento Contabeis
    /// </summary>
    public class DTOContaContabilLancamentoGetParams
    {
        /// <summary>
        /// Código Estabelecimento
        /// </summary>
        public string codigo_estab { get; set; }
        /// <summary>
        /// Data Inicial de Lançamento
        /// </summary>
        public DateTime data_lancamento_ini { get;  set; }
        /// <summary>
        /// Data Final de Lançamento
        /// </summary>
        public DateTime data_lancamento_fim { get; set; }

    }
}