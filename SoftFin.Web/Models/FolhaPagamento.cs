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
    public class FolhaPagamento: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Data de Pagamento")]
        public DateTime DataPagamento { get; set; }

        [Display(Name = "Data Base"), MaxLength(1000)]
        public string dataBase { get; set; }

        [Display(Name = "Valor")]
        public decimal valor { get; set; }

        [Display(Name = "Unidade de Negócio"), Required(ErrorMessage = "Informe a Unidade de Negócio")]
        public int unidadenegocio_id { get; set; }
        [JsonIgnore,ForeignKey("unidadenegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [Display(Name = "Tipo de Folha de Pagamento"), Required(ErrorMessage = "Informe a Folha de Pagamento")]
        public int folhapagamentotipo_id { get; set; }

        [JsonIgnore,ForeignKey("folhapagamentotipo_id")]
        public virtual FolhaPagamentoTipo FolhaPagamentoTipo { get; set; }

        [Display(Name = "Funcionário"), Required(ErrorMessage = "Informe um Funcionário")]
        public int funcionario_id { get; set; }
        [JsonIgnore,ForeignKey("funcionario_id")]
        public virtual Funcionario Funcionario { get; set; }
        
        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o Estabelecimento")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        private bool validaExistencia(DbControle db, FolhaPagamento obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(FolhaPagamento obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<FolhaPagamento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(FolhaPagamento obj, ParamBase paramBase)
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
                    db.FolhaPagamento.Remove(obj);
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


        public FolhaPagamento ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public FolhaPagamento ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.FolhaPagamento.Where(x => x.id == id && x.Funcionario.Pessoa.empresa_id == idempresa).FirstOrDefault();
        }

        public List<FolhaPagamento> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            var banco = new DbControle();

            return banco.FolhaPagamento.Where(x => x.Funcionario.Pessoa.empresa_id == idempresa).ToList();
        }
    }
}   