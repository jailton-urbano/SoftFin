using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SoftFin.Web.Models
{
    public class DocumentoPagarDetalhe
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }



        [Display(Name = "Unidade de Negócio"),
        Required(ErrorMessage = "*")]
        public int unidadenegocio_id { get; set; }

        [Display(Name = "Documento Mestre"),
        Required(ErrorMessage = "*")]
        public int documentoPagarMestre_id { get; set; }
        
        [Display(Name = "Valor"),
        Required(ErrorMessage = "*")]
        public decimal valor { get; set; }
        [Display(Name = "Historico"),
        Required(ErrorMessage = "*"),
        StringLength(500)]
        public string historico { get; set; }

        [Display(Name = "Percentual"),
        Required(ErrorMessage = "*")]
        public double percentual { get; set; }
        


        [JsonIgnore,ForeignKey("unidadenegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [JsonIgnore,ForeignKey("documentoPagarMestre_id")]
        public virtual DocumentoPagarMestre DocumentoPagarMestre { get; set; }

        [NotMapped]
        public string UnidadeNegocio_desc { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb,  DbControle banco = null)
        {
            try
            {
                int estab = pb.estab_id;


                if (banco == null)
                    banco = new DbControle();
                var obj = ObterPorId(id, banco,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", banco,pb);
                    banco.DocumentoPagarDetalhe.Remove(obj);
                    banco.SaveChanges();
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

        public bool Alterar(DocumentoPagarDetalhe obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
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


        private bool validaExistencia(DbControle db, DocumentoPagarDetalhe obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(DocumentoPagarDetalhe obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<DocumentoPagarDetalhe>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public DocumentoPagarDetalhe ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public DocumentoPagarDetalhe ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarDetalhe.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<DocumentoPagarDetalhe> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarDetalhe.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<DocumentoPagarDetalhe> ObterTodosData(DateTime dataInicial, DateTime dataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarDetalhe.Where(x => x.estabelecimento_id == estab &&
                                                       x.DocumentoPagarMestre.dataLancamento>=dataInicial &&
                                                       x.DocumentoPagarMestre.dataLancamento<=dataFinal).ToList();
        }

        public List<DocumentoPagarDetalhe> ObterPorCPAG(int p,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            return banco.DocumentoPagarDetalhe.Where(x => x.documentoPagarMestre_id == p).ToList();
        }
    }
}
