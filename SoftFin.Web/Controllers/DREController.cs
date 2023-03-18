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
    public class DREController : BaseController
    {

        decimal _resultadoFinal = 0;

        public JsonResult TotalizadorResultadoCaixa(int? id)
        {
            base.TotalizadorDash(id);
            geraDRE2(DataInicial.ToString(), DataFinal.ToString(), "true", "false");

            var soma = _resultadoFinal.ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }
        

        [HttpPost]
        public JsonResult TotalizadorGrafico(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }
        
                    [HttpPost]
        public JsonResult TotalizadorNotasEmitidas(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new NotaFiscal().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.entradaSaida == "S").Sum(x => x.valorNfse).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }


        // DRE - Demonstração do Resultado
        public ActionResult DRE()
        {
            return View();
        }
        [HttpPost]
        public JsonResult geraDRE(string dataInicial, string dataFinal)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var contas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo);
            var bcomov = new BancoMovimento().ObterTodosData(DataInicial, DataFinal, _paramBase);
            //var ov = new OrdemVenda().ObterTodosData(DataInicial, DataFinal);

            //Totalizadores
            decimal resultadoFinal = 0;
            decimal valorPagamentos = 0;
            decimal recebimentos = 0;
            decimal perc = 0;

            List<DRE> listDRE = new List<DRE>();

            foreach (var item_contas in contas)
            {

                valorPagamentos = 0;

                if (item_contas.codigo == "01")
                {
                    foreach (var item in bcomov.Where(x => x.planoDeConta_id == item_contas.id))
                    {
                        valorPagamentos += item.valor;
                    }
                    recebimentos = valorPagamentos;
                    resultadoFinal += valorPagamentos;
                }
                else
                {
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
                }
                if (valorPagamentos != 0)
                {
                    if (recebimentos > 0)
                    {
                        perc = valorPagamentos / recebimentos * 100;
                    }
                    listDRE.Add(new DRE()
                    {

                        codigoConta = item_contas.codigo,
                        descricaoConta = item_contas.descricao,
                        valor = valorPagamentos.ToString("N"),
                        percentual = perc.ToString("N")
                    });
                }
            }
            if (recebimentos > 0)
            {
                perc = resultadoFinal / recebimentos * 100;
            }
            listDRE.Add(new DRE()
            {

                codigoConta = "",
                descricaoConta = "",
                valor = "",
                percentual = ""
            });

            listDRE.Add(new DRE()
            {

                codigoConta = "99",
                descricaoConta = "Resultado Final",
                valor = resultadoFinal.ToString("N"),
                percentual = perc.ToString("N")
            });

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listDRE);

            return Json(listDRE, JsonRequestBehavior.AllowGet);
        }

        // DRE2 - Demonstração do Resultado com Drill Down
        public ActionResult DRE2()
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

            var bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataFinal, consolidado,_paramBase);

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
                        return Json(new { CDStatus = "NOK", DSMessage = "Banco principal não configurado."} , JsonRequestBehavior.AllowGet);

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

            recebimentos = bcomov.Where(x => x.PlanoDeConta.codigo.Substring(0,2) == "01").Sum(y=>y.valor);

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
            _resultadoFinal = resultadoFinal;

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listDRE);

            return Json(new { CDStatus = "OK", List = listDRE}, JsonRequestBehavior.AllowGet);
        }
        public JsonResult dreDrillDown(string dataInicial, string dataFinal, string conta, string consolidado, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataFinal, consolidado,_paramBase).Where(x => x.PlanoDeConta.codigo == conta).OrderBy(y => y.data).ToList();

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y => y.data >= DataInicial & y.data <= DataFinal).ToList();

            TipoMovimento tm = new TipoMovimento();

            Banco bc = new Banco();
            _estab = _paramBase.estab_id;

            if (ov == "true" & conta.Substring(0,2) == "01")
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
                        data = item.data.ToShortDateString(),
                        pessoa = varPessoa,
                        historico = item.historico,
                        valor = v_valor
                    });
                }
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listDRE);

            return Json(listDRE, JsonRequestBehavior.AllowGet);

        }

        //Indicador de performance de Caixa
        public ActionResult performanceCaixa()
        {
            return View();
        }
        public JsonResult geraPerformanceCaixa(string dataInicial, string dataFinal, string consolidado)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataFinal, consolidado,_paramBase);

            //Totalizadores
            decimal vRecebimentos = 0;
            decimal vTotalDosCustos = 0;
            decimal vPercCustos = 0;
            decimal vPercMercado = 0;
            decimal vPercVariação = 0;
            decimal vTotalDespesasComerciais = 0;
            decimal vTotalDespesasAdministrativas = 0;
            string cta = "";


            List<PerformanceCaixa> listPerformanceCaixa = new List<PerformanceCaixa>();

            //Total dos Custos
            vRecebimentos = bcomov.Where(x => x.PlanoDeConta.codigo.Substring(0, 2) == "01").Sum(y => y.valor);
            vTotalDosCustos = bcomov.Where(x => x.PlanoDeConta.codigo.Substring(0, 2) == "03").Sum(y => y.valor);
            vPercCustos = vTotalDosCustos / vRecebimentos * 100;
            vPercMercado = 50.0m;
            vPercVariação = vPercMercado - vPercCustos;

                            listPerformanceCaixa.Add(new PerformanceCaixa()
                            {
                                conta = "Total dos Custos",
                                percRecebimentos = vPercCustos/100,
                                percMercado = vPercMercado / 100,
                                percVariacao = vPercVariação,
                                percVariacaoS = vPercVariação.ToString("0.00")
                            });


            // Acumula lançamentos em despesas comerciais e administrativas
            foreach (var reg in bcomov)
            {
                cta = reg.PlanoDeConta.codigo.ToString();

                if (cta.Length > 4)
                {
                    // Despesas Comerciais
                    if (reg.PlanoDeConta.codigo.Substring(0, 5) == "04.01")
                    {
                        vTotalDespesasComerciais += reg.valor;
                    }

                    // Despesas Administrativas
                    if (reg.PlanoDeConta.codigo.Substring(0, 5) == "04.02")
                    {
                        vTotalDespesasAdministrativas += reg.valor;
                    }

                }
            }


            //Despesas Comerciais
            vRecebimentos = bcomov.Where(x => x.PlanoDeConta.codigo.Substring(0, 2) == "01").Sum(y => y.valor);
            vPercCustos = vTotalDespesasComerciais / vRecebimentos * 100;
            vPercMercado = 10.0m;
            vPercVariação = vPercMercado - vPercCustos;

            listPerformanceCaixa.Add(new PerformanceCaixa()
            {
                conta = "Despesas Comerciais",
                percRecebimentos = vPercCustos / 100,
                percMercado = vPercMercado / 100,
                percVariacao = vPercVariação,
                percVariacaoS = vPercVariação.ToString("0.00")
            });

            //Despesas Administrativas
            vRecebimentos = bcomov.Where(x => x.PlanoDeConta.codigo.Substring(0, 2) == "01").Sum(y => y.valor);
            vPercCustos = vTotalDespesasAdministrativas / vRecebimentos * 100;
            vPercMercado = 15.0m;
            vPercVariação = vPercMercado - vPercCustos;

            listPerformanceCaixa.Add(new PerformanceCaixa()
            {
                conta = "Despesas Administrativas",
                percRecebimentos = vPercCustos / 100,
                percMercado = vPercMercado / 100,
                percVariacao = vPercVariação,
                percVariacaoS = vPercVariação.ToString("0.00")
            });


            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listPerformanceCaixa);

            return Json(listPerformanceCaixa, JsonRequestBehavior.AllowGet);
        }

        // Movimento Bancário Agrupado
        public ActionResult movimentoBancario()
        {
            var con1 = new Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();

            var hoje = DateTime.Now;
            ViewData["DataInicial"] = new DateTime(hoje.Year, hoje.Month, 1).ToShortDateString();
            ViewData["DataFinal"] = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();

            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            ViewData["banco"] = listret;

            return View();
        }
        public ActionResult movimentoBancarioDiario()
        {
            var hoje = DateTime.Now;
            ViewData["DataInicial"] = new DateTime(hoje.Year, hoje.Month, 1).ToShortDateString();
            ViewData["DataFinal"] = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();
            var con1 = new Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            ViewData["banco"] = listret;

            return View();
        }

        // Movimento Bançario Detalhado
        [HttpPost]
        public JsonResult geraMovimentoBancario(string dataInicial, string dataFinal, int banco, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            //Totalizadores
            decimal varSaldoFinal = 0;
            decimal varSaldoInicialAno = 0;
            decimal varSaldoInicial = 0;
            string varConciliado = "";

            //Obtém Saldo Inicial do Banco
            string varAno = Convert.ToDateTime(dataInicial).Year.ToString();
            DateTime dataInicialAno = Convert.ToDateTime("01/01/" + varAno);
            DateTime dataAnteriorMovimento = Convert.ToDateTime(dataInicial).AddDays(-1);
            SaldoBancarioInicial saldo = new SaldoBancarioInicial();
            if (saldo.ObterTodos(_paramBase).Where(x => x.Ano == varAno && x.banco_id == banco).FirstOrDefault() != null)
            {
                varSaldoInicialAno = saldo.ObterTodos(_paramBase).Where(x => x.Ano == varAno && x.banco_id == banco).FirstOrDefault().saldoInicial;
            }
            else
            {
                varSaldoInicialAno = 0;
            }
            var bcomovAnterior = new BancoMovimento().ObterTodosDataBanco(dataInicialAno, dataAnteriorMovimento, banco, _paramBase);
            varSaldoInicial = varSaldoInicialAno;
            foreach (var item in bcomovAnterior)
            {
                if (item.TipoMovimento.Codigo == "ENT")
                {
                    varSaldoInicial += item.valor;
                }
                else if (item.TipoMovimento.Codigo == "SAI")
                {
                    varSaldoInicial -= item.valor;
                }
            }

            //Movimento Bancário do período solicitado
            var contas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo);
            var bcomov = new BancoMovimento().ObterTodosDataBanco(DataInicial, DataFinal, banco, _paramBase);
            List<movimentoBancario> listMovimentoBancario = new List<movimentoBancario>();

            listMovimentoBancario.Add(new movimentoBancario()
            {
                dataMovimento = Convert.ToString(dataInicial),
                contaContabil = "",
                pessoa = "",
                historicoMovimento = "Saldo Inicial",
                referencia = "",
                tipoMovimento = "",
                valorMovimento = "",
                saldoFinal = varSaldoInicial.ToString("N"),
                conciliado = ""

            });

            varSaldoFinal = varSaldoInicial;

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y => y.data >= DataInicial & y.data <= DataFinal).ToList();

            TipoMovimento tm = new TipoMovimento();

            if (ov == "true")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();
                    bm.banco_id = banco;
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

            if (bcomov.Count() > 0)
            {

                foreach (var item in bcomov)
                {

                    if (item.TipoMovimento.Codigo == "ENT")
                    {
                        varSaldoFinal += item.valor;
                    }
                    else if (item.TipoMovimento.Codigo == "SAI")
                    {
                        varSaldoFinal -= item.valor;
                    }

                    //Verificar se existe concialição para o movimento
                    DbControle db = new DbControle();
                    BancoMovimentoLanctoExtrato bvle = new BancoMovimentoLanctoExtrato();
                    if (bvle.ObterBancoMovimento_id(item.id, db).ToList().FirstOrDefault() != null)
                    {
                        varConciliado = "S";
                    }
                    else
                    {
                        varConciliado = "N";
                    }

                    string pessoa = "";
                    string referencia = "";

                    if (item.NotaFiscal != null)
                    {
                        if (item.NotaFiscal.numeroNfse != null)
                        {
                            pessoa = item.NotaFiscal.NotaFiscalPessoaTomador.razao;
                            referencia = item.NotaFiscal.discriminacaoServico;

                        }
                    }
                    else if (item.Recebimento != null)
                    {
                        pessoa = item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.razao;
                        referencia = item.Recebimento.historico;
                    }

                    if (item.DocumentoPagarParcela != null)
                    {
                        pessoa = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
                        referencia = item.DocumentoPagarParcela.historico;
                    }
                    else if (item.Pagamento != null)
                    {
                        pessoa = item.Pagamento.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
                        referencia = item.Pagamento.historico;
                    }


                    if (string.IsNullOrWhiteSpace(pessoa))
                    {
                        var nd = new NotadeDebito().ObterPorBM(item.id,_paramBase,null);
                        if (nd != null)
                        {
                            pessoa = nd.Cliente.nome;
                            referencia = nd.descricao;
                        }
                    }

                    Pessoa ps = new Pessoa();
                    if (item.historico.Length > 15)
                    {
                        if (item.historico.Substring(0, 15) == "Ordem de Venda:")
                        {
                            int id = item.notafiscal_id.GetValueOrDefault();
                            pessoa = ps.ObterPorId(id, _paramBase).nome;

                        }
                    }

                    listMovimentoBancario.Add(new movimentoBancario()
                    {
                        dataMovimento = item.data.ToShortDateString(),
                        contaContabil = item.PlanoDeConta.descricao,
                        pessoa = pessoa,
                        historicoMovimento = item.historico,
                        tipoMovimento = item.TipoMovimento.Descricao,
                        valorMovimento = item.valor.ToString("N"),
                        referencia= referencia,
                        saldoFinal = varSaldoFinal.ToString("N"),
                        conciliado = varConciliado

                    });
                }
            }
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listMovimentoBancario);

            return Json(listMovimentoBancario, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult geraMovimentoBancarioDiario(string dataInicial, string dataFinal, int banco, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            //Totalizadores
            decimal varSaldoFinal = 0;
            decimal varSaldoInicialAno = 0;
            decimal varSaldoInicial = 0;
            decimal varValorEntradas = 0;
            decimal varValorSaidas = 0;
            decimal varSaldoReal = 0;
            decimal varDiferenca = 0;
            int varBanco = 0;

            //Obtém Saldo Inicial do Banco
            string varAno = Convert.ToDateTime(dataInicial).Year.ToString();
            DateTime dataInicialAno = Convert.ToDateTime("01/01/" + varAno);
            DateTime dataAnteriorMovimento = Convert.ToDateTime(dataInicial).AddDays(-1);
            SaldoBancarioInicial saldo = new SaldoBancarioInicial();
            //varSaldoInicialAno = saldo.ObterTodos().Where(x => x.Ano == varAno).FirstOrDefault().saldoInicial;

            if (saldo.ObterTodos(_paramBase).Where(x => x.Ano == varAno && x.banco_id == banco).FirstOrDefault() != null)
            {
                varSaldoInicialAno = saldo.ObterTodos(_paramBase).Where(x => x.Ano == varAno && x.banco_id == banco).FirstOrDefault().saldoInicial;
            }
            else
            {
                varSaldoInicialAno = 0;
            }

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y=> y.data >= DataInicial & y.data <= DataFinal).ToList();

            var bcomovAnterior = new BancoMovimento().ObterTodosDataBanco(dataInicialAno, dataAnteriorMovimento, banco, _paramBase);
            varSaldoInicial = varSaldoInicialAno;
            foreach (var item in bcomovAnterior)
            {
                if (item.TipoMovimento.Codigo == "ENT")
                {
                    varSaldoInicial += item.valor;
                }
                else if (item.TipoMovimento.Codigo == "SAI")
                {
                    varSaldoInicial -= item.valor;
                }
            }

            //Movimento Bancário do período solicitado
            var contas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo);
            List<movimentoBancario> listMovimentoBancario = new List<movimentoBancario>();

            //Obtem saldo Real
            var sdoReal = new SaldoBancarioReal().ObterBancoData(banco, DataInicial.AddDays(-1), _paramBase);
            if (sdoReal != null)
            {
                varSaldoReal = sdoReal.saldoFinal;
                varDiferenca = sdoReal.saldoFinal - varSaldoInicial;
            }
            else
            {
                varSaldoReal = 0;
                varDiferenca = varSaldoInicial;
            }
            listMovimentoBancario.Add(new movimentoBancario()
            {
                dataMovimento = Convert.ToString(dataInicial),
                historicoMovimento = "Saldo Inicial",
                valorMovimento = "",
                saldoFinal = varSaldoInicial.ToString("N"),
                saldoReal = varSaldoReal.ToString("N"),
                diferenca = varDiferenca.ToString("N"),
                valorOvs = ""
            });

            varSaldoFinal = varSaldoInicial;

            var bcomov = new BancoMovimento().ObterTodosDataBanco(DataInicial, DataFinal, banco, _paramBase).OrderBy(z => z.data).ToList();

            TipoMovimento tm = new TipoMovimento();

            if (ov == "true")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();
                    bm.banco_id = banco;
                    bm.data = item.data;
                    bm.historico = item.descricao;
                    bm.valor = item.valor;
                    bm.tipoDeMovimento_id = tm.ObterTodos(_paramBase).Where(x => x.Codigo == "ENT").FirstOrDefault().id;
                    bm.TipoMovimento = tm.ObterPorId(bm.tipoDeMovimento_id, _paramBase);
                    bcomov.Add(bm);
                }
            }

            bcomov = bcomov.OrderBy(z => z.data).ToList();

            if (bcomov.Count() > 0)
            {
                var dataAnterior = new DateTime();
                dataAnterior = bcomov.FirstOrDefault().data.Date;

                foreach (var item in bcomov)
                {
                    varBanco = item.banco_id;
                    if (dataAnterior == item.data.Date)
                    {
                        if (item.TipoMovimento.Codigo == "ENT")
                        {
                            varValorEntradas += item.valor;
                            varSaldoFinal += item.valor;
                        }
                        else if (item.TipoMovimento.Codigo == "SAI")
                        {
                            varSaldoFinal -= item.valor;
                            varValorSaidas += item.valor;
                        }
                    }
                    else
                    {

                        //Obtem saldo Real
                        var sdoR = new SaldoBancarioReal().ObterBancoData(varBanco, dataAnterior, _paramBase);
                        if (sdoR != null)
                        {
                            varSaldoReal = sdoR.saldoFinal;
                            varDiferenca = sdoR.saldoFinal - varSaldoFinal;
                        }
                        else
                        {
                            varSaldoReal = 0;
                            varDiferenca = varSaldoFinal;
                        }
                        listMovimentoBancario.Add(new movimentoBancario()
                        {
                            dataMovimento = dataAnterior.ToShortDateString(),
                            historicoMovimento = "",
                            valorMovimento = (varValorEntradas - varValorSaidas).ToString("N"),
                            saldoFinal = varSaldoFinal.ToString("N"),
                            saldoReal = varSaldoReal.ToString("N"),
                            diferenca = varDiferenca.ToString("N")
                        });
                        dataAnterior = item.data.Date;
                        varValorEntradas = 0;
                        varValorSaidas = 0;

                        if (item.TipoMovimento.Codigo == "ENT")
                        {
                            varSaldoFinal += item.valor;
                            varValorEntradas += item.valor;
                        }
                        else if (item.TipoMovimento.Codigo == "SAI")
                        {
                            varSaldoFinal -= item.valor;
                            varValorSaidas += item.valor;
                        }
                    }
                }
                if (bcomov != null)
                {
                    //Obtem saldo Real
                    var sdoR = new SaldoBancarioReal().ObterBancoData(varBanco, dataAnterior, _paramBase);
                    if (sdoR != null)
                    {
                        varSaldoReal = sdoR.saldoFinal;
                        varDiferenca = sdoR.saldoFinal - varSaldoFinal;

                    }
                    else
                    {
                        varSaldoReal = 0;
                        varDiferenca = varSaldoFinal;
                    }
                    listMovimentoBancario.Add(new movimentoBancario()
                    {
                        dataMovimento = dataAnterior.ToShortDateString(),
                        historicoMovimento = "",
                        valorMovimento = (varValorEntradas - varValorSaidas).ToString("N"),
                        saldoFinal = varSaldoFinal.ToString("N"),
                        saldoReal = varSaldoReal.ToString("N"),
                        diferenca = varDiferenca.ToString("N")
                    });
                }
            }
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listMovimentoBancario);

            return Json(listMovimentoBancario, JsonRequestBehavior.AllowGet);
        }

        // Gráfico Faturamento Google Charts - Notas Fiscais Emitidas
        public ActionResult graficoFaturamento()
        {
            return View();
        }
        public JsonResult geraGraficoFaturamento(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var faturamento = new List<AnoMesFaturamento>();

            faturamento = (from nota in banco.NotaFiscal
                           where nota.dataEmissaoNfse >= DataInicial && nota.dataEmissaoNfse <= DataFinal &&
                           nota.estabelecimento_id == estab
                           group nota by new { ano = nota.dataEmissaoNfse.Year, mes = nota.dataEmissaoNfse.Month }
                               into g
                               select new AnoMesFaturamento
                               {
                                   ano = g.Key.ano,
                                   mes = g.Key.mes,
                                   valor = g.Sum(x => x.valorNfse)
                               }).OrderBy(p => p.ano).ThenBy(q => q.mes).ToList();

            return Json(faturamento, JsonRequestBehavior.AllowGet);
        }

        //Gráfico Faturamento por Tipo de Contrato
        public ActionResult graficoFaturamentoTipoContrato()
        {
            return View();
        }
        public JsonResult geraGraficoFaturamentoTipoContrato(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var faturamento = new List<TipoContratoFaturamento>();

            faturamento = (from nota in banco.NotaFiscal
                           join sts in banco.TipoContrato
                               on nota.OrdemVenda.ParcelaContrato.ContratoItem.tipoContratos_ID equals sts.id
                           where nota.dataEmissaoNfse >= DataInicial && nota.dataEmissaoNfse <= DataFinal &&
                           nota.estabelecimento_id == estab
                           group nota by new { tipo = sts.tipo }
                               into g
                               select new TipoContratoFaturamento
                               {
                                   tipo = g.Key.tipo,
                                   valor = g.Sum(x => x.valorNfse)
                               }).OrderBy(s => s.tipo).ToList();

            return Json(faturamento, JsonRequestBehavior.AllowGet);
        }
        public JsonResult graficoTipoDrillDown(string dataInicial, string dataFinal, string tipo)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var notas = new List<ListaNotas>();

            notas = (from nota in banco.NotaFiscal
                     where nota.dataEmissaoNfse >= DataInicial && nota.dataEmissaoNfse <= DataFinal &&
                     nota.estabelecimento_id == estab && nota.OrdemVenda.ParcelaContrato.ContratoItem.TipoContrato.tipo == tipo
                     select new ListaNotas
                     {
                         numeroNfse = nota.numeroNfse,
                         dataEmissaoNfse = nota.dataEmissaoNfse,
                         dataVencimentoNfse = nota.dataVencimentoNfse,
                         razaoTomador = nota.NotaFiscalPessoaTomador.razao,
                         valorNfse = nota.valorNfse,
                         valorLiquido = nota.valorLiquido
                     }).ToList();

            for (int i = 0; i < notas.Count; i++)
            {
                notas[i].dataEmissaoS = notas[i].dataEmissaoNfse.ToString("dd/MM/yyyy");
                notas[i].dataVencimentoS = notas[i].dataVencimentoNfse.ToString("dd/MM/yyyy");

            };

            return Json(notas, JsonRequestBehavior.AllowGet);

        }

        // Gráfico Previsão de Faturamento Google Charts - Contratos e Parcelas de Contratos
        public ActionResult graficoPrevisaoFaturamento()
        {
            return View();
        }
        public JsonResult geraGraficoPrevisaoFaturamento(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var faturamento = new List<AnoMesFaturamento>();

            //SELECT      YEAR(OrdemVendas.data) AS Ano, 
            //            MONTH(OrdemVendas.data) AS Mes, 
            //            StatusParcelas.status, 
            //            SUM(OrdemVendas.valor) AS Valor
            //FROM        OrdemVendas INNER JOIN
            //            StatusParcelas ON OrdemVendas.statusParcela_ID = StatusParcelas.id
            //WHERE       (OrdemVendas.estabelecimento_id = @Estab)
            //GROUP BY YEAR(OrdemVendas.data), MONTH(OrdemVendas.data), StatusParcelas.status
            //ORDER BY Ano, Mes

            faturamento = (from ordem in banco.OrdemVenda
                           join sts in banco.StatusParcela
                               on ordem.statusParcela_ID equals sts.id
                           where ordem.data >= DataInicial && ordem.data <= DataFinal &&
                           ordem.estabelecimento_id == estab
                           group ordem by new { ano = ordem.data.Year, mes = ordem.data.Month, status = sts.status }
                               into g
                               select new AnoMesFaturamento
                               {
                                   ano = g.Key.ano,
                                   mes = g.Key.mes,
                                   status = g.Key.status,
                                   valor = g.Sum(x => x.valor)
                               }).OrderBy(p => p.ano).ThenBy(q => q.mes).ThenBy(s => s.status).ToList();

            return Json(faturamento, JsonRequestBehavior.AllowGet);
        }

        // Gráfico Saldo de Caixa Google Charts
        public ActionResult graficoSaldoCaixa()
        {
            return View();
        }
        public JsonResult geraGraficoSaldoCaixa(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int empresa  = _paramBase.empresa_id;
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var db = new DbControle();
            var saldoReal = new List<SaldoBanco>();

            saldoReal = (from saldo in db.SaldoBancarioReal
                         join banco in db.Bancos
                           on saldo.banco_id equals banco.id
                         where saldo.dataSaldo >= DataInicial && saldo.dataSaldo <= DataFinal &&
                         saldo.Banco.estabelecimento_id == estab
                         group saldo by new { saldo.dataSaldo, banco = banco.codigoBanco }
                             into g
                             select new SaldoBanco
                             {
                                 data = g.Key.dataSaldo,
                                 codigoBanco = g.Key.banco,
                                 valor = g.Sum(x => x.saldoFinal)
                             }).OrderBy(p => p.data).ThenBy(q => q.codigoBanco).ToList();

            List<Banco> contas = new Banco().ObterTodos(_paramBase).Where(x => x.estabelecimento_id == estab).OrderBy(d => d.codigoBanco).ToList();

            for (int i = 0; i < saldoReal.Count; i++)
            {
                saldoReal[i].dataS = saldoReal[i].data.ToString("dd-MM");
            };

            //return Json(saldoReal, JsonRequestBehavior.AllowGet);
            return Json(new
            {
                Saldo = saldoReal,
                Banco = contas.Select(p => new
                {
                    p.agencia,
                    p.nomeBanco,
                    p.codigoBanco
                })
            }, JsonRequestBehavior.AllowGet);
        }

        // Lista Notas Emitidas Google Charts
        public ActionResult listaNotasEmitidas()
        {
            return View();
        }
        public JsonResult geraListaNotasEmitidas(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var notas = new List<ListaNotas>();

            notas = (from nota in banco.NotaFiscal
                     where nota.dataEmissaoNfse >= DataInicial && nota.dataEmissaoNfse <= DataFinal &&
                     nota.estabelecimento_id == estab
                     select new ListaNotas
                               {
                                   numeroNfse = nota.numeroNfse,
                                   dataEmissaoNfse = nota.dataEmissaoNfse,
                                   dataVencimentoNfse = nota.dataVencimentoNfse,
                                   razaoTomador = nota.NotaFiscalPessoaTomador.razao,
                                   valorNfse = nota.valorNfse,
                                   valorLiquido = nota.valorLiquido
                               }).ToList();

            for (int i = 0; i < notas.Count; i++)
            {
                notas[i].dataEmissaoS = notas[i].dataEmissaoNfse.ToString("dd/MM/yyyy");
                notas[i].dataVencimentoS = notas[i].dataVencimentoNfse.ToString("dd/MM/yyyy");

            };

            return Json(notas, JsonRequestBehavior.AllowGet);
        }

        // Lista Contas a Pagar Google Charts
        public ActionResult listaContasAPagar()
        {
            CarregaViewData();
            return View();
        }
        public JsonResult geraListaContasAPagar(string dataInicial, string dataFinal, int pessoa, int tipoPessoa, int status, int dataFiltro, int projeto)
        {
            int estab = _paramBase.estab_id;
            var banco = new DbControle();
            var cp = new List<ListaContasAPagar>();

            var listaCPAG = new DocumentoPagarParcela().ObterTodosIQ(_paramBase);
            if (dataFiltro == 1)
            {
                var dataAux = new DateTime();
                if (DateTime.TryParse(dataInicial, out dataAux))
                {
                    listaCPAG = listaCPAG.Where(p =>
                       DbFunctions.TruncateTime(p.DocumentoPagarMestre.dataDocumento) >= dataAux
                   );
                }
                var dataAux2 = new DateTime();
                if (DateTime.TryParse(dataFinal, out dataAux2))
                {
                    listaCPAG = listaCPAG.Where(p =>
                       DbFunctions.TruncateTime(p.DocumentoPagarMestre.dataDocumento) <= dataAux2
                   );
                }
            }
            if (dataFiltro == 2)
            {
                var dataAux = new DateTime();
                if (DateTime.TryParse(dataInicial, out dataAux))
                {
                    listaCPAG = listaCPAG.Where(p =>
                       DbFunctions.TruncateTime(p.vencimentoPrevisto) >= dataAux
                   );
                }
                var dataAux2 = new DateTime();
                if (DateTime.TryParse(dataFinal, out dataAux2))
                {
                    listaCPAG = listaCPAG.Where(p =>
                       DbFunctions.TruncateTime(p.vencimentoPrevisto) <= dataAux2
                   );
                }
            }
            if (dataFiltro == 3)
            {
                var dataAux = new DateTime();
                if (DateTime.TryParse(dataInicial, out dataAux))
                {
                    listaCPAG = listaCPAG.Where(p =>
                       DbFunctions.TruncateTime(p.vencimento) >= dataAux
                   );
                }
                var dataAux2 = new DateTime();
                if (DateTime.TryParse(dataFinal, out dataAux2))
                {
                    listaCPAG = listaCPAG.Where(p =>
                       DbFunctions.TruncateTime(p.vencimento) <= dataAux2
                   );
                }
            }
            
            
            if (pessoa != 0)
            {
                listaCPAG = listaCPAG.Where(p => p.DocumentoPagarMestre.Pessoa.id == pessoa);
            }

            if (projeto != 0)
            {
                listaCPAG = listaCPAG.Where(p => p.DocumentoPagarMestre.DocumentoPagarProjetos.Where( x => x.Projeto_Id == projeto).Count() > 0);
            }
            if (tipoPessoa != 999999)
            {
                listaCPAG = listaCPAG.Where(p => p.DocumentoPagarMestre.Pessoa.TipoPessoa_ID == tipoPessoa);
            }

            if (status == 1)
            {
                listaCPAG = listaCPAG.Where(p => p.DocumentoPagarMestre.StatusPagamento == 1 || p.DocumentoPagarMestre.StatusPagamento == 2);
            }

            if (status == 2)
            {
                listaCPAG = listaCPAG.Where(p => p.DocumentoPagarMestre.StatusPagamento == 3);
            }
            var lpfim = listaCPAG.ToList();

            cp = (lpfim.Select( m => new ListaContasAPagar
                  {
                      id = m.id,
                      fornecedor = m.DocumentoPagarMestre.Pessoa.nome,
                      projeto = m.DocumentoPagarMestre.ReferenciaProjeto,
                      tipoFornecedor = (m.DocumentoPagarMestre.Pessoa.TipoPessoa == null) ? "":m.DocumentoPagarMestre.Pessoa.TipoPessoa.Descricao,
                      cnpj = m.DocumentoPagarMestre.Pessoa.cnpj,
                      dataDocumento = m.DocumentoPagarMestre.dataDocumento,
                      dataVencimentoOriginal = m.vencimento,
                      dataVencimento = m.vencimentoPrevisto,
                      statusPagamento = m.DocumentoPagarMestre.StatusPagamento,
                      contaContabil = m.DocumentoPagarMestre.PlanoDeConta.descricao,
                      numeroDocumento = m.DocumentoPagarMestre.numeroDocumento,
                      valorBruto = m.valor,
                      valorPago = 0,
                      saldo = 0,
                      parcela = m.parcela
                  })).ToList();



 

            //Fim do Case dataFiltro
            Pagamento pagamento = new Pagamento();

            for (int i = 0; i < cp.Count; i++)
            {
                cp[i].dataDocumentoS = cp[i].dataDocumento.ToString("dd/MM/yyyy");
                cp[i].dataVencimentoOriginalS = cp[i].dataVencimentoOriginal.ToString("dd/MM/yyyy");
                cp[i].dataVencimentoS = cp[i].dataVencimento.ToString("dd/MM/yyyy");
                cp[i].numeroDocumentoS = cp[i].numeroDocumento.ToString();

                if (cp[i].statusPagamento == 1)
                    cp[i].statusPagamentoS = "Em Aberto";
                if (cp[i].statusPagamento == 2)
                    cp[i].statusPagamentoS = "Pago Parcial";
                if (cp[i].statusPagamento == 3)
                    cp[i].statusPagamentoS = "Pago Total";
                cp[i].valorPago = pagamento.ObterValorPagoDocumento(cp[i].id, _paramBase);
                cp[i].saldo = cp[i].valorBruto - cp[i].valorPago;
                
                if (cp[i].saldo <= 0)
                    cp[i].saldo = 0;
            };

            return Json(cp, JsonRequestBehavior.AllowGet);
        }



        //Estatísticas da Nuvem SoftFin
        public ActionResult statisticsCloud()
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int empresa  = _paramBase.empresa_id;
            int estab = _paramBase.estab_id;

            StatisticCloud sc = new StatisticCloud();

            DbControle banco = new DbControle();

            sc.qtdEmpresa = banco.Empresa.Count().ToString();
            sc.qtdEstabelecimento = banco.Estabelecimento.Count().ToString();
            sc.qtdUsuario = banco.Usuarios.Count().ToString();
            sc.qtdUnidadeNegocio = banco.UnidadeNegocio.Count().ToString();
            sc.qtdPessoa = banco.Pessoa.Count().ToString();
            sc.qtdBanco = banco.Bancos.Count().ToString();
            sc.qtdContrato = banco.Contrato.Count().ToString();
            sc.qtdNotaFiscal = banco.NotaFiscal.Count().ToString();
            sc.qtdBancoMovimento = banco.BancoMovimento.Count().ToString();
            sc.qtdSaldoBancarioReal = banco.SaldoBancarioReal.Count().ToString();
            sc.qtdProjeto = banco.Projeto.Count().ToString();
            sc.qtdContasPagar = banco.DocumentoPagarMestre.Count().ToString();
            sc.qtdLogMudanca = banco.LogMudanca.Count().ToString();

            return View(sc);
        }

        //Painel de Projetos
        public ActionResult painelProjetos()
        {
            return View();
        }
        public JsonResult geraPainelProjetos(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var projetos = new List<PainelProjetos>();

            projetos = (from projeto in banco.Projeto
                        where projeto.dataInicial >= DataInicial && projeto.dataInicial <= DataFinal &&
                        projeto.estabelecimento_id == estab
                        select new PainelProjetos
                     {
                         id = projeto.id,
                         contrato = projeto.ContratoItem.Contrato.contrato,
                         cliente = projeto.ContratoItem.Contrato.Pessoa.nome,
                         projeto = projeto.nomeProjeto,
                         dataInicial = projeto.dataInicial,
                         dataFinal = projeto.dataFinal,
                         totalHoras = projeto.totalHoras,
                         valorProjeto = 0,    //Contrato.ContratoItem.valor
                         horasApontadas = 0,    //ApontamentoDiario.qtdHoras
                         custoTotal = 0,    //ApontamentoDiario.qtdHoras * TaxaHora.taxaHoraCusto
                         margem = 0,    //(Valor do Projeto - Custo Real) / (Valor do Projeto)
                         saldoHoras = 0,     //Total de Horas - Horas Apontadas
                         ativo = (projeto.ativo == true) ? "Sim" : "Não"
                     }).ToList();

            DbControle db = new DbControle();
            var cpag = new DocumentoPagarProjeto().ObterEntreData(DataInicial, DataFinal, _paramBase);

            for (int i = 0; i < projetos.Count; i++)
            {

                //variáveis
                Decimal custoProjeto = CalculaCustoProjeto(projetos[i].id);
                ApontamentoDiario ad = new ApontamentoDiario();
                Decimal horasApontadas = ad.ObterTotalHorasProjeto(projetos[i].id, _paramBase);
                ContratoItem ci = new ContratoItem();
                Decimal valorProjeto = ci.somaParcelas(projetos[i].contrato, _paramBase);

                Decimal despesasDiretas = cpag.Where(p => p.Projeto_Id == projetos[i].id).Sum(p => p.Valor);



                //Valor do Projeto
                projetos[i].valorProjeto = valorProjeto;

                //Apontamento de horas
                projetos[i].horasApontadas = horasApontadas;

                //Apontamento de horas
                projetos[i].despesasDiretas = despesasDiretas;

                //Custo Total do Projeto
                projetos[i].custoTotal = custoProjeto;

                //Margem
                projetos[i].margem = (valorProjeto - custoProjeto) - despesasDiretas;

                //Saldo de Horas
                projetos[i].saldoHoras = projetos[i].totalHoras - horasApontadas;

                //Datas
                projetos[i].dataInicialS = projetos[i].dataInicial.ToString("dd/MM/yyyy");
                projetos[i].dataFinalS = projetos[i].dataFinal.ToString("dd/MM/yyyy");

            };

            return Json(projetos, JsonRequestBehavior.AllowGet);
        }

        //Produtividade dos Recursos
        public ActionResult produtividadeRecursos()
        {
            return View();
        }
        public JsonResult geraProdutividadeRecursos(string dataInicial, string dataFinal)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int empresa  = _paramBase.empresa_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var banco = new DbControle();
            var recursos = new List<ProdutividadeRecursos>();

            recursos = (from usuario in banco.ProjetoUsuario
                        where usuario.Usuario.empresa_id == empresa
                        orderby usuario.Usuario.nome
                        select new ProdutividadeRecursos
                        {
                            id = usuario.usuario_id,
                            recurso = usuario.Usuario.nome,
                            horasDisponiveis = 0,
                            horasApontadas = 0,
                            produtividade = 0,
                        }).Distinct().ToList();

            DbControle db = new DbControle();

            for (int i = 0; i < recursos.Count; i++)
            {
                //variáveis
                ApontamentoDiario ad = new ApontamentoDiario();
                Decimal horasDisponiveis = CalculaHorasDisponiveisPeriodo(DataInicial, DataFinal);
                Decimal horasApontadas = ad.ObterTodosDataUsuario(DataInicial, DataFinal, recursos[i].id, _paramBase).Sum(x => x.qtdHoras);
                Decimal produtividade = decimal.Round((horasApontadas / horasDisponiveis * 100), 2, MidpointRounding.AwayFromZero);

                //Horas Disponíveis
                recursos[i].horasDisponiveis = horasDisponiveis;

                //Horas Apontadas
                recursos[i].horasApontadas = horasApontadas;

                //Produtividade
                recursos[i].produtividade = produtividade;
            };

            return Json(recursos, JsonRequestBehavior.AllowGet);
        }

        public Decimal CalculaHorasDisponiveisPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            if (dataInicial > dataFinal)
                throw new ArgumentException("Data Final Incorreta" + dataFinal);

            Calendario cd = new Calendario();
            Decimal horasFeriado = cd.ObterTodosPeriodo(dataInicial, dataFinal, _paramBase).Sum(x => x.tipoDataCalendario.horasUteis);

            TimeSpan span = dataFinal - dataInicial;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;

            // descobrir se há fins de semana durante o tempo que excedam as semanas cheias
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = dataInicial.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)dataInicial.DayOfWeek;
                int lastDayOfWeek = dataFinal.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)dataFinal.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            //foreach (DateTime bankHoliday in bankHolidays)
            //{
            //    DateTime bh = bankHoliday.Date;
            //    if (firstDay <= bh && bh <= lastDay)
            //        --businessDays;
            //}

            return (businessDays * 8) - horasFeriado;
        }
        public Decimal CalculaCustoProjeto(int projeto)
        {
            DbControle db = new DbControle();
            ApontamentoDiario ad = new ApontamentoDiario();
            List<ApontamentoDiario> lista = ad.ObterTodosProjeto(projeto, _paramBase);

            Decimal custo = 0;
            for (int i = 0; i < lista.Count(); i++)
            {
                custo = custo + (lista[i].qtdHoras * ObtemCustoApontador(lista[i].apontador.id));
            }
            return custo;
        }
        public Decimal ObtemCustoApontador(int apontador)
        {
            int estab = _paramBase.estab_id;
            DbControle db = new DbControle();
            ProjetoUsuario pu = new ProjetoUsuario().ObterCategoriaApontador(apontador, _paramBase);
            if (pu == null)
                return 0;
            var conta = db.TaxaHora.Where(x => x.categoria_id == pu.categoria_id);
            if (conta.Count() == 0)
                return 0;

            return conta.Sum(x => x.taxaHoraCusto);
        }

        //Notas Fiscais em Aberto (status 1-Em Aberto ou 2-Recebida Parcial)
        public ActionResult notasEmAberto() => View();
        public JsonResult geraNotasEmAberto()
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            var banco = new DbControle();
            var notas = new List<NotasEmAberto>();

            notas = (from nf in banco.NotaFiscal
                     where (nf.SituacaoRecebimento == 1 ||
                            nf.SituacaoRecebimento == 2)
                            && nf.estabelecimento_id == estab
                     select new NotasEmAberto
                     {
                         id = nf.id,
                         numeroNfse = nf.numeroNfse,
                         dataEmissaoNfse = nf.dataEmissaoNfse,
                         dataVencimentoNfse = nf.dataVencimentoNfse,
                         razaoTomador = nf.NotaFiscalPessoaTomador.razao,
                         valorNfse = nf.valorNfse,
                         valorLiquido = nf.valorLiquido,
                         valorRecebido = 0,
                         saldoReceber = 0,
                         situacaoRecebimento = nf.SituacaoRecebimento,
                         textoRecebimento = "",
                         diasAtraso = 0
                     }).ToList();

            DbControle db = new DbControle();

            for (int i = 0; i < notas.Count; i++)
            {

                //variáveis
                Recebimento rec = new Recebimento();
                String textoRecebimento = "";
                Decimal valorRecebido = rec.ObterValorRecebidoNota(notas[i].id, _paramBase);

                if (notas[i].situacaoRecebimento == 0)
                { textoRecebimento = "Erro"; }
                if (notas[i].situacaoRecebimento == 1)
                { textoRecebimento = "Em Aberto"; }
                if (notas[i].situacaoRecebimento == 2)
                { textoRecebimento = "Recebido Parcial"; }
                if (notas[i].situacaoRecebimento == 3)
                { textoRecebimento = "Recebido Total"; }
                if (notas[i].situacaoRecebimento > 3)
                { textoRecebimento = "Erro"; }

                //Atualiza valor dos recebimentos
                notas[i].valorRecebido = valorRecebido;

                //Atualiza texto Situação Recebimento
                notas[i].textoRecebimento = textoRecebimento;

                //Saldo a Receber
                notas[i].saldoReceber = notas[i].valorLiquido - notas[i].valorRecebido;

                //Dias em atraso
                DateTime hoje = DateTime.Now;
                notas[i].diasAtraso = Math.Round((hoje - notas[i].dataVencimentoNfse).TotalDays);
                if (notas[i].diasAtraso < 0)
                {
                    notas[i].diasAtraso = 0;
                }

                //Datas
                notas[i].dataEmissaoS = notas[i].dataEmissaoNfse.ToString("dd/MM/yyyy");
                notas[i].dataVencimentoS = notas[i].dataVencimentoNfse.ToString("dd/MM/yyyy");

            };

            return Json(notas, JsonRequestBehavior.AllowGet);
        }

        private void CarregaViewData()
        {
            ViewData["TipoPessoa"] = new SelectList(new TipoPessoa().ObterTodos(_paramBase), "id", "Descricao");
            ViewData["Pessoa"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["Projeto"] = new SelectList(new Projeto().ObterTodos(_paramBase), "id", "nomeProjeto");

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

    }


}