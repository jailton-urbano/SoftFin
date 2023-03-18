using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioListaPessoasController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["TipoPessoa"] = new SelectList(new TipoPessoa().ObterTodos(_paramBase), "id", "Descricao");
            return View();
        }



        public JsonResult GetReport(int idTipoPessoa)
        {

            var objs = new Pessoa().ObterTodosPorTipoPessoa(idTipoPessoa, _paramBase);


            return Json(objs.Select(p=> new {
                            p.nome, 
                            p.eMail,
                            p.UnidadeNegocio.unidade, 
                            categoria = p.CategoriaPessoa.Descricao}), JsonRequestBehavior.AllowGet);
        }

    }
}
