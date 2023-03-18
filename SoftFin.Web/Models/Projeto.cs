using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class Projeto: BaseModels
    {
        public Projeto()
        {
            ProjetoUsuarios = new List<ProjetoUsuario>();
        }

        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o estabelecimento")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Projeto Pai")]
        public int? projeto_id { get; set; }

        [Display(Name = "Projeto Pai"), ForeignKey("projeto_id")]
        public virtual Projeto ProjetoPai { get; set; }

        [Display(Name = "Codigo Projeto"), Required(ErrorMessage = "Informe o código de projeto*")]
        public string codigoProjeto { get; set; }

        [Display(Name = "Nome Projeto"), Required(ErrorMessage = "Informe o nome de projeto")]
        public string nomeProjeto { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime dataInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime dataFinal { get; set; }
       
        [Display(Name = "Total Horas"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal totalHoras { get; set; }

        [Display(Name = "Contrato Item")]
        public int? ContratoItem_id { get; set; }

        [JsonIgnore,ForeignKey("ContratoItem_id")]
        public virtual ContratoItem ContratoItem { get; set; }

        public bool ativo { get; set; }

        [JsonIgnore]
        public List<ProjetoUsuario> ProjetoUsuarios { get; set; }


        //public virtual IEnumerable<Atividade> atividades { get; set; }

        public bool Excluir(ref string erro, ParamBase pb, DbControle db = null)
       {
           return Excluir(this.id, ref erro, pb, db); 
       }

        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
       {
           try
           {
               int estab = pb.estab_id;
               if (db == null)
                db = new DbControle();
               var obj = ObterPorId(id, db, pb);
               if (obj == null)
               {
                   erro = "Registro não encontrado";
                   return false;
               }
               else
               {
                   new LogMudanca().Incluir(obj, "", "",db, pb);
                   db.Projeto.Remove(obj);
                   db.SaveChanges();
                   return true;
               }
           }

           catch (Exception ex)
           {
               if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
               {
                   erro = "Registro esta relacionado com outro cadastro";
                   return false;
               }
               else
               {
                   throw ex;
               }
           }
       }

       public bool Alterar(ParamBase pb, DbControle db = null)
       {
           return Alterar(this, pb, db);
       }
       public bool Alterar(Projeto obj, ParamBase pb, DbControle db = null)
       {
           if (db == null)
            db = new DbControle();

           var objAux = ObterPorId(obj.id, pb);
           if (objAux == null)
               return false;
           else
           {
               new LogMudanca().Incluir(obj, objAux, "",db, pb);
               db.Entry(obj).State = EntityState.Modified;
               db.SaveChanges();

               return true;
           }
       }


       private bool validaExistencia(DbControle db, Projeto obj)
       {
           return (false);
       }
       public bool Incluir(ParamBase pb, DbControle db = null)
       {    
           return Incluir(this, pb);
       }
       public bool Incluir(Projeto obj, ParamBase pb, DbControle db = null)
       {
           if (db == null)
                db = new DbControle();

           if (validaExistencia(db, obj))
               return false;
           else
           {
               new LogMudanca().Incluir(obj, "", "",db, pb);

               db.Set<Projeto>().Add(obj);
               db.SaveChanges();

               return true;
           }
       }


       public Projeto ObterPorId(int id, ParamBase pb)
       {
           return ObterPorId(id, null, pb);
       }
       public Projeto ObterPorId(int id, DbControle db, ParamBase pb)
       {
           int estab = pb.estab_id;
           if (db == null)
               db = new DbControle();

            return db.Projeto.Where(x => x.id == id && x.estabelecimento_id == estab).Include(p => p.ProjetoUsuarios).FirstOrDefault();
       }
       public IQueryable<Projeto> ObterTodos(ParamBase pb)
       {
           int estab = pb.estab_id;
           DbControle db = new DbControle();
           return db.Projeto.Where(x => x.estabelecimento_id == estab).Include(p => p.ProjetoUsuarios).Include(p => p.ContratoItem.Contrato.Pessoa);
       }


       public SelectList CarregaProjeto(ParamBase pb)
       {
           var con1 = ObterTodos(pb).ToList();
           var items = new List<SelectListItem>();
           foreach (var item in con1)
           {
               items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2}", item.codigoProjeto, item.nomeProjeto,
                   (item.ContratoItem == null)? "": item.ContratoItem.Contrato.Pessoa.nome)});
           }
           var listret = new SelectList(items, "Value", "Text");
           return listret;
       }

       public SelectList CarregaProjetoID(int id, ParamBase pb)
       {
           var con1 = ObterTodos(pb).Where(x=> x.id == id);
           var items = new List<SelectListItem>();
           foreach (var item in con1)
           {
                items.Add(new SelectListItem()
                {
                    Value = item.id.ToString(),
                    Text = String.Format("{0} - {1} - {2}", item.codigoProjeto, item.nomeProjeto,
                    (item.ContratoItem == null) ? "" : item.ContratoItem.Contrato.Pessoa.nome)
                });
            }
            var listret = new SelectList(items, "Value", "Text");
           return listret;
       }
  
    }
}