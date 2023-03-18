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

        
    public class ParcelaContratoController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("ParcelaContrato/Incluir")]
        [HttpPost]
        public DTOGenericoRetorno<DTOParcelaContrato> Incluir(DTOParcelaContrato objParams)
        {
            
            var objRetorno = new DTOGenericoRetorno<DTOParcelaContrato>();
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

        internal void RegraIncluir(DTOParcelaContrato objParams, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            var novaEntidade = new ParcelaContrato();
            novaEntidade.Codigo = objParams.Codigo;
            novaEntidade.descricao = objParams.Descricao;

            if (objParams.Recorrente == ENUMRecorrente.SimReverso)
                novaEntidade.valor = (objParams.Valor / objParams.Parcela);
            else
                novaEntidade.valor = objParams.Valor;

            novaEntidade.parcela = objParams.Parcela;
            novaEntidade.data = objParams.Data;



            novaEntidade.usuarioalteracaoid = pb.usuario_id;
            novaEntidade.statusParcela_ID =  StatusParcela.SituacaoPendente();

            var bancodb = new Banco().ObterPorReferenciaIntegracao(objParams.BancoReferencia, db, pb);
            if (bancodb != null)
                novaEntidade.banco_id = bancodb.id;

            
            if (objParams.ContratoItem != null)
            {
                var objaux = new ContratoItem().ObterTodos(pb, 0,db).Where(p => p.pedido == objParams.ContratoItem).FirstOrDefault();
                if (objaux != null)
                {
                    novaEntidade.contratoitem_ID = objaux.id;
                }
                else
                {
                    int contratoItem_ID;
                    
                    if (int.TryParse(objParams.ContratoItem, out contratoItem_ID))
                    {
                        novaEntidade.contratoitem_ID = contratoItem_ID;
                    }
                }
            }
            


            if (objParams.Recorrente == ENUMRecorrente.Sim)
            {
                novaEntidade.IncluiParcela(novaEntidade, "true", pb, db, objParams.DiaVencimento, objParams.Prazo);
            }
            else if (objParams.Recorrente == ENUMRecorrente.Não)
            {
                novaEntidade.IncluiParcela(novaEntidade, "false", pb, db, objParams.DiaVencimento, objParams.Prazo);
            }
            else if (objParams.Recorrente == ENUMRecorrente.SimReverso)
            {
                novaEntidade.IncluiParcela(novaEntidade, "recorrente2", pb, db, objParams.DiaVencimento, objParams.Prazo);
            }

        }

        private void Validador(DTOParcelaContrato objParams, DTOGenericoRetorno<DTOParcelaContrato> objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.Codigo))
            {
                var objException = new DTOException();
                objException.codigo = "PI01";
                objException.descricao = "Informe o código.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }




        }



    }
}