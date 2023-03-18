using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SoftFin.Utils;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioRecebimentoCarteiraController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ObterRelatorio(int mes, int ano, bool excel)
        {
            
            var dataInicial = new DateTime(ano, mes, 1 );
            var dataFinal = new DateTime( ano, mes, 1 );
            Decimal totalCarteira = 0;
            Decimal totalRealizado = 0;
            Decimal totalAberto = 0;

            var lista = new List<DtoData>();

            dataFinal = dataFinal.AddMonths(1).AddDays(-1);

            var parcelaContratos = new ParcelaContrato().
                ObterEntreDataEmSituacaoEmitida(dataInicial, dataFinal, _paramBase).
                OrderBy(p => p.ContratoItem.Contrato.Pessoa.nome);

            foreach (var item in parcelaContratos)
            {
                var novo = new DtoData();
                var ov = new OrdemVenda().ObterPorParcelaId(item.id, _estab, null);

                novo.Cliente = item.ContratoItem.Contrato.Pessoa.nome;
                novo.TipoServico = item.ContratoItem.TipoContrato.tipo;
                novo.CarteiraValor = item.valor;
                novo.CarteiraVencimento = (item.DataVencimento != null) ? item.DataVencimento.Value.Day.ToString() : "";
                novo.ValoresAberto = item.valor.ToString("n") ;

                totalCarteira += novo.CarteiraValor;
                
                


                if (ov != null)
                {
                    var nf = new NotaFiscal().ObterPorOV(ov.id);
                    if (nf != null)
                    {
                        var recs = new Recebimento().ObterTodosPorNFSeId(nf.id,_paramBase);
                        if (recs.Count() > 0)
                        {

                            novo.RealizadoValor = recs.Sum( p => p.valorRecebimento);
                            novo.RealizadoData = recs.First().dataRecebimento.ToString("o");
                            novo.RealizadoBanco = recs.First().banco.nomeBanco;
                            novo.ValoresAberto = "OK";
                            totalRealizado += novo.RealizadoValor;

                        }
                        else
                        {
                            totalAberto += item.valor;
                        }
                    }
                }

                lista.Add(novo);
            }

            if (!excel)
            {
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet = workbook.CreateSheet(ano + " " + mes);

                int rowNumer = 0;

                IRow row = sheet.CreateRow(rowNumer);
                ICell cell;

                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.Alignment = HorizontalAlignment.Right;

                cell = row.CreateCell(0);
                cell.SetCellValue("Clientes");
                cell.CellStyle = style;


                cell = row.CreateCell(1);
                cell.SetCellValue("Tipo de Serviço");
                cell.CellStyle = style;


                cell = row.CreateCell(2);
                cell.SetCellValue("Carteira");
                cell.CellStyle = style;

                cell = row.CreateCell(3);
                cell.SetCellValue("");
                cell.CellStyle = style;

                cell = row.CreateCell(4);
                cell.SetCellValue("Realizado");
                cell.CellStyle = style;


                cell = row.CreateCell(5);
                cell.SetCellValue("");
                cell.CellStyle = style;


                cell = row.CreateCell(6);
                cell.SetCellValue("");
                cell.CellStyle = style;

                cell = row.CreateCell(7);
                cell.SetCellValue("");
                cell.CellStyle = style;

                rowNumer += 1;

                row = sheet.CreateRow(rowNumer);

                cell = row.CreateCell(0);
                cell.SetCellValue("");
                cell.CellStyle = style;


                cell = row.CreateCell(1);
                cell.SetCellValue("");
                cell.CellStyle = style;


                cell = row.CreateCell(2);
                cell.SetCellValue("R$");
                cell.CellStyle = style;

                cell = row.CreateCell(3);
                cell.SetCellValue("Vencimento");
                cell.CellStyle = style;

                cell = row.CreateCell(4);
                cell.SetCellValue("R$");
                cell.CellStyle = style;


                cell = row.CreateCell(5);
                cell.SetCellValue("Data");
                cell.CellStyle = style;


                cell = row.CreateCell(6);
                cell.SetCellValue("Banco");
                cell.CellStyle = style;

                cell = row.CreateCell(7);
                cell.SetCellValue("Aberto");
                cell.CellStyle = style;




                foreach (var item in lista)
                {

                    rowNumer++;
                    row = sheet.CreateRow(rowNumer);
                    int f = 0;
                    row.CreateCell(f).SetCellValue(item.Cliente);
                    f += 1;
                    row.CreateCell(f).SetCellValue(item.TipoServico);
                    f += 1;
                    row.CreateCell(f).SetCellValue(double.Parse(item.CarteiraValor.ToString("n")));
                    f += 1;
                    row.CreateCell(f).SetCellValue(item.CarteiraVencimento);
                    f += 1;
                    row.CreateCell(f).SetCellValue(double.Parse(item.RealizadoValor.ToString("n")));
                    f += 1;
                    row.CreateCell(f).SetCellValue(
                        item.RealizadoData == null ? "" : DateTime.Parse(item.RealizadoData).ToString("dd/MM/yyyy")
                        );
                    f += 1;
                    row.CreateCell(f).SetCellValue(item.RealizadoBanco);
                    f += 1;
                    row.CreateCell(f).SetCellValue(item.ValoresAberto);
                }


                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                int a = 0;
                row.CreateCell(a).SetCellValue("Total");
                a += 1;
                row.CreateCell(a).SetCellValue("");
                a += 1;
                row.CreateCell(a).SetCellValue(totalCarteira.ToString("n"));
                a += 1;
                row.CreateCell(a).SetCellValue("");
                a += 1;
                row.CreateCell(a).SetCellValue(totalRealizado.ToString("n"));
                a += 1;
                row.CreateCell(a).SetCellValue("");
                a += 1;
                row.CreateCell(a).SetCellValue("");
                a += 1;
                row.CreateCell(a).SetCellValue(totalAberto.ToString("n"));

                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);

                return File(stream.ToArray(), //The binary data of the XLS file
                    "application/vnd.ms-excel", //MIME type of Excel files
                    string.Format("RecebimentoCarteira_{0}_{1}.xlsx", ano + "_" + mes + "_", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));


            }

        }

        public class DtoData
        {
            public string Cliente { get; set; }

            public string TipoServico { get; set; }

            public decimal CarteiraValor { get; set; }
            public string CarteiraVencimento { get; set; }


            public decimal RealizadoValor { get; set; }
            public string RealizadoData { get; set; }

            public string RealizadoBanco { get; set; }

            public string ValoresAberto { get; set; }

        }

    }
}