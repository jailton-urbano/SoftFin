using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOPessoa
    {
        public string CodigoEstab { get; set; }
        public string Nome { get; set; }
        public string Razao { get; set; }
        public string TipoPessoa { get; set; }
        public string UnidadeNegocio { get; set; }
        public string Codigo { get; set; }
        public string CategoriaPessoa { get;  set; }
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string TelefoneFixo { get;  set; }
        public string Celular { get;  set; }

        public string Email { get;  set; }
        public object TipoEndereco { get;  set; }
        public string Cep { get;  set; }
        public string Numero { get;  set; }
        public string Bairro { get;  set; }
        public string UF { get;  set; }
        public string Cidade { get;  set; }
        public string Complemento { get;  set; }
        public string Logradouro { get; set; }
        public List<DTOContato> DTOContatos { get; set; }
    }

    public class DTOContato
    {
        public string Nome { get; set; }

        public string Cargo { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public string Celular { get; set; }

        public string Observacao { get; set; }

        public bool RecebeCobranca { get; set; }
    }
}