using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class Representante
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string descricao { get; set; }

        [JsonIgnore,ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "*")]
        public int pessoa_id { get; set; }


        [JsonIgnore,ForeignKey("TipoRepresentante_id")]
        public virtual TipoRepresentante TipoRepresentante { get; set; }

        [Display(Name = "Tipo Representante"), Required(ErrorMessage = "*")]
        public int TipoRepresentante_id { get; set; }

        //public List<Representante> ObterTodos()
        //{
        //    DbControle db = new DbControle();
        //    var objs = db.Representante.ToList();
        //    return objs;
        //}
    }
}