using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Web.CallApi.CallApi
{
    public class Log: BaseLog
    {
        string _uriService = "";
        public Log()
        {
            _uriService = ConfigurationManager.AppSettings["uriApiSoftFinLog"];
        }


        public string PostEvento(SoftFin.Infrastructure.DTO.DTOLogEvento evento)
        {
            evento.CodigoSistema = "SoftFin.Web";

            string objSerializado = JsonConvert.SerializeObject(evento);
            base.PostSync(_uriService + "/Envento/Incluir", objSerializado);
            return "OK";
        }

        public void PostEventoAsync(SoftFin.Infrastructure.DTO.DTOLogEvento evento)
        {
            evento.CodigoSistema = "SoftFin.Web";

            string objSerializado = JsonConvert.SerializeObject(evento);
            base.PostSync(_uriService + "/Envento/Incluir", objSerializado);
        }


        public void PostComercialAsync(SoftFin.Infrastructure.DTO.DTOLogComercial comercial)
        {
            comercial.CodigoSistema = "SoftFin.Site";
            comercial.Estabelecimento = "softfin.com.br";


            string objSerializado = JsonConvert.SerializeObject(comercial);
            base.PostSync(_uriService + "/Comercial/Incluir", objSerializado);
        }

        //public string PostImportar(SoftFin.Infrastructure.DTO.DTOLogImportar log)
        //{
        //    log.CodigoSistema = "SoftFin.Web";

        //    string objSerializado = JsonConvert.SerializeObject(log);
        //    var ret = base.PostSync(_uriService + "/Importacao/Incluir", objSerializado);
        //    return ret;
        //}

    }
}
