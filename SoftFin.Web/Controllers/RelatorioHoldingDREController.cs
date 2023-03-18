using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioHoldingDREController : BaseController
    {
        // GET: RelatorioHoldingDRE
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterRelatorio(DateTime dataInicial, DateTime dataFinal, string opc, List<int> estabs)
        {
            var contas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo);

            var bcomov = new BancoMovimento().ObterTodosHoldingDataConsolidado(dataInicial, dataFinal, _paramBase, estabs);

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodosHolding(_paramBase, estabs).
                Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").
                Where(y => y.data >= dataInicial & y.data <= dataFinal).ToList();

            TipoMovimento tm = new TipoMovimento();

            Banco bc = new Banco();
            _estab = _paramBase.estab_id;



            bcomov = bcomov.OrderBy(z => z.data).ToList();

            if (opc == "2")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();
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

            var graficoLabel = new List<string>();
            var graficoData = new List<decimal>();
            var labelsDebito = new List<string>();
            var dataDebito = new List<decimal>();

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
                                stiloValor = (subtotal >= 0 ? "" : ";color: Red") + (contaAnt.TipoConta.Equals("S") ? ";font-weight: bold; background-color:#BCD4E6" : ""),
                                stiloGeral = (contaAnt.TipoConta.Equals("S")) ? ";font-weight: bold; background-color:#BCD4E6" : "",
                                codigoConta = contaAnt.codigo,
                                tipoConta = contaAnt.TipoConta,
                                descricaoConta = contaAnt.descricao,
                                valor = subtotal,
                                percentual = perc
                            });

                            if (contaAnt.codigo.Trim().Length == 2 
                                && contaAnt.codigo != "01" && contaAnt.codigo != "05" && contaAnt.codigo != "06")
                            {
                                labelsDebito.Add(contaAnt.descricao);
                                dataDebito.Add(subtotal * -1);
                            }
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
                        stiloValor = (valorPagamentos >= 0 ? "" : ";color: Red") + (item_contas.TipoConta.Equals("S") ? ";font-weight: bold; background-color:#BCD4E6" : ""),
                        stiloGeral = item_contas.TipoConta.Equals("S") ? ";font-weight: bold; background-color:#BCD4E6" : "",
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

                    if (item_contas.TipoConta == "A")
                    {
                        if (item_contas.codigo.Substring(0, 2) == "01")
                        {
                            graficoLabel.Add(item_contas.descricao);
                            graficoData.Add(valorPagamentos);
                        }
                    }
                }

            }

            if (recebimentos != 0)
            {
                perc = valorPagamentos / recebimentos;
            }

            if (valorPagamentos != 0)
            {
                listDRE.Add(new DRE2()
                {
                    stiloValor = (valorPagamentos >= 0 ? "" : ";color: Red") + (contaAnt.TipoConta.Equals("S") ? ";font-weight: bold; background-color:#BCD4E6" : ""),
                    stiloGeral = contaAnt.TipoConta.Equals("S") ? ";font-weight: bold; background-color:#BCD4E6" : "",
                    codigoConta = contaAnt.codigo,
                    tipoConta = contaAnt.TipoConta,
                    descricaoConta = contaAnt.descricao,
                    valor = valorPagamentos,
                    percentual = perc
                });
                labelsDebito.Add(contaAnt.descricao);
                dataDebito.Add(valorPagamentos * -1);
            }


            if (recebimentos != 0)
            {
                perc = resultadoFinal / recebimentos;
                percCustos = custos / recebimentos;
                percDespesas = despesas / recebimentos;
            }


            //Total de Custos
            listDRE.Add(new DRE2()
            {
                stiloValor = (custos > 0 ? "; font-weight: bold; background-color:#BCD4E6" : ";color: Red; font-weight: bold; background-color:#BCD4E6"),
                stiloGeral = "font-weight: bold; background-color:#BCD4E6",
                codigoConta = "03",
                tipoConta = "S",
                descricaoConta = "TOTAL DOS CUSTOS",
                valor = custos,
                percentual = percCustos
            });

            labelsDebito.Add("TOTAL DOS CUSTOS");
            dataDebito.Add(custos * -1);

            //Total de Despesas
            listDRE.Add(new DRE2()
            {
                stiloValor = (despesas >= 0 ? "; font-weight: bold; background-color:#BCD4E6" : ";color: Red; font-weight: bold; background-color:#BCD4E6"),
                stiloGeral = "font-weight: bold; background-color:#BCD4E6",
                codigoConta = "04",
                tipoConta = "S",
                descricaoConta = "TOTAL DAS DESPESAS",
                valor = despesas,
                percentual = percDespesas
            });

            labelsDebito.Add("TOTAL DAS DESPESAS");
            dataDebito.Add(despesas * -1);
  

            //Total Geral
            listDRE.Add(new DRE2()
            {
                stiloGeral = "font-weight: bold; background-color:#BCD4E6",
                stiloValor = (resultadoFinal >= 0 ? "; font-weight: bold; background-color:#BCD4E6" : ";color: Red; font-weight: bold; background-color:#BCD4E6"),
                codigoConta = "99",
                tipoConta = "S",
                descricaoConta = "RESULTADO FINAL",
                valor = resultadoFinal,
                percentual = perc
            });



            return Json(new { CDStatus = "OK"
                , List = listDRE.OrderBy(p => p.codigoConta).ToList()
                
                , Grafico = new
                {
                    labelsRecebimento = graficoLabel,
                    dataRecebimentos = graficoData,
                    labelsDebito = labelsDebito,
                    dataDebito = dataDebito
                },
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult Detalhe(DateTime dataInicial, DateTime dataFinal, string opc, string conta, List<int> estabs)
        {


            var bcomov = new BancoMovimento().ObterTodosHoldingDataConsolidado(dataInicial, dataFinal, _paramBase, estabs).Where(x => x.PlanoDeConta.codigo.StartsWith(conta)).OrderBy(y => y.data).ToList();

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodosHolding(_paramBase, estabs).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                      y => y.data >= dataInicial & y.data <= dataFinal).ToList();

            TipoMovimento tm = new TipoMovimento();

            Banco bc = new Banco();
            _estab = _paramBase.estab_id;

            if (opc == "2" & conta.Substring(0, 2) == "01")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();

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
                        empresa_det = "",
                        data = item.data.ToString("o"),
                        pessoa = varPessoa,
                        historico = item.historico,
                        valor = v_valor
                    });
                }   
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listDRE);

            return Json(listDRE, JsonRequestBehavior.AllowGet);

        }

    }
}