using Newtonsoft.Json;
using SoftFin.MyLIMS.Integrar.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftFin.MyLIMS.Integrar.DTO.Works;

namespace SoftFin.MyLIMS.Integrar.CallApi
{
    public class Origem: BaseMyLIMS
    {
        string _uriService = "";
        public Origem()
        {
            _uriService = ConfigurationManager.AppSettings["uriApiMyLims"];
        }

        public String CheckConnection()
        {
            var ret = base.GetSync<string>(_uriService + "api/v2/checkConnection", true);
            return ret; 
        }

        public Works.RootObject GetWorks()
        {
            var ret = base.GetSync<Works.RootObject>(_uriService + "api/v2/works", true);
            return ret;
        }

        public RootObject WorkClasses()
        {
            var ret = base.GetSync<RootObject>(_uriService + "api/v2/workclasses",true);
            return ret;
        }

    }
}
