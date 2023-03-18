using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SoftFin.Web.Models
{
    public class ParamBase
    {
        public bool tokenValidado;
        public int perfil_id { get; set; }
        public int empresa_id { get; set; }
        public int estab_id { get; set; }
        public int usuario_id { get; set; }
        public string usuario_name { get; set; }
        public string usuario_ip { get; set; }
        public string usuario_action { get; set; }
        public string usuario_controller { get; set; }
        public int municipio_id { get;  set; }
        public int holding_id { get; set; }
    }
}