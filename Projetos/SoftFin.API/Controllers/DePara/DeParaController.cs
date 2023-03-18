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
    public class DeParaController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("DeParaItem/ObterTodos")]
        [HttpGet]
        public DTOGenericoRetorno<List<DTODeParaItem>> ObterTodos(string codigoEstab, string codigoDePara)
        {
            var objRetorno = new DTOGenericoRetorno<List<DTODeParaItem>>();
            objRetorno.status = "NOK";
            try
            {
                codigoDePara = codigoDePara.Replace("'", "");
                var objestab = new Estabelecimento().ObterPorCodigoValidandoUsuario(codigoEstab, _usuario);
                _paramBase.estab_id = objestab.id;
                _paramBase.empresa_id = objestab.Empresa_id;
                //Validador(objParams, objRetorno);

                var dePara = new DeParaMestre().ObterPorCodigo(codigoDePara, _paramBase);
                var deParaItems = new DeParaItem().ObterPorIdMestre(dePara.Id, _paramBase);

                objRetorno.objs = new List<DTODeParaItem>();
                foreach (var item in deParaItems)
                {
                    objRetorno.objs.Add(new DTODeParaItem {  De = item.De, Para = item.Para });
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