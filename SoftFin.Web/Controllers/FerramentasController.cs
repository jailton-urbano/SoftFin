using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Xml;
using System.Text;
using System.Net;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;

namespace SoftFin.Web.Controllers
{
    public class FerramentasController : BaseController
    {

        public ActionResult ExportaExcel()
        {
            CarregaViewData();
            CarregaBanco();
            return View();
        }

        private void CarregaViewData()
        {
            var items = new List<SelectListItem>();

            //Adiciona Tabelas ao List de seleção para exportação do Excel
            items.Add(new SelectListItem() { Value = "1", Text = "Banco Movimento" });
            items.Add(new SelectListItem() { Value = "2", Text = "Contas a Pagar" });
            items.Add(new SelectListItem() { Value = "3", Text = "Pessoas" });
            items.Add(new SelectListItem() { Value = "4", Text = "Extrato OFX Importado" });
            items.Add(new SelectListItem() { Value = "5", Text = "Recebimentos" });
            items.Add(new SelectListItem() { Value = "6", Text = "Pagamentos" });

            ViewData["Tabela"] = new SelectList(items, "Value", "Text");
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

        public FileStreamResult geraExcelBancoMovimento2(string dataInicial, string dataFinal, int banco)
        {

                ViewBag.usuario = Acesso.UsuarioLogado();
                ViewBag.perfil = Acesso.PerfilLogado();
                int estab = _paramBase.estab_id;

                DateTime DataInicial = new DateTime();
                DataInicial = DateTime.Parse(dataInicial);
                DateTime DataFinal = new DateTime();
                DataFinal = DateTime.Parse(dataFinal);

                var db = new DbControle();
                var bm = new List<ExcelBancoMovimento>();
                var nota = new NotaFiscal();
                var doc = new DocumentoPagarMestre();
                int vNota = 0;
                int vDocumentoPagar = 0;

                bm = (from m in db.BancoMovimento
                        where m.data >= DataInicial && m.data <= DataFinal &&
                        m.banco_id == banco
                        select new ExcelBancoMovimento
                        {
                        planoDeContas           = m.PlanoDeConta.descricao,
                        tipoMovimento           = m.TipoMovimento.Descricao,
                        tipoDocumento           = m.TipoDocumento.descricao,
                        data                    = m.data,
                        historico               = m.historico,
                        valor                   = m.valor,
                        idNotaFiscal            = m.notafiscal_id,
                        idDocumentoPagar        = m.DocumentoPagarParcela_id
                        }).ToList();

                for (int i = 0; i < bm.Count; i++)
                {
                        if (bm[i].idNotaFiscal != null)
                        {
                            //Atualiza número da Nota Fiscal
                            vNota = bm[i].idNotaFiscal.GetValueOrDefault();
                            bm[i].numeroNotaFiscal = nota.ObterPorId(vNota).numeroNfse;
                        }
                        if (bm[i].idDocumentoPagar != null)
                        {
                            //Atualiza número do Documento do Contas a Pagar
                            vDocumentoPagar = bm[i].idDocumentoPagar.GetValueOrDefault();
                            bm[i].numeroDocumentoPagar = doc.ObterPorId(vDocumentoPagar, _paramBase).numeroDocumento;
                        }
                };

            MemoryStream stream = new MemoryStream();

            // Create a SpreadsheetDocument based on a stream.
            SpreadsheetDocument xl =
            SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart wbp = xl.AddWorkbookPart();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();

            // Create a new Workbook
            Workbook wb = new Workbook();

            FileVersion fv = new FileVersion();
            fv.ApplicationName = "Microsoft Office Excel";

            //Create a new Worksheet
            Worksheet ws = new Worksheet();

            //Cria SheetData
            SheetData sd = new SheetData();

            //Cria Títulos da Planilha

            //Título Coluna Plano de Contas
            Row r0 = new Row() { RowIndex = Convert.ToUInt32(1) };
            Cell c0 = new Cell();
            c0.CellReference = "B1";
            c0.DataType = CellValues.String;
            c0.CellValue = new CellValue("Plano de Contas");
            r0.Append(c0);

            Cell c1 = new Cell();
            c1.CellReference = "C1";
            c1.DataType = CellValues.String;
            c1.CellValue = new CellValue("Tipo do Movimento");
            r0.Append(c1);
            Cell c2 = new Cell();
            c2.CellReference = "D1";
            c2.DataType = CellValues.String;
            c2.CellValue = new CellValue("Tipo do Documento");
            r0.Append(c2);
            Cell c3 = new Cell();
            c3.CellReference = "E1";
            c3.DataType = CellValues.String;
            c3.CellValue = new CellValue("Data");
            r0.Append(c3);
            Cell c4 = new Cell();
            c4.CellReference = "F1";
            c4.DataType = CellValues.String;
            c4.CellValue = new CellValue("Histórico");
            r0.Append(c4);
            Cell c5 = new Cell();
            c5.CellReference = "G1";
            c5.DataType = CellValues.String;
            c5.CellValue = new CellValue("Valor");
            r0.Append(c5);
            Cell c6 = new Cell();
            c6.CellReference = "H1";
            c6.DataType = CellValues.String;
            c6.CellValue = new CellValue("Número da Nota");
            r0.Append(c6);
            Cell c7 = new Cell();
            c7.CellReference = "I1";
            c7.DataType = CellValues.String;
            c7.CellValue = new CellValue("Número do Documento Contas a Pagar");
            r0.Append(c7);
            sd.Append(r0);


            for (int i2 = 0; i2 < bm.Count; i2++)
            {
                int i3 = i2 + 2;

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3)};

                ////Cria Célula Banco
                Cell c8 = new Cell();
                c8.CellReference = "B" + i3;
                c8.DataType = CellValues.String;
                c8.CellValue = new CellValue(bm[i2].planoDeContas);
                r1.Append(c8);

                ////Cria Célula Tipo do Movimento
                Cell c9 = new Cell();
                c9.CellReference = "C" + i3;
                c9.DataType = CellValues.String;
                c9.CellValue = new CellValue(bm[i2].tipoMovimento);
                r1.Append(c9);

                ////Cria Célula Tipo do Documento
                Cell c10 = new Cell();
                c10.CellReference = "D" + i3;
                c10.DataType = CellValues.String;
                c10.CellValue = new CellValue(bm[i2].tipoDocumento);
                r1.Append(c10);

                ////Cria Célula Data
                Cell c11 = new Cell();
                c11.CellReference = "E" + i3;
                c11.DataType = CellValues.String;
                c11.CellValue = new CellValue(bm[i2].data.ToShortDateString());
                r1.Append(c11);

                ////Cria Célula Histórico
                Cell c12 = new Cell();
                c12.CellReference = "F" + i3;
                c12.DataType = CellValues.String;
                c12.CellValue = new CellValue(bm[i2].historico);
                r1.Append(c12);

                ////Cria Célula Histórico
                Cell c13 = new Cell();
                c13.CellReference = "G" + i3;
                c13.DataType = CellValues.String;
                c13.CellValue = new CellValue(bm[i2].valor.ToString("n2"));
                r1.Append(c13);

                ////Cria Célula Histórico
                Cell c14 = new Cell();
                c14.CellReference = "H" + i3;
                c14.DataType = CellValues.String;
                c14.CellValue = new CellValue(bm[i2].numeroNotaFiscal.ToString());
                r1.Append(c14);

                ////Cria Célula Histórico
                Cell c15 = new Cell();
                c15.CellReference = "I" + i3;
                c15.DataType = CellValues.String;
                c15.CellValue = new CellValue(bm[i2].numeroDocumentoPagar.ToString());
                r1.Append(c15);

                //Adiciona linha na Sheetdata
                sd.Append(r1);
            }

            //Adicionar SheetData no Worksheet
            ws.Append(sd);

            //Adiciona worksheet no worksheetpart
            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            //SpreadsheetDocument(WorkbookPart) = Workbook(Sheets(Sheet(WorkbookPart(WorksheetPart = Worksheet(SheetData(Row(Cell)))))))

            Sheets sheets = new Sheets();
            Sheet sheet = new Sheet();
            sheet.Name = "Banco Movimento";
            sheet.SheetId = 1;
            sheet.Id = wbp.GetIdOfPart(wsp);
            sheets.Append(sheet);
            wb.Append(fv);
            wb.Append(sheets);

            xl.WorkbookPart.Workbook = wb;
            xl.WorkbookPart.Workbook.Save();
            xl.Close();

            string fileName = "arquivo-BancoMovimento" + dataInicial + "-" + dataFinal + ".xlsx";


            stream.Flush();
            stream.Position = 0;

            return File(stream,"application/Excel", fileName );
        }

        public FileStreamResult geraExcelDocumentoPagar2(string dataInicial, string dataFinal, int banco)
        {

            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var db = new DbControle();
            var bm = new List<ExcelDocumentoPagar>();
            var nota = new NotaFiscal();
            var doc = new DocumentoPagarMestre();

            bm = (from m in db.DocumentoPagarParcela
                  where m.DocumentoPagarMestre.dataDocumento >= DataInicial && m.DocumentoPagarMestre.dataDocumento <= DataFinal && m.DocumentoPagarMestre.estabelecimento_id == estab
                  && m.DocumentoPagarMestre.banco_id == banco
                  select new ExcelDocumentoPagar
                  {
                        dataLancamento = m.DocumentoPagarMestre.dataLancamento,
                        dataCompetencia     = m.DocumentoPagarMestre.dataCompetencia,
                        //dataVencimento      = m.DocumentoPagarMestre.dataVencimento,
                        valorBruto          = m.DocumentoPagarMestre.valorBruto,
                        tipoDocumento       = m.DocumentoPagarMestre.tipoDocumento.descricao,
                        numeroDocumento     = m.DocumentoPagarMestre.numeroDocumento,
                        dataDocumento       = m.DocumentoPagarMestre.dataDocumento,
                        situacaoPagamento   = m.DocumentoPagarMestre.situacaoPagamento,
                        lotePagamentoBanco  = m.lotePagamentoBanco,
                        tipoPessoa          = m.DocumentoPagarMestre.Pessoa.TipoPessoa.Descricao,
                        pessoa              = m.DocumentoPagarMestre.Pessoa.nome,
                        usuarioAutorizador  = m.usuarioAutorizador.nome,
                        banco               = m.DocumentoPagarMestre.Banco.nomeBanco,
                        linhaDigitavel      = m.DocumentoPagarMestre.LinhaDigitavel,
                        statusPagamento     = m.DocumentoPagarMestre.StatusPagamento,
                        planoDeConta        = m.DocumentoPagarMestre.PlanoDeConta.descricao

                  }).ToList();

            MemoryStream stream = new MemoryStream();

            // Create a SpreadsheetDocument based on a stream.DocumentoPagarMestre.
            SpreadsheetDocument xl =
            SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart wbp = xl.AddWorkbookPart();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();

            // Create a new Workbook
            Workbook wb = new Workbook();

            FileVersion fv = new FileVersion();
            fv.ApplicationName = "Microsoft Office Excel";

            //Create a new Worksheet
            Worksheet ws = new Worksheet();

            //Cria SheetData
            SheetData sd = new SheetData();

            //Cria Títulos da Planilha
            Row r0 = new Row() { RowIndex = Convert.ToUInt32(1) };
            Cell c0 = new Cell();
            c0.CellReference = "B1";
            c0.DataType = CellValues.String;
            c0.CellValue = new CellValue("Data Lançamento");
            r0.Append(c0);
            Cell c1 = new Cell();
            c1.CellReference = "C1";
            c1.DataType = CellValues.String;
            c1.CellValue = new CellValue("Data Competência");
            r0.Append(c1);
            Cell c2 = new Cell();
            c2.CellReference = "D1";
            c2.DataType = CellValues.String;
            c2.CellValue = new CellValue("Data Vencimento");
            r0.Append(c2);
            Cell c3 = new Cell();
            c3.CellReference = "E1";
            c3.DataType = CellValues.String;
            c3.CellValue = new CellValue("Valor Bruto");
            r0.Append(c3);
            Cell c4 = new Cell();
            c4.CellReference = "F1";
            c4.DataType = CellValues.String;
            c4.CellValue = new CellValue("Número Documento");
            r0.Append(c4);
            Cell c5 = new Cell();
            c5.CellReference = "G1";
            c5.DataType = CellValues.String;
            c5.CellValue = new CellValue("Data Documento");
            r0.Append(c5);
            Cell c6 = new Cell();
            c6.CellReference = "H1";
            c6.DataType = CellValues.String;
            c6.CellValue = new CellValue("Situação Pagamento");
            r0.Append(c6);
            Cell c7 = new Cell();
            c7.CellReference = "I1";
            c7.DataType = CellValues.String;
            c7.CellValue = new CellValue("Plano de Contas");
            r0.Append(c7);
            Cell c8 = new Cell();
            c8.CellReference = "J1";
            c8.DataType = CellValues.String;
            c8.CellValue = new CellValue("Tipo Documento");
            r0.Append(c8);
            Cell c9 = new Cell();
            c9.CellReference = "K1";
            c9.DataType = CellValues.String;
            c9.CellValue = new CellValue("Lote Pagamento Banco");
            r0.Append(c9);
            Cell c10 = new Cell();
            c10.CellReference = "L1";
            c10.DataType = CellValues.String;
            c10.CellValue = new CellValue("Tipo Pessoa");
            r0.Append(c10);
            Cell c11 = new Cell();
            c11.CellReference = "M1";
            c11.DataType = CellValues.String;
            c11.CellValue = new CellValue("Fornecedor");
            r0.Append(c11);
            Cell c12 = new Cell();
            c12.CellReference = "N1";
            c12.DataType = CellValues.String;
            c12.CellValue = new CellValue("Usuário Autorizador");
            r0.Append(c12);
            Cell c13 = new Cell();
            c13.CellReference = "O1";
            c13.DataType = CellValues.String;
            c13.CellValue = new CellValue("Banco");
            r0.Append(c13);
            Cell c14 = new Cell();
            c14.CellReference = "P1";
            c14.DataType = CellValues.String;
            c14.CellValue = new CellValue("Linha Boleto");
            r0.Append(c14);
            Cell c15 = new Cell();
            c15.CellReference = "Q1";
            c15.DataType = CellValues.String;
            c15.CellValue = new CellValue("Status Pagamento");
            r0.Append(c15);

            sd.Append(r0);


            for (int i2 = 0; i2 < bm.Count; i2++)
            {
                int i3 = i2 + 2;

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };

                ////Cria Célula Data de Lançamento
                Cell c16 = new Cell();
                c16.CellReference = "B" + i3;
                c16.DataType = CellValues.String;
                c16.CellValue = new CellValue(bm[i2].dataLancamento.ToShortDateString());
                r1.Append(c16);

                ////Cria Célula Data de Competência
                Cell c17 = new Cell();
                c17.CellReference = "C" + i3;
                c17.DataType = CellValues.String;
                c17.CellValue = new CellValue(bm[i2].dataCompetencia);
                r1.Append(c17);

                ////Cria Célula Data de Vencimento
                Cell c18 = new Cell();
                c18.CellReference = "D" + i3;
                c18.DataType = CellValues.String;
                c18.CellValue = new CellValue(bm[i2].dataVencimento.ToShortDateString());
                r1.Append(c18);

                ////Cria Célula Valor Bruto
                Cell c19 = new Cell();
                c19.CellReference = "E" + i3;
                c19.DataType = CellValues.String;
                c19.CellValue = new CellValue(bm[i2].valorBruto.ToString("n2"));
                r1.Append(c19);

                ////Cria Célula Número do Documento
                Cell c20 = new Cell();
                c20.CellReference = "F" + i3;
                c20.DataType = CellValues.String;
                c20.CellValue = new CellValue(bm[i2].numeroDocumento.ToString());
                r1.Append(c20);

                ////Cria Célula Data do Documento
                Cell c21 = new Cell();
                c21.CellReference = "G" + i3;
                c21.DataType = CellValues.String;
                c21.CellValue = new CellValue(bm[i2].dataDocumento.ToShortDateString());
                r1.Append(c21);

                ////Cria Célula Situação Pagamento
                Cell c22 = new Cell();
                c22.CellReference = "H" + i3;
                c22.DataType = CellValues.String;
                c22.CellValue = new CellValue(bm[i2].situacaoPagamento);
                r1.Append(c22);

                ////Cria Célula Plano de Contas
                Cell c23 = new Cell();
                c23.CellReference = "I" + i3;
                c23.DataType = CellValues.String;
                c23.CellValue = new CellValue(bm[i2].planoDeConta);
                r1.Append(c23);

                ////Cria Célula Lote Pagamento Banco
                Cell c24 = new Cell();
                c24.CellReference = "J" + i3;
                c24.DataType = CellValues.String;
                c24.CellValue = new CellValue(bm[i2].tipoDocumento);
                r1.Append(c24);

                ////Cria Célula Pessoa
                Cell c26 = new Cell();
                c26.CellReference = "K" + i3;
                c26.DataType = CellValues.String;
                c26.CellValue = new CellValue(bm[i2].lotePagamentoBanco);
                r1.Append(c26);

                ////Cria Célula Tipo Pessoa
                Cell c25 = new Cell();
                c25.CellReference = "L" + i3;
                c25.DataType = CellValues.String;
                c25.CellValue = new CellValue(bm[i2].tipoPessoa);
                r1.Append(c25);

                ////Cria Célula Tipo Documento
                Cell c27 = new Cell();
                c27.CellReference = "M" + i3;
                c27.DataType = CellValues.String;
                c27.CellValue = new CellValue(bm[i2].pessoa);
                r1.Append(c27);

                ////Cria Célula Usuário Autorizador
                Cell c28 = new Cell();
                c28.CellReference = "N" + i3;
                c28.DataType = CellValues.String;
                c28.CellValue = new CellValue(bm[i2].usuarioAutorizador);
                r1.Append(c28);

                ////Cria Célula Banco
                Cell c29 = new Cell();
                c29.CellReference = "O" + i3;
                c29.DataType = CellValues.String;
                c29.CellValue = new CellValue(bm[i2].banco);
                r1.Append(c29);

                ////Cria Célula Linha Digitável
                Cell c30 = new Cell();
                c30.CellReference = "P" + i3;
                c30.DataType = CellValues.String;
                c30.CellValue = new CellValue(bm[i2].linhaDigitavel);
                r1.Append(c30);

                ////Cria Célula Status Pagamento
                if (bm[i2].statusPagamento == 1)
                    bm[i2].statusPagamentoS = "Em Aberto";
                if (bm[i2].statusPagamento == 2)
                    bm[i2].statusPagamentoS = "Pago Parcial";
                if (bm[i2].statusPagamento == 3)
                    bm[i2].statusPagamentoS = "Pago Total";
                Cell c31 = new Cell();
                c31.CellReference = "Q" + i3;
                c31.DataType = CellValues.String;
                c31.CellValue = new CellValue(bm[i2].statusPagamentoS);
                r1.Append(c31);

                //Adiciona linha na Sheetdata
                sd.Append(r1);
            }

            //Adicionar SheetData no Worksheet
            ws.Append(sd);

            //Adiciona worksheet no worksheetpart
            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            //SpreadsheetDocument(WorkbookPart) = Workbook(Sheets(Sheet(WorkbookPart(WorksheetPart = Worksheet(SheetData(Row(Cell)))))))

            Sheets sheets = new Sheets();
            Sheet sheet = new Sheet();
            sheet.Name = "Banco Movimento";
            sheet.SheetId = 1;
            sheet.Id = wbp.GetIdOfPart(wsp);
            sheets.Append(sheet);
            wb.Append(fv);
            wb.Append(sheets);

            xl.WorkbookPart.Workbook = wb;
            xl.WorkbookPart.Workbook.Save();
            xl.Close();

            string fileName = "Contas a Pagar" + dataInicial + "-" + dataFinal + ".xlsx";


            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }

        public FileStreamResult geraExcelDocumentoPagar(string dataInicial, string dataFinal, int banco)
        {

            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var db = new DbControle();
            var bm = new List<ExcelDocumentoPagar>();
            var nota = new NotaFiscal();
            var doc = new DocumentoPagarMestre();

            bm = (from m in db.DocumentoPagarParcela
                  where m.DocumentoPagarMestre.dataDocumento >= DataInicial && m.DocumentoPagarMestre.dataDocumento <= DataFinal && m.DocumentoPagarMestre.estabelecimento_id == estab
                  && m.DocumentoPagarMestre.banco_id == banco
                  select new ExcelDocumentoPagar
                  {
                      dataLancamento = m.DocumentoPagarMestre.dataLancamento,
                      dataCompetencia = m.DocumentoPagarMestre.dataCompetencia,
                      dataVencimentoOriginal = m.vencimento,
                      dataVencimento = m.vencimentoPrevisto,
                      valorBruto = m.DocumentoPagarMestre.valorBruto,
                      tipoDocumento = m.DocumentoPagarMestre.tipoDocumento.descricao,
                      numeroDocumento = m.DocumentoPagarMestre.numeroDocumento,
                      dataDocumento = m.DocumentoPagarMestre.dataDocumento,
                      situacaoPagamento = m.DocumentoPagarMestre.situacaoPagamento,
                      lotePagamentoBanco = m.lotePagamentoBanco,
                      tipoPessoa = m.DocumentoPagarMestre.Pessoa.TipoPessoa.Descricao,
                      pessoa = m.DocumentoPagarMestre.Pessoa.nome,
                      usuarioAutorizador = m.usuarioAutorizador.nome,
                      banco = m.DocumentoPagarMestre.Banco.nomeBanco,
                      linhaDigitavel = m.DocumentoPagarMestre.LinhaDigitavel,
                      statusPagamento = m.DocumentoPagarMestre.StatusPagamento,
                      planoDeConta = m.DocumentoPagarMestre.PlanoDeConta.descricao

                  }).ToList();


            // Create a SpreadsheetDocument based on a stream.
            ExcelPackage ep = new ExcelPackage();
            string fileName = "Contas_a_Pagar_" + dataInicial + "_" + dataFinal + ".xlsx";
            ExcelWorksheet ews = ep.Workbook.Worksheets.Add(fileName);

            //Título da Planilha
            ews.Cells["B1"].Value = "Contas a Pagar";
            ews.Cells["B2"].Value = "Período: " + dataInicial + " até " + dataFinal;
            ews.Cells["B1"].Style.Font.Size = 22;
            ews.Cells["B2"].Style.Font.Bold = true;


            ews.Cells["B3"].Value = "Data Lançamento";
            ews.Cells["C3"].Value = "Data Competência";
            ews.Cells["D3"].Value = "Data Vencimento Original";
            ews.Cells["E3"].Value = "Data Vencimento";
            ews.Cells["F3"].Value = "Valor Bruto";
            ews.Cells["G3"].Value = "Número Documento";
            ews.Cells["H3"].Value = "Data Documento";
            ews.Cells["I3"].Value = "Situação Pagamento";
            ews.Cells["J3"].Value = "Plano de Contas";
            ews.Cells["K3"].Value = "Tipo Documento";
            ews.Cells["L3"].Value = "Lote Pagamento Banco";
            ews.Cells["M3"].Value = "Tipo Pessoa";
            ews.Cells["N3"].Value = "Fornecedor";
            ews.Cells["O3"].Value = "Usuário Autorizador";
            ews.Cells["P3"].Value = "Banco";
            ews.Cells["Q3"].Value = "Linha Boleto";
            ews.Cells["R3"].Value = "Status Pagamento";
            ews.Cells["B3:R3"].Style.Font.Bold = true;
            ews.Cells["B3:R3"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ews.Cells["B3:R3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ews.Cells["B3:R3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            ews.Cells["B3:R3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            for (int i2 = 0; i2 < bm.Count; i2++)
            {
                int i3 = i2 + 4;

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };

                //Cria células
                ews.Cells["B" + i3].Value = bm[i2].dataLancamento.ToShortDateString();
                ews.Cells["C" + i3].Value = bm[i2].dataCompetencia;
                ews.Cells["D" + i3].Value = bm[i2].dataVencimentoOriginal.ToShortDateString();
                ews.Cells["E" + i3].Value = bm[i2].dataVencimento.ToShortDateString();
                ews.Cells["F" + i3].Value = bm[i2].valorBruto;
                ews.Cells["G" + i3].Value = bm[i2].numeroDocumento;
                ews.Cells["H" + i3].Value = bm[i2].dataDocumento.ToShortDateString();
                ews.Cells["I" + i3].Value = bm[i2].situacaoPagamento;
                ews.Cells["J" + i3].Value = bm[i2].planoDeConta;
                ews.Cells["K" + i3].Value = bm[i2].tipoDocumento;
                ews.Cells["L" + i3].Value = bm[i2].lotePagamentoBanco;
                ews.Cells["M" + i3].Value = bm[i2].tipoPessoa;
                ews.Cells["N" + i3].Value = bm[i2].pessoa;
                ews.Cells["O" + i3].Value = bm[i2].usuarioAutorizador;
                ews.Cells["P" + i3].Value = bm[i2].banco;
                ews.Cells["Q" + i3].Value = bm[i2].linhaDigitavel;
                ////Cria Célula Status Pagamento
                if (bm[i2].statusPagamento == 1)
                    bm[i2].statusPagamentoS = "Em Aberto";
                if (bm[i2].statusPagamento == 2)
                    bm[i2].statusPagamentoS = "Pago Parcial";
                if (bm[i2].statusPagamento == 3)
                    bm[i2].statusPagamentoS = "Pago Total"; 
                ews.Cells["R" + i3].Value = bm[i2].statusPagamentoS;

                //Aplicando Estilos
                ews.Cells["F" + i3].Style.Numberformat.Format = "0,00";
                ews.Cells["G" + i3].Style.Numberformat.Format = "0";
                ews.Cells["C" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["D" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["E" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["H" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["I" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["K" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["M" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Change the sheet view to show it in page layout mode
            ews.View.PageLayoutView = true;

            var usuario = Acesso.UsuarioLogado();

            // set some document properties
            ep.Workbook.Properties.Title = "Contas a Pagar";
            ep.Workbook.Properties.Author = usuario;
            ep.Workbook.Properties.Comments = "";

            // set some extended property values
            ep.Workbook.Properties.Company = "SoftFin";

            // set some custom property values
            ep.Workbook.Properties.SetCustomPropertyValue("Checked by", "SoftFin");
            ep.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

            // Cria Memory Stream
            var stream = new MemoryStream(ep.GetAsByteArray());

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }

        public FileStreamResult geraExcelBancoMovimento(string dataInicial, string dataFinal, int banco)
        {

            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var db = new DbControle();
            var bm = new List<ExcelBancoMovimento>();

            bm = (from m in db.BancoMovimento
                  where m.data >= DataInicial && m.data <= DataFinal &&
                  m.banco_id == banco
                  select new ExcelBancoMovimento
                  {
                      planoDeContas = m.PlanoDeConta.descricao,
                      tipoMovimento = m.TipoMovimento.Descricao,
                      tipoDocumento = m.TipoDocumento.descricao,
                      data = m.data,
                      historico = m.historico,
                      valor = m.valor,
                      nota = m.NotaFiscal,
                      recebimento = m.Recebimento,
                      pagamento = m.Pagamento,
                      OrigemMovimento = m.OrigemMovimento.Tipo
                  }).ToList();

            for (int i = 0; i < bm.Count; i++)
            {
                if (bm[i].nota != null)
                {
                    //Atualiza número da Nota Fiscal
                    bm[i].numeroNotaFiscal = bm[i].nota.numeroNfse;
                }
                if (bm[i].documento != null)
                {
                    //Atualiza número do Documento do Contas a Pagar
                    bm[i].numeroDocumentoPagar = bm[i].documento.numeroDocumento;
                }
            };


            // Create a SpreadsheetDocument based on a stream.
            ExcelPackage ep = new ExcelPackage();
            string fileName = "Banco_Movimento_" + dataInicial + "_" + dataFinal + ".xlsx";
            ExcelWorksheet ews = ep.Workbook.Worksheets.Add(fileName);

            //Título da Planilha
            ews.Cells["B1"].Value = "Banco Movimento";
            ews.Cells["B2"].Value = "Período: " + dataInicial + " até " + dataFinal;
            ews.Cells["B1"].Style.Font.Size = 22;
            ews.Cells["B2"].Style.Font.Bold = true;

            ews.Cells["B3"].Value = "Plano de Contas";
            ews.Cells["C3"].Value = "Tipo do Movimento";
            ews.Cells["D3"].Value = "Tipo do Documento";
            ews.Cells["E3"].Value = "Data";
            ews.Cells["F3"].Value = "Histórico";
            ews.Cells["G3"].Value = "Valor";
            ews.Cells["H3"].Value = "Número da Nota";
            ews.Cells["I3"].Value = "Número do Documento Contas a Pagar";
            ews.Cells["J3"].Value = "Pessoa";
            ews.Cells["K3"].Value = "Origem Movimento";
            ews.Cells["B3:K3"].Style.Font.Bold = true;
            ews.Cells["B3:K3"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ews.Cells["B3:K3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ews.Cells["B3:K3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            ews.Cells["B3:K3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            for (int i2 = 0; i2 < bm.Count; i2++)
            {
                int i3 = i2 + 4;

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };

                //Cria células
                ews.Cells["B" + i3].Value = bm[i2].planoDeContas;
                ews.Cells["C" + i3].Value = bm[i2].tipoMovimento;
                ews.Cells["D" + i3].Value = bm[i2].tipoDocumento;
                ews.Cells["E" + i3].Value = bm[i2].data.ToShortDateString();
                ews.Cells["F" + i3].Value = bm[i2].historico;
                ews.Cells["G" + i3].Value = bm[i2].valor;
                ews.Cells["H" + i3].Value = bm[i2].numeroNotaFiscal;
                ews.Cells["I" + i3].Value = bm[i2].numeroDocumentoPagar;

                string pessoa = "";

                if (bm[i2].nota != null)
                {
                    if (bm[i2].nota.numeroNfse != null)
                    {
                        pessoa = bm[i2].nota.NotaFiscalPessoaTomador.razao;
                    }
                }
                else if (bm[i2].recebimento != null)
                {
                    pessoa = bm[i2].recebimento.notaFiscal.NotaFiscalPessoaTomador.razao;
                }

                if (bm[i2].documento != null)
                {
                    pessoa = bm[i2].documento.Pessoa.nome;
                }
                else if (bm[i2].pagamento != null)
                {
                    pessoa = bm[i2].pagamento.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
                }

                ews.Cells["j" + i3].Value = pessoa;
                ews.Cells["K" + i3].Value = bm[i2].OrigemMovimento;

                //Aplicando Estilos
                ews.Cells["C" + i3].Style.Numberformat.Format = "0";
                ews.Cells["D" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["E" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["G" + i3].Style.Numberformat.Format = "0";
                ews.Cells["H" + i3].Style.Numberformat.Format = "0";
                ews.Cells["I" + i3].Style.Numberformat.Format = "0";
            }

            // Change the sheet view to show it in page layout mode
            ews.View.PageLayoutView = true;

            var usuario = Acesso.UsuarioLogado();

            // set some document properties
            ep.Workbook.Properties.Title = "Banco Movimento";
            ep.Workbook.Properties.Author = usuario;
            ep.Workbook.Properties.Comments = "";

            // set some extended property values
            ep.Workbook.Properties.Company = "SoftFin";

            // set some custom property values
            ep.Workbook.Properties.SetCustomPropertyValue("Checked by", "SoftFin");
            ep.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

            // Cria Memory Stream
            var stream = new MemoryStream(ep.GetAsByteArray());

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }

        public FileStreamResult geraExcelPessoas()
        {

            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int emp  = _paramBase.empresa_id;

            var db = new DbControle();
            var bm = new List<ExcelPessoas>();
            var nota = new NotaFiscal();
            var doc = new DocumentoPagarMestre();


            bm = (from p in db.Pessoa
                  where p.empresa_id == emp
                  select new ExcelPessoas
                  {
                     codigo                 = p.codigo,
                     nome                   = p.nome,
                     razao                  = p.razao,
                     cnpj                   = p.cnpj,
                     inscricao              = p.inscricao,
                     ccm                    = p.ccm,
                     endereco               = p.endereco,
                     numero                 = p.numero,
                     complemento            = p.complemento,
                     bairro                 = p.bairro,
                     cidade                 = p.cidade,
                     uf                     = p.uf,
                     cep                    = p.cep,
                     eMail                  = p.eMail,
                     bancoConta             = p.bancoConta,
                     agenciaConta           = p.agenciaConta,
                     agenciaDigito          = p.agenciaDigito,
                     contaBancaria          = p.contaBancaria,
                     digitoContaBancaria    = p.digitoContaBancaria,
                     unidadeNegocio         = p.UnidadeNegocio.unidade,
                     tipoEndereco           = p.TipoEndereco.Descricao,
                     categoriaPessoa        = p.CategoriaPessoa.Descricao,
                     tipoPessoa             = p.TipoPessoa.Descricao,
                     telefoneFixo           = p.TelefoneFixo,
                     celular                = p.Celular
                  }).ToList();


            // Create a SpreadsheetDocument based on a stream.
            ExcelPackage ep = new ExcelPackage();
            string fileName = "Pessoas.xlsx";
            ExcelWorksheet ews = ep.Workbook.Worksheets.Add(fileName);

            //Título da Planilha
            ews.Cells["B1"].Value = "Pessoas";
            ews.Cells["B1"].Style.Font.Size = 22;
            ews.Cells["B1"].Style.Font.Bold = true;

            ews.Cells["B3"].Value = "Código";
            ews.Cells["C3"].Value = "Nome";
            ews.Cells["D3"].Value = "Razao Social";
            ews.Cells["E3"].Value = "Cnpj";
            ews.Cells["F3"].Value = "Inscrição";
            ews.Cells["G3"].Value = "CCM";
            ews.Cells["H3"].Value = "Endereço";
            ews.Cells["I3"].Value = "Número";
            ews.Cells["J3"].Value = "Complemento";
            ews.Cells["K3"].Value = "Bairro";
            ews.Cells["L3"].Value = "Cidade";
            ews.Cells["M3"].Value = "UF";
            ews.Cells["N3"].Value = "CEP";
            ews.Cells["O3"].Value = "E-mail";
            ews.Cells["P3"].Value = "Banco Conta";
            ews.Cells["Q3"].Value = "Agência Conta";
            ews.Cells["R3"].Value = "Agência Dígito";
            ews.Cells["S3"].Value = "Conta Bancária";
            ews.Cells["T3"].Value = "Dígito Conta Bancária";
            ews.Cells["U3"].Value = "Unidade de Negócio";
            ews.Cells["V3"].Value = "Tipo de Endereço";
            ews.Cells["W3"].Value = "Categoria Pessoa";
            ews.Cells["X3"].Value = "Tipo Pessoa";
            ews.Cells["Y3"].Value = "Telefone Fixo";
            ews.Cells["Z3"].Value = "Celular";

            ews.Cells["B3:Z3"].Style.Font.Bold = true;
            ews.Cells["B3:Z3"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ews.Cells["B3:Z3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ews.Cells["B3:Z3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            ews.Cells["B3:Z3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            for (int i2 = 0; i2 < bm.Count; i2++)
            {
                int i3 = i2 + 4;

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };
      
                //Cria células
                ews.Cells["B" + i3].Value = bm[i2].codigo;
                ews.Cells["C" + i3].Value = bm[i2].nome;
                ews.Cells["D" + i3].Value = bm[i2].razao;
                ews.Cells["E" + i3].Value = bm[i2].cnpj;
                ews.Cells["F" + i3].Value = bm[i2].inscricao;
                ews.Cells["G" + i3].Value = bm[i2].ccm;
                ews.Cells["H" + i3].Value = bm[i2].endereco;
                ews.Cells["I" + i3].Value = bm[i2].numero;
                ews.Cells["J" + i3].Value = bm[i2].complemento;
                ews.Cells["K" + i3].Value = bm[i2].bairro;
                ews.Cells["L" + i3].Value = bm[i2].cidade;
                ews.Cells["M" + i3].Value = bm[i2].uf;
                ews.Cells["N" + i3].Value = bm[i2].cep;
                ews.Cells["O" + i3].Value = bm[i2].eMail;
                ews.Cells["P" + i3].Value = bm[i2].bancoConta;
                ews.Cells["Q" + i3].Value = bm[i2].agenciaConta;
                ews.Cells["R" + i3].Value = bm[i2].agenciaDigito; 
                ews.Cells["S" + i3].Value = bm[i2].contaBancaria;
                ews.Cells["T" + i3].Value = bm[i2].digitoContaBancaria;
                ews.Cells["U" + i3].Value = bm[i2].unidadeNegocio;
                ews.Cells["V" + i3].Value = bm[i2].tipoEndereco;
                ews.Cells["W" + i3].Value = bm[i2].categoriaPessoa;
                ews.Cells["X" + i3].Value = bm[i2].tipoPessoa;
                ews.Cells["Y" + i3].Value = bm[i2].telefoneFixo;
                ews.Cells["Z" + i3].Value = bm[i2].celular;


                //Aplicando Estilos
                ews.Cells["U" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["V" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["W" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["X" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Change the sheet view to show it in page layout mode
            ews.View.PageLayoutView = true;

            var usuario = Acesso.UsuarioLogado();

            // set some document properties
            ep.Workbook.Properties.Title = "Pessoas";
            ep.Workbook.Properties.Author = usuario;
            ep.Workbook.Properties.Comments = "";

            // set some extended property values
            ep.Workbook.Properties.Company = "SoftFin";

            // set some custom property values
            ep.Workbook.Properties.SetCustomPropertyValue("Checked by", "SoftFin");
            ep.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

            // Cria Memory Stream
            var stream = new MemoryStream(ep.GetAsByteArray());

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }

        public FileStreamResult geraExcelLanctoExtrato(string dataInicial, string dataFinal, int banco)
        {

            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var db = new DbControle();
            var bm = new List<ExcelLanctoExtrato>();

            bm = (from extrato in db.LanctoExtrato
                  join b in db.Bancos on extrato.banco_id equals b.id
                  where b.estabelecimento_id == estab && extrato.data >= DataInicial
                        && extrato.data <= DataFinal && extrato.banco_id == banco
                  select new ExcelLanctoExtrato
                  {
                      data = extrato.data,
                      idLancto = extrato.idLancto,
                      descricao = extrato.descricao,
                      tipo = extrato.Tipo,
                      valor = extrato.Valor,
                      banco = extrato.banco.nomeBanco,
                      agencia = extrato.banco.agencia,
                      conta = extrato.banco.contaCorrente,
                      contaDigito = extrato.banco.contaCorrenteDigito,
                      dataConciliado = extrato.DataConciliado,
                      usuarioConciliado = extrato.UsuConciliado
                  }).ToList();

            // Create a SpreadsheetDocument based on a stream.
            ExcelPackage ep = new ExcelPackage();
            string fileName = "ExtratoOFX.xlsx";
            ExcelWorksheet ews = ep.Workbook.Worksheets.Add(fileName);

            //Título da Planilha
            ews.Cells["B1"].Value = "Extrato OFX";
            ews.Cells["B1"].Style.Font.Size = 22;
            ews.Cells["B1"].Style.Font.Bold = true;

            ews.Cells["B3"].Value = "Data";
            ews.Cells["C3"].Value = "Código Lançamento";
            ews.Cells["D3"].Value = "Descrição";
            ews.Cells["E3"].Value = "Tipo";
            ews.Cells["F3"].Value = "Valor";
            ews.Cells["G3"].Value = "Banco";
            ews.Cells["H3"].Value = "Agência";
            ews.Cells["I3"].Value = "Conta";
            ews.Cells["J3"].Value = "Conta-Dígito";
            ews.Cells["K3"].Value = "Data Conciliação";
            ews.Cells["L3"].Value = "Usuário Conciliador";

            ews.Cells["B3:L3"].Style.Font.Bold = true;
            ews.Cells["B3:L3"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ews.Cells["B3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ews.Cells["B3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            ews.Cells["B3:L3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            for (int i2 = 0; i2 < bm.Count; i2++)
            {
                int i3 = i2 + 4;

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };

                //Cria células
                ews.Cells["B" + i3].Value = bm[i2].data;
                ews.Cells["C" + i3].Value = bm[i2].idLancto;
                ews.Cells["D" + i3].Value = bm[i2].descricao;
                ews.Cells["E" + i3].Value = bm[i2].tipo;
                ews.Cells["F" + i3].Value = bm[i2].valor;
                ews.Cells["G" + i3].Value = bm[i2].banco;
                ews.Cells["H" + i3].Value = bm[i2].agencia;
                ews.Cells["I" + i3].Value = bm[i2].conta;
                ews.Cells["J" + i3].Value = bm[i2].contaDigito;
                ews.Cells["K" + i3].Value = bm[i2].dataConciliado;
                ews.Cells["L" + i3].Value = bm[i2].usuarioConciliado;

                //Aplicando Estilos
                ews.Cells["F" + i3].Style.Numberformat.Format = "0";
                ews.Cells["G" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["H" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["I" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["J" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["L" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            }

            // Change the sheet view to show it in page layout mode
            ews.View.PageLayoutView = true;

            var usuario = Acesso.UsuarioLogado();

            // set some document properties
            ep.Workbook.Properties.Title = "Extrato OFX";
            ep.Workbook.Properties.Author = usuario;
            ep.Workbook.Properties.Comments = "";

            // set some extended property values
            ep.Workbook.Properties.Company = "SoftFin";

            // set some custom property values
            ep.Workbook.Properties.SetCustomPropertyValue("Checked by", "SoftFin");
            ep.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

            // Cria Memory Stream
            var stream = new MemoryStream(ep.GetAsByteArray());

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }




        public FileStreamResult geraExcelRecebimentos(string dataInicial, string dataFinal, int banco)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var db = new DbControle();
           

            var bm = (from extrato in db.Recebimento
                  join b in db.Bancos on extrato.notaFiscal.banco.id equals b.id
                  where b.estabelecimento_id == _estab && extrato.dataRecebimento >= DataInicial
                        && extrato.dataRecebimento <= DataFinal 
                  select new 
                  {
                      dataRecebimento = extrato.dataRecebimento,
                      extrato.valorRecebimento,
                      extrato.historico,
                      extrato.banco.codigoBanco,
                      extrato.banco.agencia,
                      extrato.banco.agenciaDigito,
                      extrato.banco.contaCorrente,
                      extrato.notaFiscal.numeroNfse,
                      extrato.notaFiscal.dataEmissaoNfse,
                      extrato.notaFiscal.valorNfse,
                      extrato.notaFiscal.codigoVerificacao
                      
                  });
            

            // Create a SpreadsheetDocument based on a stream.
            ExcelPackage ep = new ExcelPackage();
            string fileName = "ExtratoRecebimentos.xlsx";
            ExcelWorksheet ews = ep.Workbook.Worksheets.Add(fileName);

            //Título da Planilha
            ews.Cells["B1"].Value = "Recebimentos";
            ews.Cells["B1"].Style.Font.Size = 22;
            ews.Cells["B1"].Style.Font.Bold = true;

            ews.Cells["B3"].Value = "Data";
            ews.Cells["C3"].Value = "Valor Recebimento";
            ews.Cells["D3"].Value = "Histórico";
            ews.Cells["E3"].Value = "Banco";
            ews.Cells["F3"].Value = "Agencia";
            ews.Cells["G3"].Value = "Digito";
            ews.Cells["H3"].Value = "Conta";
            ews.Cells["I3"].Value = "Número Nota";
            ews.Cells["J3"].Value = "Data Emissão";
            ews.Cells["K3"].Value = "Valor Nota";
            ews.Cells["L3"].Value = "Código Verificação";

            ews.Cells["B3:L3"].Style.Font.Bold = true;
            ews.Cells["B3:L3"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ews.Cells["B3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ews.Cells["B3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            ews.Cells["B3:L3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            int i2 = 0;
            int i3 = i2 + 4;
            foreach (var item in bm)
	        {
                

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };

                //Cria células
                ews.Cells["B" + i3].Value = item.dataRecebimento.ToShortDateString();
                ews.Cells["C" + i3].Value = item.valorRecebimento.ToString("n");
                ews.Cells["D" + i3].Value = item.historico;
                ews.Cells["E" + i3].Value = item.codigoBanco;
                ews.Cells["F" + i3].Value = item.agencia;
                ews.Cells["G" + i3].Value = item.agenciaDigito ;
                ews.Cells["H" + i3].Value = item.contaCorrente;
                ews.Cells["I" + i3].Value = item.numeroNfse;
                ews.Cells["J" + i3].Value = item.dataEmissaoNfse;
                ews.Cells["K" + i3].Value = item.valorNfse.ToString("n");
                ews.Cells["L" + i3].Value = item.codigoVerificacao;

                //Aplicando Estilos
                ews.Cells["F" + i3].Style.Numberformat.Format = "0";
                ews.Cells["G" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["H" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["I" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["J" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["L" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                i3 += 1;
	        }
            // Change the sheet view to show it in page layout mode
            ews.View.PageLayoutView = true;


            // set some document properties
            ep.Workbook.Properties.Title = "Recebimentos";
            ep.Workbook.Properties.Author = _usuario;
            ep.Workbook.Properties.Comments = "";

            // set some extended property values
            ep.Workbook.Properties.Company = "SoftFin";

            // set some custom property values
            ep.Workbook.Properties.SetCustomPropertyValue("Checked by", "SoftFin");
            ep.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

            // Cria Memory Stream
            var stream = new MemoryStream(ep.GetAsByteArray());

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }


        public FileStreamResult geraExcelPagamentos(string dataInicial, string dataFinal, int banco)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal).AddHours(23).AddMilliseconds(59).AddSeconds(59);

            var db = new DbControle();


            var cpags = new DocumentoPagarParcela().ObterTodosPorVencimentoBanco(DataInicial, DataFinal, banco, _paramBase);

            var cpagsfiltrado = cpags.Select( p => new { p.DocumentoPagarMestre.banco_id,
                                                         p.DocumentoPagarMestre.numeroDocumento,
                                                         p.DocumentoPagarMestre.Pessoa.nome,
                                                         dataDocumento = p.DocumentoPagarMestre.dataDocumento.ToShortDateString(),
                                                         dataVencimento = p.vencimentoPrevisto.ToShortDateString(),
                                                         valorBruto = p.DocumentoPagarMestre.valorBruto.ToString("n"),
                                                         status = auxsituacao(p.DocumentoPagarMestre.StatusPagamento),
                                                         valorPagamento = new Pagamento().ObterTodosPorDocumentoPagarParcelaId(p.id, _paramBase).Sum(e => e.valorPagamento).ToString("n")

            }).OrderBy(x => x.dataDocumento).ThenBy(x => x.nome).ThenBy(x => x.valorBruto).ToList();



            // Create a SpreadsheetDocument based on a stream.
            ExcelPackage ep = new ExcelPackage();
            string fileName = "ExtratoPagamentos.xlsx";
            ExcelWorksheet ews = ep.Workbook.Worksheets.Add(fileName);

            //Título da Planilha
            ews.Cells["B1"].Value = "Pagamentos";
            ews.Cells["B1"].Style.Font.Size = 22;
            ews.Cells["B1"].Style.Font.Bold = true;

            ews.Cells["B3"].Value = "Documento";
            ews.Cells["C3"].Value = "Favorecido";
            ews.Cells["D3"].Value = "Data Documento";
            ews.Cells["E3"].Value = "Vencimento";
            ews.Cells["F3"].Value = "Valor Bruto";
            ews.Cells["G3"].Value = "Situação";
            ews.Cells["H3"].Value = "Valor Pago";


            ews.Cells["B3:L3"].Style.Font.Bold = true;
            ews.Cells["B3:L3"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ews.Cells["B3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ews.Cells["B3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            ews.Cells["B3:L3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            int i2 = 0;
            int i3 = i2 + 4;

            foreach (var item in cpagsfiltrado)
            {
                

                Row r1 = new Row() { RowIndex = Convert.ToUInt32(i3) };

                //Cria células
                ews.Cells["B" + i3].Value = item.numeroDocumento;
                ews.Cells["C" + i3].Value = item.nome;
                ews.Cells["D" + i3].Value = item.dataDocumento;
                ews.Cells["E" + i3].Value = item.dataVencimento;
                ews.Cells["F" + i3].Value = item.valorBruto;
                ews.Cells["G" + i3].Value = item.status ;
                ews.Cells["H" + i3].Value = item.valorPagamento;


                //Aplicando Estilos
                ews.Cells["F" + i3].Style.Numberformat.Format = "0";
                ews.Cells["G" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["H" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["I" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["J" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ews.Cells["L" + i3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                i3 += 1;
            }
            // Change the sheet view to show it in page layout mode
            ews.View.PageLayoutView = true;


            // set some document properties
            ep.Workbook.Properties.Title = "Pagamentos";
            ep.Workbook.Properties.Author = _usuario;
            ep.Workbook.Properties.Comments = "";

            // set some extended property values
            ep.Workbook.Properties.Company = "SoftFin";

            // set some custom property values
            ep.Workbook.Properties.SetCustomPropertyValue("Checked by", "SoftFin");
            ep.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

            // Cria Memory Stream
            var stream = new MemoryStream(ep.GetAsByteArray());

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/Excel", fileName);
        }

        private string auxsituacao(int StatusPagamento)
        {
            string retauxsituacao = "1 - Em Aberto";

            switch (StatusPagamento)
            {
                case DocumentoPagarMestre.DOCEMABERTO:
                    break;
                case DocumentoPagarMestre.DOCPAGOTOTAL:
                    retauxsituacao = "3 - Pago Total";
                    break;
                case DocumentoPagarMestre.DOCPAGOPARC:
                    retauxsituacao = "2 - Pago Parcialmente";
                    break;
                default:
                    break;
            }
            return retauxsituacao;
        }




    }
}
