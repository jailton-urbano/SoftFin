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
    public class LojaController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("store/insert")]
        [HttpPost]
        public DTOGenericoRetorno<DTOStoreParams> Incluir(DTOStoreParams objParams)
        {
            var objRetorno = new DTOGenericoRetorno<DTOStoreParams>();
            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.code_estab, _paramBase.usuario_name).id;
                Validador(objParams, objRetorno);

                if (new Loja().ObterTodos(_paramBase).Where(p => p.codigo == objParams.code).Count() > 0)
                {
                    var objException = new DTOException();
                    objException.codigo = "^LC003";
                    objException.descricao = "Código já existe.";
                    objException.tipo = "Lógico";
                    objRetorno.exceptions.Add(objException);
                }

                if (objRetorno.exceptions.Count() > 0)
                    throw new Exception("Verificar a Validação");

                var novaEntidade = new Loja();
                novaEntidade.codigo = objParams.code;
                novaEntidade.descricao = objParams.description;
                novaEntidade.ativo = objParams.active;
                novaEntidade.estabelecimento_id = _paramBase.estab_id;
                novaEntidade.Incluir(_paramBase);

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

        private void Validador(DTOStoreParams objParams, DTOGenericoRetorno<DTOStoreParams> objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.code))
            {
                var objException = new DTOException();
                objException.codigo = "LC001";
                objException.descricao = "Informe o código.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }

            if (string.IsNullOrWhiteSpace(objParams.description))
            {
                var objException = new DTOException();
                objException.codigo = "^LC002";
                objException.descricao = "Informe o código da loja .";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }




        }


        /// <summary>
        /// Alteração da loja
        /// </summary>
        /// <param name="objParams">Paremetros de entrada da loja</param>
        /// <returns>Objeto de retorno status = "OK", ou status "NOK" não funcional (neste caso é retornado uma lista com os erros) </returns>
        [Route("store/edit")]
        [HttpPut]
        public DTOGenericoRetorno<DTOStoreParams> Alterar(DTOStoreParams objParams)
        {
            var objRetorno = new DTOGenericoRetorno<DTOStoreParams>();
            var novaEntidade = new Loja();
            var db = new DbControle();
            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.code_estab, _paramBase.usuario_name).id;
                Validador(objParams, objRetorno);

                novaEntidade = new Loja().ObterTodos(_paramBase, db).Where(p => p.codigo == objParams.code).FirstOrDefault();

                if (novaEntidade == null)
                {
                    var objException = new DTOException();
                    objException.codigo = "^LC005";
                    objException.descricao = "Código não encontrado.";
                    objException.tipo = "Lógico";
                    objRetorno.exceptions.Add(objException);
                }

                if (objRetorno.exceptions.Count() > 0)
                    throw new Exception("Verificar a Validação");

                novaEntidade.codigo = objParams.code;
                novaEntidade.descricao = objParams.description;
                novaEntidade.ativo = objParams.active;
                novaEntidade.estabelecimento_id = _paramBase.estab_id;
                novaEntidade.Alterar(_paramBase,db);

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



        /// <summary>
        /// Exclusão
        /// </summary>
        /// <param name="code_estab">Cógio da Estabelecimento</param>
        /// <param name="code_store">Código da Loja</param>
        /// <returns>Objeto de retorno status = "OK", ou status "NOK" não funcional (neste caso é retornado uma lista com os erros) </returns>
        [Route("store/delete")]
        [HttpDelete]
        public DTOGenericoRetorno<DTOStoreParams> Excluir(string code_estab, string code_store)
        {
            var objRetorno = new DTOGenericoRetorno<DTOStoreParams>();
            var db = new DbControle();

            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(code_estab, _paramBase.usuario_name).id;

                var novaEntidade = new Loja().ObterTodos(_paramBase, db).Where(p => p.codigo == code_store).FirstOrDefault();

                if (novaEntidade != null)
                {
                    var erro = "";
                    novaEntidade.Excluir(ref erro, _paramBase);
                    if (erro != "")
                    {
                        throw new Exception(erro);
                    }
                }
                else
                {
                    throw new Exception("Registro não encontrado");
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

        /// <summary>
        /// Lista
        /// </summary>
        /// <param name="code_estab">Cógio da Estabelecimento</param>
        /// <param name="code_store">Código da Loja</param>
        /// <returns>Objeto de retorno status = "OK" junto com lista(objs), ou status "NOK" não funcional (neste caso é retornado uma lista com os erros) </returns>
        [Route("store/list")]
        [HttpGet]
        public DTOGenericoRetorno<List<DTOStoreParams>> Consulta(string code_estab, string code_store = null)
        {
            var objRetorno = new DTOGenericoRetorno<List<DTOStoreParams>>();
            var db = new DbControle();

            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(code_estab, _paramBase.usuario_name).id;


                var objs = new Loja().ObterTodos(_paramBase, db);
                objRetorno.objs = new List<DTOStoreParams>();
                foreach (var item in objs)
                {
                    objRetorno.objs.Add(new DTOStoreParams { active = item.ativo, code = item.codigo, code_estab = item.Estabelecimento.Codigo, description =item.descricao} );
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