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

    public class UsuarioMaisAcessado 
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

        public List<UsuarioMaisAcessado> ObterTodos()
        {
            
            DbControle db = new DbControle();
            return db.UsuarioMaisAcessado.ToList();
        }

        

    }

}