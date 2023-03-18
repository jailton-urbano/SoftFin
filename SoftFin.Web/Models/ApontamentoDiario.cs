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
    public class ApontamentoDiario: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Atividade"), Required(ErrorMessage = "*")]
        public int atividade_id { get; set; }

        [JsonIgnore,ForeignKey("atividade_id")]
        public virtual Atividade Atividade { get; set; }


        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*")]
        public DateTime data { get; set; }

        [Display(Name = "Horas Trabalhadas"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal qtdHoras { get; set; }

        [Display(Name = "Horas Restantes"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal qtdHorasRestantes { get; set; }

        [Display(Name = "Histórico"), Required(ErrorMessage = "*"), MaxLength(400)]
        public string historico { get; set; }
        
        [Display(Name = "Aprovador")]
        public int? aprovador_id { get; set; }

        [JsonIgnore,ForeignKey("aprovador_id")]
        public virtual Usuario aprovador { get; set; }

        [Display(Name = "Data Aprovação")]
        public DateTime? dataAprovado { get; set; }

        [Display(Name = "Descrição Aprovação"),MaxLength(250)]
        public string DescricaoAprovacao { get; set; }

        [Display(Name = "Situacao Aprovação"), MaxLength(10)]
        public string SituacaoAprovacao { get; set; }

        [Display(Name = "apontador")]
        public int? apontador_id { get; set; }

        [JsonIgnore,ForeignKey("apontador_id")]
        public virtual Usuario apontador { get; set; }

        public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(this.id, ref erro, pb);
        }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.ApontamentoDiario.Remove(obj);
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

        public bool Alterar(ParamBase pb,DbControle db = null)
        {
            return Alterar(this, pb, db);
        }

        public bool Alterar(ApontamentoDiario obj, ParamBase pb,DbControle db = null)
        {
            if (db == null)
                db = new DbControle();


            var objAux = ObterPorId(obj.id,pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                db.Entry(obj).State = EntityState.Modified;
                //obj.apontador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, ApontamentoDiario obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(ApontamentoDiario obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<ApontamentoDiario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public ApontamentoDiario ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public ApontamentoDiario ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ApontamentoDiario.Where(x => x.id == id && x.Atividade.estabelecimento_id == estab).Include(p => p.Atividade.Projeto).FirstOrDefault();
        }

        public List<ApontamentoDiario> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ApontamentoDiario.Where(x => x.Atividade.estabelecimento_id == estab).ToList();
        }

        public List<ApontamentoDiario> ObterTodosUsuario(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ApontamentoDiario.Where(x => x.Atividade.estabelecimento_id == estab
                                                && x.apontador_id == id).ToList();
        }

        public List<ApontamentoDiario> ObterTodosDataUsuario(DateTime dataInicial, DateTime dataFinal, int usuario, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ApontamentoDiario.Where(x => x.Atividade.estabelecimento_id == estab 
                                                        && x.data >= DbFunctions.TruncateTime(dataInicial) 
                                                        && x.data <= DbFunctions.TruncateTime(dataFinal)
                                                        && x.apontador_id == usuario).Include(p => p.Atividade).ToList();
        }

        public Decimal ObterTotalHorasProjeto(int projeto, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();

            if (db.ApontamentoDiario.Where(x => x.Atividade.estabelecimento_id == estab
                                                        && x.Atividade.Projeto.id == projeto).Count() != 0)
            {
            return db.ApontamentoDiario.Where(x => x.Atividade.estabelecimento_id == estab
                                                        && x.Atividade.Projeto.id == projeto).Sum(x=>x.qtdHoras);
            }

            else
            {
                return 0;
            }
        }

        public List<ApontamentoDiario> ObterTodosProjeto(int projeto, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ApontamentoDiario.Where(x => x.Atividade.estabelecimento_id == estab
                                                        && x.Atividade.Projeto.id == projeto).Include(p => p.Atividade).ToList();
        }

    }
    }