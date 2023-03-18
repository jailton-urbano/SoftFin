using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioController : BaseController
    {
        //
        // GET: /Relatorio/

        public ActionResult Index()
        {
            ViewData["descricao"] = Request.QueryString["descricao"].ToString();

            return View();
        }

    }
}
