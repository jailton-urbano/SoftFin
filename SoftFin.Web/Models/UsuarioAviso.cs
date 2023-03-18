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

    public class UsuarioAviso //: GenericRepository<UsuarioAviso>
    {
        //public UsuarioAviso(DbControle context = null)
        //    : base(context)
        //{

        //}

        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Usuário"), Required(ErrorMessage = "*")]
        public int usuario_id { get; set; }
        [JsonIgnore,ForeignKey("usuario_id")]
        public virtual Usuario usuario { get; set; }

        [Display(Name = "Data Postagem"), Required(ErrorMessage = "*")]
        public DateTime dataPostagem { get; set; }

        [Display(Name = "Data Final Exibição")]
        public DateTime? dataFinalExibicao { get; set; }

        [Display(Name = "Mensagen"), Required(ErrorMessage = "*")]
        public string mensagem { get; set; }

        [Display(Name = "Url")]
        public string link { get; set; }

        [Display(Name = "Url")]
        public string Descricaolink { get; set; }

        [Display(Name = "Data Lido")]
        public DateTime? datalido { get; set; }

        [Display(Name = "Lido"), Required(ErrorMessage = "*")]
        public bool lido { get; set; }

        [Display(Name = "Titulo"), Required(ErrorMessage = "*")]
        public string titulo { get; set; }

        //public bool Alterar(UsuarioAviso obj)
        //{
        //    DbControle db = new DbControle();

        //    var objAux = ObterPorId(obj.id);
        //    if (objAux == null)
        //        return false;
        //    else
        //    {
        //        db.Entry(obj).State = EntityState.Modified;
        //        db.SaveChanges();

        //        return true;
        //    }
        //}




        //public UsuarioAviso ObterPorId(int id)
        //{
        //    return ObterPorId(id, null);
        //}
        //public UsuarioAviso ObterPorId(int id, DbControle db)
        //{
        //    int idempresa  = pb.empresa_id;
        //    if (db == null)
        //        db = new DbControle();

        //    return db.UsuarioAviso.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        //}
        public List<UsuarioAviso> ObterTodos( ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.UsuarioAviso.Where(x => x.empresa_id == idempresa).ToList();
        }

        public List<UsuarioAviso> ObterTodosNaoLidos(int usuid, int idempresa)
        {
            DbControle db = new DbControle();
            return db.UsuarioAviso.Where(x => x.empresa_id == idempresa && x.usuario_id == usuid && x.lido == false).ToList();
        }

    }

}