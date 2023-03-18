using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SoftFin.Web.Models.NFes.Envio
{
    [XmlRoot("ListaRps")]
    public class ListaRps
    {
        public ListaRps()
        {
            Rps = new Rps();
        }

        public Rps Rps { get; set; }
    }
}