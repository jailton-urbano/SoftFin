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
    public class Sinistro
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Data Ocorrência"), Required(ErrorMessage = "*")]
        public DateTime dataOcorrencia { get; set; }

        [Display(Name = "Data Aviso")]
        public DateTime? dataAviso { get; set; }


        [Display(Name = "Data Liquidação")]
        public DateTime? dataLiquidacao { get; set; }


        [Display(Name = "Data Vistoria")]
        public DateTime? dataVistoria { get; set; }


        [Display(Name = "Número Processo"),
            Required(ErrorMessage = "*"),
            StringLength(50)]
        public string numeroProcesso { get; set; }

        [Display(Name = "Valor"), Required(ErrorMessage = "*")]
        public decimal valor { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "*"),
        StringLength(150)]
        public string descricao { get; set; }

        [Display(Name = "Histórico"),
        Required(ErrorMessage = "*"),
        StringLength(150)]
        public string historico { get; set; }



        [JsonIgnore,ForeignKey("PropostaApolice_id")]

        public virtual PropostaApolice PropostaApolice { get; set; }

        [Display(Name = "Apolice Seguro"), Required(ErrorMessage = "*")]
        public int PropostaApolice_id { get; set; }

        [StringLength(40)]
        public string statusSeguro { get; set; }

        [StringLength(40)]
        public string tipodeSinistro { get; set; }



        [NotMapped]
        public string active { get; set; }

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
                    db.Sinistro.Remove(obj);
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
        public bool Alterar(Sinistro obj, ParamBase pb)
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
        private bool validaExistencia(DbControle db, Sinistro obj)
        {
            return (db.Sinistro.Where(p => p.dataOcorrencia == obj.dataOcorrencia && p.PropostaApolice_id == obj.PropostaApolice_id).Count() > 0);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Sinistro obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Sinistro>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public Sinistro ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Sinistro ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.Sinistro.Where(x => x.id == id).FirstOrDefault();
        }
        public List<Sinistro> ObterTodos()
        {
            DbControle db = new DbControle();
            return db.Sinistro.ToList();
        }
        public List<Sinistro> ObterTodosPorIDApolice(int id)
        {
            DbControle db = new DbControle();
            return db.Sinistro.Where(p => p.PropostaApolice_id == id).ToList();
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