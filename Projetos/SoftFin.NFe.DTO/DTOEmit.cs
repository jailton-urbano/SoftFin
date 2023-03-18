using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOEmit
    {
        public DTOEmit()
        {
            EnderEmit = new DTOEnderEmit();
        }

        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string xNome { get; set; }
        public string xFant { get; set; }

        public DTOEnderEmit EnderEmit { get; set; }

        public string IM { get; set; }
        public string CNAE { get; set; }

        public string CRT { get; set; }

        public string IE { get; set; }
    }

    public class DTOEnderEmit
    {
        public string xLgr { get; set; }
        public string nro { get; set; }
        public string xCpl { get; set; }

        public string xBairro { get; set; }
        public string cMun { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public string cPais { get; set; }
        public string xPais { get; set; }






        public string fone { get; set; }

        public string IE { get; set; }

        public string CNAE { get; set; }
        public string CRT { get; set; }

    }
}
