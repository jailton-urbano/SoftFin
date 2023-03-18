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
    public class DespesaPermitida: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Projeto"), Required(ErrorMessage = "*")]
        public int projeto_id { get; set; }
        [JsonIgnore,ForeignKey("projeto_id")]
        public virtual Projeto Projeto { get; set; }

        [Display(Name = "Tipo Despesa"), Required(ErrorMessage = "*")]
        public int tipodespesa_id { get; set; }
        [JsonIgnore,ForeignKey("tipodespesa_id")]
        public virtual TipoDespesa TipoDespesa { get; set; }


        [Display(Name = "Aprovador"), Required(ErrorMessage = "*")]
        public int aprovador_id { get; set; }
        [JsonIgnore,ForeignKey("aprovador_id")]
        public virtual Usuario Aprovador { get; set; }

        [Display(Name = "Valor Limite"),
        Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valorLimite { get; set; }

        [Display(Name = "Cobrável ao Cliente"),
        Required(ErrorMessage = "*")]
        public bool cobravel { get; set; }

        [Display(Name = "Reembolsável ao Consultor"),
        Required(ErrorMessage = "*")]
        public bool reembolsavel { get; set; }

        [Display(Name = "Usar Padrão"),
        Required(ErrorMessage = "*")]
        public bool usarpadrao { get; set; }


        [Display(Name = "Descrição Padrão"),
        Required(ErrorMessage = "*"), MaxLength(30)]
        public string descricao { get; set; }

        [Display(Name = "Valor Padrão"),
        Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valorPadrao { get; set; }

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
                    db.DespesaPermitida.Remove(obj);
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
            return Alterar(this, pb, db);
        }
        public bool Alterar(DespesaPermitida obj, ParamBase pb, DbControle db = null)
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
        private bool validaExistencia(DbControle db, DespesaPermitida obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(DespesaPermitida obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<DespesaPermitida>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public DespesaPermitida ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public DespesaPermitida ObterPorId(int id, DbControle DetalheDespesa, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (DetalheDespesa == null)
                DetalheDespesa = new DbControle();

            return DetalheDespesa.DespesaPermitida.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public DespesaPermitida ObterTodosPorProjeto(int projeto, DbControle DespesaPermitida, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (DespesaPermitida == null)
                DespesaPermitida = new DbControle();

            return DespesaPermitida.DespesaPermitida.Where(x => x.projeto_id == projeto && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<DespesaPermitida> ObterTodos( ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle DespesaPermitida = new DbControle();
            return DespesaPermitida.DespesaPermitida.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<DespesaPermitida> ObterPorProjeto(int projeto, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle DespesaPermitida = new DbControle();
            return DespesaPermitida.DespesaPermitida.Where(x => x.estabelecimento_id == estab && x.projeto_id == projeto).ToList();
        }

        public bool ValidaDespesaReembolsavel(int idProjeto, int idTipo, ParamBase pb)
        {
            int estab = pb.estab_id;
            var obj = new DespesaPermitida().ObterTodos(pb).Where(x=> x.tipodespesa_id == idTipo && x.projeto_id == idProjeto).ToList().FirstOrDefault();
            if (obj != null)
            {
                if (obj.reembolsavel == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ValidaDespesaCobravel(int idProjeto, int idTipo, ParamBase pb)
        {
            int estab = pb.estab_id;
            var obj = new DespesaPermitida().ObterTodos(pb).Where(x => x.tipodespesa_id == idTipo && x.projeto_id == idProjeto).ToList().FirstOrDefault();
            if (obj != null)
            {
                if (obj.cobravel == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ValidaDespesaAdiantamento(int idProjeto, int idTipo, ParamBase pb)
        {
            int estab = pb.estab_id;
            var obj = new DespesaPermitida().ObterTodos(pb).Where(x => x.tipodespesa_id == idTipo && x.projeto_id == idProjeto).ToList().FirstOrDefault();
            if (obj != null)
            {
                if (obj.descricao == "Adiantamento")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }



}