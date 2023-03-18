using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOTransp
    {
        public DTOTransp()
        {
            Transporta = new DTOTransporta();
            RetTransp = new DTORetTransp();
            Vol = new List<DTOVol>();
            VeicTransp = new DTOVeicTransp();
        }

        public string modFrete { get; set; }

        public DTOTransporta Transporta { get; set; }
        public DTORetTransp RetTransp { get; set; }

        public List<DTOVol> Vol { get; set; }

        public DTOVeicTransp VeicTransp { get; set; }
    }


    public class DTORetTransp 
    {
        public string vServ  { get; set; }
        public string vBCRet  { get; set; }
        public string pICMSRet  { get; set; }
        public string vICMSRet  { get; set; }
        public string CFOP  { get; set; }
        public string cMunFG  { get; set; }
    }

    public class DTOTransporta
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string xNome { get; set; }
        public string IE { get; set; }
        public string xEnder { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
    }
}
