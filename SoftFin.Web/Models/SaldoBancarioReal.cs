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
    public class SaldoBancarioReal: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Banco"), Required(ErrorMessage = "*")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }
        
        [Display(Name = "Data do Saldo"), Required(ErrorMessage = "*")]
        public DateTime dataSaldo { get; set; }  

        [Display(Name = "Saldo Final"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal saldoFinal { get; set; }

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
                    db.SaldoBancarioReal.Remove(obj);
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

        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb); 
        }
        public bool Alterar(SaldoBancarioReal obj, ParamBase pb)
        {
            DbControle db = new DbControle();

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


        private bool validaExistencia(DbControle db, SaldoBancarioReal obj)
        {
            var objaux = db.SaldoBancarioReal.Where(p => p.dataSaldo == obj.dataSaldo && p.banco_id == obj.banco_id).Count();
            return ((objaux != 0));
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(SaldoBancarioReal obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<SaldoBancarioReal>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public SaldoBancarioReal ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public SaldoBancarioReal ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.SaldoBancarioReal.Where(x => x.id == id && x.Banco.estabelecimento_id == idempresa).FirstOrDefault();
        }

        public SaldoBancarioReal ObterBancoData(int idBanco, DateTime data, ParamBase pb)
        {
            int idempresa = pb.estab_id;
            DbControle db = new DbControle();
            return db.SaldoBancarioReal.Where(x => x.Banco.estabelecimento_id == idempresa && x.banco_id == idBanco && x.dataSaldo == data).FirstOrDefault();
        }

        public List<SaldoBancarioReal> ObterData( DateTime data, ParamBase pb)
        {
            int idempresa = pb.estab_id;
            DbControle db = new DbControle();
            return db.SaldoBancarioReal.Where(x => x.Banco.estabelecimento_id == idempresa && x.dataSaldo == data).ToList();
        }


        public List<SaldoBancarioReal> ObterTodos(ParamBase pb)
        {
            int idempresa = pb.estab_id;
            DbControle db = new DbControle();
            return db.SaldoBancarioReal.Where(x => x.Banco.estabelecimento_id == idempresa).ToList();
        }

        public SaldoBancarioReal ObterUltimoSaldo(int idbanco, ParamBase pb)
        {
            int idempresa = pb.estab_id;
            DbControle db = new DbControle();
            var result = db.SaldoBancarioReal.Where(x => x.banco_id == idbanco);
            if (result.Count() > 0 )
                return result.OrderByDescending(x => x.dataSaldo).First();
            else
                return null;
        }
    }
}