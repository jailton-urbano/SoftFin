using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SoftFin.API
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MakeTokenEstabController : BaseApi
    {
        // GET api/<controller>/5
        public string Get(int idEstab)
        {
            //return "MakeTokenController";

            DbControle db = new DbControle();

            var usuario = new Usuario().ObterPorCodigoAtivo(_paramBase.usuario_name, db);

            usuario.tokenApi = SoftFin.Utils.Crypto.Encryption(_paramBase.usuario_name + ";"
                + ";" + idEstab.ToString()
                + ";" + DateTime.Now.ToString()
                );

            return usuario.tokenApi;
        }


    }
}