using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SDBController : BaseController
    {
        // GET: SDD
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {

            var objs = new SistemaDashBoard().ObterTodos();


            return Json(objs.Select(p => new
            {
                p.id,
                p.ativo,
                p.Codigo,
                p.Descricao,
                p.icone,
                p.sistemaMenu_id,
                SistemaMenu_Descricao = p.SistemaMenu.Descricao


            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new SistemaDashBoard().ObterPorId(id);
            if (obj == null)
            {
                obj = new SistemaDashBoard();
                //obj.empresa_id = _empresa ;
            }

            return Json(new
            {
                obj.id,
                obj.ativo,
                obj.Codigo,
                obj.Descricao,
                obj.icone,
                obj.sistemaMenu_id,
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(SistemaDashBoard obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (string.IsNullOrEmpty(obj.Descricao))
                {
                    objErros.Add("Informe a descrição");
                }

                if (string.IsNullOrEmpty(obj.Descricao))
                {
                    objErros.Add("Informe o código");
                }

                if (obj.sistemaMenu_id == 0)
                {
                    objErros.Add("Informe sistema menu");
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


        public JsonResult Excluir(SistemaDashBoard obj)
        {

            try
            {
                if (new SistemaDashBoard().Excluir(obj.id, _paramBase))
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

        public JsonResult ObterSistemaMenu()
        {
            //Funcionalidade_id
            var objs = new SistemaMenu().ObterTodos().Where(p => p.ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}