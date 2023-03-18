using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class TCController : BaseController
    {
        // GET: TD
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ObterTodos()
        {
            
            var objs = new TipoContrato().ObterTodos(_paramBase);

            return Json(objs.Select(p => new
            {
                p.id,
                p.empresa_id,
                p.tipo
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new TipoContrato().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new TipoContrato();
                //obj.empresa_id = _empresa ;
                obj.empresa_id = _empresa;
            }

            return Json(new
            {
                obj.id,
                obj.tipo,
                obj.empresa_id
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(TipoContrato obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.tipo == "")
                {
                    objErros.Add("Informe o Tipo de Contrato");
                }


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }





                if (obj.id == 0)
                {

                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(TipoContrato obj)
        {

            try
            {
                var erro = "";
                if (new TipoContrato().Excluir(obj.id,ref erro, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}