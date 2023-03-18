using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class TabelaPrecoItemProdutoServico: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }


        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "Campo Descrição obrigatório"), MaxLength(50)]
        public string descricao { get; set; }
        public virtual IEnumerable<PrecoItemProdutoServico> PrecoItemProdutoServico { get; set; }

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
                    db.TabelaPrecoItemProdutoServico.Remove(obj);
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

        public bool Alterar(TabelaPrecoItemProdutoServico obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, TabelaPrecoItemProdutoServico obj, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db.TabelaPrecoItemProdutoServico.Where(x => x.empresa_id == idempresa && x.descricao == obj.descricao).Count() != 0)
            {
                return true;
            }

            return (false);
        }

        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }

        public bool Incluir(TabelaPrecoItemProdutoServico obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<TabelaPrecoItemProdutoServico>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public TabelaPrecoItemProdutoServico ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public TabelaPrecoItemProdutoServico ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.TabelaPrecoItemProdutoServico.Where(x => x.id == id ).FirstOrDefault();
        }

        public List<TabelaPrecoItemProdutoServico> ObterTodos(ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();
            return db.TabelaPrecoItemProdutoServico.Where(x => x.empresa_id == idempresa).OrderBy(p => p.descricao).ToList();
        }



        public decimal porcentagemlucropadrao { get; set; }

        public int usuarioinclusaoid { get; set; }

        public DateTime? dataInclusao { get; set; }

        public int usuarioalteracaoid { get; set; }

        public DateTime? dataAlteracao { get; set; }

        public bool? ativo { get; set; }
    }
}