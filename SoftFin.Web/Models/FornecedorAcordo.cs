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
    public class FornecedorAcordo: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [Display(Name = "Descrição"), MaxLength(1000)]
        public string descricao { get; set; }

        [Display(Name = "Valor")]
        public decimal valor { get; set; }

        [Display(Name = "Fornecedor"), Required(ErrorMessage = "Informe o fornecedor")]
        public int fornecedor_id { get; set; }

        [JsonIgnore,ForeignKey("fornecedor_id")]
        public virtual Fornecedor Fornecedor { get; set; }
                
        private bool validaExistencia(DbControle db, FornecedorAcordo obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(FornecedorAcordo obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",null,paramBase);

                db.Set<FornecedorAcordo>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(FornecedorAcordo obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, paramBase);
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

        public bool Excluir(ref string erro, ParamBase paramBase)
        {
            return Excluir(this.id, ref erro, paramBase);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase)
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
                    db.FornecedorAcordo.Remove(obj);
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
        
        
        public FornecedorAcordo ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public FornecedorAcordo ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.FornecedorAcordo.Where(x => x.id == id && x.Fornecedor.Pessoa.empresa_id == idempresa).FirstOrDefault();
        }

        public List<FornecedorAcordo> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            var banco = new DbControle();

            return banco.FornecedorAcordo.Where(x => x.Fornecedor.Pessoa.empresa_id == idempresa).ToList();
        }
    }
}