using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LojaFechamentoCCController : BaseController
    {
        //
        // GET: /LojaFechamentoCC/


        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos(int id)
        {
           

            var objs = new LojaFechamentoCC().ObterTodos(id, _paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.LojaFechamento_id,
                obj.LojaTipoRecebimentoCaixa_id,
                obj.descricao,
                obj.tipoMovimento,
                obj.tipoVenda,
                obj.valorBruto,
                obj.valorLiquido,
                obj.valorTaxa,
                loja = obj.LojaFechamento.LojaCaixa.Loja.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new LojaFechamentoCC().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new LojaFechamentoCC();



            }

            return Json(new
            {
                obj.id,
                obj.LojaFechamento_id,
                obj.LojaTipoRecebimentoCaixa_id,
                obj.descricao,
                obj.tipoMovimento,
                obj.tipoVenda,
                obj.valorBruto,
                obj.valorLiquido,
                obj.valorTaxa
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(LojaFechamentoCC obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);
                if (obj.LojaFechamento_id == 0)
                {
                    objErros.Add("Informe a loja");
                }
                
                if (obj.valorBruto == 0)
                {
                    objErros.Add("informe o valor bruto");
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


        public JsonResult Excluir(LojaFechamentoCC obj)
        {

            try
            {

                string erro = "";
                if (new LojaFechamentoCC().Excluir(obj.id, ref erro, _paramBase))
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
            var objs = new Loja().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }


    }
}
