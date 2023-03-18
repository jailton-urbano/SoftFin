using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioUsuariosController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetReport()
        {

            var objs = new Usuario().ObterTodosUsuariosAtivos(_paramBase) ;


            return Json(objs.Select(p=> new {
                            empcod = p.Empresa.codigo, 
                            empnom = p.Empresa.nome,
                            p.codigo,
                            p.nome,
                            p.Perfil.Descricao
            }).OrderBy(p =>p.nome), JsonRequestBehavior.AllowGet);
        }

    }
}
