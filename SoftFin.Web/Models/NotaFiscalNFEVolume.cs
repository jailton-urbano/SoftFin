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
    public class NotaFiscalNFEVolume
    {
        [Key()]
        public int id { get; set; }

        public int notaFiscal_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }
        public int qtde { get; set; }

        [MaxLength(150)]
        public string especie { get; set; }
        [MaxLength(150)]
        public string marca { get; set; }

        [MaxLength(150)]
        public string numeracao { get; set; }
        public decimal pesoLiquido { get; set; }
        public decimal pesoBruto { get; set; }
        [MaxLength(150)]
        public string lacres { get; set; }

        public List<NotaFiscalNFEVolume> ObterPorNf(int nf, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            return db.NotaFiscalNFEVolume.Where(x => x.notaFiscal_id == nf).ToList();
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFEVolume.Remove(this);
            banco.SaveChanges();
        }

        public void Incluir(DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(this, "", "", db, pb);
            db.Set<NotaFiscalNFEVolume>().Add(this);
            db.SaveChanges();
            return;
        }
    }


}