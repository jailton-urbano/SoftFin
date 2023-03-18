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
    public class RamoSeguro
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(100)]
        public string descricao { get; set; }

        [Display(Name = "Identificador"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string identificador { get; set; }

        [JsonIgnore,ForeignKey("GrupoRamoSeguro_id")]
        public virtual GrupoRamoSeguro GrupoRamoSeguro { get; set; }

        [Display(Name = "GrupoRamo"), Required(ErrorMessage = "*")]
        public int GrupoRamoSeguro_id { get; set; }


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
                    db.RamoSeguro.Remove(obj);
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
        public bool Alterar(RamoSeguro obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, RamoSeguro obj)
        {
            return (db.RamoSeguro.Where(p => p.descricao == obj.descricao).Count() > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(RamoSeguro obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<RamoSeguro>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public RamoSeguro ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public RamoSeguro ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.RamoSeguro.Where(x => x.id == id).FirstOrDefault();
        }
        public List<RamoSeguro> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.RamoSeguro.ToList();
        }
        public List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState)
        {
            var erros = new List<string>();
            if (ModelState != null)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                erros.AddRange(from a in allErrors select a.ErrorMessage);
            }
            return erros;
        }

        public List<RamoSeguro> ObterTodosByGrupo(int id)
        {
            DbControle db = new DbControle();
            return db.RamoSeguro.Where(p => p.GrupoRamoSeguro_id == id).ToList();

        }
    }
}