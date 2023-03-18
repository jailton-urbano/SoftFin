using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class BancoMovimentoLanctoExtrato
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Lancto Extrato")]
        public int LanctoExtrato_id { get; set; }
        [JsonIgnore,ForeignKey("LanctoExtrato_id")]
        public virtual LanctoExtrato LanctoExtrato { get; set; }

        [Display(Name = "Banco Movimento")]
        public int BancoMovimento_id { get; set; }
        [JsonIgnore,ForeignKey("BancoMovimento_id")]
        public virtual BancoMovimento BancoMovimento { get; set; }

        [Display(Name = "Relacionamento Mestre")]
        public int BancoMovimentoLanctoExtratoMestre_id { get; set; }
        [JsonIgnore,ForeignKey("BancoMovimentoLanctoExtratoMestre_id")]
        public virtual BancoMovimentoLanctoExtratoMestre BancoMovimentoLanctoExtratoMestre { get; set; }

        public bool Excluir(int id, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();

                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.BancoMovimentoLanctoExtrato.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    //erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }

        public bool Alterar(BancoMovimentoLanctoExtrato obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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


        private bool validaExistencia(DbControle db, BancoMovimentoLanctoExtrato obj)
        {
            //if (db == null)
            //    db = new DbControle();
            //var objAux = db.BancoMovimentoLanctoExtrato.Where(x => x.LanctoExtrato_id == obj.LanctoExtrato_id
            //    && x.BancoMovimento_id == obj.BancoMovimento_id).FirstOrDefault();
            //if (objAux != null)
            //    return true;
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(BancoMovimentoLanctoExtrato obj,ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<BancoMovimentoLanctoExtrato>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public bool Incluir(DbControle db, ParamBase pb)
        {

            if (validaExistencia(db, this))
                return false;
            else
            {
                new LogMudanca().Incluir(this, "", "", db, pb);

                db.Set<BancoMovimentoLanctoExtrato>().Add(this);
                db.SaveChanges();

                return true;
            }
        }

        public BancoMovimentoLanctoExtrato ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public BancoMovimentoLanctoExtrato ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.BancoMovimentoLanctoExtrato.Where(x => x.id == id).FirstOrDefault();
        }
        public List<BancoMovimentoLanctoExtrato> ObterTodos()
        {

            DbControle db = new DbControle();
            return db.BancoMovimentoLanctoExtrato.ToList();
        }
        public List<BancoMovimentoLanctoExtrato> ObterLanctoExtrato_id(int lanctoExtrato_id)
        {
            var db = new DbControle();

            return db.BancoMovimentoLanctoExtrato.Where(x => x.LanctoExtrato_id == lanctoExtrato_id).ToList();
        }
        public List<BancoMovimentoLanctoExtrato> ObterLanctoExtrato_id(int lanctoExtrato_id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.BancoMovimentoLanctoExtrato.Where(x => x.LanctoExtrato_id == lanctoExtrato_id).ToList();
        }
        public List<BancoMovimentoLanctoExtrato> ObterBancoMovimento_id(int bancoMovimento_id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.BancoMovimentoLanctoExtrato.Where(x => x.BancoMovimento_id == bancoMovimento_id).ToList();
        }
        public List<BancoMovimentoLanctoExtrato> ObterBancoMovimentoLanctoExtratoMestre_id(int bancoMovimentoLanctoExtratoMestre_id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.BancoMovimentoLanctoExtrato.Where(x => x.BancoMovimentoLanctoExtratoMestre_id == bancoMovimentoLanctoExtratoMestre_id).ToList();
        }


    }
}