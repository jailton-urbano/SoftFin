using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class TipoRPS
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Municipio")]
        public int? municipio_id { get; set; }
        [JsonIgnore,ForeignKey("municipio_id")]
        public virtual Municipio municipio { get; set; }

        [Display(Name = "Código"), Required(ErrorMessage = "*")]
        public string codigo { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*")]
        public string descricao { get; set; }



        public TipoRPS ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }

        public TipoRPS ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();
            return db.TipoRPS.Where(x => x.id == id).FirstOrDefault();
        }

        public List<TipoRPS> ObterTodos(ParamBase pb)
        {
            int municipio_id = Acesso.EstabLogadoObj2().Municipio_id;
            DbControle db = new DbControle();
            return db.TipoRPS.Where(x => x.municipio_id == municipio_id).ToList();
        }
    }
}