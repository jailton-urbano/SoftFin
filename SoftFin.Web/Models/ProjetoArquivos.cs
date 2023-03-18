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

namespace SoftFin.Web.Models
{
    /*public class ProjetoArquivo
    {

        public int id { get; set; }

        [Display(Name = "Projeto")]
        public int projeto_id { get; set; }

        [Display(Name = "Nome Original"), Required(ErrorMessage = "*")]
        public string NomeOriginal { get; set; }

        [Display(Name = "Pasta Arquivado"), Required(ErrorMessage = "*")]
        public string PastaOriginal { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*")]
        public string Descricao { get; set; }


        //public virtual IEnumerable<Atividade> atividades { get; set; }

       public bool Excluir(ref string erro, ParamBasepb)
       {
           return Excluir(this.id, ref erro, pb); 
       }

       public bool Excluir(int id, ref string erro, ParamBase pb)
       {
           try
           {
               int estab = pb.estab_id;
               DbControle db = new DbControle();
               var obj = ObterPorId(id, db);
               if (obj == null)
               {
                   erro = "Registro não encontrado";
                   return false;
               }
               else
               {
                   new LogMudanca().Incluir(obj, "", "",db, pb);
                   db.ProjetoArquivos.Remove(obj);
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

       public bool Alterar()
       {
           return Alterar(this);
       }
       public bool Alterar(ProjetoArquivos obj)
       {
           DbControle db = new DbControle();

           var objAux = ObterPorId(obj.id);
           if (objAux == null)
               return false;
           else
           {
               new LogMudanca().Incluir(objAux, "");
               new LogMudanca().Incluir(obj, "", "",db, pb);
               db.Entry(obj).State = EntityState.Modified;
               db.SaveChanges();

               return true;
           }
       }


       private bool validaExistencia(DbControle db, ProjetoArquivos obj)
       {
           return (false);
       }
       public bool Incluir()
       {
           return Incluir(this);
       }
       public bool Incluir(ProjetoArquivos obj)
       {
           DbControle db = new DbControle();

           if (validaExistencia(db, obj))
               return false;
           else
           {
               new LogMudanca().Incluir(obj, "", "",db, pb);

               db.Set<ProjetoArquivos>().Add(obj);
               db.SaveChanges();

               return true;
           }
       }


       public ProjetoArquivos ObterPorId(int id)
       {
           return ObterPorId(id, null);
       }
       public ProjetoArquivos ObterPorId(int id, DbControle db)
       {
           int estab = pb.estab_id;
           if (db == null)
               db = new DbControle();

           return db.Projeto.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
       }
       public List<ProjetoArquivos> ObterTodos()
       {
           int estab = pb.estab_id;
           DbControle db = new DbControle();
           return db.Projeto.Where(x => x.estabelecimento_id == estab).ToList();
       }
  
    }*/
}