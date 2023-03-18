using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class TPController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}