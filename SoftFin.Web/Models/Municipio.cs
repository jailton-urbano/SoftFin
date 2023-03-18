using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class Municipio
    {
        [Key]
        public int ID_MUNICIPIO { get; set; }

        [MaxLength(100)]
        public string DESC_MUNICIPIO { get; set; }

        [MaxLength(2)]
        public string UF { get; set; }

        [MaxLength(15)]
        public string codigoIBGE { get; set; }
        public Municipio ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Municipio ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.Municipio.Where(x => x.ID_MUNICIPIO == id).FirstOrDefault();
        }
        public List<Municipio> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.Municipio.ToList();
        }


        public List<Municipio> ObterPorNome(string p, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            p = p.ToUpper();
            return db.Municipio.Where(x => x.DESC_MUNICIPIO.ToUpper().Equals(p)).ToList();
        }

        public Municipio ObterPorCodigoIBGE(string codigoIBGE, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();

            return db.Municipio.Where(x => x.codigoIBGE == codigoIBGE).FirstOrDefault();
        }
    }
}