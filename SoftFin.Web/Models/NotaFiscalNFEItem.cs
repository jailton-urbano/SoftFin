using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalNFEItem
    {
        public string cBenef { get; set; }

        [Key()]
        public int id { get; set; }
        public int notaFiscal_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }

        public int idProduto { get; set; }
        public int operacao_id { get; set; }
        public int quantidade { get; set; }
        public int item { get; set; }
        public decimal valor { get; set; }
        public decimal desconto { get; set; }

        public decimal valorICMS { get; set; }
        public decimal valorICMSST { get; set; }
        public decimal valorIPI { get; set; }
        public decimal valorISS { get; set; }
        public decimal valorIRRF { get; set; }
        public decimal valorINSS { get; set; }
        public decimal valorPIS { get; set; }
        public decimal valorCOFINS { get; set; }
        public decimal valorCSLL { get; set; }

        public string NCM { get; set; }
        public string CFOP { get; set; }
        public string CSOSN { get; set; }

        [NotMapped]
        public string produto { get; set; }

        [NotMapped]
        public string operacao { get; set; }

        public decimal aliquotaISS { get; set; }

        public decimal aliquotaINSS { get; set; }

        public decimal PISRetido { get; set; }

        public decimal COFINSRetida { get; set; }

        public decimal CSLLRetida { get; set; }

        public decimal ICMSSTRetida { get; set; }

        public decimal ICMSRetida { get; set; }

        public List<NotaFiscalNFEItem> ObterPorNf(int nf)
        {
            DbControle db = new DbControle();
            return db.NotaFiscalNFEItem.Where(x => x.notaFiscal_id == nf).ToList();
        }

        public string nomeProduto { get; set; }

        public string codigoProduto { get; set; }

        public string unidadeMedida { get; set; }

        public string EAN { get; set; }

        public decimal valorUnitario { get; set; }


        public decimal? aliquotaIPI { get; set; }

        public int TabelaPrecoItemProdutoServico_id { get; set; }

        [JsonIgnore,ForeignKey("TabelaPrecoItemProdutoServico_id")]
        public virtual TabelaPrecoItemProdutoServico TabelaPrecoItemProdutoServico { get; set; }

        [MaxLength(1)]
        public string origem { get; set; }


        public decimal pRedBC { get; set; }

        public decimal valorFrete { get; set; }

        [MaxLength(20)]
        public string CEST { get; set; }


        [MaxLength(1500)]
        public string infAdProd { get; set; }


        public decimal valorTributos { get; set; }
        
        public List<NotaFiscalNFEItem> ObterPorCapa(int id)
        {
            return ObterPorCapa(id, null);
        }
        public List<NotaFiscalNFEItem> ObterPorCapa(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFEItem.Where(nf => nf.notaFiscal_id == id).ToList();
            return nfe;
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFEItem.Remove(this);
            banco.SaveChanges();
        }

        public void Incluir(DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(this, "", "", db, pb);
            db.Set<NotaFiscalNFEItem>().Add(this);
            db.SaveChanges();
            return;
        }



        [MaxLength(4)]
        public string PISCST { get; set; }


        public decimal aliquotaPIS { get; set; }

        public decimal basePIS { get; set; }

        [MaxLength(4)]
        public string COFINSCST { get; set; }

        public decimal aliquotaCOFINS { get; set; }

        public decimal baseCOFINS { get; set; }
        public string EXTIPI { get;  set; }
        public decimal valorSeguro { get;  set; }
        public string nItemPed { get;  set; }
        public string dProd { get;  set; }
        public string nRECOPI { get;  set; }
        public string indEscala { get;  set; }
        public string CNPJFab { get; set; }
        public string CST { get;  set; }
        public string aliquotaICMS { get; internal set; }
        public string modBC { get; internal set; }
    }
}