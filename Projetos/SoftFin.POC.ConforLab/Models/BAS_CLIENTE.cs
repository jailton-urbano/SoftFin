using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.POC.ConforLab.Models
{
    public class BAS_CLIENTE
    {
        [Key]
        public int BCL_ID { get; set; }
        [MaxLength(400)]
        public string BCL_RAZAO { get; set; }
        [MaxLength(400)]
        public string BCL_NOME_FANTASIA { get; set; }
        [MaxLength(300)]
        public string BCL_ENDERECO { get; set; }
        [MaxLength(300)]
        public string BCL_BAIRRO { get; set; }
        [MaxLength(300)]
        public string BCL_CIDADE { get; set; }
        [MaxLength(2)]
        public string BCL_UF { get; set; }
        [MaxLength(9)]
        public string BCL_CEP { get; set; }
        [MaxLength(300)]
        public string BCL_CONTATO_AGENDAMENTO { get; set; }
        [MaxLength(300)]
        public string BCL_EMAIL { get; set; }
        [MaxLength(13)]
        public string BCL_TELEFONE1 { get; set; }
        [MaxLength(300)]
        public string BCL_CONTATO_LOCAL { get; set; }
        [MaxLength(300)]
        public string BCL_DEPARTAMENTO { get; set; }
        [MaxLength(13)]
        public string BCL_TELEFONE2 { get; set; }
        
        public Guid BCL_PROTOCOL { get; set; }

        [NotMapped]
        public string CodigoAtividade  { get; set; }

        [NotMapped]
        public string CodigoEmpresa { get; set; }

        [NotMapped]
        public string CodigoProcesso { get; set; }

        [NotMapped]
        public string CodigoUsuario { get; set; }
        [NotMapped]
        public string CodigoAtividadeExecucaoAtual { get;  set; }
        [NotMapped]
        public string CodigoProcessoAtual { get;  set; }
    }
}