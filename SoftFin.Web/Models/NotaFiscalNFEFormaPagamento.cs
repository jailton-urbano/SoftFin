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
    public class NotaFiscalNFEFormaPagamento
    {
        
        [Key()]
        public int id { get; set; }
        public int notaFiscal_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }

        public byte? indPag { get; set; }

        public byte? tPag { get; set; }

        public decimal vPag { get; set; }

        public byte? tpIntegra { get; set; }

        [MaxLength(14)]
        public string CNPJ { get; set; }

        public byte? tBand { get; set; }
        
        [MaxLength(20)]
        public string cAut { get; set; }

        public decimal? vTroco { get; set; }



        public List<NotaFiscalNFEFormaPagamento> ObterTodos(int id, DbControle db = null)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFEFormaPagamento.Where(nf => nf.notaFiscal_id == id).ToList();
            return nfe;
        }

        public List<NotaFiscalNFEFormaPagamento> ObterPorNf(int nf)
        {
            DbControle db = new DbControle();
            return db.NotaFiscalNFEFormaPagamento.Where(x => x.notaFiscal_id == nf).ToList();
        }

        
        public List<NotaFiscalNFEFormaPagamento> ObterPorCapa(int id)
        {
            return ObterPorCapa(id, null);
        }
        public List<NotaFiscalNFEFormaPagamento> ObterPorCapa(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFEFormaPagamento.Where(nf => nf.notaFiscal_id == id).ToList();
            return nfe;
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFEFormaPagamento.Remove(this);
            banco.SaveChanges();
        }

        public void Incluir(DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(this, "", "", db, pb);
            db.Set<NotaFiscalNFEFormaPagamento>().Add(this);
            db.SaveChanges();
            return;
        }


    }
}