using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Web.Regras;
using SoftFin.Utils;
using System;




using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using SoftFin.NFSe.DTO;
using System.Web.Http.Description;
using System.Threading;

namespace SoftFin.API.jarvirs
{
    
    public class NotaAutomaticaController : BaseApi
    {
        // GET api/<controller>
        public void Get()
        {

            DateTime primeiroDia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            DbControle db = new DbControle();


            var parcelas = new ParcelaContrato().ObterTodosEstabAutomaticoEmAberto(primeiroDia, db).OrderBy(p => p.ContratoItem.Contrato.estabelecimento_id);


            foreach (var item in parcelas)
            {
                var estab = new Estabelecimento().ObterPorId(item.ContratoItem.Contrato.estabelecimento_id, _paramBase);
                var ov = new OrdemVenda().ObterPorParcelaId(item.id, estab.id, db);
                var titulo = new StringBuilder();
                var emailaviso = estab.emailNotificacoes;

                _paramBase.empresa_id = estab.Empresa_id;
                _paramBase.estab_id = estab.id;
                _paramBase.municipio_id = estab.Municipio_id;
                _paramBase.usuario_id = new Usuario().ObterPorNomeAtivo("JARVIS", db).id;
                

                if (emailaviso == null)
                    continue;

                var corpoemail = new StringBuilder();

                if (estab.autorizacaoFaturamento == true)
                {
                    if (ov.usuarioAutorizador_id == null)
                    {
                        EmailNaoAutorizado(item, estab, ov, titulo, emailaviso, corpoemail);
                        continue;
                    }
                }
                // Calcula Nota
                var NotaFiscalCalculada = new NotaFiscal();



                new NotaFiscalCalculos().Calcula(
                    item.CodigoServicoEstabelecimento.CodigoServicoMunicipio.codigo,
                    item.data,
                    item.banco_id.Value,
                    item.operacao_id.Value,
                    item.valor,
                    item.ContratoItem.unidadeNegocio_ID,
                    item.ContratoItem.Contrato.pessoas_ID,
                    ov.id,
                    estab,
                    NotaFiscalCalculada,
                    _paramBase
                    );


                // Salva Nota
                ModelState.Clear();
                base.TryValidateModel(NotaFiscalCalculada);


                if (!ModelState.IsValid)
                {
                    EmailModeloInvalido(item, ov, titulo, corpoemail,emailaviso,estab);
                    continue;
                }

                base.TryValidateModel(NotaFiscalCalculada.NotaFiscalPessoaTomador);


                if (!ModelState.IsValid)
                {
                    EmailModeloInvalido(item, ov, titulo, corpoemail, emailaviso, estab);
                    continue;
                }

                
                try
                {
                    var dbt = new DbControle();
                    using (var dbcxtransaction = dbt.Database.BeginTransaction())
                    {
                        _paramBase.estab_id = estab.id;

                        if (NotaFiscalCalculada.Incluir(NotaFiscalCalculada, _paramBase, dbt))
                        {
                            var pct = new ParcelaContrato().ObterPorId(item.id,dbt, _paramBase);
                            pct.statusParcela_ID = StatusParcela.SituacaoEmitida();
                            pct.AlteraParcela(pct, _paramBase, dbt);

                            var ovt = new OrdemVenda().ObterPorParcelaId(pct.id,_paramBase.estab_id,dbt);
                            ovt.statusParcela_ID = StatusParcela.SituacaoEmitida();
                            ovt.Alterar(ovt, _paramBase, dbt);
                            dbt.SaveChanges();


                            if (!string.IsNullOrEmpty(estab.senhaCertificado))
                            {
                                NotaFiscalCalculada.municipio = new Municipio().ObterPorId(NotaFiscalCalculada.municipio_id.Value, dbt);
                                NotaFiscalCalculada.Operacao = item.Operacao;
                                // Converte Nota
                                DTORetornoNFEs resultado = EnvioPorServico(estab, NotaFiscalCalculada);

                                EmailSucessoEnviadoPorServico(item, estab, ov, titulo, emailaviso, corpoemail, NotaFiscalCalculada, resultado);

                                if (resultado.Cabecalho.Sucesso.ToLower() == "false")
                                {
                                    dbcxtransaction.Rollback();
                                    continue;
                                }
                                else
                                {
                                    dbcxtransaction.Commit();
                                }
                            }
                            else
                            {
                                EmailSucesso(item, ov, titulo, corpoemail, NotaFiscalCalculada, emailaviso, estab);
                                dbcxtransaction.Commit();
                                continue;
                            }


                            
                        }
                        else
                        {
                            dbcxtransaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    EmailExcessao (item, estab, ov, titulo, emailaviso, corpoemail, ex);
                    continue;
                }

                try
                {
                    Thread.Sleep(10000);
                    var retorno = Confirmacao(estab, NotaFiscalCalculada, _paramBase);
                    if (retorno.Cabecalho.Sucesso.ToLower() == "true")
                    {
                        EmailSucessoNotaAtualizadao(item, ov, titulo, corpoemail, NotaFiscalCalculada, emailaviso, estab);
                    }
                }
                catch (Exception ex)
                {
                    EmailExcessao(item, estab, ov, titulo, emailaviso, corpoemail, ex);
                    continue;
                }

                // tem Certificado


            }
        }

        private DTORetornoNFEs EnvioPorServico(Estabelecimento estab, NotaFiscal NotaFiscalCalculada)
        {
            var listaNF = new List<NotaFiscal>();
            listaNF.Add(NotaFiscalCalculada);
            var objPedidoEnvioLoteRPS = new DTONotaFiscal();
            new Conversao().ConverterNFEs(objPedidoEnvioLoteRPS, estab, listaNF);

            // Envia Nota a Prefeitura
            string caminhoArquivo;
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
            new NotaFiscalCalculos().ObtemCertificadoX509(estab, out caminhoArquivo, out cert);

            var arquivoxml = CriaPastaeNomeXML(estab);
            var service = new SoftFin.NFSe.Business.SFEnvioLoteRPS();
            var resultado = service.Execute(objPedidoEnvioLoteRPS, cert, arquivoxml);
            return resultado;
        }


        private DTORetornoNFEs Confirmacao(Estabelecimento estab, NotaFiscal NotaFiscalCalculada, ParamBase pb)
        {
            DbControle db = new DbControle();
            NotaFiscal objNF;
            DTORetornoNFEs resultado;
            new NotaFiscalCalculos().AtualizaNFBussines(NotaFiscalCalculada.id, pb, null,  db, out objNF, out resultado);

            if (resultado.Cabecalho.Sucesso.ToLower() == "true")
            {
                new NotaFiscalCalculos().BaixaNF(objNF,
                    DateTime.Parse(resultado.NFe.First().DataEmissaoNFe),
                    resultado.NFe.First().ChaveNFe.CodigoVerificacao,
                    int.Parse(resultado.NFe.First().ChaveNFe.NumeroNFe),
                    resultado.NFe.First().StatusNFe,
                    db, pb);
            }
            NotaFiscalCalculada = objNF;
            return resultado;
        }
        private void EmailSucessoEnviadoPorServico(ParcelaContrato item, Estabelecimento estab, OrdemVenda ov, StringBuilder titulo, string emailaviso, StringBuilder corpoemail, NotaFiscal NotaFiscalCalculada, DTORetornoNFEs resultado)
        {
            // Gera Email com sucesso ou erros
            if (resultado.Cabecalho.Sucesso.ToLower() == "false")
            {
                titulo.Append("SoftFin - NFS-e Automática - Nota não aceita pela prefeitura., a seguir os erros:.");
            }
            else
            {
                if (resultado.Alerta.Count() == 0)
                    titulo.Append("SoftFin - NFS-e Automática - Nota aceita pela prefeitura.");
                else
                    titulo.Append("SoftFin - NFS-e Automática - Nota aceita pela prefeitura, a seguir os alertas:");
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
            corpoemail.AppendLine(NotaFiscalCalculada.numeroRps + "-" + NotaFiscalCalculada.serieRps);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(item.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, estab);
        }

        private void EmailSucesso(ParcelaContrato item, OrdemVenda ov, StringBuilder titulo, StringBuilder corpoemail, NotaFiscal NotaFiscalCalculada, string emailaviso, Estabelecimento estab)
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
            corpoemail.AppendLine(NotaFiscalCalculada.numeroRps + "-" + NotaFiscalCalculada.serieRps);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(item.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            titulo.Append("SoftFin - NFS-e Automática - Nota Gerada com sucesso!");
            EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, estab);
        }


        private void EmailSucessoNotaAtualizadao(ParcelaContrato item, OrdemVenda ov, StringBuilder titulo, StringBuilder corpoemail, NotaFiscal NotaFiscalCalculada, string emailaviso, Estabelecimento estab)
        {
            corpoemail.AppendLine("<tr>");

            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>Numero Nota</b>");
            corpoemail.AppendLine("    </td>");

            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>Verificação</b>");
            corpoemail.AppendLine("    </td>");

            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine("<b>Data Emissão</b>");
            corpoemail.AppendLine("    </td>");

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
            corpoemail.AppendLine(NotaFiscalCalculada.numeroNfse.ToString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(NotaFiscalCalculada.codigoVerificacao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(NotaFiscalCalculada.dataEmissaoNfse.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(NotaFiscalCalculada.numeroRps + "-" + NotaFiscalCalculada.serieRps);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(item.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            titulo.Append("SoftFin - NFS-e Automática - Nota Gerada com sucesso!");
            EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, estab);
        }

        private void EmailNaoAutorizado(ParcelaContrato item, Estabelecimento estab, OrdemVenda ov, StringBuilder titulo, string emailaviso, StringBuilder corpoemail)
        {
            titulo.Append("SoftFin - NFS-e Automática - Ordem de Venda não aprovada");
            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td>");
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(item.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");
            EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, estab);
        }

        private void EmailExcessao(ParcelaContrato item, Estabelecimento estab, OrdemVenda ov, StringBuilder titulo, string emailaviso, StringBuilder corpoemail, Exception ex)
        {
            titulo.Append("SoftFin - NFS-e Automática - Nota não pode ser gerada avise o suporte!");

            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td colspan='5'>");
            corpoemail.AppendLine("     <b>Erros na geração</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");


            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td colspan='5'>");
            corpoemail.AppendLine(ex.Message);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            corpoemail.AppendLine("<tr>");
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
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(item.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");

            EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, estab);
        }

        private void EmailModeloInvalido(ParcelaContrato item, OrdemVenda ov, StringBuilder titulo, StringBuilder corpoemail, string emailaviso, Estabelecimento estab)
        {
            titulo.Append("SoftFin - NFS-e Automática - Nota não pode ser gerada, verifique os erros!");

            corpoemail.AppendLine("<tr>");
            corpoemail.AppendLine("    <td colspan='5'>");
            corpoemail.AppendLine("     <b>Erros na geração</b>");
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");



            foreach (ModelState modelState in ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    corpoemail.AppendLine("<tr>");
                    corpoemail.AppendLine("    <td colspan='5'>");
                    corpoemail.AppendLine(error.ErrorMessage);
                    corpoemail.AppendLine("    </td>");
                    corpoemail.AppendLine("</tr>");

                }
            }


            corpoemail.AppendLine("<tr>");
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
            corpoemail.AppendLine(ov.data.ToShortDateString());
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.descricao);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='padding-left: 5px;'>");
            corpoemail.AppendLine(item.ContratoItem.Contrato.Pessoa.nome);
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("    <td style='text-align:right'>");
            corpoemail.AppendLine(item.valor.ToString("n"));
            corpoemail.AppendLine("    </td>");
            corpoemail.AppendLine("</tr>");
            EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, estab);

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



        private void EnviaEmail(string titulo, string corpo, string emailaviso, Estabelecimento estab)
        {
            var email = new Email();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            var arquivohmtl = Path.Combine(path, "Email02.html");
            string readText = File.ReadAllText(arquivohmtl);
            readText = readText.Replace("{Titulo}", titulo);
            readText = readText.Replace("{Corpo}", corpo);
            readText = readText.Replace("{nomeestab}", estab.NomeCompleto);
            email.EnviarMensagem(emailaviso, titulo, readText, true);
        }


    }
}