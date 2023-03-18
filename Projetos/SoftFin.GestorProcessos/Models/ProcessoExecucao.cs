using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class ProcessoExecucao
    {
        [Key]
        public int Id { get; set; }
        public Guid Codigo { get; set; }

        public int ProcessoId { get; set; }
        [JsonIgnore, ForeignKey("ProcessoId")]
        public virtual Processo Processo { get; set; }


        public DateTime InicioProcesso { get; set; }
        public DateTime? FimProcesso { get; set; }
        public int? IdUsuario { get; set; }

        [JsonIgnore, ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
        [MaxLength(20)]
        public string Protocolo { get;  set; }
        [MaxLength(500)]
        public string MotivoCancelado { get;  set; }

        public int? IdUsuarioCancelamento { get;  set; }
        [JsonIgnore, ForeignKey("IdUsuarioCancelamento")]
        public virtual Usuario UsuarioCancelamento { get; set; }
    }
}