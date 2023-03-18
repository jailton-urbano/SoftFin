using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class PessoaContaContabil
    {
        [Key]
        public int pessoa_id { get; set; }
        [JsonIgnore, ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        public int? contaContabilDespesaPadrao_id { get; set; }
        [JsonIgnore, ForeignKey("contaContabilDespesaPadrao_id")]
        public virtual ContaContabil ContaContabilDespesaPadrao { get; set; }

        public int? contaContabilPagarPadrao_id { get; set; }
        [JsonIgnore, ForeignKey("contaContabilPagarPadrao_id")]
        public virtual ContaContabil ContaContabilPagarPadrao { get; set; }

        public int? contaContabilReceberPadrao_id { get; set; }
        [JsonIgnore, ForeignKey("contaContabilReceberPadrao_id")]
        public virtual ContaContabil ContaContabilReceberPadrao { get; set; }

        public PessoaContaContabil ObterPorPessoa(int idpessoa, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            return db.PessoaContaContabil.Where(x => x.pessoa_id == idpessoa).FirstOrDefault();
        }
        private bool validaExistencia(DbControle db, PessoaContaContabil obj)
        {
            return (false);
        }
        internal bool Incluir(ParamBase paramBase, DbControle db = null)
        {
            return Incluir(this, paramBase,db);
        }
        internal bool Incluir(PessoaContaContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
            db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<PessoaContaContabil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        internal bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }
        internal bool Alterar(PessoaContaContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.pessoa_id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        internal bool Excluir(ref string erro, ParamBase paramBase)
        {
            return Excluir(this.pessoa_id, ref erro, paramBase);
        }
        internal bool Excluir(int id, ref string erro, ParamBase paramBase)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.PessoaContaContabil.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
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


        public PessoaContaContabil ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public PessoaContaContabil ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.PessoaContaContabil.Where(x => x.pessoa_id == id).FirstOrDefault();
        }

        internal PessoaContaContabil ObterPorPlanoContas(int planoDeConta_id)
        {
            throw new NotImplementedException();
        }
    }
}