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
    public class DocumentoPagarProjeto
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Documento Mestre"), Required(ErrorMessage = "*")]
        public int DocumentoPagarMestre_id { get; set; }

        [JsonIgnore, ForeignKey("DocumentoPagarMestre_id")]
        public virtual DocumentoPagarMestre DocumentoPagarMestre { get; set; }

        [Display(Name = "Histórico"), MaxLength(50)]
        public String Historico { get; set; }
        
        [Display(Name = "Valor"), Required(ErrorMessage = "Valor")]
        public Decimal Valor { get; set; }

        [Display(Name = "Projeto")]
        public int? Projeto_Id { get; set; }

        [JsonIgnore, ForeignKey("Projeto_Id")]
        public virtual Projeto Projeto { get; set; }


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
                    banco.DocumentoPagarProjeto.Remove(obj);
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

        public bool Alterar(DocumentoPagarProjeto obj, DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.Id, pb);
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


        private bool ValidaExistencia(DbControle db, DocumentoPagarProjeto obj)
        {
            return (false);
        }
        public bool Incluir( ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, this))
                return false;
            else
            {
                new LogMudanca().Incluir(this, "", "",db, pb);

                db.Set<DocumentoPagarProjeto>().Add(this);
                db.SaveChanges();

                return true;
            }
        }


        public DocumentoPagarProjeto ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public DocumentoPagarProjeto ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarProjeto.Where(x => x.Id == id && x.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<DocumentoPagarProjeto> ObterPorCapa(int id, ParamBase pb)
        {
            return ObterPorCapa(id, null, pb);
        }
        public List<DocumentoPagarProjeto> ObterPorCapa(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.id == id && x.DocumentoPagarMestre.estabelecimento_id == estab).ToList();
        }

        public List<DocumentoPagarProjeto> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab).ToList();
        }

        public IQueryable<DocumentoPagarProjeto> ObterTodosIQ(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab);
        }
        public List<DocumentoPagarProjeto> ObterTodosData(DateTime dataInicial, DateTime dataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab &&
                                                       DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataLancamento)>=dataInicial &&
                                                       DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataLancamento) <= dataFinal).ToList();
        }

        public List<DocumentoPagarProjeto> ObterPorCPAG(int p,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            return banco.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre_id == p).Include(x => x.Projeto).ToList();
        }



        public List<DocumentoPagarProjeto> ObterEntreData(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb, int estab = 0)
        {
            if (estab == 0)
                estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataDocumento) >= DataInicial
                                        && DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataDocumento) <= DataFinal).
                                        Include(p=> p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes)
                                        .ToList();
        }

        public List<DocumentoPagarProjeto> ObterEntreDataComProjeto(System.DateTime DataInicial, System.DateTime DataFinal,int projetoid, ParamBase pb, int estab = 0)
        {
            if (estab == 0)
                estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataDocumento) >= DataInicial
                                        && DbFunctions.TruncateTime(x.DocumentoPagarMestre.dataDocumento) <= DataFinal
                                        && x.Projeto.id == projetoid).
                                        Include(p => p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes)
                                        .ToList();
        }



        public List<DocumentoPagarProjeto> ObterPorProjeto(ParamBase pb, int projetoid)
        {
            
            var estab = pb.estab_id;

            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && x.Projeto_Id == projetoid).
                                        Include(p => p.DocumentoPagarMestre).Include(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes).ToList();
        }

        public List<DocumentoPagarProjeto> ObterTodosEmAberto(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            //Lista quando em aberto ou pago parcial
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && (x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCEMABERTO ||
                                            x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCPAGOPARC)).ToList();
        }
        public List<DocumentoPagarProjeto> ObterTodosPagos( ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && (x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCPAGOTOTAL ||
                                        x.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCPAGOPARC)).ToList();
        }
        public DocumentoPagarProjeto ObterPorCodigoVerificacao(string id, ParamBase pb)
        {
            var estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarProjeto.Where(x => x.DocumentoPagarMestre.estabelecimento_id == estab
                                        && (x.DocumentoPagarMestre.CodigoVerificacao == id)).FirstOrDefault();
        }



    }
}
