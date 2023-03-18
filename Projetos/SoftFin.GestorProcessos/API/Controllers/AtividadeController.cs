using Newtonsoft.Json;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SoftFin.GestorProcessos.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AtividadeController : ApiController
    {
        [System.Web.Http.HttpPost]
        public List<DTOListaProcessos> Listar(ParamProcesso paramProcesso)
        {
            var lista = new List<DTOListaProcessos>();
            return lista;
        }
        [System.Web.Http.HttpPost]
        public DTOProximaAtividade GerarProxima(string CodigoAtividade, string resultado)
        {
            var entidade = new DTOProximaAtividade();
            return entidade;
        }
    }
}