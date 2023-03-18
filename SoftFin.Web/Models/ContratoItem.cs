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
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class ContratoItem : BaseModels
    {
        public int id { get; set; }
        
        [Display(Name = "Contrato"), Required(ErrorMessage = "Informe o contrato relacionado")]
        public int contrato_id { get; set; }
        [JsonIgnore,ForeignKey("contrato_id")]
        public virtual Contrato Contrato { get; set; }

        [Display(Name = "Número Pedido")]
        public string pedido { get; set; }


        [Display(Name = "Valor")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valor { get; set; }

        [Display(Name = "Tipo de Contrato"), Required(ErrorMessage = "Informe o tipo do contrato")]
        public int tipoContratos_ID { get; set; }

        [Display(Name = "Unidade de Negócio"), Required(ErrorMessage = "Informe a unidade de negocio")]
        public int unidadeNegocio_ID { get; set; }

        [JsonIgnore,ForeignKey("tipoContratos_ID")]
        public virtual TipoContrato TipoContrato { get; set; }

        [JsonIgnore,ForeignKey("unidadeNegocio_ID")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [Display(Name = "Item Produto/Serviço"), Required(ErrorMessage = "Informe o serviço")]
        public int itemProdutoServico_ID { get; set; }

        [Display(Name = "Tabela de Preço"), Required(ErrorMessage = "Informe a tabela da preço")]
        public int tabelaPreco_ID { get; set; }

        [JsonIgnore,ForeignKey("itemProdutoServico_ID")]
        public virtual ItemProdutoServico ItemProdutoServico { get; set; }

        [JsonIgnore,ForeignKey("tabelaPreco_ID")]
        public virtual TabelaPrecoItemProdutoServico TabelaPrecoItemProdutoServico { get; set; }


        public virtual ICollection<ParcelaContrato> ParcelaContratos { get; set; }
        public virtual ICollection<ContratoItemUnidadeNegocio> ContratoItemUnidadeNegocios { get; set; }

        [Display(Name = "Usuario Inclusão")]
        public int? usuarioinclusaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioinclusaoid")]
        public virtual Usuario UsuarioInclusao { get; set; }
        [Display(Name = "Usuario Alteração")]
        public int? usuarioalteracaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioalteracaoid")]
        public virtual Usuario UsuarioAlteracao { get; set; }

        public DateTime? dataInclusao { get; set; }

        public DateTime? dataAlteracao { get; set; }

        //0 Servico 1 Mercadoria 2 Outros
        public int TipoFaturamento { get; set; }


        public void Exclui(int ID, ParamBase pb)
        {
            var db = new DbControle();
            var obj = db.ContratoItem.Where(x => x.id == ID).First();

            new LogMudanca().Incluir(obj, "", "", db, pb);

            db.Set<ContratoItem>().Remove(obj);
            db.SaveChanges();
        }

        public bool Inclui(ContratoItem obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var contratoPesquisar = new ContratoItem();

            new LogMudanca().Incluir(obj, "", "", db, pb);

            db.ContratoItem.Add(obj);
            db.SaveChanges();
            return true;

        }

        public void Altera(ContratoItem obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(obj, "", "", db, pb);

            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<ContratoItem> lista(int idContrato)
        {
            var banco = new DbControle();
            var lista = banco.ContratoItem.Where(p => p.contrato_id ==idContrato).Include(a => a.Contrato.Pessoa).Include(b => b.TipoContrato).Include(c => c.UnidadeNegocio).ToList();
            return lista;
        }

        public ContratoItem ObterPorId(int id,ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public ContratoItem ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ContratoItem.Where(x => x.id == id && x.Contrato.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<ContratoItem> ObterTodos(ParamBase pb, int? idContrato= 0, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            if (idContrato == 0)
                return db.ContratoItem.Where(x => x.Contrato.estabelecimento_id == estab).ToList();
            else
                return db.ContratoItem.Where(x => x.Contrato.estabelecimento_id == estab && x.contrato_id == idContrato.Value).ToList();

        }

        public Decimal somaParcelas(string vcontrato, ParamBase pb)
        {
            if (vcontrato == null)
                return 0;
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ContratoItem.Where(x => x.Contrato.estabelecimento_id == estab && x.Contrato.contrato==vcontrato).Sum(x=> x.valor);
        }

        public SelectList CarregaProjetoItem(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.ContratoItem().ObterTodos(pb).OrderBy(p => p.ItemProdutoServico.descricao);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(
                    new SelectListItem() { Value = item.id.ToString()
                                        , Text = String.Format("{0} - {1} - {2}", item.Contrato.Pessoa.nome, item.Contrato.descricao, item.ItemProdutoServico.descricao)}
                        );
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }
    }
}