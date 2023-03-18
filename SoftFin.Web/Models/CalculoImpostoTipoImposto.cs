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
    public class CalculoImpostoTipoImposto
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Tipo Base")]
        public int tipoBase_id { get; set; }

        [JsonIgnore,ForeignKey("tipoBase_id")]
        public virtual TipoBase TipoBase { get; set; }
        
        public bool ativo { get; set; }

        [Display(Name = "calculoImposto")]
        public int calculoImposto_id { get; set; }

        [JsonIgnore,ForeignKey("calculoImposto_id")]
        public virtual calculoImposto calculoImposto { get; set; }


        public bool Incluir(CalculoImpostoTipoImposto obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();


            new LogMudanca().Incluir(obj, "", "",db, pb);

            db.Set<CalculoImpostoTipoImposto>().Add(obj);
            db.SaveChanges();

            return true;
            
        }





        public List<CalculoImpostoTipoImposto> ObterTodos(int p)
        {
              DbControle db = new DbControle();
            var objs = db.CalculoImpostoTipoImposto.Where(x => x.calculoImposto_id == p).ToList();
            return objs;
        }
    }
}