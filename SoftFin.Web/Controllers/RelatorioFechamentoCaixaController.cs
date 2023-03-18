using iTextSharp.text;
using iTextSharp.text.pdf;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioFechamentoCaixaController : BaseController
    {
        //
        // GET: /RelatorioFechamentoCaixa/

        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public FileResult Report( DateTime dataIni, DateTime dataFim, int idLoja, int? idOperador, int? idCaixa)
        {
            try
            {

                var objs = new LojaFechamento().ObterTodosByidLojaDataInicialFinal(idLoja, SoftFin.Utils.UtilSoftFin.TiraHora(dataIni), SoftFin.Utils.UtilSoftFin.TiraHora(dataFim), _paramBase);


                if (idOperador != null)
                {
                    objs = objs.Where(p => p.LojaOperador_id == idOperador.Value).ToList();
                }

                if (idCaixa != null)
                {
                    objs = objs.Where(p => p.LojaCaixa_id == idCaixa.Value).ToList();
                }


                objs.OrderBy(p => p.dataFechamento).ThenBy(n => n.sequencia).ToList();

                var uploadPath = Server.MapPath("~/TXTTemp/");
                Directory.CreateDirectory(uploadPath);
                var uploadPathTemplate = Server.MapPath("~/Template/PDF/");

                var filenameTemplate = "RelatorioFechamentoCaixa.pdf";
                var filename = "RelatorioFechamentoCaixa_" + _estabobj.Codigo + "_" + SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("yyyyMMddhhmmss") + ".PDF";
                var filenamePNG = "RelatorioFechamentoCaixa_" + _estabobj.Codigo + "_" + SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("yyyyMMddhhmmss") + ".PNG";

                string filepath = uploadPath + "\\" + filename;
                string filepathPNG = uploadPath + "\\" + filenamePNG;
                string filepathTemplate = uploadPathTemplate + "\\" + filenameTemplate;

                using (var template = new FileStream(filepathTemplate, FileMode.Open))
                using (var newpdf = new FileStream(filepath, FileMode.Create))
                {
                    var pdfReader = new PdfReader(template);
                    Document document = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, newpdf);
                    document.AddTitle("Fechamento de Caixa");
                    document.Open();
                    PdfContentByte cb = writer.DirectContent;
                    PdfImportedPage page = writer.GetImportedPage(pdfReader, 1);



                    float altura = 0;
                    float largura = 70;
                    float inicio = 25;
                    float fim = 210;



                    var fnt = new Font(Font.FontFamily.TIMES_ROMAN, 8);
                    fnt.Color = BaseColor.BLACK;

                    if (objs.Count == 0)
                    {
                        if (altura < 50)
                        {
                            altura = 750;

                            document.NewPage();
                            cb.AddTemplate(page, 0, 0);




                            if (!string.IsNullOrEmpty(_estabobj.Logo))
                            {
                                var arquivo = _estabobj.Logo;
                                var request = WebRequest.Create(arquivo);
                                using (var response = request.GetResponse())
                                {
                                    using (var responseStream = response.GetResponseStream())
                                    {
                                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(arquivo);
                                        logo.ScalePercent(50f);
                                        logo.SetAbsolutePosition(20, 780);
                                        document.Add(logo);
                                    }
                                }
                            }
                        }




                        inicio = 25;
                        fim = 700;
                        GravaTexto("Pesquisa não encontrou resultados", cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);


                        altura = 20;
                        inicio = 25;
                        fim = 400;
                        GravaTexto("Página 1"  , cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 25;
                        fim = 560;
                        GravaTexto(DateTime.Now.ToString(), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_RIGHT);
                    }

                    var numpagina = 0;

                    foreach (var item in objs)
                    {

                        if (altura < 50)
                        {
                            

                            document.NewPage();
                            cb.AddTemplate(page, 0, 0);


                            if (!string.IsNullOrEmpty(_estabobj.Logo))
                            {
                                var arquivo = _estabobj.Logo;
                                var request = WebRequest.Create(arquivo);
                                using (var response = request.GetResponse())
                                {
                                    using (var responseStream = response.GetResponseStream())
                                    {
                                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(arquivo);
                                        logo.ScalePercent(50f);
                                        logo.SetAbsolutePosition(20, 780);
                                        document.Add(logo);
                                    }
                                }
                            }

                            altura = 20;
                            inicio = 25;
                            fim = 400;
                            numpagina += 1;
                            GravaTexto("Página " + numpagina.ToString(), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);
                            
                            inicio = 25;
                            fim = 560;
                            GravaTexto(DateTime.Now.ToString(), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_RIGHT);

                            altura = 750;
                        }




                        inicio = 25;
                        fim = 700;
                        GravaTexto(item.dataFechamento.ToShortDateString() + " - " + item.sequencia.ToString() , cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 80;
                        fim = 700;
                        GravaTexto(item.Loja.codigo, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 130;
                        fim = 700;
                        GravaTexto(item.LojaCaixa.codigo, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 180;
                        fim = 700;
                        GravaTexto(item.LojaOperador.descricao, cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 340;
                        fim = 700;
                        GravaTexto(item.saldoInicial.ToString("n"), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 380;
                        fim = 700;
                        GravaTexto(item.valorBruto.ToString("n"), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 420;
                        fim = 700;
                        GravaTexto(item.valorTaxas.ToString("n"), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);

                        inicio = 460;
                        fim = 700;
                        GravaTexto(item.valorLiquido.ToString("n"), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);


                        inicio = 500;
                        fim = 700;
                        GravaTexto(item.saldoFinal.ToString("n"), cb, fnt, fim, altura, inicio, largura, Element.ALIGN_LEFT);


                        inicio = 75;
                        fim = 250;
                        altura -= 25;
                    }


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


        private static void GravaTexto(string value, PdfContentByte cb, Font fnt, float x, float y, float x2 = 0, float y2 = 10, int alinhamento = Element.ALIGN_RIGHT, int tipo = 0)
        {
            ColumnText ct;
            Paragraph par;

            ct = new ColumnText(cb);
            ct.SetSimpleColumn(x, y, x2, y2);
            if (value == null)
            {
                value = "-";
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
                    value = value.Replace(".", ",");
                }
                else if (tipo == 9) // numero
                {
                    value = value.Substring(0, 4)
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

        public JsonResult ObterLoja()
        {
            var objs = new Loja().ObterTodos(_paramBase).Where(p => p.ativo == true);

            var caixa = objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }
            );
            return Json(caixa, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterOperador(int id)
        {
            var objs = new LojaOperador().ObterTodosPorLoja(id, _paramBase).Where(p => p.ativo == true);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterCaixa(int id)
        {
            var objs = new LojaCaixa().ObterTodosPorLoja(id, _paramBase).Where(p => p.ativo == true);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
