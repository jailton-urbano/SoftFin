using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SoftFin.Utils;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioDiarioContabilController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public class DtoData
        {

            public string Dia { get; set; }
            public Decimal TotalD { get; set; }
            public Decimal TotalC { get; set; }
            public List<DtoDataLinha> Linha { get; set; }
        }

        public class DtoDataLinha
        {
            public string Conta_A { get; set; }
            public string Conta_B { get; set; }
            
            public string CC { get; set; }
            public string Historico { get; set; }
            public string NL { get; set; }
            public string CCC_A { get; set; }
            public string CCC_B { get; set; }

            public Decimal Credito { get; set; }
            public Decimal Debito { get; set; }

        }

        public ActionResult ObterRelatorio(int mes, int ano, bool excel)
        {
            var retorno = new List<DtoData>();

            var dataInicial = new DateTime(ano, mes, 1);
            var dataFinal = new DateTime(ano, mes, 1);

            dataFinal = dataFinal.AddMonths(1).AddDays(-1);

            var lcs = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase, dataInicial, dataFinal);

            for (DateTime i = dataInicial; i <= dataFinal; i = i.AddDays(1))
            {
                var dataIni = i;
                var dataFim = i.AddDays(1).AddMinutes(-1);
                var lcsAux = lcs.Where(p => p.data >= dataIni && p.data <= dataFim).ToList();

                var dtoData = new DtoData();
                dtoData.Dia = i.ToString("o");
                dtoData.TotalC = lcsAux.
                    Sum(p => p.LancamentoContabilDetalhes.Where(x => x.DebitoCredito == "C").Sum(v => v.valor));
                dtoData.TotalD = lcsAux.
                    Sum(p => p.LancamentoContabilDetalhes.Where(x => x.DebitoCredito == "D").Sum(v => v.valor));

                dtoData.Linha = new List<DtoDataLinha>();
                foreach (var item in lcsAux)
                {
                    var dtoDataLinha = new DtoDataLinha();
                    var detDebito = item.
                        LancamentoContabilDetalhes.
                        Where(p => p.DebitoCredito == "D");
                    var detCredito = item.
                        LancamentoContabilDetalhes.
                        Where(p => p.DebitoCredito == "C");

                    dtoDataLinha.Credito = detDebito.Sum(v => v.valor);
                    dtoDataLinha.Debito = detDebito.Sum(v => v.valor);
                    dtoDataLinha.Historico = item.historico;
                    dtoDataLinha.NL = item.codigoLancamento.ToString();

                    dtoDataLinha.Conta_A = detDebito.
                        First().ContaContabil.codigo
                        + " - " + detDebito.
                        First().ContaContabil.descricao;

                    dtoDataLinha.Conta_B = detCredito.
                        First().ContaContabil.codigo
                        + " - " + detCredito.
                        First().ContaContabil.descricao;

                    dtoDataLinha.CCC_B = detDebito.
                        First().ContaContabil.codigo
                        ;

                    dtoDataLinha.CCC_A = detCredito.
                        First().ContaContabil.codigo
                        ;
                    dtoData.Linha.Add(dtoDataLinha);
                }
                retorno.Add(dtoData);
            }


            return Json(
                retorno, JsonRequestBehavior.AllowGet);

        }

        private static void CriaFake(List<DtoData> retorno, DtoData dtofake)
        {
            dtofake.Dia = DateTime.Now.ToString();
            dtofake.TotalC = decimal.Parse("20000,00");
            dtofake.TotalD = decimal.Parse("20000,00");

            dtofake.Linha = new List<DtoDataLinha>();

            for (int i = 0; i < 8; i++)
            {
                dtofake.Linha.Add(
                    new DtoDataLinha
                    {
                        Conta_A = "Banco Itau",
                        Conta_B = "Tarifas",
                        CC = "76768",
                        Historico = "ksndjlnasjndajndsjnadkjdnakjdsnjansdka",
                        NL = "898",
                        CCC_A = "8787822",
                        CCC_B = "9867677",
                        Debito = 982822,
                        Credito = 22121
                    });


            }

            retorno.Add(dtofake);
        }

  

    }
}