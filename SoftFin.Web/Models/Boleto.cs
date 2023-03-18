using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class Boleto
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Data Vencimento"), Required(ErrorMessage = "Informe a data de Vencimento")]
        public DateTime DataVencimento { get; set; }

        [Display(Name = "Valor"), Required(ErrorMessage = "Informe o Valor")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Valor { get; set; }

        [Display(Name = "Número Documento"), Required(ErrorMessage = "Informe o número do documento"), MaxLength(15)]
        public string NumeroDoc { get; set; }

        [Display(Name = "Pago"), Required(ErrorMessage = "Informe se documento Pago")]
        public bool Pago { get; set; }

        [Display(Name = "Data Pagamento")]
        public DateTime? DataPagamento { get; set; }
        
        [Display(Name = "Valor Pago")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorPago { get; set; }

        [Display(Name = "Data Emissao"), Required(ErrorMessage = "Data de Emissão")]
        public DateTime DataEmissao { get; set; }

        [Display(Name = "Texto Boleto 01"), MaxLength(40)]
        public string TextoBoleto01 { get; set; }

        [Display(Name = "Texto Boleto 02"), MaxLength(40)]
        public string TextoBoleto02 { get; set; }

        [Display(Name = "Texto Boleto 03"), MaxLength(40)]
        public string TextoBoleto03 { get; set; }



        [Display(Name = "Banco"), Required(ErrorMessage = "Informe o banco")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco banco { get; set; }


        public bool CnabGerado { get; set; }
        public DateTime? CnabDataGerado { get; set; }
        public bool CnabCancelado { get; set; }
        public DateTime? CnabDataCancelado { get; set; }

        [JsonIgnore, ForeignKey("OrdemVenda_ID")]
        public virtual OrdemVenda OrdemVenda { get; set; }

        public int OrdemVenda_ID { get; set; }
        public string NomeArquivo { get;  set; }
        public decimal? Multa { get; internal set; }
        public decimal? JurosMora { get; internal set; }
        [MaxLength(30)]
        public string CodigoTransmissao { get; set; }
        public IQueryable<Boleto> ObterTodos(ParamBase paramBase, DbControle db)
        {
            
            return db.Boleto.Where(x => x.OrdemVenda.estabelecimento_id == paramBase.estab_id).Include(p => p.OrdemVenda.Pessoa);
        }
        public IQueryable<Boleto> ObterTodosOV(ParamBase paramBase, DateTime dataInicial, DateTime dateFinal)
        {
            DbControle banco = new DbControle();
            var retorno = banco.Boleto.Where(x => x.OrdemVenda.estabelecimento_id == paramBase.estab_id
                          && DbFunctions.TruncateTime(x.OrdemVenda.data) >= dataInicial
                          && DbFunctions.TruncateTime(x.OrdemVenda.data) <= dateFinal);


            foreach (var item in retorno)
            {
                var ov = new OrdemVenda().ObterPorId(item.OrdemVenda_ID, banco);
                item.OrdemVenda = ov;


                var nf = new NotaFiscal().ObterPorOV(item.OrdemVenda_ID);
                if (nf != null)
                    item.OrdemVenda.NotasFiscais.Add(nf);
            }

            return retorno;
        }

        public IQueryable<Boleto> ObterPorOV(ParamBase paramBase,int id)
        {
            DbControle banco = new DbControle();
            return banco.Boleto.Where(x => x.OrdemVenda.id == id

                          );
        }

        public Boleto ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Boleto ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.Boleto.Where(x => x.id == id).Include(p => banco).Include(p => p.OrdemVenda.Pessoa).FirstOrDefault();
        }

    }
}