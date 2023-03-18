using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftFin.Web.Models
{
    public class Imposto: BaseModels
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Codigo"), Required(ErrorMessage = "Informe o código")]
        public string codigo { get; set; }
        [Display(Name = "Descricao"), Required(ErrorMessage = "Informe a descrição")]
        public string descricao { get; set; }



        //ID	IMPOSTO	DESCRICAO
        //1	    ISS	    Imposto sobre Serviço
        //2	    PIS	    Programa de Incentivo Social    
        //3	    COFINS	Contribuição Fim Social
        //4	    CSLL	Contribuição sobre Lucro Líquido
        //5	    IRRF	Imposto de Renda Retido na Fonte
        //public bool Excluir(int id)
        //{
        //    DbControle banco = new DbControle();
        //    var obj = banco.Imposto.Where(x => x.id == id).First();
        //    if (obj == null)
        //        return false;
        //    else
        //    {
        //        banco.Set<Imposto>().Remove(obj);
        //        banco.SaveChanges();
        //        return true;
        //    }
        //}

        //public bool Alterar(Imposto obj)
        //{
        //    DbControle banco = new DbControle();
        //    banco.Entry(obj).State = EntityState.Modified;
        //    banco.SaveChanges();
        //    return true;
        //}

        //public  bool Incluir(Imposto obj)
        //{
        //    DbControle banco = new DbControle();
        //    var objAux = banco.Imposto.Where(x => x.descricao == obj.descricao).FirstOrDefault();
        //    if (objAux != null)
        //        return false;
        //    else
        //    {
        //        banco.Set<Imposto>().Add(obj);
        //        banco.SaveChanges();
        //        return true;
        //    }
        //}



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
                    db.Imposto.Remove(obj);
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

        public bool Alterar(Imposto obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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


        private bool validaExistencia(DbControle db, Imposto obj)
        {
            DbControle banco = new DbControle();
            var objAux = banco.Imposto.Where(x => x.descricao == obj.descricao).FirstOrDefault();
            if (objAux != null)
                return true;
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Imposto obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Imposto>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Imposto ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Imposto ObterPorId(int id, DbControle db)
        {
            
            if (db == null)
                db = new DbControle();

            return db.Imposto.Where(x => x.id == id).FirstOrDefault();
        }
        public List<Imposto> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.Imposto.ToList();
        }
  
    }
}