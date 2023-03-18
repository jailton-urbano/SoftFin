using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SoftFin.Web.Models.NFes.Envio
{
    public class DadosComplementaresTomador 
    {
        public String TipoLogradouro { get; set; }
        public String TipoBairro { get; set; }
        public String CidadeDescricao { get; set; }
        public int? DDD { get; set; }
        public String InscrEstadual { get; set; }
        public String Pais { get; set; }
        public String NomeCidadeEstrangeira { get; set; }
        public String TipoDocumento { get; set; }
        public String DocumentoEstrangeiro { get; set; }
        public int? CodigoPais { get; set; }


    }
}
