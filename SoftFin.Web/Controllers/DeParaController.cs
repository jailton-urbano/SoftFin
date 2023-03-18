using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class DeParaController : BaseController
    {
        //                                                                                                                                                                                                                                          
        // GET: /LojaTipoRecebimentoCaixa/
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new DeParaMestre().ObterTodos(_paramBase);
            
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.Codigo,
                obj.Descricao,
                obj.Estabelecimento_id
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new DeParaMestre().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new DeParaMestre();
                obj.Estabelecimento_id = _estab;
                obj.DeParaItems = new List<DeParaItem>();
                obj.DeParaItems.Add(new DeParaItem() { Id = 0, De = "", Para = "" });
            }
            else
            {
                obj.DeParaItems = new List<DeParaItem>();
                obj.DeParaItems.AddRange(new DeParaItem().ObterPorIdMestre(obj.Id,_paramBase));
            }
            return Json(new
            {
                obj.Id,
                obj.Codigo,
                obj.Descricao,
                obj.Estabelecimento_id,
                DeParaItems = obj.DeParaItems.Select(p => new
                {
                    p.De,
                    p.Para,
                    p.Id,
                    p.deleted
                })
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(DeParaMestre obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);
                if (obj.Estabelecimento_id == 0)
                {
                    objErros.Add("Informe o estabelecimento");
                }
               
                if (obj.Descricao == "")
                {
                    objErros.Add("informe a descrição");
                }

                if (string.IsNullOrWhiteSpace(obj.Codigo))
                {
                    objErros.Add("informe o código");
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
                    var vigs = obj.DeParaItems;
                    obj.Alterar(obj, _paramBase);

                    foreach (var item in vigs)
                    {
                        item.DePara_Id = obj.Id;
                        if (item.Id == 0)
                        {
                            if (!item.deleted)
                            {
                                item.Incluir(_paramBase);
                            }
                        }
                        else
                        {
                            if (!item.deleted)
                            {
                                item.Alterar(_paramBase);
                            }
                            else
                            {
                                item.Excluir(_paramBase);
                            }
                        }
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


        public JsonResult Excluir(DeParaMestre obj)
        {

            try
            {
                var objAuxs = new DeParaItem().ObterPorIdMestre(obj.Id, _paramBase);


                foreach (var item in objAuxs)
                {
                    item.Excluir(_paramBase);
                }

                string erro = "";
                if (new DeParaMestre().Excluir(obj.Id, _paramBase))
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

    }
}
