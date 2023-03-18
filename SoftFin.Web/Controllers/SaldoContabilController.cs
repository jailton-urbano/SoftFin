using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SaldoContabilController : BaseController
    {
        // GET: SaldoContabil
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new SaldoContabil().ObterTodos(_paramBase);

            return Json(objs.Select(obj => new
            {
                obj.Id,
                DataBase = obj.DataBase.ToString("MM/yyyy"),
                obj.Usuario,
                DataFechamento = obj.DataFechamento.ToString("o"),
                obj.HashCode,
                obj.Situacao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new SaldoContabil();
            if (id == 0)
            {
                obj.Estabelecimento_id = _estab;
                obj.Usuario = _usuario;
                obj.DataFechamento = DateTime.Now;
                obj.DataBase = DateTime.Now;

            }
            else
            {
                obj = new SaldoContabil().ObterPorId(id, _paramBase);

                if (obj == null)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Sando Inicial não encontrado" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new
            {
                obj.Id,
                DataBase = obj.DataBase.ToString("o"),
                DataFechamento = obj.DataFechamento.ToString("o"),
                obj.Estabelecimento_id,
                obj.HashCode,
                obj.Situacao,
                obj.Usuario,

                SaldoContabilDetalhe = obj.SaldoContabilDetalhe.Select(p => new
                    {
                        p.Id,
                        p.SaldoContabil_id,
                        p.SaldoFinal,
                        p.SaldoInicial,
                        p.TotalCredito,
                        p.TotalDebito,
                        p.Deleted,
                        p.CodigoConta,
                        p.DescricaoConta,
                        p.CodigoCentroCusto,
                        p.DescricaoCentroCusto,
                }
                )

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ObterTodosCentroCustos()
        {
            var objs = new UnidadeNegocio().ObterTodos(_paramBase);

            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.unidade,
                obj.Codigo
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTodosContaContabil ()
        {
            var objs = new ContaContabil().ObterTodos(_paramBase);

            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.codigo,
                obj.descricao
            }).OrderBy(p => p.codigo), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Salvar(SaldoContabil obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);
                if (obj.Estabelecimento_id == 0)
                {
                    objErros.Add("Informe o estabelecimento");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Errors = objErros });
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
                    var db = new DbControle();
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {

                        var vigs = obj.SaldoContabilDetalhe;
                        obj.Alterar(_paramBase, db);
                        var erro = "";

                        foreach (var item in vigs)
                        {
                            item.Id = obj.Id;
                            if (item.Id == 0)
                            {
                                if (!item.Deleted)
                                {
                                    item.Incluir(_paramBase, db);
                                }
                            }
                            else
                            {
                                if (!item.Deleted)
                                {
                                    item.Alterar(_paramBase, db);
                                }
                                else
                                {
                                    item.Excluir(ref erro, _paramBase, db);
                                    if (erro != "")
                                    {
                                        _eventos.Error(erro);
                                        return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                        }
                        dbcxtransaction.Commit();
                    }
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Excluir(SaldoContabil obj)
        {

            try
            {
                var objAuxs = new SaldoContabilDetalhe().ObterTodosMestre(obj.Id, _paramBase);

                string erro = "";
                var db = new DbControle();
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    foreach (var item in objAuxs)
                    {
                        item.Excluir(ref erro, _paramBase, db);
                        if (erro != "")
                        {
                            dbcxtransaction.Rollback();
                            _eventos.Error(erro);
                            return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    
                    if (new SaldoContabil().Excluir(ref erro, _paramBase, db))
                    {
                        if (erro != "")
                        {
                            dbcxtransaction.Rollback();
                            _eventos.Error(erro);
                            return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                        }
                        dbcxtransaction.Commit();
                        return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}