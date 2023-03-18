using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class AnoParcelaView
    {
        public int ano { get; set; }
    }

    public class AnoMesFaturamento
    {
        public int ano { get; set; }
        public int mes { get; set; }
        public String status { get; set; }
        public decimal valor { get; set; }
    }

    public class TipoContratoFaturamento
    {
        public String tipo { get; set; }
        public decimal valor { get; set; }
    }

    public class ListaNotas
    {
        public int?     numeroNfse { get; set; }
        public DateTime dataEmissaoNfse { get; set; }
        public DateTime dataVencimentoNfse { get; set; }
        public String   razaoTomador { get; set; }
        public Decimal  valorNfse { get; set; }
        public Decimal  valorLiquido { get; set; }
        public String   dataEmissaoS { get; set; }
        public String   dataVencimentoS { get; set; }
    }

    public class SaldoBanco
    {
        public DateTime data { get; set; }
        public String dataS { get; set; }
        public String codigoBanco { get; set; }
        public Decimal valor { get; set; }
    }

    public class StatisticCloud
    {
            public string qtdEmpresa { get; set; }
            public string qtdEstabelecimento { get; set; }
            public string qtdUsuario { get; set; }
            public string qtdUnidadeNegocio { get; set; }
            public string qtdPessoa { get; set; }
            public string qtdBanco { get; set; }
            public string qtdContrato { get; set; }
            public string qtdNotaFiscal { get; set; }
            public string qtdBancoMovimento { get; set; }
            public string qtdSaldoBancarioReal { get; set; }
            public string qtdProjeto { get; set; }
            public string qtdContasPagar { get; set; }
            public string qtdLogMudanca { get; set; }
    }

    public class PainelProjetos
    {
        public int id { get; set; }  
        public string contrato { get; set; }        //Contrato	Contrato.contrato
        public string cliente { get; set; }         //Cliente	Contrato.Pessoa.nome
        public string projeto { get; set; }         //Projeto	Projeto.nomeProjeto
        public DateTime dataInicial { get; set; }   //Data Inicial	Projeto.dataInicial
        public DateTime dataFinal { get; set; }     //Data Final	Projeto.dataFinal
        public Decimal totalHoras { get; set; }     //Total de Horas	Projeto.totalHoras
        public Decimal valorProjeto { get; set; }   //Valor do Projeto	Contrato.ContratoItem.valor
        public Decimal horasApontadas { get; set; } //Horas Apontadas	ApontamentoDiario.qtdHoras
        public Decimal custoTotal { get; set; }     //Custo Real	ApontamentoDiario.qtdHoras * TaxaHora.taxaHoraCusto
        public Decimal margem { get; set; }         //Margem	Valor do Projeto - Custo Real
        public Decimal saldoHoras { get; set; }     //Saldo de Horas	Total de Horas - Horas Apontadas
        public String dataInicialS { get; set; }
        public String dataFinalS { get; set; }
        public decimal despesasDiretas { get; internal set; }
        public string ativo { get; internal set; }
    }

    public class ProdutividadeRecursos
    {
        public int id { get; set; }
        public string recurso { get; set; }
        public Decimal horasDisponiveis { get; set; }
        public Decimal horasApontadas { get; set; }
        public Decimal produtividade { get; set; } // horasApontadas / horasDisponíveis * 100 (base 100%)
    }

    public class NotasEmAberto
    {
        public int id { get; set; }
        public int? numeroNfse { get; set; }
        public DateTime dataEmissaoNfse { get; set; }
        public DateTime dataVencimentoNfse { get; set; }
        public String razaoTomador { get; set; }
        public Decimal valorNfse { get; set; }
        public Decimal valorLiquido { get; set; }
        public Decimal valorRecebido { get; set; }
        public Decimal saldoReceber { get; set; }
        public String dataEmissaoS { get; set; }
        public String dataVencimentoS { get; set; }
        public int situacaoRecebimento { get; set; }
        public String textoRecebimento { get; set; }
        public Double diasAtraso { get; set; }
    }

    public class NotasDebitoEmAberto
    {
        public int id { get; set; }
        public int numeroND { get; set; }
        public DateTime dataEmissaoND { get; set; }
        public DateTime dataVencimentoND { get; set; }
        public string dataEmissaoNDS { get; set; }
        public string dataVencimentoNDS { get; set; }
        public String cliente { get; set; }
        public Decimal valorND { get; set; }
        public Double diasAtraso { get; set; }
    }

    public class comboProjeto
    {
        public int id { get; set; }
        public string nomeProjeto { get; set; }
    }

    public class comboTipoDespesa
    {
        public int id { get; set; }
        public string descricao { get; set; }
    }

    public class ListaContasAPagar
    {
        
        public string unidade { get; set; }
        public int id { get; set; }
        public string fornecedor { get; set; }
        public string tipoFornecedor { get; set; }
        public string cnpj { get; set; }
        public int parcela { get; set; }
        public int statusPagamento { get; set; }
        public string statusPagamentoS { get; set; }

        public DateTime dataDocumento { get; set; }
        public DateTime dataVencimentoOriginal { get; set; }
        public String dataVencimentoOriginalS { get; set; }
        public DateTime dataVencimento { get; set; }
        public String dataDocumentoS { get; set; }
        public String dataVencimentoS { get; set; }
        public String contaContabil { get; set; }
        public int numeroDocumento { get; set; }
        public string numeroDocumentoS { get; set; }
        public Decimal valorBruto { get; set; }
        public Decimal valorPago { get; set; }
        public Decimal saldo { get; set; }
        public string projeto { get; set; }
    }

    public class ExcelBancoMovimento
    {
        public string OrigemMovimento;
        public string planoDeContas { get; set; }
        public string tipoMovimento { get; set; }
        public string tipoDocumento { get; set; }
        public DateTime data { get; set; }
        public string historico { get; set; }
        public Decimal valor { get; set; }
        public int? idNotaFiscal { get; set; }
        public int? numeroNotaFiscal { get; set; }
        public int? idDocumentoPagar { get; set; }
        public int numeroDocumentoPagar { get; set; }
        public NotaFiscal nota { get; set; }
        public Recebimento recebimento { get; set; }
        public Pagamento pagamento { get; set; }
        public DocumentoPagarMestre documento { get; set; }
    }

    public class ExcelDocumentoPagar
    {
        public DateTime dataLancamento { get; set; }
        public string dataCompetencia { get; set; }
        public DateTime dataVencimentoOriginal { get; set; }
        public DateTime dataVencimento { get; set; }
        public Decimal valorBruto { get; set; }
        public string tipoDocumento { get; set; }
        public int numeroDocumento { get; set; }
        public DateTime dataDocumento { get; set; }
        public string situacaoPagamento { get; set; }
        public string lotePagamentoBanco { get; set; }
        public string tipoPessoa { get; set; }
        public string pessoa { get; set; }
        public string usuarioAutorizador { get; set; }
        public string banco { get; set; }
        public string linhaDigitavel { get; set; }
        public int statusPagamento { get; set; }
        public string statusPagamentoS { get; set; }
        public string planoDeConta { get; set; }
    }

    public class ExcelPessoas
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string razao { get; set; }
        public string cnpj { get; set; }
        public string inscricao { get; set; }
        public string ccm { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string eMail { get; set; }
        public string bancoConta { get; set; }
        public string agenciaConta { get; set; }
        public string agenciaDigito { get; set; }
        public string contaBancaria { get; set; }
        public string digitoContaBancaria { get; set; }
        public string unidadeNegocio { get; set; }
        public string tipoEndereco { get; set; }
        public string categoriaPessoa { get; set; }
        public string tipoPessoa { get; set; }
        public string telefoneFixo { get; set; }
        public string celular { get; set; }
    }

    public class ExcelLanctoExtrato 
    {
        public DateTime data { get; set; }
        public string idLancto { get; set; }
        public string descricao { get; set; }
        public string tipo { get; set; }
        public Decimal valor { get; set; }
        public string banco { get; set; }
        public string agencia { get; set; }
        public string conta { get; set; }
        public string contaDigito { get; set; }
        public DateTime? dataConciliado { get; set; }
        public string usuarioConciliado { get; set; }
    }
}