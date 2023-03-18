using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioResultadoCaixaPorUnidadeController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["unidades"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
            return View();
        }

        public JsonResult geraResultadoPorUnidade(string dataInicial, string dataFinal, string consolidado, string ov, int unidade_id)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var contas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo);

            var bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataFinal, consolidado, _paramBase);

            bcomov = TrataUnidadesNegocio(bcomov, unidade_id);

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y => y.data >= DataInicial & y.data <= DataFinal).ToList();

            ovs = ovs.Where(p => p.unidadeNegocio_ID == unidade_id).ToList();

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

        private List<BancoMovimento> TrataUnidadesNegocio(List<BancoMovimento> bcomov, int unidade_id)
        {
            var listaBcoMovimento = new List<BancoMovimento>();
            
            foreach (var item in bcomov)
            {
                decimal valor = item.valor;
                decimal valorInsert = 0;
                bool insere = false;
                if (item.DocumentoPagarParcela_id != null)
                {
                    var cpags = item.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes.Where(p => p.unidadenegocio_id == unidade_id);
                    if (cpags.Count() > 0)
                    {
                        foreach (var item2 in cpags)
                        {
                            decimal percentual = (item2.DocumentoPagarMestre.valorBruto * 100) / item2.valor;
                            valorInsert += (item.valor / 100) * percentual;
                            insere = true;
                        }
                    }

                }
                else if (item.notafiscal_id != null)
                {
                    if (item.NotaFiscal.OrdemVenda.unidadeNegocio_ID == unidade_id)
                    {
                        valorInsert = item.valor;
                        insere = true;
                    }
                }
                else if (item.recebimento_id != null)
                {
                    if (item.Recebimento.notaFiscal.OrdemVenda.unidadeNegocio_ID == unidade_id)
                    {
                        valorInsert = item.valor;
                        insere = true;
                    }

                }
                else if (item.pagamento_id != null)
                {
                    var cpags = item.Pagamento.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes.Where(p => p.unidadenegocio_id == unidade_id);
                    if (cpags.Count() > 0)
                    {
                        foreach (var item2 in cpags)
                        {
                            decimal percentual = (item2.DocumentoPagarMestre.valorBruto * 100) / item2.valor;
                            valorInsert += (item.valor / 100) * percentual;
                            insere = true;
                        }
                    }

                }
                else if (item.UnidadeNegocio != null)
                {
                    if (item.UnidadeNegocio_id == unidade_id)
                    {
                        valorInsert = item.valor;
                        insere = true;
                    }

                }

                if (insere)
                {
                    item.valor = valorInsert;
                    listaBcoMovimento.Add(item);
                }
            }





            return listaBcoMovimento;
        }
        public JsonResult dreDrillDown(string dataInicial, string dataFinal, string conta, string consolidado, string ov, int unidade_id)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataFinal, consolidado, _paramBase).Where(x => x.PlanoDeConta.codigo == conta).OrderBy(y => y.data).ToList();

            bcomov = TrataUnidadesNegocio(bcomov, unidade_id);


            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                                    y => y.data >= DataInicial & y.data <= DataFinal).ToList();
            ovs = ovs.Where(p => p.unidadeNegocio_ID == unidade_id).ToList();

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
    }


}