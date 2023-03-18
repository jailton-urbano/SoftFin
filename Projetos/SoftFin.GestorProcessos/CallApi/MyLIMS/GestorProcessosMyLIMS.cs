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

namespace SoftFin.GestorProcessos.CallApi.MyLIMS
{

    public class GestorProcessosMyLIMS : BaseCallApiMyLIMS
    {
        internal string CarregarGrid(string url)
        {
            var ret = base.GetSyncResult(url);
            return ret;
        }

        internal string ExecutaIntegracao(string url, string contrato)
        {
            var ret = base.GetSyncResult(url + "?contrato=" + contrato);
            return ret;
        }
    }
}
