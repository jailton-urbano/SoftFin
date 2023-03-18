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
    public class FuncionarioSalario: BaseModels
    {
        [Key]
        public int id { get; set; }

        [JsonIgnore,ForeignKey("funcionario_id")]
        public virtual Funcionario Funcionario { get; set; }

        [Display(Name = "Funcionario"), Required(ErrorMessage = "*Informe o usuário")]
        public int funcionario_id { get; set; }

        [Display(Name = "Data de Inicio")]
        public DateTime? dataInicial { get; set; }

        [Display(Name = "Valor Piso Salarial")]
        public decimal valorBruto { get; set; }

        [Display(Name = "Valor do Adiantamento")]
        public decimal valorAdiantamento { get; set; }

        [Display(Name = "Valor do Complemento")]
        public decimal valorComplemento { get; set; }


        public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(this.id, ref erro,pb);
        }
        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.FuncionarioSalario.Remove(obj);
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
        public bool Alterar(FuncionarioSalario obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, FuncionarioSalario obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(FuncionarioSalario obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<FuncionarioSalario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public FuncionarioSalario ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public FuncionarioSalario ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.FuncionarioSalario.Where(x => x.id == id && x.Funcionario.Pessoa.empresa_id == idempresa).FirstOrDefault();
        }


        public List<FuncionarioSalario> ObterTodos(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            var banco = new DbControle();

            return banco.FuncionarioSalario.Where(x => x.Funcionario.Pessoa.empresa_id == idempresa).ToList();
        }
    }
}