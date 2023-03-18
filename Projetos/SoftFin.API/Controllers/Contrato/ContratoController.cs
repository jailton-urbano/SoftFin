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
    public class ContratoController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("Contrato/Incluir")]
        [HttpPost]
        public DTOGenericoRetorno<DTOContrato> Incluir(DTOContrato objParams)
        {
            var objRetorno = new DTOGenericoRetorno<DTOContrato>();
            objRetorno.status = "NOK";
            try
            {
                DbControle db = new DbControle();
                var estab = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.CodigoEstab, _usuario, db);
                _paramBase.estab_id = estab.id;
                _paramBase.empresa_id = estab.Empresa_id;
                Validador(objParams, objRetorno);

                if (new Contrato().ObterTodosPorContrato(_paramBase, objParams.Codigo, db).Count() > 0)
                {
                    var objException = new DTOException();
                    objException.codigo = "^CN003";
                    objException.descricao = "Código já existe.";
                    objException.tipo = "Lógico";
                    objRetorno.exceptions.Add(objException);
                }

                if (objRetorno.exceptions.Count() > 0)
                    throw new Exception("Verificar a Validação");

                var novaEntidade = new Contrato();
                novaEntidade.estabelecimento_id = _paramBase.estab_id;
                novaEntidade.contrato = objParams.Codigo;

                if (objParams.DataEmissao == null)
                    novaEntidade.emissao = DateTime.Now;
                else
                    novaEntidade.emissao = objParams.DataEmissao.Value;

                novaEntidade.descricao = objParams.Descricao;
                novaEntidade.valortotal = objParams.Valor;

                if (objParams.Prazo == null)
                    novaEntidade.prazo = ".";
                else
                    novaEntidade.prazo = objParams.Prazo + "-" + objParams.CondicaoPagamento;

                novaEntidade.DataInicioVigencia = objParams.InicioVigencia;
                novaEntidade.DataFinalVigencia = objParams.FimVigencia;

                if (objParams.CodigoCliente != null)
                {
                    var pessoaid = new Pessoa().ObterPorCodigo(objParams.CodigoCliente, _paramBase, db);
                    if (pessoaid != null)
                    {
                        novaEntidade.pessoas_ID = pessoaid.id;
                    }
                    else
                    {
                        var objException = new DTOException();
                        objException.codigo = "^CN004";
                        objException.descricao = "Pessoa não encontrada.";
                        objException.tipo = "Lógico";
                        objRetorno.exceptions.Add(objException);
                        throw new Exception("Pessoa não encontrada.");
                    }
                }

                if (objParams.CodigoVendedor != null)
                {
                    var pessoaid = new Pessoa().ObterPorCodigo(objParams.CodigoVendedor, _paramBase, db);
                    if (pessoaid != null)
                    {
                        novaEntidade.Vendedor_id = pessoaid.id;
                    }
                    pessoaid = new Pessoa().ObterPorNome(objParams.CodigoVendedor, _paramBase, db);
                    if (pessoaid != null)
                    {
                        novaEntidade.Vendedor_id = pessoaid.id;
                    }
                }

                if (objParams.CodigoMunicipioIBGE != null)
                {
                    var municipioid = new Municipio().ObterPorNome(objParams.CodigoMunicipioIBGE);
                    if (municipioid.Count() != 0)
                    {
                        novaEntidade.MunicipioPrestador_id = municipioid.FirstOrDefault().ID_MUNICIPIO;
                    }
                }
                novaEntidade.dataInclusao = DateTime.Now;
                novaEntidade.usuarioinclusaoid = _paramBase.usuario_id;
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    novaEntidade.Incluir(_paramBase, db);
                    foreach (var item in objParams.DTOContratoItems)
                    {
                        item.Contrato = item.Codigo;
                        item.DTOParcelaContrato.ContratoItem = new ContratoItemController().RegraIncluir(item, db,_paramBase).ToString();


                        new ParcelaContratoController().RegraIncluir(item.DTOParcelaContrato, _paramBase, db);

                        foreach (var itemUnidade in item.DTOContratoItemUnidades)
                        {
                            itemUnidade.CodigoContratoItem = item.DTOParcelaContrato.ContratoItem.ToString();
                            new ContratoItemUnidadeController().RegraIncluir(itemUnidade, _paramBase, db);
                        }

                    }

                    dbcxtransaction.Commit();
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

        private void Validador(DTOContrato objParams, DTOGenericoRetorno<DTOContrato> objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.Codigo))
            {
                var objException = new DTOException();
                objException.codigo = "CN01";
                objException.descricao = "Informe o código.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }




        }

        [Route("Contrato/GetContratoPorCodigo")]
        [HttpGet]
        public DTOGenericoRetorno<DTOContrato> Obter(string codigo, string codigoEstab)
        {
           
            var objRetorno = new DTOGenericoRetorno<DTOContrato>();
            objRetorno.status = "NOK";
            try
            {
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoValidandoUsuario(codigoEstab, _usuario).id;
                var contrato = new Contrato().ObterTodos(_paramBase).Where(p => p.contrato == codigo).FirstOrDefault();
                objRetorno.objs = new DTOContrato();
                if (contrato != null)
                {
                    objRetorno.objs.Codigo = codigo;
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