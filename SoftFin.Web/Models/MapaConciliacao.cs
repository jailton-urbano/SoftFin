using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;

namespace SoftFin.Web.Models
{
    public class MapaConciliacao
    {

        public string data {get; set;}
        public string statusSaldoReal { get; set; }
        public string statusSaldoCalculado { get; set; }
        public string statusLanctosConciliacao { get; set; }

    }

    public class MapaConciliacao2
    {
        public string data { get; set; }
        public Boolean statusSaldoReal { get; set; }
        public Boolean statusSaldoCalculado { get; set; }
        public Boolean statusLanctosConciliacao { get; set; }

    }
}