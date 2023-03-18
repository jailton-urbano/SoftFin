using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FAController : BaseController
    {
        //
        // GET: /FA/
        public override JsonResult TotalizadorDash(int? id)
        {
            //base.TotalizadorDash(id);
            var soma = new FornecedorAcordo().ObterTodos(_paramBase).Sum(p => p.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new FornecedorAcordo().ObterTodos(_paramBase);
            return Json(objs.Select(item => new
            {
                    item.id,
                    item.Fornecedor.Pessoa.nome,
                    item.Fornecedor.Pessoa.cnpj,
                    Data = item.Data.ToString("o"),
                    item.descricao,
                    item.valor
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new FornecedorAcordo().ObterPorId(id,_paramBase);
            if (obj == null)
            {
                obj = new FornecedorAcordo();
                obj.Data = DateTime.Now;
            }

            return Json(new
            {
                obj.id,
                Data = obj.Data.ToString("o"),
                obj.descricao,
                obj.fornecedor_id,
                obj.valor,
                nome =(obj.Fornecedor == null) ? "" : obj.Fornecedor.Pessoa.nome
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(FornecedorAcordo obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);



                if (obj.fornecedor_id == 0)
                {
                    objErros.Add("Informe o Fornecedor");
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


        public JsonResult Excluir(FornecedorAcordo obj)
        {

            try
            {

                string erro = "";
                if (new FornecedorAcordo().Excluir(obj.id, ref erro,_paramBase))
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



        public JsonResult ObterFornecedor()
        {
            var objs = new Fornecedor().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Pessoa.nome
            }), JsonRequestBehavior.AllowGet);
        }
        //Fornecedor().ObterListaTodos(_paramBase)
    }
}
