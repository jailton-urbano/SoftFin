using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LayoutTemplateController : BaseController
    {
        // GET: LayoutTemplate
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {

            var objs = new LayoutTemplate().ObterTodos(_paramBase);

            
            return Json(objs.Select(p => new
            {
                p.Id,
                p.Template,
                p.Codigo,
                p.Empresa_id

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new LayoutTemplate().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new LayoutTemplate();
                //obj.empresa_id = _empresa ;
                obj.Empresa_id = _empresa;
            }

            return Json(new
            {
                obj.Id,
                obj.Template,
                obj.Codigo,
                obj.Empresa_id
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(LayoutTemplate obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.Codigo == "")
                {
                    objErros.Add("Informe o Código");
                }

                if (obj.Template == "")
                {
                    objErros.Add("Informe o template");
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
                if (obj.Empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);





                if (obj.Id == 0)
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


        public JsonResult Excluir(LayoutTemplate obj)
        {

            try
            {
                if (obj.Empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                var erro = "";

                if (new LayoutTemplate().Excluir(obj.Id, ref erro, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluido com sucesso" }, JsonRequestBehavior.AllowGet);
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