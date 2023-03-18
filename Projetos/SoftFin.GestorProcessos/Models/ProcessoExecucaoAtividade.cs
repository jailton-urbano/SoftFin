using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class ProcessoExecucaoAtividade
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int ProcessoExecucaoId { get; set; }
        [JsonIgnore, ForeignKey("ProcessoExecucaoId")]
        public virtual ProcessoExecucao ProcessoExecucao { get; set; }
        public DateTime? InicioAtividade { get; set; }
        public DateTime? FimAtividade { get; set; }
        public int? IdUsuario { get; set; }

        [JsonIgnore, ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
        public int? IdUsuarioExecucao { get; set; }

        [JsonIgnore, ForeignKey("IdUsuarioExecucao")]
        public virtual Usuario UsuarioExecucao { get; set; }

        public string ResultadoFinal { get; set; }
        [MaxLength(15)]
        public string Situacao { get; set; }
        //Executando - Finalizado
        public int IdAtividade { get; set; }
        [JsonIgnore, ForeignKey("IdAtividade")]
        public virtual Atividade Atividade { get; set; }
        public string Motivo { get; internal set; }
    }
}