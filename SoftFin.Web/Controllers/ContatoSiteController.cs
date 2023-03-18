using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Web.Regras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ContatoSiteController : Controller
    {

        [HttpPost]
        public ActionResult Create(ContatoSite obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.Incluir(obj) == true)
                    {
                        ViewBag.msg = "Obrigado por entrar em contato";
                    }
                    else
                    {
                        ViewBag.msg = "Site em manutenção";
                    }
                    return View(obj);
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);
                }
            }
            catch 
            {
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Create()
        {
            var obj = new ContatoSite();
            obj.Data = DateTime.Now;
            return View(obj);
        }

    }
}
