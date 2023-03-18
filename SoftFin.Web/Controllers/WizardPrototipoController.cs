using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class WizardPrototipoController : BaseController
    {
        // GET: WizardPrototipo
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ObterTodos()
        {
            DbControle db = new DbControle();

            var objs = new ContaContabil().ObterTodos(_paramBase, db);

            var objTratado = objs.Select(obj => new
            {
                obj.empresa_id,
                contaContabil_id = obj.id,
                ccds = string.Format("{0} - {1}", obj.codigo, obj.descricao),
                obj.codigo,
                obj.Tipo,
                obj.codigoSuperior
            }).OrderBy(p => p.codigo) ;

            var returnAux = new List<DtoWizard>();
            

            foreach (var item in objTratado)
            {

                var x = new DtoWizard();

                x.empresa_id = item.empresa_id;
                x.contaContabil_id = item.contaContabil_id;
                x.ccds = item.ccds;
                x.Codigo = item.codigo;
                x.codigoSuperior = item.codigoSuperior;
                x.Tipo = null;
                if (string.IsNullOrWhiteSpace(item.codigoSuperior))
                {
                    x.Tipo = "G";
                }
                else
                {
                    if (objTratado.Where(p => p.codigoSuperior == item.codigo).Count() == 0)
                    {
                        x.Tipo = "A";
                    }
                }
                returnAux.Add(x);
            }


            foreach (var item in returnAux.Where(p => p.Tipo == null))
            {
                if (returnAux.Where(p => p.Codigo == item.codigoSuperior && p.Tipo == "G").Count() == 0)
                {
                    item.Tipo = "P";
                }
                else
                {

                    item.Tipo = "S";
                }
            }

            return Json(returnAux, JsonRequestBehavior.AllowGet);
        }   

        private class DtoWizard
        {
            public int empresa_id { get; set; }
            public int contaContabil_id { get; set; }
            public string ccds { get; set; }
            public string Codigo { get; set; }
            public string Tipo { get; set; }

            public string codigoSuperior { get; set; }
        }
    }
}