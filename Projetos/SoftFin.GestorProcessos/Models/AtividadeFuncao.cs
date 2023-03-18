using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class AtividadeFuncao : BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        public int IdAtividade { get; set; }
        [JsonIgnore, ForeignKey("IdAtividade")]
        public virtual Atividade Atividade { get; set; }
        public int IdFuncao { get; set; }
        [JsonIgnore, ForeignKey("IdFuncao")]
        public virtual Funcao Funcao { get; set; }


        private bool ValidaExistencia(DBGPControle db, AtividadeFuncao obj)
        {
            return db.AtividadeFuncao.Where(x => x.IdFuncao == obj.IdFuncao).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(AtividadeFuncao obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<AtividadeFuncao>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(AtividadeFuncao obj, ParamBase pb)
        {
            var db = new DBGPControle();

            var objAux = ObterPorId(obj.Id,db, pb);
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
                    db.AtividadeFuncao.Remove(obj);
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
        public AtividadeFuncao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public AtividadeFuncao ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.AtividadeFuncao.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<AtividadeFuncao> ObterTodos(int IdAtividade, ParamBase pb)
        {
            var db = new DBGPControle();
            return db.AtividadeFuncao.Where(x => x.IdAtividade == IdAtividade).ToList();
        }


    }
}