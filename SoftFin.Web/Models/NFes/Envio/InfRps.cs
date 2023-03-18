using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class InfRps
    {
        [StringLength(255)]
        public string Id { get; set; }
        public string Versao { get; set; }
        public DateTime Competencia { get; set; }
        public DateTime DataEmissao { get; set; }
        public int NaturezaOperacao { get; set; }
        public int RegimeEspecialTributacao { get; set; }
        public int OptanteSimplesNacional { get; set; }
        public int IncentivoFiscal { get; set; }
        public int Status { get; set; }
        public int TributarMunicipio { get; set; }
        public int TributarPrestador { get; set; }
        [StringLength(50)]
        public string CodigoVerificacao { get; set; }

    }
}