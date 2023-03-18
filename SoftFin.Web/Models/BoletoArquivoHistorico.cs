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
    public class BoletoArquivoHistorico
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Data Vencimento"), Required(ErrorMessage = "Informe a data de Vencimento")]
        public DateTime DataGeracao { get; set; }
        public string Caminho { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o Extabelecimeno")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore, ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }



        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(BoletoArquivoHistorico obj, ParamBase pb)
        {
            DbControle db = new DbControle();
            new LogMudanca().Incluir(obj, "", "", db, pb);
            db.Set<BoletoArquivoHistorico>().Add(obj);
            db.SaveChanges();
            return true;
        }
        public List<BoletoArquivoHistorico> ObterTodos(ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            return db.BoletoArquivoHistorico.
                Where(x => x.estabelecimento_id == paramBase.estab_id).OrderByDescending(p => p.DataGeracao)
                .ToList();
        }
        public BoletoArquivoHistorico ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public BoletoArquivoHistorico ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.BoletoArquivoHistorico.Where(x => x.id == id).FirstOrDefault();
        }

    }
}