using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class EstabelecimentoCodigoLanctoContabil
    {
        [Key,Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore, ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        public int codigoLancto { get; set; }

        public int ObterUltimoLacto(ParamBase pb, DbControle db)
        {
            var aux = db.EstabelecimentoCodigoLanctoContabil.Where(x => x.estabelecimento_id == pb.estab_id).FirstOrDefault();

            if (aux == null)
            {
                db.EstabelecimentoCodigoLanctoContabil.Add(new EstabelecimentoCodigoLanctoContabil
                {
                    codigoLancto = 1,
                    estabelecimento_id = pb.estab_id
                });
                aux = new EstabelecimentoCodigoLanctoContabil();
                aux.codigoLancto = 1;
            }
            else
            {
                aux.codigoLancto += 1;
                db.Entry(aux).State = EntityState.Modified;
                
            }
            db.SaveChanges();
            return aux.codigoLancto;
        }
    }
}