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
    public class EmpresaContaContabil: BaseModels
    {
        [Key]
        public int empresa_id { get; set; }
        [JsonIgnore, ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        public int ContaContabilTitulo_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilTitulo_id")]
        public virtual ContaContabil ContaContabilTitulo { get; set; }

        public int ContaContabilPagamento_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilPagamento_id")]
        public virtual ContaContabil ContaContabilPagamento { get; set; }

        public int ContaContabilRecebimento_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilRecebimento_id")]
        public virtual ContaContabil ContaContabilRecebimento { get; set; }
        
        public int ContaContabilNFMercadoria_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilNFMercadoria_id")]
        public virtual ContaContabil ContaContabilNFMercadoria { get; set; }

        public int ContaContabilNFServico_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilNFServico_id")]
        public virtual ContaContabil ContaContabilNFServico { get; set; }

        public int ContaContabilOutro_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilOutro_id")]
        public virtual ContaContabil ContaContabilOutro { get; set; }

        public int? ContaContabilRecebimentoDC_id { get; set; }

        [JsonIgnore, ForeignKey("ContaContabilRecebimentoDC_id")]
        public virtual ContaContabil ContaContabilRecebimentoDC { get; set; }

        public EmpresaContaContabil ObterPorEmpresa(ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            return db.EmpresaContaContabil.Where(x => x.empresa_id == paramBase.empresa_id).FirstOrDefault();
        }
        private bool validaExistencia(DbControle db, EmpresaContaContabil obj)
        {
            return (false);
        }
        internal bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        internal bool Incluir(EmpresaContaContabil obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", null, paramBase);

                db.Set<EmpresaContaContabil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        internal bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }
        internal bool Alterar(EmpresaContaContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.empresa_id, paramBase);

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
            return Excluir(this.empresa_id , ref erro, paramBase);
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
                    new LogMudanca().Incluir(obj, "", "", null, paramBase);
                    db.EmpresaContaContabil.Remove(obj);
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


        public EmpresaContaContabil ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public EmpresaContaContabil ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.EmpresaContaContabil.Where(x => x.empresa_id == idempresa).FirstOrDefault();
        }

        internal List<EmpresaContaContabil> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            var banco = new DbControle();

            return banco.EmpresaContaContabil.Where(x => x.empresa_id == idempresa).ToList();
        }
    }
}