using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class CFOP 
    {
        [Key]
        public int id { get; set; }

        [MaxLength(5)]
        public string codigo { get; set; }

        [MaxLength(500)]
        public string descricao { get; set; }

        [MaxLength(1)]
        public string indNFe        { get; set; }
        [MaxLength(1)]
        public string indComunica    { get; set; }
        [MaxLength(1)]
        public string indTransp        { get; set; }
        [MaxLength(1)]
        public string indDevol        { get; set; }






        public CFOP ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public CFOP ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.CFOP.Where(x => x.id == id).FirstOrDefault();
        }
        public CFOP ObterPorCfop(string cfop, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();

            return db.CFOP.Where(x => x.codigo == cfop).FirstOrDefault();
        }
        public List<CFOP> ObterTodos()
        {
            DbControle db = new DbControle();
            var objs = db.CFOP.ToList();
            return objs;
        }

    }
}
