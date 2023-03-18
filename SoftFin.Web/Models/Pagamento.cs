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
    public class Pagamento
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Data Pagamento"), Required(ErrorMessage = "*")]
        public DateTime dataPagamento { get; set; }

        [Display(Name = "Valor Pagamento"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorPagamento { get; set; }

        [Display(Name = "Historico"), Required(ErrorMessage = "*")]
        public string historico { get; set; }

        
        [Display(Name = "Banco"), Required(ErrorMessage = "*")]
        public int banco_ID { get; set; }

        [JsonIgnore,ForeignKey("banco_ID")]
        public virtual Banco banco { get; set; }


        //[Display(Name = "Contas a Pagar"), Required(ErrorMessage = "*")]
        //public int documentoPagarMestre_ID { get; set; }

        //[JsonIgnore,ForeignKey("documentoPagarMestre_ID")]
        //public virtual DocumentoPagarMestre documentoPagarMestre { get; set; }



        [Display(Name = "Parcela"), Required]
        public int DocumentoPagarParcela_ID { get; set; }

        [JsonIgnore,ForeignKey("DocumentoPagarParcela_ID")]
        public virtual DocumentoPagarParcela DocumentoPagarParcela { get; set; }


        public Decimal? multa { get; set; }

        public Decimal? juros { get; set; }

        public int? contaContabilCredito_id { get; set; }

        [JsonIgnore, ForeignKey("contaContabilCredito_id")]
        public virtual ContaContabil ContaContabilCredito { get; set; }

        public Decimal ValorRefinanciado { get; set; }

        public Decimal PorcentagemJurosRefinanciamento { get; set; }

        public Decimal ValorJurosRefinanciamento { get; set; }


        public Decimal ValorRefinanciadoAnterior { get; set; }

        public Decimal PorcentagemJurosRefinanciamentoAnterior { get; set; }

        public Decimal ValorJurosRefinanciamentoAnterior { get; set; }

        public bool FlagCartao { get; set; }
        

        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    
                    db.Pagamento.Remove(obj);
                    db.SaveChanges();
                    new LogMudanca().Incluir(obj, "", "", db, pb);
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

        public bool Alterar(Pagamento obj, ParamBase pb, DbControle db = null)
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


        private bool validaExistencia(DbControle db, Pagamento obj, ParamBase pb)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(Pagamento obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Pagamento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Pagamento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Pagamento ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Pagamento.Where(x => x.id == id && x.estabelecimento_id == estab).Include(p => p.DocumentoPagarParcela).FirstOrDefault();
        }
        public List<Pagamento> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Pagamento.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<Pagamento> ObterEntreData(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Pagamento.Where(x => x.estabelecimento_id == estab
                                        && x.dataPagamento >= DataInicial
                                        && x.dataPagamento <= DataFinal).ToList();
        }

        public Pagamento ObterPorDocumentoPagarMestreId(int id,  ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Pagamento.Where(x => x.DocumentoPagarParcela.DocumentoPagarMestre_id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<Pagamento> ObterTodosPorDocumentoPagarParcelaId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Pagamento.Where(x => x.DocumentoPagarParcela.id == id && x.estabelecimento_id == estab).ToList();
        }

        public Decimal ObterValorPagoDocumento(int documentoID, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var valorPagamento = db.Pagamento.Where(x => x.estabelecimento_id == estab && x.DocumentoPagarParcela_ID == documentoID);
            if (valorPagamento.Count() == 0)
                return 0;
            return valorPagamento.Sum(x => x.valorPagamento);
        }

    }
}