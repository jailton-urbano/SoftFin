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
    public class MakeTokenController : ApiController
    {
        // GET api/<controller>/5
        public string Get(string codigoUsuario)
        {
            DbControle db = new DbControle();
            var usuario = new Usuario().ObterPorCodigoAtivo(codigoUsuario, db);
            usuario.tokenApi = SoftFin.Utils.Crypto.Encryption(
                usuario.id.ToString() + ";" + DateTime.Now.ToString("o"));
            usuario.Alterar(usuario, null, db);
            return usuario.tokenApi;
        }
    }
}