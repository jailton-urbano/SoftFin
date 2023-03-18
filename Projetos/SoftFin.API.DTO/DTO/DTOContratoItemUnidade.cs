using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOContratoItemUnidade
    {
        public string Unidade { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string CodigoEstab { get; set; }
        public string CodigoContratoItem { get; set; }
    }
}