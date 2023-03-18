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
    public class ParcelaSeguro
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Codigo"),
        Required(ErrorMessage = "*")]
        public int numero { get; set; }

        [Display(Name = "Histórico"),
        Required(ErrorMessage = "*"),
        StringLength(35)]
        public string historico { get; set; }

        [JsonIgnore,ForeignKey("PropostaApolice_id")]

        public virtual PropostaApolice PropostaApolice { get; set; }

        [Display(Name = "Apolice Seguro"), Required(ErrorMessage = "*")]
        public int PropostaApolice_id { get; set; }

        [Display(Name = "Data Original de Vencto"), Required(ErrorMessage = "*")]
        public DateTime dataOriginalVencto { get; set; }

        [Display(Name = "Data Pagto")]
        public DateTime? dataPagto { get; set; }

        [Display(Name = "Data Conciliado")]
        public DateTime? dataConciliado { get; set; }

        [Display(Name = "Valor a Pagar")]
        public decimal? valorPagar { get; set; }

        [Display(Name = "Valor a Receber")]
        public decimal? valorReceber { get; set; }

        [Display(Name = "Valor Parcela Representante")]
        public decimal? valorParcelaRepresentante { get; set; }

        [NotMapped]
        public int numeroParcelas { get; set; }

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
                    db.ParcelaSeguro.Remove(obj);
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
        public bool Alterar(ParcelaSeguro obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, ParcelaSeguro obj)
        {
            return (db.ParcelaSeguro.Where(p => p.numero == obj.numero && p.PropostaApolice_id == obj.PropostaApolice_id).Count() > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(ParcelaSeguro obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<ParcelaSeguro>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public ParcelaSeguro ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public ParcelaSeguro ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.ParcelaSeguro.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<ParcelaSeguro> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.ParcelaSeguro.ToList();
        }
        public List<ParcelaSeguro> ObterTodosPorIDApolice(int id)
        {
            DbControle db = new DbControle();
            return db.ParcelaSeguro.Where(p => p.PropostaApolice_id == id).ToList();
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
}