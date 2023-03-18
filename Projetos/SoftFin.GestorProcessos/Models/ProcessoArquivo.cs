using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class ProcessoArquivo
    {
        public ProcessoArquivo()
        {
            DataInclucao = DateTime.Now;
        }
        public int Id { get; set; }

        public DateTime DataInclucao { get; set; }

        [MaxLength(50)]
        public String Descricao { get; set; }

        [Display(Name = "Arquivo Real"), Required(ErrorMessage = "*"), MaxLength(750)]
        public string ArquivoReal { get; set; }
        [Display(Name = "Arquivo Original"), Required(ErrorMessage = "*"), MaxLength(750)]
        public string ArquivoOriginal { get; set; }

        public int Tamanho { get; set; }

        [MaxLength(10)]
        public string ArquivoExtensao { get; set; }

        [MaxLength(75)]
        public string RotinaOwner { get; set; }

        [MaxLength(75)]
        public string Codigo { get; set; }

        public string CodigoEmpresa { get; set; }


        public int ProcessoId { get; set; }
        [JsonIgnore, ForeignKey("ProcessoId")]
        public virtual Processo Processo { get; set; }

    }
}