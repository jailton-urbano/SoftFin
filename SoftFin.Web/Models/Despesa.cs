using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class Despesa: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Colaborador"), Required(ErrorMessage = "*")]
        public int colaborador_id { get; set; }
        [JsonIgnore,ForeignKey("colaborador_id")]
        public virtual Usuario Colaborador { get; set; }


        [Display(Name = "Cliente"), Required(ErrorMessage = "*")]
        public int cliente_id { get; set; }
        [JsonIgnore,ForeignKey("cliente_id")]
        public virtual Pessoa Cliente { get; set; }

        [Display(Name = "Projeto"), Required(ErrorMessage = "*")]
        public int projeto_id { get; set; }
        [JsonIgnore,ForeignKey("projeto_id")]
        public virtual Projeto Projeto { get; set; }

        [Display(Name = "Tipo Despesa"), Required(ErrorMessage = "*")]
        public int tipoDespesa_id { get; set; }
        [JsonIgnore,ForeignKey("tipoDespesa_id")]
        public virtual TipoDespesa TipoDespesa { get; set; }

        [DisplayName("Data"),
        Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        
        [Display(Name = "Valor"),
        Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valor { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "*"),MaxLength(30)]
        public string descricao { get; set; }

        [Display(Name = "Aprovador")]
        public int? aprovador_id { get; set; }
        [JsonIgnore,ForeignKey("aprovador_id")]
        public virtual Usuario Aprovador { get; set; }

        [Display(Name = "Data Aprovação")]
        public DateTime? dataAprovado { get; set; }

        [Display(Name = "Descrição Aprovação"), MaxLength(250)]
        public string DescricaoAprovacao { get; set; }

        [Display(Name = "Situacao Aprovação"), MaxLength(10)]
        public string SituacaoAprovacao { get; set; }

        [Display(Name = "Lote Cobrança")]
        public int? loteCobranca_id { get; set; }
        [JsonIgnore,ForeignKey("loteCobranca_id")]
        public virtual LoteDespesa LoteCobranca { get; set; }

        [Display(Name = "Lote Reembolso")]
        public int? loteReembolso_id { get; set; }
        [JsonIgnore,ForeignKey("loteReembolso_id")]
        public virtual LoteDespesa LoteReembolsoDespesa { get; set; }

        [Display(Name = "Lote Adiantamento")]
        public int? loteAdiantamento_id { get; set; }
        [JsonIgnore,ForeignKey("loteAdiantamento_id")]
        public virtual LoteDespesa LoteAdiantamento { get; set; }


        public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(this.id,ref erro,pb);
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
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Despesa.Remove(obj);
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

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb,db);
        }
        public bool Alterar(Despesa obj, ParamBase pb, DbControle db = null)
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


        private bool validaExistencia(DbControle db, Despesa obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(Despesa obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Despesa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Despesa ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Despesa ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Despesa.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<Despesa> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle Despesa = new DbControle();
            return Despesa.Despesa.Where(x => x.estabelecimento_id == estab).Include(p => p.LoteCobranca).Include(p => p.LoteReembolsoDespesa).Include(p => p.LoteAdiantamento).ToList();
        }

        public List<Despesa> ObterTodosDataUsuario(DateTime dataInicial, DateTime dataFinal, int usuario, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Despesa.Where(x => x.estabelecimento_id == estab
                                                        && x.Data >= dataInicial
                                                        && x.Data <= dataFinal
                                                        && x.colaborador_id == usuario).Include(p => p.TipoDespesa).Include(q=> q.Projeto).ToList();
        }

        public List<Despesa> ObterTodosData(DateTime dataInicial, DateTime dataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Despesa.Where(x => x.estabelecimento_id == estab
                                                        && x.Data >= dataInicial
                                                        && x.Data <= dataFinal).Include(p => p.TipoDespesa).Include(q => q.Projeto).ToList();
        }

    }
}