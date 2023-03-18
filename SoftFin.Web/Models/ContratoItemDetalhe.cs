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
    public class ContratoItemUnidadeNegocio
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Parcela Contrato"), Required(ErrorMessage = "Informe o Parcela Contrato")]
        public int ContratoItem_Id { get; set; }

        [JsonIgnore, ForeignKey("ContratoItem_Id")]
        public virtual ContratoItem ContratoItem { get; set; }



        [Display(Name = "Unidade"), Required(ErrorMessage = "Informe a Unidade Contrato")]
        public int UnidadeNegocio_Id { get; set; }

        [JsonIgnore, ForeignKey("UnidadeNegocio_Id")]
        public virtual UnidadeNegocio UnidadeNegocio  { get; set; }
    
        [MaxLength(50)]
        public string Descricao { get; set; }
        
        public decimal Valor { get; set; }



        public void Excluir(int Id, DbControle db)
        {
            var obj = db.ContratoItemUnidadeNegocio.Where(x => x.Id == Id).First();
            db.Set<ContratoItemUnidadeNegocio>().Remove(obj);
            db.SaveChanges();
        }

        public bool Incluir(ContratoItemUnidadeNegocio obj, DbControle db)
        {

            var contratoPesquisar = new ContratoItemUnidadeNegocio();
            db.ContratoItemUnidadeNegocio.Add(obj);
            db.SaveChanges();
            return true;

        }

        public void Alterar(ContratoItemUnidadeNegocio obj)
        {
            var banco = new DbControle();
            banco.Entry(obj).State = EntityState.Modified;
            banco.SaveChanges();
        }

        public ContratoItemUnidadeNegocio ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public ContratoItemUnidadeNegocio ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ContratoItemUnidadeNegocio.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<ContratoItemUnidadeNegocio> ObterTodos(ParamBase pb, DbControle db, int? iditem = 0)
        {
            int estab = pb.estab_id;
            
            return db.ContratoItemUnidadeNegocio.Where(x => x.ContratoItem_Id == iditem).ToList();
        }
    }
}