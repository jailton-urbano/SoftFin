using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SBIController : BaseController
    {
        // GET: SBI
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ObterTodos()
        {

            var objs = new SaldoBancarioInicial().ObterTodos(_paramBase);



            return Json(objs.Select(p => new
            {
                p.id,
                p.banco_id,
                p.Ano,
                p.saldoInicial,
                p.Banco.nomeBanco
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            
            var p = new SaldoBancarioInicial().ObterPorId(id, _paramBase);
            if (p == null)
            {
                p = new SaldoBancarioInicial();
            }

            return Json(new
            {
                p.id,
                p.banco_id,
                p.Ano,
                p.saldoInicial
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(SaldoBancarioInicial obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.banco_id == 0)
                {
                    objErros.Add("Informe o banco");
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


        public JsonResult Excluir(SaldoBancarioInicial obj)
        {

            try
            {
                string erro = "";
                if (obj.Excluir(ref erro, _paramBase))
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


        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }
    }
}