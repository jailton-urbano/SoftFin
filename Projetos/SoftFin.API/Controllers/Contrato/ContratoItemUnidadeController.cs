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
    /// Par
    /// 

        
    public class ContratoItemUnidadeController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("ContratoItemUnidade/Incluir")]
        [HttpPost]
        public DTOGenericoRetorno<DTOContratoItemUnidade> Incluir(DTOContratoItemUnidade objParams)
        {
            
            var objRetorno = new DTOGenericoRetorno<DTOContratoItemUnidade>();
            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.CodigoEstab, _usuario).id;
                Validador(objParams, objRetorno);

                if (objRetorno.exceptions.Count() > 0)
                    throw new Exception("Verificar a Validação");

                RegraIncluir(objParams,_paramBase);
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

        internal void RegraIncluir(DTOContratoItemUnidade objParams, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            var novaEntidade = new ContratoItemUnidadeNegocio();
            novaEntidade.Descricao = objParams.Descricao;
            novaEntidade.Valor = objParams.Valor;


            if (objParams.CodigoContratoItem != null)
            {
                var objaux = new ContratoItem().ObterTodos(pb, 0,db).Where(p => p.pedido == objParams.CodigoContratoItem).FirstOrDefault();
                if (objaux != null)
                {
                    novaEntidade.ContratoItem_Id = objaux.id;
                }
                else
                {
                    int contratoItem_ID;
                    
                    if (int.TryParse(objParams.CodigoContratoItem, out contratoItem_ID))
                    {
                        novaEntidade.ContratoItem_Id = contratoItem_ID;
                    }
                }
            }
            if (objParams.Unidade != null)
            {
                var unidade = objParams.Unidade.Trim();
                var objaux = new UnidadeNegocio().ObterTodos(pb, db).Where(p => p.unidade == unidade).FirstOrDefault();
                if (objaux != null)
                {
                    novaEntidade.UnidadeNegocio_Id = objaux.id;
                }
                else
                {
                    throw new Exception("Unidade não encontrada (" + objParams.Unidade + ")");
                }
            }
            novaEntidade.Incluir(novaEntidade, db);

        }

        private void Validador(DTOContratoItemUnidade objParams, DTOGenericoRetorno<DTOContratoItemUnidade> objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.CodigoContratoItem))
            {
                var objException = new DTOException();
                objException.codigo = "UC01";
                objException.descricao = "Informe o código.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }
            if (string.IsNullOrWhiteSpace(objParams.CodigoEstab))
            {
                var objException = new DTOException();
                objException.codigo = "UC02";
                objException.descricao = "Informe o código do estab.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }
        }



    }
}