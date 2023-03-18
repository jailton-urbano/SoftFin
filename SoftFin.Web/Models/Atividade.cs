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

namespace SoftFin.Web.Models
{
    public class Atividade: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Descrição")]
        public string descricao { get; set; }
        [Display(Name = "Sequencia")]
        public int sequencia { get; set; }
        [Display(Name = "Predescessora")]
        public int? predescessora_id { get; set; }
        [Display(Name = "Sucessora")]
        public int? sucessora_id { get; set; }
        //public virtual IEnumerable<Pessoa> pessoas { get; set; }

        [Display(Name = "Projeto")]
        public int projeto_id { get; set; }

        [JsonIgnore,ForeignKey("projeto_id")]
        public virtual Projeto Projeto { get; set; }


        [Display(Name = "Data Inicial")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Quantidade Horas"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal qtdHoras { get; set; }


        public bool Excluir(ParamBase pb, DbControle db = null)
        {
            return Excluir(this.id, pb, db);
        }

        public bool Excluir(int id, ParamBase pb, DbControle db = null)
        {
           try
           {
               int estab = pb.estab_id;
               
               var obj = ObterPorId(id, db,pb);
               if (obj == null)
               {
                   return false;
               }
               else
               {
                   new LogMudanca().Incluir(obj, "", "",db, pb);
                   db.Atividade.Remove(obj);
                   db.SaveChanges();
                   return true;
               }
           }
           catch (Exception ex)
           {
               if (ex.Message.IndexOf("FK") > 0)
               {
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
        public bool Alterar(Atividade obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,db,pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);

                db.Entry(objAux).CurrentValues.SetValues(obj);
                db.Entry(objAux).State = EntityState.Modified;

                db.SaveChanges();

                return true;
            }
       }


       private bool validaExistencia(DbControle db, Atividade obj)
       {
           return (false);
       }
       public bool Incluir(ParamBase pb, DbControle db = null)
       {
           return Incluir(this, pb, db);
       }
       public bool Incluir(Atividade obj, ParamBase pb, DbControle db = null)
       {
           if (db == null)
                db = new DbControle();

           if (validaExistencia(db, obj))
               return false;
           else
           {
               new LogMudanca().Incluir(obj, "", "", db, pb);

               db.Set<Atividade>().Add(obj);
               db.SaveChanges();

               return true;
           }
       }
       public Atividade ObterPorId(int id, ParamBase pb)
       {
           return ObterPorId(id, null,  pb);
       }
       public Atividade ObterPorId(int id, DbControle db, ParamBase pb)
       {
           int estab = pb.estab_id;
           if (db == null)
               db = new DbControle();

           return db.Atividade.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
       }
       public List<Atividade> ObterTodos(ParamBase pb)
       {
           int estab = pb.estab_id;
           DbControle db = new DbControle();
           return db.Atividade.Where(x => x.estabelecimento_id == estab).ToList();
       }



       public List<Atividade> ObterTodosPorIdProjeto(int idprojeto, ParamBase pb)
       {
           int estab = pb.estab_id;
           DbControle db = new DbControle();
           return db.Atividade.Where(x => x.estabelecimento_id == estab &&  x.projeto_id == idprojeto).ToList();
       }

       public List<Atividade> ObterTodosPorUsuario(ParamBase pb)
       {
            var usu = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return (from y in db.Atividade
                  join c in db.AtividadeUsuario
                      on y.id equals c.atividade_id
                      where c.usuario_id == usu
                      orderby y.DataInicial
                  select y).ToList();
       }
    }
}