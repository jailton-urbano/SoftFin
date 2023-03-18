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
    public class RelatorioBalancoController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }
        public class Dtofake
        {

            public string Ano { get; set; }
            public string Anterior { get; set; }
            public List<DtofakeLinha> Ativo { get; set; }
            public List<DtofakeLinha> Passivo { get; set; }
            public List<DtofakeLinha> Patrimonio { get; set; }

        }

        public class DtofakeLinha
        {
            public string Descricao { get; set; }
            public decimal Anterior { get; set; }
            public decimal Janeiro { get; set; }
            public decimal Fevereiro { get; set; }
            public decimal Marco { get; set; }
            public decimal Abril { get; set; }
            public decimal Maio { get; set; }
            public decimal Junho { get; set; }
            public decimal Julho { get; set; }
            public decimal Agosto { get; set; }
            public decimal Setembro { get; set; }
            public decimal Outubro { get; set; }
            public decimal Novembro { get; set; }
            public decimal Dezembro { get; set; }
            public int Ordem { get; set; }
        }

        public ActionResult ObterRelatorio(int mes, int ano, bool excel)
        {
            var retorno = new List<Dtofake>();

            var dtofake = new Dtofake();
            for (int i = 0; i < 5; i++)
            {
                CriaFake(retorno, dtofake);
            }

            return Json(
                retorno, JsonRequestBehavior.AllowGet);

        }

        private static void CriaFake(List<Dtofake> retorno, Dtofake dtofake)
        {
            dtofake.Ano = "2018";
            dtofake.Anterior = "2017";
            


            dtofake.Ativo = new List<DtofakeLinha>();

            dtofake.Ativo.Add(
                new DtofakeLinha
                {
                    Descricao = "Ativo",
                    Janeiro = 54115.174M,
                    Fevereiro = 54115.1134M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 1
                });

            dtofake.Ativo.Add(
                new DtofakeLinha
                {
                    Descricao = "1.1",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 2
                });

            dtofake.Ativo.Add(
                new DtofakeLinha
                {
                    Descricao = "1.1.1",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 3
                });




            dtofake.Passivo = new List<DtofakeLinha>();

            dtofake.Passivo.Add(
                new DtofakeLinha
                {
                    Descricao = "Passivo",
                    Janeiro = 78954115.14M,
                    Fevereiro = 12354115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 55554115.14M,
                    Ordem = 4
                });

            dtofake.Passivo.Add(
                new DtofakeLinha
                {
                    Descricao = "1.1",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 5
                });

            dtofake.Passivo.Add(
                new DtofakeLinha
                {
                    Descricao = "1.1.1",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 6
                });

            dtofake.Patrimonio = new List<DtofakeLinha>();

            dtofake.Patrimonio.Add(
                new DtofakeLinha
                {
                    Descricao = "Patrimonio",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 7
                });

            dtofake.Patrimonio.Add(
                new DtofakeLinha
                {
                    Descricao = "1.1",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 8
                });

            dtofake.Patrimonio.Add(
                new DtofakeLinha
                {
                    Descricao = "1.1.1",
                    Janeiro = 54115.14M,
                    Fevereiro = 54115.14M,
                    Marco = 54115.14M,
                    Maio = 54115.14M,
                    Junho = 54115.14M,
                    Julho = 54115.14M,
                    Agosto = 54115.14M,
                    Setembro = 54115.14M,
                    Abril = 54115.14M,
                    Outubro = 54115.14M,
                    Novembro = 54115.14M,
                    Dezembro = 54115.14M,
                    Anterior = 54115.14M,
                    Ordem = 9
                });

            retorno.Add(dtofake);
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