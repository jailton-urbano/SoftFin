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
    public class RelatorioDREMensalController : BaseController
    {
        List<PlanoDeConta> _planoContas = new List<PlanoDeConta>();
        // DRE2 - Demonstração do Resultado com Drill Down
        public ActionResult Index()
        {
            return View();
        }

        //[OutputCache(Duration = 60, VaryByParam = "ano")]
        public JsonResult geraDRE2(int ano, string consolidado, string ov)
        {
            var DataInicial = DateTime.Parse(ano + "/01/01");
            var DataFinal = DateTime.Parse((ano + 1) + "/01/01");



            Banco bc = new Banco();
            var bancoprincipal = bc.ObterTodos( _paramBase).Where(x => x.estabelecimento_id == _estab & x.principal == true).FirstOrDefault();
            if (bancoprincipal == null)
                return Json(new { CDStatus = "NOK", DSMessage = "Banco principal não configurado." }, JsonRequestBehavior.AllowGet);

            List<DRE3> listDRE = CalculaDRE(ov, DataInicial, DataFinal, bancoprincipal, consolidado);


            return Json(new
            {
                CDStatus = "OK",
                List = listDRE.Select(p => new { p.codigoConta, p.descricaoConta, p.tipoConta, p.valor, p.percentual })

            }, JsonRequestBehavior.AllowGet);
        }

        public List<DRE3> CalculaDRE(string ov, DateTime DataInicial, DateTime DataFinal,  Banco bancoprincipal, string consolidado)
        {

            _planoContas = new PlanoDeConta().ObterTodos().OrderBy(x => x.codigo).ToList();
            List<BancoMovimento> bcomov = new BancoMovimento().ObterTodosDataConsolidado(DataInicial, DataFinal, consolidado,_paramBase);

            List<OrdemVenda> ovs = new OrdemVenda().ObterTodos(_paramBase).Where(x => x.statusParcela.status == "Liberada" || x.statusParcela.status == "Pendente").Where(
                                        y => y.data >= DataInicial & y.data <= DataFinal).ToList();
            TipoMovimento tm = new TipoMovimento();


            if (ov == "true")
            {
                foreach (var item in ovs)
                {
                    BancoMovimento bm = new BancoMovimento();


                    bm.banco_id = bancoprincipal.id;
                    bm.data = item.data;
                    bm.historico = "Ordem de Venda: " + item.Numero;
                    bm.PlanoDeConta = _planoContas.Where(x => x.id == 1).FirstOrDefault();
                    bm.planoDeConta_id = 1;
                    bm.valor = item.valor;
                    bm.tipoDeMovimento_id = tm.ObterTodos(_paramBase).Where(x => x.Codigo == "ENT").FirstOrDefault().id;
                    bm.TipoMovimento = tm.ObterPorId(bm.tipoDeMovimento_id, _paramBase);
                    bcomov.Add(bm);
                }
            }
            List<DRE3> listDRE = new List<DRE3>();

            var planoDeContas = _planoContas.Where(p => p.nivelSuperior == null).OrderBy(e => e.codigo);
            List<List<decimal>> valorCalculadoFinal = new List<List<decimal>>();

            foreach (var item in planoDeContas)
            {
                List<DRE3> listDRETemp = new List<DRE3>();
                List<decimal> valorCalculado = new List<decimal>();
                for (int mes = 1; mes <= 12; mes++)
                {
                    decimal valorplano = bcomov.Where(p => p.data.Month == mes && p.planoDeConta_id == item.id).Sum(p => p.valor) * ((item.DebitoCredito == "D") ? -1 : 1);
                    var planoTotalSub = ContaSub(bcomov, item, mes);
                    valorplano += planoTotalSub;
                    valorCalculado.Add(valorplano);
                }
                valorCalculado.Add(valorCalculado.Sum(p => p));
                if (valorCalculado.Last() != 0)
                {
                    listDRE.Add(new DRE3
                    {
                        codigoConta = item.codigo,
                        descricaoConta = item.descricao,
                        tipoConta = item.TipoConta,
                        valor = valorCalculado
                    });
                    valorCalculadoFinal.Add(valorCalculado);
                    PlanoInferior(bcomov, item.id, listDRE);
                }
            }

            List<decimal> somaFinal = new List<decimal>();
            for (int i = 0; i <= 12; i++)
            {
                decimal somaMes = 0;
                foreach (var item in valorCalculadoFinal)
                {
                    somaMes += item[i];
                }
                somaFinal.Add(somaMes);
            }

            listDRE.Add(new DRE3
            {
                codigoConta = "99",
                descricaoConta = "Resultado da Conta",
                tipoConta = "S",
                valor = somaFinal
            });
            return listDRE;
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