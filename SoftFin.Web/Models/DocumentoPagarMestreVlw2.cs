using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class DocumentoPagarMestreAutorizacao
    {
        public int id { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "*")]
        public string pessoa { get; set; }

        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public string dataLancamento { get; set; }

        [Display(Name = "Data Competencia"), Required(ErrorMessage = "*"), StringLength(7)]
        public string dataCompetencia { get; set; }

        [Display(Name = "Data Vencimento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public string dataVencimento { get; set; }

        [Display(Name = "Valor Bruto"), Required(ErrorMessage = "*")]
        public string valorBruto { get; set; }

        [Display(Name = "Tipo Documento"), Required(ErrorMessage = "*")]
        public string tipodocumento { get; set; }
        
        [Display(Name = "Banco"),
        Required(ErrorMessage = "*")]
        public string banco { get; set; }

        [Display(Name = "Numero Doc")]
        public string numerodocumento { get; set; }
        public string erro { get;  set; }
    }

    public class DocumentoPagarMestreVlw2
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "*")]
        public int pessoa_id { get; set; }

        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public DateTime dataLancamento { get; set; }

        [Display(Name = "Data Competencia"), Required(ErrorMessage = "*"), StringLength(7)]
        public string dataCompetencia { get; set; }

        [Display(Name = "Data Vencimento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public DateTime dataVencimento { get; set; }

        [Display(Name = "Data Vencimento Original"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public DateTime dataVencimentoOriginal { get; set; }

        [Display(Name = "Valor Bruto"), Required(ErrorMessage = "*")]
        public decimal valorBruto { get; set; }

        [Display(Name = "Tipo Documento"), Required(ErrorMessage = "*")]
        public int tipodocumento_id { get; set; }

        [Display(Name = "Tipo Lançamento"), Required(ErrorMessage = "*"), StringLength(1)]
        public string tipolancamento { get; set; }

        [Display(Name = "Número Documento"), Required(ErrorMessage = "*")]
        public int numeroDocumento { get; set; }

        [Display(Name = "Data Documento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public DateTime dataDocumento { get; set; }

        [Display(Name = "Situação Pagamento"), Required(ErrorMessage = "*"), StringLength(1)]
        public string situacaoPagamento { get; set; }

        [Display(Name = "Data Pagamento"), DataType(DataType.Date)]
        public DateTime? dataPagamanto { get; set; }

        [Display(Name = "Código"),]
        public int? codigoPagamento { get; set; }

        [Display(Name = "Lote Pagamento Banco"), StringLength(15)]
        public string lotePagamentoBanco { get; set; }


        [JsonIgnore,ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [JsonIgnore,ForeignKey("tipodocumento_id")]
        public virtual TipoDocumento tipoDocumento { get; set; }

        [Display(Name = "Documento Pagar Aprovacao")]
        public int? documentopagaraprovacao_id { get; set; }

        [JsonIgnore,ForeignKey("documentopagaraprovacao_id")]
        public virtual DocumentoPagarAprovacao DocumentoPagarAprovacao { get; set; }


        [Display(Name = "Banco"),
        Required(ErrorMessage = "*")]
        public int? banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }


        #region FGTS
        [Display(Name = "Código da Receita")]
        public int FGTScodigoReceita { get; set; }
        [Display(Name = "Tipo de Inscrição")]
        public int FGTStipoInscricao { get; set; }
        [Display(Name = "CNPJ"), MaxLength(18)]
        public string FGTScnpj { get; set; }
        [Display(Name = "Código de Barras"), MaxLength(48)]
        public string FGTScodigoBarras { get; set; }
        [Display(Name = "Identificador FGTS"), MaxLength(16)]
        public string FGTSidentificadorFgts { get; set; }
        [Display(Name = "Lacre Conectividade Social")]
        public int FGTSlacreConectividadeSocial { get; set; }
        [Display(Name = "Dígita do Lacre")]
        public int FGTSdigitoLacre { get; set; }
        [Display(Name = "Nome do Contribuinte"), MaxLength(30)]
        public string FGTSnomeContribuinte { get; set; }
        [Display(Name = "Data de Pagamento")]
        public DateTime FGTSdataPagamento { get; set; }
        [Display(Name = "Valor Pagamento")]
        public Decimal FGTSvalorPagamento { get; set; }
        
        #endregion

        #region GPS
        [Display(Name = "Código de Pagamento")]
        public int GPScodigoPagamento { get; set; }
        [Display(Name = "Competência"), MaxLength(7)]
        public string GPScompetencia { get; set; }
        [Display(Name = "Identificador"), MaxLength(14)]
        public string GPSidentificador { get; set; }
        [Display(Name = "Valor do Tributo")]
        public decimal GPSvalorTributo { get; set; }
        [Display(Name = "Valor Outras Entidades")]
        public decimal GPSvalorOutrasEntidades { get; set; }
        [Display(Name = "Valor Atualização Monetária")]
        public decimal GPSvalorAtualizacaoMonetaria { get; set; }
        [Display(Name = "Valor Arrecadado")]
        public decimal GPSvalorArrecadado { get; set; }
        [Display(Name = "Data da Arrecadação")]
        public DateTime GPSdataArrecadacao { get; set; }
        [Display(Name = "Informações Complementares"), MaxLength(50)]
        public string GPSinformacoesComplementares { get; set; }
        [Display(Name = "Nome do Contribuinte"), MaxLength(30)]
        public string GPSnomeContribuinte { get; set; }
        #endregion

        #region DARF
        [Display(Name = "Código da Receita")]
        public int DARFcodigoReceita { get; set; }

        [Display(Name = "CNPJ"), MaxLength(18)]
        public string DARFcnpj { get; set; }
        [Display(Name = "Período de Apuração")]
        public DateTime DARFperiodoApuracao { get; set; }

        [Display(Name = "Número de Referência"), MaxLength(17)]
        public string DARFnumeroReferencia { get; set; }
        [Display(Name = "Valor Principal")]
        public decimal DARFvalorPrincipal { get; set; }
        [Display(Name = "Valor da Multa")]
        public decimal DARFmulta { get; set; }
        [Display(Name = "Valor Juros e Encargos")]
        public decimal DARFjurosEncargos { get; set; }
        [Display(Name = "Valor Total")]
        public decimal DARFvalorTotal { get; set; }
        [Display(Name = "Data de Vencimento")]
        public DateTime DARFdataVencimento { get; set; }
        [Display(Name = "Data de Pagamento")]
        public DateTime DARFdataPagamento { get; set; }
        [Display(Name = "Nome do Contribuinte"), MaxLength(30)]
        public string DARFnomeContribuinte { get; set; }
        [Display(Name = "Estabelecimento")]
        public int DARFestabelecimento_id { get; set; }
        #endregion

        [Display(Name = "Codigo Barra")]
        public string linhadigitavel { get; set; }

         
    }

    //public class CPAGVM
    //{
    //    public CPAGVM()
    //    {
    //        RepetirLancamento = 1;
    //    }
    //    public string pessoa_id { get; set; }
    //    public int codigoPagamento { get; set; }
    //    public string dataLancamento { get; set; }
    //    public string dataCompetencia { get; set; }
    //    public string dataDocumento { get; set; }
    //    public string dataVencimentoOriginal { get; set; }
    //    public string dataVencimento { get; set; }
    //    public string dataBruto { get; set; }
    //    public int numeroDocumento { get; set; }
    //    public string tipolancamento { get; set; }
    //    public string situacaoPagamento { get; set; }
    //    public string dataPagamanto { get; set; }
    //    public string lotePagamentoBanco { get; set; }
    //    public string banco_id { get; set; }
    //    public decimal valorBruto { get; set; }
    //    public string linhadigitavel { get; set; }
    //    public FGTSVM FGTSVM { get; set; }
    //    public DARFVM DARFVM { get; set; }
    //    public GPSVM GPSVM { get; set; }

    //    public string ContaContabil { get; set; }
        
    //    public List<CPAGITEMVM> CPAGITEMVMs { get; set; }


    //    public int id { get; set; }

    //    public string tipodocumento_id { get; set; }

    //    public int RepetirLancamento { get; set; }

    //    public int estabelecimento_id { get; set; }

    //    public string CodigoVerificacao { get; set; }
    //}

    //public class CPAGITEMVM
    //{

    //    public string UnidadeNegocio { get; set; }
    //    public decimal Valor { get; set; }
    //    public string historico { get; set; }
    //    public string PorcentagemLct { get; set; }

    //    public int id { get; set; }
    //}

    //public class FGTSVM
    //{
    //    [Display(Name = "Código da Receita")]
    //    public int FGTScodigoReceita { get; set; }
    //    [Display(Name = "Tipo de Inscrição")]
    //    public int FGTStipoInscricao { get; set; }
    //    [Display(Name = "CNPJ"), MaxLength(18)]
    //    public string FGTScnpj { get; set; }
    //    [Display(Name = "Código de Barras"), MaxLength(48)]
    //    public string FGTScodigoBarras { get; set; }
    //    [Display(Name = "Identificador FGTS"), MaxLength(16)]
    //    public string FGTSidentificadorFgts { get; set; }
    //    [Display(Name = "Lacre Conectividade Social")]
    //    public int FGTSlacreConectividadeSocial { get; set; }
    //    [Display(Name = "Dígita do Lacre")]
    //    public int FGTSdigitoLacre { get; set; }
    //    [Display(Name = "Nome do Contribuinte"), MaxLength(30)]
    //    public string FGTSnomeContribuinte { get; set; }
    //    [Display(Name = "Data de Pagamento")]
    //    public DateTime FGTSdataPagamento { get; set; }
    //    [Display(Name = "Valor Pagamento")]
    //    public Decimal FGTSvalorPagamento { get; set; }
    //}

    //public class DARFVM
    //{
    //    [Display(Name = "Código da Receita")]
    //    public int DARFcodigoReceita { get; set; }
    //    [Display(Name = "CNPJ"), MaxLength(18)]
    //    public String DARFcnpj { get; set; }
    //    [Display(Name = "Período de Apuração")]
    //    public String DARFperiodoApuracao { get; set; }
    //    [Display(Name = "Número de Referência"), MaxLength(17)]
    //    public string DARFnumeroReferencia { get; set; }
    //    [Display(Name = "Valor Principal")]
    //    public decimal DARFvalorPrincipal { get; set; }
    //    [Display(Name = "Valor da Multa")]
    //    public decimal DARFmulta { get; set; }
    //    [Display(Name = "Valor Juros e Encargos")]
    //    public decimal DARFjurosEncargos { get; set; }
    //    [Display(Name = "Valor Total")]
    //    public decimal DARFvalorTotal { get; set; }
    //    [Display(Name = "Data de Vencimento")]
    //    public String DARFdataVencimento { get; set; }
    //    [Display(Name = "Data de Pagamento")]
    //    public String DARFdataPagamento { get; set; }
    //    [Display(Name = "Nome do Contribuinte"), MaxLength(30)]
    //    public string DARFnomeContribuinte { get; set; }
    //    [Display(Name = "Estabelecimento")]
    //    public int DARFestabelecimento_id { get; set; }
    //}

    //public class GPSVM
    //{
    //    [Display(Name = "Código de Pagamento")]
    //    public int GPScodigoPagamento { get; set; }
    //    [Display(Name = "Competência"), MaxLength(7)]
    //    public string GPScompetencia { get; set; }
    //    [Display(Name = "Identificador"), MaxLength(14)]
    //    public string GPSidentificador { get; set; }
    //    [Display(Name = "Valor do Tributo")]
    //    public decimal GPSvalorTributo { get; set; }
    //    [Display(Name = "Valor Outras Entidades")]
    //    public decimal GPSvalorOutrasEntidades { get; set; }
    //    [Display(Name = "Valor Atualização Monetária")]
    //    public decimal GPSvalorAtualizacaoMonetaria { get; set; }
    //    [Display(Name = "Valor Arrecadado")]
    //    public decimal GPSvalorArrecadado { get; set; }
    //    [Display(Name = "Data da Arrecadação")]
    //    public DateTime GPSdataArrecadacao { get; set; }
    //    [Display(Name = "Informações Complementares"), MaxLength(50)]
    //    public string GPSinformacoesComplementares { get; set; }
    //    [Display(Name = "Nome do Contribuinte"), MaxLength(30)]
    //    public string GPSnomeContribuinte { get; set; }
    //}
}