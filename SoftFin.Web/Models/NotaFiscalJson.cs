using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class NotaFiscalJson
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Situação Prefeitura"), Required(ErrorMessage = "*")]
        public int situacaoPrefeitura_id { get; set; }
        //1-Em aberto, 2-RPS Enviado, 3-Numero da Nota Atualizado

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [Display(Name = "Ordem de Venda")]
        public Nullable<int> ordemVenda_id { get; set; }

        [Display(Name = "Código Serviço"), Required(ErrorMessage = "*")]
        public int codigoServicoEstabelecimento_id { get; set; }



        [Display(Name = "Código Serviço")]
        public string codigoServico { get; set; }


        [Display(Name = "Banco"), Required(ErrorMessage = "*")]
        public int banco_id { get; set; }

        [Display(Name = "Operação"), Required(ErrorMessage = "*")]
        public int operacao_id { get; set; }

        [Display(Name = "Tipo RPS"), Required(ErrorMessage = "*")]
        public int tipoRps { get; set; }

        [Display(Name = "Serie RPS"), Required(ErrorMessage = "*"),StringLength(15)]
        public string serieRps { get; set; }

        [Display(Name = "Número RPS"), Required(ErrorMessage = "*")]
        public int numeroRps { get; set; }

        [Display(Name = "Data Emissão RPS"), Required(ErrorMessage = "*")]
        public string dataEmissaoRps { get; set; }

        [Display(Name = "Situação RPS"), Required(ErrorMessage = "*"), StringLength(15)]
        public string situacaoRps { get; set; }

        [Display(Name = "Número Nfse")]
        public int? numeroNfse { get; set; }

        [Display(Name = "Data Emissão NFse"), Required(ErrorMessage = "*")]
        public string dataEmissaoNfse { get; set; }

        [Display(Name = "Data Vencimento NFse"), Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public string dataVencimentoNfse { get; set; }

        [Display(Name = "Codigo Verificação")]
        public string codigoVerificacao { get; set; }

        [Display(Name = "Razão Tomador"), Required(ErrorMessage = "*"), StringLength(50)]
        public string razaoTomador { get; set; }

        [Display(Name = "Indicador CNPJ CPF"), Required(ErrorMessage = "*")]
        public int indicadorCnpjCpf { get; set; }

        [Display(Name = "CNPJ CPF"), Required(ErrorMessage = "*"), StringLength(18)]
        public string cnpjCpf { get; set; }

        [Display(Name = "Inscrição Municipal"), Required(ErrorMessage = "*"), StringLength(17)]
        public string inscricaoMunicipal { get; set; }

        [Display(Name = "Inscrição Estadual"), Required(ErrorMessage = "*"), StringLength(20)]
        public string inscricaoEstadual { get; set; }

        [Display(Name = "Tipo Endereço"), Required(ErrorMessage = "*"), StringLength(35)]
        public string tipoEndereco { get; set; }

        [Display(Name = "Endereço Tomador"), Required(ErrorMessage = "*"), StringLength(35)]
        public string enderecoTomador { get; set; }

        [Display(Name = "Número Tomador"), Required(ErrorMessage = "*"), StringLength(15)]
        public string numeroTomador { get; set; }

        [Display(Name = "Complemento Tomador"),  StringLength(20)]
        public string complementoTomador { get; set; }

        [Display(Name = "Bairro Tomador"), Required(ErrorMessage = "*"), StringLength(30)]
        public string bairroTomador { get; set; }

        [Display(Name = "Cidade Tomador"), Required(ErrorMessage = "*"), StringLength(30)]
        public string cidadeTomador { get; set; }

        [Display(Name = "UF Tomador"), Required(ErrorMessage = "*"), StringLength(20)]
        public string ufTomador { get; set; }

        [Display(Name = "CEP Tomador"), Required(ErrorMessage = "*"), StringLength(10)]
        public string cepTomador { get; set; }

        [Display(Name = "Email Tomador"), Required(ErrorMessage = "*"), StringLength(300)]
        public string emailTomador { get; set; }


        [Display(Name = "Valor NFSe"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string valorNfse { get; set; }

        [Display(Name = "Valor Deduções"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string valorDeducoes { get; set; }

        [Display(Name = "Base de Calculo"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string basedeCalculo { get; set; }

        [Display(Name = "Aliquota ISS"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string aliquotaISS { get; set; }

        [Display(Name = "Valor ISS"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string valorISS { get; set; }

        [Display(Name = "Credito Imposto"), Required(ErrorMessage = "*")]
        public string creditoImposto { get; set; }

        [Display(Name = "Descriminação Serviço"), Required(ErrorMessage = "*"), StringLength(1500)]
        public string discriminacaoServico { get; set; }

        [Display(Name = "IRRF"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string irrf { get; set; }

        [Display(Name = "Pis REtido"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string pisRetido { get; set; }

        [Display(Name = "Confin Retido"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public string cofinsRetida { get; set; }

        [Display(Name = "CSLL Retido"), Required(ErrorMessage = "*"), DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public string csllRetida { get; set; }

        [Display(Name = "Valor Liquido"), Required(ErrorMessage = "*"), DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public string valorLiquido { get; set; }

        [Display(Name = "Aliquita de IRRF"), Required(ErrorMessage = "*"), DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public string aliquotaIrrf { get; set; }


        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }



        [JsonIgnore,ForeignKey("codigoServicoEstabelecimento_id")]
        public virtual CodigoServicoEstabelecimento codigoServicoEstabelecimento { get; set; }

        [JsonIgnore,ForeignKey("ordemVenda_id")]
        public virtual OrdemVenda OrdemVenda { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco banco { get; set; }

        [JsonIgnore,ForeignKey("operacao_id")]
        public virtual Operacao Operacao { get; set; }

        public virtual IEnumerable<Recebimento> Recebimentos { get; set; }




        public string valorINSS { get; set; }

        public string aliquotaINSS { get; set; }
        public string ValAliqISSRetido { get; internal set; }
        public string ValAliqCSLL { get; internal set; }
        public string ValAliqPIS { get; internal set; }
        public string ValISSRetido { get; internal set; }
        public bool ISSRetido { get; internal set; }
    }
}
