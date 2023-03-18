using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class Recebimento: BaseModels
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o estabelecimento")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Data Recebimento"), Required(ErrorMessage = "Informe a data de recebimento")]
        public DateTime dataRecebimento { get; set; }
        [Display(Name = "Valor Recebimento"), Required(ErrorMessage = "Informe o valor do recebimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorRecebimento { get; set; }
        [Display(Name = "Historico"), Required(ErrorMessage = "Informe o histórico"), MaxLength(500)]               
        public string historico { get; set; }
        [Display(Name = "Nota Fiscal"), Required(ErrorMessage = "Informe a nota Fiscal")]
        public int notaFiscal_ID { get; set; }
        [Display(Name = "Banco"), Required(ErrorMessage = "Infore o banco")]
        public int banco_ID { get; set; }

        [JsonIgnore,ForeignKey("banco_ID")]
        public virtual Banco banco { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_ID")]
        public virtual NotaFiscal notaFiscal { get; set; }

        [Display(Name = "Tipo Documento")]
        public int? tipodocumento_id { get; set; }

        [JsonIgnore,ForeignKey("tipodocumento_id")]
        public virtual TipoDocumento TipoDocumento { get; set; }

        //[Display(Name = "Banco Movimento")]
        //public int? bancoMovimento_id { get; set; }
        //[JsonIgnore,ForeignKey("bancoMovimento_id")]
        //public virtual BancoMovimento BancoMovimento { get; set; }



        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db ==null) 
                    db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Recebimento.Remove(obj);
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

        public bool Alterar(Recebimento obj, ParamBase pb, DbControle db=null)
        {
            if (db == null)
                db = new DbControle();

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


        private bool validaExistencia(DbControle db, Recebimento obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(Recebimento obj, ParamBase pb, DbControle db= null)
        {
            if (db == null)
             db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Recebimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Recebimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Recebimento ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Recebimento.Where(x => x.id == id && x.estabelecimento_id == estab).Include(p => p.notaFiscal).FirstOrDefault();
        }
        public List<Recebimento> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Recebimento.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<Recebimento> ObterEntreData(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Recebimento.Where(x => x.estabelecimento_id == estab
                                        && x.dataRecebimento >= DataInicial
                                        && x.dataRecebimento <= DataFinal).ToList();
        }

        public Recebimento ObterPorNFSeId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Recebimento.Where(x => x.notaFiscal_ID == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<Recebimento> ObterTodosPorNFSeId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Recebimento.Where(x => x.notaFiscal_ID == id && x.estabelecimento_id == estab).ToList();
        }

        public Decimal ObterValorRecebidoNota(int notaID, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var valorRecebimento = db.Recebimento.Where(x => x.estabelecimento_id == estab && x.notaFiscal_ID == notaID);
            if (valorRecebimento.Count() == 0)
                return 0;
            return valorRecebimento.Sum(x => x.valorRecebimento);
        }
 
    }
}