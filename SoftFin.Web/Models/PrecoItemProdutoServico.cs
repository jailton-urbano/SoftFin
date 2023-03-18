using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class PrecoItemProdutoServico
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Descrição"),
        MaxLength(70)]
        public string descricao { get; set; }

        [Display(Name = "Preço")]
        public decimal preco { get; set; }

        [Display(Name = "Item Produto/Serviço")]
        public int ItemProdutoServico_ID { get; set; }
        [JsonIgnore,ForeignKey("ItemProdutoServico_ID")]
        public virtual ItemProdutoServico ItemProdutoServico { get; set; }

        [Display(Name = "Tabela Preço Item Produto/Serviço")]
        public int TabelaPrecoItemProdutoServico_ID { get; set; }
        [JsonIgnore,ForeignKey("TabelaPrecoItemProdutoServico_ID")]
        public virtual TabelaPrecoItemProdutoServico TabelaPrecoItemProdutoServico { get; set; }

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
                    db.PrecoItemProdutoServico.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Impossível excluir, Pessoa está relacionada com outro cadastro";
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

        public bool Alterar(PrecoItemProdutoServico obj, ParamBase pb, DbControle db = null)
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


        private bool validaExistencia(DbControle db, PrecoItemProdutoServico obj, ParamBase pb)
        {
            DbControle banco = new DbControle();

            int idempresa  = pb.empresa_id;


            var objAux = banco.PrecoItemProdutoServico.Where(x => x.empresa_id == idempresa && x.descricao == obj.descricao && x.TabelaPrecoItemProdutoServico_ID == obj.TabelaPrecoItemProdutoServico_ID).FirstOrDefault();
            if (objAux != null)
                return true;
            return (false);
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }

        public bool Incluir(PrecoItemProdutoServico obj, ParamBase pb, DbControle banco = null)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
                db = banco;

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<PrecoItemProdutoServico>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public PrecoItemProdutoServico ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public PrecoItemProdutoServico ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.PrecoItemProdutoServico.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }

        public List<PrecoItemProdutoServico> ObterTodos(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.PrecoItemProdutoServico.Where(x => x.empresa_id == idempresa).OrderBy(p => p.descricao).ToList();
        }


        public PrecoItemProdutoServico ObterPorTabelaEProduto(int p1, int p2, ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.PrecoItemProdutoServico.Where(x => 
                x.TabelaPrecoItemProdutoServico_ID == p1 
                && x.ItemProdutoServico_ID == p2
                && x.empresa_id == idempresa).FirstOrDefault();
        }

        public List<PrecoItemProdutoServico> ObterPorProduto(int idproduto, ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.PrecoItemProdutoServico.Where(x => x.ItemProdutoServico_ID == idproduto
                && x.empresa_id == idempresa).ToList();
        }
    }

    public class TabelaPrecoView
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public List<PrecoItemView> PrecoItemList { get; set; }
    }

    public class PrecoItemView
    {
        public int id { get; set; }
        public decimal preco { get; set; }
        public string item { get; set; }

    }

    public class ItemView
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }

    }
}
