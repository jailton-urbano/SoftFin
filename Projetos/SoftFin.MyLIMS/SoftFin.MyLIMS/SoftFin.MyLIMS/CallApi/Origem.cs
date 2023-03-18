using Newtonsoft.Json;
using SoftFin.MyLIMS.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftFin.MyLIMS.DTO.Works;

namespace SoftFin.MyLIMS.CallApi
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
            var ret = base.GetSync<string>(_uriService + "api/v2/checkConnection");
            return ret; 
        }

        public Works.RootObject GetWorks()
        {
            var ret = base.GetSync<Works.RootObject>(_uriService + "api/v2/works");
            return ret;
        }

        public RootObject WorkClasses()
        {
            var ret = base.GetSync<RootObject>(uriService + "api/v2/workclasses");
            return ret;
        }

    }
}
