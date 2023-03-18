using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    /// <summary>
    /// Paramentros de Manutenção de Loja
    /// </summary>
    public class DTOStoreParams
    {
        /// <summary>
        /// Código do Loja
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Descrição do Loja
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Loja Ativa
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// Código do Estabelecimento
        /// </summary>
        public string code_estab { get; set; }
    }
}