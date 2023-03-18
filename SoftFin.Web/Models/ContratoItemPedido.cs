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
    public class ContratoItemPedido
    {
        public int Id { get; set; }

        [Display(Name = "Parcela Contrato"), Required(ErrorMessage = "Informe o Parcela Contrato")]
        public int ParcelaContrato_Id { get; set; }

        [JsonIgnore, ForeignKey("ParcelaContrato_Id")]
        public virtual ParcelaContrato ParcelaContrato { get; set; }

        [MaxLength(30),Required(ErrorMessage = "Informe o Pedido")]
        public string Pedido { get; set; }
        [MaxLength(50)]
        public string Descricao { get; set; }
        
        public decimal Valor { get; set; }

        [Display(Name = "Unidade de Negocio"), Required(ErrorMessage = "Informe a unidade de negocio")]
        public int unidadenegocio_id { get; set; }

        [JsonIgnore, ForeignKey("unidadenegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }


        public void Excluir(int Id, DbControle db)
        {
            var obj = db.ContratoItemPedido.Where(x => x.Id == Id).First();
            db.Set<ContratoItemPedido>().Remove(obj);
            db.SaveChanges();
        }

        public bool Incluir(ContratoItemPedido obj, DbControle db)
        {

            var contratoPesquisar = new ContratoItemPedido();
            db.ContratoItemPedido.Add(obj);
            db.SaveChanges();
            return true;

        }

        public void Alterar(ContratoItemPedido obj)
        {
            var banco = new DbControle();
            banco.Entry(obj).State = EntityState.Modified;
            banco.SaveChanges();
        }

        public ContratoItemPedido ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public ContratoItemPedido ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ContratoItemPedido.Where(x => x.Id == id).FirstOrDefault();
        }


        public List<ContratoItemPedido> ObterPorParcela(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ContratoItemPedido.Where(x => x.ParcelaContrato_Id == id).ToList();
        }
        public List<ContratoItemPedido> ObterTodos(ParamBase pb, DbControle db, int? iditem = 0)
        {
            int estab = pb.estab_id;
            
            return db.ContratoItemPedido.Where(x => x.ParcelaContrato_Id == iditem).ToList();
        }
    }
}