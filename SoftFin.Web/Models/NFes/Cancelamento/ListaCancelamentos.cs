using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Cancelamento
{
    public class ListaCancelamentos
    {
        List<Cancelamento> Cancelamento { get; set; }
    }
    public class Cancelamento
    {
        List<InfPedidoCancelamento> InfPedidoCancelamento {get; set;}
    }

    public class InfPedidoCancelamento
    {

        public String Id { get; set; }
        public int Versao { get; set; }
        public int Numero { get; set; }
        public int Cnpj { get; set; }
        public int InscricaoMunicipal { get; set; }
        public int CodigoMunicipio { get; set; }
        public String CodigoCancelamento { get; set; }
    }
}