using SoftFin.API.DTO;

using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SoftFin.API.ContaContabil
{
    /// <summary>
    /// Lançamentos Contabeis
    /// </summary>
    public class LancamentoController : BaseApi
    {
        /// <summary>
        /// Consulta de Lançamento Contabeis
        /// </summary>
        /// <param name="objParams"> Paramentros de Entrada 
        /// {codigo_estab = "Codigo Estabelecimento, Obrigatório, numerico",
        /// data_lancamento_ini = "Data Inicial, Obrigatório, data",
        /// data_lancamento_fim = "Data Final, Obrigatório, data"
        /// }</param>
        /// <returns></returns>
        [Route("ContaContabil/Lancamento/Consultar")]
        [HttpPost]
        public DTOContaContabilLancamentoRetorno Consultar(DTOContaContabilLancamentoGetParams objParams)
        {
            var objRetorno = new DTOContaContabilLancamentoRetorno();
            try
            {
                var objKey = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.codigo_estab, _paramBase.usuario_name);

                _paramBase.estab_id = objKey.id;
                _paramBase.empresa_id = objKey.Empresa_id;

                Validator(objParams, objRetorno);

                if (objRetorno.exceptions.Count() > 0)
                {
                    objRetorno.status = "NOK";
                    return objRetorno;
                }
                var objs = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase,objParams.data_lancamento_ini, objParams.data_lancamento_fim);
                var objsRet = new APIConversor().ToDTOContaContabilLancamento(objs);

                objRetorno.status = "OK";
                objRetorno.objs = objsRet;
                return objRetorno;
            }
            catch(Exception ex)
            {
                objRetorno.exceptions.Add(new DTOException { codigo = "ACESSO", descricao = ex.Message.ToString(), id = 1, tipo = "Error" });
                objRetorno.status = "NOK";
                return objRetorno;
            }
        }

        private static void Validator(DTOContaContabilLancamentoGetParams objParams, DTOContaContabilLancamentoRetorno objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.codigo_estab))
            {
                var objException = new DTOException();
                objException.codigo = "CCL001";
                objException.descricao = "Informe o código do estabelecimento.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }

            if (objParams.data_lancamento_ini != null)
            {
                if (objParams.data_lancamento_ini.Year < 2000)
                {
                    var objException = new DTOException();
                    objException.codigo = "CCL002";
                    objException.descricao = "Data inválida de lançamento Inicial.";
                    objException.tipo = "Data Inválida";
                    objRetorno.exceptions.Add(objException);
                }
            }
            else
            {
                var objException = new DTOException();
                objException.codigo = "CCL003";
                objException.descricao = "Data inválida de lançamento Inicial.";
                objException.tipo = "Data Inválida";
                objRetorno.exceptions.Add(objException);

            }

            if (objParams.data_lancamento_fim != null)
            {
                if (objParams.data_lancamento_fim.Year < 2000)
                {
                    var objException = new DTOException();
                    objException.codigo = "CCL004";
                    objException.descricao = "Data inválida de lançamento final.";
                    objException.tipo = "Data Inválida";
                    objRetorno.exceptions.Add(objException);
                }
            }
            else
            {
                var objException = new DTOException();
                objException.codigo = "CCL005";
                objException.descricao = "Data inválida de lançamento final.";
                objException.tipo = "Data Inválida";
                objRetorno.exceptions.Add(objException);

            }
        }

        //public DTOFechamentoCaixa Post(string codEstab, DTOFechamentoCaixa fechamentocaixa)
        //{

        //}

        //public void Delete(string codEstab, string codLoja, DateTime dataFechamento, int sequencial)
        //{

        //}


    }
}
