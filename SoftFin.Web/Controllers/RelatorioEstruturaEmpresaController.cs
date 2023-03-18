using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioEstruturaEmpresaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

                
        public JsonResult GetReport()
        {
            var rpt = new Estabelecimento().ObterTodos(_paramBase);

            var x = rpt.Select(p => new { codigoHolding = p.Empresa.Holding.codigo,
                                          NomeHolding = p.Empresa.Holding.nome,
                                          CodigoEmpresa = p.Empresa.codigo,
                                          NomeEmpresa = p.Empresa.nome,
                                          CodigoEstabelecimento = p.Codigo,
                                          NomeEstabelecimento = p.NomeCompleto
            });

            return Json(x, JsonRequestBehavior.AllowGet);
        }


    }
}
