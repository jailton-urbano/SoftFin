using Newtonsoft.Json;
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
    public class SistemaArquivo
    {
        public SistemaArquivo()
        {
            DataInclucao = DateTime.Now;
        }
        public int id { get; set; }

        public DateTime DataInclucao { get; set; }

        [MaxLength(50)]
        public String Descricao { get; set; }

        [Display(Name = "Arquivo Real"),
        Required(ErrorMessage = "*"), MaxLength(750)]
        public string arquivoReal { get; set; }
        [Display(Name = "Arquivo Original"),
        Required(ErrorMessage = "*"), MaxLength(750)]
        public string arquivoOriginal { get; set; }

        public int tamanho { get; set; }

        [MaxLength(10)]
        public string arquivoExtensao { get; set; }

        [MaxLength(75)]
        public string rotinaOwner { get; set; }

        [MaxLength(75)]
        public string codigo { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        public List<SistemaArquivo> ObterPorOwner(string owner, ParamBase pb, DbControle db)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.SistemaArquivo.Where(x => x.rotinaOwner == owner).ToList();
        }

        public SistemaArquivo ObterPorId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.SistemaArquivo.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public void Excluir(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            var x = ObterPorId(id, pb,db);
            db.SistemaArquivo.Remove(x);
            db.SaveChanges();
        }

        public void Salvar(ParamBase pb)
        {
            DbControle db = new DbControle();
            new LogMudanca().Incluir(this, "", "", db, pb);

            db.Set<SistemaArquivo>().Add(this);
            db.SaveChanges();
        }

        public List<SistemaArquivo> ObterPorNomeDeArquivo(string owner, string codigo, string nomearquivo, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.SistemaArquivo.Where(x => x.rotinaOwner == owner && x.codigo == codigo && x.arquivoOriginal == nomearquivo).ToList();
        }

        public List<SistemaArquivo> ObterOwnerCodigo(string owner, string codigo, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.SistemaArquivo.Where(x => x.rotinaOwner == owner && x.codigo == codigo ).ToList();
        }
    }


}