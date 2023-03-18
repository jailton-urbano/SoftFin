using SoftFin.API.DTO;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SoftFin.API.Controllers.caixa
{
    /// <summary>
    /// Rotina de Manutenção de Loja
    /// </summary>
    public class UnidadeNegocioController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("UnidadeNegocio/ObterTodos")]
        [HttpGet]
        public DTOGenericoRetorno<List<DTOUnidadeNegocio>> ObterTodos(string codigoEstab)
        {
            var objRetorno = new DTOGenericoRetorno<List<DTOUnidadeNegocio>>();
            objRetorno.status = "NOK";
            try
            {
                var objestab = new Estabelecimento().ObterPorCodigoValidandoUsuario(codigoEstab, _usuario);
                _paramBase.estab_id = objestab.id;
                _paramBase.empresa_id = objestab.Empresa_id;
                //Validador(objParams, objRetorno);

                var unidadeNegocios = new UnidadeNegocio().ObterTodos(_paramBase);

                objRetorno.objs = new List<DTOUnidadeNegocio>();
                foreach (var item in unidadeNegocios)
                {
                    objRetorno.objs.Add(new DTOUnidadeNegocio { Unidade = item.unidade });
                }

                objRetorno.status = "OK";
                return objRetorno;
            }
            catch (Exception ex)
            {
                objRetorno.exceptions.Add(new DTOException { codigo = "Error Execution", descricao = ex.Message.ToString(), id = 1, tipo = "Error" });
                objRetorno.status = "NOK";
                return objRetorno;
            }
        }

    }
}