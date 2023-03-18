using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class TipoContrato : BaseModels
    {
        public int id { get; set; }
        
        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Required(ErrorMessage = "Campo Tipo obrigatório")]
        public string tipo { get; set; }
        public virtual IEnumerable<TipoContrato> TipoContratos { get; set; }

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
                    db.TipoContrato.Remove(obj);
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

        public bool Alterar(TipoContrato obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, TipoContrato obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TipoContrato obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<TipoContrato>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public TipoContrato ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

          public TipoContrato ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.TipoContrato.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }
        public List<TipoContrato> ObterTodos(ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();
            var objs = db.TipoContrato.Where(x => x.empresa_id == idempresa).ToList();
            if (objs.Count() == 0)
            {
                ConfiguraZero(db);
                objs = db.TipoContrato.Where(x => x.empresa_id == idempresa).ToList();
            }
            return objs;

        }


        private void ConfiguraZero(DbControle db)
        {
            //var obj = new TipoContrato();
            //obj.tipo = "Projeto";
            //obj.empresa_id  = pb.empresa_id;
            //obj.Incluir();

            //obj = new TipoContrato();
            //obj.tipo = "Alocação";
            //obj.empresa_id  = pb.empresa_id;
            //obj.Incluir();

            //obj = new TipoContrato();
            //obj.tipo = "Manutenção";
            //obj.empresa_id  = pb.empresa_id;
            //obj.Incluir();

            //obj = new TipoContrato();
            //obj.tipo = "Suporte";
            //obj.empresa_id  = pb.empresa_id;
            //obj.Incluir();

        }
    }
}