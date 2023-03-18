using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
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
    public class DocumentoPagarMestreController : BaseController
    {

        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var APagar = new DocumentoPagarParcela().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.usuarioAutorizador_id == null).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + APagar }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult Excel()
        //{
        //    var obj = new DocumentoPagarMestre();
        //    var lista = obj.ObterTodos();
        //    CsvExport myExport = new CsvExport();
        //    foreach (var item in lista)
        //    {
        //        myExport.AddRow();
        //        myExport["id"] = item.id;
        //        myExport["estabelecimento_id"] = item.estabelecimento_id;
        //        myExport["Estabelecimento"] = item.Estabelecimento.NomeCompleto;
        //        myExport["pessoa_id"] = item.pessoa_id;
        //        myExport["dataLancamento"] = item.dataLancamento;
        //        myExport["dataCompetencia"] = item.dataCompetencia;
        //        myExport["dataVencimento"] = item.dataVencimento;
        //        myExport["valorBruto"] = item.valorBruto;
        //        myExport["tipodocumento_id"] = item.tipodocumento_id;
        //        myExport["tipolancamento"] = item.tipolancamento;
        //        myExport["numeroDocumento"] = item.numeroDocumento;
        //        myExport["dataDocumento"] = item.dataDocumento;
        //        myExport["situacaoPagamento"] = item.situacaoPagamento;
        //        myExport["dataPagamanto"] = item.dataPagamanto;
        //        myExport["codigoPagamento"] = item.codigoPagamento;
        //        myExport["lotePagamentoBanco"] = item.lotePagamentoBanco;
        //        myExport["Pessoa"] = item.Pessoa;
        //        myExport["tipoDocumento"] = item.tipoDocumento;
        //    }
        //    string myCsv = myExport.Export();
        //    byte[] myCsvData = myExport.ExportToBytes();
        //    return File(myCsvData, "application/vnd.ms-excel", "DocumentoPagarMestre.csv");
        //}

        public ActionResult cnpj(int id)
        {
            var pes = new Pessoa().ObterPorId(id, _paramBase);

            if (pes != null)
                if (pes.cnpj != null)
                    return Content(pes.cnpj.ToString());

            return Content("");
        }


        [HttpPost]
        public ActionResult codigoBarras(string LinhaDigitavel)
        {
            DateTime dataInicial = Convert.ToDateTime("07/10/1997");
            string varLinhaDigitavel = "";
            string varCodigoBarras = "";
            string auxLinhaDigitavel = "";
            string auxVencimento = "";
            string auxValor = "";
            string auxErro = "";

            varLinhaDigitavel = LinhaDigitavel.Replace(".", "").Replace(" ", "");

            if (varLinhaDigitavel.Length == 36 && varLinhaDigitavel.Substring(33, 3) == "000")
            {
                //341917531421323592044001730900026000
                varLinhaDigitavel = varLinhaDigitavel + "00000000000";
                LinhaDigitavel = varLinhaDigitavel;
            }

            if (varLinhaDigitavel.Length == 47 || varLinhaDigitavel.Length == 48)
            {
                if (varLinhaDigitavel.Substring(0, 1) == "8") //Concessionárias e IPTU
                {
                    varCodigoBarras = varLinhaDigitavel.Substring(0, 11) +
                                varLinhaDigitavel.Substring(12, 11) +
                                varLinhaDigitavel.Substring(24, 11) +
                                varLinhaDigitavel.Substring(36, 11);

                    if (varLinhaDigitavel.Length == 48 && calculaDV2(varCodigoBarras) == true)
                    {
                        auxLinhaDigitavel = varLinhaDigitavel.Substring(0, 12) + " " +
                                                    varLinhaDigitavel.Substring(12, 12) + " " +
                                                    varLinhaDigitavel.Substring(24, 12) + " " +
                                                    varLinhaDigitavel.Substring(36, 12);

                        auxVencimento = "";
                        auxValor = (Decimal.Parse(varCodigoBarras.Substring(4, 11)) / 100).ToString("n");
                    }
                    else
                    {
                        auxErro = "Linha do Boleto inválida!";
                    }
                }
                else //Bloqueto de Cobrança
                {
                    varCodigoBarras = varLinhaDigitavel.Substring(0, 3) + //Código do Banco favorecido (0, 3)
                              varLinhaDigitavel.Substring(3, 1) + //Código da Moeda (3, 1)
                              varLinhaDigitavel.Substring(32, 1) + //DV do Código de Barras (32, 1)
                              varLinhaDigitavel.Substring(33, 4) + //Fator de Vencimento - ex.: 01/05/2002 (33, 4) - Data Base Febraban 
                              varLinhaDigitavel.Substring(37, 10) +  //Valor do Título (37, 10)
                              varLinhaDigitavel.Substring(4, 5) + //Campo Livre - Parte I (4, 5)
                              varLinhaDigitavel.Substring(10, 10) + //Campo Livre - Parte II (10, 10)
                              varLinhaDigitavel.Substring(21, 10); //Campo Livre - Parte III (21, 10)

                    if (varLinhaDigitavel.Length == 47 && calculaDV(varCodigoBarras) == true)
                    {
                        auxLinhaDigitavel = varLinhaDigitavel.Substring(0, 5) + "." +
                                                  varLinhaDigitavel.Substring(5, 5) + " " +
                                                  varLinhaDigitavel.Substring(10, 5) + "." +
                                                  varLinhaDigitavel.Substring(15, 6) + " " +
                                                  varLinhaDigitavel.Substring(21, 5) + "." +
                                                  varLinhaDigitavel.Substring(26, 6) + " " +
                                                  varLinhaDigitavel.Substring(32, 1) + " " +
                                                  varLinhaDigitavel.Substring(33, 14);

                        auxVencimento = dataInicial.AddDays(Convert.ToInt32(varLinhaDigitavel.Substring(33, 4))).ToShortDateString();
                        auxValor = varLinhaDigitavel.Substring(37, 10);
                        decimal valorcalc = Decimal.Parse(auxValor) / 100;
                        auxValor = valorcalc.ToString("n");
                    }
                    else
                    {
                        auxErro = "Linha do Boleto inválida!";
                    }
                }
            }
            else
            {
                auxErro = "Linha do Boleto inválida!";
            }

            return Json(new { Erro = auxErro, LinhaDigitavel = auxLinhaDigitavel, Vencimento = auxVencimento, Valor = auxValor }, JsonRequestBehavior.AllowGet);
        }

        public bool calculaDV(string codigoBarras) //Valida Código de Barras do Boleto
        {
            string codigoSemDv = codigoBarras.Substring(0, 4) + codigoBarras.Substring(5, 39);

            Int32 DvCodigoBarras = Convert.ToInt32(codigoBarras.Substring(4, 1));
            Int32 DvCalculado = 0;
            Int32 Acumulador = 0;
            Int32 mult = 2;
            for (int i = 42; i >= 0; i--)
            {
                Acumulador += (Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult);
                if (mult < 9)
                {
                    mult += 1;
                }
                else
                {
                    mult = 2;
                }
            }

            DvCalculado = 11 - (Acumulador % 11);
            if (DvCalculado == 10)
            {
                DvCalculado = 0;
            }
            if (DvCalculado == 11)
            {
                DvCalculado = 1;
            }
            if (DvCalculado == DvCodigoBarras)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool calculaDV2(string codigoBarras) //Valida Código de Barras Concessionárias e IPTU/ISS
        {
            string codigoSemDv = codigoBarras.Substring(0, 3) + codigoBarras.Substring(4, 40);

            Int32 DvCodigoBarras = Convert.ToInt32(codigoBarras.Substring(3, 1));
            Int32 DvCalculado = 0;
            Int32 Acumulador = 0;
            Int32 mult = 2;
            for (int i = 42; i >= 0; i--)
            {

                if ((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult) > 9)
                {
                    Acumulador += Convert.ToInt32((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult).ToString().Substring(0, 1));
                    Acumulador += Convert.ToInt32((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult).ToString().Substring(0, 2));
                }
                else
                {
                    Acumulador += (Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult);
                }
                if (mult == 2)
                {
                    mult -= 1;
                }
                else
                {
                    mult = 2;
                }
            }

            DvCalculado = 10 - (Acumulador % 10);
            if (DvCalculado == 10)
            {
                DvCalculado = 0;
            }

            if (DvCalculado == DvCodigoBarras)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Index()
        {
            CarregaViewData();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "P", Text = "Provisório" });
            items.Add(new SelectListItem() { Value = "D", Text = "Definitivo" });
            ViewData["TipoLancamento"] = new SelectList(items, "Value", "Text");
            ViewData["competencia"] = DateTime.Now.ToString("MM/yyyy");
            ViewData["banco"] = new Banco().CarregaBancoGeral(_paramBase);
            var obj = new DocumentoPagarMestreVlw();
            return View(obj);
        }

        public ActionResult Index2()
        {
            CarregaViewData();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "P", Text = "Provisório" });
            items.Add(new SelectListItem() { Value = "D", Text = "Definitivo" });
            ViewData["TipoLancamento"] = new SelectList(items, "Value", "Text");


            ViewData["competencia"] = DateTime.Now.ToString("MM/yyyy");

            ViewData["banco"] = new Banco().CarregaBancoGeral(_paramBase);

            var obj = new DocumentoPagarMestreVlw();
            return View(obj);
        }

        //public ActionResult Listas(JqGridRequest request)
        //{
        //    string Valorpessoa_id = Request.QueryString["pessoa_id"];
        //    string Valorbanco = Request.QueryString["banco"];
        //    string ValordataLancamentoIni = Request.QueryString["dataLancamentoIni"];
        //    string ValordataLancamentoFim = Request.QueryString["dataLancamentoFim"];
        //    string ValordataCompetencia = Request.QueryString["dataCompetencia"];
        //    string ValordataVencimentoIni = Request.QueryString["dataVencimentoIni"];
        //    string ValordataVencimentoFim = Request.QueryString["dataVencimentoFim"];
        //    string ValorvalorBrutoIni = Request.QueryString["valorBrutoIni"];
        //    string ValorvalorBrutoFim = Request.QueryString["valorBrutoFim"];
        //    string Valortipodocumento_id = Request.QueryString["tipodocumento_id"];
        //    string Valortipolancamento = Request.QueryString["tipolancamento"];
        //    string ValornumeroDocumento = Request.QueryString["numeroDocumento"];
        //    string ValordataDocumentoIni = Request.QueryString["dataDocumentoIni"];
        //    string ValordataDocumentoFim = Request.QueryString["dataDocumentoFim"];
        //    string ValorsituacaoPagamento = Request.QueryString["situacaoPagamento"];
        //    string ValordataPagamentoIni = Request.QueryString["dataPagamentoIni"];
        //    string ValordataPagamentoFim = Request.QueryString["dataPagamentoFim"];
        //    string ValorcodigoPagamento = Request.QueryString["codigoPagamento"];
        //    string ValorlotePagamentoBanco = Request.QueryString["lotePagamentoBanco"];
        //    string Valordocumentopagaraprovacao_id = Request.QueryString["documentopagaraprovacao_id"];

        //    int totalRecords = 0;
        //    DocumentoPagarMestre obj = new DocumentoPagarMestre();
        //    var objs = new DocumentoPagarMestre().ObterTodos();

        //    if (!String.IsNullOrEmpty(Valorpessoa_id))
        //    {
        //        int aux;
        //        int.TryParse(Valorpessoa_id, out aux);
        //        objs = objs.Where(p => p.pessoa_id == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(Valorbanco))
        //    {
        //        int aux;
        //        int.TryParse(Valorbanco, out aux);
        //        objs = objs.Where(p => p.banco_id == aux).ToList();
        //    }

        //    if (!String.IsNullOrEmpty(ValordataLancamentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataLancamentoIni, out aux);
        //        objs = objs.Where(p => p.dataLancamento >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataLancamentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataLancamentoFim, out aux);
        //        objs = objs.Where(p => p.dataLancamento <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataCompetencia))
        //    {
        //        objs = objs.Where(p => p.dataCompetencia == ValordataCompetencia).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataVencimentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataVencimentoIni, out aux);
        //        objs = objs.Where(p => p.dataVencimento >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataVencimentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataVencimentoFim, out aux);
        //        objs = objs.Where(p => p.dataVencimento <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorvalorBrutoIni))
        //    {
        //        decimal aux;
        //        decimal.TryParse(ValorvalorBrutoIni, out aux);
        //        objs = objs.Where(p => p.valorBruto >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorvalorBrutoFim))
        //    {
        //        decimal aux;
        //        decimal.TryParse(ValorvalorBrutoFim, out aux);
        //        objs = objs.Where(p => p.valorBruto <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(Valortipodocumento_id))
        //    {
        //        int aux;
        //        int.TryParse(Valortipodocumento_id, out aux);
        //        objs = objs.Where(p => p.tipodocumento_id == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(Valortipolancamento))
        //    {
        //        objs = objs.Where(p => p.tipolancamento.Contains(Valortipolancamento)).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValornumeroDocumento))
        //    {
        //        int aux;
        //        int.TryParse(ValornumeroDocumento, out aux);
        //        objs = objs.Where(p => p.numeroDocumento == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataDocumentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataDocumentoIni, out aux);
        //        objs = objs.Where(p => p.dataDocumento >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataDocumentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataDocumentoFim, out aux);
        //        objs = objs.Where(p => p.dataDocumento <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorsituacaoPagamento))
        //    {
        //        objs = objs.Where(p => p.situacaoPagamento.Contains(ValorsituacaoPagamento)).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataPagamentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataPagamentoIni, out aux);
        //        objs = objs.Where(p => p.dataPagamanto >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataPagamentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataPagamentoFim, out aux);
        //        objs = objs.Where(p => p.dataPagamanto <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorcodigoPagamento))
        //    {
        //        int aux;
        //        int.TryParse(ValorcodigoPagamento, out aux);
        //        objs = objs.Where(p => p.codigoPagamento == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorlotePagamentoBanco))
        //    {
        //        objs = objs.Where(p => p.lotePagamentoBanco.Contains(ValorlotePagamentoBanco)).ToList();
        //    }


        //    totalRecords = objs.Count();

        //    JqGridResponse response = new JqGridResponse()
        //    {
        //        TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
        //        PageIndex = request.PageIndex,
        //        TotalRecordsCount = totalRecords
        //    };

        //    objs = Organiza(request, objs);
        //    objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
        //    foreach (var item in objs)
        //    {
        //        string Aprovado = "";
        //        if (item.usuarioAutorizador_id == null)
        //            Aprovado = "NÂO";
        //        else
        //            Aprovado = "SIM";

        //        var datPagto = "";

        //        if (item.dataPagamanto != null)
        //            datPagto = item.dataPagamanto.Value.ToShortDateString();

        //        response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
        //        {
        //            item.Banco.codigoBanco + " - " + item.Banco.nomeBanco + " - " + item.Banco.agencia + " - " + item.Banco.contaCorrente,
        //            item.Pessoa.nome,
        //            item.tipoDocumento.descricao,
        //            item.dataCompetencia,
        //            item.dataVencimento.ToShortDateString(),
        //            item.valorBruto.ToString("n"),
        //            item.tipolancamento,
        //            item.dataDocumento.ToShortDateString(),
        //            item.numeroDocumento.ToString(),
        //            datPagto,
        //            item.situacaoPagamento,
        //            Aprovado

        //        }));
        //    }
        //    return new JqGridJsonResult() { Data = response };
        //}


        //[HttpPost]
        //public ActionResult ListasDT()
        //{
        //    string Valorpessoa_id = Request.QueryString["pessoa_id"];
        //    string Valorbanco = Request.QueryString["banco"];
        //    string ValordataLancamentoIni = Request.QueryString["dataLancamentoIni"];
        //    string ValordataLancamentoFim = Request.QueryString["dataLancamentoFim"];
        //    string ValordataCompetencia = Request.QueryString["dataCompetencia"];
        //    string ValordataVencimentoIni = Request.QueryString["dataVencimentoIni"];
        //    string ValordataVencimentoFim = Request.QueryString["dataVencimentoFim"];
        //    string ValorvalorBrutoIni = Request.QueryString["valorBrutoIni"];
        //    string ValorvalorBrutoFim = Request.QueryString["valorBrutoFim"];
        //    string Valortipodocumento_id = Request.QueryString["tipodocumento_id"];
        //    string Valortipolancamento = Request.QueryString["tipolancamento"];
        //    string ValornumeroDocumento = Request.QueryString["numeroDocumento"];
        //    string ValordataDocumentoIni = Request.QueryString["dataDocumentoIni"];
        //    string ValordataDocumentoFim = Request.QueryString["dataDocumentoFim"];
        //    string ValorsituacaoPagamento = Request.QueryString["situacaoPagamento"];
        //    string ValordataPagamentoIni = Request.QueryString["dataPagamentoIni"];
        //    string ValordataPagamentoFim = Request.QueryString["dataPagamentoFim"];
        //    string ValorcodigoPagamento = Request.QueryString["codigoPagamento"];
        //    string ValorlotePagamentoBanco = Request.QueryString["lotePagamentoBanco"];
        //    string Valordocumentopagaraprovacao_id = Request.QueryString["documentopagaraprovacao_id"];

        //    int totalRecords = 0;
        //    DocumentoPagarMestre obj = new DocumentoPagarMestre();
        //    var objs = new DocumentoPagarMestre().ObterTodos();

        //    if (!String.IsNullOrEmpty(Valorpessoa_id))
        //    {
        //        int aux;
        //        int.TryParse(Valorpessoa_id, out aux);
        //        objs = objs.Where(p => p.pessoa_id == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(Valorbanco))
        //    {
        //        int aux;
        //        int.TryParse(Valorbanco, out aux);
        //        objs = objs.Where(p => p.banco_id == aux).ToList();
        //    }

        //    if (!String.IsNullOrEmpty(ValordataLancamentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataLancamentoIni, out aux);
        //        objs = objs.Where(p => p.dataLancamento >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataLancamentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataLancamentoFim, out aux);
        //        objs = objs.Where(p => p.dataLancamento <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataCompetencia))
        //    {
        //        objs = objs.Where(p => p.dataCompetencia == ValordataCompetencia).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataVencimentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataVencimentoIni, out aux);
        //        objs = objs.Where(p => p.dataVencimento >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataVencimentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataVencimentoFim, out aux);
        //        objs = objs.Where(p => p.dataVencimento <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorvalorBrutoIni))
        //    {
        //        decimal aux;
        //        decimal.TryParse(ValorvalorBrutoIni, out aux);
        //        objs = objs.Where(p => p.valorBruto >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorvalorBrutoFim))
        //    {
        //        decimal aux;
        //        decimal.TryParse(ValorvalorBrutoFim, out aux);
        //        objs = objs.Where(p => p.valorBruto <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(Valortipodocumento_id))
        //    {
        //        int aux;
        //        int.TryParse(Valortipodocumento_id, out aux);
        //        objs = objs.Where(p => p.tipodocumento_id == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(Valortipolancamento))
        //    {
        //        objs = objs.Where(p => p.tipolancamento.Contains(Valortipolancamento)).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValornumeroDocumento))
        //    {
        //        int aux;
        //        int.TryParse(ValornumeroDocumento, out aux);
        //        objs = objs.Where(p => p.numeroDocumento == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataDocumentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataDocumentoIni, out aux);
        //        objs = objs.Where(p => p.dataDocumento >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataDocumentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataDocumentoFim, out aux);
        //        objs = objs.Where(p => p.dataDocumento <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorsituacaoPagamento))
        //    {
        //        objs = objs.Where(p => p.situacaoPagamento.Contains(ValorsituacaoPagamento)).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataPagamentoIni))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataPagamentoIni, out aux);
        //        objs = objs.Where(p => p.dataPagamanto >= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValordataPagamentoFim))
        //    {
        //        DateTime aux;
        //        DateTime.TryParse(ValordataPagamentoFim, out aux);
        //        objs = objs.Where(p => p.dataPagamanto <= aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorcodigoPagamento))
        //    {
        //        int aux;
        //        int.TryParse(ValorcodigoPagamento, out aux);
        //        objs = objs.Where(p => p.codigoPagamento == aux).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(ValorlotePagamentoBanco))
        //    {
        //        objs = objs.Where(p => p.lotePagamentoBanco.Contains(ValorlotePagamentoBanco)).ToList();
        //    }


        //    totalRecords = objs.Count();




        //    var retorno = new StringBuilder();

        //    retorno.AppendLine("<table class='table table-striped table-bordered table-hover' id='dataTables'>");
        //    retorno.AppendLine("    <thead>");
        //    retorno.AppendLine("        <tr>");
        //    retorno.AppendLine("            <th>key</th>");
        //    retorno.AppendLine("            <th>Banco</th>");
        //    retorno.AppendLine("            <th>Pessoa</th>");
        //    retorno.AppendLine("            <th>Tipo Documento</th>");
        //    retorno.AppendLine("            <th>Competencia</th>");
        //    retorno.AppendLine("            <th>Vencimento</th>");
        //    retorno.AppendLine("            <th>Valor</th>");
        //    retorno.AppendLine("            <th>Tipo Lancto</th>");
        //    retorno.AppendLine("            <th>Data Documento</th>");
        //    retorno.AppendLine("            <th>Número Doc</th>");
        //    retorno.AppendLine("            <th>Data Pgto</th>");
        //    retorno.AppendLine("            <th>Situação Pgto</th>");
        //    retorno.AppendLine("            <th>Aprovado</th>");
        //    retorno.AppendLine("        </tr>");
        //    retorno.AppendLine("    </thead>");
        //    retorno.AppendLine("    <tbody>");

        //    foreach (var item in objs)
        //    {
        //        string Aprovado = "";
        //        if (item.usuarioAutorizador_id == null)
        //            Aprovado = "NÂO";
        //        else
        //            Aprovado = "SIM";

        //        retorno.AppendLine("<tr>");
        //        retorno.AppendLine("<td>" + item.id + "</td>");
        //        retorno.AppendLine("<td>" + item.Banco.codigoBanco + " - " + item.Banco.nomeBanco + " - " + item.Banco.agencia + " - " + item.Banco.contaCorrente + "</td>");
        //        retorno.AppendLine("<td>" + item.Pessoa.nome + "</td>");
        //        retorno.AppendLine("<td>" + item.tipoDocumento.descricao + "</td>");
        //        retorno.AppendLine("<td>" + item.dataCompetencia + "</td>");
        //        retorno.AppendLine("<td>" + item.dataVencimento.ToShortDateString() + "</td>");
        //        retorno.AppendLine("<td>" + item.valorBruto.ToString("n") + "</td>");
        //        retorno.AppendLine("<td>" + item.tipolancamento + "</td>");
        //        retorno.AppendLine("<td>" + item.dataDocumento.ToShortDateString() + "</td>");
        //        retorno.AppendLine("<td>" + item.numeroDocumento.ToString() + "</td>");
        //        retorno.AppendLine("<td>" + item.dataPagamanto + "</td>");
        //        retorno.AppendLine("<td>" + item.situacaoPagamento + "</td>");
        //        retorno.AppendLine("<td>" + Aprovado + "</td>");
        //        retorno.AppendLine("</tr");
        //    }
        //    retorno.AppendLine("    </tbody>");
        //    retorno.AppendLine("</table>");

        //    return Content(retorno.ToString());
        //}



        //private static List<DocumentoPagarMestre> Organiza(JqGridRequest request, List<DocumentoPagarMestre> objs)
        //{
        //    switch (request.SortingName)
        //    {
        //        case "Pessoa":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.Pessoa.nome).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.Pessoa.nome).ToList();
        //            break;
        //        case "banco":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.Banco.nomeBanco).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.Banco.nomeBanco).ToList();
        //            break;
        //        case "dataLancamento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.dataLancamento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.dataLancamento).ToList();
        //            break;
        //        case "Competencia":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.dataCompetencia).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.dataCompetencia).ToList();
        //            break;
        //        case "DataVencimento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.dataVencimento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.dataVencimento).ToList();
        //            break;
        //        case "Valor":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.valorBruto).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.valorBruto).ToList();
        //            break;
        //        case "TipoDocumento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.tipoDocumento.descricao).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.tipoDocumento.descricao).ToList();
        //            break;
        //        case "Tipo":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.tipolancamento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.tipolancamento).ToList();
        //            break;
        //        case "NumDoc":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.numeroDocumento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.numeroDocumento).ToList();
        //            break;
        //        case "DataDocumento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.dataDocumento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.dataDocumento).ToList();
        //            break;
        //        case "situacaoPagamento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.situacaoPagamento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.situacaoPagamento).ToList();
        //            break;
        //        case "DataPagamento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.dataPagamanto).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.dataPagamanto).ToList();
        //            break;
        //        case "codigoPagamento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.codigoPagamento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.codigoPagamento).ToList();
        //            break;
        //        case "lotePagamentoBanco":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.lotePagamentoBanco).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.lotePagamentoBanco).ToList();
        //            break;
        //        case "tipoDocumento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.tipoDocumento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.tipoDocumento).ToList();
        //            break;
        //        case "documentopagaraprovacao_id":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.documentopagaraprovacao_id).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.documentopagaraprovacao_id).ToList();
        //            break;
        //        case "DocumentoPagarAprovacao":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.DocumentoPagarAprovacao).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.DocumentoPagarAprovacao).ToList();
        //            break;
        //        case "Aprovado":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.usuarioAutorizador_id).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.usuarioAutorizador_id).ToList();
        //            break;
        //        case "SitPagamento":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.situacaoPagamento).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.situacaoPagamento).ToList();
        //            break;
        //        case "banco_id":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.banco_id).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.banco_id).ToList();
        //            break;
        //        case "Banco":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.Banco).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.Banco).ToList();
        //            break;
        //        case "LinhaDigitavel":
        //            if (request.SortingOrder == JqGridSortingOrders.Desc)
        //                objs = objs.OrderByDescending(p => p.LinhaDigitavel).ToList();
        //            else
        //                objs = objs.OrderBy(p => p.LinhaDigitavel).ToList();
        //            break;
        //    }
        //    return objs;
        //}

        //[HttpPost]
        //public ActionResult Create(DocumentoPagarMestreVlw2 obj)
        //{
        //    try
        //    {
        //        CarregaViewData();
        //        obj.dataLancamento = DateTime.Now;
        //        List<DocumentoPagarDetalhe> col = ExtraiDetalhe();
        //        ViewData["DocumentoPagarDetalhe"] = col;
        //        obj.estabelecimento_id = pb.estab_id;

        //        if (col.Count() == 0)
        //        {
        //            ModelState.AddModelError(string.Empty, "Erro! Informe o detalhe do lançamento");
        //            return View(obj);
        //        }

        //        if (obj.valorBruto != col.Sum(p => p.valor))
        //        {
        //            ModelState.AddModelError(string.Empty, "Erro! Valor total do detalhe não é igual ao bruto invalido");
        //            return View(obj);
        //        }

        //        var documento = new DocumentoPagarMestre();
        //        documento.banco_id = obj.banco_id;
        //        documento.codigoPagamento = obj.codigoPagamento;
        //        documento.dataCompetencia = obj.dataCompetencia;
        //        documento.dataDocumento = obj.dataDocumento;
        //        documento.dataLancamento = obj.dataLancamento;
        //        documento.dataPagamanto = obj.dataPagamanto;
        //        documento.dataVencimento = obj.dataVencimento;
        //        documento.documentopagaraprovacao_id = obj.documentopagaraprovacao_id;
        //        documento.estabelecimento_id = obj.estabelecimento_id;
        //        documento.lotePagamentoBanco = obj.lotePagamentoBanco;
        //        documento.numeroDocumento = obj.numeroDocumento;
        //        documento.pessoa_id = obj.pessoa_id;
        //        documento.situacaoPagamento = obj.situacaoPagamento;
        //        documento.tipodocumento_id = obj.tipodocumento_id;
        //        documento.tipolancamento = obj.tipolancamento;
        //        documento.valorBruto = obj.valorBruto;
        //        documento.LinhaDigitavel = Request.Form["LinhaDigitavel"].ToString();
        //        ViewBag.LinhaDigitavel = documento.LinhaDigitavel;

        //        var tipoDoc = new TipoDocumento().ObterPorId(obj.tipodocumento_id).codigo;

        //        var db = new DbControle();

        //        using (var dbcxtransaction = db.Database.BeginTransaction())
        //        {
        //            if (documento.Incluir(documento, col, db) == true)
        //            {


        //                if (tipoDoc == "GPS")
        //                {
        //                    var gps = new GPS();
        //                    gps.DocumentoPagarMestre_id = documento.id;
        //                    gps.codigoPagamento = obj.GPScodigoPagamento;
        //                    gps.competencia = obj.GPScompetencia;
        //                    gps.estabelecimento_id = obj.estabelecimento_id;
        //                    gps.identificador = obj.GPSidentificador;
        //                    gps.informacoesComplementares = obj.GPSinformacoesComplementares;
        //                    gps.nomeContribuinte = obj.GPSnomeContribuinte;
        //                    gps.valorArrecadado = obj.GPSvalorArrecadado;
        //                    gps.valorAtualizacaoMonetaria = obj.GPSvalorAtualizacaoMonetaria;
        //                    gps.valorOutrasEntidades = obj.GPSvalorOutrasEntidades;
        //                    gps.valorTributo = obj.GPSvalorTributo;
        //                    gps.dataArrecadacao = obj.GPSdataArrecadacao;
        //                    gps.Incluir(db);
        //                }


        //                if (tipoDoc == "DARF")
        //                {
        //                    var darf = new DARF();
        //                    darf.DocumentoPagarMestre_id = documento.id;
        //                    darf.cnpj = obj.DARFcnpj;
        //                    darf.codigoReceita = obj.DARFcodigoReceita;
        //                    darf.dataPagamento = obj.DARFdataPagamento;
        //                    darf.dataVencimento = obj.DARFdataVencimento;
        //                    darf.estabelecimento_id = obj.estabelecimento_id;
        //                    darf.jurosEncargos = obj.DARFjurosEncargos;
        //                    darf.multa = obj.DARFmulta;
        //                    darf.nomeContribuinte = obj.DARFnomeContribuinte;
        //                    darf.numeroReferencia = obj.DARFnumeroReferencia;
        //                    darf.periodoApuracao = obj.DARFperiodoApuracao;
        //                    darf.valorPrincipal = obj.DARFvalorPrincipal;
        //                    darf.valorTotal = obj.DARFvalorTotal;
        //                    darf.Incluir(db);
        //                }

        //                if (tipoDoc == "FGTS")
        //                {

        //                    var fgts = new FGTS();
        //                    fgts.DocumentoPagarMestre_id = documento.id;
        //                    fgts.cnpj = obj.FGTScnpj;
        //                    fgts.codigoBarras = obj.FGTScodigoBarras;
        //                    fgts.codigoReceita = obj.FGTScodigoReceita;
        //                    fgts.dataPagamento = obj.FGTSdataPagamento;
        //                    fgts.digitoLacre = obj.FGTSdigitoLacre;
        //                    fgts.estabelecimento_id = obj.estabelecimento_id;
        //                    fgts.identificadorFgts = obj.FGTSidentificadorFgts;
        //                    fgts.nomeContribuinte = obj.FGTSnomeContribuinte;
        //                    fgts.tipoInscricao = obj.FGTStipoInscricao;
        //                    fgts.valorPagamento = obj.FGTSvalorPagamento;
        //                    fgts.Incluir(db);
        //                }


        //                dbcxtransaction.Commit();
        //                ViewBag.msg = "Documento incluído com sucesso";
        //            }
        //            else
        //            {
        //                ViewBag.msg = "Documento já cadastrado";

        //            }

        //        }

        //        ViewData["DocumentoPagarDetalhe"] = ExtraiDetalhe();
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        //private List<DocumentoPagarDetalhe> ExtraiDetalhe()
        //{

        //        List<DocumentoPagarDetalhe> col = new List<DocumentoPagarDetalhe>();
        //        foreach (string key in Request.Form.AllKeys)
        //        {
        //            if (key.StartsWith("postCC"))
        //            {
        //                string codigo = key.Substring(6);
        //                string chave = "postCC" + codigo;
        //                int conta = int.Parse(Request.Form[chave]);

        //                var contaobj = new PlanoDeConta().ObterPorId(conta);

        //                chave = "postUND" + codigo;
        //                int unidade = int.Parse(Request.Form[chave]);

        //                var unidadeobj = new UnidadeNegocio().ObterTodos().Where(p => p.id == unidade).First();

        //                chave = "postHST" + codigo;
        //                string hist = Request.Form[chave].ToString();

        //                chave = "postVLR" + codigo;
        //                decimal valor = decimal.Parse(Request.Form[chave]);

        //                chave = "postPOR" + codigo;
        //                double porcentagem = double.Parse(Request.Form[chave]);

        //                col.Add(new DocumentoPagarDetalhe
        //                {
        //                    planoDeConta_id = conta,
        //                    unidadenegocio_id = unidade,
        //                PlanoDeConta = contaobj,
        //                    UnidadeNegocio = unidadeobj,
        //                    historico = hist,
        //                    valor = valor,
        //                    percentual = porcentagem,
        //                    estabelecimento_id = pb.estab_id
        //                });
        //            }
        //        }
        //        return col;
        //}

        private void CarregaViewData()
        {

            ViewData["Pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["TipoDocumento"] = new SelectList(new TipoDocumento().ObterTodos(), "id", "descricao");

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "P", Text = "Provisório", Selected = true });
            //items.Add(new SelectListItem() { Value = "D", Text = "Definitivo" });
            ViewData["TipoLancamento"] = new SelectList(items, "Value", "Text");

            ViewData["PlanoDeContas"] = new PlanoDeConta().ObterTodosTipoA();
            ViewData["UnidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");

            CarregaBanco();
            //items.Add(new SelectListItem() { Value = "A", Text = "Em Aberto", Selected = true });
            //items.Add(new SelectListItem() { Value = "P", Text = "Pago" });
            //items.Add(new SelectListItem() { Value = "C", Text = "Cancelador" });
            //ViewData["SituacaoPagamento"] = new SelectList(items, "Value", "Text");
        }

        private void CarregaBanco()
        {

            var con1 = new Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            ViewData["banco"] = listret;
        }

        //public ActionResult Create()
        //{
        //    try
        //    {
        //        CarregaViewData();

        //        List<DocumentoPagarDetalhe> col = new List<DocumentoPagarDetalhe>();
        //        ViewData["DocumentoPagarDetalhe"] = col;

        //        var obj = new DocumentoPagarMestreVlw2();
        //        obj.dataLancamento = DateTime.Now;
        //        obj.dataDocumento = DateTime.Now;
        //        obj.dataVencimento = DateTime.Now;
        //        obj.dataVencimentoOriginal = DateTime.Now;
        //        obj.dataCompetencia = DateTime.Now.ToString("MM/yyyy");
        //        obj.DARFdataPagamento = DateTime.Now;

        //        obj.situacaoPagamento = "A";
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        //public ActionResult Create2()
        //{
        //    try
        //    {
        //        CarregaViewData();

        //        List<DocumentoPagarDetalhe> col = new List<DocumentoPagarDetalhe>();
        //        ViewData["DocumentoPagarDetalhe"] = col;

        //        var obj = new DocumentoPagarMestreVlw2();
        //        obj.dataLancamento = DateTime.Now;
        //        obj.dataDocumento = DateTime.Now;
        //        obj.dataVencimento = DateTime.Now;
        //        obj.dataVencimentoOriginal = DateTime.Now;
        //        obj.dataCompetencia = DateTime.Now.ToString("MM/yyyy");
        //        obj.DARFdataPagamento = DateTime.Now;

        //        obj.situacaoPagamento = "A";
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        //[HttpPost]
        //public ActionResult Delete(DocumentoPagarMestreVlw2 obj)
        //{


        //    try
        //    {
        //        var db = new DbControle();

        //        CarregaViewData();

        //        var bancoMovimentos = new BancoMovimento().ObterPorCPAG(obj.id, db);

        //        using (var dbcxtransaction = db.Database.BeginTransaction())
        //        {
        //            foreach (var item in bancoMovimentos)
        //            {
        //                string erro = "";
        //                new BancoMovimento().Excluir(item.id, ref erro, db);
        //                if (erro != "")
        //                    throw new Exception(erro);
        //            }


        //            var gps = new GPS().ObterPorCPAG(obj.id, db);

        //            if (gps != null)
        //                new GPS().Excluir(gps.id, db);


        //            var darf = new DARF().ObterPorCPAG(obj.id, db);
        //            if (darf != null)
        //                new DARF().Excluir(darf.ID, db);

        //            var fgts = new FGTS().ObterPorCPAG(obj.id, db);
        //            if (fgts != null)
        //                new GPS().Excluir(fgts.ID, db);

        //            var documentoPagarItems = new DocumentoPagarDetalhe().ObterPorCPAG(obj.id, db);

        //            foreach (var item in documentoPagarItems)
        //            {
        //                string erro = "";
        //                new DocumentoPagarDetalhe().Excluir(item.id, ref erro, db);
        //                if (erro != "")
        //                    throw new Exception(erro);
        //            }


        //            var cs = new DocumentoPagarMestre();
        //            cs.Excluir(obj.id, db);
        //            dbcxtransaction.Commit();
        //        }

        //        ViewBag.msg = "Documento excluído com sucesso";
        //        return RedirectToAction("/Index");
        //    }
        //    catch (Exception ex)
        //    {

        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}


        //[HttpPost]
        //public ActionResult Prorrogar(int id, DateTime dataVencimento)
        //{
        //    try
        //    {
        //        DbControle banco = new DbControle();

        //        using (var dbcxtransaction = banco.Database.BeginTransaction())
        //        {
        //            var documento = new DocumentoPagarMestre().ObterPorId(id, banco);
        //            documento.dataVencimento = dataVencimento;
        //            documento.Alterar(documento, null, banco);

        //            var bancoMovimentos = new BancoMovimento().ObterPorCPAG(documento.id, banco);

        //            foreach (var item in bancoMovimentos)
        //            {
        //                item.data = dataVencimento;
        //                item.Alterar(banco);
        //            }
        //            dbcxtransaction.Commit();
        //        }

        //        ViewBag.msg = "Documento alterado com sucesso "
        //            + id.ToString() + " - "
        //            + dataVencimento.ToShortDateString();
        //        //return View();
        //        return Prorrogar(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        //public ActionResult Detail2(int ID, string SemBarras)
        //{
        //    return Detail(ID, SemBarras);
        //}

        //public ActionResult Detail(int ID, string SemBarras)
        //{
        //    try
        //    {
        //        ViewData["objs"] = new DocumentoPagarArquivo().ObterPorCPAG(ID);
        //        DocumentoPagarMestreVlw2 obj = new DocumentoPagarMestreVlw2();
        //        CarregaViewData();

        //        var documento = new DocumentoPagarMestre().ObterPorId(ID);

        //        obj.banco_id = documento.banco_id;
        //        obj.codigoPagamento = documento.codigoPagamento;
        //        obj.dataCompetencia = documento.dataCompetencia;
        //        obj.dataDocumento = documento.dataDocumento;
        //        obj.dataLancamento = documento.dataLancamento;
        //        obj.dataPagamanto = documento.dataPagamanto;
        //        obj.dataVencimento = documento.dataVencimento;
        //        obj.dataVencimentoOriginal = documento.dataVencimentoOriginal;
        //        obj.documentopagaraprovacao_id = documento.documentopagaraprovacao_id;
        //        obj.estabelecimento_id = documento.estabelecimento_id;
        //        obj.lotePagamentoBanco = documento.lotePagamentoBanco;
        //        obj.numeroDocumento = documento.numeroDocumento;
        //        obj.pessoa_id = documento.pessoa_id;
        //        obj.situacaoPagamento = documento.situacaoPagamento;
        //        obj.tipodocumento_id = documento.tipodocumento_id;
        //        obj.tipolancamento = documento.tipolancamento;
        //        obj.valorBruto = documento.valorBruto;
        //        obj.linhadigitavel = documento.LinhaDigitavel;

        //        ViewData["DocumentoPagarDetalhe"] = new DocumentoPagarDetalhe().ObterPorCPAG(documento.id);
        //        ViewData["SemBarras"] = SemBarras;

        //        var tipoDoc = new TipoDocumento().ObterPorId(documento.tipodocumento_id).codigo;

        //        if (tipoDoc == "GPS")
        //        {
        //            var gps = new GPS().ObterPorCPAG(documento.id);
        //            if (gps != null)
        //            {
        //                obj.GPScodigoPagamento = gps.codigoPagamento;
        //                obj.GPScompetencia = gps.competencia;
        //                obj.GPSidentificador = gps.identificador;
        //                obj.GPSinformacoesComplementares = gps.informacoesComplementares;
        //                obj.GPSnomeContribuinte = gps.nomeContribuinte;
        //                obj.GPSvalorArrecadado = gps.valorArrecadado;
        //                obj.GPSvalorAtualizacaoMonetaria = gps.valorAtualizacaoMonetaria;
        //                obj.GPSvalorOutrasEntidades = gps.valorOutrasEntidades;
        //                obj.GPSvalorTributo = gps.valorTributo;
        //                obj.GPSdataArrecadacao = gps.dataArrecadacao;
        //            }
        //        }


        //        if (tipoDoc == "DARF")
        //        {
        //            var darf = new DARF().ObterPorCPAG(documento.id);

        //            if (darf != null)
        //            {
        //                obj.DARFcnpj = darf.cnpj;
        //                obj.DARFcodigoReceita = darf.codigoReceita;
        //                obj.DARFdataPagamento = darf.dataPagamento;
        //                obj.DARFdataVencimento = darf.dataVencimento;
        //                obj.estabelecimento_id = darf.estabelecimento_id;
        //                obj.DARFjurosEncargos = darf.jurosEncargos;
        //                obj.DARFmulta = darf.multa;
        //                obj.DARFnomeContribuinte = darf.nomeContribuinte;
        //                obj.DARFnumeroReferencia = darf.numeroReferencia;
        //                obj.DARFperiodoApuracao = darf.periodoApuracao;
        //                obj.DARFvalorPrincipal = darf.valorPrincipal;
        //                obj.DARFvalorTotal = darf.valorTotal;
        //            }
        //        }

        //        if (tipoDoc == "FGTS")
        //        {

        //            var fgts = new FGTS().ObterPorCPAG(documento.id);

        //            if (fgts != null)
        //            {
        //                obj.FGTScnpj = fgts.cnpj;
        //                obj.FGTScodigoBarras = fgts.codigoBarras;
        //                obj.FGTScodigoReceita = fgts.codigoReceita;
        //                obj.FGTSdataPagamento = fgts.dataPagamento;
        //                obj.FGTSdigitoLacre = fgts.digitoLacre;
        //                obj.estabelecimento_id = fgts.estabelecimento_id;
        //                obj.FGTSidentificadorFgts = fgts.identificadorFgts;
        //                obj.FGTSnomeContribuinte = fgts.nomeContribuinte;
        //                obj.FGTStipoInscricao = fgts.tipoInscricao;
        //                obj.FGTSvalorPagamento = fgts.valorPagamento;
        //            }
        //        }


        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        //public ActionResult Delete(int ID)
        //{
        //    try
        //    {
        //        DocumentoPagarMestreVlw2 obj = new DocumentoPagarMestreVlw2();
        //        CarregaViewData();

        //        var documento = new DocumentoPagarMestre().ObterPorId(ID);
        //        SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(documento);

        //        obj.banco_id = documento.banco_id;
        //        obj.codigoPagamento = documento.codigoPagamento;
        //        obj.dataCompetencia = documento.dataCompetencia;
        //        obj.dataDocumento = documento.dataDocumento;
        //        obj.dataLancamento = documento.dataLancamento;
        //        obj.dataPagamanto = documento.dataPagamanto;
        //        obj.dataVencimento = documento.dataVencimento;
        //        obj.dataVencimentoOriginal = documento.dataVencimentoOriginal;
        //        obj.documentopagaraprovacao_id = documento.documentopagaraprovacao_id;
        //        obj.estabelecimento_id = documento.estabelecimento_id;
        //        obj.lotePagamentoBanco = documento.lotePagamentoBanco;
        //        obj.numeroDocumento = documento.numeroDocumento;
        //        obj.pessoa_id = documento.pessoa_id;
        //        obj.situacaoPagamento = documento.situacaoPagamento;
        //        obj.tipodocumento_id = documento.tipodocumento_id;
        //        obj.tipolancamento = documento.tipolancamento;
        //        obj.valorBruto = documento.valorBruto;

        //        ViewData["DocumentoPagarDetalhe"] = new DocumentoPagarDetalhe().ObterPorCPAG(documento.id);


        //        var tipoDoc = new TipoDocumento().ObterPorId(documento.tipodocumento_id).codigo;

        //        if (tipoDoc == "GPS")
        //        {
        //            var gps = new GPS().ObterPorCPAG(documento.id);
        //            if (gps != null)
        //            {
        //                obj.GPScodigoPagamento = gps.codigoPagamento;
        //                obj.GPScompetencia = gps.competencia;
        //                obj.GPSidentificador = gps.identificador;
        //                obj.GPSinformacoesComplementares = gps.informacoesComplementares;
        //                obj.GPSnomeContribuinte = gps.nomeContribuinte;
        //                obj.GPSvalorArrecadado = gps.valorArrecadado;
        //                obj.GPSvalorAtualizacaoMonetaria = gps.valorAtualizacaoMonetaria;
        //                obj.GPSvalorOutrasEntidades = gps.valorOutrasEntidades;
        //                obj.GPSvalorTributo = gps.valorTributo;
        //                obj.GPSdataArrecadacao = gps.dataArrecadacao;
        //            }
        //        }


        //        if (tipoDoc == "DARF")
        //        {
        //            var darf = new DARF().ObterPorCPAG(documento.id);

        //            if (darf != null)
        //            {
        //                obj.DARFcnpj = darf.cnpj;
        //                obj.DARFcodigoReceita = darf.codigoReceita;
        //                obj.DARFdataPagamento = darf.dataPagamento;
        //                obj.DARFdataVencimento = darf.dataVencimento;
        //                obj.estabelecimento_id = darf.estabelecimento_id;
        //                obj.DARFjurosEncargos = darf.jurosEncargos;
        //                obj.DARFmulta = darf.multa;
        //                obj.DARFnomeContribuinte = darf.nomeContribuinte;
        //                obj.DARFnumeroReferencia = darf.numeroReferencia;
        //                obj.DARFperiodoApuracao = darf.periodoApuracao;
        //                obj.DARFvalorPrincipal = darf.valorPrincipal;
        //                obj.DARFvalorTotal = darf.valorTotal;
        //            }
        //        }

        //        if (tipoDoc == "FGTS")
        //        {

        //            var fgts = new FGTS().ObterPorCPAG(documento.id);

        //            if (fgts != null)
        //            {
        //                obj.FGTScnpj = fgts.cnpj;
        //                obj.FGTScodigoBarras = fgts.codigoBarras;
        //                obj.FGTScodigoReceita = fgts.codigoReceita;
        //                obj.FGTSdataPagamento = fgts.dataPagamento;
        //                obj.FGTSdigitoLacre = fgts.digitoLacre;
        //                obj.estabelecimento_id = fgts.estabelecimento_id;
        //                obj.FGTSidentificadorFgts = fgts.identificadorFgts;
        //                obj.FGTSnomeContribuinte = fgts.nomeContribuinte;
        //                obj.FGTStipoInscricao = fgts.tipoInscricao;
        //                obj.FGTSvalorPagamento = fgts.valorPagamento;
        //            }
        //        }


        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        public ActionResult Autorizacao()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
            return View();
        }

        [HttpPost]
        public ActionResult Autorizacao(string dataInicial, string dataFinal)
        {
            @ViewData["DataInicial"] = dataInicial;
            @ViewData["DataFinal"] = dataFinal;
            PerfilPagarAprovacao PF = new PerfilPagarAprovacao();

            PF = new PerfilPagarAprovacao().ObterTodos(_paramBase).Where(p => p.usuarioAutorizador.codigo == Acesso.UsuarioLogado()).FirstOrDefault();

            if (PF == null)
            {
                ViewBag.msg = "Usuário não tem acesso a autorização.";
                return View();
            }


            List<int> cpgs = new List<int>();
            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 3) == "cpg")
                {
                    cpgs.Add(int.Parse(item.ToString().Substring(3)));
                }

            }

            int idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
            int estab = _paramBase.estab_id;
            string msgextra = "";
            var db = new DbControle();

            foreach (var item in cpgs)
            {
                var ov = db.DocumentoPagarParcela.Where(x => x.id == item && x.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();

                if (ov != null)
                {
                    if (ov.valor <= decimal.Parse(PF.valorLimiteCPAG.ToString()))
                    {
                        ov.usuarioAutorizador_id = idusuario;
                    }
                    else
                    {
                        msgextra = ", 1 ou mais notas não autorizadas por exceder o limite de valores";
                    }

                }
            }

            db.SaveChanges();
            ViewBag.msg = "Autorizado com sucesso." + msgextra;

            return View();
        }



        [HttpPost]
        public JsonResult ConsultaAutorizacao(string dataInicial, string dataFinal)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);


            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal).AddDays(1);


            var cpgs = new DocumentoPagarParcela().ObterTodosNaoAutorizado(DataInicial, DataFinal,_paramBase);
            List<DocumentoPagarMestreAutorizacao> rets = new List<DocumentoPagarMestreAutorizacao>();

            foreach (var item in cpgs)
            {
                string auxbanco = "";
                if (item.DocumentoPagarMestre.Banco != null)
                    auxbanco = item.DocumentoPagarMestre.Banco.nomeBanco;

                rets.Add(new DocumentoPagarMestreAutorizacao()
                {
                    banco = auxbanco,
                    dataCompetencia = item.DocumentoPagarMestre.dataCompetencia.ToString(),
                    dataLancamento = item.DocumentoPagarMestre.dataLancamento.ToShortDateString(),
                    dataVencimento = item.vencimentoPrevisto.ToShortDateString(),
                    id = item.id,
                    pessoa = item.DocumentoPagarMestre.Pessoa.nome,
                    tipodocumento = item.DocumentoPagarMestre.tipoDocumento.descricao,
                    valorBruto = item.valor.ToString("n"),
                    numerodocumento = item.DocumentoPagarMestre.numeroDocumento.ToString()
                });
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

            return Json(rets, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult Prorrogar(int ID)
        //{
        //    try
        //    {
        //        DocumentoPagarMestreVlw2 obj = CarregaProrrocacaoBaixaManual(ID);
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}


        //public ActionResult BaixaManual(int ID)
        //{
        //    try
        //    {
        //        DocumentoPagarMestreVlw2 obj = CarregaProrrocacaoBaixaManual(ID);

        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}


        //[HttpPost]
        //public ActionResult BaixaManual(int id, DateTime dataPagamanto)
        //{
        //    try
        //    {
        //        DbControle banco = new DbControle();

        //        using (var dbcxtransaction = banco.Database.BeginTransaction())
        //        {
        //            var documento = new DocumentoPagarMestre().ObterPorId(id, banco);
        //            documento.dataPagamanto = dataPagamanto;

        //            documento.Alterar(documento, null, banco);

        //            dbcxtransaction.Commit();
        //        }

        //        ViewBag.msg = "Documento alterado com sucesso "
        //            + id.ToString() + " - "
        //            + dataPagamanto.ToShortDateString();
        //        //return View();
        //        return Prorrogar(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}

        //private DocumentoPagarMestreVlw2 CarregaProrrocacaoBaixaManual(int ID)
        //{
        //    DocumentoPagarMestreVlw2 obj = new DocumentoPagarMestreVlw2();
        //    CarregaViewData();

        //    var documento = new DocumentoPagarMestre().ObterPorId(ID);

        //    obj.banco_id = documento.banco_id;
        //    obj.codigoPagamento = documento.codigoPagamento;
        //    obj.dataCompetencia = documento.dataCompetencia;
        //    obj.dataDocumento = documento.dataDocumento;
        //    obj.dataLancamento = documento.dataLancamento;
        //    obj.dataPagamanto = documento.dataPagamanto;
        //    obj.dataVencimento = documento.dataVencimento;
        //    obj.dataVencimentoOriginal = documento.dataVencimentoOriginal;
        //    obj.documentopagaraprovacao_id = documento.documentopagaraprovacao_id;
        //    obj.estabelecimento_id = documento.estabelecimento_id;
        //    obj.lotePagamentoBanco = documento.lotePagamentoBanco;
        //    obj.numeroDocumento = documento.numeroDocumento;
        //    obj.pessoa_id = documento.pessoa_id;
        //    obj.situacaoPagamento = documento.situacaoPagamento;
        //    obj.tipodocumento_id = documento.tipodocumento_id;
        //    obj.tipolancamento = documento.tipolancamento;
        //    obj.valorBruto = documento.valorBruto;
        //    obj.linhadigitavel = documento.LinhaDigitavel;

        //    ViewData["DocumentoPagarDetalhe"] = new DocumentoPagarDetalhe().ObterPorCPAG(documento.id);
        //    ViewData["SemBarras"] = "S";

        //    var tipoDoc = new TipoDocumento().ObterPorId(documento.tipodocumento_id).codigo;

        //    if (tipoDoc == "GPS")
        //    {
        //        var gps = new GPS().ObterPorCPAG(documento.id);
        //        if (gps != null)
        //        {
        //            obj.GPScodigoPagamento = gps.codigoPagamento;
        //            obj.GPScompetencia = gps.competencia;
        //            obj.GPSidentificador = gps.identificador;
        //            obj.GPSinformacoesComplementares = gps.informacoesComplementares;
        //            obj.GPSnomeContribuinte = gps.nomeContribuinte;
        //            obj.GPSvalorArrecadado = gps.valorArrecadado;
        //            obj.GPSvalorAtualizacaoMonetaria = gps.valorAtualizacaoMonetaria;
        //            obj.GPSvalorOutrasEntidades = gps.valorOutrasEntidades;
        //            obj.GPSvalorTributo = gps.valorTributo;
        //            obj.GPSdataArrecadacao = gps.dataArrecadacao;
        //        }
        //    }


        //    if (tipoDoc == "DARF")
        //    {
        //        var darf = new DARF().ObterPorCPAG(documento.id);

        //        if (darf != null)
        //        {
        //            obj.DARFcnpj = darf.cnpj;
        //            obj.DARFcodigoReceita = darf.codigoReceita;
        //            obj.DARFdataPagamento = darf.dataPagamento;
        //            obj.DARFdataVencimento = darf.dataVencimento;
        //            obj.estabelecimento_id = darf.estabelecimento_id;
        //            obj.DARFjurosEncargos = darf.jurosEncargos;
        //            obj.DARFmulta = darf.multa;
        //            obj.DARFnomeContribuinte = darf.nomeContribuinte;
        //            obj.DARFnumeroReferencia = darf.numeroReferencia;
        //            obj.DARFperiodoApuracao = darf.periodoApuracao;
        //            obj.DARFvalorPrincipal = darf.valorPrincipal;
        //            obj.DARFvalorTotal = darf.valorTotal;
        //        }
        //    }

        //    if (tipoDoc == "FGTS")
        //    {

        //        var fgts = new FGTS().ObterPorCPAG(documento.id);

        //        if (fgts != null)
        //        {
        //            obj.FGTScnpj = fgts.cnpj;
        //            obj.FGTScodigoBarras = fgts.codigoBarras;
        //            obj.FGTScodigoReceita = fgts.codigoReceita;
        //            obj.FGTSdataPagamento = fgts.dataPagamento;
        //            obj.FGTSdigitoLacre = fgts.digitoLacre;
        //            obj.estabelecimento_id = fgts.estabelecimento_id;
        //            obj.FGTSidentificadorFgts = fgts.identificadorFgts;
        //            obj.FGTSnomeContribuinte = fgts.nomeContribuinte;
        //            obj.FGTStipoInscricao = fgts.tipoInscricao;
        //            obj.FGTSvalorPagamento = fgts.valorPagamento;
        //        }
        //    }
        //    return obj;
        //}


        //public ActionResult Arquivo(int id)
        //{
        //    ViewData["id"] = id;
        //    ViewData["objs"] = new DocumentoPagarArquivo().ObterPorCPAG(id);
        //    return View();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Arquivo(int id, FormCollection formCollection)
        //{
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        HttpPostedFileBase arquivo = Request.Files[i];
        //        string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png" };
        //        if (arquivo.FileName != "")
        //        {
        //            if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
        //            {
        //                ViewBag.msg = "Impossivel salvar, extenção não permitida";
        //                return RedirectToAction("Arquivo", new { id = id });
        //            }
        //        }
        //    }

        //    var pessoa = new Pessoa();

        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        HttpPostedFileBase arquivo = Request.Files[i];

        //        if (arquivo.ContentLength > 0)
        //        {
        //            var uploadPath = Server.MapPath("~/TXTTemp/");
        //            Directory.CreateDirectory(uploadPath);

        //            var nomearquivonovo = arquivo.FileName;

        //            string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

        //            arquivo.SaveAs(caminhoArquivo);

        //            AzureStorage.UploadFile(caminhoArquivo,
        //                        "CPAG/" + id.ToString() + "/" + nomearquivonovo,
        //                        ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

        //            var db = new DbControle();

        //            var documentoPagarArquivo = new DocumentoPagarArquivo();
        //            documentoPagarArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
        //                        "CPAG/" + id.ToString() + "/" + nomearquivonovo;
        //            documentoPagarArquivo.arquivoOriginal = nomearquivonovo;
        //            documentoPagarArquivo.documentoPagarMestre_id = id;
        //            documentoPagarArquivo.Salvar();
        //        }
        //    }
        //    ViewBag.msg = "Alterado com sucesso ";
        //    return RedirectToAction("Arquivo", new { id = id });
        //}


        //public ActionResult RemoveArquivo(int id)
        //{
        //    var documentoPagarArquivo = new DocumentoPagarArquivo().ObterPorId(id);


        //    AzureStorage.DeleteFile(documentoPagarArquivo.arquivoReal,
        //                        "CPAG/" + documentoPagarArquivo.documentoPagarMestre_id.ToString() + "/" + documentoPagarArquivo.arquivoOriginal,
        //                        ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

        //    new DocumentoPagarArquivo().Excluir(id);

        //    return RedirectToAction("Arquivo", new { id = documentoPagarArquivo.documentoPagarMestre_id });
        //}

    }
}


