using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.Controllers
{
    public class TBController : BaseController
    {
        // GET: TB
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new Tabela().ObterTodos(base._paramBase);
            return Json(objs.Select(p => new
            {
                p.Id,
                p.IdEmpresa,
                p.Nome,
                p.Descricao,
                p.Ativo,
                p.Empresa,
                p.OrdemCriacao,
                p.CadastroAuxiliar,
                p.SQLCadastroAuxiliar
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosCampos(int id)
        {
            var objs = new TabelaCampo().ObterTodos(id);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.Ativo,
                obj.Campo,
                obj.IdChaveEstrageira,
                obj.IdTipoCampo,
                obj.Obrigatorio,
                obj.Ordem,
                obj.Precisao,
                obj.SQLDefault,
                obj.Tabela_Id,
                obj.TamanhoCampo,
                obj.TamanhoColuna,
                
                tipocampodesc = obj.TipoCampo.Descricao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new Tabela().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new Tabela();
                obj.IdEmpresa = _paramBase.Empresa;
            }

            return Json(new
            {
                obj.Id,
                obj.IdEmpresa,
                obj.Descricao,
                obj.Ativo,
                obj.Nome,
                obj.Empresa,
                obj.OrdemCriacao,
                obj.CadastroAuxiliar,
                obj.SQLCadastroAuxiliar,
                TabelaCampos = obj.TabelaCampos.Select(p => new {
                    p.Id,
                    p.Ativo,
                    p.Campo,
                    p.ChaveEstrageira,
                    p.Descricao,
                    p.IdChaveEstrageira,
                    p.IdTipoCampo,
                    p.Obrigatorio,
                    p.Ordem,
                    p.Precisao,
                    p.SQLDefault,
                    p.Tabela_Id,
                    p.TamanhoCampo,
                    p.TamanhoColuna,
                    tipocampodesc = p.TipoCampo.Descricao }).OrderBy(p => p.Ordem)
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ObterPorIdCampos(int id)
        {
            var obj = new TabelaCampo().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new TabelaCampo();
                obj.Tabela_Id = id;
            }

            return Json(new
            {
                obj.Id,
                obj.Ativo,
                obj.Campo,
                obj.IdChaveEstrageira,
                obj.IdTipoCampo,
                obj.Obrigatorio,
                obj.Ordem,
                obj.Precisao,
                obj.SQLDefault,
                obj.Tabela_Id,
                obj.TamanhoCampo,
                obj.TamanhoColuna,
                tipocampodesc = (obj.TipoCampo == null)? "": obj.TipoCampo.Descricao
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Salvar(Tabela obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.IdEmpresa != _paramBase.Empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                if (obj.Id == 0)
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SalvarCampo(TabelaCampo obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.Id == 0)
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(Tabela obj)
        {

            try
            {
                if (obj.IdEmpresa != _paramBase.Empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                string erro = "";

                if (new Tabela().Excluir(obj.Id, ref erro, _paramBase))
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ExcluirCampo(TabelaCampo obj)
        {

            try
            {
                string erro = "";

                if (new TabelaCampo().Excluir(obj.Id, ref erro, _paramBase))
                {
                    if (erro != "")
                        throw new Exception(erro);

                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluido com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ObterTabelas()
        {
            var objs = new Tabela().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Nome);
            return Json(objs.Select(p => new
            {
                Value = p.Id,
                Text = p.Nome
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTipoCampos()
        {
            var objs = new TipoCampo().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.Id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}