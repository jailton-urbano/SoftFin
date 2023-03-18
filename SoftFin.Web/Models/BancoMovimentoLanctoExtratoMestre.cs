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
    public class BancoMovimentoLanctoExtratoMestre
    {
        public BancoMovimentoLanctoExtratoMestre()
        {
            dataInclusao = DateTime.Now;
        }

        [Key]
        public int id { get; set; }

        [DisplayName("Data Inclusão"),
        Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime dataInclusao { get; set; }

        [Display(Name = "Banco"),
        Required(ErrorMessage = "*")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }


        [Display(Name = "Usuario"), MaxLength(500)]
        public string Usuario { get; set; }

        [DisplayName("Data Conciliação"),
        Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime dataConciliacao { get; set; }


        //[InverseProperty("BancoMovimentoLanctoExtratoMestre_id")]
        //public List<BancoMovimentoLanctoExtrato> BancoMovimentoLanctoExtrato { get; set; } 




        public bool Excluir(int id, DbControle db, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                if (db==null)
                    db = new DbControle();

                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.BancoMovimentoLanctoExtratoMestre.Remove(obj);
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

        public bool Alterar(BancoMovimentoLanctoExtratoMestre obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, BancoMovimentoLanctoExtratoMestre obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(BancoMovimentoLanctoExtratoMestre obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<BancoMovimentoLanctoExtratoMestre>().Add(obj);
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

                db.Set<BancoMovimentoLanctoExtratoMestre>().Add(this);
                db.SaveChanges();

                return true;
            }
        }

        public BancoMovimentoLanctoExtratoMestre ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public BancoMovimentoLanctoExtratoMestre ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.BancoMovimentoLanctoExtratoMestre.Where(x => x.id == id).FirstOrDefault();
        }
        public List<BancoMovimentoLanctoExtratoMestre> ObterTodos()
        {

            DbControle db = new DbControle();
            return db.BancoMovimentoLanctoExtratoMestre.ToList();
        }
        public List<BancoMovimentoLanctoExtratoMestre> ObterTodos(int banco, DateTime data)
        {

            DbControle db = new DbControle();
            return db.BancoMovimentoLanctoExtratoMestre.Where(p => p.banco_id == banco && p.dataConciliacao == data).ToList();
        }

    }
}