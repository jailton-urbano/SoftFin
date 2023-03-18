using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class BCController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            
            var objs = new Banco().ObterTodos(_paramBase);
            return Json(objs.Select(banco => new
            {
                banco.id,
                banco.codigo,
                banco.ReferenciaIntegracao,
                banco.nomeBanco,
                banco.codigoBanco,
                banco.agencia,
                banco.agenciaDigito,
                banco.contaCorrente,
                banco.contaCorrenteDigito,
                banco.nossoNumero,
                banco.numeroDocumento,
                banco.carteira,
                banco.TextoBoleto01,
                banco.TextoBoleto02,
                banco.TextoBoleto03,
                banco.observacao,
                banco.principal,
                banco.estabelecimento_id,
                banco.EmiteBoleto,
                banco.ValorLimite,
                banco.TipoConta,
                banco.NumeroArquivoRemessa,
                banco.NumeroConvenio,
                banco.AplicacaoAutomatica,
                banco.JurosDia,
                banco.Multa,
                banco.SequencialLoteCNAB,
                banco.EmissaoBoletoAposNF,
                banco.EmissaoBoletoComNFPDF
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterPorId(int id)
        {
            var banco = new Banco().ObterPorId(id, _paramBase);
            if (banco == null)
            {
                banco = new Banco();
                banco.estabelecimento_id = _estab;

            }

            return Json(new
            {
                banco.id,
                banco.codigo,
                banco.ReferenciaIntegracao,
                banco.nomeBanco,
                banco.codigoBanco,
                banco.agencia,
                banco.agenciaDigito,
                banco.contaCorrente,
                banco.contaCorrenteDigito,
                banco.nossoNumero,
                banco.numeroDocumento,
                banco.carteira,
                banco.TextoBoleto01,
                banco.TextoBoleto02,
                banco.TextoBoleto03,
                banco.observacao,
                banco.principal,
                banco.estabelecimento_id,
                banco.EmiteBoleto,
                banco.ValorLimite,
                banco.TipoConta,
                banco.NumeroArquivoRemessa,
                banco.NumeroConvenio,
                banco.AplicacaoAutomatica,
                banco.JurosDia,
                banco.Multa,
                banco.SequencialLoteCNAB,
                banco.CodigoTransmissao,
                banco.EmissaoBoletoAposNF,
                banco.EmissaoBoletoComNFPDF

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(Banco obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                if (obj.id == 0)
                {

                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Banco incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Banco já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Banco alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

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

                if (new Banco().Excluir(id, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Banco excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Banco" }, JsonRequestBehavior.AllowGet);
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
