using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class SistemaMenu : BaseModels
    {
        [Key]
        public int id { get; set; }
        [StringLength(15)]
        public string Codigo { get; set; }

        [StringLength(75)]
        public string Descricao { get; set; }
        public bool ativo { get; set; }
        [StringLength(35)]
        public string icone { get; set; }

        public int? Ordem { get; set; }



        public List<SistemaMenu> ObterTodosAtivos()
        {
            DbControle db = new DbControle();
            return db.SistemaMenu.Where(
                    p => p.ativo == true
                    ).ToList();
        }

        public List<SistemaMenu> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.SistemaMenu.ToList();
        }

        public SistemaMenu ObterAtivoPorId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            
            return db.SistemaMenu.Where(
                    p => p.ativo == true && p.id == id
                    ).FirstOrDefault();
        }
        public SistemaMenu ObterPorId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            return db.SistemaMenu.Where(
                    p => p.id == id
                    ).FirstOrDefault();
        }


        public bool Excluir(ParamBase pb, DbControle db = null)
        {
            return Excluir(this.id, pb, db);
        }
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
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.SistemaMenu.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
                {
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
            return Alterar(this,pb, db);
        }
        public bool Alterar(SistemaMenu obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterAtivoPorId(obj.id, db);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);

                db.Entry(objAux).CurrentValues.SetValues(obj);
                db.Entry(objAux).State = EntityState.Modified;

                db.SaveChanges();

                return true;
            }
        }
        private bool ValidaExistencia(DbControle db, SistemaMenu obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(SistemaMenu obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<SistemaMenu>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


    }

}