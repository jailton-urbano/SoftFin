using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class EnderecoTomador
    {
        public String Endereco { get; set; }
        public int Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public int CodigoMunicipio { get; set; }
        public int CodigoMunicipioSiafi { get; set; }
        public String Uf { get; set; }
        public int? Cep { get; set; }
        public int? CodigoMunicipioOutros { get; set; }

    }
}