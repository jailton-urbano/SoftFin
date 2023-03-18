using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System.Data.Entity;

namespace SoftFin.Web.Controllers
{
    public class RelatorioFluxoCaixaEconomicoController : BaseController
    {
        List<PlanoDeConta> _planoContas = new List<PlanoDeConta>();
        // DRE2 - Demonstração do Resultado com Drill Down
        public ActionResult Index()
        {
            return View();
        }

        
        public JsonResult geraDRE2(string dataInicial, string dataFinal, string consolidado, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var contas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo);

            var bcomov = new BancoMovimento().ObterTodosJoinCPAGDataConsolidado(DataInicial, DataFinal, consolidado, _paramBase);

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y => y.data >= DataInicial & y.data <= DataFinal).ToList();

            TipoMovimento tm = new TipoMovimento();

            Banco bc = new Banco();
            _estab = _paramBase.estab_id;

            if (ov == "true")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();
                    var bancoprincipal = bc.ObterTodos(_paramBase).Where(x => x.estabelecimento_id == _estab & x.principal == true).FirstOrDefault();
                    if (bancoprincipal == null)
                        return Json(new { CDStatus = "NOK", DSMessage = "Banco principal não configurado." }, JsonRequestBehavior.AllowGet);

                    bm.banco_id = bancoprincipal.id;
                    bm.data = item.data;
                    bm.historico = "Ordem de Venda: " + item.Numero;
                    bm.PlanoDeConta = new PlanoDeConta().ObterTodos().Where(x => x.id == 1).FirstOrDefault();
                    bm.planoDeConta_id = 1;
                    bm.valor = item.valor;
                    bm.tipoDeMovimento_id = tm.ObterTodos(_paramBase).Where(x => x.Codigo == "ENT").FirstOrDefault().id;
                    bm.TipoMovimento = tm.ObterPorId(bm.tipoDeMovimento_id, _paramBase);
                    bcomov.Add(bm);
                }
            }

            bcomov = bcomov.OrderBy(z => z.data).ToList();

            //Totalizadores
            decimal resultadoFinal = 0;
            decimal valorPagamentos = 0;
            decimal recebimentos = 0;
            decimal perc = 0;
            decimal percCustos = 0;
            decimal percDespesas = 0;
            decimal subtotal = 0;
            decimal custos = 0;
            decimal despesas = 0;


            List<DRE2> listDRE = new List<DRE2>();

            var contaAnt = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo).Where(y => y.TipoConta == "S").FirstOrDefault();

            recebimentos = bcomov.Where(x => x.PlanoDeConta.codigo.Substring(0, 2) == "01").Sum(y => y.valor);

            foreach (var item_contas in contas)
            {
                subtotal += valorPagamentos;
                valorPagamentos = 0;

                if (item_contas.TipoConta == "S")
                {
                    if (contaAnt.codigo != item_contas.codigo)
                    {
                        if (recebimentos != 0)
                        {
                            perc = subtotal / recebimentos;
                        }

                        if (contaAnt.codigo != "03" & contaAnt.codigo != "04" & subtotal != 0)
                        {
                            if (contaAnt.codigo == "01")
                            {
                                perc = 1;
                            }
                            listDRE.Add(new DRE2()
                            {

                                codigoConta = contaAnt.codigo,
                                tipoConta = contaAnt.TipoConta,
                                descricaoConta = contaAnt.descricao,
                                valor = subtotal,
                                percentual = perc
                            });
                        }

                        subtotal = 0;
                        contaAnt.codigo = item_contas.codigo;
                        contaAnt.TipoConta = item_contas.TipoConta;
                        contaAnt.descricao = item_contas.descricao;
                    }
                }
                foreach (var item in bcomov.Where(x => x.planoDeConta_id == item_contas.id))
                {
                    if (item.TipoMovimento.Codigo == "ENT")
                    {
                        resultadoFinal += item.valor;
                        valorPagamentos += item.valor;
                    }
                    else if (item.TipoMovimento.Codigo == "SAI")
                    {
                        resultadoFinal -= item.valor;
                        valorPagamentos -= item.valor;
                    }
                }

                if (valorPagamentos != 0)
                {
                    if (recebimentos != 0)
                    {
                        perc = valorPagamentos / recebimentos;
                    }
                    listDRE.Add(new DRE2()
                    {
                        codigoConta = item_contas.codigo,
                        tipoConta = item_contas.TipoConta,
                        descricaoConta = item_contas.descricao,
                        valor = valorPagamentos,
                        percentual = perc
                    });

                    // Acumula Total de Custos
                    if (item_contas.codigo.Substring(0, 2) == "03")
                    {
                        custos += valorPagamentos;
                    }

                    // Acumula Total de Despesas
                    if (item_contas.codigo.Substring(0, 2) == "04")
                    {
                        despesas += valorPagamentos;
                    }

                }
            }
            if (recebimentos != 0)
            {
                perc = valorPagamentos / recebimentos;
            }

            listDRE.Add(new DRE2()
            {

                codigoConta = contaAnt.codigo,
                tipoConta = contaAnt.TipoConta,
                descricaoConta = contaAnt.descricao,
                valor = valorPagamentos,
                percentual = perc
            });

            if (recebimentos != 0)
            {
                perc = resultadoFinal / recebimentos;
                percCustos = custos / recebimentos;
                percDespesas = despesas / recebimentos;
            }


            //Total de Custos
            listDRE.Add(new DRE2()
            {

                codigoConta = "03",
                tipoConta = "S",
                descricaoConta = "TOTAL DOS CUSTOS",
                valor = custos,
                percentual = percCustos
            });

            //Total de Despesas
            listDRE.Add(new DRE2()
            {

                codigoConta = "04",
                tipoConta = "S",
                descricaoConta = "TOTAL DAS DESPESAS",
                valor = despesas,
                percentual = percDespesas
            });

            //Total Geral
            listDRE.Add(new DRE2()
            {

                codigoConta = "99",
                tipoConta = "S",
                descricaoConta = "RESULTADO FINAL",
                valor = resultadoFinal,
                percentual = perc
            });
            //_resultadoFinal = resultadoFinal;

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listDRE);

            return Json(new { CDStatus = "OK", List = listDRE }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult dreDrillDown(string dataInicial, string dataFinal, string conta, string consolidado, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var bcomov = new BancoMovimento().ObterTodosJoinCPAGDataConsolidado(DataInicial, DataFinal, consolidado, _paramBase).Where(x => x.PlanoDeConta.codigo == conta).OrderBy(y => y.data).ToList();

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y => y.data >= DataInicial & y.data <= DataFinal).ToList();

            TipoMovimento tm = new TipoMovimento();

            Banco bc = new Banco();
            _estab = _paramBase.estab_id;

            if (ov == "true" & conta.Substring(0, 2) == "01")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();
                    bm.banco_id = bc.ObterTodos(_paramBase).Where(x => x.estabelecimento_id == _estab & x.principal == true).FirstOrDefault().id;
                    bm.data = item.data;
                    bm.historico = "Ordem de Venda: " + item.Numero;
                    bm.notafiscal_id = item.pessoas_ID;
                    bm.PlanoDeConta = new PlanoDeConta().ObterTodos().Where(x => x.id == 1).FirstOrDefault();
                    bm.planoDeConta_id = 1;
                    bm.valor = item.valor;
                    bm.tipoDeMovimento_id = tm.ObterTodos(_paramBase).Where(x => x.Codigo == "ENT").FirstOrDefault().id;
                    bm.TipoMovimento = tm.ObterPorId(bm.tipoDeMovimento_id, _paramBase);
                    bcomov.Add(bm);
                }
            }

            bcomov = bcomov.OrderBy(z => z.data).ToList();


            List<DRE_detalhe> listDRE = new List<DRE_detalhe>();

            foreach (var item in bcomov)
            {
                Decimal v_valor = 0;
                if (item.TipoMovimento.Codigo == "ENT")
                {
                    v_valor = item.valor;
                }
                if (item.TipoMovimento.Codigo == "SAI")
                {
                    v_valor = item.valor * -1;
                }

                var varPessoa = "";

                if (item.NotaFiscal != null && item.valor > 0)
                {
                    varPessoa = item.NotaFiscal.NotaFiscalPessoaTomador.razao;
                }
                if (item.Recebimento != null)
                {
                    varPessoa = item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.razao;
                }

                if (item.DocumentoPagarParcela != null)
                {
                    varPessoa = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
                }

                if (item.Pagamento != null)
                {
                    varPessoa = item.Pagamento.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
                }

                Pessoa ps = new Pessoa();
                if (item.historico.Length > 15)
                {
                    if (item.historico.Substring(0, 15) == "Ordem de Venda:")
                    {
                        int id = item.notafiscal_id.GetValueOrDefault();
                        varPessoa = ps.ObterPorId(id, _paramBase).nome;
                    }
                }

                if (item.valor > 0)
                {
                    listDRE.Add(new DRE_detalhe()
                    {
                        data = (item.DocumentoPagarParcela != null) ? 
                                    item.DocumentoPagarParcela.DocumentoPagarMestre.dataDocumento.ToShortDateString() 
                                    : item.data.ToShortDateString(),
                        pessoa = varPessoa,
                        historico = item.historico,
                        valor = v_valor
                    });
                }
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listDRE);

            return Json(listDRE, JsonRequestBehavior.AllowGet);

        }
        private decimal ContaSub(List<BancoMovimento> bcomov, PlanoDeConta item, int mes)
        {
            var planoDeContasSub = _planoContas.Where(p => p.nivelSuperior == item.id);
            if (planoDeContasSub.Count() == 0)
                return 0;

            decimal planoTotalSub = 0;
            foreach (var itemSub in planoDeContasSub)
            {
                planoTotalSub += bcomov.Where(p => p.data.Month == mes && p.planoDeConta_id == itemSub.id).Sum(p => p.valor) * ((itemSub.DebitoCredito == "D")? -1 : 1);
                planoTotalSub += ContaSub(bcomov, itemSub, mes);
            }
            return planoTotalSub;
        }

        private void PlanoInferior(List<BancoMovimento> bcomov, int idPlano, List<DRE3> listDRE3)
        {
            var planoDeContasItens = _planoContas.Where(p => p.nivelSuperior == idPlano);

            if (planoDeContasItens.Count() == 0)
                return;

            

            foreach (var itemTemp in planoDeContasItens)
            {
                var itemDRETemp = new DRE3();

                for (int mes = 1; mes <= 12; mes++)
                {
                    decimal valorplano = bcomov.Where(p => p.data.Month == mes && p.planoDeConta_id == itemTemp.id).Sum(p => p.valor) * ((itemTemp.DebitoCredito == "D") ? -1 : 1); ;
                    var planoTotalSub = ContaSub(bcomov, itemTemp, mes);
                    valorplano += planoTotalSub;
                    itemDRETemp.valor.Add(valorplano);
                }

                itemDRETemp.valor.Add(itemDRETemp.valor.Sum(p => p));

                if (itemDRETemp.valor.Last() != 0)
                {
                    itemDRETemp.codigoConta = itemTemp.codigo;
                    itemDRETemp.descricaoConta = itemTemp.descricao;
                    itemDRETemp.tipoConta = itemTemp.TipoConta;

                    //Calcula Porcentagem 
                    foreach (var itemPorc in itemDRETemp.valor)
                    {
                        if (itemPorc == 0)
                        {
                            itemDRETemp.percentual.Add(0);
                        }
                        else
                        {
                            var calculo = itemPorc / itemDRETemp.valor.Last();
                            itemDRETemp.percentual.Add(Math.Round(calculo, 2));
                        }
                    }

                    
                    
                    listDRE3.Add(itemDRETemp);


                }


                PlanoInferior(bcomov, itemTemp.id, listDRE3);

            }
        }

    }


}