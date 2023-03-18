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
    public class SFDetalhe : BaseModels
    {
        [Key]
        public int id { get; set; }

        [MaxLength(500)]
        public string tabela { get; set; }

        public int idAfetado { get; set; }

        public DateTime novoVencimento { get; set; }
        
        [Required(ErrorMessage = "*")]
        public int SFCapa_id { get; set; }

        [JsonIgnore,ForeignKey("SFCapa_id")]
        public virtual SFCapa SFCapa { get; set; }

        [MaxLength(1)]
        public string flag_debcred { get; set; }


        public List<SFDetalhe> ObterTodos(int idcapa)
        {
            DbControle db = new DbControle();
            return db.SFDetalhe.Where(x => x.SFCapa_id == idcapa).ToList();
        }

        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }

        public bool Incluir(SFDetalhe obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();


            new LogMudanca().Incluir(obj, "", "", db, pb);

            db.Set<SFDetalhe>().Add(obj);
            db.SaveChanges();
            return true;
        }

        public bool Alterar(ParamBase pb, DbControle db= null)
        {
            return Alterar(this, pb, db); ;
        }

        public bool Alterar(SFDetalhe obj, ParamBase pb, DbControle db= null)
        {
            if (db  == null)
                db = new DbControle();

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

        public SFDetalhe ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public SFDetalhe ObterPorId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            return db.SFDetalhe.Where(x => x.id == id).FirstOrDefault();
        }

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
                    db.SFDetalhe.Remove(obj);
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


        public SFDetalhe ObterPorCpagCapa(int p1, int p2,  DbControle db = null)
        {
            if (db == null)
                db = new DbControle();


            return db.SFDetalhe.Where(x => x.id == p1 && x.SFCapa_id == p2).FirstOrDefault();
        }



        public List<SFDetalhe> ObterPorCapa(int id)
        {
            var db = new DbControle();

            return db.SFDetalhe.Where(x => x.SFCapa_id == id).ToList();
        }

        public decimal valor { get; set; }

        public DateTime VencimentoOriginal { get; set; }


        public List<SFDetalhe> ObterTodosTranferidos(DateTime dataInicial, DateTime dataFinal, int id)
        {
            var db = new DbControle();

            return db.SFDetalhe.Where(x => x.SFCapa_id == id && x.novoVencimento >= dataInicial && x.novoVencimento <= dataFinal ).ToList();
        }
    }
}