using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class AtividadeUsuario
    {
        public AtividadeUsuario()
        {
            DataInclusao = DateTime.Now;
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "Atividade")]
        public int atividade_id { get; set; }

        [JsonIgnore,ForeignKey("atividade_id")]
        public virtual Atividade Atividade { get; set; }

        [Display(Name = "Usuario")]
        public int usuario_id { get; set; }

        [JsonIgnore,ForeignKey("usuario_id")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Data Inclusao")]
        public DateTime DataInclusao { get; set; }


        private bool validaExistencia(DbControle db, AtividadeUsuario obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(AtividadeUsuario obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<AtividadeUsuario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public bool Excluir(int id, ParamBase pb, DbControle db= null)
        {
            try
            {
                int estab = pb.estab_id;
                
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.AtividadeUsuario.Remove(obj);
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



        public AtividadeUsuario ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,  pb);
        }
        public AtividadeUsuario ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.AtividadeUsuario.Where(x => x.id == id && x.Atividade.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<AtividadeUsuario> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.AtividadeUsuario.Where(x => x.Atividade.estabelecimento_id == estab).ToList();
        }



        public List<AtividadeUsuario> ObterTodosPorAtividade(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.AtividadeUsuario.Where(x => x.atividade_id == id && x.Atividade.estabelecimento_id == estab).ToList();
        }
        
    }
}