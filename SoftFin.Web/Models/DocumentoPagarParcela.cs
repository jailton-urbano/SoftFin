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
    public class DocumentoPagarParcela
    {
        [Key]
        public int id { get; set; }
       

        [Display(Name = "Documento Mestre"), Required(ErrorMessage = "*")]
        public int DocumentoPagarMestre_id { get; set; }


        [Display(Name = "Parcela"), Required(ErrorMessage = "Parcela")]
        public int parcela { get; set; }

        [Display(Name = "Vencimento"), Required(ErrorMessage = "Vencimento")]
        public DateTime vencimento { get; set; }

        [Display(Name = "Vencimento Real"), Required(ErrorMessage = "Vencimento Real")]
        public DateTime vencimentoPrevisto { get; set; }

        [Display(Name = "Vencimento Real"), Required(ErrorMessage = "Vencimento Real")]
        public Decimal valor { get; set; }

        [Display(Name = "Histórico"), MaxLength(500),Required(ErrorMessage = "Informe o histórico")]
        public String historico { get; set; }

        [JsonIgnore,ForeignKey("DocumentoPagarMestre_id")]
        public virtual DocumentoPagarMestre DocumentoPagarMestre { get; set; }

        [Display(Name = "Usuário Autorizador:")]
        public int? usuarioAutorizador_id { get; set; }

        [JsonIgnore,ForeignKey("usuarioAutorizador_id")]
        public virtual Usuario usuarioAutorizador { get; set; }

        //[Display(Name = "Codigo Pagamento")]
        //public int? codigoPagamento { get; set; }

        [Display(Name = "Lote Pagamento Banco"), StringLength(15)]
        public string lotePagamentoBanco { get; set; }


        //public string CodigoVerificacao { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb,DbControle banco = null)
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
                    new LogMudanca().Incluir(obj, "", "", banco, pb);
                    banco.DocumentoPagarParcela.Remove(obj);
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

        public bool Alterar(DocumentoPagarParcela obj, DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, DocumentoPagarParcela obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(DocumentoPagarParcela obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<DocumentoPagarParcela>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public DocumentoPagarParcela ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public DocumentoPagarParcela ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarParcela.Where(x => x.id == id && x.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<DocumentoPagarParcela> ObterPorCapa(int id, ParamBase pb)
        {
            return ObterPorCapa(id, null, pb);
        }
        public List<DocumentoPagarParcela> ObterPorCapa(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.id == id && x.DocumentoPagarMestre.estabelecimento_id == estab).ToList();
        }

        public List<DocumentoPagarParcela> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab).ToList();
        }

        public IQueryable<DocumentoPagarParcela> ObterTodosIQ(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab);
        }
        public List<DocumentoPagarParcela> ObterTodosData(DateTime dataInicial, DateTime dataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab &&
                                                       DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataLancamento)>=dataInicial &&
                                                       DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataLancamento) <= dataFinal).ToList();
        }

        public List<DocumentoPagarParcela> ObterPorCPAG(int p,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            return banco.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre_id == p).ToList();
        }



        public List<DocumentoPagarParcela> ObterEntreData(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb, int estab = 0)
        {
            if (estab == 0)
                estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataDocumento) >= DataInicial
                                        && DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataDocumento) <= DataFinal).
                                        Include(p=> p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes)
                                        .ToList();
        }
        public List<DocumentoPagarParcela> ObterEntreDataVencimento(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb, int estab = 0)
        {
            if (estab == 0)
                estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.vencimentoPrevisto) >= DataInicial
                                        && DbFunctions.TruncateTime(x.vencimentoPrevisto) <= DataFinal).
                                        Include(p => p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes).ToList();
        }


        public List<DocumentoPagarParcela> ObterEntreDataVencimentoComProjeto(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb, int estab = 0)
        {
            if (estab == 0)
                estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.vencimentoPrevisto) >= DataInicial
                                        && DbFunctions.TruncateTime(x.vencimentoPrevisto) <= DataFinal
                                        && x.DocumentoPagarMestre.DocumentoPagarProjetos.Count() >= 0).
                                        Include(p => p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes).ToList();
        }


        public List<DocumentoPagarParcela> ObterPorProjeto(ParamBase pb, int projetoid)
        {
            
            var estab = pb.estab_id;

            DbControle db = new DbControle();
            var aux =  db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && x.DocumentoPagarMestre.DocumentoPagarProjetos.Where( p => p.Projeto_Id == projetoid && p.DocumentoPagarMestre_id == x.DocumentoPagarMestre_id).Count() > 0).
                                        Include(p => p.DocumentoPagarMestre).
                                        Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes).
                                        Include(p => p.DocumentoPagarMestre.DocumentoPagarParcelas);
            return aux.ToList();
        }
        public List<DocumentoPagarParcela> ObterEntreDataVencimentoOriginal(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb, int estab = 0)
        {
            if (estab == 0)
                estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.vencimento) >= DataInicial
                                        && DbFunctions.TruncateTime(x.vencimento) <= DataFinal).
                                        Include(p => p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes).ToList();
        }
        public List<DocumentoPagarParcela> ObterTodosEmail(int estab)
        {
            DbControle db = new DbControle();
            var dataInicial = DateTime.Now.AddDays(-1);
            var dataFinal = DateTime.Now.AddDays(7);
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                            && DbFunctions.TruncateTime(x.vencimentoPrevisto) >= dataInicial
                            && DbFunctions.TruncateTime(x.vencimentoPrevisto) <= dataFinal).ToList();
        }
        public List<DocumentoPagarParcela> ObterTodosNaoAutorizado(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {

            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                            && DbFunctions.TruncateTime(x.vencimentoPrevisto) >= DataInicial
                            && DbFunctions.TruncateTime(x.vencimentoPrevisto) <= DataFinal
                            && x.usuarioAutorizador_id == null).ToList();
        }
        public List<DocumentoPagarParcela> ObterTodosAutorizado(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                            && DbFunctions.TruncateTime(x.vencimentoPrevisto) >= DataInicial
                            && DbFunctions.TruncateTime(x.vencimentoPrevisto) <= DataFinal
                            && x.usuarioAutorizador_id != null
                            && x.DocumentoPagarMestre.tipoDocumento.descricao.ToUpper().Contains("CNAB")).ToList();
        }
        public List<DocumentoPagarParcela> ObterTodosEmAberto(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            //Lista quando em aberto ou pago parcial
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && (x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCEMABERTO ||
                                            x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCPAGOPARC)).ToList();
        }
        public List<DocumentoPagarParcela> ObterTodosPagos( ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && (x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCPAGOTOTAL ||
                                        x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCPAGOPARC)).ToList();
        }
        public DocumentoPagarParcela ObterPorCodigoVerificacao(string id, ParamBase pb)
        {
            var estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && (x.DocumentoPagarMestre.CodigoVerificacao == id)).FirstOrDefault();
        }
        public List<DocumentoPagarParcela> ObterTodosPorVencimentoBanco(DateTime dataInicial, DateTime dataFinal, int idbanco, ParamBase pb)
        {
            var estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarParcela.Where(x => x.vencimentoPrevisto >= dataInicial
                                        && DbFunctions.TruncateTime(x.vencimentoPrevisto) <= dataFinal
                                        && x.DocumentoPagarMestre.banco_id == idbanco
                                        && x.DocumentoPagarMestre.estabelecimento_id == estab).ToList();
        }

        public int? statusPagamento { get; set; }


    }
}
