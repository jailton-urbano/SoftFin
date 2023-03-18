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
    public class LoteDespesa
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Código do Lote"),
        Required(ErrorMessage = "*")]
        public int codigo { get; set; }

        [DisplayName("Data"),
        Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Display(Name = "Histórico"),
        Required(ErrorMessage = "*"),MaxLength(200)]
        public string Historico { get; set; }

        [Display(Name = "Número da Nota de Débito")]
        public int? nd_id { get; set; }
        [JsonIgnore,ForeignKey("nd_id")]
        public virtual NotadeDebito NotadeDebito { get; set; }

        [Display(Name = "Valor do Lote")]
        public decimal ValorLote { get; set; }

        [Display(Name = "Tipo Lote Despesa"), Required(ErrorMessage = "*")]
        public int tipoLoteDespesa_id { get; set; }
        [JsonIgnore,ForeignKey("tipoLoteDespesa_id")]
        public virtual TipoLoteDespesa TipoLoteDespesa { get; set; }

        [Display(Name = "Situação Lote Despesa"), Required(ErrorMessage = "*")]
        public int situacaoLoteDespesa_id { get; set; }
        [JsonIgnore,ForeignKey("situacaoLoteDespesa_id")]
        public virtual SituacaoLoteDespesa SituacaoLoteDespesa { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.LoteDespesa.Remove(obj);
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

        public bool Alterar(LoteDespesa obj, ParamBase pb)
        {
            DbControle db = new DbControle();

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


        private bool validaExistencia(DbControle db, LoteDespesa obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(LoteDespesa obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<LoteDespesa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public LoteDespesa ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public LoteDespesa ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
               db = new DbControle();
            return db.LoteDespesa.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<LoteDespesa> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.LoteDespesa.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public int ObterUltimo(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            if (db.LoteDespesa.Where(x => x.estabelecimento_id == estab).Count() > 0)
            {
                return db.LoteDespesa.Where(x => x.estabelecimento_id == estab).Max(m => m.codigo);
            }
            else
            {
                return 0;
            }
        }

    }
}