using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;

namespace SoftFin.Web.Models
{
    public class RecebimentoFiltro
    {
        [Display(Name = "Razão Tomador"), StringLength(50)]
        public string razaoTomador { get; set; }

        [Display(Name = "CNPJ"), StringLength(18)]
        public string cnpj { get; set; }

        [Display(Name = "CPF"), StringLength(15)]
        public string cpf { get; set; }

        [Display(Name = "Número NFS-e Inicial"), StringLength(15)]
        public string numeroNfseIni { get; set; }

        [Display(Name = "Número NFS-e Final"), StringLength(15)]
        public string numeroNfseFim { get; set; }

        [Display(Name = "Emissão NFS-e Inicial")]
        public DateTime? dataEmissaoNfseIni { get; set; }

        [Display(Name = "Emissão NFS-e Final")]
        public DateTime? dataEmissaoNfseFim { get; set; }

        [Display(Name = "Vencimento Inicial")]
        [DataType(DataType.Date)]
        public DateTime? dataVencimentoNfseIni { get; set; }

        [Display(Name = "Vencimento Final")]
        [DataType(DataType.Date)]
        public DateTime? dataVencimentoNfseFim { get; set; }


        [Display(Name = "Data de Recebimento NFS-e Inicial")]
        [DataType(DataType.Date)]
        public DateTime? dataRecebimentoIni { get; set; }

        [Display(Name = "Data de Recebimento NFS-e Final")]
        [DataType(DataType.Date)]
        public DateTime? dataRecebimentoFim { get; set; }
        
        [Display(Name = "Histórico"), StringLength(30)]
        public string historico { get; set; }

        [Display(Name = "Banco"), StringLength(30)]
        public string banco_id { get; set; }

        [Display(Name = "Valor Liquido NFS-e Inicial")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorLiquidoIni { get; set; }

        [Display(Name = "Valor Liquido NFS-e Final")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorLiquidoFim { get; set; } 

        [Display(Name = "Valor Recebimento NFS-e Inicial")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorRecebimentoIni { get; set; }

        [Display(Name = "Valor Recebimento NFS-e Final")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? ValorRecebimentoFim { get; set; }

        [Display(Name = "Em aberto"), StringLength(15)]
        public bool emaberto { get; set; }

    }
}
