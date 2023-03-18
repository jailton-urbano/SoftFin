using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;

namespace SoftFin.Web.Models
{
    public class OpcaoTributariaSimples
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Código"), Required(ErrorMessage = "*")]
        public string codigo { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*")]
        public string descricao { get; set; }

        public OpcaoTributariaSimples ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public OpcaoTributariaSimples ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();
            return db.OpcaoTributariaSimples.Where(x => x.id == id ).FirstOrDefault();
        }

        public List<OpcaoTributariaSimples> ObterTodos( ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.OpcaoTributariaSimples.ToList();
        }
    }
}