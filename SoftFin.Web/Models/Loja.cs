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
    public class Loja : BaseModels
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Informe o código da Loja"), MaxLength(25)]
        public string codigo { get; set; }

        [Required(ErrorMessage = "Informe o código da Loja"), MaxLength(50)]
        public string descricao { get; set; }

        [Required(ErrorMessage = "ativo")]
        public bool ativo { get; set; }
        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        
        public virtual Estabelecimento Estabelecimento { get; set; }

        private bool validaExistencia(DbControle db, Loja obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(Loja obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",null,paramBase);

                db.Set<Loja>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase,db);
        }
        public bool Alterar(Loja obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, db,paramBase);
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
                    db.Loja.Remove(obj);
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


        public Loja ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public Loja ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Loja.Where(x => x.id == id && x.estabelecimento_id == idestab).FirstOrDefault();
        }

        public List<Loja> ObterTodos(ParamBase paramBase, DbControle banco = null)
        {
            int idestab = paramBase.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Loja.Where(x => x.estabelecimento_id == idestab).ToList();
        }
        
    }
}