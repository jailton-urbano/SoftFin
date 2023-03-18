using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SMController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {

            var objs = new SistemaMenu().ObterTodos();
                      

            return Json(objs.Select(p => new
            {
                p.id,
                p.Descricao,
                p.ativo,
                p.Codigo,
                p.icone
              
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new SistemaMenu().ObterAtivoPorId(id);
            if (obj == null)
            {
                obj = new SistemaMenu();
                //obj.empresa_id = _empresa ;
            }

            return Json(new
            {
                obj.id,
                obj.Descricao,
                obj.ativo,
                obj.Codigo,
                obj.icone
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(SistemaMenu obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if ( string.IsNullOrEmpty(obj.Descricao))
                {
                    objErros.Add("Informe a descrição");
                }

                if (string.IsNullOrEmpty(obj.Descricao))
                {
                    objErros.Add("Informe o código");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        Errors = objErros,
                        DSMessage = "Verifique os dados informados"
                    });
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

                    obj.Alterar(_paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(SistemaMenu obj)
        {

            try
            {
                if (new SistemaMenu().Excluir(obj.id, _paramBase))
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