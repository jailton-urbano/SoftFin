using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class UsuarioFuncao
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }

        [JsonIgnore, ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
        public int IdFuncao { get; set; }

        [JsonIgnore, ForeignKey("IdFuncao")]
        public virtual Funcao Funcao { get; set; }
    }
}