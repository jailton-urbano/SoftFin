using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class ProcessoAnotacao
    {
        [Key]
        public int Id { get; set; }

        public int ProcessoExecucaoId { get; set; }
        [JsonIgnore, ForeignKey("ProcessoExecucaoId")]
        public virtual ProcessoExecucao ProcessoExecucao { get; set; }

        public DateTime DataInclusao { get; set; }

        [MaxLength(250)]
        public string Empresa{ get; set; }

        [MaxLength(250)]
        public string Usuario { get; set; }

        public string Descricao { get; set; }
    }
}