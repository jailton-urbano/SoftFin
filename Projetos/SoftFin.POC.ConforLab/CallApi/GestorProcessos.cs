using Newtonsoft.Json;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.POC.ConforLab
{
    public class GestorProcessos: BaseCallApi
    {
        string _uri = ConfigurationManager.AppSettings["urlGestorProcessos"].ToString();
 
        public DTOProximaAtividade GeraProximaAtividade(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<DTOProximaAtividade>(_uri + "AtividadeExecucao/GerarProximaAtividade", jsonstr);
            return ret;
        }


    }
}
