using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SoftFin.Web.Models;

namespace SoftFin.Web.Classes
{
    public class DbControle : DbContext
    {
        public DbControle()
        {
            
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Contrato> Contrato { get; set; }
        public DbSet<StatusParcela> StatusParcela { get; set; }
        public DbSet<TipoContrato> TipoContrato { get; set; }
        public DbSet<ParcelaContrato> ParcelaContrato { get; set; }
        public DbSet<UnidadeNegocio> UnidadeNegocio { get; set; }
        public DbSet<TipoEndereco> TipoEndereco { get; set; }
        public DbSet<TipoPessoa> TipoPessoa { get; set; }
        public DbSet<CodigoServico> CodigoServico { get; set; }
        public DbSet<NotaFiscal> NotaFiscal { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Recebimento> Recebimento { get; set; }
        public DbSet<Funcionalidade> Funcionalidade { get; set; }
        public DbSet<PerfilFuncionalidade> PerfilFuncionalidade { get; set; }
        public DbSet<Estabelecimento> Estabelecimento { get; set; }
        public DbSet<Municipio> Municipio { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Imposto> Imposto { get; set; }
        public DbSet<calculoImposto> calculoImposto { get; set; }
        public DbSet<parametroDescricaoNota> parametroDescricaoNota { get; set; }
        public DbSet<Operacao> Operacao { get; set; }
        public DbSet<Holding> Holding { get; set; }
        public DbSet<UsuarioEstabelecimento> UsuarioEstabelecimento { get; set; }
        public DbSet<LogEvento> LogEvento { get; set; }
        public DbSet<BancoMovimento> BancoMovimento { get; set; }
        public DbSet<DocumentoPagarAprovacao> DocumentoPagarAprovacao { get; set; }
        public DbSet<DocumentoPagarDetalhe> DocumentoPagarDetalhe { get; set; }
        public DbSet<DocumentoPagarMestre> DocumentoPagarMestre { get; set; }
        public DbSet<OrigemMovimento> OrigemMovimento { get; set; }
        public DbSet<PerfilPagarAprovacao> PerfilPagarAprovacao { get; set; }
        public DbSet<PlanoDeConta> PlanoDeConta { get; set; }
        public DbSet<SaldoBancarioInicial> SaldoBancarioInicial { get; set; }
        public DbSet<SaldoBancarioReal> SaldoBancarioReal { get; set; }
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
        public DbSet<TipoMovimento> TipoMovimento { get; set; }
        public DbSet<ContratoItem> ContratoItem { get; set; }
        public DbSet<LogMudanca> LogMudanca { get; set; }
        public DbSet<Atividade> Atividade { get; set; }

        public DbSet<CategoriaPessoa> CategoriaPessoa { get; set; }
        public DbSet<Projeto> Projeto { get; set; }

        public DbSet<Despesa> Despesa { get; set; }
        public DbSet<LoteDespesa> LoteDespesa { get; set; }
        public DbSet<TipoDespesa> TipoDespesa { get; set; }
        public DbSet<DespesaPermitida> DespesaPermitida { get; set; }
        public DbSet<NotadeDebito> NotadeDebito { get; set; }
        public DbSet<Boleto> Boleto { get; set; }
        public DbSet<LanctoExtrato> LanctoExtrato { get; set; }
        public DbSet<OrdemVenda> OrdemVenda { get; set; }
        public DbSet<TipoRelacionamento> TipoRelacionamento  { get; set; }
        public DbSet<RelacionamentoPessoa> RelacionamentoPessoa { get; set; }
        public DbSet<ParcelaComissao> ParcelaComissao { get; set; }
        public DbSet<ExtratoComissao> ExtratoComissao { get; set; }
        public DbSet<ApoliceCertificado> ApoliceCertificado { get; set; }
        public DbSet<BancoMovimentoConfig> BancoMovimentoConfig { get; set; }
        public DbSet<GPS> GPS { get; set; }
        public DbSet<FGTS> FGTS { get; set; }
        public DbSet<DARF> DARF { get; set; }
        public DbSet<ContatoSite> ContatoSite { get; set; }
        public DbSet<BancoMovimentoLanctoExtrato> BancoMovimentoLanctoExtrato { get; set; }
        public DbSet<BancoMovimentoLanctoExtratoUsuario> BancoMovimentoLanctoExtratoUsuario { get; set; }
        public DbSet<BancoMovimentoLanctoExtratoMestre> BancoMovimentoLanctoExtratoMestre { get; set; }
        public DbSet<Atendimento> Atendimento { get; set; }
        public DbSet<AtendimentoCategoria> AtendimentoCategoria { get; set; }
        public DbSet<AtendimentoHistorico> AtendimentoHistorico { get; set; }
        public DbSet<AtendimentoStatus> AtendimentoStatus { get; set; }
        public DbSet<AtendimentoTipoHistorico> AtendimentoTipoHistorico { get; set; }
        public DbSet<ApontamentoDiario> ApontamentoDiario { get; set; }
        //public DbSet<MensagemSistema> MensagemSistema { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<FuncionarioSalario> FuncionarioSalario { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<FornecedorAcordo> FornecedorAcordo { get; set; }
        public DbSet<FolhaPagamento> FolhaPagamento { get; set; }
        public DbSet<FolhaPagamentoTipo> FolhaPagamentoTipo { get; set; }
        public DbSet<FuncionarioFuncao> FuncionarioFuncao { get; set; }
        public DbSet<CadastroCliente> Cadastro { get; set; }
        public DbSet<UsuarioAviso> UsuarioAviso { get; set; }

        public DbSet<DocumentoPagarArquivo> DocumentoPagarArquivo { get; set; }
        public DbSet<ContratoArquivo> ContratoArquivo { get; set; }

        public DbSet<CategoriaProfissional> CategoriaProfissional { get; set; }
        public DbSet<TaxaHora> TaxaHora { get; set; }
        public DbSet<AtividadeUsuario> AtividadeUsuario { get; set; }
        public DbSet<ProjetoUsuario> ProjetoUsuario { get; set; }

        public DbSet<TipoDataCalendario> TipoDataCalendario { get; set; }
        public DbSet<Calendario> Calendario { get; set; }
        public DbSet<TipoLoteDespesa> TipoLoteDespesa { get; set; }
        public DbSet<SituacaoLoteDespesa> SituacaoLoteDespesa { get; set; }
        public DbSet<SituacaoNotaDebito> SituacaoNotaDebito { get; set; }
        public DbSet<Pagamento> Pagamento { get; set; }
        public DbSet<RegistroPonto> RegistroPonto { get; set; }
        public DbSet<OpcaoTributariaSimples> OpcaoTributariaSimples { get; set; }
        public DbSet<SituacaoTributariaNota> SituacaoTributariaNota { get; set; }
        public DbSet<TipoRPS> TipoRPS { get; set; }

        public DbSet<ContaAssinatura> ContaAssinatura { get; set; }



        public DbSet<ComissaoSeguro> ComissaoSeguro { get; set; }
        public DbSet<EndossoSeguro> EndossoSeguro { get; set; }

        public DbSet<ExtratoSeguradora> ExtratoSeguradora { get; set; }
        public DbSet<ParcelaSeguro> ParcelaSeguro { get; set; }
        public DbSet<ProdutoSeguradora> ProdutoSeguradora { get; set; }
        public DbSet<RamoSeguro> RamoSeguro { get; set; }
        public DbSet<Regiao> Regiao { get; set; }

        public DbSet<Representante> Representante { get; set; }
        public DbSet<Sinistro> Sinistro { get; set; }

        public DbSet<Sucursal> Sucursal { get; set; }
        public DbSet<TipoCondicaoPagamentoPremio> TipoCondicaoPagamentoPremio { get; set; }
        public DbSet<TipoEndossoSeguradora> TipoEndossoSeguradora { get; set; }
        public DbSet<TipoPropostaSeguro> TipoPropostaSeguro { get; set; }
        public DbSet<TipoRepresentante> TipoRepresentante { get; set; }
        public DbSet<TipoSinistro> TipoSinistro { get; set; }
        public DbSet<TipoSituacaoSinistro> TipoSituacaoSinistro { get; set; }
        public DbSet<TipoStatusPropostaSeguro> TipoStatusPropostaSeguro { get; set; }

        public DbSet<ItemProdutoServico> ItemProdutoServico { get; set; }
        public DbSet<CategoriaItemProdutoServico> CategoriaItemProdutoServico { get; set; }
        public DbSet<PrecoItemProdutoServico> PrecoItemProdutoServico { get; set; }
        public DbSet<TabelaPrecoItemProdutoServico> TabelaPrecoItemProdutoServico { get; set; }




        public DbSet<PropostaApolice> PropostaApolice { get; set; }


        public DbSet<PropostaApoliceRamoItem> PropostaApoliceRamoItem { get; set; }

        public DbSet<AuxiliarContador> AuxiliarContador { get; set; }

        public DbSet<SistemaMenu> SistemaMenu { get; set; }

        public DbSet<SistemaDashBoard> SistemaDashBoard { get; set; }

        public DbSet<SistemaDashBoardFuncionalidade> SistemaDashBoardFuncionalidade { get; set; }

        public DbSet<GrupoRamoSeguro> GrupoRamoSeguro { get; set; }

        public DbSet<UsuarioFavorito> UsuarioFavorito { get; set; }

        public DbSet<UsuarioMaisAcessado> UsuarioMaisAcessado { get; set; }
        
        public DbSet<SistemaArquivo> SistemaArquivo { get; set; }

        public DbSet<CodigoServicoMunicipio> CodigoServicoMunicipio { get; set; }
        public DbSet<CodigoServicoEstabelecimento> CodigoServicoEstabelecimento { get; set; }

        public DbSet<PessoaContato> PessoaContato { get; set; }

        public DbSet<CadastroClienteSeguradora> CadastroClienteSeguradora { get; set; }

        public DbSet<SinistroHistorico> SinistroHistorico { get; set; }

        public DbSet<LogNFXMLPrincipal> LogNFXML { get; set; }

        public DbSet<LogNFXMLAlerta> LogNFXMLAlerta { get; set; }
        public DbSet<LogNFXMLErro> LogNFXMLErro { get; set; }


        public DbSet<SFCapa> SFCapa { get; set; }
        public DbSet<SFDetalhe> SFDetalhe { get; set; }
        public DbSet<DocumentoPagarParcela> DocumentoPagarParcela { get; set; }
        public DbSet<NotaFiscalPessoa> NotaFiscalPessoa { get; set; }

        public DbSet<EstoqueMovto> EstoqueMovto { get; set; }
        public DbSet<EstoqueMovtoItem> EstoqueMovtoItem { get; set; }

        public DbSet<NotaFiscalNFE> NotaFiscalNFE { get; set; }
        public DbSet<NotaFiscalNFEDuplicata> NotaFiscalNFEDuplicata { get; set; }
        public DbSet<NotaFiscalNFEEntrega> NotaFiscalNFEEntrega { get; set; }
        public DbSet<NotaFiscalNFEItem> NotaFiscalNFEItem { get; set; }
        public DbSet<NotaFiscalNFEReboque> NotaFiscalNFEReboque { get; set; }
        public DbSet<NotaFiscalNFEReferenciada> NotaFiscalNFEReferenciada { get; set; }
        public DbSet<NotaFiscalNFERetensao> NotaFiscalNFERetensao { get; set; }
        public DbSet<NotaFiscalNFERetirada> NotaFiscalNFERetirada { get; set; }
        public DbSet<NotaFiscalNFETransportadora> NotaFiscalNFETransportadora { get; set; }
        public DbSet<NotaFiscalNFEVolume> NotaFiscalNFEVolume { get; set; }

        public DbSet<CFOP> CFOP { get; set; }
        public DbSet<TipoBase> TipoBase { get; set; }

        public DbSet<CalculoImpostoTipoImposto> CalculoImpostoTipoImposto { get; set; }

        public DbSet<CST> CST { get; set; }
        public DbSet<NFeInformacao> NFeInformacao { get; set; }

        public DbSet<UrlSefaz> UrlSefaz { get; set; }
        public DbSet<UrlSefazUF> UrlSefazUF { get; set; }

        public DbSet<Loja> Loja { get; set; }

        public DbSet<LojaCaixa> LojaCaixa { get; set; }

        public DbSet<LojaOperador> LojaOperador { get; set; }

        public DbSet<LojaTipoRecebimentoCaixa> LojaTipoRecebimentoCaixa { get; set; }

        public DbSet<LojaFechamento> LojaFechamento { get; set; }


        public DbSet<LojaFechamentoCC> LojaFechamentoCC { get; set; }

        public DbSet<ContaContabil> ContaContabil { get; set; }

        public DbSet<ContaContabilCategoriaDespesa> ContaContabilCategoriaDespesa { get; set; }

        public DbSet<EmpresaContaContabil> EmpresaContaContabil { get; set; }

        public DbSet<PessoaContaContabil> PessoaContaContabil { get; set; }

        public DbSet<LancamentoContabil> LancamentoContabil { get; set; }

        public DbSet<EstabelecimentoCodigoLanctoContabil> EstabelecimentoCodigoLanctoContabil { get; set; }

        public DbSet<CPAGEstimativa> CPAGEstimativa { get; set; }

        public DbSet<LancamentoContabilDetalhe> LancamentoContabilDetalhe { get; set; }

        public DbSet<LojaTipoRecebimentoCaixaVigencia> LojaTipoRecebimentoCaixaVigencia { get; set; }

        public DbSet<SistemaPrincipal> SistemaPrincipal { get; set; }
        public DbSet<SistemaPrincipalPerfil> SistemaPrincipalPerfil { get; set; }

        public DbSet<TransfConta> TransfConta { get; set; }
        public DbSet<ContratoItemPedido> ContratoItemPedido { get; set; }
        public DbSet<ContratoItemUnidadeNegocio> ContratoItemUnidadeNegocio { get; set; }

        public DbSet<NotaFiscalNFEFormaPagamento> NotaFiscalNFEFormaPagamento { get; set; }
        public DbSet<DocumentoPagarProjeto> DocumentoPagarProjeto { get; set; }

        
        public DbSet<IBPT> IBTP { get; set; }
        public DbSet<DeParaMestre> DeParaMestre { get; set; }
        public DbSet<DeParaItem> DeParaItem { get; set; }
        public DbSet<NotaFiscalOutroItem> NotaFiscalOutroItem { get; set; }
        public DbSet<BoletoArquivoHistorico> BoletoArquivoHistorico { get; set; }

        public DbSet<LayoutTemplate> LayoutTemplate { get; set; }

        public DbSet<SaldoContabil> SaldoContabil { get; set; }
        public DbSet<SaldoContabilDetalhe> SaldoContabilDetalhe { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}