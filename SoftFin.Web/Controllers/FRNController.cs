using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FRNController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new Fornecedor().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                p.id,
                p.NomeEmpresa,
                dataContratada = (p.dataContratada == null) ? "" : p.dataContratada.Value.ToString("o"),
                dataNascimento = (p.dataSaida == null) ? "" : p.dataSaida.Value.ToString("o"),
                p.Pessoa.cnpj,
                p.Pessoa.nome,
                p.pessoa_id,
                p.responsavel_id,
                p.unidadeNegocio_id
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var p = new Fornecedor().ObterPorId(id,_paramBase);
            if (p == null)
            {
                p = new Fornecedor();
            }

            return Json(new
            {
                p.id,
                dataContratada = (p.dataContratada == null) ? "" : p.dataContratada.Value.ToString("o"),
                dataSaida = (p.dataSaida == null) ? "" : p.dataSaida.Value.ToString("o"),
                p.pessoa_id,
                p.responsavel_id,
                p.unidadeNegocio_id,
                p.NomeEmpresa
               

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(Fornecedor obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.pessoa_id == 0)
                {
                    objErros.Add("Informe a pessoa");
                }
                if (obj.responsavel_id == 0)
                {
                    objErros.Add("Informe o Responsável");
                }
                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                var pesver = new Pessoa().ObterPorId(obj.pessoa_id, _paramBase);



                if (pesver.empresa_id != _empresa)
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


        public JsonResult Excluir(Fornecedor obj)
        {

            try
            {
                var pesver = new Pessoa().ObterPorId(obj.pessoa_id,_paramBase,  null);



                if (pesver.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                string erro = "";
                if (new Fornecedor().Excluir(obj.id, ref erro, _paramBase))
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
        public JsonResult ObterPessoas()
        {
            var objs = new Pessoa().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nome 
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterUnidadeNegocios()
        {
            var objs = new UnidadeNegocio().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.unidade
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
