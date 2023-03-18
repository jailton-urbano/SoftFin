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
    public class AtividadeVisao:BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int Ordem { get; set; }
        public int IdAtividade { get; set; }
        [JsonIgnore, ForeignKey("IdAtividade")]
        public virtual Atividade Atividade { get; set; }
        public int IdVisao { get; set; }
        [JsonIgnore, ForeignKey("IdVisao")]
        public virtual Visao Visao { get; set; }
        public string Titulo { get; set; }

        private bool ValidaExistencia(DBGPControle db, AtividadeVisao obj)
        {
            return db.AtividadeVisao.Where(x => x.IdAtividade == obj.IdAtividade).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(AtividadeVisao obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<AtividadeVisao>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(AtividadeVisao obj, ParamBase pb)
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
                    db.AtividadeVisao.Remove(obj);
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
        public AtividadeVisao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public AtividadeVisao ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.AtividadeVisao.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<AtividadeVisao> ObterTodos(int IdAtividade, ParamBase pb)
        {
            var db = new DBGPControle();
            return db.AtividadeVisao.Where(x => x.IdAtividade == IdAtividade).ToList();
        }

    }
}