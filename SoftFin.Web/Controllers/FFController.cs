using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FFController : BaseController
    {
        //
        // GET: /FF/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new FuncionarioFuncao().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.nome,
                obj.descricao,
                obj.valorpiso,
                obj.id
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new FuncionarioFuncao().ObterPorId(id,_paramBase);
            if (obj == null)
            {
                obj = new FuncionarioFuncao();

                obj.estabelecimento_id = _estab;

            }

            return Json(new
            {
                obj.id,
                obj.descricao,
                obj.nome,
                obj.valorpiso,
                obj.estabelecimento_id,
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(FuncionarioFuncao obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id == 0)
                {
                    objErros.Add("Informe o estabelecimento");
                }

                if (string.IsNullOrWhiteSpace(obj.nome))
                {
                    objErros.Add("Informe o nome");
                }

                if (string.IsNullOrWhiteSpace(obj.descricao))
                {
                    objErros.Add("Informe a descrição");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);





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


        public JsonResult Excluir(FuncionarioFuncao obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                string erro = "";
                if (new FuncionarioFuncao().Excluir(obj.id, ref erro, _paramBase))
                {
                    if (erro != "")
                        throw new Exception(erro);

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
