using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class TransfContaController : BaseController
    {
        #region Public
        public ActionResult Index()
        {
            return View();
        }

        public class DtoFiltro
        {
            public int? banco_id { get; set; }
            public DateTime? dataIni { get; set; }
            public DateTime? dataFim { get; set; }
        }


        public JsonResult ObterTodos(DtoFiltro dtoFiltro)
        {

            var objs = new TransfConta().ObterTodos(_paramBase);

            if (dtoFiltro.banco_id != null)
                objs = objs.Where(p => p.BancoSaida_Id == dtoFiltro.banco_id);

            if (dtoFiltro.dataIni != null)
            {
                DateTime dataAux = new DateTime(dtoFiltro.dataIni.Value.Year
                                                , dtoFiltro.dataIni.Value.Month
                                                , dtoFiltro.dataIni.Value.Day, 0, 0, 0);
                objs = objs.Where(p => p.Data >= dataAux);
            }

            if (dtoFiltro.dataFim != null)
            {
                DateTime dataAux = new DateTime(dtoFiltro.dataFim.Value.Year
                                                , dtoFiltro.dataFim.Value.Month
                                                , dtoFiltro.dataFim.Value.Day, 0, 0, 0).AddDays(1);
                objs = objs.Where(p => p.Data <= dataAux);
            }


            var objs2 = objs.ToList();


            return Json(objs2.Select(p => new
            {
                p.Id,
                Estab = _estab,
                Data = p.Data.ToString("o"),
                p.Descricao,
                p.BancoSaida_Id,
                p.BancoEntrada_Id,
                p.UnidadeEntrada_Id,
                p.UnidadeSaida_Id,
                p.BancoMovimentoEntrada_Id,
                p.BancoMovimentoSaida_Id,
                p.Valor,
                TelaBancoSaida = p.BancoSaida.nomeBanco + " - " + p.BancoSaida.agencia + " - " + p.BancoSaida.contaCorrente,
                TelaBancoEntrada = p.BancoEntrada.nomeBanco + " - " + p.BancoEntrada.agencia + " - " + p.BancoEntrada.contaCorrente,
                TelaUnidadeSaida = p.UnidadeSaida.unidade,
                TelaUnidadeEntrada = p.UnidadeEntrada.unidade


            }).ToList(), JsonRequestBehavior.AllowGet);


        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new TransfConta().ObterPorId(id);
            if (obj == null)
            {
                obj = new TransfConta();
                obj.Data = DateTime.Now;
            }

                return Json(new
                {
                    Estab = _estab,
                    obj.Id,
                    Data = obj.Data.ToString("o"),
                    obj.Descricao,
                    obj.Valor,
                    obj.BancoSaida_Id,
                    obj.BancoEntrada_Id,
                    obj.UnidadeEntrada_Id,
                    obj.UnidadeSaida_Id,
                    obj.BancoMovimentoSaida_Id,
                    obj.BancoMovimentoEntrada_Id
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(TransfConta obj)
        {
            try
            {
                var funcionou = false;

                if (obj.Estab != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var objErros = obj.Validar(ModelState);
                if (obj.BancoEntrada_Id == 0)
                {
                    objErros.Add("Informe o banco Entrada");
                }
                if (obj.BancoSaida_Id == obj.BancoEntrada_Id)
                {
                    objErros.Add("Informe o banco Saída e Entrada devem ser diferentes");
                }

                if (obj.BancoSaida_Id == 0)
                {
                    objErros.Add("Informe o banco Saída");
                }


                if (obj.UnidadeEntrada_Id == 0)
                {
                    objErros.Add("Informe o Unidade de Entrada");
                }
                if (obj.UnidadeSaida_Id == 0)
                {
                    objErros.Add("Informe o Unidade de Saída");
                }

                if (string.IsNullOrWhiteSpace(obj.Descricao))
                {
                    objErros.Add("Informe a Descrição");
                }


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.Id == 0)
                {
                    DbControle db = new DbControle();

                    obj.UsuarioInclusao_Id = _usuarioobj.id;
                    obj.DataInclusao = DateTime.Now;
                    var OrigemTransf = new OrigemMovimento().TipoTransferencia(_paramBase);
                    var TipoMovimentoTipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
                    var TipoMovimentoTipoSaida = new TipoMovimento().TipoSaida(_paramBase);
                    var bmsaida = new BancoMovimento();
                    bmsaida.banco_id = obj.BancoSaida_Id;
                    bmsaida.data = obj.Data;
                    bmsaida.dataInclusao = DateTime.Now;
                    bmsaida.estab = _estab;
                    bmsaida.historico = obj.Descricao;
                    bmsaida.origemmovimento_id = OrigemTransf; 
                    bmsaida.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("06.04", db).id;
                    bmsaida.tipoDeMovimento_id = TipoMovimentoTipoSaida;
                    bmsaida.tipoDeDocumento_id = new TipoDocumento().TipoTransf();
                    bmsaida.UnidadeNegocio_id = obj.UnidadeSaida_Id;
                    bmsaida.valor = obj.Valor;
                    
                    var bmentrada = new BancoMovimento();
                    bmentrada.banco_id = obj.BancoEntrada_Id;
                    bmentrada.data = obj.Data;
                    bmentrada.dataInclusao = DateTime.Now;
                    bmentrada.estab = _estab;
                    bmentrada.historico = obj.Descricao;
                    bmentrada.origemmovimento_id = OrigemTransf;
                    bmentrada.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("06.05", db).id;
                    bmentrada.tipoDeMovimento_id = TipoMovimentoTipoSaida;
                    bmentrada.tipoDeDocumento_id = new TipoDocumento().TipoTransf();
                    bmentrada.UnidadeNegocio_id = obj.UnidadeEntrada_Id;
                    bmentrada.valor = obj.Valor;

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        bmsaida.Incluir(_paramBase, db);
                        bmentrada.Incluir(_paramBase, db);

                        obj.BancoMovimentoSaida_Id = bmsaida.id;
                        obj.BancoMovimentoEntrada_Id = bmentrada.id;
                        obj.DataAlteracao = DateTime.Now;
                        funcionou = obj.Incluir(_paramBase,db);

                        if (!funcionou)
                            dbcxtransaction.Rollback();
                        else
                            dbcxtransaction.Commit();
                    }



                }

                if (funcionou)
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Transferencia incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Transferencia já cadastrado" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(TransfConta obj)
        {

            try
            {

                if (obj.Estab != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                DbControle db = new DbControle();

                string erro = "";
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    new TransfConta().Excluir(obj.Id, ref erro, _paramBase, db);
                    new BancoMovimento().Excluir(obj.BancoMovimentoSaida_Id, ref erro, _paramBase, db);
                    new BancoMovimento().Excluir(obj.BancoMovimentoEntrada_Id, ref erro, _paramBase, db);
                    
                    if (erro != "")
                        dbcxtransaction.Rollback();
                    else
                        dbcxtransaction.Commit();
                }


                if (erro == "")
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Transferencia excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Transferencia" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
    }
}