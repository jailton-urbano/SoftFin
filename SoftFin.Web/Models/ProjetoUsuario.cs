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
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class ProjetoUsuario
    {
        public int id { get; set; }

        [Display(Name = "Projeto")]
        public int projeto_id { get; set; }

        [JsonIgnore,ForeignKey("projeto_id")]
        public virtual Projeto Projeto { get; set; }

        [Display(Name = "Categoria Profissional"), Required(ErrorMessage = "*")]
        public int categoria_id { get; set; }

        [JsonIgnore,ForeignKey("categoria_id")]
        public virtual CategoriaProfissional categoriaProfissional { get; set; }
        
        [Display(Name = "Usuario")]
        public int usuario_id { get; set; }

        [JsonIgnore,ForeignKey("usuario_id")]
        public virtual Usuario Usuario { get; set; }


        [NotMapped]
        public bool selecionado { get; set; }

        private bool validaExistencia(DbControle db, ProjetoUsuario obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(ProjetoUsuario obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<ProjetoUsuario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public bool Excluir(int id, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;

                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.ProjetoUsuario.Remove(obj);
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


        public bool Excluir( ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                var obj = ObterPorId(this.id,  db, pb);
                new LogMudanca().Incluir(obj, "", "", db, pb);
                db.ProjetoUsuario.Remove(obj);
                
                db.SaveChanges();
                return true;
                
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

        public ProjetoUsuario ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public ProjetoUsuario ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ProjetoUsuario.Where(x => x.id == id && x.Projeto.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<ProjetoUsuario> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ProjetoUsuario.Where(x => x.Projeto.estabelecimento_id == estab).ToList();
        }



        public List<ProjetoUsuario> ObterTodosPorProjeto(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ProjetoUsuario.Where(x => x.projeto_id == id && x.Projeto.estabelecimento_id == estab).ToList();
        }

        public void Alterar(ProjetoUsuario obj, DbControle db)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ProjetoUsuario ObterCategoriaApontador(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ProjetoUsuario.Where(x => x.usuario_id == id && x.Projeto.estabelecimento_id == estab).FirstOrDefault();
        }
    
    }
}
