using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FSController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new  FuncionarioSalario().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                dataInicial = (obj.dataInicial == null) ? "" : obj.dataInicial.Value.ToString("o"),
                obj.funcionario_id,
                nome = obj.Funcionario.Pessoa.nome,
                obj.valorAdiantamento,
                obj.valorBruto,
                obj.valorComplemento
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new FuncionarioSalario().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new FuncionarioSalario();
            }

            return Json(new
            {
                obj.id,
                dataInicial = (obj.dataInicial == null) ? "" : obj.dataInicial.Value.ToString("o"),
                obj.funcionario_id,
                nome = (obj.Funcionario == null)  ? "" : obj.Funcionario.Pessoa.nome,
                obj.valorAdiantamento,
                obj.valorBruto,
                obj.valorComplemento
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(FuncionarioSalario obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.funcionario_id == 0)
                {
                    objErros.Add("Informe a pessoa");
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


        public JsonResult Excluir(FuncionarioSalario obj)
        {

            try
            {


                string erro = "";
                if (new Funcionario().Excluir(obj.id, _paramBase, ref erro))
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
        public JsonResult ObterFuncionarios()
        {
            var objs = new Funcionario().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Pessoa.nome
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
