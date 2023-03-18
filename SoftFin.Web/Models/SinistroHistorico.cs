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
    public class SinistroHistorico
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "*"), StringLength(200)]
        public string usuario { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "*"), StringLength(3000)]
        public string historico { get; set; }

        [JsonIgnore,ForeignKey("sinistro_id")]
        public virtual Sinistro Sinistro { get; set; }

        [Required(ErrorMessage = "*")]
        public int sinistro_id { get; set; }

        public List<SinistroHistorico> ObteTodosPorIdSinistro(int id)
        {
            DbControle db = new DbControle();
            return db.SinistroHistorico.Where(p => p.sinistro_id == id).ToList(); ;
        }

        public void Incluir(SinistroHistorico sinistroHistorico)
        {
            DbControle db = new DbControle();

            db.Set<SinistroHistorico>().Add(sinistroHistorico);
            db.SaveChanges();

        }
    }
}