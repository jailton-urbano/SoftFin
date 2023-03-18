using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class NotaFiscalFiltro
    {

        [Display(Name = "Razão Tomador"), StringLength(50)]
        public string razaoTomador { get; set; }

        [Display(Name = "CNPJ"), StringLength(18)]
        public string cnpj { get; set; }

        [Display(Name = "CPF"),  StringLength(18)]
        public string cpf { get; set; }

        [Display(Name = "Situação Prefeitura")]
        public int? situacaoPrefeitura_id { get; set; }

        [Display(Name = "Código Serviço")]
        public int? codigoServicoEmpresa_id { get; set; }

        [Display(Name = "Unidade de negócio")]
        public int? unidadeNegocio_id { get; set; }

        [Display(Name = "Número RPS Inicial")]
        public int? numeroRpsIni { get; set; }

        [Display(Name = "Número RPS Final")]
        public int? numeroRpsFim { get; set; }

        
        [Display(Name = "Data Emissão RPS Inicial")]
        public DateTime? dataEmissaoRpsIni { get; set; }

        [Display(Name = "Data Emissão RPS Final")]
        public DateTime? dataEmissaoRpsFim { get; set; }

        [Display(Name = "Número NFS-e Inicial"), StringLength(15)]
        public string numeroNfseIni { get; set; }

        [Display(Name = "Número NFS-e Inicial"), StringLength(15)]
        public string numeroNfseFim { get; set; }


        
        [Display(Name = "Data Emissão NFS-e Inicial")]
        public DateTime? dataEmissaoNfseIni { get; set; }

        [Display(Name = "Data Emissão NFS-e Final")]
        public DateTime? dataEmissaoNfseFim { get; set; }

        [Display(Name = "Data Vencimento NFS-e Inicial")]
        [DataType(DataType.Date)]
        public DateTime? dataVencimentoNfseIni { get; set; }

        [Display(Name = "Data Vencimento NFS-e Final")]
        [DataType(DataType.Date)]
        public DateTime? dataVencimentoNfseFim { get; set; }

        [JsonIgnore,ForeignKey("codigoServicoEmpresa_id")]
        public virtual CodigoServicoEstabelecimento codigoServicoEstabelecimento { get; set; }

        [JsonIgnore,ForeignKey("unidadeNegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

    }
}
