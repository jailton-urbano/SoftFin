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
    public class TaxaHora : BaseModels
    {
        // Informa as taxas de venda e de custo para profissionais alocados em projetos
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Categoria Profissional"), Required(ErrorMessage = "*")]
        public int categoria_id { get; set; }
        [JsonIgnore,ForeignKey("categoria_id")]
        public virtual CategoriaProfissional categoriaProfissional { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string descricao { get; set; }

        [Display(Name = "Data de Validade"), Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime dataValidade { get; set; }

        [Display(Name = "Taxa Hora Venda"), Required(ErrorMessage = "*")]
        public Decimal taxaHoraVenda { get; set; }

        [Display(Name = "Taxa Hora Custo"), Required(ErrorMessage = "*")]
        public Decimal taxaHoraCusto { get; set; }


        public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(ref erro, pb);
        }
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
                    db.TaxaHora.Remove(obj);
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

        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(TaxaHora obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, TaxaHora obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TaxaHora obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<TaxaHora>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public TaxaHora ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public TaxaHora ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.TaxaHora.Where(x => x.id == id && x.empresa_id == empresa).Include(p => p.categoriaProfissional).FirstOrDefault();
        }
        public List<TaxaHora> ObterTodos(ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.TaxaHora.Where(x => x.empresa_id == empresa).ToList();
        }

    }
}