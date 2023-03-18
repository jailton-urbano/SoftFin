using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.POC.AnaliseAmostra.Models
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
    }
}