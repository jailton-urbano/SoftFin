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
    public class OrigemMovimento
    {
        //Jarvis
        //Conteúdos Permitidos
        //Tipo="A", Modulo="Faturamento"
        //Tipo="A", Modulo="Recebimento"

        [Key]
        public int id { get; set; }

        [Display(Name = "Tipo"), Required(ErrorMessage = "*"), MaxLength(30)]
        public string Tipo { get; set; }

        [Display(Name = "Modulo"), Required(ErrorMessage = "*"), MaxLength(30)]
        public string Modulo { get; set; }




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
                    db.OrigemMovimento.Remove(obj);
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

        public bool Alterar(OrigemMovimento obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, OrigemMovimento obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(OrigemMovimento obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<OrigemMovimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public OrigemMovimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public OrigemMovimento ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.OrigemMovimento.Where(x => x.id == id).FirstOrDefault();
        }
        public List<OrigemMovimento> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.ToList();
        }


        public int TipoManual(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.Where(x => x.Tipo == "M").FirstOrDefault().id;
        }


        public int TipoTransferencia(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.Where(x => x.Tipo == "TR").FirstOrDefault().id;
        }
        public int TipoFaturamento(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.Where(x => x.Modulo == "Faturamento").FirstOrDefault().id;
        }

        public int TipoCaixa(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.Where(x => x.Tipo == "CX").FirstOrDefault().id;
        }

        public int TipoNotaDebito(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.Where(x => x.Modulo == "ND").FirstOrDefault().id;
        }
        public int TipoCPAG(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrigemMovimento.Where(x => x.Tipo == "A").FirstOrDefault().id;
        }
    }
}