using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SoftFin.Web.Models
{
    public class TipoMovimento
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Codigo"),
        Required(ErrorMessage = "*"),
        MaxLength(30)]
        public string Codigo { get; set; }

        [Display(Name = "Descricao"),
        Required(ErrorMessage = "*"),
        MaxLength(30)]
        public string Descricao { get; set; }

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
                    db.TipoMovimento.Remove(obj);
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

        public bool Alterar(TipoMovimento obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, TipoMovimento obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TipoMovimento obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<TipoMovimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public TipoMovimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public TipoMovimento ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.TipoMovimento.Where(x => x.id == id).FirstOrDefault();
        }
        public List<TipoMovimento> ObterTodos(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.TipoMovimento.ToList();
        }

        public int TipoEntrada(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.TipoMovimento.Where(x =>  x.Codigo == "ENT").FirstOrDefault().id;
        }

        public int TipoEntrada(DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            return db.TipoMovimento.Where(x => x.Codigo == "ENT").FirstOrDefault().id;
        }

        public int TipoSaida(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.TipoMovimento.Where(x => x.Codigo == "SAI").FirstOrDefault().id;
        }

    }
}
