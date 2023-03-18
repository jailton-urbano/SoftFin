using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class PesquisarController : BaseController
    {
        //
        // GET: /Pesquisar/

        public ActionResult Index()
        {
            if (Request.Form["txtPesquisar"] == null)
                ViewData["txtPesquisar"] = "";
            else
                ViewData["txtPesquisar"] = Request.Form["txtPesquisar"];
            return View();
        }

        [HttpPost]
        public JsonResult Pesquisar(String texto)
        {
            var dbControle = new DbControle();
            
            return Json( Acesso.RetornaFuncionalidades(texto),JsonRequestBehavior.AllowGet);
        }

    }
}
