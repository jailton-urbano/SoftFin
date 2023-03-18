using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CPController : BaseController
    {
        // GET: TH
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ObterTodos()
        {

            var objs = new CategoriaProfissional().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                p.id,
                p.categoria, //Categoria Grid
                p.descricao, // Descrição Grid
                p.empresa_id, // 

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new CategoriaProfissional().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new CategoriaProfissional();
                obj.empresa_id = _empresa;
            }

            return Json(new
            {
                obj.id,
                obj.categoria, //Categoria TextBox 50 
                obj.descricao, // Descrição TextBox 50 
                obj.empresa_id, // 
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Salvar(CategoriaProfissional obj)
        {
            try
            {
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var objErros = obj.Validar(ModelState);

                if (string.IsNullOrEmpty(obj.descricao))
                {
                    objErros.Add("Informe a descrição");
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

                    obj.Alterar(obj,_paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Excluir(CategoriaProfissional obj)
        {

            try
            {
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                var erro = "";
                var funcionou = new CategoriaProfissional().Excluir(obj.id, ref erro, _paramBase);

                if ((funcionou) && (erro == ""))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (erro == "")
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                    }
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
