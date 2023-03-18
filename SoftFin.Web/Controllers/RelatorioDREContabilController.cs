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
    public class RelatorioDREContabilController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ObterRelatorio(int mes, int ano, bool excel)
        {
            var relatorioRec = new List<DtoData>();
            var relatorioPag = new List<DtoData>();

            var dataInicial = new DateTime(ano, mes, 1 );
            var dataFinal = new DateTime( ano, mes, 1 );

            dataFinal = dataFinal.AddMonths(1).AddDays(-1);

            var totalRecebimento = new Dictionary<String, Decimal>();
            var totalPagamento = new Dictionary<String, Decimal>();
            var totalCaixa = new Dictionary<String, Decimal>();
            var totalCaixaInicial = new Dictionary<String, Decimal>();
            var totalCaixaFinal = new Dictionary<String, Decimal>();

            Decimal totaltotalRecebimento = 0;
            Decimal totaltotalPagamento = 0;
            Decimal totaltotalCaixa = 0;
            Decimal totaltotalCaixaInicial = 0;
            Decimal totaltotalCaixaFinal = 0;

            var recebimentos = new Recebimento().ObterEntreData(dataInicial, dataFinal, _paramBase);
            var pagamentos = new Pagamento().ObterEntreData(dataInicial, dataFinal, _paramBase);

            //Extrai Pessoas
           
            foreach (var item in recebimentos)
            {
                if (relatorioRec.Where(p => p.CPF ==
                        item.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf).Count() == 0)
                {
                    relatorioRec.Add(new DtoData
                    {
                        CPF = item.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf,
                        Nome = item.notaFiscal.NotaFiscalPessoaTomador.razao
                    }
                    );
                }
            }

            foreach (var item in pagamentos)
            {
                if (relatorioPag.Where(p => p.CPF ==
                        item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.cnpj).Count() == 0)
                {
                    relatorioPag.Add(new DtoData
                    {
                        CPF = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.cnpj,
                        Nome = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome, 
                        TotalRecebimento = 0,
                        TotalPagamento = 0
                    }
                    );
                }
            }

            
            for (int dia = 1; dia <= dataFinal.Day; dia++)
            {
                decimal total = 0;
                foreach (var item in relatorioRec)
                {
                    Decimal recsoma = recebimentos.Where(p => p.dataRecebimento.Day == dia
                            && p.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf == item.CPF).Sum(p => p.valorRecebimento);

                    relatorioRec.Where(p => p.CPF == item.CPF).First().TotalRecebimento += recsoma;

                    item.Recebimentos.Add(dia, recsoma);
                    total += recsoma;
                }
                totalRecebimento.Add(dia.ToString(), total);
                var diaTxt = dia.ToString();
                if (totalCaixa.Where(p => p.Key == diaTxt).Count() == 0)
                {
                    totalCaixa.Add(diaTxt, total);
                }
                else
                {
                    totalCaixa[diaTxt] += total;
                }
                totaltotalRecebimento += total;
                totaltotalCaixa += total;
            }

            decimal saldoAnterior = 0;

            var sbr = new SaldoBancarioReal().ObterData(dataInicial.AddDays(-1),_paramBase);

            if (sbr.Count > 0)
                saldoAnterior = sbr.Sum(p => p.saldoFinal);



            for (int dia = 1; dia <= dataFinal.Day; dia++)
            {
                decimal total = 0;
                foreach (var item in relatorioPag)
                {
                    Decimal recsoma = pagamentos.Where(p => p.dataPagamento.Day == dia
                            && p.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.cnpj == item.CPF).Sum(p => p.valorPagamento);

                    relatorioPag.Where(p => p.CPF == item.CPF).First().TotalPagamento += recsoma;

                    item.Pagamentos.Add(dia, recsoma);
                    total += recsoma;
                }
                totalPagamento.Add(dia.ToString(), total);
                var diaTxt = dia.ToString();

                totalCaixa[diaTxt] -= total;
                
                totaltotalPagamento += total;
                totaltotalCaixa -= total;

                totalCaixaInicial.Add(dia.ToString(), saldoAnterior);
                saldoAnterior = saldoAnterior + totalCaixa[diaTxt];
                totalCaixaFinal.Add(dia.ToString(), saldoAnterior);
            }


            totaltotalCaixaInicial = totalCaixaInicial.Sum(p => p.Value);
            totaltotalCaixaFinal = totalCaixaFinal.Sum(p => p.Value);

            if (!excel)
            {
                var retorno = new {
                    relatorioRec = relatorioRec.OrderBy(z => z.Nome).Select(p => new { p.Nome, p.TotalRecebimento,
                        Recebimentos = p.Recebimentos.Select(x => new { Key = x.Key.ToString(), x.Value })
                    }),
                    totalRecebimento,
                    totaltotalRecebimento,
                    relatorioPag = relatorioPag.OrderBy(z => z.Nome).Select(p => new { p.Nome, p.TotalPagamento,
                        Pagamentos = p.Pagamentos.Select(x => new { Key = x.Key.ToString(), x.Value } ) }),
                    totalPagamento,
                    totaltotalPagamento,
                    totalCaixa,
                    totalCaixaInicial,
                    totalCaixaFinal,
                    totaltotalCaixa,
                    totaltotalCaixaInicial,
                    totaltotalCaixaFinal
                };
                return Json(retorno, JsonRequestBehavior.AllowGet);
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
                cell.SetCellValue("Recebimentos");
                cell.CellStyle = style;


                for (int dia = 1; dia <= dataFinal.Day; dia++)
                {
                    cell = row.CreateCell(dia);
                    cell.SetCellValue(dia);
                    cell.CellStyle = style;
                }

                cell = row.CreateCell(dataFinal.Day + 1);
                cell.SetCellValue("Total");
                cell.CellStyle = style;
                foreach (var item in relatorioRec.OrderBy(z => z.Nome))
                {
                    rowNumer++;
                    row = sheet.CreateRow(rowNumer);
                    row.CreateCell(0).SetCellValue(item.Nome);


                    int a = 0;
                    foreach (var item2 in item.Recebimentos)
                    {
                        a += 1;
                        row.CreateCell(a).SetCellValue(double.Parse(item2.Value.ToString("n")));
                    }
                    a += 1;
                    row.CreateCell(a).SetCellValue(double.Parse(item.TotalRecebimento.ToString("n")));
                }
                //Total Recebimentos
                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                row.CreateCell(0).SetCellValue("Total");
                int b = 0;
                foreach (var item in totalRecebimento)
                {
                    b += 1;
                    row.CreateCell(b).SetCellValue(double.Parse(item.Value.ToString("n")));
                }

                b += 1;
                row.CreateCell(b).SetCellValue(double.Parse(totalRecebimento.Sum(p => p.Value).ToString("n")));

                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                cell = row.CreateCell(0);
                cell.SetCellValue("Pagamentos");
                cell.CellStyle = style;


                for (int dia = 1; dia <= dataFinal.Day; dia++)
                {
                    cell = row.CreateCell(dia);
                    cell.SetCellValue(dia);
                    cell.CellStyle = style;

                }

                cell = row.CreateCell(dataFinal.Day + 1);
                cell.SetCellValue("Total");
                cell.CellStyle = style;

                foreach (var item in relatorioPag.OrderBy(z => z.Nome))
                {
                    rowNumer++;
                    row = sheet.CreateRow(rowNumer);
                    row.CreateCell(0).SetCellValue(item.Nome);
                    

                    int a = 0;
                    foreach (var item2 in item.Pagamentos)
                    {
                        a += 1;
                        row.CreateCell(a).SetCellValue(double.Parse(item2.Value.ToString("n")));
                        
                    }
                    a += 1;
                    row.CreateCell(a).SetCellValue(double.Parse(item.TotalPagamento.ToString("n")));
                }

                //Total Pagamento
                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                row.CreateCell(0).SetCellValue("Total");
                int c = 0;
                foreach (var item in totalPagamento)
                {
                    c += 1;
                    row.CreateCell(c).SetCellValue(double.Parse(item.Value.ToString("n")));
                }

                c += 1;
                row.CreateCell(c).SetCellValue(double.Parse(totalPagamento.Sum(p => p.Value).ToString("n")));

                //Total Caixa
                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                row.CreateCell(0).SetCellValue("Caixa");
                int d = 0;
                foreach (var item in totalCaixa)
                {
                    d += 1;
                    row.CreateCell(d).SetCellValue(double.Parse(item.Value.ToString("n")));
                }

                d += 1;
                row.CreateCell(d).SetCellValue(double.Parse(totalCaixa.Sum(p => p.Value).ToString("n")));


                //Total Inicial

                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                row.CreateCell(0).SetCellValue("Caixa Inicial");
                int e = 0;
                foreach (var item in totalCaixaInicial)
                {
                    e += 1;
                    row.CreateCell(e).SetCellValue(double.Parse(item.Value.ToString("n")));
                }

                e += 1;
                row.CreateCell(e).SetCellValue(double.Parse(totalCaixaInicial.Sum(p => p.Value).ToString("n")));

                //Total Final

                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                row.CreateCell(0).SetCellValue("Caixa Final");
                int f = 0;
                foreach (var item in totalCaixaFinal)
                {
                    f += 1;
                    row.CreateCell(f).SetCellValue(double.Parse(item.Value.ToString("n")));
                }

                f += 1;
                row.CreateCell(f).SetCellValue(double.Parse(totalCaixaFinal.Sum(p => p.Value).ToString("n")));

                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);

                return File(stream.ToArray(), //The binary data of the XLS file
                    "application/vnd.ms-excel", //MIME type of Excel files
                    string.Format("FluxoCaixa_{0}_{1}.xlsx", ano + "_" + mes + "_", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
            }

        }

        public class DtoData
        {
            public DtoData()
            {
                Recebimentos = new Dictionary<int, decimal>();
                Pagamentos = new Dictionary<int, decimal>();
            }

            public string Nome { get; set; }

            public string CPF { get; set; }
            public Dictionary<int, decimal> Recebimentos { get; set; }

            public Dictionary<int, decimal> Pagamentos { get; set; }
            public Decimal TotalRecebimento { get; set; }
            public Decimal TotalPagamento { get; set; }
        }

    }
}