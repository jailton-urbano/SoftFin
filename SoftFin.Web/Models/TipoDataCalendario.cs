using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SoftFin.Web.Models
{
    public class TipoDataCalendario
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Tipo"),
        Required(ErrorMessage = "*"),
        StringLength(20)]
        public string tipo { get; set; }

        [Display(Name = "Horas Úteis"),
        Required(ErrorMessage = "*")]
        public decimal horasUteis { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.TipoDataCalendario.Remove(obj);
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
        public bool Alterar(TipoDataCalendario obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, TipoDataCalendario obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TipoDataCalendario obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<TipoDataCalendario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public TipoDataCalendario ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public TipoDataCalendario ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.TipoDataCalendario.Where(x => x.id == id).FirstOrDefault();
        }
        public List<TipoDataCalendario> ObterTodos()
        {

            DbControle db = new DbControle();
            return db.TipoDataCalendario.ToList();
        }


    }
}