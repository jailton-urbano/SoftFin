using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.MyLIMS.Integracao.CallApi
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
            evento.Estabelecimento = ConfigurationManager.AppSettings["empresa"].ToString() + ":" + ConfigurationManager.AppSettings["estab"].ToString(); 
            string objSerializado = JsonConvert.SerializeObject(evento);
            var ret = base.PostSync<string> (_uriService + "/Envento/Incluir", objSerializado);
            return ret;
        }

        public string PostImportar(SoftFin.Infrastructure.DTO.DTOLogImportar log)
        {
            log.Estabelecimento = ConfigurationManager.AppSettings["empresa"].ToString() + ":" + ConfigurationManager.AppSettings["estab"].ToString();
            log.CodigoSistema = "MyLins.Integra";
            
            string objSerializado = JsonConvert.SerializeObject(log);
            var ret = base.PostSync<string>(_uriService + "/Importacao/Incluir", objSerializado);
            return ret;
        }

    }
}
