using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SoftFin.Web.Models
{
    public class AtendimentoStatus
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Codigo"), Required(ErrorMessage = "*")]
        public string codigo { get; set; }
        [Display(Name = "Descricao"), Required(ErrorMessage = "*")]
        public string descricao { get; set; }


        public bool Excluir( ParamBase pb)
        {
            return Excluir(this.id,pb);
        }
        public bool Excluir(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var obj = ObterPorId(id, db);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.AtendimentoStatus.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(AtendimentoStatus obj, ParamBase pb)
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
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(AtendimentoStatus obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<AtendimentoStatus>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle banco, AtendimentoStatus obj, ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.AtendimentoStatus.Where(x =>
                    x.descricao == obj.descricao).FirstOrDefault();
            return (objAux != null);
        }
        public AtendimentoStatus ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public AtendimentoStatus ObterPorId(int id, DbControle banco)
        {
            
            if (banco == null)
                banco = new DbControle();

            return banco.AtendimentoStatus.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<AtendimentoStatus> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.AtendimentoStatus.ToList();
        }

        public int StatusEmAberto( ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.AtendimentoStatus.Where(x => x.codigo == "ABERTO").FirstOrDefault().id;

        }

    }
}
