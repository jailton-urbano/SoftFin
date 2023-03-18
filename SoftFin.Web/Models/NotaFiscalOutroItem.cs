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
    public class NotaFiscalOutroItem
    {
        [Key]
        public int Id { get; set; }

        [ Display(Name = "Nota Fiscal"), Required(ErrorMessage = "Informe o Nota Fiscal")]
        public int notafical_id { get; set; }

        [JsonIgnore, ForeignKey("notafical_id")]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [MaxLength(20)]
        public string Codigo { get; set; }


        [MaxLength(70)]
        public string Descricao { get; set; }
        
        public decimal Valor { get; set; }

        [Display(Name = "Unidade de Negocio"), Required(ErrorMessage = "Informe a unidade de negocio")]
        public int unidadenegocio_id { get; set; }

        [JsonIgnore, ForeignKey("unidadenegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }


        public void Excluir(int Id, DbControle db, ParamBase pb)
        {
            var obj = db.NotaFiscalOutroItem.Where(x => x.Id == Id).First();
            new LogMudanca().Incluir(obj, "", "", db, pb);
            db.Set<NotaFiscalOutroItem>().Remove(obj);
            db.SaveChanges();
        }

        public bool Incluir(NotaFiscalOutroItem obj, DbControle db, ParamBase pb)
        {

            var contratoPesquisar = new NotaFiscalOutroItem();
            new LogMudanca().Incluir(obj, "", "", db, pb);
            db.NotaFiscalOutroItem.Add(obj);
            db.SaveChanges();
            return true;

        }

        public void Alterar(NotaFiscalOutroItem obj, ParamBase pb)
        {
            var db = new DbControle();
            new LogMudanca().Incluir(obj, "", "", db, pb);
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public NotaFiscalOutroItem ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public NotaFiscalOutroItem ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.NotaFiscalOutroItem.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<NotaFiscalOutroItem> ObterPorNF(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.NotaFiscalOutroItem.Where(x => x.notafical_id == id).ToList();
        }

        public List<NotaFiscalOutroItem> ObterTodos(ParamBase pb, DbControle db, int? iditem = 0)
        {
            int estab = pb.estab_id;
            
            return db.NotaFiscalOutroItem.Where(x => x.notafical_id == iditem).ToList();
        }
    }
}