using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class CPAGEstimativa
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o Estabelecimento")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore, ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "Informe a Pessoa")]
        public int pessoa_id { get; set; }
        [JsonIgnore, ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Valor Padrão"), Required(ErrorMessage = "Informe o Valor Padrão")]
        public decimal valorPadrao { get; set; }
        
        [Display(Name = "Banco"), Required(ErrorMessage = "Informe o Banco")]
        public int banco_id { get; set; }
        [JsonIgnore, ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }

        [Display(Name = "Tipo Documento"), Required(ErrorMessage = "Informe o Tipo de Documento*")]
        public int tipodocumento_id { get; set; }
        [JsonIgnore, ForeignKey("tipodocumento_id")]
        public virtual TipoDocumento tipoDocumento { get; set; }

        [Display(Name = "Tipo Lançamento"), Required(ErrorMessage = "*"), StringLength(1)]
        public string tipolancamento { get; set; }

        [Display(Name = "Usuario Inclusão")]
        public int? usuarioinclusaoid { get; set; }
        [JsonIgnore, ForeignKey("usuarioinclusaoid")]
        public virtual Usuario UsuarioInclusao { get; set; }

        [Display(Name = "Usuario Alteração")]
        public int? usuarioalteracaoid { get; set; }
        [JsonIgnore, ForeignKey("usuarioalteracaoid")]
        public virtual Usuario UsuarioAlteracao { get; set; }
        
        [Display(Name = "Plano de Conta"),
        Required(ErrorMessage = "Informe o plano de contas")]
        public int planoDeConta_id { get; set; }
        [JsonIgnore, ForeignKey("planoDeConta_id")]
        public virtual PlanoDeConta PlanoDeConta { get; set; }

        public DateTime VigenciaInicial { get; set; }

        public DateTime? VigenciaFinal { get; set; }

        public int diaVencimento { get; set; }

        public bool usaUltimoValorPago { get; set; }

    }
}