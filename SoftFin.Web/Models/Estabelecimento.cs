using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{


    public class Estabelecimento : BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Nome Fantasia"), Required(ErrorMessage = "Campo Nome Fantasia obrigatório, tamanho máximo 50"), StringLength(150)]
        public string Apelido { get; set; }
        [Display(Name = "Código"), Required(ErrorMessage = "Campo Código obrigatório, tamanho máximo 30"), StringLength(30)]
        public string Codigo { get; set; }



        [Display(Name = "Nome"), Required(ErrorMessage = "Campo Nome obrigatório, tamanho máximo 30"), StringLength(50)]
        public string NomeCompleto { get; set; }

        public object ObterPorCodigoValidandoUsuario(object codigoEstab, string usuario)
        {
            throw new NotImplementedException();
        }

        [Display(Name = "Logradouro"), Required(ErrorMessage = "Campo Logradouro obrigatório, tamanho máximo 50"), StringLength(150)]
        public string Logradouro { get; set; }
        [Display(Name = "Número"), Required(ErrorMessage = "Campo Número obrigatório, somente número")]
        public int? NumeroLogradouro { get; set; }
        
        [Display(Name = "Complemento"), StringLength(50)]
        public string Complemento { get; set; }
        
        [Display(Name = "Bairro"), Required(ErrorMessage = "Campo Bairro obrigatório, tamanho máximo 50"), StringLength(50)]
        public string BAIRRO { get; set; }

        [Display(Name = "Municipio"), Required(ErrorMessage = "Campo Municipio obrigatório, somente números")]
        public int Municipio_id { get; set; }

        [NotMapped]
        public string MunicipioDesc { get; set; }

        [Display(Name = "UF"), Required(ErrorMessage = "Campo UF obrigatório, tamanho máximo 2"), StringLength(2)]
        public string UF { get; set; }
        [Display(Name = "CEP"), Required(ErrorMessage = "Campo CEP obrigatório, tamanho máximo 9"), StringLength(9)]
        public string CEP { get; set; }
        [Display(Name = "Empresa"), Required(ErrorMessage = "Campo Empresa obrigatório")]
        public int Empresa_id { get; set; }
        [Display(Name = "CNPJ"), Required(ErrorMessage = "Campo CNPJ obrigatório"), StringLength(20)]
        public string CNPJ { get; set; }
        [Display(Name = "Incrição Estadual"), Required(ErrorMessage = "Campo Incrição Estadual obrigatório, tamanho máximo 15"), StringLength(15)]
        public string InscricaoEstadual { get; set; }
        [Display(Name = "Inscrição Municipal"), Required(ErrorMessage = "Campo Inscrição Municipal obrigatório, tamanho máximo 25")]
        public int InscricaoMunicipal { get; set; }
        [JsonIgnore,ForeignKey("Empresa_id")]
        public virtual Empresa Empresa { get; set; }
        [JsonIgnore,ForeignKey("Municipio_id")]
        public virtual Municipio Municipio { get; set; }

        [Display(Name = "Logo"), StringLength(120)]
        public string Logo { get; set; }

        [Display(Name = "Senha Certificado"), StringLength(150)]
        public string senhaCertificado { get; set; }

        [Display(Name = "Ativo")]
        public bool? ativo { get; set; }


        [Display(Name = "Opção Tributária Simples")]
        public int? opcaoTributariaSimples_id { get; set; }
        [JsonIgnore,ForeignKey("opcaoTributariaSimples_id")]
        public virtual OpcaoTributariaSimples opcaoTributariaSimples { get; set; }


        [StringLength(150)]
        public string CodigoTributacaoMunicipio { get; set; }


        public bool? autorizacaoCPAG { get; set; }

        public bool? autorizacaoFaturamento { get; set; }


        [Display(Name = "Email notificações"), StringLength(350)]
        public string emailNotificacoes { get; set; }

        [StringLength(30)]
        public string codigoEstabelecimentoContabil { get; set; }

        public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(this.id, ref erro, pb);
        }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Estabelecimento.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb, db);
        }

        public bool Alterar(Estabelecimento obj, ParamBase pb,DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, Estabelecimento obj, ParamBase pb, int idempresa = 0)
        {
            if (obj.ObterTodos(pb, idempresa).Where(p => p.NomeCompleto.Equals(obj.NomeCompleto)).Count() > 1)
                return true;
            if (obj.ObterTodos(pb, idempresa).Where(p => p.Apelido.Equals(obj.Apelido)).Count() > 1)
                return true;
            if (obj.ObterTodos(pb, idempresa).Where(p => p.Codigo.Equals(obj.Codigo)).Count() > 1)
                return true;

            return (false);
        }

        public bool Incluir(ParamBase pb, DbControle banco = null,int idempresa = 0)
        {
            return Incluir(this, pb, banco,idempresa );
        }
        public bool Incluir(Estabelecimento obj, ParamBase pb, DbControle banco = null,int idempresa = 0)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
                db = banco;

            if (validaExistencia(db, obj, pb, idempresa))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Estabelecimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Estabelecimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Estabelecimento ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.Estabelecimento.Where(x => x.id == id ).FirstOrDefault();
        }
        public Estabelecimento ObterPorIdMail(int id, DbControle db=null)
        {
            if (db == null)
                db = new DbControle();

            return db.Estabelecimento.Where(x => x.id == id).FirstOrDefault();
        }
        public List<Estabelecimento> ObterTodos(ParamBase pb, int idempresa = 0)
        {
            if (idempresa == 0)
                idempresa  = pb.empresa_id;

            DbControle db = new DbControle();
            return db.Estabelecimento.Where(x => x.Empresa_id == idempresa).ToList();
        }





        public List<Estabelecimento> ObterPorEmpresaId(int id)
        {
            DbControle db = new DbControle();
            return db.Estabelecimento.Where(x => x.Empresa_id == id).ToList();
        }

        public List<Estabelecimento> ObterTodosComCertificado(DbControle db = null)
        {
            return db.Estabelecimento.Where(x => x.senhaCertificado == null || x.senhaCertificado == "").ToList();
        }

        public Estabelecimento ObterPorCodigoValidandoUsuario(string codigoEstab, string codigoUsuario, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var usu = new Usuario().ObterPorCodigo(codigoUsuario);
            var rel = new UsuarioEstabelecimento().ObterTodosPorIdUsuario(usu.id);
            var estab = db.Estabelecimento.Where(x => x.Codigo == codigoEstab && x.Empresa_id == usu.empresa_id).FirstOrDefault();
            if (estab == null)
                throw new Exception("Estabelecimento não encontrado");

            var y = rel.Where(p => p.estabelecimento_id == estab.id);
            if (y.Count() == 0)
                throw new Exception("Estabelecimento não encontrado ou não configurado");

            return estab;
        }

        public Estabelecimento ObterPorCodigoPrimeiroEstab(string usuario_name)
        {
            DbControle db = new DbControle();
            var usu = new Usuario().ObterPorCodigo(usuario_name);
            var rel = new UsuarioEstabelecimento().ObterTodosPorIdUsuario(usu.id).FirstOrDefault();
            if (rel == null)
                throw new Exception("Estabelecimento não encontrado ou não configurado");

            return rel.Estabelecimento;
        }

        public string Fone { get; set; }

        public string CRT { get; set; }
        public string MigrateCode { get; set; }
        public string CNAE { get; set; }
    }
}