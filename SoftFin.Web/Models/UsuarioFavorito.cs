using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{

    public class UsuarioFavorito 
    {
        [Key]
        public int id { get; set; }

        public DateTime DataInclusao { get; set; }

        public int Funcionalidade_id { get; set; }

        [JsonIgnore,ForeignKey("Funcionalidade_id")]
        public virtual Funcionalidade Funcionalidade { get; set; }

        public int Usuario_id { get; set; }

        [JsonIgnore,ForeignKey("Usuario_id")]
        public virtual Usuario Usuario { get; set; }

        public List<UsuarioFavorito> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.UsuarioFavorito.ToList();
        }

        public UsuarioFavorito ObterPorIdFuncionalida(int id, ParamBase pb)
        {
            return ObterPorIdFuncionalida(id, null, pb);
        }
        public UsuarioFavorito ObterPorIdFuncionalida(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.UsuarioFavorito.Where(x => x.Funcionalidade_id == id ).FirstOrDefault();
        }


        public string SalvaFavorito(int id, ParamBase pb)
        {

            DbControle db = new DbControle();
            var usuario_id = Acesso.idUsuarioLogado();

            var objAux = ObterPorIdFuncionalida(id, db, pb);
            if (objAux != null)
            {
                db.UsuarioFavorito.Remove(objAux);
                db.SaveChanges();
                return "Adicionado";
            }
            else
            {
                db.UsuarioFavorito.Add(new UsuarioFavorito{DataInclusao = DateTime.Now, Funcionalidade_id = id, Usuario_id = usuario_id});
                db.SaveChanges();
                return "Adicionado";
            }
        
        }

        public List<UsuarioFavorito> ObterTodosPorUsuario(int  usuario)
        {
            DbControle db = new DbControle();
            if (db == null)
                db = new DbControle();

            return db.UsuarioFavorito.Where(x => x.Usuario_id == usuario).ToList();
        }
    }

}