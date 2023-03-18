using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LojaTipoRecebimentoCaixaController : BaseController
    {
        //                                                                                                                                                                                                                                          
        // GET: /LojaTipoRecebimentoCaixa/

 



        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new LojaTipoRecebimentoCaixa().ObterTodos(_paramBase);
            
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.ativo,
                obj.codigo,
                obj.descricao,
                obj.Loja_id,
                vigencias = obj.vigencias.Select(p => new
                    { dataFimVigencia = (p.dataFimVigencia == null) ? "" : p.dataFimVigencia.Value.ToString("o"),
                        p.historico, 
                        p.id,
                        p.LojaTipoRecebimentoCaixa_id,
                        p.prazoDias,
                        p.taxa
                    }),
                obj.banco_id,
                loja = obj.Loja.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new LojaTipoRecebimentoCaixa().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new LojaTipoRecebimentoCaixa();

                obj.vigencias.Add(new LojaTipoRecebimentoCaixaVigencia() { id = 0, dataFimVigencia = null, prazoDias = 1, taxa = 0, historico = "Nova Inclusão" });

            }

            return Json(new
            {
                obj.id,
                obj.ativo,
                obj.codigo,
                obj.descricao,
                obj.Loja_id,
                vigencias = obj.vigencias.Select(p => new
                {
                    dataFimVigencia = (p.dataFimVigencia == null) ? "" : p.dataFimVigencia.Value.ToString("o"),
                    p.historico,
                    p.id,
                    p.LojaTipoRecebimentoCaixa_id,
                    p.prazoDias,
                    p.taxa,
                    p.deleted
                }),
                obj.banco_id

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(LojaTipoRecebimentoCaixa obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);
                if (obj.Loja_id == 0)
                {
                    objErros.Add("Informe a loja");
                }
                else
                {
                    var objLoja = new Loja().ObterPorId(obj.Loja_id, _paramBase);
                    if (objLoja == null)
                        return Json(new { CDMessage = "NOK", DSMessage = "Loja não encontrada" }, JsonRequestBehavior.AllowGet);
                    if (objLoja.estabelecimento_id != _estab)
                        return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                }

                if (obj.banco_id == 0)
                {
                    objErros.Add("informe o banco corretamete");
                }

                if (string.IsNullOrWhiteSpace(obj.codigo))
                {
                    objErros.Add("informe o código da caixa");
                }
                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                foreach (var item in obj.vigencias)
                {
                    if (item.dataFimVigencia != null)
                    {
                        item.dataFimVigencia = SoftFin.Utils.UtilSoftFin.TiraHora(item.dataFimVigencia.Value); 
                    }
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
                    var vigs = obj.vigencias;
                    obj.vigencias = null;
                    obj.Alterar(obj, _paramBase);

                    foreach (var item in vigs)
                    {
                        item.LojaTipoRecebimentoCaixa_id = obj.id;
                        if (item.id == 0)
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
                                var erro = "";
                                item.Excluir(ref erro, _paramBase);
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


        public JsonResult Excluir(LojaTipoRecebimentoCaixa obj)
        {

            try
            {
                var objLoja = new Loja().ObterPorId(obj.id, _paramBase);
                if (objLoja == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Loja não encontrada" }, JsonRequestBehavior.AllowGet);
                if (objLoja.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                string erro = "";
                if (new LojaTipoRecebimentoCaixa().Excluir(obj.id, ref erro, _paramBase))
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

        public JsonResult ObterLoja()
        {
            var objs = new Loja().ObterTodos(_paramBase).Where(p => p.ativo == true);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }



        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = int.Parse(p.Value),
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }


    }
}
