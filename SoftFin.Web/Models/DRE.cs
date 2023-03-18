using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;

namespace SoftFin.Web.Models
{
    public class DRE
    {
        public string codigoConta { get; set; }
        public string descricaoConta { get; set; }
        public string valor { get; set; }
        public string percentual { get; set; }
    }

    public class movimentoBancario
    {
        public string unidadenegocio { get; set; }
        public string dataMovimento { get; set; }
        public string contaContabil { get; set; }
        public string pessoa { get; set; }
        public string historicoMovimento { get; set; }
        public string tipoMovimento { get; set; }
        public string valorMovimento { get; set; }
        public string saldoFinal { get; set; }
        public string conciliado { get; set; }
        public string saldoReal { get; set; }
        public string diferenca { get; set; }
        public string valorOvs { get; set; }

        public string referencia { get; set; }
    }

    public class DRE2
    {
   
        public string stiloGeral { get; set; }
        public string codigoConta { get; set; }
        public string tipoConta { get; set; }
        public string descricaoConta { get; set; }
        public Decimal valor { get; set; }
        public Decimal percentual { get; set; }
        public string stiloValor { get;  set; }
    }

    public class DRE3
    {
        public DRE3()
        {
            valor = new List<decimal>();
            percentual = new List<decimal>();

        }
        public string codigoConta { get; set; }
        public string tipoConta { get; set; }
        public string descricaoConta { get; set; }
        public List<Decimal> valor { get; set; }
        public List<Decimal> percentual { get; set; }
    }

    public class DRE_detalhe
    {
        
        public string empresa_det { get; set; }
        public string data { get; set; }
        public string pessoa { get; set; }
        public string historico { get; set; }
        public Decimal valor { get; set; }
    }

    public class PerformanceCaixa
    {
        public string conta { get; set; }
        public Decimal percRecebimentos { get; set; }
        public Decimal percMercado { get; set; }
        public Decimal percVariacao { get; set; }
        public string percVariacaoS { get; set; }
    }
}