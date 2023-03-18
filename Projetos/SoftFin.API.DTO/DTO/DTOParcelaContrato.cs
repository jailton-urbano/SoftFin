using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOParcelaContrato
    {
        public ENUMRecorrente Recorrente { get; set; }
        public int Parcela { get; set; }
        public string Codigo { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public string CodigoEstab { get; set; }
        public string ContratoItem { get; set; }
        public int DiaVencimento { get; set; }
        public string BancoReferencia { get; set; }
        public string Prazo { get; set; }
    }

    public enum ENUMRecorrente
    {
        Sim = 1,
        Não = 2,
        SimReverso = 3
    }
}