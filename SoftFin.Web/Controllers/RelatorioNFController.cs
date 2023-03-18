using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioNFController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetReport(string dataInicial, string dataFinal)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var objs = new NotaFiscal().ObterEntreData(DataInicial, DataFinal, _paramBase);


            return Json(objs.Select(p => new
            {
                p.Estabelecimento.Apelido,
                p.numeroNfse,
                Ano = p.dataEmissaoNfse.Year,
                Mes = p.dataEmissaoNfse.Month,
                dataEmissaoNfse = p.dataEmissaoNfse.ToShortDateString(),
                p.NotaFiscalPessoaTomador.razao,
                p.valorNfse,
                p.valorISS,
                p.pisRetido,
                p.cofinsRetida,
                p.csllRetida,
                p.valorLiquido
            }).OrderBy(p =>p.numeroNfse), JsonRequestBehavior.AllowGet);
        }

    }
}
