using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTODest
    {
        public DTODest()
        {
            EnderDest = new DTOEnderDest();
        }

        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string idEstrangeiro { get; set; }
        public string xNome { get; set; }

        public DTOEnderDest EnderDest { get; set; }
        public string indIEDest { get; set; }
        public string IE { get; set; }
        public string ISUF { get; set; }
        public string IM { get; set; }
        public string email { get; set; }
    }

    public class DTOEnderDest
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
        
    }

}
