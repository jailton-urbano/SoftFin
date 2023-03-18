using SoftFin.GestorProcessos.CallApi.MyLIMS;
using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.Controllers
{
    public class MyLIMSController : Controller
    {
        private DBGPControle db = new DBGPControle();
        // GET: MyLIMS
        public JsonResult CarregarGrid()
        {
            var callapi = new GestorProcessosMyLIMS();
            var parametro = db.EmpresaParametro.Where(p => p.Empresa.Id == 2 && p.Codigo == "UrlConInt").First();
            var dados = callapi.CarregarGrid(parametro.Valor);
            return Json(dados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExecutaIntegracaoPorContrato(string contrato)
        {
            var callapi = new GestorProcessosMyLIMS();
            var parametro = db.EmpresaParametro.Where(p => p.Empresa.Id == 2 && p.Codigo == "UrlIntPorCnt").First();
            var dados = callapi.ExecutaIntegracao(parametro.Valor, contrato);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}