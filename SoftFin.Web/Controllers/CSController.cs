using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CSController : BaseController
    {
        //
        // GET: /CI/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new CodigoServicoEstabelecimento().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                p.CodigoServicoMunicipio.codigo,
                p.CodigoServicoMunicipio.descricao,
                p.CodigoServicoMunicipio.aliquota,
                p.CodigoServicoMunicipio.municipio.DESC_MUNICIPIO,
                p.id,
                p.estabelecimento_id
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new CodigoServicoEstabelecimento().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new CodigoServicoEstabelecimento();

                obj.estabelecimento_id = _estab;

            }

            return Json(new
            {
                obj.id,
                obj.codigoServicoMunicipio_id,
                obj.estabelecimento_id,
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(CodigoServicoEstabelecimento obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id == 0)
                {
                    objErros.Add("Informe o imposto");
                }
                if (obj.codigoServicoMunicipio_id == 0)
                {
                    objErros.Add("Informe a Código do Municipio");
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


        public JsonResult Excluir(CodigoServicoEstabelecimento obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                string erro = "";
                if (new CodigoServicoEstabelecimento().Excluir(obj.id,ref erro, _paramBase))
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

        public JsonResult ObterCodigoServicoMunicipio()
        {
            var objs = new CodigoServicoMunicipio().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p =>p.codigo);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.codigo + " - " +  p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
