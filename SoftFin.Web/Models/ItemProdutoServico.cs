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
    public class ItemProdutoServico: BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Código"),
        MaxLength(20),
        Required(ErrorMessage = "Campo Código obrigatório")]
        public string codigo { get; set; }

        [Display(Name = "Descrição"),
        MaxLength(70),
        Required(ErrorMessage = "Campo Descrição Obrigatório")]
        public string descricao { get; set; }

        [Display(Name = "Unidade de Medida"),
        MaxLength(50)]
        public string unidadeMedida { get; set; }

        [Display(Name = "NCM"),
        MaxLength(50)]
        public string ncm { get; set; }

        [Display(Name = "Categoria Item Produto/Serviço")]
        public int CategoriaItemProdutoServico_ID { get; set; }
        [JsonIgnore,ForeignKey("CategoriaItemProdutoServico_ID")]
        public virtual CategoriaItemProdutoServico CategoriaItemProdutoServico { get; set; }

        [Display(Name = "Código de Barras"), MaxLength(120)]
        public string codigoBarrasEAN { get; set; }

        [Display(Name = "Marca"), MaxLength(120)]
        public string marca { get; set; }

        [Display(Name = "Origem"), MaxLength(1)]
        public string origem { get; set; }

        [Display(Name = "EstoqueMinimo")]
        public int? estoque { get; set; }

        [Display(Name = "Custo")]
        public decimal? custo { get; set; }

        [Display(Name = "Margen")]
        public decimal? margem { get; set; }

        [Display(Name = "Preço Venda")]
        public decimal? precoVenda { get; set; }

        [Display(Name = "Informações Complementares"),MaxLength(250)]
        public string informacoesComplementares { get; set; }

        [Display(Name = "EXTIPI (Código de Excessão da tabela IPI)"), MaxLength(30)]
        public string EXTIPI { get; set; }

        [Display(Name = "CEST (Código Especificador de Substituição Tributaria)"), MaxLength(30)]
        public string CEST { get; set; }

        [Display(Name = "Peso Liquido")]
        public decimal? pesoLiquido { get; set; }

        [Display(Name = "Peso Bruto")]
        public decimal? pesoBruto { get; set; }

        [Display(Name = "Ativo")]
        public bool? Ativo { get; set; }

        [Display(Name = "Porcentagem Valor Tributos (IBPT)")]
        public decimal valorTributos { get; set; }

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
                    db.ItemProdutoServico.Remove(obj);
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

        public bool Alterar(ItemProdutoServico obj, ParamBase pb, DbControle db = null)
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


        private bool validaExistencia(DbControle db, ItemProdutoServico obj, ParamBase pb)
        {
            DbControle banco = new DbControle();

            int idempresa  = pb.empresa_id;


            var objAux = banco.ItemProdutoServico.Where(x => x.empresa_id == idempresa && x.codigo == obj.codigo).FirstOrDefault();
            if (objAux != null)
                return true;
            return (false);
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }

        public bool Incluir(ItemProdutoServico obj, ParamBase pb, DbControle banco = null)
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

                db.Set<ItemProdutoServico>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public ItemProdutoServico ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public ItemProdutoServico ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.ItemProdutoServico.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }

        public List<ItemProdutoServico> ObterTodos(ParamBase pb, DbControle db = null)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();
            return db.ItemProdutoServico.Where(x => x.empresa_id == idempresa).OrderBy(p => p.descricao).ToList();
        }


        public int? usuarioinclusaoid { get; set; }

        public DateTime? dataInclusao { get; set; }

        public int? usuarioalteracaoid { get; set; }

        public DateTime? dataAlteracao { get; set; }
    }
}
