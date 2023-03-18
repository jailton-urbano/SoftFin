


using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioGraficoFaturamentoUnidadeController : BaseController
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

            var objs = new NotaFiscal().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p=> p.OrdemVenda != null ).ToList();

            var unidades = new UnidadeNegocio().ObterTodos(_paramBase).Select(p => new { p.unidade }).Distinct().ToList();

            var lista = new List<List<object>>();

            for (DateTime idata = DataInicial; idata <= DataFinal; idata = idata.AddMonths(1))
			{
                var listaaux = new List<object>();


                listaaux.Add(idata.Month.ToString() + "/" + idata.Year.ToString() );
 
			    foreach (var item in unidades)
	            {
                    decimal soma = 0;
		            var soma2 = objs.Where(p => p.dataVencimentoNfse.Year == idata.Year && p.dataVencimentoNfse.Month == idata.Month && p.OrdemVenda.UnidadeNegocio.unidade == item.unidade);
                    if (soma2.Count() != 0)
                        soma = soma2.Sum(p => p.valorNfse);
                                        
                    listaaux.Add( soma);
                }
                lista.Add(listaaux);
			}



            return Json(new { unidades = unidades , objs = lista }, JsonRequestBehavior.AllowGet);
        }

    }
}
