using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Adianta.DTO
{
    public class DTOEmpresa
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string revenue { get; set; }
        public string email { get; set; }

        public DTOaddress address { get; set; }
        public DTOadmin admin { get; set; }
    }
    public class DTOaddress
    {
        public string street { get; set; }
        public string no { get; set; }
        public string comp { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class DTOadmin
    {
        public string name { get; set; }
        public string CPF { get; set; }
        public string phone { get; set; }

        public string email { get; set; }
        public string rg { get; set; }
        public string position { get; set; }
        public DTOaddress address { get; set; }

    }

    
}
