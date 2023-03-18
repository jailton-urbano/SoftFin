using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class ComissaoSeguro
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string descricao { get; set; }


        [NotMapped]
        public string SeguradoraDescricao { get; set; }

        [JsonIgnore,ForeignKey("Seguradora_id")]
        public virtual Pessoa Seguradora { get; set; }

        [Display(Name = "Seguradora"), Required(ErrorMessage = "*")]
        public int Seguradora_id { get; set; }


        [JsonIgnore,ForeignKey("ProdutoSeguradora_id")]
        public virtual ProdutoSeguradora ProdutoSeguradora { get; set; }

        [Display(Name = "ProdutoSeguradora"), Required(ErrorMessage = "*")]
        public int ProdutoSeguradora_id { get; set; }

        [Display(Name = "Data Inclusão"), Required(ErrorMessage = "*")]
        public DateTime dataInclusao { get; set; }

        [Display(Name = "Data Alteração"), Required(ErrorMessage = "*")]
        public DateTime dataAlteracao  { get; set; }

        [Display(Name = "Percentual Seguradora"), Required(ErrorMessage = "*")]
        public decimal PercentualSeguradora { get; set; }

        [Display(Name = "Percentual Vendedor"), Required(ErrorMessage = "*")]
        public decimal PercentualVendedor { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.ComissaoSeguro.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
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
        public bool Alterar(ComissaoSeguro obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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
        private bool validaExistencia(DbControle db, ComissaoSeguro obj)
        {
            return (db.ComissaoSeguro.Where(p => p.Seguradora_id == obj.Seguradora_id 
                    && p.ProdutoSeguradora_id == obj.ProdutoSeguradora_id ).Count() > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(ComissaoSeguro obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<ComissaoSeguro>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public ComissaoSeguro ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public ComissaoSeguro ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.ComissaoSeguro.Where(x => x.id == id).FirstOrDefault();
        }
        public List<ComissaoSeguro> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.ComissaoSeguro.ToList();
        }
        public List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState)
        {
            var erros = new List<string>();
            if (ModelState != null)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                erros.AddRange(from a in allErrors select a.ErrorMessage);
            }
            return erros;
        }
    }

    public class ComissaoSeguroVM
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string descricao { get; set; }

        [NotMapped]
        public string SeguradoraDescricao { get; set; }

        [Display(Name = "Seguradora"), Required(ErrorMessage = "*")]
        public int Seguradora_id { get; set; }

        [Display(Name = "ProdutoSeguradora"), Required(ErrorMessage = "*")]
        public int ProdutoSeguradora_id { get; set; }

        [Display(Name = "Data Inclusão"), Required(ErrorMessage = "*")]
        public string dataInclusao { get; set; }

        [Display(Name = "Data Alteração"), Required(ErrorMessage = "*")]
        public string dataAlteracao { get; set; }

        [Display(Name = "Percentual Seguradora"), Required(ErrorMessage = "*")]
        public decimal PercentualSeguradora { get; set; }

        [Display(Name = "Percentual Vendedor"), Required(ErrorMessage = "*")]
        public decimal PercentualVendedor { get; set; }
    }

}