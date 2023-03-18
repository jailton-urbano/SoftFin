using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SDBFController : BaseController
    {
        // GET: SDDF
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ObterTodos()
        {

            var objs = new SistemaDashBoardFuncionalidade().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                p.id,
                p.ativo,
                p.Action,
                p.Descricao,
                p.cadastro,
                p.Controller,
                p.cor,
                p.Funcionalidade_id,
                Funcionalidade_Descricao = p.Funcionalidade.Descricao,
                p.Texto,
                p.relatorio,
                p.sistemaDashBoard_id,
                SistemaDashBoard_Descricao = p.SistemaDashBoard.Descricao
                }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new SistemaDashBoardFuncionalidade().ObterPorId(id);
            if (obj == null)
            {
                obj = new SistemaDashBoardFuncionalidade();
                //obj.empresa_id = _empresa ;
            }

            return Json(new
            {
                obj.id,
                obj.ativo,
                obj.Action,
                obj.Descricao,
                obj.cadastro,
                obj.Controller,
                obj.cor,
                obj.Funcionalidade_id,

                obj.Texto,
                obj.relatorio,
                obj.sistemaDashBoard_id

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(SistemaDashBoardFuncionalidade obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (string.IsNullOrEmpty(obj.Descricao))
                {
                    objErros.Add("Informe a descrição");
                }

                if (string.IsNullOrEmpty(obj.Controller))
                {
                    objErros.Add("Informe a controller");
                }

                if (obj.sistemaDashBoard_id == 0)
                {
                    objErros.Add("Informe sistema menu");
                }


                if (obj.Funcionalidade_id == 0)
                {
                    objErros.Add("Informe a funcionalidade");
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


        public JsonResult Excluir(SistemaDashBoardFuncionalidade obj)
        {

            try
            {
                if (new SistemaDashBoardFuncionalidade().Excluir(obj.id, _paramBase))
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



        public JsonResult ObterFuncionalidade()
        {
            //Funcionalidade_id
            var objs = new Funcionalidade().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterSistemaDashBoard()
        {
            //Funcionalidade_id
            var objs = new SistemaDashBoard().ObterTodos().Where(p => p.ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }

    }
}