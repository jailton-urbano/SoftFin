using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FCController : BaseController
    {
        //
        // GET: /CI/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new Funcionario().ObterTodos2(_paramBase);
            return Json(objs.Select(p => new
            {
                p.id,
                dataAdmissao = (p.dataAdmissao == null) ? "" : p.dataAdmissao.Value.ToString("o"),
                dataNascimento = (p.dataNascimento == null) ? "" : p.dataNascimento.Value.ToString("o"),
                dataSaida = (p.dataSaida == null) ? "" : p.dataSaida.Value.ToString("o"),
                Funcao = (p.Funcao == null) ? "" : p.Funcao.descricao,
                unidade = (p.UnidadeNegocio == null) ? "" : p.UnidadeNegocio.unidade,
                p.profissao,
                p.estadocivil,
                pessoa_funcionario = (p.Pessoa == null) ? "" : p.Pessoa.nome,
                pessoa_responsavel = (p.Responsavel == null) ? "" : p.Responsavel.nome,
                p.pessoa_id,
                p.responsavel_id,
               
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new Funcionario().ObterPorId(id,_paramBase);
            if (obj == null)
            {
                obj = new Funcionario();
            }

            return Json(new
            {
                obj.id,
                dataAdmissao = (obj.dataAdmissao == null) ? "" : obj.dataAdmissao.Value.ToString("o"),
                dataNascimento = (obj.dataNascimento == null) ? "" : obj.dataNascimento.Value.ToString("o"),
                dataSaida = (obj.dataSaida == null) ? "" : obj.dataSaida.Value.ToString("o"),
                obj.profissao,
                obj.estadocivil,
                obj.funcao_id,
                obj.pessoa_id,
                obj.responsavel_id,
                obj.unidadeNegocio_id,
                pessoa_funcionario = (obj.Pessoa == null) ? "" : obj.Pessoa.nome,
                pessoa_responsavel = (obj.Responsavel == null) ? "" : obj.Responsavel.nome
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(Funcionario obj)
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


        public JsonResult Excluir(Funcionario obj)
        {

            try
            {
                var pesver = new Pessoa().ObterPorId(obj.pessoa_id, _paramBase);



                if (pesver.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);


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
        public JsonResult ObterFuncao()
        {
            var objs = new FuncionarioFuncao().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
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
