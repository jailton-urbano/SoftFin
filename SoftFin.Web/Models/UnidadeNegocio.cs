using System;
using System.Collections.Generic;
using System.Linq;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class UnidadeNegocio : BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Unidade"),
        Required(ErrorMessage = "Campo unidade obrigatório"),
        MaxLength(50)]
        public string unidade { get; set; }

        [Display(Name = "Código"),
        MaxLength(20)]
        public string Codigo { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.UnidadeNegocio.Remove(obj);
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

        public bool Alterar(UnidadeNegocio obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
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


        private bool validaExistencia(DbControle db, UnidadeNegocio obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(UnidadeNegocio obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(new { obj.empresa_id, obj.unidade, obj.id }, "", "",db, pb);

                db.Set<UnidadeNegocio>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public UnidadeNegocio ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public UnidadeNegocio ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.UnidadeNegocio.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }
        public List<UnidadeNegocio> ObterTodos(ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();
            return db.UnidadeNegocio.Where(x => x.empresa_id == idempresa).ToList();
        }

        public List<UnidadeNegocio> ObterTodosporIdEmpresa(int idempresa)
        {

            DbControle db = new DbControle();
            return db.UnidadeNegocio.Where(x => x.empresa_id == idempresa).ToList();
        }

    }
}
