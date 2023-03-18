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
    public class PessoaController : BaseApi
    {
        /// <summary>
        /// Incluir Loja
        /// </summary>
        /// <returns></returns>
        [Route("Pessoa/Incluir")]
        [HttpPost]
        public DTOGenericoRetorno<DTOPessoa> Incluir(DTOPessoa objParams)
        {
            var objRetorno = new DTOGenericoRetorno<DTOPessoa>();
            objRetorno.status = "NOK";
            try
            {
                var objestab = new Estabelecimento().ObterPorCodigoValidandoUsuario(objParams.CodigoEstab, _usuario);
                _paramBase.estab_id = objestab.id;
                _paramBase.empresa_id = objestab.Empresa_id;
                Validador(objParams, objRetorno);

                if (new Pessoa().ObterPorCodigo(objParams.Codigo,_paramBase,null) != null)
                {
                    var objException = new DTOException();
                    objException.codigo = "^PS003";
                    objException.descricao = "Código já existe.";
                    objException.tipo = "Lógico";
                    objRetorno.exceptions.Add(objException);
                }

                if (objRetorno.exceptions.Count() > 0)
                    throw new Exception("Verificar a Validação");

                var novaEntidade = new Pessoa();
                novaEntidade.empresa_id = _paramBase.empresa_id;
                novaEntidade.endereco = objParams.Logradouro;
                novaEntidade.nome = objParams.Nome;
                novaEntidade.razao = objParams.Razao;
                novaEntidade.codigo = objParams.Codigo;
                novaEntidade.TelefoneFixo = objParams.TelefoneFixo;
                novaEntidade.Celular = objParams.Celular;
                novaEntidade.eMail = objParams.Email;
                novaEntidade.cep = objParams.Cep;
                novaEntidade.numero = objParams.Numero;
                novaEntidade.complemento = objParams.Complemento;
                novaEntidade.bairro = objParams.Bairro;
                novaEntidade.uf = objParams.UF;
                novaEntidade.cidade = objParams.Cidade;


                if (objParams.CNPJ != null)
                {
                    novaEntidade.CategoriaPessoa_ID = 2;
                    novaEntidade.cnpj = objParams.CNPJ;
                }
                else
                if (objParams.CPF != null)
                {
                    novaEntidade.CategoriaPessoa_ID = 1;
                    novaEntidade.cnpj = objParams.CPF;
                }


                if (objParams.TipoPessoa != null)
                {
                    var cp = new TipoPessoa().ObterTodos(_paramBase).Where(p => p.Descricao.Equals(objParams.TipoPessoa)).FirstOrDefault();
                    if (cp != null)
                    {
                        novaEntidade.TipoPessoa_ID = cp.id;
                    }
                }

                if (objParams.UnidadeNegocio != null)
                {
                    var cp = new UnidadeNegocio().ObterTodos(_paramBase).Where(p => p.unidade.Equals(objParams.UnidadeNegocio)).FirstOrDefault();
                    if (cp != null)
                    {
                        novaEntidade.UnidadeNegocio_ID = cp.id;
                    }
                }

                if (objParams.TipoEndereco != null)
                {
                    var cp = new TipoEndereco().ObterTodos(_paramBase).Where(p => p.Descricao.Equals(objParams.TipoEndereco)).FirstOrDefault();
                    if (cp != null)
                    {
                        novaEntidade.TipoEndereco_ID = cp.id;
                    }
                }

                novaEntidade.Incluir(_paramBase);

                if (objParams.DTOContatos != null)
                {
                    foreach (var item in objParams.DTOContatos)
                    {
                        var pessoaContato = new PessoaContato();
                        pessoaContato.nome = item.Nome;
                        pessoaContato.celular = item.Celular;
                        pessoaContato.email = item.Email;
                        pessoaContato.observacao = item.Observacao;
                        pessoaContato.RecebeCobranca = item.RecebeCobranca;
                        pessoaContato.telefone = item.Telefone;
                        pessoaContato.pessoa_id = novaEntidade.id;

                        pessoaContato.Incluir(_paramBase, null);
                    }
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

        private void Validador(DTOPessoa objParams, DTOGenericoRetorno<DTOPessoa> objRetorno)
        {
            if (string.IsNullOrWhiteSpace(objParams.Codigo))
            {
                var objException = new DTOException();
                objException.codigo = "PS01";
                objException.descricao = "Informe o código.";
                objException.tipo = "Parametro Obrigatorio";
                objRetorno.exceptions.Add(objException);
            }




        }



    }
}