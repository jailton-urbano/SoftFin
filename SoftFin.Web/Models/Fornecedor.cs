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
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace SoftFin.Web.Models
{
    public class Fornecedor: BaseModels
    {
        [Key]
        public int id { get; set; }

        [JsonIgnore,ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "Informe a Pessoa")]
        public int pessoa_id { get; set; }

        [JsonIgnore,ForeignKey("responsavel_id")]
        public virtual Pessoa Responsavel { get; set; }

        [Display(Name = "Responsável"), Required(ErrorMessage = "Informe o Responsável")]
        public int responsavel_id { get; set; }

        [Display(Name = "Data de Contratação")]
        public DateTime? dataContratada { get; set; }

        [Display(Name = "Unidade de negócio")]
        public int? unidadeNegocio_id { get; set; }

        [JsonIgnore,ForeignKey("unidadeNegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [Display(Name = "Data de Saida")]
        public DateTime? dataSaida { get; set; }

        [Display(Name = "Nome Empresa"), MaxLength(50)]
        public string NomeEmpresa { get; set; }


        private bool validaExistencia(DbControle db, Fornecedor obj, ParamBase paramBase)
        {

            return (ObterPorPessoa_id(obj.pessoa_id, paramBase) != null);
        }

        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(Fornecedor obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj, paramBase))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<Fornecedor>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(Fornecedor obj, ParamBase paramBase)
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
                    db.Fornecedor.Remove(obj);
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



        public Fornecedor ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public Fornecedor ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Fornecedor.Where(x => x.id == id && x.Pessoa.empresa_id == idempresa).FirstOrDefault();
        }


        public List<Fornecedor> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            var banco = new DbControle();

            return banco.Fornecedor.Where(x => x.Pessoa.empresa_id == idempresa).ToList();
        }


        public Fornecedor ObterPorPessoa_id(int id, ParamBase paramBase)
        {
            return ObterPorPessoa_id(id, null, paramBase);
        }
        public Fornecedor ObterPorPessoa_id(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Fornecedor.Where(x => x.pessoa_id == id && x.Pessoa.empresa_id== idempresa).FirstOrDefault();
        }

    }
}