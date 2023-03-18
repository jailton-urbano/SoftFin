using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CIController : BaseController
    {
        //
        // GET: /CI/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new calculoImposto().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                p.id,
                p.arrecadador,
                p.baseCalculo,
                p.CST,
                p.imposto.descricao,
                p.margemValorAgregado,
                p.modalidade,
                operacao = p.operacao.descricao,
                p.retido
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new calculoImposto().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new calculoImposto();
                //obj.empresa_id = _empresa ;
                obj.estabelecimento_id = _estab;

            }

            return Json(new
            {
                obj.id,
                obj.arrecadador,
                obj.baseCalculo,
                obj.CST,
                obj.imposto_id,
                obj.margemValorAgregado,
                obj.modalidade,
                obj.operacao_id,
                obj.retido,
                obj.estabelecimento_id,
                obj.aliquota
                
                
                

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(calculoImposto obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.imposto_id == 0)
                {
                    objErros.Add("Informe o imposto");
                }
                if (obj.operacao_id == 0)
                {
                    objErros.Add("Informe a operação");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                


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


        public JsonResult Excluir(calculoImposto obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                if (new Banco().Excluir(obj.id,_paramBase))
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


        public JsonResult ObterImposto()
        {
            var objs = new Imposto().ObterTodos();
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterOperacao()
        {
            var objs = new Operacao().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
