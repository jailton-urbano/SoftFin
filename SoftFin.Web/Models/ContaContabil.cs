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
    public class ContaContabil: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "empresa"), Required(ErrorMessage = "Informe a empresa")]
        public int empresa_id { get; set; }

        [JsonIgnore, ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [MaxLength(20), Required(ErrorMessage = "Informe o código")]
        public string codigo { get; set; }

        [MaxLength(150), Required(ErrorMessage = "Informe a descrição")]
        public string descricao { get; set; }

        [MaxLength(20)]
        public string codigoSuperior { get; set; }

        [MaxLength(1)]
        public String Tipo { get; set; }

        [MaxLength(2)]
        public String CategoriaGeral { get; set; }

        [MaxLength(3)]
        public String SubCategoria { get; set; }

        [MaxLength(2)]
        public String IndicacaoPublicacao { get; set; }
        
        public bool Ativo { get; set; }


        private bool validaExistencia(DbControle db, ContaContabil obj)
        {
            return (false);
        }

        internal ContaContabil ObterPorIdCodigo(string codigo, ParamBase paramBase, DbControle db = null)
        {
            int idempresa = paramBase.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.ContaContabil.Where(x => x.codigo == codigo && x.empresa_id == idempresa).FirstOrDefault();
        }

        internal bool Incluir(ParamBase paramBase, DbControle db = null)
        {
            return Incluir(this, paramBase, db);
        }
        internal bool Incluir(ContaContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<ContaContabil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        internal bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }
        internal bool Alterar(ContaContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

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
                    db.ContaContabil.Remove(obj);
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


        public ContaContabil ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public ContaContabil ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.ContaContabil.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }



        internal List<ContaContabil> ObterTodos(ParamBase paramBase, DbControle banco = null)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();
            return banco.ContaContabil.Where(x => x.empresa_id == idempresa).ToList();
        }


    }
}