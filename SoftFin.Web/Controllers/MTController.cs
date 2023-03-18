using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class MTController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            
            var objs = new Operacao().ObterTodos(_paramBase);
            return Json(objs.Select(operacao => new
            {
                operacao.id,
                operacao.empresa_id,
                operacao.codigo,
                operacao.descricao,
                operacao.descricaoNota,
                operacao.tipoRPS_id,
                operacao.situacaoTributariaNota_id,
                operacao.entradaSaida,
                operacao.estabelecimento_id,
                operacao.idAux,
                operacao.CFOP,
                operacao.CSOSN,
                operacao.produtoServico,
                operacao.percentualCargaTributaria,
                operacao.fonteCargaTributaria,
                CalculoImposto = operacao.CalculoImposto.Select(
                    p => new { 
                        p.aliquota,
                        p.arrecadador,
                        p.baseCalculo,
                        p.estabelecimento_id,
                        p.imposto_id,
                        p.margemValorAgregado,
                        p.modalidade,
                        p.operacao_id,
                        p.retido,
                        p.CST,
                        CalculoImpostoTipoImpostos = p.CalculoImpostoTipoImpostos.Select(
                        y => new {
                            y.ativo,
                            y.TipoBase,
                            descricao = y.TipoBase.descricao,
                            y.id
                        })
                    }
                )
                
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterPorId(int id)
        {
            var operacao = new Operacao().ObterPorId(id, _paramBase);
            if (operacao == null)
            {
                operacao = new Operacao();
                operacao.empresa_id = _empresa;
                operacao.estabelecimento_id = _estab;
            }

            var CalculoImpostoTipoImposto = new CalculoImpostoTipoImposto();
            return Json(new
            {
                operacao.id,
                operacao.empresa_id,
                operacao.codigo,
                operacao.descricao,
                operacao.descricaoNota,
                operacao.tipoRPS_id,
                operacao.situacaoTributariaNota_id,
                operacao.entradaSaida,
                operacao.estabelecimento_id,
                operacao.idAux,
                operacao.CFOP,
                operacao.CSOSN,
                operacao.produtoServico,
                operacao.percentualCargaTributaria,
                operacao.fonteCargaTributaria,


                CalculoImpostos = operacao.CalculoImposto.Select(
                    p => new
                    {
                        id = 0,
                        p.aliquota,
                        p.arrecadador,
                        p.baseCalculo,
                        p.estabelecimento_id,
                        p.imposto_id,
                        p.margemValorAgregado,
                        p.modalidade,
                        p.operacao_id,
                        p.retido,
                        p.CST,
                        CalculoImpostoTipoImpostos = CalculoImpostoTipoImposto.ObterTodos(p.id).Select(
                        y => new
                        {
                            y.ativo,
                            y.TipoBase,
                            y.tipoBase_id,
                            descricao = y.TipoBase.descricao,
                            id = 0
                        })
                    }
                )

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult NovoCalculoImposto(int id)
        {
            var ci = new calculoImposto().ObterPorId(id, _paramBase);
            var objs = new TipoBase().ObterTodos(_paramBase);
            if (ci == null)
                ci = new calculoImposto();

            ci.CalculoImpostoTipoImpostos = new List<CalculoImpostoTipoImposto>();

            

            foreach (var item in objs)
            {
                var y = ci.CalculoImpostoTipoImpostos.Where(p => p.id == item.id).ToList();

                if (y.Count() == 0)
                {
                    ci.CalculoImpostoTipoImpostos.Add(new CalculoImpostoTipoImposto
                            {
                                tipoBase_id = item.id,
                                TipoBase = item,
                                ativo = false
                            }
                    );
                }
            }
            


            return Json(new
            {
                ci = new
                    {
                        ci.aliquota,
                        ci.arrecadador,
                        ci.baseCalculo,
                        empresa_id = _empresa,
                        ci.imposto_id,
                        ci.margemValorAgregado,
                        ci.modalidade,
                        ci.operacao_id,
                        ci.retido,
                        ci.CST,
                        CalculoImpostoTipoImpostos = ci.CalculoImpostoTipoImpostos.Select(
                        y => new
                        {
                            y.ativo,
                            y.TipoBase,
                            descricao = y.TipoBase.descricao,
                            y.id,
                            y.tipoBase_id
                        })
                    }
                

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(Operacao obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.produtoServico == "S")
                {
                    if (obj.situacaoTributariaNota_id == null)
                        objErros.Add("Infome a situação tributária");
                    if (obj.tipoRPS_id == null)
                        objErros.Add("Infome o tipo RPS");
                }


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.estabelecimento_id != _estabobj.id)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var operacao = new Operacao();

                    if (obj.id != 0)
                    {
                        operacao = new Operacao().ObterPorId(obj.id,db,_paramBase);
                        foreach (var item in operacao.CalculoImposto)
                        {
                            string erro = "";
                            item.Excluir(item.id, ref erro, _paramBase);
                        }
                    }

                    operacao.empresa_id = obj.empresa_id;
                    operacao.estabelecimento_id = obj.estabelecimento_id;
                    operacao.CFOP = obj.CFOP;
                    operacao.codigo = obj.codigo;
                    operacao.CSOSN = obj.CSOSN;
                    operacao.descricao = obj.descricao;
                    operacao.descricaoNota = obj.descricaoNota;
                    operacao.situacaoTributariaNota_id = obj.situacaoTributariaNota_id;
                    operacao.tipoRPS_id = obj.tipoRPS_id;
                    operacao.entradaSaida = obj.entradaSaida;
                    operacao.produtoServico = obj.produtoServico;
                    operacao.fonteCargaTributaria = obj.fonteCargaTributaria;
                    operacao.percentualCargaTributaria = obj.percentualCargaTributaria;
                    if (obj.id == 0)
                    {
                        operacao.Incluir(operacao, _paramBase, db);
                    }
                    else
                    {
                        operacao.Alterar(operacao, _paramBase, db);
                    }

                    foreach (var item in obj.CalculoImposto)
                    {
                        var ci = new calculoImposto();
                        ci.aliquota = item.aliquota;
                        ci.arrecadador = item.arrecadador;
                        ci.baseCalculo = item.baseCalculo;
                        ci.estabelecimento_id = _paramBase.estab_id;
                        ci.imposto_id = item.imposto_id;
                        ci.margemValorAgregado = item.margemValorAgregado;
                        ci.modalidade = item.modalidade;
                        ci.operacao_id = operacao.id;
                        ci.retido = item.retido;
                        ci.CST = item.CST;
                        ci.Incluir(ci, _paramBase, db);

                        foreach (var itemBase in item.CalculoImpostoTipoImpostos)
                        {
                            var bs = new CalculoImpostoTipoImposto();
                            bs.ativo = itemBase.ativo;
                            bs.tipoBase_id = itemBase.tipoBase_id;
                            bs.calculoImposto_id = ci.id;
                            bs.Incluir(bs, _paramBase, db);
                        }

                    }

                    dbcxtransaction.Commit();
                }

                return Json(new { CDStatus = "OK", DSMessage = "Operação salvo com sucesso" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(int id)
        {

            try
            {
                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var operacao = new Operacao();

                    operacao = new Operacao().ObterPorId(id, db,_paramBase);
                    foreach (var item in operacao.CalculoImposto)
                    {
                        string erro = "";
                        item.Excluir(item.id, ref erro, _paramBase);
                    }
                    if (new Operacao().Excluir(id, db))
                    {
                        dbcxtransaction.Commit();
                        return Json(new { CDStatus = "OK", DSMessage = "Operação excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Operação" }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult ListaSituacaoTributaria()
        {
            var objs = new SituacaoTributariaNota().ObterTodos();
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaImposto()
        {
            return Json(new SelectList(new Imposto().ObterTodos(), "id", "descricao"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObterCFOP()
        {
            var objs = new CFOP().ObterTodos();


            return Json(objs.Select(p => new
            {
                Value = p.codigo,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObtemEntidade(int id)
        {
            OperacaoView obj = new OperacaoView();

            if (id != 0)
            {
                var data = new Operacao().ObterPorId(id, _paramBase);

                obj.CalculoImpostoList = new List<CalculoImpostoView>();

                if (data != null)
                {
                    obj.id = data.id;
                    obj.codigo = data.codigo;
                    obj.descricao = data.descricao;
                    obj.descricaoNota = data.descricaoNota;
                    


                    obj.tipoRPS_id = data.tipoRPS_id.ToString();
                    obj.situacaoTributariaNota_id = data.situacaoTributariaNota_id.ToString();
                    obj.entradaSaida = data.entradaSaida;


                    var dataCalculoImposto = new calculoImposto().ObterTodos(_paramBase).Where(x => x.operacao_id == id).ToList();
                    foreach (var item in dataCalculoImposto)
                    {
                        var impostoaux = new Imposto().ObterTodos().Where(p => p.id == item.imposto.id);
                        if (impostoaux.Count() == 0)
                        {
                            return Json(new { CDMessage = "NOK", DSErroReturn = "Imposto não encontrado" }, JsonRequestBehavior.AllowGet);
                        }

                        obj.CalculoImpostoList.Add(new CalculoImpostoView
                        {
                            id = data.id,
                            aliquota = item.aliquota,
                            arrecadador = item.arrecadador,
                            retido = item.retido,
                            imposto = impostoaux.First().descricao,
                        });
                    }
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ListaTipoRPS()
        {
            var objs = new TipoRPS().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ObterCST(int impostoid)
        {
            var objs = new CST().ObterPorImposto(impostoid);


            return Json(objs.Select(p => new
            {
                Value = p.codigo,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
