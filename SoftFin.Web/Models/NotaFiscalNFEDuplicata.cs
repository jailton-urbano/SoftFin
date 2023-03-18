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
    public class NotaFiscalNFEDuplicata
    {
        [Key]
        public int id { get; set; }

        public int notaFiscal_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }

        [MaxLength(15)]
        public string numero { get; set; }
        
        public DateTime vencto { get; set; }

        public decimal valor { get; set; }


        public List<NotaFiscalNFEDuplicata> ObterPorNf(int nf)
        {
            DbControle db = new DbControle();
            return db.NotaFiscalNFEDuplicata.Where(x => x.notaFiscal_id == nf).ToList();
        }


        public List<NotaFiscalNFEDuplicata> ObterTodos(int id)
        {
            return ObterTodos(id, null);
        }
        public List<NotaFiscalNFEDuplicata> ObterTodos(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFEDuplicata.Where(nf => nf.notaFiscal_id == id).ToList();
            return nfe;
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFEDuplicata.Remove(this);
            banco.SaveChanges();
        }

        public void Incluir(DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(this, "", "", db, pb);
            db.Set<NotaFiscalNFEDuplicata>().Add(this);
            db.SaveChanges();
            return;
        }
    }
}