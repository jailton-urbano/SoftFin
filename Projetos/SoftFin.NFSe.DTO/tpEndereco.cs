using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.NFSe.DTO
{
    public class tpEndereco
    {
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }

        public string ComplementoEndereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }


        public string CodigoMunicipio { get; set; }
    }
}