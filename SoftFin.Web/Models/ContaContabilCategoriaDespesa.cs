using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class ContaContabilCategoriaDespesa: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Conta Contábil"), Required(ErrorMessage = "Informe a conta contabil")]
        public int contaContabil_id { get; set; }

        [JsonIgnore, ForeignKey("contaContabil_id")]
        public virtual ContaContabil ContaContabil { get; set; }
        
        [Display(Name = "Categoria Despesa"), Required(ErrorMessage = "Informe a categoria despesa")]
        public int planoDeContas_id { get; set; }

        [JsonIgnore, ForeignKey("planoDeContas_id")]
        public virtual PlanoDeConta PlanoDeConta { get; set; }



        [Display(Name = "empresa"), Required(ErrorMessage = "Informe a empresa")]
        public int empresa_id { get; set; }

        [JsonIgnore, ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        private bool validaExistencia(DbControle db, ContaContabilCategoriaDespesa obj)
        {
            return (false);
        }

        internal bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        internal bool Incluir(ContaContabilCategoriaDespesa obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<ContaContabilCategoriaDespesa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        internal bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        internal bool Alterar(ContaContabilCategoriaDespesa obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        internal bool Excluir(ref string erro, ParamBase paramBase)
        {
            return Excluir(this.id, ref erro, paramBase);
        }
        internal bool Excluir(int id, ref string erro, ParamBase paramBase)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.ContaContabilCategoriaDespesa.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
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


        public ContaContabilCategoriaDespesa ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public ContaContabilCategoriaDespesa ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.ContaContabilCategoriaDespesa.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }



        internal List<ContaContabilCategoriaDespesa> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            var banco = new DbControle();

            return banco.ContaContabilCategoriaDespesa.Where(x => x.empresa_id == idempresa).ToList();
        }

        internal ContaContabilCategoriaDespesa ObterPorPlanoContas(int planoDeConta_id, ParamBase paramBase, DbControle db)
        {
            if (db == null)
                db = new DbControle();
            int idempresa = paramBase.empresa_id;
            return db.ContaContabilCategoriaDespesa.Where(x => x.empresa_id == idempresa && x.planoDeContas_id == planoDeConta_id).FirstOrDefault();
        }

        internal ContaContabilCategoriaDespesa ObterPorCC(int planoDeConta_id, ParamBase paramBase, DbControle db)
        {
            if (db == null)
                db = new DbControle();
            int idempresa = paramBase.empresa_id;
            return db.ContaContabilCategoriaDespesa.Where(x => x.empresa_id == idempresa && x.contaContabil_id == planoDeConta_id).FirstOrDefault();
        }

    }
}