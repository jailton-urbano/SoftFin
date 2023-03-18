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
    public class SistemaDashBoard: BaseModels
    {
        [Key]
        public int id { get; set; }

        [StringLength(15)]
        public string Codigo { get; set; }
        [StringLength(75)]
        public string Descricao { get; set; }
        public bool ativo { get; set; }

        public int sistemaMenu_id { get; set; }

        [JsonIgnore,ForeignKey("sistemaMenu_id")]
        public virtual SistemaMenu SistemaMenu { get; set; }

        [StringLength(35)]
        public string icone { get; set; }

        public int? Ordem { get; set; }
        
        public SistemaDashBoard ObterAtivoPorId(int id, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();
            return db.SistemaDashBoard.Where(
                    p => p.ativo == true && p.id == id
                    ).FirstOrDefault();
        }
        public SistemaDashBoard ObterPorId(int id, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();
            return db.SistemaDashBoard.Where(
                    p => p.id == id
                    ).FirstOrDefault();
        }

        public List<SistemaDashBoard> ObterTodosPorIdSistemaMenu(int id)
        {
            DbControle db = new DbControle();
            return db.SistemaDashBoard.Where(
                    p => p.ativo == true && p.sistemaMenu_id == id
                    ).ToList();
        }

        public List<SistemaDashBoard> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.SistemaDashBoard.ToList();
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
                    db.SistemaDashBoard.Remove(obj);
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
            return Alterar(this, pb, db);
        }
        public bool Alterar(SistemaDashBoard obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, db);
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
        private bool ValidaExistencia(DbControle db, SistemaDashBoard obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(SistemaDashBoard obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<SistemaDashBoard>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
    }
}