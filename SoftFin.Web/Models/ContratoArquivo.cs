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
    public class ContratoArquivo
    {
        public int id { get; set; }


        [Display(Name = "Arquivo Real"),
        Required(ErrorMessage = "*"),MaxLength(200)]
        public string arquivoReal { get; set; }

        [Display(Name = "Arquivo Original"),
        Required(ErrorMessage = "*"), MaxLength(200)]
        public string arquivoOriginal { get; set; }

        [Required(ErrorMessage = "*")]
        public int contrato_id { get; set; }

        [JsonIgnore,ForeignKey("contrato_id")]
        public virtual Contrato Contrato { get; set; }


        [ MaxLength(200)]
        public string descricao { get; set; }

        
        public List<ContratoArquivo> ObterPorContrato(int id,ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ContratoArquivo.Where(x => x.contrato_id == id && x.Contrato.estabelecimento_id == estab).ToList();
        }

        public ContratoArquivo ObterPorId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ContratoArquivo.Where(x => x.id == id && x.Contrato.estabelecimento_id == estab).FirstOrDefault();
        }

        public void Excluir(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            var x = ObterPorId(id, pb,db);
            db.ContratoArquivo.Remove(x);
            db.SaveChanges();
        }

        public void Salvar(ParamBase pb)
        {
            DbControle db = new DbControle();
            new LogMudanca().Incluir(this, "", "", db, pb);

            db.Set<ContratoArquivo>().Add(this);
            db.SaveChanges();
        }
    }
}