using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.POC.ConforLab.Models
{
    public class POC_RETIRADA_ANALISE_AMOSTRA_CAB
    {
        [Key]
        public int RAC_ID { get; set; }

        public int PRE_ID { get; set; }

        public int PEA_ID { get; set; }

        [MaxLength(18)]
        public string RAC_N_PEDIDO { get; set; }

        public int BCL_ID { get; set; }
        [JsonIgnore, ForeignKey("BCL_ID")]
        public virtual BAS_CLIENTE BAS_CLIENTE { get; set; }

        public int BCT_ID { get; set; }
        [JsonIgnore, ForeignKey("BCT_ID")]
        public virtual BAS_CAD_TECNICO BAS_CAD_TECNICO { get; set; }

        public int BGA_ID { get; set; }
        [JsonIgnore, ForeignKey("BGA_ID")]
        public virtual BAS_PROCEDIMENTO BAS_PROCEDIMENTO { get; set; }

        public int BPR_ID { get; set; }
        public int BTE_ID { get; set; }

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
        [MaxLength(400)]
        public string RAC_PROC_AMOSTRA { get; set; }
        [MaxLength(100)]
        public string RAC_N_PONTOS { get; set; }
        [MaxLength(300)]
        public string RAC_ACOMPANHANTE { get; set; }
        public DateTime RAC_DT_RET_AMOSTRA { get; set; }
        [MaxLength(200)]
        public string RAC_COMPLEMENTO { get; set; }
        public DateTime RAC_DT_REC_AMOSTRA { get; set; }
        public DateTime RAC_H_REC_AMOSTRA_LAB { get; set; }
        [MaxLength(300)]
        public string RAC_CONFERIDO { get; set; }
        [MaxLength(10)]
        public string RAC_PLANO { get; set; }
        [MaxLength(200)]
        public string RAC_N_PATRIMONIO { get; set; }
        [MaxLength(200)]
        public string RAC_VER_APA { get; set; }
        [MaxLength(5)]
        public string RAC_BUFFER_1 { get; set; }
        [MaxLength(5)]
        public string RAC_BUFFER_2 { get; set; }
        [MaxLength(5)]
        public string RAC_BUFFER_3 { get; set; }
        public DateTime RAC_DATA { get; set; }
        [MaxLength(400)]
        public string RAC_PLAN_AMOSTRAGEM { get; set; }
        [MaxLength(400)]
        public string RAC_OBS { get; set; }

    }
}