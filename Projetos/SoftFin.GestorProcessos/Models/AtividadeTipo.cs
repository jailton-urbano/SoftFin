using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class AtividadeTipo
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        [MaxLength(15)]
        public string Codigo { get; set; }

        private bool ValidaExistencia(DBGPControle db, AtividadeTipo obj)
        {
            return db.AtividadeTipo.Where(x => x.Codigo == obj.Codigo).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(AtividadeTipo obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<AtividadeTipo>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(AtividadeTipo obj, ParamBase pb)
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
                    db.AtividadeTipo.Remove(obj);
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
        public AtividadeTipo ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public AtividadeTipo ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.AtividadeTipo.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<AtividadeTipo> ObterTodos(ParamBase pb)
        {
            var db = new DBGPControle();
            return db.AtividadeTipo.ToList();
        }


    }
}