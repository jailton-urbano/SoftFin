using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class PlanoDeConta
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Codigo"), Required(ErrorMessage = "*"), StringLength(15)]
        public string codigo { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), StringLength(50)]
        public string descricao { get; set; }

        [Display(Name = "Nivel Superior")]
        public int? nivelSuperior { get; set; }

        [JsonIgnore,ForeignKey("nivelSuperior")]
        public virtual PlanoDeConta PlanoDeContasnivelSuperior { get; set; }

        [Display(Name = "Tipo de Conta"), StringLength(1)]
        public string TipoConta { get; set; }

        [Display(Name = "Débito ou Crédito(D/C)"), StringLength(1)]
        public string DebitoCredito { get; set; }

        public bool Ativo { get; set; }

        public bool Excluir(int id)
        {
            DbControle banco = new DbControle();
            var obj = banco.PlanoDeConta.Where(x => x.id == id).First();
            if (obj == null)
                return false;
            else
            {
                banco.Set<PlanoDeConta>().Remove(obj);
                banco.SaveChanges();
                return true;
            }
        }

        public bool Alterar(PlanoDeConta obj)
        {
            DbControle banco = new DbControle();
            banco.Entry(obj).State = EntityState.Modified;
            banco.SaveChanges();
            return true;
        }

        public bool Incluir(PlanoDeConta obj)
        {
            DbControle banco = new DbControle();
            var objAux = banco.PlanoDeConta.Where(x => x.descricao == obj.descricao).FirstOrDefault();
            if (objAux != null)
                return false;
            else
            {
                banco.Set<PlanoDeConta>().Add(obj);
                banco.SaveChanges();
                return true;
            }
        }

        public PlanoDeConta ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public PlanoDeConta ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.PlanoDeConta.Where(x => x.id == id).FirstOrDefault();
        }
        public List<PlanoDeConta> ObterTodos()
        {
            DbControle db = new DbControle();
            //var xs = db.PlanoDeConta.ToList();
            var xs = db.PlanoDeConta.OrderBy(p => p.codigo).ToList();
            return xs;
        }
        public List<PlanoDeConta> ObterTodos(DbControle db)
        {
            if (db == null)
                db = new DbControle();
            //var xs = db.PlanoDeConta.ToList();
            var xs = db.PlanoDeConta.OrderBy(p => p.codigo).ToList();
            return xs;
        }
        public SelectList ObterTodosTipoA()
        {
            var objs = ObterTodos().Where(p => p.TipoConta == "A" && p.Ativo == true).OrderBy(p => p.codigo);
            var items = new List<SelectListItem>();
            foreach (var item in objs)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(),
                    Text = String.Format("{0} {1} ", (item.codigo + "___________").Substring(0,11) + " - ", item.descricao)});
            }
            return new SelectList(items, "Value", "Text");
        }

        public SelectList ObterTodosTipoAS()
        {
            var objs = ObterTodos().Where(p => p.TipoConta == "A" && p.Ativo == true).OrderBy(p => p.codigo);
            var items = new List<SelectListItem>();
            foreach (var item in objs)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(),
                    Text = String.Format("{0} {1} ", (item.codigo + "___________").Substring(0, 11) + " - ", item.descricao) });
            }

            return new SelectList(items, "Value", "Text");
        }

        public SelectList ObterNotadeDebito()
        {
            var objs = ObterTodos().Where(p => p.TipoConta == "A" && (p.id == 14 || p.id == 64) && p.Ativo == true).OrderBy(p => p.codigo);
            var items = new List<SelectListItem>();
            foreach (var item in objs)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} {1} ", (item.codigo + "___________").Substring(0, 11) + " - ", item.descricao) });
            }
            return new SelectList(items, "Value", "Text");
        }

        public int Recebimento(ParamBase pb)
        {
            int estab  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.PlanoDeConta.Where(x =>  x.codigo == "01" && x.Ativo == true).FirstOrDefault().id;
        }

        public PlanoDeConta ObterPorCodigo(string codigo, DbControle db)
        {
            var obj = ObterTodos(db).Where(p => p.codigo == codigo).FirstOrDefault();
            return obj;
        }

        public List<PlanoDeConta> ObterTodosDebito()
        {
            var objs = ObterTodos().Where(p => p.TipoConta == "A" && p.Ativo == true).OrderBy(p => p.codigo).ToList();
            return objs;
        }

    }
}