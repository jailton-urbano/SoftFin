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
    public class DocumentoPagarArquivo
    {
        public int id { get; set; }


        [Display(Name = "Arquivo Real"),
        Required(ErrorMessage = "*"),MaxLength(200)]
        public string arquivoReal { get; set; }

        [Display(Name = "Arquivo Original"),
        Required(ErrorMessage = "*"), MaxLength(200)]
        public string arquivoOriginal { get; set; }

        [Required(ErrorMessage = "*")]
        public int documentoPagarMestre_id { get; set; }

        [JsonIgnore,ForeignKey("documentoPagarMestre_id")]
        public virtual DocumentoPagarMestre DocumentoPagarMestre { get; set; }

        public string descricao { get; set; }

        public List<DocumentoPagarArquivo> ObterPorCPAG(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarArquivo.Where(x => x.documentoPagarMestre_id == id && x.DocumentoPagarMestre.estabelecimento_id == estab).ToList();
        }

        public List<DocumentoPagarArquivo> ObterPorCPAGArquivo(int id, string nomeArquivo, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarArquivo.Where(x => x.documentoPagarMestre_id == id
            && x.DocumentoPagarMestre.estabelecimento_id == estab
            && x.arquivoReal == nomeArquivo).ToList();
        }
        public DocumentoPagarArquivo ObterPorId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarArquivo.Where(x => x.id == id && x.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();
        }

        public void Excluir(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            var x = ObterPorId(id, pb,db);
            db.DocumentoPagarArquivo.Remove(x);
            db.SaveChanges();
        }

        public void Salvar(ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            new LogMudanca().Incluir(this, "", "", db, pb);

            db.Set<DocumentoPagarArquivo>().Add(this);
            db.SaveChanges();
        }


    }
}