using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SaldoDiarioController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        public class DtoSaldos
        {
            public string Descricao { get; set; }
            public decimal DiaAnterior { get; set; }
            public decimal Entradas { get; set; }
            public decimal Saida { get; set; }
            public decimal Valor { get; set; }
        }
        public JsonResult SaldoDiario(string dataInicial, string dataFinal, string consolidado, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            List<DtoSaldos> listSaldo = new List<DtoSaldos>();

            decimal saldo = 0;
            decimal diaanteriorFinal = 0;
            decimal EntradasFinal = 0;
            decimal SaidaFinal = 0;
            decimal diaanterior = 0;
            for (DateTime i = DataInicial; i <= DataFinal; i = i.AddDays(1) )
            {
                var saldoDia = new BancoMovimento().ObterTodosDataConsolidado (i, i,consolidado,_paramBase);


                decimal debitos = saldoDia.Where(p => p.TipoMovimento.Codigo == "SAI").Sum(p=> p.valor);
                decimal creditos = saldoDia.Where(p => p.TipoMovimento.Codigo == "ENT").Sum(p => p.valor);

                if (ov.ToUpper().Equals("TRUE"))
                {
                    List<OrdemVenda> ovs = new OrdemVenda().ObterEntreData(i, i, _paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").ToList();
                    creditos += ovs.Sum(p => p.valor);
                }

                
                listSaldo.Add(new DtoSaldos
                        {
                            Descricao = i.ToString("dd/MM/yyyy"),
                            DiaAnterior = diaanterior,
                            Entradas = creditos,
                            Saida= debitos,
                            Valor = (creditos - debitos) + diaanterior 
                        }
                );
                
                diaanterior = (creditos - debitos) + diaanterior;
                EntradasFinal += creditos;
                SaidaFinal += debitos;
                saldo += diaanterior;
            }

            listSaldo.Add(new DtoSaldos
                            {
                                DiaAnterior = diaanteriorFinal,
                                Entradas = EntradasFinal,
                                Saida = SaidaFinal,
                                Descricao = " Saldo de " + DataInicial.ToString("dd/MM/yyyy") + " " + DataFinal.ToString("dd/MM/yyyy"),
                                Valor = (EntradasFinal - SaidaFinal) + diaanteriorFinal 
                            }
                );

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(listSaldo);

            return Json(listSaldo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DrillDown(string data, string consolidado, string ov)
        {
            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(data);

            var bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataInicial, consolidado,_paramBase).OrderBy(y => y.data).ToList();

            //Obtém Ordens de Venda com StatusParcela "Liberada" ou "Pendente"
            List<OrdemVenda> ovs = new OrdemVenda().ObterEntreData(DataInicial, DataInicial, _paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").ToList();

            TipoMovimento tm = new TipoMovimento();

            Banco bc = new Banco();
            _estab = _paramBase.estab_id;

            if (ov.ToUpper().Equals("TRUE"))
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
                        varPessoa = ps.ObterPorId(id,_paramBase).nome;
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

   
            return Json(listDRE, JsonRequestBehavior.AllowGet);

        }

    }
}
