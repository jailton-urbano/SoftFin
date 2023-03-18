//using BoletoNet;
using BoletoNet;
using SelectPdf;
using SoftFin.Utils;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class BoletoController : BaseController
    {
        // GET: Boleto
        public ActionResult Index()
        {
            return View();
        }

        public class Filtro
        {
            public DateTime dataIni { get; set; }
            public DateTime dataFim { get; set; }
            public string TipoVinculo { get; set; }
        }

        public JsonResult CancelarBoleto(ParamNotas pn)
        {
            try
            {
                var db = new DbControle();
                var boleto = new SoftFin.Web.Models.Boleto().ObterPorId(pn.idboleto,db);

                if (boleto.CnabGerado == false)
                    db.Boleto.Remove(boleto);
                else
                    boleto.CnabCancelado = true;

                db.SaveChanges();
                string caminho = urlDownload();
                return Json(new { CDStatus = "OK"});
            }
            catch (Exception ex)
            {
                _eventos.Error(ex.ToString());
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });
            }
        }

        public JsonResult GerarBoleto(ParamNotas pn)
        {
            try
            {
                var bancoAux = new SoftFin.Web.Models.Banco().ObterPorId(pn.idbanco, _paramBase);
                var ordemVenda = new SoftFin.Web.Models.OrdemVenda().ObterPorId(pn.ov_id);

                var seuNumero = "";
                var codigoAcessoNFse = "";
                if (bancoAux.EmissaoBoletoAposNF == true)
                {
                    var nf = new NotaFiscal().ObterPorOV(pn.ov_id);

                    if (nf == null)
                        return Json(new { CDStatus = "NOK", DSMessage = "É necessario emitir a nota antes da emissão do boleto." });

                    if (nf.TipoFaturamento == 0)
                    {
                        if  (nf.numeroNfse == 0)
                            return Json(new { CDStatus = "NOK", DSMessage = "É necessario emitir a nota antes da emissão do boleto." });

                        seuNumero = nf.numeroNfse.ToString();
                        codigoAcessoNFse = nf.codigoVerificacao.ToString();
                    }
                    else if(nf.TipoFaturamento == 1)
                    {
                        if (nf.numeroNfe == 0)
                            return Json(new { CDStatus = "NOK", DSMessage = "É necessario emitir a nota antes da emissão do boleto." });

                        seuNumero = nf.numeroNfe.ToString();
                    }

                }


                if (pn.codigoBanco == "237" && pn.carteira.Length == 1)
                {
                    pn.carteira = "0" + pn.carteira;
                }

                var boletos = new SoftFin.Web.Models.Boleto().ObterPorOV(_paramBase, pn.ov_id).ToList();

                if (boletos.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Boleto ja gerado." });
                }

                if (seuNumero == "")
                {
                    pn.numeroDocumento = 1;
                    if (pn.numeroDocumento != 0)
                        pn.numeroDocumento = bancoAux.numeroDocumento + 1;
                }
                else
                {
                    pn.numeroDocumento = int.Parse(seuNumero);
                }


                pn.Estabelecimento = _estabobj;
                pn.Pessoa = ordemVenda.Pessoa;

                if (bancoAux.codigoBanco == "033")
                {
                    pn.nossoNumero = "00000000";
                }

                BoletoBancario boletoBancario = CalculaBoleto(pn);

                if (bancoAux.codigoBanco == "033")
                {
                    pn.nossoNumero = "00000000";
                }

                var arquivogerado = SalvaArquivoBoleto(boletoBancario);

                if (codigoAcessoNFse != null)
                {
                    SalvaNFseParaPdf(arquivogerado.Replace(".pdf", "_nfse.pdf"), 
                        _estabobj.InscricaoMunicipal.ToString(), 
                        seuNumero, 
                        codigoAcessoNFse);
                }

                if (seuNumero == "")
                {
                    AtualizaNumeroBoleto(pn.idbanco, pn.numeroDocumento);
                }


                DbControle db = new DbControle();

                var boleto = new Models.Boleto();
                boleto.banco_id = pn.idbanco;
                boleto.DataEmissao = DateTime.Now;
                
                boleto.NumeroDoc = pn.numeroDocumento.ToString();
                        
                boleto.OrdemVenda_ID = pn.ov_id;
                boleto.Valor = pn.valor;
                boleto.TextoBoleto01 = pn.TextoBoleto01;
                boleto.TextoBoleto02 = pn.TextoBoleto02;
                boleto.TextoBoleto03 = pn.TextoBoleto03;
                boleto.Multa = pn.Multa;
                boleto.JurosMora = pn.JurosMora;
                boleto.NomeArquivo = arquivogerado;
                boleto.DataVencimento = pn.DataVencimento;
                boleto.CodigoTransmissao = pn.CodigoTransmissao;



                db.Boleto.Add(boleto);
                db.SaveChanges();
                string caminho = urlDownload();
                return Json(new { CDStatus = "OK", numeroDocumento = boleto.NumeroDoc , URL = caminho + arquivogerado } );
            }
            catch (Exception ex)
            {
                _eventos.Error(ex.ToString());
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });
            }
        }

        private void SalvaNFseParaPdf(string arquivo, string icm, string nf, string codigoAcessoNFse)
        {
            var uploadPath = Server.MapPath("~/TXTTemp/");
            Directory.CreateDirectory(uploadPath);

            string caminhoArquivo = Path.Combine(@uploadPath, arquivo);
            string url = "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?inscricao={icm}&nf={nf}&verificacao={codigoacesso}";
            url = url.Replace("{icm}", icm);
            url = url.Replace("{nf}", nf);
            url = url.Replace("{codigoacesso}", codigoAcessoNFse);

            HtmlToPdf converter = new HtmlToPdf();

            // convert the url to pdf
            PdfDocument doc = converter.ConvertUrl(url);

            // save pdf document
            doc.Save(caminhoArquivo);

            // close pdf document
            doc.Close();

            AzureStorage.UploadFile(caminhoArquivo, RetornaS3Boleto(_paramBase.estab_id, arquivo), ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());
        }


        public JsonResult GerarArquivoCNAB(int idbanco)
        {
            try
            {

                var db = new DbControle();
                var boletos = new SoftFin.Web.Models.Boleto().ObterTodos(_paramBase, db).Where(p => p.banco_id == idbanco).Where(p => p.CnabGerado == false).ToList();

                if (boletos.Count() == 0)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foram encontrados boletos para a geração." });
                }

                string arquivo = Guid.NewGuid() + ".txt";

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    Boletos boletosAux = new Boletos();
                    var banco = boletos.First().banco;
                    Cedente cedente = new Cedente(UtilSoftFin.Limpastrings( _estabobj.CNPJ),_estabobj.NomeCompleto, banco.agencia, banco.contaCorrente,banco.contaCorrenteDigito);

                    foreach (var item in boletos)
                    {
                        ParamNotas pn = new ParamNotas();
                        pn.agencia = item.banco.agencia;
                        pn.agenciaDigito = banco.agenciaDigito;
                        if (item.banco.codigoBanco == "237" )
                            pn.carteira = banco.carteira.ToString("00");
                        else
                            pn.carteira = banco.carteira.ToString();
                        pn.codigoBanco = banco.codigoBanco;
                        pn.contaCorrente = banco.contaCorrente;
                        pn.contaCorrenteDigito = banco.contaCorrenteDigito;
                        pn.DataVencimento = item.DataVencimento;
                        pn.Estabelecimento = _estabobj;
                        pn.idbanco = banco.id;
                        pn.idboleto = item.id;
                        if (item.banco.codigoBanco != "033")
                        {
                            pn.nossoNumero = banco.nossoNumero;
                        }
                        else
                        {
                            pn.nossoNumero = "00000000";
                        }
                        pn.numeroDocumento = int.Parse(item.NumeroDoc);
                        pn.ov_id = item.OrdemVenda_ID;
                        pn.Pessoa = item.OrdemVenda.Pessoa;
                        pn.Multa = item.Multa.Value;
                        pn.JurosMora = item.JurosMora.Value;
                        pn.TextoBoleto01 = item.TextoBoleto01;
                        pn.TextoBoleto02 = item.TextoBoleto02;
                        pn.TextoBoleto03 = item.TextoBoleto03;
                        pn.valor = item.Valor;

                        pn.GeraArquivo = true;


                        pn.CodigoTransmissao = banco.CodigoTransmissao;
                        BoletoBancario boletoBancario = CalculaBoleto(pn);
                        boletoBancario.Boleto.Remessa = new Remessa(TipoOcorrenciaRemessa.EntradaDeTitulos);
                        boletoBancario.Boleto.Remessa.TipoDocumento = "Boleto";

                        if (pn.JurosMora != 0)
                            boletoBancario.Boleto.JurosMora = Math.Round((pn.valor / 100) * pn.JurosMora,2);

                        if (item.banco.codigoBanco == "001")
                        {
                            cedente.Carteira = banco.carteira.ToString();
                            pn.carteira = banco.carteira.ToString();
                            
                            boletoBancario.Boleto.Cedente.Carteira = banco.carteira.ToString();
                        }
                        boletoBancario.Boleto.Cedente.CodigoTransmissao = banco.CodigoTransmissao;
                        cedente.CodigoTransmissao = banco.CodigoTransmissao;
                        if (item.banco.codigoBanco == "033")
                        {
                            boletoBancario.Boleto.Cedente.Codigo = banco.NumeroConvenio;
                        }
                        boletosAux.Add(boletoBancario.Boleto);
                        //cedente = boletosAux.Cedente;
                        item.CnabGerado = true;
                    }
                    db.SaveChanges();



                    // geração do arquivo de remessa - Feito com a ajuda de jbueno
                    var objRemessa = new ArquivoRemessa(TipoArquivo.CNAB400);
                    var memoryStr = new MemoryStream();

                    AtualizaNumeroArquivoRemessa(banco.id, banco.NumeroArquivoRemessa + 1, db);
                    string  erro = "";
                    objRemessa.ValidarArquivoRemessa(banco.NumeroConvenio, new BoletoNet.Banco(int.Parse(banco.codigoBanco)), cedente, boletosAux, banco.NumeroArquivoRemessa + 1, out erro);
                    if (erro != "")
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = erro });
                    }
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);
                    string caminhoArquivo = Path.Combine(@uploadPath, arquivo);

                    objRemessa.GerarArquivoRemessa(banco.NumeroConvenio, new BoletoNet.Banco(int.Parse(banco.codigoBanco)), cedente, boletosAux, 
                        new FileStream(@caminhoArquivo, FileMode.Create, System.IO.FileAccess.Write), banco.NumeroArquivoRemessa + 1);

                    AzureStorage.UploadFile(caminhoArquivo, RetornaS3Boleto(_paramBase.estab_id, arquivo), ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());




                    dbcxtransaction.Commit();
                }
                string caminho = urlDownload();

                new BoletoArquivoHistorico().Incluir(new BoletoArquivoHistorico
                {
                    Caminho = caminho + arquivo,
                    DataGeracao = DateTime.Now,
                    estabelecimento_id = _estab,
                }, _paramBase);

                return Json(new { CDStatus = "OK",  URL = caminho + arquivo });
            }
            catch (Exception ex)
            {
                _eventos.Error(ex.ToString());
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message.ToString());


                foreach (var item in SoftFin.Utils.UtilSoftFin.FlattenHierarchy(ex))
                {
                    sb.AppendLine(item.Message.ToString() + "</BR>");
                }

                return Json(new { CDStatus = "NOK", DSMessage = sb.ToString() });
            }
        }

        private string SalvaArquivoBoleto(BoletoBancario boleto)
        {
            var uploadPath = Server.MapPath("~/TXTTemp/");
            Directory.CreateDirectory(uploadPath);

            var pdf = boleto.MontaBytesPDF(true);
            string arquivo = Guid.NewGuid() + ".pdf";
            string caminhoArquivo = Path.Combine(@uploadPath, arquivo);

            System.IO.File.WriteAllBytes(caminhoArquivo, pdf.ToArray());

            AzureStorage.UploadFile(caminhoArquivo, RetornaS3Boleto(_paramBase.estab_id, arquivo), ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

            return arquivo;
        }

        private BoletoBancario CalculaBoleto(ParamNotas pn)
        {
            var boleto = new BoletoNet.BoletoBancario();
            switch (pn.codigoBanco)
            {
                case "237":
                    boleto = Bradesco(pn);
                    break;
                case "341":
                    boleto = Itau(pn);
                    break;
                case "033":
                case "33":
                    boleto = Santander(pn);
                    break;
                case "104":
                    boleto = Caixa(pn);
                    break;
                case "001":
                case "01":
                case "1":
                    boleto = BB(pn);
                    break;
                default:
                    throw new Exception("Banco não preparado");

            }

            return boleto;
        }

        private static string RetornaS3Boleto(int id, string arquivo)
        {
            return "Boletos/" + id.ToString() + "/" + arquivo;
        }
        public class ParamNotas
        {
            public int numeroDocumento { get; set; }
            public int ov_id { get; set; }
            public int idbanco { get; set; }
            public Pessoa Pessoa { get; set; }
            public Estabelecimento Estabelecimento { get; set; }
            public DateTime DataVencimento { get; set; }
            public string TextoBoleto01 { get; set; }
            public string TextoBoleto02 { get; set; }
            public string TextoBoleto03 { get; set; }
            public decimal valor { get; set; }
            public string codigoBanco { get; set; }

            public string nossoNumero { get; set; }

            public string agencia { get; set; }

            public string agenciaDigito { get; set; }

            public string contaCorrente { get; set; }
            public string contaCorrenteDigito { get; set; }

            public string carteira { get; set; }

            public int idboleto { get; set; }

            public string Email { get; set; }
            public string URL { get; set; }
            public decimal Multa { get;  set; }
            public decimal JurosMora { get;  set; }
            public bool GeraArquivo { get; internal set; }
            public string CodigoTransmissao { get;  set; }
        }

        public JsonResult ObterBanco()
        {
            var objs = new Models.Banco().CarregaBancoGeralBoleto(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        private void AtualizaNumeroBoleto(int bancoid, int numerodocumento)
        {
            DbControle bancoaux = new DbControle();
            var banco = bancoaux.Bancos.Where(x => x.id == bancoid).FirstOrDefault();
            banco.numeroDocumento = numerodocumento;
            bancoaux.SaveChanges();
        }

        private void AtualizaNumeroArquivoRemessa(int bancoid, int numeroNumeroArquivoRemessa, DbControle bancoaux )
        {
            var banco = bancoaux.Bancos.Where(x => x.id == bancoid).FirstOrDefault();
            banco.NumeroArquivoRemessa = numeroNumeroArquivoRemessa;
            bancoaux.SaveChanges();
        }

        public JsonResult Boleto(int idbanco)
        {
            try
            {
                var bancoAux = new SoftFin.Web.Models.Banco().ObterPorId(idbanco, _paramBase);


                var retorno = new
                {
                    bancoAux.carteira,
                    bancoAux.nossoNumero,
                    bancoAux.codigoBanco,
                    bancoAux.TextoBoleto01,
                    bancoAux.TextoBoleto02,
                    bancoAux.TextoBoleto03,
                    bancoAux.agencia,
                    bancoAux.agenciaDigito,
                    bancoAux.contaCorrente,
                    bancoAux.contaCorrenteDigito,
                    bancoAux.Multa,
                    bancoAux.JurosDia,
                    bancoAux.CodigoTransmissao

                };

                return Json(retorno, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex.ToString());
                return null;
            }
        }

        public JsonResult ObterTodos(Filtro filtro)
        {

            DateTime dataIni = new DateTime(filtro.dataIni.Year
                                            , filtro.dataIni.Month
                                            , filtro.dataIni.Day, 0, 0, 0);

            DateTime dataFim = new DateTime(filtro.dataFim.Year
                                , filtro.dataFim.Month
                                , filtro.dataFim.Day, 23, 59, 59);

            if (filtro.TipoVinculo == "1")
            {
                var objs = new Models.OrdemVenda().ObterEntreDataSemBoleto(dataIni, dataFim, _paramBase);
                var objs2 = objs.ToList().Where(p => p.NotasFiscais.Count() > 0);
                var jsonRetorno = new {
                    CDStatus = "OK",
                    objs = objs2.Select(p => new {
                    DataVencimento = p.NotasFiscais.Count() > 0 ? p.NotasFiscais.First().DataVencimentoOriginal.ToString("o") : p.data.ToString("o"),
                    NumeroDoc = "",
                    Valor = p.NotasFiscais.Count() > 0 ? p.NotasFiscais.First().valorLiquido : p.valor,
                    CnabGerado = false,
                    CnabCancelado = false,
                
                    numeroNfe = p.NotasFiscais.Count() > 0 ? p.NotasFiscais.First().numeroNfe.ToString() : "",
                    numeroNfse = p.NotasFiscais.Count() > 0 ? p.NotasFiscais.First().numeroNfse.ToString() : "",
                    serieNfe = p.NotasFiscais.Count() > 0 ? p.NotasFiscais.First().serieNfe.ToString() : "",
                    p.Numero,
                    Pessoa = p.Pessoa.razao,
                    ov_id = p.id,
                    NomeArquivo = "",
                    idbanco = p.NotasFiscais.Count() > 0 ? p.NotasFiscais.First().banco_id : null,
                    Email = p.Pessoa.eMail
                }).ToList()};

                return Json(jsonRetorno, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var objs = new Models.Boleto().ObterTodosOV(_paramBase, dataIni, dataFim);
                var objs2 = objs.ToList();
                string caminho = urlDownload();
                var jsonRetorno = new
                {
                    CDStatus = "OK",
                    objs = objs2.Select(p => new
                    {
                        DataVencimento = p.DataVencimento.ToString("o"),
                        p.NumeroDoc,
                        p.Valor,
                        p.CnabGerado,
                        p.CnabCancelado,
                        numeroNfe = p.OrdemVenda.NotasFiscais.Count() > 0 ? p.OrdemVenda.NotasFiscais.First().numeroNfe.ToString() : "",
                        numeroNfse = p.OrdemVenda.NotasFiscais.Count() > 0 ? p.OrdemVenda.NotasFiscais.First().numeroNfse.ToString() : "",
                        serieNfe = p.OrdemVenda.NotasFiscais.Count() > 0 ? p.OrdemVenda.NotasFiscais.First().serieNfe.ToString() : "",
                        p.OrdemVenda.Numero,
                        Pessoa = p.OrdemVenda.Pessoa.razao,
                        ov_id = p.OrdemVenda.id,
                        URL = caminho + p.NomeArquivo,
                        idbanco = p.OrdemVenda.NotasFiscais.Count() > 0 ? p.OrdemVenda.NotasFiscais.First().banco_id : null
                        ,
                        Email = p.OrdemVenda.Pessoa.eMail, 
                        idboleto = p.id
                    }).ToList()
                };
                return Json(jsonRetorno, JsonRequestBehavior.AllowGet);

            }



        }

        private string urlDownload()
        {
            return ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                            "Boletos/" + _estab + "/";
        }

        private BoletoBancario Santander(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.DataVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.agencia,
                    paramNotas.agenciaDigito,
                    paramNotas.contaCorrente,
                    paramNotas.contaCorrenteDigito);

            c.Codigo = "13000"; 
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.carteira.ToString(), paramNotas.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numeroDocumento).ToString();
            b.NossoNumero = paramNotas.nossoNumero;
            b.Sacado = new Sacado(paramNotas.Pessoa.cnpj, paramNotas.Pessoa.razao);
            b.Sacado.Endereco.End = paramNotas.Pessoa.endereco;
            
            b.Sacado.Endereco.Bairro = paramNotas.Pessoa.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.Pessoa.cidade;
            b.Sacado.Endereco.CEP = paramNotas.Pessoa.cep;
            b.Sacado.Endereco.UF = paramNotas.Pessoa.uf;
            b.ValorBoleto = paramNotas.valor;
            b.PercMulta = paramNotas.Multa;
            b.PercJurosMora = paramNotas.JurosMora;
            b.DataVencimento = paramNotas.DataVencimento;
            b.CodigoTransmissao = paramNotas.CodigoTransmissao;

            Instrucao i = new Instrucao(33);
            i.Descricao = paramNotas.TextoBoleto01;
            if (!string.IsNullOrEmpty( i.Descricao))
                b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(33);
            i2.Descricao = paramNotas.TextoBoleto02;
            if (!string.IsNullOrEmpty(i2.Descricao))
                b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(33);
            i3.Descricao = paramNotas.TextoBoleto03;
            if (!string.IsNullOrEmpty(i3.Descricao))
                b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 33;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = false;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Cedente.CodigoTransmissao = paramNotas.CodigoTransmissao;
            boletoBancario.Boleto.Valida();

            return boletoBancario;
        }

        private BoletoBancario Caixa(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.DataVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.agencia,
                    paramNotas.agenciaDigito,
                    paramNotas.contaCorrente,
                    paramNotas.contaCorrenteDigito);

            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.carteira.ToString(), paramNotas.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numeroDocumento).ToString();
            b.NossoNumero = paramNotas.nossoNumero;
            b.Sacado = new Sacado(paramNotas.Pessoa.cnpj, paramNotas.Pessoa.razao);
            b.Sacado.Endereco.End = paramNotas.Pessoa.endereco;
            b.Sacado.Endereco.Bairro = paramNotas.Pessoa.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.Pessoa.cidade;
            b.Sacado.Endereco.CEP = paramNotas.Pessoa.cep;
            b.Sacado.Endereco.UF = paramNotas.Pessoa.uf;
            b.ValorBoleto = paramNotas.valor;
            b.PercMulta = paramNotas.Multa;
            b.PercJurosMora = paramNotas.JurosMora;
            b.DataVencimento = paramNotas.DataVencimento;

            Instrucao i = new Instrucao(33);
            i.Descricao = paramNotas.TextoBoleto01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(33);
            i2.Descricao = paramNotas.TextoBoleto02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(33);
            i3.Descricao = paramNotas.TextoBoleto03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 104;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = false;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario;
        }

        private BoletoBancario BB(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.DataVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.agencia,
                    paramNotas.agenciaDigito,
                    paramNotas.contaCorrente,
                    paramNotas.contaCorrenteDigito);

            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.carteira.ToString(), paramNotas.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numeroDocumento).ToString();
            b.NossoNumero = paramNotas.nossoNumero;
            b.Sacado = new Sacado(paramNotas.Pessoa.cnpj, paramNotas.Pessoa.razao);
            b.Sacado.Endereco.End = paramNotas.Pessoa.endereco;
            b.Sacado.Endereco.Bairro = paramNotas.Pessoa.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.Pessoa.cidade;
            b.Sacado.Endereco.CEP = paramNotas.Pessoa.cep;
            b.Sacado.Endereco.UF = paramNotas.Pessoa.uf;
            b.ValorBoleto = paramNotas.valor;
            b.PercMulta = paramNotas.Multa;
            b.PercJurosMora = paramNotas.JurosMora;
            b.DataVencimento = paramNotas.DataVencimento;
            

            Instrucao i = new Instrucao(33);
            i.Descricao = paramNotas.TextoBoleto01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(33);
            i2.Descricao = paramNotas.TextoBoleto02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(33);
            i3.Descricao = paramNotas.TextoBoleto03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 1;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = false;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario;
        }

        private BoletoBancario Itau(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.DataVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.agencia,
                    paramNotas.agenciaDigito,
                    paramNotas.contaCorrente,
                    paramNotas.contaCorrenteDigito);

            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.carteira.ToString(), paramNotas.contaCorrente, c);
            b.DataVencimento = vencimento;
            b.PercMulta = paramNotas.Multa;
            b.PercJurosMora = paramNotas.JurosMora;
            b.NumeroDocumento = (paramNotas.numeroDocumento).ToString();
            b.NossoNumero = paramNotas.nossoNumero;
            b.Sacado = new Sacado(paramNotas.Pessoa.cnpj, paramNotas.Pessoa.razao);
            b.Sacado.Endereco.End = paramNotas.Pessoa.endereco;
            b.Sacado.Endereco.Bairro = paramNotas.Pessoa.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.Pessoa.cidade;
            b.Sacado.Endereco.CEP = paramNotas.Pessoa.cep;
            b.Sacado.Endereco.UF = paramNotas.Pessoa.uf;
            b.ValorBoleto = paramNotas.valor;
            
            if (paramNotas.TextoBoleto01 != null)
            {
                Instrucao i = new Instrucao(341);
                i.Descricao = paramNotas.TextoBoleto01;
                b.Instrucoes.Add(i);
            }



            if (paramNotas.TextoBoleto02 != null)
            {
                Instrucao i2 = new Instrucao(341);
                i2.Descricao = paramNotas.TextoBoleto02;
                b.Instrucoes.Add(i2);
            }

            if (paramNotas.TextoBoleto03 != null)
            {
                Instrucao i3 = new Instrucao(341);
                i3.Descricao = paramNotas.TextoBoleto03;
                b.Instrucoes.Add(i3);
            }

            if (paramNotas.GeraArquivo == false)
            {
                var textoMulta = "Após vencimento:";

                if (paramNotas.Multa != 0)
                    textoMulta += " Multa " + paramNotas.Multa.ToString("n") + "% = R$" + Math.Round((paramNotas.valor / 100) * paramNotas.Multa, 4);

                if (paramNotas.JurosMora != 0)
                    textoMulta += " Juros " + paramNotas.JurosMora.ToString("n") + "% a.d. = R$" + Math.Round((paramNotas.valor / 100) * paramNotas.JurosMora, 4) + "/dia";



                if (textoMulta != "Após vencimento:")
                {
                    Instrucao i4 = new Instrucao(341);
                    i4.Descricao = textoMulta;
                    i4.Automativo = true;
                    b.Instrucoes.Add(i4);
                }
            }

            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 341;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = false;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario;
        }

        private BoletoBancario Bradesco(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.DataVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.agencia,
                    paramNotas.agenciaDigito,
                    paramNotas.contaCorrente,
                    paramNotas.contaCorrenteDigito);

            c.Codigo = "13000";

            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.carteira.ToString(), paramNotas.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numeroDocumento).ToString();
            b.NossoNumero = paramNotas.nossoNumero;
            b.Sacado = new Sacado(paramNotas.Pessoa.cnpj, paramNotas.Pessoa.razao);
            b.Sacado.Endereco.End = paramNotas.Pessoa.razao;
            b.Sacado.Endereco.Bairro = paramNotas.Pessoa.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.Pessoa.cidade;
            b.Sacado.Endereco.CEP = paramNotas.Pessoa.cep;
            b.Sacado.Endereco.UF = paramNotas.Pessoa.uf;
            b.ValorBoleto = paramNotas.valor;


            Instrucao i = new Instrucao(237);
            i.Descricao = paramNotas.TextoBoleto01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(237);
            i2.Descricao = paramNotas.TextoBoleto02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(237);
            i3.Descricao = paramNotas.TextoBoleto03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 237;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = true;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario;
        }


        private string ConsultaLinkPF(NotaFiscal notaFiscal)
        {
            string url = "";
            if (notaFiscal.numeroNfse != null)
            {
                url = "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?inscricao={inscricao}&nf={numeronf}&verificacao={codigoverificacao}";
                url = url.Replace("{codigoverificacao}", notaFiscal.codigoVerificacao);
                url = url.Replace("{numeronf}", notaFiscal.numeroNfse.ToString());
                url = url.Replace("{inscricao}", _estabobj.InscricaoMunicipal.ToString().Replace("-", "").Replace(".", "").Replace("/", ""));

            }
            return url;
        }


        public JsonResult EnviarEmail(ParamNotas pn)
        {

            try
            {
                var db = new DbControle();
                var boleto = new SoftFin.Web.Models.Boleto().ObterPorId(pn.idboleto, db);
                var corpoemail = new StringBuilder();
                var titulo = new StringBuilder();

                titulo.Append("SoftFin - Envio de Boleto");

                var lt = new LayoutTemplate().ObterPorCodigo("NOTABOLTITULO",null,_paramBase);
                if (lt != null)
                {
                    titulo.Clear();
                    String titulonovo = "";
                    titulonovo = lt.Template;
                    if (boleto.banco.EmissaoBoletoComNFPDF == true)
                    {
                        var nf = new NotaFiscal().ObterPorOV(boleto.OrdemVenda_ID);
                        titulonovo = titulonovo.
                            Replace("#NumeroNF", nf.numeroNfse.ToString()).
                            Replace("#DataVencimento", nf.dataVencimentoNfse.ToString("dd/MM/yyyy"));
                    }

                    titulo.Append(titulonovo);
                    

                }

                var lc = new LayoutTemplate().ObterPorCodigo("NOTABOLCORPO", null, _paramBase);

                StringBuilder variaveis = new StringBuilder();
                variaveis.AppendLine("<a href='" + pn.URL + "' target='_blank'>Clique aqui para acessar o boleto </a>");
                if (boleto.banco.EmissaoBoletoComNFPDF == true)
                {
                    //variaveis.AppendLine("<br> <a href='" + pn.URL.Replace(".pdf", "_nfse.pdf") + "' target='_blank'>Clique aqui para acessar a nota fiscal emitida </a>");
                    var nf = new NotaFiscal().ObterPorOV(boleto.OrdemVenda_ID);


                    var linq = ConsultaLinkPF(nf);

                    variaveis.AppendLine("<br> <a href='" + linq + "' target='_blank'>Clique aqui para acessar a nota fiscal emitida </a>");


                }


                if (lc == null)
                {
                    corpoemail = variaveis;
                }
                else
                {
                    var temp = lc.Template.Replace("#Links", variaveis.ToString());
                    corpoemail.Append(temp);
                }


                EnviaEmail(titulo.ToString(), corpoemail.ToString(), pn.Email, _estabobj);

                
                return Json(new { CDStatus = "OK" });
            }
            catch (Exception ex)
            {
                _eventos.Error(ex.ToString());
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });
            }

        }

        public JsonResult PegaEmailContato(ParamNotas pn)
        {

            try
            {
                var db = new DbControle();
                var boleto = new SoftFin.Web.Models.Boleto().ObterPorId(pn.idboleto, db);

                var emailaviso = boleto.OrdemVenda.Pessoa.eMail;
                var contatos = new PessoaContato().ObterPorTodos(boleto.OrdemVenda.Pessoa.id,_paramBase,db);

                foreach (var item in contatos)
                {
                    if (item.RecebeCobranca)
                    {
                        if (string.IsNullOrEmpty(emailaviso))
                        {
                            emailaviso = item.email;
                        }
                        else
                        {
                            emailaviso += ";" + item.email;
                        }
                    }
                }

               

                return Json(new { CDStatus = "OK", Email = emailaviso });
            }
            catch (Exception ex)
            {
                _eventos.Error(ex.ToString());
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });
            }

        }

        private void EnviaEmail(string titulo, string corpo, string emailaviso, Estabelecimento estab)
        {
            var email = new Email();
            //var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            //var arquivohmtl = Path.Combine(path, "Email02.html");
            //string readText = System.IO.File.ReadAllText(arquivohmtl);
            //readText = readText.Replace("{Titulo}", titulo);
            //readText = readText.Replace("{Corpo}", corpo);
            //readText = readText.Replace("{nomeestab}", estab.NomeCompleto);
            email.EnviarMensagem(emailaviso, titulo, corpo, true);
        }

        public JsonResult ObterArquivosHistoricos()
        {

            var objs = new BoletoArquivoHistorico().ObterTodos(_paramBase,null);
            return Json(objs.Select(banco => new
            {
                banco.id,
                DataGeracao = banco.DataGeracao.ToString("o"),
                banco.Caminho
            }), JsonRequestBehavior.AllowGet);
        }



    }
}