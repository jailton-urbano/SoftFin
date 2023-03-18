using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LojaCaixaController : BaseController
    {
        //
        // GET: /LojaCaixa/

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new LojaCaixa().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.ativo,
                obj.codigo,
                obj.descricao,
                obj.Loja_id,
                loja = obj.Loja.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new LojaCaixa().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new LojaCaixa();
                obj.ativo = true;


            }

            return Json(new
            {
                obj.id,
                obj.ativo,
                obj.codigo,
                obj.descricao,
                obj.Loja_id
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(LojaCaixa obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);
                if (obj.Loja_id == 0)
                {
                    objErros.Add("Informe a loja");
                }
                else
                {
                    var objLoja = new Loja().ObterPorId(obj.Loja_id, _paramBase);
                    if (objLoja == null)
                        return Json(new { CDMessage = "NOK", DSMessage = "Loja não encontrada" }, JsonRequestBehavior.AllowGet);
                    if (objLoja.estabelecimento_id != _estab)
                        return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(obj.codigo))
                {
                    objErros.Add("informe o código da caixa");
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


        public JsonResult Excluir(LojaCaixa obj)
        {

            try
            {
                var objLoja = new Loja().ObterPorId(obj.Loja_id, _paramBase);
                if (objLoja == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Loja não encontrada" }, JsonRequestBehavior.AllowGet);
                if (objLoja.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                string erro = "";
                if (new LojaCaixa().Excluir(obj.id, ref erro, _paramBase))
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

        public JsonResult ObterLoja()
        {
            var objs = new Loja().ObterTodos(_paramBase).Where(p => p.ativo == true);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        
    }
}
