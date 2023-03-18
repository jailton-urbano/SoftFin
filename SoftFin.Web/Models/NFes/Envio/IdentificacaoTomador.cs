using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class IdentificacaoTomador
    {
        public int Cpf { get; set; }
        public int Cnpj { get; set; }
        public String InscricaoMunicipal { get; set; }
    }
}