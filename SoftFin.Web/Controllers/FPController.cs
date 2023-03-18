using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FPController : BaseController
    {
        //
        // GET: /FP/

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new FolhaPagamento().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.dataBase,
                DataPagamento = obj.DataPagamento.ToString("o"),
                obj.folhapagamentotipo_id,
                obj.funcionario_id,
                obj.estabelecimento_id,
                obj.unidadenegocio_id,
                obj.valor,
                funcionario = obj.Funcionario.Pessoa.nome,
                cpf = obj.Funcionario.Pessoa.cnpj,
                folhatipo = obj.FolhaPagamentoTipo.descricao,
                unidade = obj.UnidadeNegocio.unidade
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new FolhaPagamento().ObterPorId(id,_paramBase);
            if (obj == null)
            {
                obj = new FolhaPagamento();

                obj.estabelecimento_id = _estab;
                obj.DataPagamento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();

            }

            return Json(new
            {
                obj.id,
                obj.dataBase,
                DataPagamento= obj.DataPagamento.ToString("o"),
                obj.folhapagamentotipo_id,
                obj.funcionario_id,
                obj.estabelecimento_id,
                obj.unidadenegocio_id,
                obj.valor
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(FolhaPagamento obj)
        {
            try
            {
                _eventos.Info("Teste FP");
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                var objErros = obj.Validar(ModelState);

                if (obj.unidadenegocio_id == 0)
                {
                    objErros.Add("Informe a unidade de negocio");
                }
                if (obj.folhapagamentotipo_id== 0)
                {
                    objErros.Add("Informe o tipo de folha de pagamento");
                }
                if (obj.funcionario_id == 0)
                {
                    objErros.Add("Informe o funcionário");
                }



                if (obj.DataPagamento <= SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().AddDays(-1))
                {
                    objErros.Add("Data de Pagamento deve ser maior que o dia atual");
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


        public JsonResult Excluir(FolhaPagamento obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


                string erro = "";
                if (new FolhaPagamento().Excluir(obj.id, ref erro, _paramBase))
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

        public JsonResult ObterUnidades()
        {
            var objs = new UnidadeNegocio().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.unidade
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterFolhaPagamentoTipo()
        {
            var objs = new FolhaPagamentoTipo().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
