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
    public class PessoaContato
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int pessoa_id { get; set; }

        [JsonIgnore,ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [StringLengthAttribute(75)]
        public string nome { get; set; }

        [StringLengthAttribute(75)]
        public string cargo { get; set; }

        [StringLengthAttribute(75)]
        public string email { get; set; }

        [StringLengthAttribute(75)]
        public string telefone { get; set; }

        [StringLengthAttribute(75)]
        public string celular { get; set; }

        [StringLengthAttribute(150)]
        public string observacao { get; set; }

        public bool RecebeCobranca { get; set; }




        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();
                
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.PessoaContato.Remove(obj);
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

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb,db);
        }

        public bool Alterar(PessoaContato obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId( obj.id, db, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                objAux.cargo = obj.cargo;
                objAux.celular = obj.celular;
                objAux.email = obj.email;
                objAux.nome = obj.nome;
                objAux.celular = obj.celular;
                objAux.observacao = obj.observacao;
                objAux.pessoa_id = obj.pessoa_id;
                objAux.telefone = obj.telefone;
                db.Entry(objAux).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, PessoaContato obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(PessoaContato obj, ParamBase pb, DbControle banco = null)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
                db = banco;

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<PessoaContato>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public PessoaContato ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public PessoaContato ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.PessoaContato.Where(x => x.id == id).FirstOrDefault();
        }

        public List<PessoaContato> ObterPorTodos(int p, ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.PessoaContato.Where(x => x.pessoa_id == p).ToList();
        }


    }
}