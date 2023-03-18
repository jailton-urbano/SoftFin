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
    public class SaldoContabilDetalhe : BaseModels
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe a Conta Contabil"), MaxLength(20)]
        public string CodigoConta { get; set; }

        [Required(ErrorMessage = "Informe a Descricao Conta Contabil"), MaxLength(150)]
        public string DescricaoConta { get; set; }

        [Required(ErrorMessage = "Informe a Conta Centro Custo"), MaxLength(20)]
        public string CodigoCentroCusto { get; set; }

        [Required(ErrorMessage = "Informe a Descricao Centro Custo"), MaxLength(150)]
        public string DescricaoCentroCusto { get; set; }


        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }
        public decimal TotalCredito { get; set; }
        public decimal TotalDebito { get; set; }
        public int SaldoContabil_id { get; set; }
        [JsonIgnore, ForeignKey("SaldoContabil_id")]
        public virtual SaldoContabil SaldoContabil { get; set; }
        [NotMapped]
        public bool Deleted { get;  set; }

        private bool ValidaExistencia(DbControle db, SaldoContabilDetalhe obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase, DbControle db)
        {
            return Incluir(this, paramBase, db);
        }
        public bool Incluir(SaldoContabilDetalhe obj, ParamBase paramBase, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",null,paramBase);

                db.Set<SaldoContabilDetalhe>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase, DbControle db)
        {
            return Alterar(this, paramBase, db);
        }
        public bool Alterar(SaldoContabilDetalhe obj, ParamBase paramBase, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.Id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", null, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool Excluir(ref string erro, ParamBase paramBase, DbControle db  = null)
        {
            return Excluir(this.Id, ref erro, paramBase, db);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase, DbControle db = null)
        {
            try
            {
                if (db == null)
                    db = new DbControle();
                
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.SaldoContabilDetalhe.Remove(obj);
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


        public SaldoContabilDetalhe ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public SaldoContabilDetalhe ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;

            if (banco == null)
                banco = new DbControle();

            return banco.SaldoContabilDetalhe.Where(x => x.Id == id && x.SaldoContabil.Estabelecimento_id == idestab).FirstOrDefault();
        }

        public List<SaldoContabilDetalhe> ObterTodos(int saldoContabil_id, ParamBase paramBase, DbControle banco = null)
        {
            int idempresa = paramBase.empresa_id;
            
            if (banco == null)
                banco = new DbControle();

            return banco.SaldoContabilDetalhe.Where(x => x.SaldoContabil_id == saldoContabil_id).ToList();
        }

        public List<SaldoContabilDetalhe> ObterTodosMestre(int saldoContabil_id, ParamBase paramBase, DbControle banco = null)
        {
            int idempresa = paramBase.empresa_id;

            if (banco == null)
                banco = new DbControle();

            return banco.SaldoContabilDetalhe.Where(x => x.SaldoContabil_id == saldoContabil_id).ToList();
        }
    }
}