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
    public class EstoqueMovtoItem
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Movimentação Estoque"), Required(ErrorMessage = "*")]
        public int estoqueMovto_id { get; set; }

        [JsonIgnore,ForeignKey("estoqueMovto_id")]
        public virtual EstoqueMovto EstoqueMovto { get; set; }
               

        [Display(Name = "Produto"), Required(ErrorMessage = "*")]
        public int itemProdutoServico_id { get; set; }

        [JsonIgnore,ForeignKey("itemProdutoServico_id")]
        public virtual ItemProdutoServico ItemProdutoServico { get; set; }

        public decimal quantidade { get; set; }

        public decimal valorUnitario { get; set; }

        public decimal valorTotal { get; set; }





        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.EstoqueMovtoItem.Remove(obj);
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

        public bool Alterar(EstoqueMovtoItem obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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


        private bool validaExistencia(DbControle db, EstoqueMovtoItem obj)
        {
            return (false);
        }

        public bool Incluir(EstoqueMovtoItem obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                var estab = pb.estab_id;
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<EstoqueMovtoItem>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }


        public EstoqueMovtoItem ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public EstoqueMovtoItem ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.EstoqueMovtoItem.Where(x => x.id == id).FirstOrDefault();
        }
        public List<EstoqueMovtoItem> ObterTodos(int idproduto, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.EstoqueMovtoItem.Where(x => x.itemProdutoServico_id == idproduto).ToList();
        }



        //public List<EstoqueMovtoItem> ObterEntreData(DateTime DataInicial, DateTime DataFinal)
        //{
        //    int estab = pb.estab_id;
        //    DbControle db = new DbControle();
        //    var situacaoliberada = StatusParcela.SituacaoLiberada();
        //    return db.EstoqueMovtoItem.Where(x => x.estabelecimento_id == estab && x.dataInclusao >= DataInicial && x.dataInclusao <= DataFinal).ToList();
        //}




        public List<EstoqueMovtoItem> ObterPorIdProduto(int itemProdutoServico_id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.EstoqueMovtoItem.Where(x => x.itemProdutoServico_id == itemProdutoServico_id).ToList();
        }
    }
}