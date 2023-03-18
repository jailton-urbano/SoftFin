using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioOVController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterRelatorio(DateTime dataIni, DateTime dataFim, string tipo, int? situacao)
        {
            var ovs = new OrdemVenda().ObterEntreData(dataIni, dataFim, _paramBase);

            if (tipo == "A")
            {
                ovs = ovs.Where(p => p.usuarioAutorizador_id != null).ToList();
            }
            if (tipo == "NA")
            {
                ovs = ovs.Where(p => p.usuarioAutorizador_id == null).ToList();
            }

            if (situacao != null)
            {
                ovs = ovs.Where(p => p.statusParcela_ID == situacao).ToList();
            }


            return Json(ovs.Select(p => new
            {
                Contrato = (p.ParcelaContrato == null)? "": 
                    p.ParcelaContrato.ContratoItem.Contrato.contrato,
                Parcela = (p.ParcelaContrato == null) ? "" : p.ParcelaContrato.parcela.ToString(),
                Data = p.data.ToString("o"),
                p.Numero,
                Valor = p.valor,
                Descricao = p.descricao,
                Cliente = p.Pessoa.nome,
                Situacao = p.statusParcela.status,
                Autorizado = (p.usuarioAutorizador_id == null) ? "Pendente Aprovaçao" : "Aprovado",
                NomeAutorizado = (p.usuarioAutorizador == null) ? null : p.usuarioAutorizador.nome,
            }), JsonRequestBehavior.AllowGet);

        }

    }
}