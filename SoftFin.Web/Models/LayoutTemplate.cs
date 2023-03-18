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
    public class LayoutTemplate : BaseModels
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Código"), Required(ErrorMessage = "Informe o Código"), MaxLength(50)]
        public string Codigo { get; set; }

        [Display(Name = "Template"), Required(ErrorMessage = "Informe o Template")]
        public string Template { get; set; }


        [Display(Name = "Empresa"), Required(ErrorMessage = "Informe a Empresa")]
        public int Empresa_id { get; set; }

        [JsonIgnore, ForeignKey("Empresa_id")]
        public virtual Empresa Empresa { get; set; }




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
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.LayoutTemplate.Remove(obj);
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

        public bool Alterar(ParamBase pb, DbControle db)
        {
            return Alterar(this, pb, db);
        }


        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb, null);
        }

        public bool Alterar(LayoutTemplate obj, ParamBase pb)
        {
            return Alterar(obj, pb, null);
        }

        public bool Alterar(LayoutTemplate obj, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.Id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(LayoutTemplate obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();


            new LogMudanca().Incluir(obj, "", "", banco, pb);
            banco.Set<LayoutTemplate>().Add(obj);
            banco.SaveChanges();
            return true;
        }

        public LayoutTemplate ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }

        public LayoutTemplate ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.LayoutTemplate.Where(x => x.Empresa_id == paramBase.empresa_id && x.Id == id).FirstOrDefault();
        }

        public LayoutTemplate ObterPorCodigo(string codigo, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.LayoutTemplate.Where(x => x.Empresa_id == paramBase.empresa_id && x.Codigo == codigo).FirstOrDefault();
        }


        public List<LayoutTemplate> ObterTodos(ParamBase paramBase)
        {
            DbControle banco = new DbControle();
            return banco.LayoutTemplate.Where(x => x.Empresa_id == paramBase.empresa_id).ToList();
        }

        public IQueryable<LayoutTemplate> ObterTodosQ(ParamBase paramBase)
        {
            DbControle banco = new DbControle();
            return banco.LayoutTemplate.Where(x => x.Empresa_id == paramBase.empresa_id);
        }
        public List<LayoutTemplate> ObterTodos(DbControle banco, ParamBase paramBase)
        {

            return banco.LayoutTemplate.Where(x => x.Empresa_id == paramBase.empresa_id).ToList();
        }

    }
}