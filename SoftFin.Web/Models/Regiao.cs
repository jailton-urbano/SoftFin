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
    public class Regiao
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Nome"), Required(ErrorMessage = "*")]
        public string nome { get; set; }


        [Display(Name = "Estabelecimento")]
        public int? estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }


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
                    db.Regiao.Remove(obj);
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
        public bool Alterar(ParamBase pb, Regiao obj)
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
        private bool validaExistencia(DbControle db, Regiao obj)
        {
            return (db.Regiao.Where(p => p.nome == obj.nome).Count() > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Regiao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Regiao>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public Regiao ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Regiao ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.Regiao.Where(x => x.id == id).FirstOrDefault();
        }
        public List<Regiao> ObterTodos(int estab)
        {
            DbControle db = new DbControle();
            return db.Regiao.Where(p => p.estabelecimento_id == estab).ToList();
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
    }
}