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
    public class AtendimentoTipoHistorico
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Codigo"), Required(ErrorMessage = "*")]
        public string codigo { get; set; }
        [Display(Name = "Descricao"), Required(ErrorMessage = "*")]
        public string descricao { get; set; }



        public bool Excluir(ParamBase pb)
        {
            return Excluir(this.id,pb);
        }
        public bool Excluir(int id,ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var obj = ObterPorId(id, db,pb);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.AtendimentoTipoHistorico.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(AtendimentoTipoHistorico obj, ParamBase pb)
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
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(AtendimentoTipoHistorico obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<AtendimentoTipoHistorico>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle banco, AtendimentoTipoHistorico obj, ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.AtendimentoTipoHistorico.Where(x =>
                    x.descricao == obj.descricao).FirstOrDefault();
            return (objAux != null);
        }
        public AtendimentoTipoHistorico ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null);
        }
        public AtendimentoTipoHistorico ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.AtendimentoTipoHistorico.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<AtendimentoTipoHistorico> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.AtendimentoTipoHistorico.ToList();
        }
    }

    
}
