using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
        public class OperacaoView
        {
            public int id { get; set; }
            public string codigo {get; set;}
            public string descricao {get; set;}
            public string descricaoNota {get; set;}
            public int?   NaturezaOperacao {get; set;}
            public int?   RegimeEspecialTributacao {get; set;}
            public int?   OptanteSimplesNacional {get; set;}
            public int?   IncentivoFiscal {get; set;}
            public int?   TributarMunicipio {get; set;}
            public int?   TributarPrestador {get; set;}
            public string SituacaoNF { get; set; }
            public string tipoRPS_id { get; set; }
            public string situacaoTributariaNota_id { get; set; }
            public string entradaSaida { get; set; }

            public List<CalculoImpostoView> CalculoImpostoList { get; set; }
        }

        public class CalculoImpostoView
        {                
            public int      id {get; set; } 
            public decimal  aliquota {get; set; }
            public string   arrecadador {get; set; }
            public bool     retido {get; set; }
            public string   imposto { get; set; }

        }

        public class ImpostoView
        {
            public int id { get; set; }
            public string codigo { get; set; }
            public string descricao { get; set; }

        }
}