using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class TipoDocumento
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Codigo"),
        Required(ErrorMessage = "*"),
        StringLength(20)]
        public string codigo { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "*"),
        StringLength(35)]
        public string descricao { get; set; }

        [Display(Name = "Historico Padrão"),
        Required(ErrorMessage = "*"),
        StringLength(35)]
        public string historicoPadrao { get; set; }

        [Display(Name = "Inativo")]
        public bool Inativo { get; set; }

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
                    db.TipoDocumento.Remove(obj);
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
        public bool Alterar(TipoDocumento obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, TipoDocumento obj)
        {
            return (db.TipoDocumento.Where(p => p.descricao == obj.descricao).Count() > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TipoDocumento obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<TipoDocumento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public TipoDocumento ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public TipoDocumento ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.TipoDocumento.Where(x => x.id == id).FirstOrDefault();
        }
        public List<TipoDocumento> ObterTodos()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.ToList();
        }

        public int TipoNotaPromissoria()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.Where(x => x.codigo == "NOTPRO").FirstOrDefault().id;
        }
        public int TipoNotaDebito()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.Where(x => x.codigo == "ND").FirstOrDefault().id;
        }
        public int TipoNota()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.Where(x => x.codigo == "NF").FirstOrDefault().id;
        }

        public int TipoCreditoConta()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.Where(x => x.codigo == "CC").FirstOrDefault().id;
        }


        public int TipoTransf()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.Where(x => x.codigo == "Transf").FirstOrDefault().id;
        }

        public int TipoFechamentoCaixa()
        {

            DbControle db = new DbControle();
            return db.TipoDocumento.Where(x => x.codigo == "FECCAI").FirstOrDefault().id;
        }
        public SelectList CarregaCboGeral()
        {
            var con1 = ObterTodos();
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}", item.descricao) });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }


        public TipoDocumento ObterPorCodigo(string codigo)
        {
            return ObterPorCodigo(codigo, null);
        }
        public TipoDocumento ObterPorCodigo(string codigo, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.TipoDocumento.Where(x => x.codigo == codigo).FirstOrDefault();
        }

    }
}
