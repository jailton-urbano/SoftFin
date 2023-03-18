using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class Calendario
    {
        public int id { get; set; }

        [Display(Name = "Data"),
        Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime data { get; set; }

        [Display(Name = "Descrição")]
        public string descricao { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Tipo Data"),
        Required(ErrorMessage = "*")]
        public int tipoData_ID { get; set; }
        [JsonIgnore,ForeignKey("tipoData_ID")]
        public virtual TipoDataCalendario tipoDataCalendario {get; set; }

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
                    db.Calendario.Remove(obj);
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

        private bool validaExistencia(DbControle db, Calendario obj)
        {
            return (false);
        }

        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(Calendario obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Calendario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public bool Altera(Calendario obj)
        {
            var db = new DbControle();
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<Calendario> lista(int idCalendario)
        {
            var banco = new DbControle();
            var lista = banco.Calendario.Where(p => p.id == idCalendario).ToList();
            return lista;
        }

        public Calendario ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public Calendario ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.Calendario.Where(x => x.id == id && x.empresa_id == empresa).FirstOrDefault();
        }

        public List<Calendario> ObterTodos(ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.Calendario.Where(x => x.empresa_id == empresa).ToList();
        }

        public List<Calendario> ObterTodosPeriodo(DateTime dataInicial, DateTime dataFinal, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.Calendario.Where(x => x.empresa_id == empresa && x.data >= dataInicial && x.data <= dataFinal).ToList();
        }

    }
}