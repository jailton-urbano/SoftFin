using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioSeguroVencimentoController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["DataInicial"] = DateTime.Now.Date.ToShortDateString();
            ViewData["DataFinal"] = DateTime.Now.AddMonths(+1).ToShortDateString();
            return View();
        }



        public JsonResult GetReport(string DataInicial, string DataFinal)
        {

            var objs = new PropostaApolice().ObterTodosVencimento(DateTime.Parse(DataInicial), DateTime.Parse(DataFinal), _estab);


            return Json(objs.Select(p=> new {
                            nome = p.Segurado_Principal.nome,
                            email = p.Segurado_Principal.eMail,
                            telefone = p.Segurado_Principal.TelefoneFixo,
                            celular = p.Segurado_Principal.Celular,
                            ini = p.dataVigenciaInicio.ToShortDateString(),
                            fim = p.dataVigenciaFim.ToShortDateString(),
                            apo = p.numeroApolice
                            
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
