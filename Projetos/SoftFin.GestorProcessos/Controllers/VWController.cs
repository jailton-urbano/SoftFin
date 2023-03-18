using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.Controllers
{
    public class VWController : BaseController
    {
        // GET: TB
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new Visao().ObterTodos(base._paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.IdEmpresa,
                obj.Ativo,
                obj.Descricao,
                obj.IdTabela,
                obj.TipoVisao,
                Tabela = new {obj.Ativo, obj.Descricao, obj.Id, TipoViscaoNome = obj.TipoVisao.Descricao} 
                }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosCampos(int idVisao)
        {
            var objs = new VisaoCampo().ObterTodos(idVisao, _paramBase);
            return Json(objs.Select(p => new
            {
                p.Id,
                p.Ativo,
                p.IdTabelaCampo,
                p.IdVisao,
                p.PadraoSalva,
                p.ReferenciaNgChange,
                p.ReferenciaNgModel,
                p.Transferivel,
                p.Visivel,
                Campo = p.TabelaCampo.Campo,
                Descricao = p.TabelaCampo.Descricao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterPorId(int id)
        {
            var obj = new Visao().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new Visao();
                obj.IdEmpresa = _paramBase.Empresa;
            }

            return Json(new
            {
                obj.Id,
                obj.IdEmpresa,
                obj.Ativo,
                obj.Descricao,
                obj.IdTabela,
                obj.IdTipoVisao,
                obj.TipoVisao,
                VisaoCampos = obj.VisaoCampos.Select(p => new {
                    p.Id,
                    p.Ativo,
                    p.IdTabelaCampo,
                    p.IdVisao,
                    p.PadraoSalva,
                    p.ReferenciaNgChange,
                    p.ReferenciaNgModel,
                    p.Transferivel,
                    p.Visivel,
                    Campo = p.TabelaCampo.Campo,
                    Descricao = p.TabelaCampo.Descricao

                })
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ObterPorIdCampos(int id)
        {
            var obj = new VisaoCampo().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new VisaoCampo();
                obj.IdTabelaCampo = id;
            }

            return Json(new
            {
                obj.Id,
                obj.Ativo,
                obj.IdTabelaCampo,
                obj.IdVisao,
                obj.PadraoSalva,
                obj.ReferenciaNgChange,
                obj.ReferenciaNgModel,
                obj.TabelaCampo,
                obj.Transferivel,
                obj.Visivel,
                Campo = obj.TabelaCampo.Campo,
                Descricao = obj.TabelaCampo.Descricao
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Salvar(Visao obj)
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

        public ActionResult SalvarCampo(VisaoCampo obj)
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


        public JsonResult Excluir(Visao obj)
        {

            try
            {
                if (obj.IdEmpresa != _paramBase.Empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                string erro = "";

                if (new Visao().Excluir(obj.Id, ref erro, _paramBase))
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

        public JsonResult ExcluirCampo(VisaoCampo obj)
        {

            try
            {
                string erro = "";

                if (new VisaoCampo().Excluir(obj.Id, ref erro, _paramBase))
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

        public JsonResult ObterTipoVisao()
        {
            var objs = new TipoVisao().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.Id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTabela()
        {
            var objs = new Tabela().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.Id,
                Text = p.Descricao
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
        public JsonResult ObterTabelaCampos(int tabela_id)
        {
            var objs = new TabelaCampo().ObterTodos(tabela_id).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.Id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}