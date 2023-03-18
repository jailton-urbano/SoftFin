using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioListaCPAGController : BaseController
    {
        private void CarregaViewData()
        {
            ViewData["TipoPessoa"] = new SelectList(new TipoPessoa().ObterTodos(_paramBase), "id", "Descricao");
            ViewData["Pessoa"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["unidade"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");

            //Monta Lista Tipos de Data
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "1", Text = "Data Documento" });
            items.Add(new SelectListItem() { Value = "2", Text = "Data Prevista Pagar" });
            items.Add(new SelectListItem() { Value = "3", Text = "Data Vencimento" });
            ViewData["DataFiltro"] = new SelectList(items, "Value", "Text");

            //Monta Lista Status de Pagamento
            var items2 = new List<SelectListItem>();
            items2.Add(new SelectListItem() { Value = "1", Text = "Em Aberto" });
            items2.Add(new SelectListItem() { Value = "2", Text = "Pago" });
            ViewData["StatusPagamento"] = new SelectList(items2, "Value", "Text");


        }
        // GET: RelatorioListaCPAG
        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }

        public JsonResult geraListaContasAPagar(string dataInicial, string dataFinal, int pessoa, int tipoPessoa, int status, int dataFiltro, int unidade)
        {
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var cp = new List<ListaContasAPagar>();

            var objs = new List<DocumentoPagarParcela>();

            if (dataFiltro == 1)
            {
                objs = new DocumentoPagarParcela().ObterEntreData(DataInicial, DataFinal, _paramBase);
            }
            else if (dataFiltro == 2)
            {
                objs = new DocumentoPagarParcela().ObterEntreDataVencimento(DataInicial, DataFinal, _paramBase);
            }
            else if (dataFiltro == 3)
            {
                objs = new DocumentoPagarParcela().ObterEntreDataVencimentoOriginal(DataInicial, DataFinal, _paramBase);
            }

            if (pessoa != 0)
            {
                objs = objs.Where(p => p.DocumentoPagarMestre.pessoa_id == pessoa).ToList();
            }
            if (status != 0)
            {
                objs = objs.Where(p => p.statusPagamento == status).ToList();
            }
            if (tipoPessoa != 0)
            {
                objs = objs.Where(p => p.DocumentoPagarMestre.Pessoa.TipoPessoa.id == tipoPessoa).ToList();
            }
            if (unidade != 0)
            {
                objs = objs.Where(p => p.DocumentoPagarMestre.DocumentoPagarDetalhes.Where(e => e.unidadenegocio_id == unidade).Count() > 0).ToList();
            }


            //Fim do Case dataFiltro
            Pagamento pagamento = new Pagamento();



            foreach (var item3 in objs.OrderBy(p => p.vencimentoPrevisto))
            {
                var item = new ListaContasAPagar();
                item.fornecedor = item3.DocumentoPagarMestre.Pessoa.nome;
                item.tipoFornecedor = item3.DocumentoPagarMestre.Pessoa.TipoPessoa.Descricao;
                item.cnpj = item3.DocumentoPagarMestre.Pessoa.cnpj;
                item.dataDocumentoS = item3.DocumentoPagarMestre.dataDocumento.ToString("dd/MM/yyyy");
                item.dataVencimentoOriginalS = item3.vencimento.ToString("dd/MM/yyyy");
                item.dataVencimentoS = item3.vencimento.ToString("dd/MM/yyyy");
                item.numeroDocumentoS = item3.DocumentoPagarMestre.numeroDocumento.ToString();
                item.parcela = item3.parcela;
                item.contaContabil = item3.DocumentoPagarMestre.PlanoDeConta.descricao;
                if (item3.statusPagamento == 1)
                    item.statusPagamentoS = "Em Aberto";
                if (item3.statusPagamento == 2)
                    item.statusPagamentoS = "Pago Parcial";
                if (item3.statusPagamento == 3)
                    item.statusPagamentoS = "Pago Total";
                item.valorPago = pagamento.ObterValorPagoDocumento(item.id, _paramBase);
                item.saldo = item3.DocumentoPagarMestre.valorBruto - item.valorPago;

                if (item.saldo <= 0)
                    item.saldo = 0;

                foreach (var item2 in item3.DocumentoPagarMestre.DocumentoPagarDetalhes)
                {
                    item.unidade += item2.UnidadeNegocio.unidade + ";";
                }
                cp.Add(item);
            }


            return Json(cp, JsonRequestBehavior.AllowGet);
        }

    }
}