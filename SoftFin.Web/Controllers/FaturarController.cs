using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Regras;
using System.Text;
using System.IO;
using SoftFin.Utils;
using SoftFin.NFSe.DTO;

namespace SoftFin.Web.Controllers
{
    public class FaturarController : BaseController
    {
        #region Public
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ObterOrdemVendaAberto()
        {
            var Listas = PesquisaNFse();


            return Json(
                            Listas.Select(p => new
                                {
                                    data = p.data.ToString("o"),
                                    dataAutorizacao = (p.dataAutorizacao == null) ? "" : p.dataAutorizacao.Value.ToString("o"),
                                    p.estabelecimento_id,
                                    p.id,
                                    p.itemProdutoServico_ID,
                                    p.Numero,
                                    p.valor,
                                    nome = p.Pessoa.nome,
                                    unidadeNegocioid =  p.unidadeNegocio_ID.ToString(),
                                    unidade = p.UnidadeNegocio.unidade,
                                    descricaoparcela = (p.parcelaContrato_ID == null)? "":  p.ParcelaContrato.descricao,
                                    pedido = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.ContratoItem.pedido,
                                    contrato = (p.parcelaContrato_ID == null) ? "" : (p.ParcelaContrato.ContratoItem.Contrato.contrato),
                                    descricaocontrato = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.ContratoItem.Contrato.descricao,
                                    numeroparcela = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.parcela.ToString(),
                                    codigoservico = (p.parcelaContrato_ID == null) ? "" : (
                                            p.ParcelaContrato.codigoServicoEstabelecimento_id == null ? "" : 
                                            p.ParcelaContrato.CodigoServicoEstabelecimento.CodigoServicoMunicipio.codigo),
                                    bancoid = (p.parcelaContrato_ID == null) ? "" :((p.ParcelaContrato.banco_id == null)? "": p.ParcelaContrato.banco_id.ToString()), 
                                    banco = (p.parcelaContrato_ID == null) ? "" : 
                                            (
                                                (p.ParcelaContrato.banco_id == null)
                                                ? "" : p.ParcelaContrato.banco.nomeBanco
                                                + " " +
                                                p.ParcelaContrato.banco.agencia 
                                                + " " +
                                                p.ParcelaContrato.banco.contaCorrente 
                                                + "-" +
                                                p.ParcelaContrato.banco.contaCorrenteDigito
                                            ),
                                    operacaoid = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.operacao_id == null) ? "" : p.ParcelaContrato.operacao_id.ToString()), 
                                    operacao = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.operacao_id == null)
                                                ? "" : p.ParcelaContrato.Operacao.descricao),
                                    pessoaid = p.pessoas_ID
                                    
                                }
                            )
                            , JsonRequestBehavior.AllowGet
                        )
                 ;

        }
        [HttpPost]
        public JsonResult ObterOperacoes()
        {
            var objs = new Operacao().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id.ToString(),
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObterCodigoServicos()
        {
            var objs = new CodigoServicoEstabelecimento().ObterTodos(_paramBase).Where(p => p.CodigoServicoMunicipio.municipio_id == _estabobj.Municipio_id) ;


            return Json(objs.Select(p => new
            {
                Value = p.CodigoServicoMunicipio.codigo,
                Text = p.CodigoServicoMunicipio.codigo + " - " + p.CodigoServicoMunicipio.descricao
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //Calcula Nota
        public JsonResult CalculaNotaTela(
            string codigoServico,
            DateTime data,
            int bancoid,
            int operacaoid,
            decimal valor,
            int unidadeNegocioid,
            int ovid,
            int pessoaid)
        {

            try
            {

                var nf = new NotaFiscal();

                new NotaFiscalCalculos().Calcula(codigoServico,
                                                data,
                                                bancoid,
                                                operacaoid,
                                                valor,
                                                unidadeNegocioid,
                                                pessoaid,
                                                ovid,
                                                _estabobj, nf, _paramBase);

                return Json(
                    new
                    {
                        CDStatus = "OK", 
                        obj = new { nf.aliquotaINSS,
                            nf.ordemVenda_id,
                            nf.aliquotaIrrf,
                            nf.aliquotaISS,
                            
                            nf.banco_id,
                            nf.basedeCalculo,
                            nf.codigoServico,
                            nf.codigoVerificacao,
                            nf.cofinsRetida,
                            nf.creditoImposto,
                            nf.csllRetida,
                            dataEmissaoNfse = (nf.dataEmissaoNfse== null) ? "" : nf.dataEmissaoNfse.ToString("o"),
                            dataEmissaoRps = (nf.dataEmissaoRps == null) ? "" : nf.dataEmissaoRps.ToString("o"),
                            dataVencimentoNfse = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
                            DataVencimentoOriginal = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
                            nf.discriminacaoServico,
                            nf.entradaSaida,
                            nf.estabelecimento_id,
                            nf.id,
                            nf.irrf,
                            nf.municipio_id,
                            nf.numeroNfse,
                            nf.numeroRps,
                            nf.operacao_id,
                            nf.pisRetido,
                            nf.serieRps,
                            nf.situacaoPrefeitura_id,
                            nf.SituacaoRecebimento,
                            nf.situacaoRps,
                            nf.tipoRps,
                            nf.valorDeducoes,
                            nf.valorINSS,
                            nf.valorISS,
                            nf.valorLiquido,
                            nf.valorNfse, 
                            nf.percentualCargaTributaria,
                            nf.valorCargaTributaria,
                            nf.fonteCargaTributaria,
                            nf.codigoCEI,
                            nf.matriculaObra,
                            NotaFiscalPessoaTomador = new
                            {
                                nf.NotaFiscalPessoaTomador.cep,
                                nf.NotaFiscalPessoaTomador.cidade,
                                cnpjCpf = nf.NotaFiscalPessoaTomador.cnpjCpf.Replace("-", "").Replace("/", "").Replace(".", ""),
                                nf.NotaFiscalPessoaTomador.email,
                                nf.NotaFiscalPessoaTomador.endereco,
                                nf.NotaFiscalPessoaTomador.indicadorCnpjCpf,
                                nf.NotaFiscalPessoaTomador.inscricaoEstadual,
                                nf.NotaFiscalPessoaTomador.inscricaoMunicipal,
                                nf.NotaFiscalPessoaTomador.numero,
                                nf.NotaFiscalPessoaTomador.razao,
                                nf.NotaFiscalPessoaTomador.tipoEndereco,
                                nf.NotaFiscalPessoaTomador.uf,
                                nf.NotaFiscalPessoaTomador.complemento,
                                nf.NotaFiscalPessoaTomador.bairro
                            }
                        },
                            
                        }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        
        
        //Salva Nota
        [HttpPost]
        public JsonResult Salvar(NotaFiscal notafiscal)
        {
            bool notaGerada = false;


            try
            {
                var erroscpag = new List<string>();
                var titulo = new StringBuilder();
                var emailaviso = _estabobj.emailNotificacoes;
                var corpoemail = new StringBuilder();


                var erros = notafiscal.Validar(ModelState);

                if (string.IsNullOrEmpty(notafiscal.NotaFiscalPessoaTomador.inscricaoEstadual))
                {
                    erros.Add("Informe a inscrição estadual..");
                }


                if (erros.Count() > 0)
                {
                    return Json(new { CDMessage = "NOK", DSMessage = "Campos Inválidos", Erros = erros }, JsonRequestBehavior.AllowGet);
                }

                if (notafiscal.estabelecimento_id != _estab)
                {
                    return Json(new { CDMessage = "NOK", DSMessage = "Estabelecimento inválido, saia do sistema e entre novamente (troca entre abas do navegador)", Erros = erros }, JsonRequestBehavior.AllowGet);
                }

                DbControle db = new DbControle();
                var ov = new OrdemVenda().ObterPorId(notafiscal.ordemVenda_id.Value, db);

                var pc = new ParcelaContrato();

                if (ov.parcelaContrato_ID != null)
                {
                    pc = new ParcelaContrato().ObterPorId(ov.parcelaContrato_ID.Value, db, _paramBase);
                }

                // Inicio Lançamento Contabil
                var idCredito = 0;
                var idDebito = 0;
                var ccLC = new LancamentoContabil();
                var ccDebito = new LancamentoContabilDetalhe();
                var ccCredito = new LancamentoContabilDetalhe();

                var pcf = new PessoaContaContabil().ObterPorPessoa(ov.pessoas_ID, db);

                if (pcf != null)
                {
                    if (pcf.contaContabilReceberPadrao_id != null)
                    {
                        idDebito = pcf.contaContabilReceberPadrao_id.Value;
                    }
                }

                var ecf = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);
                if (ecf != null)
                {
                    idCredito = ecf.ContaContabilNFServico_id;
                    if (idDebito == 0)
                        idDebito = ecf.ContaContabilRecebimento_id;
                }

                if (idCredito != 0 && idDebito != 0)
                {
                    //var estabelecimentoCodigoLanctoContabil = new EstabelecimentoCodigoLanctoContabil().ObterNovoCodigoContabil();


                    ccLC.data = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                    ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                    ccLC.estabelecimento_id = _paramBase.estab_id;
                    ccLC.historico = ov.descricao;
                    ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                    ccLC.origemmovimento_id = new OrigemMovimento().TipoFaturamento(_paramBase);
                    ccLC.UnidadeNegocio_ID = ov.unidadeNegocio_ID;

                    ccDebito.contaContabil_id = idDebito;
                    ccDebito.DebitoCredito = "D";
                    ccDebito.valor = ov.valor;
                    
                    ccCredito.contaContabil_id = idCredito;
                    ccCredito.DebitoCredito = "C";
                    ccCredito.valor = ov.valor;
                }
                //Fim Lançamento Contabil


                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    Pessoa pessoa = new Pessoa().ObterPorId(ov.pessoas_ID,_paramBase,db);

                    pessoa.cep = notafiscal.NotaFiscalPessoaTomador.cep;
                    pessoa.cidade = notafiscal.NotaFiscalPessoaTomador.cidade;
                    pessoa.cnpj = notafiscal.NotaFiscalPessoaTomador.cnpjCpf.Replace("-", "").Replace("/", "").Replace(".", "");
                    pessoa.eMail = notafiscal.NotaFiscalPessoaTomador.email;
                    pessoa.endereco = notafiscal.NotaFiscalPessoaTomador.endereco;
                    pessoa.inscricao = notafiscal.NotaFiscalPessoaTomador.inscricaoEstadual;
                    pessoa.ccm = notafiscal.NotaFiscalPessoaTomador.inscricaoMunicipal;
                    pessoa.numero = notafiscal.NotaFiscalPessoaTomador.numero;
                    pessoa.razao = notafiscal.NotaFiscalPessoaTomador.razao;
                    pessoa.uf = notafiscal.NotaFiscalPessoaTomador.uf;
                    pessoa.complemento = notafiscal.NotaFiscalPessoaTomador.complemento;
                    pessoa.bairro = notafiscal.NotaFiscalPessoaTomador.bairro;

                    pessoa.Alterar(_paramBase, db);

                    notafiscal.estabelecimento_id = _estab;
                    notafiscal.dataEmissaoRps = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                    notafiscal.situacaoPrefeitura_id = 1;
                    notafiscal.entradaSaida = "S";
                    notafiscal.usuarioinclusaoid = _usuarioobj.id;
                    notafiscal.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(); ;
                    notafiscal.dataVencimentoNfse = SoftFin.Utils.UtilSoftFin.TiraHora(notafiscal.dataVencimentoNfse);
                    notafiscal.DataVencimentoOriginal = notafiscal.dataVencimentoNfse;
                    if (notafiscal.Incluir(_paramBase, db))
                    {
                        if (ov.parcelaContrato_ID != null)
                        {
                            pc.statusParcela_ID = StatusParcela.SituacaoEmitida();
                        }
                        ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
                        
                        //Inicio Lançamento Contabil
                        if (idCredito != 0 && idDebito != 0)
                        {
                            var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                            ccLC.codigoLancamento = numeroLcto;
                            ccLC.notafiscal_id = notafiscal.id;
                            ccLC.Incluir(_paramBase, db);
                            ccDebito.lancamentoContabil_id = ccLC.id;
                            ccDebito.Incluir(_paramBase, db);
                            ccCredito.lancamentoContabil_id = ccLC.id;
                            ccCredito.Incluir(_paramBase, db);
                        }
                        //Fim Lançamento Contabil

                        db.SaveChanges();
                        dbcxtransaction.Commit();
                        notaGerada = true;
                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        return Json(new { CDStatus = "NOK", DSMessage = "não foi possivel incluir sua nota" }, JsonRequestBehavior.AllowGet);
                    }
                }
                SoftFin.NFSe.DTO.DTORetornoNFEs resultado = new SoftFin.NFSe.DTO.DTORetornoNFEs();

                resultado.Cabecalho = new SoftFin.NFSe.DTO.tpCabecalho();

                if (!string.IsNullOrEmpty(_estabobj.senhaCertificado))
                {
                    // Converte Nota
                    var listaNF = new List<NotaFiscal>();
                    listaNF.Add(notafiscal.ObterPorId(notafiscal.id));
                    var objPedidoEnvioLoteRPS = new DTONotaFiscal();
                    new Conversao().ConverterNFEs(objPedidoEnvioLoteRPS, _estabobj, listaNF);

                    // Envia Nota a Prefeitura
                    string caminhoArquivo;
                    System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
                    ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);

                    var arquivoxml = CriaPastaeNomeXML(_estabobj);
                    var service = new SoftFin.NFSe.Business.SFEnvioLoteRPS();
                    resultado = service.Execute(objPedidoEnvioLoteRPS, cert, arquivoxml);

                    // Gera Email com sucesso ou erros
                    GeraEmailComEnvio(notafiscal, titulo, corpoemail, pc, resultado);
                }
                else
                {
                    if (emailaviso != null)
                    {
                        GeraEmailSemCertificado(notafiscal, titulo, corpoemail, ov, pc);
                    }
                }

                if (emailaviso != null)
                {
                    EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, _estabobj);
                }


                if (resultado.Erro.Count() > 0)
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com sucesso, mas não foi aceito pela prefeitura", AlertasPrefeitura = resultado.Alerta, ErrosPrefeitura = resultado.Erro }, JsonRequestBehavior.AllowGet);
                }
                else if (resultado.Alerta.Count() > 0)
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com sucesso, aceita na prefeitura (aguarde geração de número de nf), com o seguintes alertas", AlertasPrefeitura = resultado.Alerta, ErrosPrefeitura = resultado.Erro }, JsonRequestBehavior.AllowGet);
                }
                else if (!string.IsNullOrEmpty(_estabobj.senhaCertificado))
                {
                    if (resultado.Cabecalho.Sucesso.ToUpper() == "TRUE")
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com sucesso, aceita na prefeitura (aguarde geração de número de nf)", AlertasPrefeitura = resultado.Alerta, ErrosPrefeitura = resultado.Erro }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com sucesso, não aceita na prefeitura", AlertasPrefeitura = resultado.Alerta, ErrosPrefeitura = resultado.Erro }, JsonRequestBehavior.AllowGet);
                    }
                }
                else 
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com sucesso", AlertasPrefeitura = resultado.Alerta, ErrosPrefeitura = resultado.Erro }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                if (notaGerada)
                    return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com suceso, mas ocorreram erros: " + ex.Message }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Private
        private List<OrdemVenda> PesquisaNFse()
        {
            var obj = new OrdemVenda();
            var date = DateTime.Now.AddMonths(1);
            IEnumerable<OrdemVenda> lista;

            if (_estabobj.autorizacaoFaturamento == null)
                _estabobj.autorizacaoFaturamento = false;

            //if ((contrato == null || contrato == string.Empty) & (Request.QueryString["data"] == null))
            //{
                if (_estabobj.autorizacaoFaturamento.Value)
                    lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.data <= date );
                else
                    lista = obj.ObterTodosPendentes(_paramBase).Where(x => x.data <= date);
            //}
            //else
            //{
            //    if (data == null || data == null)
            //    {
            //        if (contrato == null || contrato == string.Empty)
            //        {
            //            if (_estabobj.autorizacaoFaturamento.Value)
            //                lista = obj.ObterTodosPendentesAutorizadas();
            //            else
            //                lista = obj.ObterTodosPendentes();

            //        }
            //        else
            //        {
            //            if (_estabobj.autorizacaoFaturamento.Value)
            //                lista = obj.ObterTodosPendentesAutorizadas().Where(x => x.ParcelaContrato.ContratoItem.Contrato.contrato == contrato);
            //            else
            //                lista = obj.ObterTodosPendentes().Where(x => x.ParcelaContrato.ContratoItem.Contrato.contrato == contrato);

            //        }
            //    }
            //    else
            //    {
            //        if (contrato == null || contrato == string.Empty)
            //        {
            //            date = Convert.ToDateTime(data);
            //            date = DateTime.Now.AddMonths(1);
            //            if (_estabobj.autorizacaoFaturamento.Value)
            //                lista = obj.ObterTodosPendentesAutorizadas().Where(x => x.data <= date);
            //            else
            //                lista = obj.ObterTodosPendentes().Where(x => x.data <= date);

            //        }
            //        else
            //        {
            //            date = Convert.ToDateTime(data);
            //            date = DateTime.Now.AddMonths(1);
            //            if (_estabobj.autorizacaoFaturamento.Value)
            //                lista = obj.ObterTodosPendentesAutorizadas().Where(x => x.ParcelaContrato.ContratoItem.Contrato.contrato == contrato & x.data <= date);
            //            else
            //                lista = obj.ObterTodosPendentes().Where(x => x.ParcelaContrato.ContratoItem.Contrato.contrato == contrato & x.data <= date);

            //        }
            //    }
            //}
            return lista.OrderBy(p => p.data).ToList();
        }
        private static void GeraEmailComEnvio(NotaFiscal notafiscal, StringBuilder titulo, StringBuilder corpoemail, ParcelaContrato pc, SoftFin.NFSe.DTO.DTORetornoNFEs resultado)
        {
            if (resultado.Cabecalho.Sucesso.ToLower() == "false")
            {
                titulo.Append("SoftFin - NFS-e Manual - Nota não aceita pela prefeitura., a seguir os erros:.");
            }
            else
            {
                if (resultado.Alerta.Count() == 0)
                    titulo.Append("SoftFin - NFS-e Manual - Nota aceita pela prefeitura.");
                else
                    titulo.Append("SoftFin - NFS-e Manual - Nota aceita pela prefeitura, a seguir os alertas:");
            }


            if (resultado.Alerta.Count() > 0)
            {
                corpoemail.AppendLine("<tr>");
                corpoemail.AppendLine("    <td colspan='5'>");
                corpoemail.AppendLine("     <b>Alertas</b>");
                corpoemail.AppendLine("    </td>");
                corpoemail.AppendLine("</tr>");
            }

            foreach (var item2 in resultado.Alerta)
            {
                corpoemail.AppendLine("<tr>");
                corpoemail.AppendLine("    <td colspan='5'>");
                corpoemail.AppendLine(item2.Codigo + " - " + item2.Descricao);
                corpoemail.AppendLine("    </td>");
                corpoemail.AppendLine("</tr>");
            }

            if (resultado.Erro.Count() > 0)
            {
                corpoemail.AppendLine("<tr>");
                corpoemail.AppendLine("    <td colspan='5'>");
                corpoemail.AppendLine("     <b>Erros</b>");
                corpoemail.AppendLine("    </td>");
                corpoemail.AppendLine("</tr>");
            }


            foreach (var item2 in resultado.Erro)
            {
                corpoemail.AppendLine("<tr>");
                corpoemail.AppendLine("    <td colspan='5'>");
                corpoemail.AppendLine(item2.Codigo + " - " + item2.Descricao);
                corpoemail.AppendLine("    </td>");
                corpoemail.AppendLine("</tr>");
            }


            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>RPS</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>Data</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine("<b>Descrição</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine("<b>Tomador</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine("<b>Valor</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(notafiscal.numeroRps + "-" + notafiscal.serieRps);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(notafiscal.dataEmissaoRps.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(pc.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            if (pc.ContratoItem != null)
            {
                corpoemail.AppendLine(pc.ContratoItem.Contrato.Pessoa.nome);
            }
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(pc.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");
        }
        private static void GeraEmailSemCertificado(NotaFiscal notafiscal, StringBuilder titulo, StringBuilder corpoemail, OrdemVenda ov, ParcelaContrato pc)
        {
            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>RPS</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>Data</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine("<b>Descrição</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine("<b>Tomador</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine("<b>Valor</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(notafiscal.numeroRps + "-" + notafiscal.serieRps);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(pc.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(notafiscal.NotaFiscalPessoaTomador.razao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(notafiscal.valorNfse.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            titulo.Append("SoftFin - NFS-e Manual - Nota Gerada com sucesso!");
        }
        private void ObtemCertificadoX509(Estabelecimento objEstab, out string caminhoArquivo, out System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CertTMP/");
            Directory.CreateDirectory(uploadPath);

            var nomearquivonovo = Guid.NewGuid().ToString();

            caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

            cert = UtilSoftFin.BuscaCert(objEstab.id, objEstab.senhaCertificado, caminhoArquivo, objEstab.CNPJ);
        }
        private void EnviaEmail(string titulo, string corpo, string emailaviso, Estabelecimento estab)
        {
            var email = new Email();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            var arquivohmtl = Path.Combine(path, "Email02.html");
            string readText = System.IO.File.ReadAllText(arquivohmtl);
            readText = readText.Replace("{Titulo}", titulo);
            readText = readText.Replace("{Corpo}", corpo);
            readText = readText.Replace("{nomeestab}", estab.NomeCompleto);
            email.EnviarMensagem(emailaviso, titulo, readText, true);
        }
        private string CriaPastaeNomeXML(Estabelecimento estab)
        {
            var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/OFXTemp/");
            Directory.CreateDirectory(uploadPath);
            var arquivoxml = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/OFXTemp/"), "NFEnvioAutomatico" + estab.id + ".xml");

            try
            {
                System.IO.File.Delete(arquivoxml);
            }
            catch
            {
            }
            return arquivoxml;
        }
        #endregion
    }
}
