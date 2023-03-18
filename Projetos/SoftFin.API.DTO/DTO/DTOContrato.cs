using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOContrato
    {
        public DTOContrato()
        {
            DTOVendedor = new List<DTOPessoa>();
            DTOClientes = new List<DTOPessoa>();
        }
        public string Codigo { get; set; }
        public string CodigoEstab { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Prazo { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? InicioVigencia { get; set; }
        public DateTime? FimVigencia { get; set; }

        public string CodigoCliente { get; set; }
        public string CodigoVendedor { get; set; }

        public string CodigoMunicipioIBGE { get; set; }

        public bool Importa { get; set; }
        public Guid ProcessoIntegrado { get; set; }
        public string Resultado { get; set; }

        public List<DTOContratoItem> DTOContratoItems { get; set; }

        public List<DTOPessoa> DTOClientes { get; set; }
        public List<DTOPessoa> DTOVendedor { get; set; }
        public string CodigoOrigem { get; set; }
        public string CondicaoPagamento { get; set; }
    }
}