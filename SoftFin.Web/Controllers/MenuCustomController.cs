using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class MenuCustomController : BaseController
    {
        // GET: MenuCustom
        public ActionResult Index()
        {
            var param = new GestorProcessos.Comum.Param.ParamProcesso();
            param.CodigoEmpresa = _estabobj.Empresa.codigo;
            param.CodigoUsuario = _usuario;

            var retorno = new CallApi.GestorProcessos().ObterProcessosIniciaisPorUsuario(param);

            ViewData["Processos"] = retorno;

            return View();
        }
    }
}