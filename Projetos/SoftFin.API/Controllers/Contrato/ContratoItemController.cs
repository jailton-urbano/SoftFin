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
    public class ContratoItemController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("ContratoItem/Incluir")]
        [HttpPost]
        public DTOGenericoRetorno<DTOContratoItem> Incluir(DTOContratoItem objParams)
        {
            var objRetorno = new DTOGenericoRetorno<DTOContratoItem>();
            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.CodigoEstab, _usuario).id;
                DbControle db = new DbControle();
                Validador(objParams, objRetorno);
                if (objRetorno.exceptions.Count() > 0)
                    throw new Exception("Verificar a Validação");

                RegraIncluir(objParams,  db, _paramBase);

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


        internal int RegraIncluir(DTOContratoItem objParams, DbControle db, ParamBase pb)
        {
            var novaEntidade = new ContratoItem();
            novaEntidade.pedido = objParams.Codigo;
            novaEntidade.valor = objParams.Valor;

            if (objParams.TipoContrato != null)
            {
                var objaux = new TipoContrato().ObterTodos(pb, db).Where(p => p.tipo == objParams.TipoContrato).FirstOrDefault();
                if (objaux == null)
                {
                    var tp = new TipoContrato();
                    tp.tipo = objParams.TipoContrato;
                    tp.empresa_id = pb.empresa_id;
                    new TipoContrato().Incluir(tp, pb, db);
                    novaEntidade.tipoContratos_ID = tp.id;
                }
                else
                { 
                    novaEntidade.tipoContratos_ID = objaux.id;
                }
            }

            if (objParams.UnidadeNegocio != null)
            {
                var objaux = new UnidadeNegocio().ObterTodos(pb, db).Where(p => p.unidade == objParams.UnidadeNegocio).FirstOrDefault();
                if (objaux != null)
                {
                    novaEntidade.unidadeNegocio_ID = objaux.id;
                }

            }
            else
            {
                novaEntidade.unidadeNegocio_ID = new UnidadeNegocio().ObterTodos(pb, db).First().id;
            }
            if (objParams.Codigo != null)
            {
                var objaux = new Contrato().ObterTodos(pb, db).Where(p => p.contrato == objParams.Codigo).FirstOrDefault();
                if (objaux != null)
                {
                    novaEntidade.contrato_id = objaux.id;
                }
            }

            novaEntidade.tabelaPreco_ID = new TabelaPrecoItemProdutoServico().ObterTodos(pb, db).First().id;
            novaEntidade.itemProdutoServico_ID = new ItemProdutoServico().ObterTodos(pb, db).First().id;
            novaEntidade.dataInclusao = DateTime.Now;
            novaEntidade.usuarioinclusaoid = pb.usuario_id;
            novaEntidade.Inclui(novaEntidade, _paramBase, db);
            return novaEntidade.id;
            
        }

        private void Validador(DTOContratoItem objParams, DTOGenericoRetorno<DTOContratoItem> objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.Codigo))
            {
                var objException = new DTOException();
                objException.codigo = "CI01";
                objException.descricao = "Informe o código.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }
        }
    }
}