using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class MensagemSistema
    {
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Usuario"), Required(ErrorMessage = "*")]
        public int usuario_id { get; set; }
        
        [Display(Name = "Titulo"), Required(ErrorMessage = "*"), MaxLength(200)]
        public string titulo { get; set; }
        
        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(2000)]
        public string descricao { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(20)]
        public string descricaoMini { get; set; }

        [Display(Name = "Data Criação")]
        public DateTime dataCriacao { get; set; }

        [Display(Name = "Data Lida")]
        public DateTime? dataExibida{ get; set; }

        [Display(Name = "Data Descartada")]
        public DateTime? dataDescartada { get; set; }
    }
}