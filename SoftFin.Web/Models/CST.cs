using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class CST: BaseModels
    {
        [Key]
        public int id { get; set; }

        [MaxLength(5)]
        public string codigo { get; set; }

        [MaxLength(500)]
        public string descricao { get; set; }

        public int imposto_id { get; set; }




        public CST ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public CST ObterPorId(int id, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();

            return db.CST.Where(x => x.id == id).FirstOrDefault();
        }
        public List<CST> ObterPorImposto(int impostoid, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();

            return db.CST.Where(x => x.imposto_id == impostoid).ToList();
        }
        public List<CST> ObterTodos()
        {
            DbControle db = new DbControle();
            var objs = db.CST.ToList();
            return objs;
        }
    }
}