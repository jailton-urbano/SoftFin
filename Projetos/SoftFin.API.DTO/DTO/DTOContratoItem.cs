using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOContratoItem
    {
        public string Codigo { get; set; }
        public decimal Valor { get; set; }

        public string TipoContrato { get; set; }

        public string UnidadeNegocio { get; set; }
        public string Contrato { get; set; }
        public List<DTOContratoItemUnidade> DTOContratoItemUnidades { get; set; }

        public DTOParcelaContrato DTOParcelaContrato { get; set; }
        public string CodigoEstab { get; set; }
    }
}