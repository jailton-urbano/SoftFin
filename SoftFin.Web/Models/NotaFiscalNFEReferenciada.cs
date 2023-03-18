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
    public class NotaFiscalNFEReferenciada
    {
        [Key]
        public int id { get; set; }

        public int notaFiscal_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }

        [MaxLength(15), Display(Name = "NFe")]
        public string NFe { get; set; }
        [MaxLength(15), Display(Name = "CTe")]
        public string CTe { get; set; }
        [MaxLength(1), Display(Name = "Série de Nota Referenciada")]
        public string nfserie { get; set; }

        [MaxLength(15), Display(Name = "Número de Nota Referenciada")]
        public string nfnumero { get; set; }
        [MaxLength(10), Display(Name = "Model de Nota Referenciada")]
        public string nfmodelo { get; set; }
        [MaxLength(2), Display(Name = "UF de Nota Nota Referenciada")]
        public string nfuf { get; set; }
        [MaxLength(4), Display(Name = "Ano e Mes de Nota Referenciada")]
        public string nfanoMesEmissao { get; set; }
        [MaxLength(14), Display(Name = "CNPJ de Nota Referenciada")]
        public string nfcnpj { get; set; }


        [MaxLength(1), Display(Name = "Série de Nota Referenciada Produtor")]
        public string nfprodserie { get; set; }

        [MaxLength(15), Display(Name = "Número de Nota Referenciada Produtor")]
        public string nfprodnumero { get; set; }
        [MaxLength(10), Display(Name = "Modelo de Nota Referenciada Produtor")]
        public string nfprodmodelo { get; set; }
        [MaxLength(2), Display(Name = "UF de Nota Referenciada Produtor")]
        public string nfproduf { get; set; }
        [MaxLength(4), Display(Name = "Mês e Ano de Nota Referenciada Produtor")]
        public string nfprodanoMesEmissao { get; set; }
        [MaxLength(14), Display(Name = "CNPJ/CPF de Nota Referenciada Produtor")]
        public string nfprodcnpjCpf { get; set; }
        [MaxLength(14), Display(Name = "IE de Nota Referenciada Produtor")]
        public string nfprodIE { get; set; }

        [MaxLength(20),  Display(Name = "ECF Cupom Referenciado")]
        public string ECF { get; set; }
        [MaxLength(20),  Display(Name = "COO Cupom Referenciado")]
        public string numeroCOO { get; set; }
        [MaxLength(2),  Display(Name = "Modelo Cupom Referenciado")]
        public string modelo { get; set; }





        public List<NotaFiscalNFEReferenciada> ObterTodos(int id)
        {
            return ObterTodos(id, null);
        }
        public List<NotaFiscalNFEReferenciada> ObterTodos(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFEReferenciada.Where(nf => nf.notaFiscal_id == id).ToList();
            return nfe;
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFEReferenciada.Remove(this);
            banco.SaveChanges();
        }

        public void Incluir(DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(this, "", "", db, pb);
            db.Set<NotaFiscalNFEReferenciada>().Add(this);
            db.SaveChanges();
            return;
        }
    }
}