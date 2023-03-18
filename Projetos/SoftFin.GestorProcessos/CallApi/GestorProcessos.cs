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
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.CallApi
{

    public class GestorProcessos : BaseCallApi
    {
        //string _uri = ConfigurationManager.AppSettings["urlGestorProcessos"].ToString();
        public GestorProcessos(string uri)
        {
            _uri = uri;
        }
        string _uri = "";

        internal string CarregarEntidade()
        {
            var ret = base.GetSyncResult(_uri);
            return ret;
        }

        internal string CarregarEntidade(string numeroProcesso)
        {
            var ret = base.GetSyncResult(_uri + "?protocolo=" + numeroProcesso);
            return ret;
        }
    }
}
