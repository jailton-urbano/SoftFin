using Newtonsoft.Json;
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
    public class UsuarioEstabelecimento
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Usuário"),
        Required(ErrorMessage = "*")]
        public int usuario_id { get; set; }
        [Display(Name = "Estabelecimento"),
        Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("usuario_id")]

        public virtual Usuario Usuario { get; set; }
        
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }


        public bool Excluir(int id, ParamBase pb)
        {
            DbControle db = new DbControle();
            var obj = ObterPorId(id, db, pb);
            db.UsuarioEstabelecimento.Remove(obj);
            db.SaveChanges();
            return true;
        }


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
                    db.UsuarioEstabelecimento.Remove(obj);
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

        public bool Alterar(UsuarioEstabelecimento obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, db, pb);
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


        private bool validaExistencia(DbControle db, UsuarioEstabelecimento obj, ParamBase pb)
        {
            if (obj.ObterTodos(pb).Where(x => x.usuario_id == obj.usuario_id && x.estabelecimento_id == obj.estabelecimento_id).Count() > 0)
            {
                return true;
            }
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(UsuarioEstabelecimento obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<UsuarioEstabelecimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public UsuarioEstabelecimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public UsuarioEstabelecimento ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.UsuarioEstabelecimento.Where(p => p.id == id).FirstOrDefault();
        }
        public List<UsuarioEstabelecimento> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.UsuarioEstabelecimento.Where(p => p.estabelecimento_id == estab).ToList();
        }

        public List<UsuarioEstabelecimento> ObterTodosRelacionados(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.UsuarioEstabelecimento.ToList();
        }
        public List<UsuarioEstabelecimento> ObterTodosEmail(int idusuario)
        {

            DbControle db = new DbControle();
            return db.UsuarioEstabelecimento.Where(p => p.usuario_id == idusuario).ToList();
        }



        public List<UsuarioEstabelecimento> ObterTodosPorIdUsuario(int idusuario)
        {
            DbControle db = new DbControle();
            return db.UsuarioEstabelecimento.Where(p => p.usuario_id == idusuario).ToList();
        }
    }
}
