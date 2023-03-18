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
    public class AtividadePlano: BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int ProcessoId { get; set; }
        [JsonIgnore, ForeignKey("ProcessoId")]
        public virtual Processo Processo { get; set; }


        public int AtividadeId { get; set; }
        [JsonIgnore, ForeignKey("AtividadeId")]
        public virtual Atividade Atividade { get; set; }
       
        public int? AtividadeIdEntrada { get; set; }
        [JsonIgnore, ForeignKey("AtividadeIdEntrada")]
        public virtual Atividade AtividadeEntrada { get; set; }
        public string CondicaoEntrada { get; set; }

        public bool Inicial { get; set; }
        public bool Final { get; set; }

        private bool ValidaExistencia(DBGPControle db, AtividadePlano obj)
        {
            return db.AtividadePlano.Where(x => x.AtividadeId == obj.AtividadeId).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(AtividadePlano obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<AtividadePlano>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(AtividadePlano obj, ParamBase pb)
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
                    db.AtividadePlano.Remove(obj);
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
        public AtividadePlano ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public AtividadePlano ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.AtividadePlano.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<AtividadePlano> ObterTodos(int IdProcesso, ParamBase pb)
        {
            var db = new DBGPControle();
            return db.AtividadePlano.Where(x => x.ProcessoId == IdProcesso).ToList();
        }

    }
}