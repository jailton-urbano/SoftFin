
using iTextSharp.text;
using iTextSharp.text.pdf;
using SoftFin.Web.Classes;
using SoftFin.Web.Controllers;
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
    public class NDController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var APagar = new DocumentoPagarParcela().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + APagar }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Excel()
        {
            var obj = new NotadeDebito();
            var lista = obj.ObterTodos(_paramBase);
            SoftFin.Utils.CsvExport myExport = new SoftFin.Utils.CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["estabelecimento_id"] = item.estabelecimento_id;
                myExport["Estabelecimento"] = item.Estabelecimento.NomeCompleto;
                myExport["numero"] = item.numero;
                myExport["DataEmissao"] = item.DataEmissao.ToString();
                myExport["DataVencimento"] = item.DataVencimento.ToString();
                myExport["cliente_id"] = item.cliente_id;
                myExport["Cliente"] = item.Cliente.nome;
                myExport["descricao"] = item.descricao;
                myExport["valor"] = item.valor.ToString("0.00");
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "NotadeDebito.csv");
        }

        public FileStreamResult PDF(int ID)
        {
            var nd = new NotadeDebito().ObterPorId(ID, _paramBase);
            int estab = _estab;
            MemoryStream stream = new MemoryStream();

            Document doc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
            pdfWriter.CloseStream = false;

            doc.Open();

            //Carrega Logo
            try
            {
                var request = WebRequest.Create(_estabobj.Logo);
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("https://sistemahom.blob.core.windows.net/compartilhado/Logo/Logo_" + estab + ".jpg");
                        logo.ScalePercent(50f);
                        //logo.SetAbsolutePosition(70f, 10f);
                        doc.Add(logo);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                    ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Do something
                    }
                    else
                    {
                        // Do something else
                    }
                }
                else
                {
                    // Do something else
                }
            }


            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Título
            Font verdanaTitle = FontFactory.GetFont("Verdana", 24, Font.BOLD, BaseColor.DARK_GRAY);
            Chunk title = new Chunk("Nota de Débito");
            title.SetUnderline(1f, -2f);
            title.Font = verdanaTitle;
            Phrase f1 = new Phrase();
            f1.Add(title);
            Paragraph p1 = new Paragraph();
            p1.Add(f1);
            p1.Alignment = 1;
            doc.Add(p1);

            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Tabela dados do Estabelecimento e dados da ND
            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.TotalWidth = 500f;
            tableHeader.LockedWidth = true;
            float[] widths = new float[] { 300f, 200f };
            tableHeader.SetWidths(widths);

            //Dados do Estabelecimento
            Chunk nome = new Chunk(nd.Estabelecimento.NomeCompleto);
            Chunk cnpj = new Chunk("CNPJ: " + nd.Estabelecimento.CNPJ);
            Chunk endereco = new Chunk("Endereço: " + nd.Estabelecimento.Logradouro + ", "
                                               + nd.Estabelecimento.NumeroLogradouro.ToString()
                );
            Chunk municipio = new Chunk(nd.Estabelecimento.Municipio.DESC_MUNICIPIO + " - " + nd.Estabelecimento.UF);
            Chunk bairro = new Chunk(nd.Estabelecimento.BAIRRO + " - " + nd.Estabelecimento.CEP);

            //Dados da Nota de Débito
            Font verdanaND = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.BLACK);
            Chunk numero = new Chunk("Número da Fatura: " + nd.numero);
            numero.SetUnderline(1f, -2f);
            numero.Font = verdanaND;
            Chunk emissao = new Chunk("Emissão: " + nd.DataEmissao.ToString("dd/MMM/yyyy"));
            Chunk vencimento = new Chunk("Vencimento: " + nd.DataVencimento.ToString("dd/MMM/yyyy"));

            //Célula do Estabelecimento #1
            PdfPCell c1 = new PdfPCell(new Phrase(nome));
            c1.HorizontalAlignment = 0;
            c1.Border = 0;
            tableHeader.AddCell(c1);

            //Célula dados da ND #1
            PdfPCell c2 = new PdfPCell(new Phrase(numero));
            c2.HorizontalAlignment = 2;
            c2.Border = 0;
            tableHeader.AddCell(c2);

            //Célula do Estabelecimento #2
            PdfPCell c3 = new PdfPCell(new Phrase(cnpj));
            c3.HorizontalAlignment = 0;
            c3.Border = 0;
            tableHeader.AddCell(c3);

            //Célula dados da ND #2
            PdfPCell c4 = new PdfPCell(new Phrase(emissao));
            c4.HorizontalAlignment = 2;
            c4.Border = 0;
            tableHeader.AddCell(c4);

            //Célula do Estabelecimento #3
            PdfPCell c5 = new PdfPCell(new Phrase(endereco));
            c5.HorizontalAlignment = 0;
            c5.Border = 0;
            tableHeader.AddCell(c5);

            //Célula dados da ND #3
            PdfPCell c6 = new PdfPCell(new Phrase(vencimento));
            c6.HorizontalAlignment = 2;
            c6.Border = 0;
            tableHeader.AddCell(c6);

            //Célula do Estabelecimento #4
            PdfPCell c7 = new PdfPCell(new Phrase(municipio));
            c7.HorizontalAlignment = 0;
            c7.Border = 0;
            tableHeader.AddCell(c7);

            //Célula dados da ND #4 - Data de Cancelamento
            if (nd.DataCancelamento != null)
            {
                Chunk cancelamento = new Chunk("Nota Cancelada em: " + nd.DataCancelamento);
                Font FontCancel = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.RED);
                cancelamento.Font = FontCancel;
                PdfPCell c8 = new PdfPCell(new Phrase(cancelamento));
                c8.HorizontalAlignment = 2;
                c8.Border = 0;
                tableHeader.AddCell(c8);
            }
            else
            {
                PdfPCell c8 = new PdfPCell(new Phrase(""));
                c8.HorizontalAlignment = 2;
                c8.Border = 0;
                tableHeader.AddCell(c8);
            }

            //Célula do Estabelecimento #5
            PdfPCell c9 = new PdfPCell(new Phrase(bairro));
            c9.HorizontalAlignment = 0;
            c9.Border = 0;
            tableHeader.AddCell(c9);

            //Célula dados da ND #5
            PdfPCell c10 = new PdfPCell(new Phrase(""));
            c10.HorizontalAlignment = 2;
            c10.Border = 0;
            tableHeader.AddCell(c10);

            doc.Add(tableHeader);

            //Pula linhas
            doc.Add(Chunk.NEWLINE);

            //Tabela Dados do Cliente
            PdfPTable tableCliente = new PdfPTable(1);
            tableCliente.TotalWidth = 500f;
            tableCliente.LockedWidth = true;
            float[] widths3 = new float[] { 500f };
            tableHeader.SetWidths(widths);

            Chunk cliente = new Chunk("Cliente: " + nd.Cliente.nome);
            cliente.SetUnderline(1f, -2f);
            PdfPCell cliente1 = new PdfPCell(new Phrase(cliente));
            cliente1.Border = 0;
            tableCliente.AddCell(cliente1);
            PdfPCell cliente2 = new PdfPCell(new Phrase("CNPJ: " + nd.Cliente.cnpj));
            cliente2.Border = 0;
            tableCliente.AddCell(cliente2);
            PdfPCell cliente3 = new PdfPCell(new Phrase("Endereço: " + nd.Cliente.endereco + ", " + nd.Cliente.numero));
            cliente3.Border = 0;
            tableCliente.AddCell(cliente3);
            PdfPCell cliente4 = new PdfPCell(new Phrase(nd.Cliente.bairro + " - " + nd.Cliente.cep));
            cliente4.Border = 0;
            tableCliente.AddCell(cliente4);
            PdfPCell cliente5 = new PdfPCell(new Phrase(nd.Cliente.cidade + " - " + nd.Cliente.uf));
            cliente5.Border = 0;
            tableCliente.AddCell(cliente5);

            doc.Add(tableCliente);

            //Pula duas linhas
            doc.Add(Chunk.NEWLINE);
            doc.Add(Chunk.NEWLINE);


            //Corpo da ND
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            float[] widths2 = new float[] { 400f, 100f };
            table.SetWidths(widths2);

            //Fonte do Título da Tabela
            Font tableTitle = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.WHITE);

            //Título Descrição
            Chunk desc = new Chunk("Descrição");
            desc.Font = tableTitle;
            Phrase descP = new Phrase(desc);
            PdfPCell cellDescricao = new PdfPCell(descP);
            cellDescricao.HorizontalAlignment = 1;
            cellDescricao.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(cellDescricao);

            //Título Valor
            Chunk val = new Chunk("Valor");
            val.Font = tableTitle;
            Phrase descV = new Phrase(val);
            PdfPCell cellValor = new PdfPCell(descV);
            cellValor.HorizontalAlignment = 1;
            cellValor.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(cellValor);

            //Conteúdo Descricao
            PdfPCell cellConteudoDescricao = new PdfPCell(new Phrase(nd.descricao));
            cellConteudoDescricao.HorizontalAlignment = 0;
            table.AddCell(cellConteudoDescricao);

            //Conteúdo Valor
            PdfPCell cellConteudoValor = new PdfPCell(new Phrase("R$ " + nd.valor.ToString("n")));
            cellConteudoValor.HorizontalAlignment = 1;
            table.AddCell(cellConteudoValor);


            doc.Add(table);

            doc.Close();

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/pdf", "ND - " + nd.numero + ".pdf");
        }


        public JsonResult ObterTodos()
        {
            var objs = new NotadeDebito().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.numero,
                DataEmissao = obj.DataEmissao.ToString("o"),
                DataVencimento = obj.DataVencimento.ToString("o"),
                DataRecebimento = (obj.DataRecebimento == null) ? "": obj.DataRecebimento.Value.ToString("o"),
                dataInclusao = (obj.dataInclusao == null) ? "" : obj.dataInclusao.Value.ToString("o"),
                obj.Cliente.nome,
                obj.cliente_id,
                obj.SituacaoNotaDebito.descricao,
                obj.valor,
                obj.bancoMovimento_id,
                obj.estabelecimento_id

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new NotadeDebito().ObterPorId(id, _paramBase);
            BancoMovimento bm = new BancoMovimento();
            if (obj == null)
            {
                obj = new NotadeDebito();
                obj.DataEmissao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                obj.DataVencimento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().AddDays(15);
                obj.numero = obj.ObterUltima(_paramBase) + 1;
                obj.estabelecimento_id = _estab;
            }
            else
            {
                bm = new BancoMovimento().ObterND(obj.numero, obj.bancoMovimento_id, null, _paramBase);
                if (bm == null)
                {
                    bm = new BancoMovimento();
                }
            }

            return Json(new
            {
                obj.id,
                banco_id = bm.banco_id,
                bancoMovimento_id = bm.id,
                obj.cliente_id,
                cliente_nome = (obj.Cliente == null) ? "" : obj.Cliente.nome,
                obj.comentario,
                DataCancelamento = (obj.DataCancelamento == null) ? "" : obj.DataCancelamento.Value.ToString("o"),
                DataEmissao = obj.DataEmissao.ToString("o"),
                dataInclusao = (obj.dataInclusao == null) ? "" : obj.dataInclusao.Value.ToString("o"),
                DataRecebimento = (obj.DataRecebimento == null) ? "" : obj.DataRecebimento.Value.ToString("o"),
                DataVencimento = obj.DataVencimento.ToString("o"),
                obj.descricao,
                obj.estabelecimento_id,
                obj.numero,
                planoconta_id = bm.planoDeConta_id,
                obj.situacaoNotaDebito_id,
                usuarioRecebido_id = obj.usuarioRecebimento_id,
                obj.valor
            }, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult Salvar(NotadeDebito obj, bool receber = false)
        {
            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                if (receber == false)
                {
                    var objErros = obj.Validar(ModelState);

                    if (obj.cliente_id == 0)
                    {
                        objErros.Add("Informe o cliente");
                    }

                    if (obj.valor == 0)
                    {
                        objErros.Add("Informe o valor");
                    }

                    if (obj.banco_id == 0)
                    {
                        objErros.Add("Informe o Banco/Agência");
                    }
                    if (obj.planoconta_id == 0)
                    {
                        objErros.Add("Informe o plano de contas");
                    }


                    if (objErros.Count() > 0)
                    {
                        return Json(new { CDStatus = "NOK", Erros = objErros });
                    }
                }

                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    if (receber == false)
                    {
                        BancoMovimento bm = new BancoMovimento().ObterND(obj.numero, obj.bancoMovimento_id, db, _paramBase);
                        if (bm == null)
                            bm = new BancoMovimento();


                        bm.data = obj.DataVencimento;
                        bm.historico = "Nota de Débito: " + obj.numero.ToString();
                        bm.origemmovimento_id = new OrigemMovimento().TipoNotaDebito(_paramBase);
                        bm.tipoDeMovimento_id = new TipoMovimento().TipoEntrada(_paramBase);
                        bm.tipoDeDocumento_id = new TipoDocumento().TipoNotaDebito();
                        bm.banco_id = obj.banco_id;
                        bm.planoDeConta_id = obj.planoconta_id;
                        bm.valor = obj.valor;
                        if (bm.id == 0)
                            bm.Incluir(_paramBase, db);
                        else
                            bm.Alterar(_paramBase, db);
                        obj.bancoMovimento_id = bm.id;
                    }

                    
                    if (obj.id == 0)
                    {
                        obj.estabelecimento_id = _estab;
                        obj.situacaoNotaDebito_id = 1;
                        obj.DataRecebimento = null;
                        obj.DataCancelamento = null;
                        obj.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        obj.numero = obj.ObterUltima(_paramBase, db) + 1;
                        obj.Incluir(_paramBase, db);
                    }
                    else
                    {
                        if (receber)
                        {
                            obj.situacaoNotaDebito_id = 2;
                            obj.DataRecebimento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia() ;
                            obj.usuarioRecebimento = _usuarioobj.codigo;
                            obj.usuarioRecebimento_id = _usuarioobj.id;
                        }
                        obj.Alterar(obj, _paramBase, db);
                    }
                    dbcxtransaction.Commit();
                }
                return Json(new { CDStatus = "OK", DSMessage = "Registro salvo com sucesso número: " + obj.numero }, JsonRequestBehavior.AllowGet);
                

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(NotadeDebito obj)
        {

            try
            {
                var objAux = new NotadeDebito().ObterPorId(obj.id, _paramBase);
                if (objAux == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Nota não encontrada" }, JsonRequestBehavior.AllowGet);
                if (objAux.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                string erro = "";

                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    if (new NotadeDebito().Excluir(obj.id, ref erro, _paramBase, db))
                    {
                        if (erro != "")
                        {
                            dbcxtransaction.Rollback();
                            throw new Exception(erro);
                        }

                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                    }

                    BancoMovimento bm = new BancoMovimento().ObterND(obj.numero, obj.bancoMovimento_id, db, _paramBase);

                    if (bm != null)
                    {
                        if (bm.Excluir(bm.id, ref erro, _paramBase,db))
                        {
                            if (erro != "")
                            {
                                dbcxtransaction.Rollback();
                                throw new Exception(erro);
                            }
                        }
                        else
                        {
                            dbcxtransaction.Rollback();
                            return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    dbcxtransaction.Commit();
                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);


                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = int.Parse(p.Value),
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterCliente()
        {
            var objs = new Pessoa().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nome
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPlanoConta()
        {
            var objs =  new PlanoDeConta().ObterNotadeDebito();
            return Json(objs.Select(p => new
            {
                Value = int.Parse(p.Value),
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        
    }
}
