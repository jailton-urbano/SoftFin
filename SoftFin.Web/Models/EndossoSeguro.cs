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
    public class EndossoSeguro
    {
        public EndossoSeguro()
        {
            dataRegistro = DateTime.Now;
        }

        [Key]
        public int id { get; set; }


        [Display(Name = "Número"),
        Required(ErrorMessage = "*")]
        public int numero { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "*"),
        StringLength(35)]
        public string descricao { get; set; }

        [JsonIgnore,ForeignKey("PropostaApolice_id")]
        public virtual PropostaApolice ApoliceSeguro { get; set; }

        [Display(Name = "PropostaApolice"), Required(ErrorMessage = "*")]
        public int PropostaApolice_id { get; set; }


        [Display(Name = "Data Registro"), Required(ErrorMessage = "*")]
        public DateTime dataRegistro { get; set; }

        [Display(Name = "Data Endosso"), Required(ErrorMessage = "*")]
        public DateTime data { get; set; }

        [Display(Name = "Valor"), Required(ErrorMessage = "*")]
        public decimal valor { get; set; }


        [Display(Name = "Status"),MaxLength(70)]
        public string status { get; set; }

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
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.EndossoSeguro.Remove(obj);
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
        public bool Alterar(EndossoSeguro obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, EndossoSeguro obj)
        {
            var existe = db.EndossoSeguro.Where(p => p.numero == obj.numero && p.PropostaApolice_id == obj.PropostaApolice_id);
            var total = existe.Count();
            return (total > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(EndossoSeguro obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<EndossoSeguro>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public EndossoSeguro ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public EndossoSeguro ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.EndossoSeguro.Where(x => x.id == id).FirstOrDefault();
        }
        public List<EndossoSeguro> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.EndossoSeguro.ToList();
        }
        public List<EndossoSeguro> ObterTodosPorIDApolice(int id)
        {
            DbControle db = new DbControle();
            return db.EndossoSeguro.Where(p => p.PropostaApolice_id == id).ToList();
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

        [NotMapped]
        public int numeroParcelas { get; set; }
    }
}