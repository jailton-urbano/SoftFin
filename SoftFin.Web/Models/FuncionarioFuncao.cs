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
    public class FuncionarioFuncao: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Nome"), MaxLength(50)]
        public string nome { get; set; }

        [Display(Name = "Descrição"), MaxLength(1000)]
        public string descricao { get; set; }

        [Display(Name = "Valor Piso Salarial")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valorpiso { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

                public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(this.id, ref erro, pb);
        }
        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.FuncionarioFuncao.Remove(obj);
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
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(FuncionarioFuncao obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, FuncionarioFuncao obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(FuncionarioFuncao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<FuncionarioFuncao>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public FuncionarioFuncao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public FuncionarioFuncao ObterPorId(int id, DbControle banco,ParamBase pb)
        {
            
            if (banco == null)
                banco = new DbControle();

            return banco.FuncionarioFuncao.Where(x => x.id == id && x.estabelecimento_id == pb.estab_id).FirstOrDefault();
        }

        public List<FuncionarioFuncao> ObterTodos(ParamBase pb)
        {
            int idempresa = pb.estab_id;
            var banco = new DbControle();

            return banco.FuncionarioFuncao.Where(x => x.estabelecimento_id == idempresa).ToList();
        }
    }
}