using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ProcessoHistoricoController : BaseController
    {
        // GET: ProcessoAndamento
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Obter(string codigoProcesso)
        {
            ParamProcesso paramProcesso = new ParamProcesso();
            paramProcesso.CodigoProcesso = codigoProcesso;
            paramProcesso.CodigoEmpresa = _codigoEmpresa;
            paramProcesso.CodigoUsuario = _usuario;
            var retorno = new CallApi.GestorProcessos().ObterAtividadeExecucaoHistorico(paramProcesso);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObterProcessos()
        {
            var param = new GestorProcessos.Comum.Param.ParamProcesso();
            param.CodigoEmpresa = _estabobj.Empresa.codigo;
            param.CodigoUsuario = _usuario;

            var retorno = new CallApi.GestorProcessos().ObterProcessos(param);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Detalhar(string codigoProcessoAtual)
        {
            var param = new GestorProcessos.Comum.Param.ParamProcesso();
            param.CodigoEmpresa = _estabobj.Empresa.codigo;

            var retorno = new CallApi.GestorProcessos().DetalharProcesso(param, codigoProcessoAtual);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetUrlProcesso()
        {
            var urlSPG = ConfigurationManager.AppSettings["urlSPG"].ToString();
            return Json(urlSPG, JsonRequestBehavior.AllowGet);
        }


    }
}