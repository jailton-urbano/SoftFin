using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
//using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class Pessoa:BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Código"),
        MaxLength(20),
        Required(ErrorMessage = "Campo Código obrigatório")]
        public string codigo { get; set; }
        [Display(Name = "Nome Pessoa"),
        MaxLength(150),
        Required(ErrorMessage = "Campo Nome obrigatório")]
        public string nome { get; set; }
        [Display(Name = "Razão Social"),
        MaxLength(150)]
        public string razao { get; set; }
        [Display(Name = "CNPJ/CPF"),
        MaxLength(50)]
        public string cnpj { get; set; }
        [Display(Name = "Incrição Estadual"),
        MaxLength(20)]
        public string inscricao { get; set; }
        [Display(Name = "CCM"),
        MaxLength(8)]
        public string ccm { get; set; }
        [Display(Name = "Endereço"),
        MaxLength(50)]
        public string endereco { get; set; }
        [Display(Name = "Número"),
        MaxLength(20)]
        public string numero { get; set; }
        [Display(Name = "Complemento"),
        MaxLength(50)]
        public string complemento { get; set; }
        [Display(Name = "Bairro"),
        MaxLength(50)]
        public string bairro { get; set; }
        [Display(Name = "Cidade"),
        MaxLength(30)]
        public string cidade { get; set; }
        [Display(Name = "UF"),
        MaxLength(2)]
        public string uf { get; set; }
        [Display(Name = "CEP"),
        MaxLength(9)]
        public string cep { get; set; }
        [Display(Name = "E-Mail"),
        MaxLength(70),
        EmailAddress]
        public string eMail { get; set; }

        [Display(Name = "Código Banco"), MaxLength(3)]
        public string bancoConta { get; set; }
        [Display(Name = "Agência"), MaxLength(10)]
        public string agenciaConta { get; set; }
        [Display(Name = "Digito"), MaxLength(1)]
        public string agenciaDigito { get; set; }
        [Display(Name = "Conta"), MaxLength(12)]
        public string contaBancaria { get; set; }
        [Display(Name = "Dígito Conta"), MaxLength(1)]
        public string digitoContaBancaria { get; set; }

        [Display(Name = "Unidade de  Negocio")]
        public int? UnidadeNegocio_ID { get; set; }
        [Display(Name = "Tipo Endereço")]
        public int? TipoEndereco_ID { get; set; }
        [Display(Name = "Categoria Pessoa")]
        public int? CategoriaPessoa_ID { get; set; }
        [Display(Name = "Tipo Pessoa")]
        public int? TipoPessoa_ID { get; set; }
        [JsonIgnore,ForeignKey("UnidadeNegocio_ID")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }
        [JsonIgnore,ForeignKey("TipoEndereco_ID")]
        public virtual TipoEndereco TipoEndereco { get; set; }
        [JsonIgnore,ForeignKey("CategoriaPessoa_ID")]
        public virtual CategoriaPessoa CategoriaPessoa { get; set; }
        [JsonIgnore,ForeignKey("TipoPessoa_ID")]
        public virtual TipoPessoa TipoPessoa { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Pessoa> Pessoas { get; set; }

        [Display(Name = "Telefone Fixo"),StringLength(20)]
        public string TelefoneFixo { get; set; }

        [Display(Name = "Celular"), StringLength(20)]
        public string Celular { get; set; }

        [Display(Name = "Foto"), StringLength(120)]
        public string Foto { get; set; }

        [Display(Name = "Segurado")]
        public Boolean? flgSegurado { get; set; }

        [Display(Name = "Perfil Pessoal"), StringLength(700)]
        public string perfilPessoal { get; set; }

        [Display(Name = "Perfil Profissional"), StringLength(700)]
        public string perfilProfissional { get; set; }

        [Display(Name = "Perfil Familiar"), StringLength(700)]
        public string perfilFamiliar { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<PessoaContato> contatos { get; set; }

        [Display(Name = "Aliquota ICMS")]
        public decimal? aliquotaICMS { get; set; }

        [Display(Name = "Suframa")]
        public string suframa { get; set; }


        [Display(Name = "CFOP")]
        public string CFOP { get; set; }

        [Display(Name = "Identificador de Estrangeiro")]
        public string identificadorCompradorEstrangeiro { get; set; }

        [Display(Name = "CFOP / CSOSN")]
        public string CST_CSOSN { get; set; }

        [Display(Name = "CNAE")]
        public string CNAE { get; set; }

        [Display(Name = "Observação Fatura")]
        public string observacaoFatura { get; set; }

        public bool Excluir(int id, ParamBase pb, ref string erro)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = ObterPorId(id, pb, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Pessoa.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Impossível excluir, Pessoa está relacionada com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool Alterar( ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }

        public bool Alterar(Pessoa obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, Pessoa obj, ParamBase paramBase)
        {
            DbControle banco = new DbControle();

            int idempresa = paramBase.empresa_id;


            var objAux = banco.Pessoa.Where(x => x.empresa_id == idempresa && x.codigo == obj.codigo).FirstOrDefault();
            if (objAux != null)
                return true;

            objAux = banco.Pessoa.Where(x => x.empresa_id == idempresa && x.nome == obj.nome).FirstOrDefault();
            if (objAux != null)
                return true;
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(Pessoa obj, ParamBase pb, DbControle banco = null)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
                db = banco;

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                //new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Pessoa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }



        public Pessoa ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, paramBase, null);
        }
        public Pessoa ObterPorId(int id, ParamBase paramBase, DbControle db)
        {
            int idempresa = paramBase.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.Pessoa.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }
        public List<Pessoa> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == idempresa).OrderBy(p => p.nome).ToList();
        }

        public List<Pessoa> ObterFuncionarios(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == idempresa && (
                x.TipoPessoa.Descricao == "Colaborador" || x.TipoPessoa.Descricao == "Funcionário")
                ).OrderBy(p => p.nome).ToList();
        }

        public List<Pessoa> ObterFornecedores(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == idempresa && (x.TipoPessoa.Descricao == "Fornecedor")
                ).OrderBy(p => p.nome).ToList();
        }

        public List<Pessoa> ObterCliente(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == idempresa && (x.TipoPessoa.Descricao == "Cliente")
                ).OrderBy(p => p.nome).ToList();
        }

        public SelectList CarregaPessoa(ParamBase paramBase)
        {
            var con1 = ObterTodos(paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2}", item.codigo, item.nome, item.TipoPessoa.Descricao) });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }
        public SelectList ObterClienteComCNPJ(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            var con1 = db.Pessoa.Where(x => x.empresa_id == idempresa && (x.TipoPessoa.Descricao == "Cliente")
                ).ToList();

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}, {1} ", item.nome, item.cnpj)});
            }
            return new SelectList(items, "Value", "Text");
        }

        public SelectList ObterTodosComCNPJ(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            var con1 = db.Pessoa.Where(x => x.empresa_id == idempresa).ToList();

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}, {1} ", item.nome, item.cnpj) });
            }
            return new SelectList(items, "Value", "Text");
        }

        public SelectList ObterTodosRacaoComCNPJ(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            var con1 = db.Pessoa.Where(x => x.empresa_id == idempresa).ToList();

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}, {1} ", item.razao, item.cnpj) });
            }
            return new SelectList(items, "Value", "Text");
        }
        public SelectList ObterFornecedorComCNPJ(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            var con1 = db.Pessoa.Where(x => x.empresa_id == idempresa && (x.TipoPessoa.Descricao == "Fornecedor")
                ).ToList();

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}, {1} ", item.nome, item.cnpj) });
            }
            return new SelectList(items, "Value", "Text");
        }


        public Pessoa ObterPorNomeCNPJ(string nomePessoa, string cnpjPessoa, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            if (cnpjPessoa.Trim() != "")
            {
                return db.Pessoa.Where(x => x.empresa_id == idempresa && x.cnpj == cnpjPessoa.Trim() && x.nome == nomePessoa.Trim()).FirstOrDefault();
            }
            else
            {
                return db.Pessoa.Where(x => x.empresa_id == idempresa && x.nome == nomePessoa.Trim()).FirstOrDefault();
            }
        }


        public Pessoa ObterPorRazaoCNPJ(string razao, string cnpjPessoa, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            if (cnpjPessoa.Trim() != "")
            {
                return db.Pessoa.Where(x => x.empresa_id == idempresa && x.cnpj == cnpjPessoa.Trim() && x.nome == razao.Trim()).FirstOrDefault();
            }
            else
            {
                return db.Pessoa.Where(x => x.empresa_id == idempresa && x.razao == razao.Trim()).FirstOrDefault();
            }
        }

        public List<Pessoa> ObterSeguradoras(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id; 
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == idempresa && (x.TipoPessoa.Descricao.Contains("Seguradora"))
                ).OrderBy(p => p.nome).ToList();
        }

        public Pessoa ObterPorCodigo(string codigo, ParamBase paramBase, DbControle db)
        {
            int idempresa = paramBase.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.Pessoa.Where(x => x.empresa_id == idempresa && (x.codigo == codigo)
                ).FirstOrDefault();
        }
        public Pessoa ObterPorNome(string nome, ParamBase paramBase, DbControle db)
        {
            int idempresa = paramBase.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.Pessoa.Where(x => x.empresa_id == idempresa && (x.codigo == nome)
                ).FirstOrDefault();
        }
        public Pessoa ObterPorCNPJ(string cnpj, ParamBase paramBase, DbControle db = null)
        {
            int idempresa = paramBase.empresa_id;
            if (db == null)
                db = new DbControle();

            cnpj = cnpj.Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "");
            return db.Pessoa.Where(x => x.empresa_id == idempresa && (x.cnpj.Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "") == cnpj)
                ).FirstOrDefault();
        }

        public int Quantidade(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == idempresa).Count();
        }

        public List<Pessoa> ObterTodosPorTipoPessoa(int idTipoPessoa, ParamBase paramBase)
        {
            int idempresa = empresa_id;
            DbControle db = new DbControle();
            return db.Pessoa.Where(x => x.empresa_id == paramBase.empresa_id && (x.TipoPessoa_ID == idTipoPessoa)
                ).OrderBy(p => p.nome).ToList();
        }

        public string ObterUltimoCodigoDisponivel(ParamBase pb )
        {

            DbControle db = new DbControle();
            var pes =  db.Pessoa.Where(x => x.empresa_id == pb.empresa_id);
            List<int> numbers = new List<int>();

            foreach (var item in pes)
            {
                int outInt;
                if (int.TryParse(item.codigo, out outInt))
                    numbers.Add(outInt);
            }
            if (numbers.Count() == 0)
            {
                return "1";
            }
            else
            {
                return (numbers.Max(p => p) + 1).ToString();
            }

        }
    }
}

