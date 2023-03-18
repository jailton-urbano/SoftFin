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
    public class Atividade: BaseModel
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Descricao { get; set; }
        [MaxLength(20)]

        public string Codigo { get; set; }
        public bool Ativo { get; set; }
        public string Url { get; set; }

        public int IdAtividadeTipo { get; set; }
        [JsonIgnore, ForeignKey("IdAtividadeTipo")]
        public virtual AtividadeTipo AtividadeTipo { get; set; }
        public int IdProcesso { get; set; }
        [JsonIgnore, ForeignKey("IdProcesso")]
        public virtual Processo Processo { get; set; }

        private bool ValidaExistencia(DBGPControle db, Atividade obj)
        {
            return db.Atividade.Where(x => x.Codigo == obj.Codigo).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Atividade obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<Atividade>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(Atividade obj, ParamBase pb)
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
                    db.Atividade.Remove(obj);
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
        public Atividade ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Atividade ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.Atividade.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<Atividade> ObterTodos(int IdProcesso, ParamBase pb)
        {
            var db = new DBGPControle();
            return db.Atividade.Where(x => x.IdProcesso == IdProcesso).ToList();
        }



    }
}