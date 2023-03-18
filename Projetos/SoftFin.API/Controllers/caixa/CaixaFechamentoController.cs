using SoftFin.API.DTO;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SoftFin.API.users
{
    public class CaixaFechamentoController : BaseApi
    {
        [Route("Caixa/Fechamento/Consultar")]
        [HttpPost]
        public DTOFechamentoCaixaRetorno Consultar(DTOFechamentoCaixaGetParams objParams)
        {
            var objRetorno = new DTOFechamentoCaixaRetorno();
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.codigo_estab, _paramBase.usuario_name).id;

                Validator(objParams, objRetorno);

                if (objRetorno.exceptions.Count() > 0)
                {
                    objRetorno.status = "NOK";
                    return objRetorno;
                }
                var objs = new LojaFechamento().ObterTodos(_paramBase);
                var objsRet = new APIConversor().ToDTOFechamentoCaixa(objs);

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

        private static void Validator(DTOFechamentoCaixaGetParams objParams, DTOFechamentoCaixaRetorno objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.codigo_estab))
            {
                var objException = new DTOException();
                objException.codigo = "FC001";
                objException.descricao = "Informe o código do fechamento do caixa.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }

            if (string.IsNullOrWhiteSpace(objParams.codigo_loja))
            {
                var objException = new DTOException();
                objException.codigo = "FC002";
                objException.descricao = "Informe o código da loja .";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }

            if (objParams.data_fechamento_ini != null)
            {
                if (objParams.data_fechamento_ini.Value.Year < 2000)
                {
                    var objException = new DTOException();
                    objException.codigo = "FC003";
                    objException.descricao = "Data inválida de Fechamento Inicial.";
                    objException.tipo = "Data Inválida";
                    objRetorno.exceptions.Add(objException);
                }
            }


            if (objParams.data_fechamento_fim != null)
            {
                if (objParams.data_fechamento_fim.Value.Year < 2000)
                {
                    var objException = new DTOException();
                    objException.codigo = "FC004";
                    objException.descricao = "Data inválida de Fechamento Final.";
                    objException.tipo = "Data Inválida";
                    objRetorno.exceptions.Add(objException);
                }
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
