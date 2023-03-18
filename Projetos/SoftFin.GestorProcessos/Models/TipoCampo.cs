using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class TipoCampo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(250)]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public string TemplateHTML { get; set; }
        public string TemplateHTMLModal { get; set; }

        public string TemplateJSControl { get; set; }

        public string TemplateJSScript { get; set; }

        [MaxLength(30)]
        public string TipoBancoDados { get; set; }

        [MaxLength(500)]
        public string SQLDefault { get; set; }



        private bool ValidaExistencia(DBGPControle db, TipoCampo obj)
        {
            return db.TipoCampo.Where(x => x.Descricao == obj.Descricao).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TipoCampo obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<TipoCampo>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(TipoCampo obj, ParamBase pb)
        {
            var db = new DBGPControle();

            var objAux = ObterPorId(obj.Id, pb);
            if (objAux == null)
                return false;
            else
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                var db = new DBGPControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    db.TipoCampo.Remove(obj);
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


        public TipoCampo ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public TipoCampo ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.TipoCampo.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<TipoCampo> ObterTodos(ParamBase pb)
        {
            var db = new DBGPControle();
            return db.TipoCampo.ToList();
        }
    }
}