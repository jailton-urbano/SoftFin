using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using System.Web.Helpers;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.Web.Mvc.Html;
using System.Globalization;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Reflection;
using System.IO;
using System.Text;

using SoftFin.Utils;
using SoftFin.NFe.DTO;
using SoftFin.Web.Regras;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Configuration;

using System.Threading;

namespace SoftFin.Web.Controllers
{
	public class NFeController : BaseController
	{
        #region Public

        public JsonResult ObterOrdemVendaAberto()
        {
            var Listas = PesquisaNFse2();


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
                                codigoServico = (p.parcelaContrato_ID == null) ? "" : (
                                            p.ParcelaContrato.codigoServicoEstabelecimento_id == null ? "" :
                                            p.ParcelaContrato.CodigoServicoEstabelecimento.CodigoServicoMunicipio.codigo),

                                DataVencimentoOriginal = (p.ParcelaContrato != null) ?
                                        (
                                            (p.ParcelaContrato.DataVencimento != null) ? p.ParcelaContrato.DataVencimento.Value.ToString("o") : "") : "",
                                nome = p.Pessoa.nome,
                                pessoanome = p.Pessoa.nome + ", " + p.Pessoa.cnpj,
                                unidadeNegocio_ID = p.unidadeNegocio_ID,
                                unidade = p.UnidadeNegocio.unidade,
                                p.descricao,
                                descricaoparcela = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.descricao,
                                pedido = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.ContratoItem.pedido,
                                contrato = (p.parcelaContrato_ID == null) ? "" : (p.ParcelaContrato.ContratoItem.Contrato.contrato),
                                descricaocontrato = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.ContratoItem.Contrato.descricao,
                                numeroparcela = (p.parcelaContrato_ID == null) ? "" : p.ParcelaContrato.parcela.ToString(),

                                banco_id = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.banco_id == null) ? "" : p.ParcelaContrato.banco_id.ToString()),
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
                                operacao_id = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.operacao_id == null) ? "" : p.ParcelaContrato.operacao_id.ToString()),
                                operacao = (p.parcelaContrato_ID == null) ? "" : ((p.ParcelaContrato.operacao_id == null)
                                                ? "" : p.ParcelaContrato.Operacao.descricao),
                                pessoaid = p.pessoas_ID,
                                tabelaPreco_ID = (p.tabelaPreco_ID != null) ? p.tabelaPreco_ID.Value : 0


                            }
                            )
                            , JsonRequestBehavior.AllowGet
                        )
                 ;

        }

        private List<OrdemVenda> PesquisaNFse2()
        {
            var obj = new OrdemVenda();
            var date = DateTime.Now.AddMonths(1);
            IEnumerable<OrdemVenda> lista;

            if (_estabobj.autorizacaoFaturamento == null)
                _estabobj.autorizacaoFaturamento = false;

            if (_estabobj.autorizacaoFaturamento.Value)
                lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.data <= date).Where(p => p.TipoFaturamento ==1) ;
            else
                lista = obj.ObterTodosPendentes(_paramBase).Where(x => x.data <= date).Where(p => p.TipoFaturamento == 1);

            return lista.OrderBy(p => p.data).ToList();
        }



        public ActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public JsonResult TotalizadorAutorizacao(int? id)
		{
			base.TotalizadorDash(id);
			var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.usuarioAutorizador_id == null).Sum(x => x.valor).ToString("n");
			return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult AlterarVencimento(int id, int estab, DateTime vencimento)
		{

			try
			{
				if (estab != _estab)
				{


					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}


				DbControle db = new DbControle();

				using (var dbcxtransaction = db.Database.BeginTransaction())
				{

					var nf = new NotaFiscal().ObterPorId(id, db);
					nf.dataVencimentoNfse = vencimento;

					//Atualiza Banco Movimento
					var banco = db.BancoMovimento.Where(x => x.notafiscal_id == id).FirstOrDefault();
					if (banco != null)
					{
						banco.data = vencimento;
					}

					nf.usuarioalteracaoid = _usuarioobj.id;
					nf.dataAlteracao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();

					if (nf.Alterar(_paramBase, db))
					{
						dbcxtransaction.Commit();
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Alterado com sucesso"

						});


					}
					else
					{
						dbcxtransaction.Rollback();
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Impossivel alterar, registro excluído"

						});
					}

				}

			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}

		public JsonResult ListaPessoas()
		{
            return Json(new Pessoa().ObterTodosComCNPJ(_paramBase), JsonRequestBehavior.AllowGet);
		}

		public JsonResult ListaMunicipios()
		{
			return Json(new Municipio().ObterTodos(), JsonRequestBehavior.AllowGet);
		}


	   

		public JsonResult HistoricoDetalhe(int id)
		{
			try
			{
				var listaErros = new LogNFXMLErro().ObterTodosPorCapa(id);
				var listaAlertas = new LogNFXMLAlerta().ObterTodosPorCapa(id);
				return Json(new { CDStatus = "OK", 
								  listaAlertas = listaAlertas.Select( p=>new {p.codigo,p.descricao}),
								  listaErros = listaErros.Select(p => new { p.codigo, p.descricao })
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}
		public JsonResult ObterUnidadeNegocios()
		{
			var objs = new UnidadeNegocio().ObterTodos(_paramBase);


			return Json(objs.Select(p => new
			{
				Value = p.id,
				Text = p.unidade
			}), JsonRequestBehavior.AllowGet);
		}


		public JsonResult ObterItemProdutoServicos()
		{
			var objs = new ItemProdutoServico().ObterTodos(_paramBase);


			return Json(objs.Select(p => new
			{
				Value = p.id,
				Text = p.descricao
			}), JsonRequestBehavior.AllowGet);
		}

		public JsonResult ObterTabelaPrecoItemProdutoServicos()
		{
			var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);


			return Json(objs.Select(p => new
			{
				Value = p.id,
				Text = p.descricao
			}), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public override JsonResult TotalizadorDash(int? id)
		{
			base.TotalizadorDash(id);
			var soma = new NotaFiscal().ObterEntreDataSomenteNFe(DataInicial, DataFinal, _paramBase).Where(p => p.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA || p.situacaoPrefeitura_id == Models.NotaFiscal.NFGERADAENVIADA || p.situacaoPrefeitura_id == Models.NotaFiscal.NFBAIXA).Sum(x => x.valorNfse).ToString("n");
			return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);
		}

		public class dtoFiltro
		{
			public DateTime? dataEmissaoRpsIni { get; set; }
			public DateTime? dataEmissaoRpsFim { get; set; }
			public DateTime? dataVencimentoNfseIni { get; set; }
			public DateTime? dataVencimentoNfseFim { get; set; }
			public decimal? valorBrutoIni { get; set; }
			public decimal? valorBrutoFim { get; set; }
            public int Numero { get;  set; }
        }


		[HttpPost]
		public JsonResult ObterNF(dtoFiltro data)
		{
			var Listas = PesquisaNFse();

            if (data.Numero != 0)
                Listas = Listas.Where(p => p.OrdemVenda.Numero <= data.Numero).ToList();

            if (data.dataEmissaoRpsIni != null)
				Listas = Listas.Where(p => p.dataEmissaoRps >= data.dataEmissaoRpsIni).ToList();

			if (data.dataEmissaoRpsFim != null)
				Listas = Listas.Where(p => p.dataEmissaoRps <= data.dataEmissaoRpsFim.Value.AddDays(1)).ToList();


			if (data.dataVencimentoNfseIni != null)
				Listas = Listas.Where(p => p.dataVencimentoNfse >= data.dataVencimentoNfseIni).ToList();

			if (data.dataVencimentoNfseFim != null)
				Listas = Listas.Where(p => p.dataVencimentoNfse <= data.dataVencimentoNfseFim.Value.AddDays(1)).ToList();

			if (data.valorBrutoIni != null)
				Listas = Listas.Where(p => p.valorNfse >= data.valorBrutoIni).ToList();

			if (data.valorBrutoFim != null)
				Listas = Listas.Where(p => p.valorNfse <= data.valorBrutoFim).ToList();


			return Json(
							Listas.Select(p => new
							{
								p.numeroNfe,
								p.serieNfe,
								p.loteNfe,
								dataEmissaoNfse = (p.dataEmissaoNfse == null) ? "" : p.dataEmissaoNfse.ToString("o"),
								dataVencimentoNfse = (p.dataVencimentoNfse == null) ? "" : p.dataVencimentoNfse.ToString("o"),
                                DataVencimentoOriginal = (p.DataVencimentoOriginal == null) ? "" : p.DataVencimentoOriginal.ToString("o"),
                                razao = (p.NotaFiscalPessoaTomador == null) ? ((p.NotaFiscalPessoaPrestador == null) ? "" : p.NotaFiscalPessoaPrestador.razao) : p.NotaFiscalPessoaTomador.razao,
								p.NotaFiscalNFE.valor,
								p.NotaFiscalNFE.CFOP,
								p.NotaFiscalNFE.situacao,
								p.estabelecimento_id,
								p.entradaSaida, 
								p.id,
                                p.OrdemVenda.Numero


							}
							)
							, JsonRequestBehavior.AllowGet
						)
				 ;

		}

		//public JsonResult NFXMLConsulta(NotaFiscal obj)
		//{
		//    try
		//    {

		//        if (obj.estabelecimento_id != _estab)
		//        {


		//            return Json(new
		//            {
		//                CDStatus = "NOK",
		//                DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
		//            });

		//        }
		//        if (Acesso.pb.estab_idObj().senhaCertificado == null)
		//        {
		//            return Json(new
		//            {
		//                CDStatus = "NOK",
		//                DSMessage = "Certificado não configurado para esta estabelecimento."
		//            }, JsonRequestBehavior.AllowGet);
		//        }
		//        DbControle db;
		//        NotaFiscal objNF;
		//        DTORetornoNFe resultado;

		//        AtualizaNFBussines(obj.id, out db, out objNF, out resultado);

		//        if (resultado.Cabecalho.Sucesso.ToLower() == "false")
		//        {
		//            return Json(new
		//            {
		//                CDStatus = "NOK",
		//                DSMessage = "Comando não aceito pela prefeitura, a seguir os erros:",
		//                Erros = resultado.Erro,
		//                Alertas = resultado.Alerta
		//            });
		//        }
		//        else
		//        {
		//            new NotaFiscalCalculos().BaixaNF(objNF,
		//                DateTime.Parse(resultado.NFe.First().DataEmissaoNFe),
		//                resultado.NFe.First().ChaveNFe.CodigoVerificacao,
		//                int.Parse(resultado.NFe.First().ChaveNFe.NumeroNFe),
		//                resultado.NFe.First().StatusNFe,
		//                db);

		//            if (resultado.Alerta.Count() == 0)
		//                return Json(new
		//                {
		//                    CDStatus = "OK",
		//                    DSMessage = "Comando aceito pela prefeitura",
		//                    Erros = resultado.Erro,
		//                    Alertas = resultado.Alerta
		//                });
		//            else
		//                return Json(new
		//                {
		//                    CDStatus = "OK",
		//                    DSMessage = "Comando aceito pela prefeitura, a seguir os alertas:",
		//                    Erros = resultado.Erro,
		//                    Alertas = resultado.Alerta
		//                });

		//        }


		//    }
		//    catch (Exception ex)
		//    {
		//        return Json(new
		//        {
		//            CDStatus = "NOK",
		//            DSMessage = ex.Message.ToString()
		//        });
		//    }
		//}
		public JsonResult NFXMLEnvio(NotaFiscal obj)
		{


			try
			{
				if (obj.estabelecimento_id != _estab)
				{


					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}
				if (_estabobj.senhaCertificado == null)
				{
					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Certificado não configurado para esta estabelecimento."
					}, JsonRequestBehavior.AllowGet);
				}

				var resultado = EnviaNFBussiness(obj.id);

				if (!resultado.Sucesso)
				{
					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Comando não aceito, a seguir os erros:",
						Erros = resultado.Erros,
						Alertas = resultado.Alertas
					});
				}
				else
				{
					if (resultado.Alertas.Count() == 0)
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Comando aceito.",
							Erros = resultado.Erros,
							Alertas = resultado.Alertas
						});
					else
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Comando aceito, a seguir os alertas:",
							Erros = resultado.Erros,
							Alertas = resultado.Alertas
						});
				}

			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}


		

		[HttpGet]
		public FileResult HistorioDowloadXML(int id)
		{
			try
			{
				var log = new LogNFXMLPrincipal().ObterPorId(id, _paramBase);

				if (log.NotaFiscal.estabelecimento_id != _estab)
					throw new Exception("XML não encontrado");


				var uploadPath = Server.MapPath("~/TXTTemp/");
				Directory.CreateDirectory(uploadPath);

				var filename = "SoftFin-HistoricoXML_" + _estab.ToString() + "_" + id.ToString() + ".xml";
				string filepath = uploadPath + "\\" + filename;

				System.IO.File.WriteAllText(filepath, log.xml);

				byte[] filedata = System.IO.File.ReadAllBytes(filepath);
				string contentType = MimeMapping.GetMimeMapping(filepath);

				var cd = new System.Net.Mime.ContentDisposition
				{
					FileName = filename,
					Inline = true,
				};

				Response.AppendHeader("Content-Disposition", cd.ToString());

				return File(filedata, contentType);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				throw new HttpException(ex.Message);
			}
		}


		[HttpGet]
		public FileResult GerarXML(int id)
		{
			try
			{

				var resultado = EnviaNFBussiness(id,true);

				var uploadPath = Server.MapPath("~/TXTTemp/");
				Directory.CreateDirectory(uploadPath);
				var filename = "SoftFin-NFe.xml";

				string filepath = uploadPath + "\\" + filename;

				resultado.xml.Save(filepath);
				byte[] filedata = System.IO.File.ReadAllBytes(filepath);
				string contentType = MimeMapping.GetMimeMapping(filepath);

				var cd = new System.Net.Mime.ContentDisposition
				{
					FileName = filename,
					Inline = true,
				};

				Response.AppendHeader("Content-Disposition", cd.ToString());

				return File(filedata, contentType);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				throw new HttpException(ex.Message);
			}
		}


		[HttpGet]
		public FileResult GerarDANFE(int id)
		{
			try
			{

				var objNF = new NotaFiscal().ObterPorId(id);
				var dTONfe = new DTONfe();
				var listaNF = new List<NotaFiscal>();
				listaNF.Add(objNF);
				new Conversao().ConverterNFe(dTONfe, _estabobj, listaNF);
				var uploadPath = Server.MapPath("~/TXTTemp/");
				Directory.CreateDirectory(uploadPath);
				var uploadPathTemplate = Server.MapPath("~/Template/PDF/");

				var filenameTemplate = "DANFE.pdf";
				var filename = "DANFE_" + _estabobj.Codigo + "_" + SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("yyyyMMddhhmmss") +".PDF";
				var filenamePNG = "DANFE_" + _estabobj.Codigo + "_" + SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("yyyyMMddhhmmss") + ".PNG";

				string filepath = uploadPath + "\\" + filename;
				string filepathPNG = uploadPath + "\\" + filenamePNG;
				string filepathTemplate = uploadPathTemplate + "\\" + filenameTemplate;

				using (var template = new FileStream(filepathTemplate, FileMode.Open))
				using (var newpdf = new FileStream(filepath, FileMode.Create))
				{
					var pdfReader = new PdfReader(template);
					Document document = new Document();
					PdfWriter writer = PdfWriter.GetInstance(document, newpdf);

					document.Open();
					PdfContentByte cb = writer.DirectContent;
					PdfImportedPage page = writer.GetImportedPage(pdfReader, 1);

					document.NewPage();
					cb.AddTemplate(page, 0, 0);

					var fnt = new Font(Font.FontFamily.TIMES_ROMAN, 10);
					fnt.Color = BaseColor.BLACK;

					float altura = 725;
					float largura = 70;
					float inicio = 25;
					float fim = 210;

					if (!string.IsNullOrEmpty(_estabobj.Logo))
					{
						var arquivo = _estabobj.Logo;
						var request = WebRequest.Create(arquivo);
						using (var response = request.GetResponse())
						{
							using (var responseStream = response.GetResponseStream())
							{
								iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(arquivo);
								logo.ScalePercent(100f);
								logo.SetAbsolutePosition(70, 720);
								document.Add(logo);
							}
						}
					}
					//CodeBar

					BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
					{
						IncludeLabel = true,
						Alignment = BarcodeLib.AlignmentPositions.LEFT,
						Width = 50,
						Height = 50,
						RotateFlipType =  System.Drawing.RotateFlipType.RotateNoneFlipNone,
						BackColor = System.Drawing.Color.White,
						ForeColor = System.Drawing.Color.Black
					};

					var img = BarcodeLib.Barcode.DoEncode(BarcodeLib.TYPE.CODE128C, dTONfe.InfNFe.Ide.chavenota);

					img.Save(filepathPNG,System.Drawing.Imaging.ImageFormat.Png);

					iTextSharp.text.Image codebarfile = iTextSharp.text.Image.GetInstance(filepathPNG);
					codebarfile.ScaleAbsolute(270, 30);
					codebarfile.SetAbsolutePosition(325, 740);
					document.Add(codebarfile);


					//Logo
					GravaTexto(dTONfe.InfNFe.Emi.xFant + "  Cnpj: "
						+ dTONfe.InfNFe.Emi.CNPJ.Substring(0, 2) + "." + 
						dTONfe.InfNFe.Emi.CNPJ.Substring(2, 3) + "." + 
						dTONfe.InfNFe.Emi.CNPJ.Substring(5, 3) + "/" + 
						dTONfe.InfNFe.Emi.CNPJ.Substring(8, 4) + "-" + 
						dTONfe.InfNFe.Emi.CNPJ.Substring(12, 2) + ""
						, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 6);
					altura = 705;
					GravaTexto(dTONfe.InfNFe.Emi.EnderEmit.xLgr + ", "
						+ dTONfe.InfNFe.Emi.EnderEmit.nro + " "
						+ dTONfe.InfNFe.Emi.EnderEmit.xCpl + " "
						+ dTONfe.InfNFe.Emi.EnderEmit.xBairro + " - "
						+ dTONfe.InfNFe.Emi.EnderEmit.xMun + " - "
						+ dTONfe.InfNFe.Emi.EnderEmit.UF + " - CEP: "
						+ dTONfe.InfNFe.Emi.EnderEmit.CEP

					, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 6);

					//for (altura = 825; altura > 0; altura -= 25)
					//{
					//    for (inicio = 25; inicio < 700; inicio += 50)
					//    {
					//        GravaTexto(altura + "X" + inicio, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					//    }                         
					//}

					altura = 820;
					largura = 4;
					inicio = 25;
					fim = 700;

					//RECEBEMOS DE
					GravaTexto(dTONfe.InfNFe.Emi.xNome + "  Cnpj: "
						+ dTONfe.InfNFe.Emi.CNPJ.Substring(0, 2) + "." +
						dTONfe.InfNFe.Emi.CNPJ.Substring(2, 3) + "." +
						dTONfe.InfNFe.Emi.CNPJ.Substring(5, 3) + "/" +
						dTONfe.InfNFe.Emi.CNPJ.Substring(8, 4) + "-" +
						dTONfe.InfNFe.Emi.CNPJ.Substring(12, 2) + "", cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

					altura = 820;
					fim = 580;
					//NF-e RECEBEMOS DE
					GravaTexto(dTONfe.InfNFe.Ide.nNF, cb, fnt, fim, altura);
					altura = 800;
					GravaTexto(dTONfe.InfNFe.Ide.serie, cb, fnt, fim, altura);

					//DANFE
					altura = 740;
					inicio = 305;
					GravaTexto(dTONfe.InfNFe.Ide.tpNF, cb, fnt, inicio, altura);
					fim = 750;
					altura = 735;
					inicio = 340;
					GravaTexto(dTONfe.InfNFe.Ide.chavenota, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT,9);

					altura = 718;
					inicio = 320;
					GravaTexto(dTONfe.InfNFe.Ide.nNF, cb, fnt, inicio, altura);
					altura = 708;
					GravaTexto(dTONfe.InfNFe.Ide.serie, cb, fnt, inicio, altura);
					altura = 698;
					GravaTexto("1 de 1", cb, fnt, inicio, altura);

					//NATUREZA OPERACAO
					altura = 673;
					inicio = 25;
					fim = 700;
					GravaTexto(dTONfe.InfNFe.Ide.natOp, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);


					inicio = 350;
					GravaTexto(dTONfe.InfNFe.Ide.nProt, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					
					
					
					//INSCRICAO ESTADUAL
					altura = 650;
					inicio = 25;

					GravaTexto(dTONfe.InfNFe.Emi.IE, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					if (dTONfe.InfNFe.Emi.CNPJ != null)
					{
						inicio = 413;
						var cnpj = dTONfe.InfNFe.Emi.CNPJ;
						GravaTexto(cnpj, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT,4);
					}

					//NOME RAZAO SOCIAL
					altura = 616;
					inicio = 25;
					GravaTexto(dTONfe.InfNFe.Dest.xNome, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					if (dTONfe.InfNFe.Emi.CNPJ != null)
					{
						inicio = 400;
						var cnpj = dTONfe.InfNFe.Dest.CNPJ;
						GravaTexto(cnpj, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 4);
					}
					inicio = 525;
					GravaTexto(dTONfe.InfNFe.Ide.dhEmi, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 2);

					//ENDERECO
					altura = 595;
					inicio = 25;
					GravaTexto(dTONfe.InfNFe.Dest.EnderDest.xLgr, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 350;
					GravaTexto(dTONfe.InfNFe.Dest.EnderDest.xBairro, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 460;
					GravaTexto(dTONfe.InfNFe.Dest.EnderDest.CEP, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 525;
					GravaTexto(dTONfe.InfNFe.Ide.dhSaiEnt, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 2);

					//Municipio
					altura = 571;
					inicio = 25;
					GravaTexto(dTONfe.InfNFe.Dest.EnderDest.xMun, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 245;
					GravaTexto(dTONfe.InfNFe.Dest.EnderDest.fone, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 328;
					GravaTexto(dTONfe.InfNFe.Dest.EnderDest.UF, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 395;
					GravaTexto(dTONfe.InfNFe.Dest.IE, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 525;
					GravaTexto(dTONfe.InfNFe.Ide.dhSaiEnt, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 3);


					altura = 492;
					fim = 0;
					inicio = 125;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vBC, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 245;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vICMS, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 350;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vBCST, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 470;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vST, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 580;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vProd, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);


					altura = 470;
					inicio = 100;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vFrete, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 200;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vSeg, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 280;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vDesc, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 375;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vOutro, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 470;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vIPI, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);
					inicio = 580;
					GravaTexto(dTONfe.InfNFe.Total.ICMSTot.vNF, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8);

					//TRANSPORTADOR
					altura = 441;
					inicio = 25;
					fim = 700;
					GravaTexto(dTONfe.InfNFe.Transp.Transporta.xNome, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 295;
					GravaTexto(dTONfe.InfNFe.Transp.modFrete, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 310;
					GravaTexto(dTONfe.InfNFe.Transp.VeicTransp.RNTC, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 360;
					GravaTexto(dTONfe.InfNFe.Transp.VeicTransp.placa, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 412;
					GravaTexto(dTONfe.InfNFe.Transp.VeicTransp.UF, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 460;
					GravaTexto(dTONfe.InfNFe.Transp.Transporta.CNPJ, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT, 4);

					//TRANSPORTADOR
					altura = 420;
					inicio = 25;
					GravaTexto(dTONfe.InfNFe.Transp.Transporta.xEnder, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 245;
					GravaTexto(dTONfe.InfNFe.Transp.Transporta.xMun, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 412;
					GravaTexto(dTONfe.InfNFe.Transp.Transporta.UF, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
					inicio = 460;
					GravaTexto(dTONfe.InfNFe.Transp.Transporta.IE, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

					//volume


					//Produtos
					altura = 355;

					fnt = new Font(Font.FontFamily.TIMES_ROMAN, 7);
					fnt.Color = BaseColor.BLACK;
					foreach (var item in dTONfe.InfNFe.Det)
					{
						string produtoExtra = "";
						if (!string.IsNullOrEmpty(item.Prod.infAdProd))
							produtoExtra = " \r " + item.Prod.infAdProd;

						inicio = 25;
						fim = 700;
						GravaTexto(item.Prod.cProd, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
						inicio = 75;
						fim = 250;
						GravaTexto(item.Prod.xProd + produtoExtra, cb, fnt, fim, altura - 7, inicio, largura, Element.ALIGN_LEFT,7);
						inicio = 265;
						fim = 0;
						GravaTexto(item.Prod.NCM, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT);
						inicio = 281;
						GravaTexto("000", cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT); //CST
						inicio = 298;
						GravaTexto(item.Prod.CFOP, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT); //CFOP
						inicio = 320;
						GravaTexto(item.Prod.qTrib, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT); //Quantidade
						inicio = 340;
						GravaTexto(item.Prod.uCom, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //V. Unitário
						inicio = 389;
						GravaTexto(item.Prod.vUnTrib, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //V. Unitário
						inicio = 431;
						GravaTexto(item.Prod.vProd, cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //Valor Total
						inicio = 474;
						GravaTexto("0.00", cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //Base Calc. ICMS
						inicio = 508;
						GravaTexto("0.00", cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //Valor ICMS 
						inicio = 547;
						GravaTexto("0.00", cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //Valor IPI 
						inicio = 568;
						GravaTexto("0.00", cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT, 8); //Aliq. ICMS
						inicio = 590;
						GravaTexto("0.00", cb, fnt, inicio, altura, fim, largura, Element.ALIGN_RIGHT,8); //Aliq. IPI
						altura -= 25;
					}

					inicio = 25;
					fim = 350;
					largura = 80;
					altura = 115;
					GravaTexto(dTONfe.InfNFe.InfAdic.infCpl, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

					inicio = 775;
					fim = 375;
					largura = 80;
					altura = 115;
					GravaTexto(dTONfe.InfNFe.InfAdic.infAdFisco, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);


					document.Close();  
					writer.Close();
				}
				
				
				
				byte[] filedata = System.IO.File.ReadAllBytes(filepath);
				string contentType = MimeMapping.GetMimeMapping(filepath);

				var cd = new System.Net.Mime.ContentDisposition
				{
					FileName = filename,
					Inline = true,
				};

				Response.AppendHeader("Content-Disposition", cd.ToString());

				return File(filedata, contentType);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				throw new HttpException(ex.Message);
			}
		}


		[HttpGet]
		public FileResult Download(int id)
		{
			try
			{
				var nf = new NotaFiscal().ObterPorId(id);

				if (nf.estabelecimento_id != _estab)
					throw new Exception("XML não encontrado");

                
 
				var uploadPath = Server.MapPath("~/TXTTemp/");
				Directory.CreateDirectory(uploadPath);

				var filename = "SoftFin-NFXML_" + _estab.ToString() + "_" + id.ToString() + ".txt";
				string filepath = uploadPath + "\\" + filename;

				string caminhoArquivo;
				System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
				ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);
				DbControle db = new DbControle();
				
				var url = db.UrlSefazUF.Where(p => p.UF == _estabobj.UF);

				var urlServico = "";
				if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
				{
					urlServico = url.Where(p => p.codigo == "NFeDonwload").First().UrlSefazPrincipal.url;
				}
				else
				{
					urlServico = url.Where(p => p.codigo == "NFeDonwload").First().UrlSefazPrincipal.urlHomologacao;
				}
				
				
				var service = new SoftFin.NFe.Business.ConsultaNFe();

				var retorno = service.Execute(nf.NotaFiscalNFE.chaveAcesso, cert, "", urlServico);


				System.IO.File.WriteAllText(filepath, retorno.xmlRetorno);

				byte[] filedata = System.IO.File.ReadAllBytes(filepath);
				string contentType = MimeMapping.GetMimeMapping(filepath);

				var cd = new System.Net.Mime.ContentDisposition
				{
					FileName = filename,
					Inline = true,
				};

				Response.AppendHeader("Content-Disposition", cd.ToString());

				return File(filedata, contentType);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				throw new HttpException(ex.Message);
			}
		}
		private static void GravaTexto(string value, PdfContentByte cb, Font fnt, float x, float y, float x2 = 0, float y2=10, int alinhamento = Element.ALIGN_RIGHT, int tipo =0)
		{
			ColumnText ct;
			Paragraph par;

			ct = new ColumnText(cb);
			ct.SetSimpleColumn(x, y, x2, y2);
			if (value == null)
			{
				value = "------";
			}
			else
			{

				if (tipo == 2) // Data
				{
					var data = DateTime.Parse(value);
					value = data.ToString("dd/MM/yyyy");
				}
				else if (tipo == 3) // Hora
				{
					var data = DateTime.Parse(value);
					value = data.ToString("hh:mm:ss");
				}
				else if (tipo == 4) // CNPJ
				{
					value = value.Substring(0, 2) + "." + value.Substring(2, 3) + "." + value.Substring(5, 3) + "/" + value.Substring(8, 4) + "-" + value.Substring(12, 2);
				}
				else if (tipo == 5) // CPF
				{
					value = value.Substring(0, 3) + "." + value.Substring(3, 3) + "." + value.Substring(6, 3) + "-" + value.Substring(9, 2);
				}
				else if (tipo == 8) // numero
				{
					value = value.Replace(".",",");
				}
				else if (tipo == 9) // numero
				{
					 value =  value.Substring(0, 4) 
									+ " " + value.Substring(4, 4) 
									+ " " + value.Substring(8, 4)
									+ " " + value.Substring(12, 4)
									+ " " + value.Substring(16, 4)
									+ " " + value.Substring(20, 4)
									+ " " + value.Substring(24, 4)
									+ " " + value.Substring(28, 4)
									+ " " + value.Substring(32, 4)
									+ " " + value.Substring(36, 4)
									+ " " + value.Substring(40, 4);
				}
			}

			par = new Paragraph(value, fnt);
			par.Alignment = alinhamento;
			ct.Alignment = alinhamento;
			if (tipo == 6)
			{
				ct.SetLeading(1, 1);
				par.Alignment = Element.ALIGN_CENTER;
				ct.Alignment = Element.ALIGN_CENTER;
			}
			if (tipo == 7)
			{
				ct.SetLeading(1, 1);
			}
			ct.AddText(par);
			ct.Go();
		}

		[HttpPost]
		public JsonResult Cancelamento(NotaFiscal obj, string motivo)
		{
			try
			{
				if (obj.estabelecimento_id != _estab)
				{
					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}


				if (string.IsNullOrEmpty( motivo) )
				{
					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Informe o motivo",
					});

				}

				var db = new DbControle();
				using (var dbcxtransaction = db.Database.BeginTransaction())
				{
					var nf = new NotaFiscal().ObterPorId(obj.id, db);
					if (nf.NotaFiscalNFE.situacao == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA)
					{
						if (nf.NotaFiscalNFE.NotaFiscalNFETransportadora_id != null)
						{
							var transp = new NotaFiscalNFETransportadora().ObterPorId(nf.NotaFiscalNFE.NotaFiscalNFETransportadora_id.Value, db);
							transp.Excluir(_paramBase, db);
						}
						if (nf.NotaFiscalNFE.NotaFiscalNFERetirada_id != null)
						{
							var retirada = new NotaFiscalNFERetirada().ObterPorId(nf.NotaFiscalNFE.NotaFiscalNFERetirada_id.Value, db);
							retirada.Excluir(_paramBase, db);
						}
						if (nf.NotaFiscalNFE.NotaFiscalNFEEntrega_id != null)
						{
							var retirada = new NotaFiscalNFEEntrega().ObterPorId(nf.NotaFiscalNFE.NotaFiscalNFEEntrega_id.Value, db);
							retirada.Excluir(_paramBase, db);
						}
						if (nf.NotaFiscalPessoaTomador != null)
						{
							nf.NotaFiscalPessoaTomador.Excluir(_paramBase, db);
						}


						var listaNFReferenciada = new NotaFiscalNFEReferenciada().ObterTodos(nf.NotaFiscalNFE.id,db);

						foreach (var item in listaNFReferenciada)
						{
							item.Excluir(_paramBase, db);
						}
						var listaDuplicatas = new NotaFiscalNFEDuplicata().ObterTodos(nf.NotaFiscalNFE.id, db);

						foreach (var item in listaDuplicatas)
						{
							item.Excluir(_paramBase, db);
						}
						var listaVolumes = new NotaFiscalNFEVolume().ObterPorNf(nf.NotaFiscalNFE.id, db);

						foreach (var item in listaVolumes)
						{
							item.Excluir(_paramBase, db);
						}

						var listaReboques = new NotaFiscalNFEReboque().ObterTodos(nf.NotaFiscalNFE.id, db);

						foreach (var item in listaReboques)
						{
							item.Excluir(_paramBase, db);
						}

						var listaItens = new NotaFiscalNFEItem().ObterPorCapa(nf.NotaFiscalNFE.id, db);

						foreach (var item in listaItens)
						{
							item.Excluir(_paramBase, db);
						}

						if (nf.ordemVenda_id != null)
						{
							var ov = new OrdemVenda().ObterPorId(nf.ordemVenda_id.Value, db);
							ov.statusParcela_ID = Models.StatusParcela.SituacaoLiberada();

							if (ov.ParcelaContrato != null)
							{
								if (ov.parcelaContrato_ID != null)
								{
									var contratoParcela = new ParcelaContrato().ObterPorId(ov.parcelaContrato_ID.Value, db, _paramBase);
									contratoParcela.statusParcela_ID = Models.StatusParcela.SituacaoLiberada();
								}
							}
						}

						var logsMestres = new LogNFXMLPrincipal().ObterTodosPorNota(nf.id, db);

						foreach (var item in logsMestres)
						{
							var logsAlertas = new LogNFXMLAlerta().ObterTodosPorCapa(item.id, db);
							foreach (var item2 in logsAlertas)
							{
								item2.Excluir(_paramBase, db);
							}
							var logsErros = new LogNFXMLErro().ObterTodosPorCapa(item.id, db);
							;
							foreach (var item2 in logsErros)
							{
								item2.Excluir(_paramBase, db);
							}
							item.Excluir(db, _paramBase);
						}

                        var bcoMovs = new BancoMovimento().ObterPorNFES(obj.id, db, _paramBase);
						foreach (var item in bcoMovs)
						{
							string erro = "";
                            item.Excluir(item.id, ref erro, _paramBase, db);
							if (erro != "")
							{
								dbcxtransaction.Rollback();
								string strerro = "Erro ao excluir o banco movimento id (" + nf.id.ToString() + ") : " + erro;
								throw new Exception(strerro);
							}
						}

						nf.NotaFiscalNFE.Excluir(_paramBase, db);
						
						string erro2 = "";

                        //  Exclusão LancamentoContabil Inicio
                        var objLcs = new LancamentoContabil().ObterPorNotaFiscal(nf.id, _paramBase, db);
                        foreach (var itemLC in objLcs)
                        {
                            var errolc = "";
                            itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                            if (errolc != "")
                                throw new Exception(errolc);
                        }
                        //  Exclusão LancamentoContabil Fim

                        nf.Excluir(ref erro2, _paramBase, db);

						if (erro2 != "")
						{
							dbcxtransaction.Rollback();
							string strerro = "Erro ao excluir a nota id (" + nf.id.ToString() + ") tabela NotaFiscal : " + erro2;
							throw new Exception(strerro);
						}

						db.SaveChanges();

					}
					else if (nf.NotaFiscalNFE.situacao == Models.NotaFiscal.NFGERADAENVIADA)
					{
						var bcoMovs = new BancoMovimento().ObterPorNFES(obj.id, db,_paramBase);
						foreach (var item in bcoMovs)
						{
							string erro = "";
							item.Excluir(item.id, ref erro,_paramBase, db);
							if (erro != "")
							{
								dbcxtransaction.Rollback();
								string strerro = "Erro ao excluir o banco movimento id (" + nf.id.ToString() + ") : " + erro;
								throw new Exception(strerro);
							}
						}
						nf.NotaFiscalNFE.situacao = Models.NotaFiscal.NFCANCELADACCONF;
						nf.NotaFiscalNFE.motivoCancelamento = motivo;
						db.SaveChanges();

						string caminhoArquivo;
						System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
						ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);

						var estabSF = new Estabelecimento().ObterPorId(3, _paramBase); // SoftFin

						string caminhoArquivoSF;
						System.Security.Cryptography.X509Certificates.X509Certificate2 certSF;
						ObtemCertificadoX509(estabSF, out caminhoArquivoSF, out certSF);

						var service = new SoftFin.NFe.Business.CancelamentoNFe();
						var serviceStatus = new SoftFin.NFe.Business.Status();


						DTOLogEvento dTOEvento = new DTOLogEvento();
						dTOEvento.cOrgao = "35";
						dTOEvento.cnpj = SoftFin.Utils.UtilSoftFin.Limpastrings(_estabobj.CNPJ);
						dTOEvento.chNFe = nf.NotaFiscalNFE.chaveAcesso;
						dTOEvento.dhEvento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("yyyy-MM-ddTHH:mm:ss") + "-03:00";
						dTOEvento.tpEvento = "110111";
						dTOEvento.nSeqEvento = "1";
						dTOEvento.verEvento = "1.00";
						dTOEvento.descEvento = "Cancelamento";
						dTOEvento.nProt = nf.NotaFiscalNFE.protocoloAutorizacao;
						dTOEvento.xJust = motivo;

						var caminhoXSD = Server.MapPath("~/XSDDOCS/NFe/Evento_Canc_PL_v1.01/envEventoCancNFe_v1.00.xsd");

						string urlServico = null;

						string urlServicoStatus = null;
						var url = db.UrlSefazUF.Where(p => p.UF == "SP");

						if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
						{
							urlServico = url.Where(p => p.codigo == "NFeEvento").First().UrlSefazPrincipal.url;
							urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;

						}
						else
						{
							urlServico = url.Where(p => p.codigo == "NFeEvento").First().UrlSefazPrincipal.urlHomologacao;
							urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;
						}


						var resultado = serviceStatus.Execute("35", certSF, urlServicoStatus);
						resultado = service.Execute(dTOEvento, cert, caminhoXSD, urlServico);
						resultado.tipo = "NFeCancelamento";
						new Conversao().ConverteRetornoGravaLog(resultado, nf.id, _usuario);

						


						if (!resultado.Sucesso)
						{
							dbcxtransaction.Rollback();
							return Json(new
							{
								CDStatus = "NOK",
								DSMessage = "Comando não aceito, a seguir os erros:",
								Erros = resultado.Erros,
								Alertas = resultado.Alertas
							});
						}
						else
						{
							dbcxtransaction.Commit();
							if (resultado.Alertas.Count() == 0)
								return Json(new
								{
									CDStatus = "OK",
									DSMessage = "Comando aceito.",
									Erros = resultado.Erros,
									Alertas = resultado.Alertas
								});
							else
								return Json(new
								{
									CDStatus = "OK",
									DSMessage = "Comando aceito, a seguir os alertas:",
									Erros = resultado.Erros,
									Alertas = resultado.Alertas
								});
						}
					}

					
					

				}
				



				return Json(new
				{
					CDStatus = "OK",
					DSMessage = "Cancelado com sucesso"

				});

			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		public JsonResult CartaCorrecao(NotaFiscal obj, string correcao)
		{
			try
			{
				if (obj.estabelecimento_id != _estab)
				{
					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}


				if (string.IsNullOrEmpty(correcao))
				{
					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Informe a correção",
					});

				}

				var db = new DbControle();

				var nf = new NotaFiscal().ObterPorId(obj.id, db);
				var service = new SoftFin.NFe.Business.CartaCorrecao();
				var serviceStatus = new SoftFin.NFe.Business.Status();


				DTOLogEvento dTOEvento = new DTOLogEvento();
				dTOEvento.cOrgao = "35";
				dTOEvento.cnpj = SoftFin.Utils.UtilSoftFin.Limpastrings(_estabobj.CNPJ);
				dTOEvento.chNFe = nf.NotaFiscalNFE.chaveAcesso;
				dTOEvento.dhEvento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("yyyy-MM-ddTHH:mm:ss") + "-03:00";
				dTOEvento.tpEvento = "110110 ";
				dTOEvento.nSeqEvento = "1";
				dTOEvento.verEvento = "1.00";
				dTOEvento.descEvento = "Carta de Correção";
				dTOEvento.nProt = nf.NotaFiscalNFE.protocoloAutorizacao;
				dTOEvento.xCorrecao = correcao;

				var caminhoXSD = Server.MapPath("~/XSDDOCS/NFe/CCe_NT2011.003/CCe_v1.00a/envCCe_v1.00.xsd");

				string urlServico = null;
				
				string urlServicoStatus = null;
				var url = db.UrlSefazUF.Where(p => p.UF == "SP");

				if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
				{
					urlServico = url.Where(p => p.codigo == "NFeEvento").First().UrlSefazPrincipal.url;
					urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;

				}
				else
				{
					urlServico = url.Where(p => p.codigo == "NFeEvento").First().UrlSefazPrincipal.urlHomologacao;
					urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;
				}


				string caminhoArquivo;
				System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
				ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);

				string caminhoArquivoSF;
				System.Security.Cryptography.X509Certificates.X509Certificate2 certSF;
				var estabSF = new Estabelecimento().ObterPorId(3, _paramBase); // SoftFin
				ObtemCertificadoX509(estabSF, out caminhoArquivoSF, out certSF);

				var resultado = serviceStatus.Execute("35", certSF, urlServicoStatus);
				resultado = service.Execute(dTOEvento, cert, caminhoXSD, urlServico);
				resultado.tipo = "NFeCartaCorrecao";
				new Conversao().ConverteRetornoGravaLog(resultado, nf.id, _usuario);


				if (!resultado.Sucesso)
				{

					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Comando não aceito, a seguir os erros:",
						Erros = resultado.Erros,
						Alertas = resultado.Alertas
					});
				}
				else
				{
					if (resultado.Alertas.Count() == 0)
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Comando aceito.",
							Erros = resultado.Erros,
							Alertas = resultado.Alertas
						});
					else
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Comando aceito, a seguir os alertas:",
							Erros = resultado.Erros,
							Alertas = resultado.Alertas
						});
				}




			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}


		[HttpGet]
		public JsonResult Status()
		{
			try
			{

				var db = new DbControle();
				var serviceStatus = new SoftFin.NFe.Business.Status();

				string urlServicoStatus = null;
				var url = db.UrlSefazUF.Where(p => p.UF == "SP");

				if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
				{
					urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;
				}
				else
				{
					urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;
				}

				string caminhoArquivoSF;
				System.Security.Cryptography.X509Certificates.X509Certificate2 certSF;
				var estabSF = new Estabelecimento().ObterPorId(3, _paramBase); // SoftFin
				ObtemCertificadoX509(estabSF, out caminhoArquivoSF, out certSF);

				var resultado = serviceStatus.Execute("35", certSF, urlServicoStatus);



				if (!resultado.Sucesso)
				{

					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Comando não aceito, a seguir os erros:",
						Erros = resultado.Erros,
						Alertas = resultado.Alertas
					},JsonRequestBehavior.AllowGet);
				}
				else
				{
					if (resultado.Alertas.Count() == 0)
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Serviço em Operação.",
							Erros = resultado.Erros,
							Alertas = resultado.Alertas
						}, JsonRequestBehavior.AllowGet);
					else
						return Json(new
						{
							CDStatus = "OK",
							DSMessage = "Serviço em Operação",
							Erros = resultado.Erros,
							Alertas = resultado.Alertas
						}, JsonRequestBehavior.AllowGet);
				}



			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		public JsonResult BaixaPerda(NotaFiscal obj)
		{
			try
			{
				if (obj.estabelecimento_id != _estab)
				{


					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}

				var db = new DbControle();
				var nfe = db.NotaFiscal.Where(nf => nf.id == obj.id && nf.estabelecimento_id == _estab).FirstOrDefault();

				nfe.situacaoPrefeitura_id = Models.NotaFiscal.NFBAIXA;
				nfe.SituacaoRecebimento = 4;

				//Exclui Banco Movimento quando baixada a nota fiscal
				var bcoMov = db.BancoMovimento.Where(nf => nf.notafiscal_id == nfe.id && nf.Banco.estabelecimento_id == _estab).FirstOrDefault();
				if (bcoMov != null)
				{
					db.BancoMovimento.Remove(bcoMov);
				}
				db.SaveChanges();

				return Json(new
				{
					CDStatus = "OK",
					DSMessage = "Sucesso na Baixa"

				});
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new
				{
					CDStatus = "NOK",
					DSMessage = ex.Message.ToString()
				});
			}
		}
		[HttpPost]
		public JsonResult ObterOperacoes()
		{
			var objs = new Operacao().ObterTodos(_paramBase).Where(p => p.produtoServico == "P");


			return Json(objs.Select(p => new
			{
				Value = p.id.ToString(),
				Text = p.descricao
			}), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult ObterCFOP()
		{
			var objs = new CFOP().ObterTodos();


			return Json(objs.Select(p => new
			{
				Value = p.codigo.ToString(),
				Text = p.descricao
			}), JsonRequestBehavior.AllowGet);
		}

	   
		public JsonResult ListaTabelas()
		{
			try
			{

				var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);

				return Json(new
				{
					CDStatus = "OK",
					DSMessage = "",
					objs = objs.Select(p => new
					{
						Value = p.id,
						Text = p.descricao
					})
				}, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		//Lista Produto Combo
		public JsonResult ListaProdutos(int idTabela)
		{
			try
			{

				var objs = new PrecoItemProdutoServico().ObterTodos(_paramBase).Where(p => p.TabelaPrecoItemProdutoServico_ID == idTabela && p.ItemProdutoServico.CategoriaItemProdutoServico.descricao  == "Produto").ToList();

				return Json(new
				{
					CDStatus = "OK",
					DSMessage = "",
					objs = objs.Select(p => new
					{
						Value = p.ItemProdutoServico.id,
						Text = p.ItemProdutoServico.descricao,
						p.ItemProdutoServico.precoVenda,
						p.preco,
						Price = ((p.ItemProdutoServico.precoVenda / 100) * p.preco) + p.ItemProdutoServico.precoVenda,
						Unit = p.ItemProdutoServico.unidadeMedida
					})
				}, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult ObterCodigoServicos()
		{
            var objs = new CodigoServicoEstabelecimento().ObterTodos(_paramBase).Where(p => p.CodigoServicoMunicipio.municipio_id == _estabobj.Municipio_id);


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
		public JsonResult ObterNFPorId(int id, int ovid, bool copiar)
		{
			try
			{
				var nf = new NotaFiscal();
				var ov = new OrdemVenda();

                if (ovid != 0)
                {
                    ov = new OrdemVenda().ObterPorId(ovid);
                }

				var notaFiscalNFEItem = new NotaFiscalNFEItem();
				var notaFiscalNFEItems = new List<NotaFiscalNFEItem>();
				var nfe = new NotaFiscalNFE();
				var tomador = new NotaFiscalPessoa();
				var transp = new NotaFiscalNFETransportadora();
				var retirada = new NotaFiscalNFERetirada();
				var entrega = new NotaFiscalNFEEntrega();
				var listaNFReferenciada = new List<NotaFiscalNFEReferenciada>();
				var listaDuplicatas = new List<NotaFiscalNFEDuplicata>();
				var listaVolumes = new List<NotaFiscalNFEVolume>();
				var listaReboques = new List<NotaFiscalNFEReboque>();
                var listaFormaPagamento = new List<NotaFiscalNFEFormaPagamento>();

                if (id != 0)
				{
					nf = nf.ObterPorId(id);
					nfe = nfe.ObterPorId(nf.NotaFiscalNFE_id.Value);
					ov = nf.OrdemVenda;
					tomador = nf.NotaFiscalPessoaTomador;
					notaFiscalNFEItems = notaFiscalNFEItem.ObterPorCapa(nfe.id);
					transp = nfe.NotaFiscalNFETransportadora.ObterPorId(nfe.NotaFiscalNFETransportadora_id.Value);
					retirada = nfe.NotaFiscalNFERetirada.ObterPorId(nfe.NotaFiscalNFERetirada_id.Value); ;
					entrega = nfe.NotaFiscalNFEEntrega.ObterPorId(nfe.NotaFiscalNFEEntrega_id.Value); ;
					listaNFReferenciada = new NotaFiscalNFEReferenciada().ObterTodos(nfe.id);
					listaDuplicatas = new NotaFiscalNFEDuplicata().ObterTodos(nfe.id);
					listaVolumes = new NotaFiscalNFEVolume().ObterPorNf(nfe.id);
					listaReboques = new NotaFiscalNFEReboque().ObterTodos(nfe.id);
                    listaFormaPagamento = new NotaFiscalNFEFormaPagamento().ObterTodos(nfe.id);


                    if (retirada != null)
					{
						if (!string.IsNullOrEmpty(retirada.codMunicipio))
						{
							var munic = new Municipio().ObterPorCodigoIBGE(retirada.codMunicipio);

							if (munic != null)
								retirada.cidade = munic.DESC_MUNICIPIO; 
						}
						if (retirada.cnpjCPF != null)
							retirada.indicadorCnpjCpf = (UtilSoftFin.Limpastrings(retirada.cnpjCPF).Length == 14) ? 2 : 1;
					}

					if (entrega != null)
					{
						if (!string.IsNullOrEmpty(entrega.codMunicipio))
						{
							var munic = new Municipio().ObterPorCodigoIBGE(entrega.codMunicipio);

							if (munic != null)
								entrega.cidade = munic.DESC_MUNICIPIO; 
						}
						if (entrega.cnpjCPF != null)
							entrega.indicadorCnpjCpf = (UtilSoftFin.Limpastrings(entrega.cnpjCPF).Length == 14) ? 2 : 1;
					}
					if (!string.IsNullOrEmpty(transp.cnpjCPF)) 
						transp.indicadorCnpjCpf = (UtilSoftFin.Limpastrings(transp.cnpjCPF).Length == 14)? 2: 1;

					if (copiar)
					{
						nf.id = 0;
						nf.NotaFiscalNFE_id = null;
						nf.notaFiscalPrestador_id = null;
						nf.notaFiscalTomador_id = null;
						ov.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
						ov.data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
						nf.dataEmissaoRps = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
						nf.dataEmissaoNfse = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
						nf.dataVencimentoNfse = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
						nf.numeroNfe = new NotaFiscal().ObterTodosUltimaNFe();
						nf.loteNfe = nf.numeroNfe;

						nfe.id = 0;
						nfe.protocoloAutorizacao = null;
						nfe.chaveAcesso = null;
						nfe.NotaFiscalNFEEntrega_id = null;
						nfe.NotaFiscalNFERetensao_id = null;
						nfe.NotaFiscalNFERetirada_id = null;
						nfe.NotaFiscalNFETransportadora_id = null;
						nfe.dataHoraSaida = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
						
						ov.id = 0;

						
						tomador.id  = 0;
						foreach (var item in notaFiscalNFEItems)
						{
							item.id = 0;
						}
						
						transp.id = 0; 
						retirada.id = 0; 
						entrega.id = 0;

						foreach (var item in listaNFReferenciada)
						{
							item.id = 0;
						}


						foreach (var item in listaDuplicatas)
						{
							item.id = 0;
						}

						foreach (var item in listaVolumes)
						{
							item.id = 0;
						}

						foreach (var item in listaReboques)
						{
							item.id = 0;
						}
                        foreach (var item in listaFormaPagamento)
                        {
                            item.id = 0;
                        }
                    }

				}
				else
				{
                    if (ov.id == 0)
                    {
                        ov.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        ov.data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        ov.descricao = "NFe Manual";
                        ov.estabelecimento_id = _estab;
                        ov.itemProdutoServico_ID = new ItemProdutoServico().ObterTodos(_paramBase).First().id;
                        ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
                        ov.tabelaPreco_ID = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase).First().id;
                        ov.usuarioinclusaoid = _usuarioobj.id;
                        ov.usuarioAutorizador_id = _usuarioobj.id;
                    }
                        nf.codigoServico = "NFe";
                        nf.discriminacaoServico = "NFe";
                        nf.aliquotaINSS = 0;
                        nf.valorINSS = 0;
                        nf.dataEmissaoRps = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        nf.dataEmissaoNfse = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        nf.dataVencimentoNfse = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        nf.entradaSaida = "S";
                        nf.estabelecimento_id = _estab;
                        nf.municipio_id = _estabobj.Municipio_id;
                        nf.numeroRps = 0;
                        nf.operacao_id = new Operacao().ObterTodos(_paramBase).First().id;
                        nf.situacaoPrefeitura_id = Models.NotaFiscal.NFGERADAENVIADA;
                        nf.situacaoRps = "1";
                        nf.tipoRps = 1;
                        nf.serieRps = "K";
                        nf.usuarioinclusaoid = _usuarioobj.id;
                        nfe.dataHoraSaida = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        nfe.situacao = 1;
                        nfe.CFOP = "5102";
                        nf.tipoNfe = "1";
                        nf.banco_id = new Banco().ObterPrincipal(_paramBase).id;
                        nf.serieNfe = 1;
                        nf.numeroNfe = new NotaFiscal().ObterTodosUltimaNFe();
                        nf.loteNfe = nf.numeroNfe;
                        transp.indicadorCnpjCpf = 2;
                    
				}

				return Json(new
				{
					CDStatus = "OK",
					ov = new
					{
						ov.id,
						ov.descricao,
						ov.valor,
						data = ov.data.ToString("o"),
						ov.parcelaContrato_ID,
						ov.statusParcela_ID,
						ov.unidadeNegocio_ID,
						ov.pessoas_ID,
						ov.estabelecimento_id,
						ov.usuarioAutorizador_id,
						dataAutorizacao = (ov.dataAutorizacao == null) ? null : ov.dataAutorizacao.Value.ToString("o"),
						ov.Numero,
						ov.itemProdutoServico_ID,
						ov.tabelaPreco_ID,
						ov.usuarioinclusaoid,
						ov.usuarioalteracaoid,
						dataInclusao = (nf.dataInclusao == null) ? null : nf.dataInclusao.Value.ToString("o"),
						dataAlteracao = (nf.dataAlteracao == null) ? null : nf.dataAlteracao.Value.ToString("o")


					},
					nf = new
					{
						nf.id,
						nf.situacaoPrefeitura_id,
						nf.estabelecimento_id,
						nf.ordemVenda_id,
						banco_id = nf.banco_id.ToString(),
						nf.operacao_id,
						nf.tipoRps,
						nf.serieRps,
						nf.numeroRps,
						dataEmissaoRps = nf.dataEmissaoRps.ToString("o"),
						nf.situacaoRps,
						nf.numeroNfse,
						dataEmissaoNfse = nf.dataEmissaoNfse.ToString("o"),
						dataVencimentoNfse = nf.dataVencimentoNfse.ToString("o"),
						nf.codigoVerificacao,
						nf.valorNfse,
						nf.valorDeducoes,
						nf.basedeCalculo,
						nf.aliquotaISS,
						nf.valorISS,
						nf.creditoImposto,
						nf.discriminacaoServico,
						nf.irrf,
						nf.pisRetido,
						nf.cofinsRetida,
						nf.csllRetida,
						nf.valorLiquido,
						nf.aliquotaIrrf,
						nf.SituacaoRecebimento,
						nf.entradaSaida,
						nf.municipio_id,
						nf.aliquotaINSS,
						nf.valorINSS,
						nf.codigoServico,
						nf.usuarioinclusaoid,
						nf.usuarioalteracaoid,
						dataInclusao = (nf.dataInclusao == null) ? null : nf.dataInclusao.Value.ToString("o"),
						dataAlteracao = (nf.dataAlteracao == null) ? null : nf.dataAlteracao.Value.ToString("o"),
						nf.notaFiscalTomador_id,
						nf.notaFiscalPrestador_id,
						nf.NotaFiscalNFE_id,
						nf.serieNfe,
						nf.numeroNfe,
						nf.loteNfe,
						nf.tipoNfe, 
					},
					nfe = new
					{
						nfe.id,
						dataHoraSaida = nfe.dataHoraSaida.ToString("o"),
						nfe.finalidadeEmissao,
						nfe.chaveAcesso,
						nfe.faturaFormaPgto,
						nfe.faturaNumero,
						nfe.faturaValorOriginal,
						nfe.faturaValorDesconto,
						nfe.faturaValorLiquido,
						nfe.informacaoComplementar,
						nfe.informacaoComplementarFisco,
						nfe.indicadorPresencaComprador,
						nfe.emailDestinatario,
						nfe.localEmbarqueExportacao,
						nfe.ufEmbarqueExportacao,
						nfe.identificacaoCompradorExtrangeiro,
						nfe.informacaoPedidoCompra,
						nfe.informacaoContato,
						nfe.informacaoNotaEmpenhoCompras,
						nfe.NotaFiscalNFEEntrega_id,
						nfe.NotaFiscalNFERetensao_id,
						nfe.NotaFiscalNFERetirada_id,
						nfe.NotaFiscalNFETransportadora_id,
						nfe.situacao,
						nfe.CFOP,
						nfe.valor,
						nfe.baseICMS,
						nfe.valorICMS,
						nfe.valorICMSDesonerado,
						nfe.baseICMSST,
						nfe.valorICMSST,
						nfe.valorProduto,
						nfe.valorFrete,
						nfe.valorSeguro,
						nfe.valorDesconto,
						nfe.valorII,
						nfe.valorIPI,
						nfe.valorPIS,
						nfe.valorCONFINS,
						nfe.valorOutro,
						nfe.valorCSLL, 

					},
					NotaFiscalNFEItem = new
					{
						notaFiscalNFEItem.id,
						notaFiscalNFEItem.produto,
						notaFiscalNFEItem.operacao,
						notaFiscalNFEItem.notaFiscal_id,
						notaFiscalNFEItem.idProduto,
						notaFiscalNFEItem.operacao_id,
						notaFiscalNFEItem.quantidade,
						notaFiscalNFEItem.item,
						notaFiscalNFEItem.valor,
						notaFiscalNFEItem.desconto,
						notaFiscalNFEItem.valorICMS,
						notaFiscalNFEItem.valorIPI,
						notaFiscalNFEItem.NCM,
						notaFiscalNFEItem.CFOP,
						notaFiscalNFEItem.CSOSN,
						notaFiscalNFEItem.valorICMSST,
						notaFiscalNFEItem.valorISS,
						notaFiscalNFEItem.valorIRRF,
						notaFiscalNFEItem.valorINSS,
						notaFiscalNFEItem.valorPIS,
						notaFiscalNFEItem.valorCOFINS,
						notaFiscalNFEItem.valorCSLL,
						notaFiscalNFEItem.aliquotaISS,
						notaFiscalNFEItem.aliquotaINSS,
						notaFiscalNFEItem.aliquotaCOFINS,
						notaFiscalNFEItem.PISRetido,
						notaFiscalNFEItem.COFINSRetida,
						notaFiscalNFEItem.CSLLRetida,
						notaFiscalNFEItem.ICMSSTRetida,
						notaFiscalNFEItem.ICMSRetida,
						notaFiscalNFEItem.nomeProduto,
						notaFiscalNFEItem.codigoProduto,
						notaFiscalNFEItem.unidadeMedida,
						notaFiscalNFEItem.EAN,
						notaFiscalNFEItem.valorUnitario,
						notaFiscalNFEItem.aliquotaIPI,
						notaFiscalNFEItem.TabelaPrecoItemProdutoServico_id,
						notaFiscalNFEItem.origem,
						notaFiscalNFEItem.CEST,
						notaFiscalNFEItem.infAdProd,
						notaFiscalNFEItem.aliquotaPIS,
						notaFiscalNFEItem.basePIS,
						notaFiscalNFEItem.baseCOFINS,
						notaFiscalNFEItem.PISCST,
						notaFiscalNFEItem.COFINSCST,
						notaFiscalNFEItem.valorTributos
					},
					NotaFiscalNFEItems = notaFiscalNFEItems.Select( p => new {
							p.id, 
							p.notaFiscal_id, 
							p.idProduto, 
							p.operacao,
							p.operacao_id, 
							p.quantidade, 
							p.item, 
							p.valor, 
							p.desconto, 
							p.valorICMS, 
							p.valorIPI, 
							p.NCM, 
							p.CFOP, 
							p.CSOSN, 
							p.valorICMSST, 
							p.valorISS, 
							p.valorIRRF, 
							p.valorINSS, 
							p.valorPIS, 
							p.valorCOFINS, 
							p.valorCSLL, 
							p.aliquotaISS, 
							p.aliquotaINSS, 
							p.PISRetido, 
							p.COFINSRetida, 
							p.CSLLRetida, 
							p.ICMSSTRetida, 
							p.ICMSRetida, 
							p.nomeProduto, 
							p.codigoProduto, 
							p.unidadeMedida, 
							p.EAN, 
							p.aliquotaIPI, 
							p.TabelaPrecoItemProdutoServico_id, 
							p.origem, 
							p.valorUnitario,
							p.pRedBC,
							p.CEST,
							p.infAdProd,
							p.valorTributos,
							p.PISCST,
							p.aliquotaPIS,
							p.basePIS,
							p.COFINSCST,
							p.aliquotaCOFINS,
							p.baseCOFINS
					}),
					retirada = new
					{
						retirada.id,
						retirada.notaFiscal_id,
						retirada.cnpjCPF,
						retirada.endereco,
						retirada.numero,
						retirada.complemento,
						retirada.codMunicipio,
						retirada.indicadorCnpjCpf,
						retirada.cidade

					},
					entrega = new
					{
						entrega.id,
						entrega.cnpjCPF,
						entrega.endereco,
						entrega.numero,
						entrega.complemento,
						entrega.codMunicipio,
						entrega.bairro,
						entrega.indicadorCnpjCpf,
						entrega.cidade

					},
					transp = new
					{
						transp.id,
						transp.nomeRazao,
						transp.cnpjCPF,
						transp.cidade,
						transp.uf,
						transp.placa,
						transp.ufplaca,
						transp.RNTC,
						transp.baseCalculo,
						transp.aliquota,
						transp.valorServico,
						transp.ICMSRetido,
						transp.CFOP,
						transp.modalidadeFrete,
						transp.IE,
						transp.EnderecoCompleto,
						transp.codigoMunicipioOcorrencia, 
						transp.indicadorCnpjCpf 
					   
					},
					NotaFiscalPessoaTomador = new
					{
						tomador.id,
						tomador.razao,
						tomador.indicadorCnpjCpf,
						tomador.cnpjCpf,
						tomador.inscricaoMunicipal,
						tomador.inscricaoEstadual,
						tomador.tipoEndereco,
						tomador.endereco,
						tomador.numero,
						tomador.complemento,
						tomador.bairro,
						tomador.cidade,
						tomador.uf,
						tomador.cep,
						tomador.email,
						tomador.notaFiscalOriginal,
						tomador.fone
					},
					listaNFReferenciada = listaNFReferenciada.Select(
						p => new
						{
							p.id,
							p.notaFiscal_id,
							p.NFe,
							p.CTe,
							p.nfserie,
							p.nfnumero,
							p.nfmodelo,
							p.nfuf,
							p.nfanoMesEmissao,
							p.nfcnpj,
							p.nfprodserie,
							p.nfprodnumero,
							p.nfprodmodelo,
							p.nfproduf,
							p.nfprodanoMesEmissao,
							p.nfprodcnpjCpf,
							p.nfprodIE,
							p.ECF,
							p.numeroCOO,
							p.modelo
						}
					),
					listaDuplicatas = listaDuplicatas.Select(
						p => new
						{
							p.id, 
							p.notaFiscal_id, 
							p.numero, 
							vencto = p.vencto.ToString("o"), 
							p.valor
						}
					),
					listaVolumes = listaVolumes.Select(
						p => new 
						{
							p.id, 
							p.notaFiscal_id, 
							p.qtde, 
							p.especie, 
							p.marca, 
							p.numeracao, 
							p.pesoLiquido, 
							p.pesoBruto, 
							p.lacres
						}
					),
					listaReboques = listaReboques.Select(
						p => new
						{
							p.id, 
							p.notaFiscal_id, 
							p.placa, 
							p.ufplaca, 
							p.RNTC
						}
					),
                    listaFormaPagamento = listaFormaPagamento.Select(
                        P => new
                        {
                            P.cAut,
                            P.CNPJ,
                            P.id,
                            P.indPag,
                            P.notaFiscal_id,
                            P.tBand,
                            P.tPag,
                            P.tpIntegra,
                            P.vPag,
                            P.vTroco
                        }
                    )


				}, JsonRequestBehavior.AllowGet);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}


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
			string pessoastr)
		{

			try
			{




				var nf = new NotaFiscal();

				var nomePessoa = pessoastr.Split(',')[0];
				var cnpjPessoa = pessoastr.Split(',')[1];
				var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa, _paramBase);

				if (pessoa == null)
					throw new Exception("Pessoa não encontrada ou não informada.");

				new NotaFiscalCalculos().Calcula(codigoServico,
												data,
												bancoid,
												operacaoid,
												valor,
												unidadeNegocioid,
												pessoa.id,
												ovid,
												_estabobj,
												nf, 
                                                _paramBase);

				var ov = new OrdemVenda();

				ov.data = data;
				ov.dataAutorizacao = null;
				ov.estabelecimento_id = _estab;
				ov.itemProdutoServico_ID = 0;
				ov.Numero = 0;
				ov.parcelaContrato_ID = null;
				ov.pessoas_ID = pessoa.id;
				ov.statusParcela_ID = 0;
				ov.tabelaPreco_ID = 0;
				ov.unidadeNegocio_ID = unidadeNegocioid;
				ov.usuarioAutorizador_id = null;
				ov.valor = valor;


				return Json(
					new
					{
						CDStatus = "OK",
						ov = new
						{
							data = ov.data.ToString("o"),
							dataAutorizacao = (ov.dataAutorizacao == null) ? "" : ov.dataAutorizacao.Value.ToString("o"),
							ov.estabelecimento_id,
							ov.id,
							ov.itemProdutoServico_ID,
							ov.Numero,
							ov.parcelaContrato_ID,
							ov.pessoas_ID,
							ov.statusParcela_ID,
							ov.tabelaPreco_ID,
							ov.unidadeNegocio_ID,
							ov.usuarioAutorizador_id,
							ov.valor
						},
						obj = new
						{
							nf.aliquotaINSS,
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
							dataEmissaoNfse = (nf.dataEmissaoNfse == null) ? "" : nf.dataEmissaoNfse.ToString("o"),
							dataEmissaoRps = (nf.dataEmissaoRps == null) ? "" : nf.dataEmissaoRps.ToString("o"),
							dataVencimentoNfse = (nf.dataVencimentoNfse == null) ? "" : nf.dataVencimentoNfse.ToString("o"),
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
							nf.loteNfe,
							nf.tipoNfe,
							NotaFiscalPessoaTomador = (nf.NotaFiscalPessoaTomador == null) ? null : new
							{
								nf.NotaFiscalPessoaTomador.razao,
								nf.NotaFiscalPessoaTomador.numero,
								nf.NotaFiscalPessoaTomador.bairro,
								nf.NotaFiscalPessoaTomador.cep,
								nf.NotaFiscalPessoaTomador.cidade,
								cnpjCpf = nf.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
								nf.NotaFiscalPessoaTomador.complemento,
								nf.NotaFiscalPessoaTomador.email,
								nf.NotaFiscalPessoaTomador.endereco,
								nf.NotaFiscalPessoaTomador.indicadorCnpjCpf,
								nf.NotaFiscalPessoaTomador.inscricaoEstadual,
								nf.NotaFiscalPessoaTomador.inscricaoMunicipal,
								nf.NotaFiscalPessoaTomador.tipoEndereco,
								nf.NotaFiscalPessoaTomador.uf,
								nf.NotaFiscalPessoaTomador.id
							},

							NotaFiscalPessoaPrestador = (nf.NotaFiscalPessoaPrestador == null) ? null : new
							{
								nf.NotaFiscalPessoaPrestador.razao,
								nf.NotaFiscalPessoaPrestador.numero,
								nf.NotaFiscalPessoaPrestador.bairro,
								nf.NotaFiscalPessoaPrestador.cep,
								nf.NotaFiscalPessoaPrestador.cidade,
								cnpjCpf = nf.NotaFiscalPessoaPrestador.cnpjCpf.Replace(".", "").Replace("/", "").Replace("-", ""),
								nf.NotaFiscalPessoaPrestador.complemento,
								nf.NotaFiscalPessoaPrestador.email,
								nf.NotaFiscalPessoaPrestador.endereco,
								nf.NotaFiscalPessoaPrestador.indicadorCnpjCpf,
								nf.NotaFiscalPessoaPrestador.inscricaoEstadual,
								nf.NotaFiscalPessoaPrestador.inscricaoMunicipal,
								nf.NotaFiscalPessoaPrestador.tipoEndereco,
								nf.NotaFiscalPessoaPrestador.uf,
								nf.NotaFiscalPessoaPrestador.id
							}
						}

					}, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}
		//Salva Nota
		[HttpPost]
		public JsonResult Salvar(NotaFiscal notafiscal, OrdemVenda ov)
		{
			try
			{
				var erroscpag = new List<string>();
				var titulo = new StringBuilder();
				var emailaviso = _estabobj.emailNotificacoes;
				var corpoemail = new StringBuilder();


				var erros = notafiscal.Validar(ModelState);

				//Validação Transp
				//Validação CNPJ e CPF

				if (notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF != null)
				{
					if (notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length == 14)
					{
						if (UtilSoftFin.IsCnpj(notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF) == false)
						{
							erros.Add("Cnpj de Transportadora é inválido");
						}
					}
					else if (notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length == 11)
					{
						if (UtilSoftFin.IsCpf(notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF) == false)
						{
							erros.Add("Cpf de Transportadora é inválido");
						}
					}
					else if (notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF.Length != 0)
					{
						erros.Add("Cpf ou Cnpj de Transportadora é inválido");
					}
				}
                if (ov.unidadeNegocio_ID == 0)
                {
                    erros.Add("O campo unidade de negócio é obrigatótio");
                }


                if (erros.Count() > 0)
				{
					return Json(new { CDMessage = "NOK", DSMessage = "Campos Inválidos", Erros = erros }, JsonRequestBehavior.AllowGet);
				}

				if (notafiscal.estabelecimento_id != _estab)
				{
					return Json(new { CDMessage = "NOK", DSMessage = "Estabelecimento inválido, saia do sistema e entre novamente (troca entre abas do navegador)", Erros = erros }, JsonRequestBehavior.AllowGet);
				}


				if (notafiscal.estabelecimento_id != _estab)
				{


					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}


				var plano = new PlanoDeConta().ObterTodos().Where(p => p.codigo.Equals("01")).FirstOrDefault();

				if (plano == null)
				{
					return Json(new { CDMessage = "NOK", DSMessage = "Plano de contas 01 não configurado", Erros = erros }, JsonRequestBehavior.AllowGet);
				}


                var nfejaexiste = new NotaFiscal().ObterTodosPorNumNFe(notafiscal.numeroNfe,notafiscal.serieNfe, _paramBase);

				if (nfejaexiste.Count() >0)
				{
					if (notafiscal.id != 0)
						nfejaexiste = nfejaexiste.Where(p => p.id != notafiscal.id).ToList();
					if (nfejaexiste.Count() > 0)
					{
						return Json(new { CDMessage = "NOK", DSMessage = "Número de Nota Fiscal já utilizada", Erros = erros }, JsonRequestBehavior.AllowGet);
					}
				}

				//var nomePessoa = ov.pessoanome.Split(',')[0];
				//var cnpjPessoa = ov.pessoanome.Split(',')[1];
				//var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa);






				DbControle db = new DbControle();

				if (notafiscal.id == 0)
				{
                    notafiscal.DataVencimentoOriginal = notafiscal.dataVencimentoNfse;

                    ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
                    Incluir(notafiscal, ov, plano, db);
				}
				else
				{
					

					using (var dbcxtransaction = db.Database.BeginTransaction())
					{
                        var ovaux = new OrdemVenda().ObterPorId(ov.id, db);
                        ovaux.statusParcela_ID = StatusParcela.SituacaoEmitida();
                        ovaux.Alterar(ovaux, _paramBase, db);


						var bm = new BancoMovimento().ObterPorNFES(notafiscal.id,db,_paramBase).FirstOrDefault();

						bm.dataAlteracao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        bm.UnidadeNegocio_id = ov.unidadeNegocio_ID;
                        bm.usuarioalteracaoid = _usuarioobj.id;
						bm.banco_id = notafiscal.banco_id.Value;
						bm.data = notafiscal.dataVencimentoNfse;
						bm.historico = ov.descricao;
						bm.valor = notafiscal.NotaFiscalNFE.valor;
						bm.Alterar(_paramBase, db);

						var orinf = new NotaFiscal().ObterPorId(notafiscal.id, db);

						orinf.banco_id = notafiscal.banco_id;
						orinf.operacao_id = notafiscal.operacao_id;
						orinf.numeroNfse = notafiscal.numeroNfse;
						orinf.dataEmissaoNfse = notafiscal.dataEmissaoNfse;
						orinf.dataVencimentoNfse = notafiscal.dataVencimentoNfse;
						orinf.codigoVerificacao = notafiscal.codigoVerificacao;
						orinf.valorNfse = notafiscal.valorNfse;
						orinf.valorDeducoes = notafiscal.valorDeducoes;
						orinf.basedeCalculo = notafiscal.basedeCalculo;
						orinf.aliquotaISS = notafiscal.aliquotaISS;
						orinf.valorISS = notafiscal.valorISS;
						orinf.creditoImposto = notafiscal.creditoImposto;
						orinf.discriminacaoServico = notafiscal.discriminacaoServico;
						orinf.irrf = notafiscal.irrf;
						orinf.pisRetido = notafiscal.pisRetido;
						orinf.cofinsRetida = notafiscal.cofinsRetida;
						orinf.csllRetida = notafiscal.csllRetida;
						orinf.valorLiquido = notafiscal.valorLiquido;
						orinf.aliquotaIrrf = notafiscal.aliquotaIrrf;
						orinf.SituacaoRecebimento = notafiscal.SituacaoRecebimento;
						orinf.entradaSaida = notafiscal.entradaSaida;
						orinf.municipio_id = notafiscal.municipio_id;
						orinf.aliquotaINSS = notafiscal.aliquotaINSS;
						orinf.valorINSS = notafiscal.valorINSS;
						orinf.codigoServico = notafiscal.codigoServico;
						orinf.serieNfe = notafiscal.serieNfe;
						orinf.numeroNfe = notafiscal.numeroNfe;
						orinf.loteNfe = notafiscal.loteNfe;
						orinf.tipoNfe = notafiscal.tipoNfe;
                        orinf.situacaoPrefeitura_id = Models.NotaFiscal.NFAVULSA;


                        orinf.NotaFiscalNFE.id = notafiscal.NotaFiscalNFE.id;
						orinf.NotaFiscalNFE.dataHoraSaida = notafiscal.NotaFiscalNFE.dataHoraSaida;
						orinf.NotaFiscalNFE.finalidadeEmissao = notafiscal.NotaFiscalNFE.finalidadeEmissao;
						orinf.NotaFiscalNFE.chaveAcesso = notafiscal.NotaFiscalNFE.chaveAcesso;
						orinf.NotaFiscalNFE.faturaFormaPgto = notafiscal.NotaFiscalNFE.faturaFormaPgto;
						orinf.NotaFiscalNFE.faturaNumero = notafiscal.NotaFiscalNFE.faturaNumero;
						orinf.NotaFiscalNFE.faturaValorOriginal = notafiscal.NotaFiscalNFE.faturaValorOriginal;
						orinf.NotaFiscalNFE.faturaValorDesconto = notafiscal.NotaFiscalNFE.faturaValorDesconto;
						orinf.NotaFiscalNFE.faturaValorLiquido = notafiscal.NotaFiscalNFE.faturaValorLiquido;
						orinf.NotaFiscalNFE.informacaoComplementar = notafiscal.NotaFiscalNFE.informacaoComplementar;
						orinf.NotaFiscalNFE.informacaoComplementarFisco = notafiscal.NotaFiscalNFE.informacaoComplementarFisco;
						orinf.NotaFiscalNFE.indicadorPresencaComprador = notafiscal.NotaFiscalNFE.indicadorPresencaComprador;
						orinf.NotaFiscalNFE.emailDestinatario = notafiscal.NotaFiscalNFE.emailDestinatario;
						orinf.NotaFiscalNFE.localEmbarqueExportacao = notafiscal.NotaFiscalNFE.localEmbarqueExportacao;
						orinf.NotaFiscalNFE.ufEmbarqueExportacao = notafiscal.NotaFiscalNFE.ufEmbarqueExportacao;
						orinf.NotaFiscalNFE.identificacaoCompradorExtrangeiro = notafiscal.NotaFiscalNFE.identificacaoCompradorExtrangeiro;
						orinf.NotaFiscalNFE.informacaoPedidoCompra = notafiscal.NotaFiscalNFE.informacaoPedidoCompra;
						orinf.NotaFiscalNFE.informacaoContato = notafiscal.NotaFiscalNFE.informacaoContato;
						orinf.NotaFiscalNFE.informacaoNotaEmpenhoCompras = notafiscal.NotaFiscalNFE.informacaoNotaEmpenhoCompras;
						orinf.NotaFiscalNFE.situacao = notafiscal.NotaFiscalNFE.situacao;
						orinf.NotaFiscalNFE.CFOP = notafiscal.NotaFiscalNFE.CFOP;
						orinf.NotaFiscalNFE.valor = notafiscal.NotaFiscalNFE.valor;
						orinf.NotaFiscalNFE.baseICMS = notafiscal.NotaFiscalNFE.baseICMS;
						orinf.NotaFiscalNFE.valorICMS = notafiscal.NotaFiscalNFE.valorICMS;
						orinf.NotaFiscalNFE.valorICMSDesonerado = notafiscal.NotaFiscalNFE.valorICMSDesonerado;
						orinf.NotaFiscalNFE.baseICMSST = notafiscal.NotaFiscalNFE.baseICMSST;
						orinf.NotaFiscalNFE.valorICMSST = notafiscal.NotaFiscalNFE.valorICMSST;
						orinf.NotaFiscalNFE.valorProduto = notafiscal.NotaFiscalNFE.valorProduto;
						orinf.NotaFiscalNFE.valorFrete = notafiscal.NotaFiscalNFE.valorFrete;
						orinf.NotaFiscalNFE.valorSeguro = notafiscal.NotaFiscalNFE.valorSeguro;
						orinf.NotaFiscalNFE.valorDesconto = notafiscal.NotaFiscalNFE.valorDesconto;
						orinf.NotaFiscalNFE.valorII = notafiscal.NotaFiscalNFE.valorII;
						orinf.NotaFiscalNFE.valorIPI = notafiscal.NotaFiscalNFE.valorIPI;
						orinf.NotaFiscalNFE.valorPIS = notafiscal.NotaFiscalNFE.valorPIS;
						orinf.NotaFiscalNFE.valorCONFINS = notafiscal.NotaFiscalNFE.valorCONFINS;
						orinf.NotaFiscalNFE.valorOutro = notafiscal.NotaFiscalNFE.valorOutro;
						orinf.NotaFiscalNFE.valorCSLL = notafiscal.NotaFiscalNFE.valorCSLL;
												
						orinf.NotaFiscalPessoaTomador.razao = notafiscal.NotaFiscalPessoaTomador.razao;
						orinf.NotaFiscalPessoaTomador.indicadorCnpjCpf = notafiscal.NotaFiscalPessoaTomador.indicadorCnpjCpf;
						orinf.NotaFiscalPessoaTomador.cnpjCpf = notafiscal.NotaFiscalPessoaTomador.cnpjCpf;
						orinf.NotaFiscalPessoaTomador.inscricaoMunicipal = notafiscal.NotaFiscalPessoaTomador.inscricaoMunicipal;
						orinf.NotaFiscalPessoaTomador.inscricaoEstadual = notafiscal.NotaFiscalPessoaTomador.inscricaoEstadual;
						orinf.NotaFiscalPessoaTomador.tipoEndereco = notafiscal.NotaFiscalPessoaTomador.tipoEndereco;
						orinf.NotaFiscalPessoaTomador.endereco = notafiscal.NotaFiscalPessoaTomador.endereco;
						orinf.NotaFiscalPessoaTomador.numero = notafiscal.NotaFiscalPessoaTomador.numero;
						orinf.NotaFiscalPessoaTomador.complemento = notafiscal.NotaFiscalPessoaTomador.complemento;
						orinf.NotaFiscalPessoaTomador.bairro = notafiscal.NotaFiscalPessoaTomador.bairro;
						orinf.NotaFiscalPessoaTomador.cidade = notafiscal.NotaFiscalPessoaTomador.cidade;
						orinf.NotaFiscalPessoaTomador.uf = notafiscal.NotaFiscalPessoaTomador.uf;
						orinf.NotaFiscalPessoaTomador.cep = notafiscal.NotaFiscalPessoaTomador.cep;
						orinf.NotaFiscalPessoaTomador.email = notafiscal.NotaFiscalPessoaTomador.email;
						orinf.NotaFiscalPessoaTomador.notaFiscalOriginal = notafiscal.NotaFiscalPessoaTomador.notaFiscalOriginal;
						orinf.NotaFiscalPessoaTomador.fone = notafiscal.NotaFiscalPessoaTomador.fone;
												
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.nomeRazao = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.nomeRazao;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cnpjCPF;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.cidade = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.cidade;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.uf = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.uf;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.placa = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.placa;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.ufplaca = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.ufplaca;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.RNTC = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.RNTC;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.baseCalculo = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.baseCalculo;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.aliquota = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.aliquota;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.valorServico = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.valorServico;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.ICMSRetido = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.ICMSRetido;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.CFOP = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.CFOP;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.modalidadeFrete = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.modalidadeFrete;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.IE = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.IE;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.EnderecoCompleto = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.EnderecoCompleto;
						orinf.NotaFiscalNFE.NotaFiscalNFETransportadora.codigoMunicipioOcorrencia = notafiscal.NotaFiscalNFE.NotaFiscalNFETransportadora.codigoMunicipioOcorrencia;
							
						orinf.NotaFiscalNFE.NotaFiscalNFERetirada.cnpjCPF = notafiscal.NotaFiscalNFE.NotaFiscalNFERetirada.cnpjCPF;
						orinf.NotaFiscalNFE.NotaFiscalNFERetirada.endereco = notafiscal.NotaFiscalNFE.NotaFiscalNFERetirada.endereco;
						orinf.NotaFiscalNFE.NotaFiscalNFERetirada.numero = notafiscal.NotaFiscalNFE.NotaFiscalNFERetirada.numero;
						orinf.NotaFiscalNFE.NotaFiscalNFERetirada.complemento = notafiscal.NotaFiscalNFE.NotaFiscalNFERetirada.complemento;
						orinf.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio = notafiscal.NotaFiscalNFE.NotaFiscalNFERetirada.codMunicipio;
						orinf.NotaFiscalNFE.NotaFiscalNFERetirada.bairro = notafiscal.NotaFiscalNFE.NotaFiscalNFERetirada.bairro;
						
						orinf.NotaFiscalNFE.NotaFiscalNFEEntrega.cnpjCPF = notafiscal.NotaFiscalNFE.NotaFiscalNFEEntrega.cnpjCPF;
						orinf.NotaFiscalNFE.NotaFiscalNFEEntrega.endereco = notafiscal.NotaFiscalNFE.NotaFiscalNFEEntrega.endereco;
						orinf.NotaFiscalNFE.NotaFiscalNFEEntrega.numero = notafiscal.NotaFiscalNFE.NotaFiscalNFEEntrega.numero;
						orinf.NotaFiscalNFE.NotaFiscalNFEEntrega.complemento = notafiscal.NotaFiscalNFE.NotaFiscalNFEEntrega.complemento;
						orinf.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio = notafiscal.NotaFiscalNFE.NotaFiscalNFEEntrega.codMunicipio;
						orinf.NotaFiscalNFE.NotaFiscalNFEEntrega.bairro = notafiscal.NotaFiscalNFE.NotaFiscalNFEEntrega.bairro;
						
						//Itens
						orinf.Alterar(_paramBase, db);

						var listaFiscalNFEItems = new NotaFiscalNFEItem().ObterPorCapa(orinf.NotaFiscalNFE.id, db);
						foreach (var item in listaFiscalNFEItems)
						{
							item.Excluir(_paramBase, db);
						}
						var listaNFReferenciada = new NotaFiscalNFEReferenciada().ObterTodos(orinf.NotaFiscalNFE.id, db);
						foreach (var item in listaNFReferenciada)
						{
							item.Excluir(_paramBase, db);
						}
						var listaDuplicatas = new NotaFiscalNFEDuplicata().ObterTodos(orinf.NotaFiscalNFE.id, db);
						foreach (var item in listaDuplicatas)
						{
							item.Excluir(_paramBase, db);
						}
						var listaVolumes = new NotaFiscalNFEVolume().ObterPorNf(orinf.NotaFiscalNFE.id, db);
						foreach (var item in listaVolumes)
						{
							item.Excluir(_paramBase, db);
						}
						var listaReboques = new NotaFiscalNFEReboque().ObterTodos(orinf.NotaFiscalNFE.id, db);
						foreach (var item in listaReboques)
						{
							item.Excluir(_paramBase, db);
						}

                        var listaPG = new NotaFiscalNFEFormaPagamento().ObterTodos(orinf.NotaFiscalNFE.id, db);
                        foreach (var item in listaPG)
                        {
                            item.Excluir(_paramBase, db);
                        }


                        //incluir itens
                        foreach (var item in notafiscal.NotaFiscalNFE.NotaFiscalNFEItems)
						{
							item.id = 0;
							item.notaFiscal_id = notafiscal.NotaFiscalNFE.id; 
							item.Incluir(db, _paramBase);
						}
						foreach (var item in notafiscal.NotaFiscalNFE.NotaFiscalNFEReferenciadas)
						{
							item.id = 0;
							item.notaFiscal_id = notafiscal.NotaFiscalNFE.id; 
							item.Incluir(db, _paramBase);
						}
						foreach (var item in notafiscal.NotaFiscalNFE.NotaFiscalNFEDuplicatas)
						{
							item.id = 0;
							item.notaFiscal_id = notafiscal.NotaFiscalNFE.id; 
							item.Incluir(db, _paramBase);
						}
						foreach (var item in notafiscal.NotaFiscalNFE.NotaFiscalNFEVolume)
						{
							item.id = 0;
							item.notaFiscal_id = notafiscal.NotaFiscalNFE.id; 
							item.Incluir(db, _paramBase);
						}
						foreach (var item in notafiscal.NotaFiscalNFE.NotaFiscalNFEReboques)
						{
							item.id = 0;
							item.notaFiscal_id = notafiscal.NotaFiscalNFE.id; 
							item.Incluir(db, _paramBase);
						}
                        foreach (var item in notafiscal.NotaFiscalNFE.NotaFiscalNFEFormaPagamentos)
                        {
                            item.id = 0;
                            item.notaFiscal_id = notafiscal.NotaFiscalNFE.id;
                            item.Incluir(db, _paramBase);
                        }
                        dbcxtransaction.Commit();
					}

				}

				//if (emailaviso != null)
				//{
				//    EnviaEmail(titulo.ToString(), corpoemail.ToString(), emailaviso, _estabobj);
				//}

				return Json(new { CDStatus = "OK", DSMessage = "Nota gerada com sucesso" }, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		private void Incluir(NotaFiscal notafiscal, OrdemVenda ov, PlanoDeConta plano, DbControle db)
		{
			notafiscal.NotaFiscalNFE.situacao = 1;
			var tipoManual = new OrigemMovimento().TipoManual(_paramBase);
			var tipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
			var tipoNotaPromissoria = new TipoDocumento().TipoNotaPromissoria();

			var bm = new BancoMovimento();

			bm.banco_id = notafiscal.banco_id.Value;
			bm.data = notafiscal.dataVencimentoNfse;
			bm.historico = ov.descricao;

			bm.valor = notafiscal.NotaFiscalNFE.valor;
			bm.origemmovimento_id = tipoManual;
			bm.tipoDeMovimento_id = tipoEntrada;
			bm.tipoDeDocumento_id = tipoNotaPromissoria;
			bm.planoDeConta_id = plano.id;
			bm.usuarioinclusaoid = _usuarioobj.id;
			bm.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();

			ov.usuarioinclusaoid = _usuarioobj.id;
			ov.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
			ov.statusParcela_ID = new StatusParcela().ObterTodos().Where(p => p.status == "Emitida").First().id;
			//ov.pessoas_ID = pessoa.id;


			notafiscal.situacaoPrefeitura_id = Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA;


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
                ccLC.data = notafiscal.dataVencimentoNfse;
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
				int numeroref = 0;
                

                if (ov.id == 0)
                {
                    ov.Incluir(ov, ref numeroref, _paramBase, db);
                }
                else
                {
                    var ovaux = new OrdemVenda().ObterPorId(ov.id, db);
                    ovaux.statusParcela_ID = StatusParcela.SituacaoEmitida();
                    ovaux.pessoas_ID = ov.pessoas_ID;
                    ovaux.Alterar(ovaux, _paramBase, db);
                }
                
				notafiscal.ordemVenda_id = ov.id;
				notafiscal.usuarioinclusaoid = _usuarioobj.id;
				notafiscal.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
				if (notafiscal.Incluir(_paramBase, db))
				{

					bm.notafiscal_id = notafiscal.id;

					if (!bm.Incluir(bm, _paramBase, db))
					{
						dbcxtransaction.Rollback();
						throw new Exception("Impossivel salvar, este registro esta cadastrado");
					}
                    //Inicio Lançamento Contabil
                    if (idCredito != 0 && idDebito != 0)
                    {
                        var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                        ccLC.codigoLancamento = numeroLcto;
                        ccLC.notafiscal_id = notafiscal.id;
                        ccLC.Incluir(_paramBase, db);
                        ccDebito.lancamentoContabil_id = ccLC.id;
                        ccCredito.lancamentoContabil_id = ccLC.id;
                        ccDebito.Incluir(_paramBase, db);
                        ccCredito.Incluir(_paramBase, db);
                    }
                    //Fim Lançamento Contabil
                    db.SaveChanges();
					dbcxtransaction.Commit();
						
				}
				else
				{
					dbcxtransaction.Rollback();
					throw new Exception("não foi possivel incluir sua nota");

				}

			}
		}


		public JsonResult Historico(NotaFiscal notafiscal)
		{
			try
			{

				if (notafiscal.estabelecimento_id != _estab)
				{


					return Json(new
					{
						CDStatus = "NOK",
						DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
					});

				}

				var objs = new LogNFXMLPrincipal().ObterTodosPorNota(notafiscal.id).OrderBy(p => p.dataInsert);

				return Json(new
				{
					CDStatus = "OK",
					objs = objs.Select(p => new
					{
						aceito = p.aceito ? "SIM" : "NÃO",
						dataInsert = p.dataInsert.ToString("o"),
						p.tipo,
						p.usuarioInsert,
						p.id,
						alertas = p.logNFXMLAlertas.Select(e => new { e.codigo, e.descricao }),
						erros = p.logNFXMLErros.Select(e => new { e.codigo, e.descricao })
					}
											)
				}, JsonRequestBehavior.AllowGet);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}




		public JsonResult ObtemPessoa(string pessoanome)
		{
			try 
			{
				var nomePessoa = pessoanome.Split(',')[0];
				var cnpjPessoa = pessoanome.Split(',')[1];
				var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa, _paramBase);

				var pessoaTomador = new NotaFiscalPessoa();

				pessoaTomador.razao = pessoa.razao;

				if (pessoa.CategoriaPessoa != null)
				{
					if (pessoa.CategoriaPessoa.Descricao == "Física")
						pessoaTomador.indicadorCnpjCpf = 1;
					else
						pessoaTomador.indicadorCnpjCpf = 2;
				}

				if (pessoa.cnpj != null)
					pessoaTomador.cnpjCpf = pessoa.cnpj.Replace(".", "").Replace("-", "").Replace("/", ""); 
				
				pessoaTomador.inscricaoMunicipal = pessoa.ccm; 
				pessoaTomador.inscricaoEstadual= pessoa.inscricao;
				if (pessoa.TipoEndereco != null)
				{
					pessoaTomador.tipoEndereco = pessoa.TipoEndereco.Descricao;
				}

				pessoaTomador.endereco= pessoa.endereco;
				pessoaTomador.numero= pessoa.numero;
				pessoaTomador.complemento= pessoa.complemento;
				pessoaTomador.bairro= pessoa.bairro;
				pessoaTomador.cidade= pessoa.cidade;
				pessoaTomador.uf= pessoa.uf;
				pessoaTomador.cep= pessoa.cep; 
				pessoaTomador.email= pessoa.eMail; 
				

				return Json(new
					{
						CDStatus = "OK",
						pessoaTomador.id,
						pessoaTomador.razao,
						pessoaTomador.indicadorCnpjCpf,
						pessoaTomador.cnpjCpf,
						pessoaTomador.inscricaoMunicipal,
						pessoaTomador.inscricaoEstadual,
						pessoaTomador.tipoEndereco,
						pessoaTomador.endereco,
						pessoaTomador.numero,
						pessoaTomador.complemento,
						pessoaTomador.bairro,
						pessoaTomador.cidade,
						pessoaTomador.uf,
						pessoaTomador.cep,
						pessoaTomador.email,
						pessoaTomador.notaFiscalOriginal
					}
				, JsonRequestBehavior.AllowGet);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}

		}

		public JsonResult ObterItem(NotaFiscalNFEItem notaFiscalNFEItem, List<NotaFiscalNFEItem> notaFiscalNFEItems)
		{
			try
			{
				if (notaFiscalNFEItem == null)
				{
					notaFiscalNFEItem = new NotaFiscalNFEItem();
					if (new Operacao().ObterTodos(_paramBase).Where(p => p.produtoServico == "P").Count() != 0)
						notaFiscalNFEItem.operacao_id = new Operacao().ObterTodos(_paramBase).Where(p => p.produtoServico == "P").ToList().FirstOrDefault().id;
					else
						notaFiscalNFEItem.operacao_id = 0;

					if (new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase).Count() != 0)
						notaFiscalNFEItem.TabelaPrecoItemProdutoServico_id = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase).FirstOrDefault().id;
					else
						notaFiscalNFEItem.TabelaPrecoItemProdutoServico_id = 0;

				}
				if (notaFiscalNFEItems == null)
				{
					notaFiscalNFEItems = new List<NotaFiscalNFEItem>();
				}

				return Json(new
				{
					CDStatus = "OK",
					NotaFiscalNFEItem = new
					{
						notaFiscalNFEItem.id,
						notaFiscalNFEItem.notaFiscal_id,
						notaFiscalNFEItem.idProduto,
						operacao_id = notaFiscalNFEItem.operacao_id.ToString(),
						notaFiscalNFEItem.quantidade,
						notaFiscalNFEItem.item,
						notaFiscalNFEItem.valor,
						notaFiscalNFEItem.desconto,
						notaFiscalNFEItem.valorICMS,
						notaFiscalNFEItem.valorIPI,
						notaFiscalNFEItem.NCM,
						notaFiscalNFEItem.CFOP,
						notaFiscalNFEItem.CSOSN,
						notaFiscalNFEItem.valorICMSST,
						notaFiscalNFEItem.valorISS,
						notaFiscalNFEItem.valorIRRF,
						notaFiscalNFEItem.valorINSS,
						notaFiscalNFEItem.valorPIS,
						notaFiscalNFEItem.valorCOFINS,
						notaFiscalNFEItem.valorCSLL,
						notaFiscalNFEItem.aliquotaISS,
						notaFiscalNFEItem.aliquotaINSS,
						notaFiscalNFEItem.PISRetido,
						notaFiscalNFEItem.COFINSRetida,
						notaFiscalNFEItem.CSLLRetida,
						notaFiscalNFEItem.ICMSSTRetida,
						notaFiscalNFEItem.ICMSRetida,
						notaFiscalNFEItem.nomeProduto,
						notaFiscalNFEItem.codigoProduto,
						notaFiscalNFEItem.unidadeMedida,
						notaFiscalNFEItem.EAN,
						valorUnidario = notaFiscalNFEItem.valorUnitario,
						notaFiscalNFEItem.aliquotaIPI,
						TabelaPrecoItemProdutoServico_id = notaFiscalNFEItem.TabelaPrecoItemProdutoServico_id.ToString()
					},
					NotaFiscalNFEItems = notaFiscalNFEItems
				}, JsonRequestBehavior.AllowGet);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult CalculaItem(NotaFiscalNFEItem notaFiscalNFEItem, List<NotaFiscalNFEItem> notaFiscalNFEItems, NotaFiscalNFE notaFiscalNFE)
		{
			try
			{
				if ((notaFiscalNFEItem != null))
				{

					if (notaFiscalNFEItems == null)
					{
						notaFiscalNFEItems = new List<NotaFiscalNFEItem>();
					}

					if (notaFiscalNFEItem.quantidade == 0)
						return Json(new { CDStatus = "NOK", DSMessage = "Quantidade não pode ser igual a zero." }, JsonRequestBehavior.AllowGet);

					var produto = new ItemProdutoServico().ObterPorId(notaFiscalNFEItem.idProduto, _paramBase);
					if (produto == null)
						return Json(new { CDStatus = "NOK", DSMessage = "Produto inválido" }, JsonRequestBehavior.AllowGet);

					var operacao = new Operacao().ObterPorId(notaFiscalNFEItem.operacao_id, _paramBase);
					if (operacao == null)
						return Json(new { CDStatus = "NOK", DSMessage = "Operação inválida" }, JsonRequestBehavior.AllowGet);



					decimal valorDeducoes = 0;
					var op = new Operacao().ObterPorId(notaFiscalNFEItem.operacao_id, _paramBase);


					//var aliquotas = new calculoImposto().ObterTodos(_estab).Where(p => p.operacao_id == notaFiscalNFEItem.operacao_id);

					//Calcula FATOR
					var somaOutrosItems = notaFiscalNFEItems.Sum(p => p.valor);
					var valorDesteItem = notaFiscalNFEItem.valor;
					var somaTodosItems = somaOutrosItems + valorDesteItem;
					var porcentagemDesteItem = (100 / valorDesteItem) * somaTodosItems;
					porcentagemDesteItem = Math.Round(porcentagemDesteItem, 2);

					
					//Calcula CONFINS
					decimal aliquota = 0;
					decimal valorbase = 0;
					string cstcalculo = "01";
					calculaImposto("COFINS",
							op.CalculoImposto, 
							notaFiscalNFEItem, 
							notaFiscalNFE,
							porcentagemDesteItem,
							ref valorDeducoes, 
							ref aliquota, 
							ref valorbase,
							ref cstcalculo);
					notaFiscalNFEItem.aliquotaCOFINS = aliquota; 
					notaFiscalNFEItem.baseCOFINS = valorbase;
					notaFiscalNFEItem.COFINSCST = cstcalculo;

					//Calcula PIS
					aliquota = 0;
					valorbase = 0;
					cstcalculo = "01";
					
					calculaImposto("PIS",
							op.CalculoImposto,
							notaFiscalNFEItem,
							notaFiscalNFE,
							porcentagemDesteItem,
							ref valorDeducoes,
							ref aliquota,
							ref valorbase,
							ref cstcalculo);
					notaFiscalNFEItem.aliquotaPIS = aliquota;
					notaFiscalNFEItem.basePIS = valorbase;
					notaFiscalNFEItem.PISCST = cstcalculo;



					notaFiscalNFEItem.produto = produto.descricao;
					notaFiscalNFEItem.operacao = operacao.descricao;
					notaFiscalNFEItem.CFOP = op.CFOP;
					notaFiscalNFEItem.CSOSN = op.CSOSN;
					notaFiscalNFEItem.origem = produto.origem;
					notaFiscalNFEItem.codigoProduto = produto.codigo;
					notaFiscalNFEItem.EAN = produto.codigoBarrasEAN;
					notaFiscalNFEItem.nomeProduto = produto.descricao;
					notaFiscalNFEItem.NCM = produto.ncm;
					notaFiscalNFEItem.unidadeMedida = produto.unidadeMedida;
					notaFiscalNFEItem.valorUnitario = produto.precoVenda.Value;
					notaFiscalNFEItem.CEST = produto.CEST;
					notaFiscalNFEItem.infAdProd = produto.informacoesComplementares;
					
					if ((produto.valorTributos == 0) || (notaFiscalNFEItem.valorUnitario == 0))
						notaFiscalNFEItem.valorTributos = 0;
					else
						notaFiscalNFEItem.valorTributos = (notaFiscalNFEItem.valorUnitario / 100) * (produto.valorTributos);
					
					notaFiscalNFEItems.Add(notaFiscalNFEItem);
				}


				notaFiscalNFE.valor = 0;
				notaFiscalNFE.baseICMS = 0;
				notaFiscalNFE.valorICMS = 0;
				notaFiscalNFE.valorICMSDesonerado = 0;
				notaFiscalNFE.baseICMSST = 0;
				notaFiscalNFE.valorICMSST = 0;
				notaFiscalNFE.valorProduto = 0;
				notaFiscalNFE.valorDesconto = 0;
				notaFiscalNFE.valorII = 0;
				notaFiscalNFE.valorIPI = 0;
				notaFiscalNFE.valorPIS = 0;
				notaFiscalNFE.valorCONFINS = 0;
				notaFiscalNFE.valorCSLL = 0;

				if (notaFiscalNFEItems == null)
				{
					notaFiscalNFEItems = new List<NotaFiscalNFEItem>();

				}

					foreach (var item in notaFiscalNFEItems)
					{
						notaFiscalNFE.valor += (item.valor - item.desconto) + notaFiscalNFE.valorFrete + notaFiscalNFE.valorSeguro + notaFiscalNFE.valorOutro;
						notaFiscalNFE.baseICMS += item.pRedBC;
						notaFiscalNFE.valorICMS += item.valorICMS;
						notaFiscalNFE.valorICMSDesonerado += 0;
						notaFiscalNFE.baseICMSST += 0;
						notaFiscalNFE.valorICMSST += 0;
						notaFiscalNFE.valorProduto += item.valor;
						notaFiscalNFE.valorDesconto = item.desconto;
						notaFiscalNFE.valorII = 0;
						notaFiscalNFE.valorIPI = 0;
						notaFiscalNFE.valorPIS = 0;
						notaFiscalNFE.valorCONFINS = 0;
						notaFiscalNFE.valorCSLL = 0;
					}
				



				return Json(new
				{
					CDStatus = "OK",
					NotaFiscalNFEItems = notaFiscalNFEItems.Select(p =>
					new {
						p.id,
						p.produto,
						p.operacao,
						p.notaFiscal_id,
						p.idProduto,
						p.operacao_id,
						p.quantidade,
						p.item,
						p.valor,
						p.desconto,
						p.valorICMS,
						p.valorIPI,
						p.NCM,
						p.CFOP,
						p.CSOSN,
						p.valorICMSST,
						p.valorISS,
						p.valorIRRF,
						p.valorINSS,
						p.valorPIS,
						p.valorCOFINS,
						p.valorCSLL,
						p.aliquotaISS,
						p.aliquotaINSS,
						p.aliquotaCOFINS,
						p.PISRetido,
						p.COFINSRetida,
						p.CSLLRetida,
						p.ICMSSTRetida,
						p.ICMSRetida,
						p.nomeProduto,
						p.codigoProduto,
						p.unidadeMedida,
						p.EAN,
						p.valorUnitario,
						p.aliquotaIPI,
						p.TabelaPrecoItemProdutoServico_id,
						p.origem,
						p.CEST,
						p.infAdProd,
						p.aliquotaPIS,
						p.basePIS,
						p.baseCOFINS,
						p.PISCST,
						p.COFINSCST,
						p.valorTributos
					}),
					NotaFiscalNFE = new
					{
						notaFiscalNFE.id,
						dataHoraSaida = notaFiscalNFE.dataHoraSaida.ToString("o"),
						notaFiscalNFE.finalidadeEmissao,
						notaFiscalNFE.chaveAcesso,
						notaFiscalNFE.faturaFormaPgto,
						notaFiscalNFE.faturaNumero,
						notaFiscalNFE.faturaValorOriginal,
						notaFiscalNFE.faturaValorDesconto,
						notaFiscalNFE.faturaValorLiquido,
						notaFiscalNFE.informacaoComplementar,
						notaFiscalNFE.informacaoComplementarFisco,
						notaFiscalNFE.indicadorPresencaComprador,
						notaFiscalNFE.emailDestinatario,
						notaFiscalNFE.localEmbarqueExportacao,
						notaFiscalNFE.ufEmbarqueExportacao,
						notaFiscalNFE.identificacaoCompradorExtrangeiro,
						notaFiscalNFE.informacaoPedidoCompra,
						notaFiscalNFE.informacaoContato,
						notaFiscalNFE.informacaoNotaEmpenhoCompras,
						notaFiscalNFE.NotaFiscalNFEEntrega_id,
						notaFiscalNFE.NotaFiscalNFERetensao_id,
						notaFiscalNFE.NotaFiscalNFERetirada_id,
						notaFiscalNFE.NotaFiscalNFETransportadora_id,
						notaFiscalNFE.situacao,
						notaFiscalNFE.CFOP,
						notaFiscalNFE.valor,
						notaFiscalNFE.baseICMS,
						notaFiscalNFE.valorICMS,
						notaFiscalNFE.valorICMSDesonerado,
						notaFiscalNFE.baseICMSST,
						notaFiscalNFE.valorICMSST,
						notaFiscalNFE.valorProduto,
						notaFiscalNFE.valorFrete,
						notaFiscalNFE.valorSeguro,
						notaFiscalNFE.valorDesconto,
						notaFiscalNFE.valorII,
						notaFiscalNFE.valorIPI,
						notaFiscalNFE.valorPIS,
						notaFiscalNFE.valorCONFINS,
						notaFiscalNFE.valorOutro,
						notaFiscalNFE.valorCSLL
						
					}
				}, JsonRequestBehavior.AllowGet);


			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		private static void calculaImposto(
			string imposto, 
			IEnumerable<calculoImposto> aliquotas, 
			NotaFiscalNFEItem nfitem, 
			NotaFiscalNFE nf, 
			decimal valorPercentualDesteitem,
			ref decimal valorDeducoes,  
			ref decimal aliquota, 
			ref decimal baseCalculo, 
			ref string cst)
		{
			var aliquotax = aliquotas.Where(p => p.imposto.codigo.Equals(imposto)).FirstOrDefault();

			if (aliquotax == null)
			{
				return;
			}

			//Calcula Base
			

			//TODO 
			foreach (var item in aliquotax.CalculoImpostoTipoImpostos)
			{
				if (item.TipoBase.descricao == "Valor da Mercadoria/Serviço")
				{
					baseCalculo += nfitem.valorUnitario;
				}
				if (item.TipoBase.descricao == "Frete")
				{
					baseCalculo += (nf.valorFrete / 100) * valorPercentualDesteitem;
				}
				if (item.TipoBase.descricao == "Seguro")
				{
					baseCalculo += (nf.valorSeguro / 100) * valorPercentualDesteitem;
				}
				if (item.TipoBase.descricao == "Outras Despesas")
				{
					baseCalculo += (nf.valorOutro / 100) * valorPercentualDesteitem;
				}
				if (item.TipoBase.descricao == "Desconto")
				{
					baseCalculo += nfitem.desconto;
				}
				if (item.TipoBase.descricao == "IPI")
				{
					baseCalculo += nfitem.valorIPI;
				}
				if (item.TipoBase.descricao == "ICMS")
				{
					baseCalculo += nfitem.valorICMS;
				}
				if (item.TipoBase.descricao == "COFINS")
				{
					baseCalculo += nfitem.valorCOFINS;
				}
			}
			
			//

			cst = aliquotax.CST;
			aliquotax.CST = aliquotax.CST;
			if (aliquotax != null)
			{
				aliquota = aliquotax.aliquota;
				baseCalculo = baseCalculo * aliquotax.aliquota / 100;

				if (aliquotax.retido)
				{
					valorDeducoes = valorDeducoes + baseCalculo;
				}
			}
		}
		#endregion
		#region Private
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


		public const string RPSEMITIDA_TEXTO = "1 - RPS Emitido";
		public const string NFSEGERADA_TEXTO = "2 - NFS-e Gerada";
		public const string NFSECANCELADAEMCONF_TEXTO = "3 - NFS-e cancelada sem confirmação";
		public const string NFSECANCELADACCONF_TEXTO = "4 - NFS-e cancelada com confirmação";
		public const string NFSEBAIXADA_TEXTO = "5 - NFS-e baixada como perda";

		private string ConsultaSituacao(int situacao)
		{
			switch (situacao)
			{
				case 1:
					return RPSEMITIDA_TEXTO;
				case 2:
					return NFSEGERADA_TEXTO;
				case 3:
					return NFSECANCELADAEMCONF_TEXTO;
				case 4:
					return NFSECANCELADACCONF_TEXTO;
				case 5:
					return NFSEBAIXADA_TEXTO;
				default:
					return "Indefinido";
			}
		}
		private DTORetornoNFe EnviaNFBussiness(int id, Boolean simulaXML = false)
		{
			var dTONfe = new DTONfe();
			var db = new DbControle();
			var objNF = new NotaFiscal().ObterPorId(id, db);
			var estabSF = new Estabelecimento().ObterPorId(3, _paramBase); // SoftFin
			
			var resultado = new DTORetornoNFe();

			AtualizaNFServiceValidar(_estabobj, objNF);

			var listaNF = new List<NotaFiscal>();
			listaNF.Add(objNF);

			string caminhoArquivo;
			System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
			ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);

			string caminhoArquivoSF;
			System.Security.Cryptography.X509Certificates.X509Certificate2 certSF;
			ObtemCertificadoX509(estabSF, out caminhoArquivoSF, out certSF);


			new Conversao().ConverterNFe(dTONfe, _estabobj, listaNF);

			var service = new SoftFin.NFe.Business.EnvioNFe();
			var serviceStatus = new SoftFin.NFe.Business.Status();
			
			//var caminhoXSD = @"C:\Users\Ricardo\OneDrive\Projeto2\XSDDOCS\NFe\3_10\enviNFe_v3.10.xsd";
			var caminhoXSD = Server.MapPath("~/XSDDOCS/NFe/4_00/enviNFe_v4.00.xsd");
			
			string urlServico = null;
			string urlServicoRet = null;
			string urlServicoStatus = null;

			var url = db.UrlSefazUF.Where(p => p.UF == _estabobj.UF);

			if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
			{
				urlServico = url.Where(p => p.codigo == "NFeAutorizacao").First().UrlSefazPrincipal.url;
				urlServicoRet = url.Where(p => p.codigo == "NFeRetAutorizacao").First().UrlSefazPrincipal.url;
				urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.url;
			}
			else
			{
				urlServico = url.Where(p => p.codigo == "NFeAutorizacao").First().UrlSefazPrincipal.urlHomologacao;
				urlServicoRet = url.Where(p => p.codigo == "NFeRetAutorizacao").First().UrlSefazPrincipal.urlHomologacao;
				urlServicoStatus = url.Where(p => p.codigo == "NfeStatusServico").First().UrlSefazPrincipal.urlHomologacao;
			}

			resultado = serviceStatus.Execute("35", certSF, urlServicoStatus);
			Thread.Sleep(2000);
			resultado = service.Execute(dTONfe, cert, simulaXML, caminhoXSD, urlServico,urlServicoRet);

			if (!simulaXML)
			{
				new Conversao().ConverteRetornoGravaLog(resultado, objNF.id, _usuario);
				if (resultado.Sucesso)
				{
					objNF.NotaFiscalNFE.situacao = Models.NotaFiscal.NFGERADAENVIADA;
					objNF.NotaFiscalNFE.chaveAcesso = resultado.chaveAcesso;
					objNF.NotaFiscalNFE.protocoloAutorizacao = resultado.protocoloAutorizacao;
					objNF.Alterar(_paramBase, db);
				}
			}
			try
			{
				cert = null;
				System.IO.File.Delete(caminhoArquivo);
				certSF = null;
				System.IO.File.Delete(caminhoArquivoSF);
			}
			catch
			{
			}

			return resultado;
		}

		//private void AtualizaNFBussines(int id, out DbControle db, out NotaFiscal objNF, out SoftFin.NFSe.DTO.DTORetornoNFEs resultado)
		//{
		//    //var service = new SoftFin.NFSe.SaoPaulo.Bussiness.ConsultaNFe();
		//    var obj = new DTORetornoNFEs();

		//    db = new DbControle();
		//    var objEstab = Acesso.pb.estab_idObj();
		//    objNF = new NotaFiscal().ObterPorId(id, db);


		//    //AtualizaNFServiceValidar(objEstab, objNF);

		//    var dTONF = new DTONotaFiscal();
		//    var listaNFs = new List<NotaFiscal>();
		//    listaNFs.Add(objNF);
		//    new Conversao().ConverterNFEs(dTONF, _estabobj, listaNFs);
		//    var arquivoxml = "";


		//    //obj.NFe.First().CnpjPrestador.CNPJ = UtilSoftFin.ReplaceCNPJCPF(objEstab.CNPJ);
		//    ////obj.Detalhe.ChaveNFe.NumeroNFe = "1";
		//    //obj.Detalhe.ChaveNFe.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();
		//    //obj.Detalhe.ChaveRPS.NumeroRPS = objNF.numeroRps.ToString();
		//    //obj.Detalhe.ChaveRPS.SerieRPS = objNF.serieRps;
		//    //obj.Detalhe.ChaveRPS.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();// UtilSoftFin.ReplaceCNPJCPF(objEstab.CNPJ);


		//    var uploadPath = Server.MapPath("~/CertTMP/");
		//    Directory.CreateDirectory(uploadPath);
		//    var nomearquivonovo = Guid.NewGuid().ToString();
		//    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
		//    var cert = UtilSoftFin.BuscaCert(_estab, objEstab.senhaCertificado, caminhoArquivo, objEstab.CNPJ);

		//    //resultado = service.Execute(objNF, cert);

		//    var regra = new SoftFin.NFSe.Business.SFConsultaNFSe();

		//    var result = regra.Execute(dTONF, cert);

		//    resultado = result;

		//    try
		//    {
		//        cert = null;
		//        System.IO.File.Delete(caminhoArquivo);
		//    }
		//    catch
		//    {
		//    }
		//}
		private static void AtualizaNFServiceValidar(Estabelecimento objEstab, NotaFiscal objNF)
		{
			if (objNF == null)
				throw new Exception("Nota não encotrada.");

			if (objNF.estabelecimento_id != objEstab.id)
				throw new Exception("Estabelecimento não compativel.");


			if (objNF.serieRps == null)
				throw new Exception("RPS não emitido.");
		}

		private List<NotaFiscal> PesquisaNFse()
		{
			var lista = new NotaFiscal().ObterTodosNFe(_paramBase);
			return lista.ToList();
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
			corpoemail.AppendLine(pc.ContratoItem.Contrato.Pessoa.nome);
			corpoemail.AppendLine("    </td>");
			corpoemail.AppendLine("    <td style='text-align:right'>");
			corpoemail.AppendLine(pc.valor.ToString("n"));
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


		#region NFeInformacao
		public JsonResult ObterTodosNFeInformacaoDropDown()
		{
			var objs = new NFeInformacao().ObterTodos(_paramBase);


			return Json(objs.Select(p => new
			{
				Text = p.descricao,
				Value = p.id
			}), JsonRequestBehavior.AllowGet);
		}
		public JsonResult ObterTodosNFeInformacao()
		{
			var objs = new NFeInformacao().ObterTodos(_paramBase);


			return Json(objs.Select(p => new
			{
				p.descricao,
				p.estabelecimento_id,
				p.id,
				p.informacaoComplementar,
				p.informacaoComplementarFisco
			}), JsonRequestBehavior.AllowGet);
		}

		public JsonResult ObterPorIdNFeInformacao(int id)
		{
			var obj = new NFeInformacao().ObterPorId(id, _paramBase);
			if (obj == null)
			{
				obj = new NFeInformacao();
				obj.estabelecimento_id = _estab;
				obj.descricao = "Nova informação para nota fiscal " + SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToShortDateString();
			}
			return Json(new
			{
				obj.descricao,
				obj.estabelecimento_id,
				obj.id,
				obj.informacaoComplementar,
				obj.informacaoComplementarFisco
			}, JsonRequestBehavior.AllowGet);
		}

		public JsonResult SalvarNFeInformacao(NFeInformacao obj)
		{
			try
			{
				var objErros = obj.Validar(ModelState);

				if (obj.estabelecimento_id != _estab)
					return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

				if (obj.descricao == null)
					return Json(new { CDMessage = "NOK", DSMessage = "Informe a descrição" }, JsonRequestBehavior.AllowGet);

				if (obj.informacaoComplementar == null)
					return Json(new { CDMessage = "NOK", DSMessage = "Informe a descrição complementar" }, JsonRequestBehavior.AllowGet);





				var tp = new NFeInformacao();
				

				if (obj.id == 0)
				{

					if (tp.Incluir(obj, _paramBase) == true)
					{
						return Json(new { CDStatus = "OK", DSMessage = "Salvo com sucesso" }, JsonRequestBehavior.AllowGet);
					}
					else
					{
						return Json(new { CDStatus = "NOK", DSMessage = "Erro ao salvar" }, JsonRequestBehavior.AllowGet);
					}
				}
				else
				{
					tp.Alterar(obj, _paramBase);
					return Json(new { CDStatus = "OK", DSMessage = "Salvo com sucesso" }, JsonRequestBehavior.AllowGet);
				}

			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}


		public JsonResult ExcluirNFeInformacao(NFeInformacao obj)
		{

			try
			{

				
				if (obj.Excluir(obj.id, _paramBase))
				{
					return Json(new { CDStatus = "OK", DSMessage = "Excluida com sucesso" }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir " }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}

		}
		#endregion

	}
}
