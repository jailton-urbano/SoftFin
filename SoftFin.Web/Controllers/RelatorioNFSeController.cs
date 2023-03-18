using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioNFSeController : BaseController
    {
        private void CarregaViewData()
        {
            ViewData["Pessoa"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "nome", "nome");

            //Monta Lista Tipos de Data
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "1", Text = "Data Emissão NFEs" });
            items.Add(new SelectListItem() { Value = "2", Text = "Data Vencimento" });

            ViewData["DataFiltro"] = new SelectList(items, "Value", "Text");

            //Monta Lista Status de Pagamento
            var items2 = new List<SelectListItem>();
            items2.Add(new SelectListItem() { Value = "1", Text = "Em Aberto" });
            items2.Add(new SelectListItem() { Value = "2", Text = "Recebido Parcialmente" });
            items2.Add(new SelectListItem() { Value = "3", Text = "Recebido Total" });
            ViewData["StatusPagamento"] = new SelectList(items2, "Value", "Text");

            var items32 = new List<SelectListItem>();
            items32.Add(new SelectListItem() { Value = "1", Text = "Não Cancelados" });
            items32.Add(new SelectListItem() { Value = "2", Text = "Cancelados" });
            ViewData["Situacao"] = new SelectList(items32, "Value", "Text");
        }

        // GET: RelatorioNFe
        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }
        public JsonResult getRelatorio(string dataInicial, string dataFinal, string pessoa, int status, int DataFiltro, int situacao)
        {
            ViewBag.usuario = _paramBase.usuario_name;
            ViewBag.perfil = _paramBase.perfil_id; ;
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);
            
            var Listas = new List<NotaFiscal>();

            if (DataFiltro == 1)
            {
                Listas = new NotaFiscal().ObterEntreData( DataInicial, DataFinal, _paramBase);
            }
            else
            {
                Listas = new NotaFiscal().ObterEntreDataVencto(DataInicial, DataFinal, _paramBase);
            }

            if (pessoa != "")
            {
                Listas = Listas.Where(p => p.NotaFiscalPessoaTomador.razao == pessoa).ToList();
            }

            if (status != 0)
            {
                Listas = Listas.Where(p => p.SituacaoRecebimento == status).ToList();
            }

            if (situacao != 0)
            {
                if (situacao == 1)
                {
                    Listas = Listas.Where(p => p.situacaoPrefeitura_id != 3 && p.situacaoPrefeitura_id != 4).ToList();
                }
                if (situacao == 2)
                {
                    Listas = Listas.Where(p => p.situacaoPrefeitura_id == 3 || p.situacaoPrefeitura_id == 4).ToList();
                }
            }


            return Json(
                Listas.Select(p => new
                    {
                        p.NotaFiscalPessoaTomador.razao,
                        numeroNfse = p.numeroNfse.ToString(),
                        p.numeroRps,
                        dataEmissaoNfse = (p.dataEmissaoNfse == null) ? "" : p.dataEmissaoNfse.ToShortDateString(),
                        dataVencimentoNfse = (p.dataVencimentoNfse == null) ? "" : p.dataVencimentoNfse.ToShortDateString(),
                        p.valorNfse,
                        p.valorLiquido,
                        situacao = Situacao(p.SituacaoRecebimento),
                        status = Status(p.situacaoPrefeitura_id),
                        recebido = (p.Recebimentos.Count() > 0) ? Math.Round(p.Recebimentos.Sum(e => e.valorRecebimento),2) : 0,
                        saldo = (p.Recebimentos.Count() > 0) ? p.valorLiquido - Math.Round(p.Recebimentos.Sum(e => e.valorRecebimento), 2) : p.valorLiquido,
                        p.valorISS,
                        p.valorINSS,
                        p.valorDeducoes,
                        p.pisRetido,
                        p.cofinsRetida,
                        p.csllRetida,
                        p.irrf
                       

                    }
                )
                , JsonRequestBehavior.AllowGet
            );

        }

        public string Situacao(int situacao)
        {
            switch (situacao)
            {
                case 1:
                    return "1 - Em Aberto";
 
                case 2:
                    return "2 - Recebido Parcialmente";

                case 3:
                    return "3 - Recebido Integralmente";
 
                default:
                    return "--------------------------";
            }
        }

        public string Status(int situacao)
        {
            switch (situacao)
            {
                case NotaFiscal.RPS_NF_EMITIDANAOENVIADA:
                    return NotaFiscal.RPSEMITIDA_TEXTO;

                case NotaFiscal.NFGERADAENVIADA:
                    return NotaFiscal.NFSEGERADA_TEXTO;

                case NotaFiscal.NFCANCELADAEMCONF:
                    return NotaFiscal.NFSECANCELADAEMCONF_TEXTO;

                case NotaFiscal.NFCANCELADACCONF:
                    return NotaFiscal.NFSECANCELADACCONF_TEXTO;

                case NotaFiscal.NFBAIXA:
                    return NotaFiscal.NFSEBAIXADA_TEXTO;

                case NotaFiscal.NFAVULSA:
                    return NotaFiscal.NFAVULSA_TEXTO;

                default:
                    return "--------------------------";

            }
        }

    }
}