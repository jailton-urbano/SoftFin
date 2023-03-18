using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class Rps
    {
        public Rps()
        {
            InfRps = new List<InfRps>();
        }
        public List<InfRps> InfRps { get; set; }
    }
}