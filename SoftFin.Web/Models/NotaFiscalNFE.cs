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
    public class NotaFiscalNFE
    {
        public NotaFiscalNFE()
        {
            NotaFiscalNFEDuplicatas = new List<NotaFiscalNFEDuplicata>();
            NotaFiscalNFEItems = new List<NotaFiscalNFEItem>();
            NotaFiscalNFEItems = new List<NotaFiscalNFEItem>();
            NotaFiscalNFEReboques = new List<NotaFiscalNFEReboque>();
            NotaFiscalNFEReferenciadas = new List<NotaFiscalNFEReferenciada>();
            NotaFiscalNFEVolume = new List<NotaFiscalNFEVolume>();
            NotaFiscalNFETransportadora = new NotaFiscalNFETransportadora();
            NotaFiscalNFEFormaPagamentos = new List<NotaFiscalNFEFormaPagamento>();
        }
        public int id { get; set; }

        public DateTime dataHoraSaida { get; set; }

        public int finalidadeEmissao { get; set; }
        public string chaveAcesso { get; set; }

        public int faturaFormaPgto { get; set; }

        public string faturaNumero { get; set; }

        public decimal faturaValorOriginal { get; set; }
        public decimal faturaValorDesconto { get; set; }
        public decimal faturaValorLiquido { get; set; }

        [MaxLength(500)]
        public string informacaoComplementar { get; set; }

        [MaxLength(500)]
        public string informacaoComplementarFisco { get; set; }

        public int  indicadorPresencaComprador { get; set; }

        [MaxLength(100)]
        public string emailDestinatario { get; set; }

        [MaxLength(100)]
        public string localEmbarqueExportacao { get; set; }
        [MaxLength(2)]
        public string ufEmbarqueExportacao { get; set; }

        [MaxLength(100)]
        public string identificacaoCompradorExtrangeiro { get; set; }

        [MaxLength(100)]
        public string informacaoPedidoCompra { get; set; }
        [MaxLength(100)]
        public string informacaoContato { get; set; }
        [MaxLength(100)]
        public string informacaoNotaEmpenhoCompras { get; set; }


        public int situacao { get; set; }
        //1 - Criada, 3 - Emitida, 6 - Cancelada, 7 - Perda, 9 - Paga  

        public string CFOP { get; set; }


        public int? NotaFiscalNFEEntrega_id { get; set; }

        [JsonIgnore,ForeignKey("NotaFiscalNFEEntrega_id")]
        public virtual NotaFiscalNFEEntrega NotaFiscalNFEEntrega { get; set; }


        public int? NotaFiscalNFERetensao_id { get; set; }

        [JsonIgnore,ForeignKey("NotaFiscalNFERetensao_id")]
        public virtual NotaFiscalNFERetensao NotaFiscalNFERetensao { get; set; }

        public int? NotaFiscalNFERetirada_id { get; set; }

        [JsonIgnore,ForeignKey("NotaFiscalNFERetirada_id")]
        public virtual NotaFiscalNFERetirada NotaFiscalNFERetirada { get; set; }


        public int? NotaFiscalNFETransportadora_id { get; set; }

        [JsonIgnore,ForeignKey("NotaFiscalNFETransportadora_id")]
        public virtual NotaFiscalNFETransportadora NotaFiscalNFETransportadora { get; set; }
        public List<NotaFiscalNFEFormaPagamento> NotaFiscalNFEFormaPagamentos { get;  set; }


        public List<NotaFiscalNFEDuplicata> NotaFiscalNFEDuplicatas { get; set; }
        public List<NotaFiscalNFEItem> NotaFiscalNFEItems { get; set; }
        public List<NotaFiscalNFEReboque> NotaFiscalNFEReboques { get; set; }
        public List<NotaFiscalNFEReferenciada> NotaFiscalNFEReferenciadas { get; set; }
        public List<NotaFiscalNFEVolume> NotaFiscalNFEVolume { get; set; }


        public decimal valor { get; set; }

        public decimal baseICMS { get; set; }

        public decimal valorICMS { get; set; }

        public decimal valorICMSDesonerado { get; set; }

        public decimal baseICMSST { get; set; }

        public decimal valorICMSST { get; set; }

        public decimal valorProduto { get; set; }

        public decimal valorFrete { get; set; }

        public decimal valorSeguro { get; set; }

        public decimal valorDesconto { get; set; }

        public decimal valorII { get; set; }

        public decimal valorIPI { get; set; }

        public decimal valorPIS { get; set; }

        public decimal valorCONFINS { get; set; }

        public decimal valorOutro { get; set; }

        public decimal valorCSLL { get; set; }

        public NotaFiscalNFE ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public NotaFiscalNFE ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFE.Include("NotaFiscalNFETransportadora").Where(nf => nf.id == id).FirstOrDefault();
            //db.Entry(nfe).State = EntityState.Detached;

            return nfe;
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFE.Remove(this);
            banco.SaveChanges();
        }

        [MaxLength(255)]
        public string motivoCancelamento { get; set; }

        [MaxLength(15)]
        public string protocoloAutorizacao { get; set; }
        public int TipoOperacao { get; set; } // 0 "0-entrada 1-saída"
        public string IndicadorDestino { get;  set; }
        public string IndicadorFinal { get;  set; }
        public string IndicadorPresencial { get;  set; }
    }
}