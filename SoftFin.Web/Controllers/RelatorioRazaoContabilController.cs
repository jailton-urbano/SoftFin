using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SoftFin.Utils;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioRazaoContabilController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public class DtoData
        {

            public string CC { get; set; }
            
            public decimal SaldoInicial { get; internal set; }
            public decimal SaldoFinal { get; internal set; }

            public List<DtoCentroCusto> DtoCentroCustos { get; set; }
        }



        public class DtoCentroCusto
        {
            public string CentroCusto{ get; set; }

            public List<DtoDataLinha> Linha { get; set; }

            public decimal SaldoInicial { get; internal set; }
            public decimal SaldoFinal { get; internal set; }
        }
        

        public class DtoDataLinha
        {
            public string Data { get; set; }

            public string Historico { get; set; }
            public string NL { get; set; }
            public string CCC { get; set; }

            public Decimal Credito { get; set; }
            public Decimal Debito { get; set; }

            public Decimal Saldo { get; set; }
        }

        public ActionResult ObterRelatorio(int mes, int ano, bool excel)
        {
            var retorno = new List<DtoData>();

            var dataInicial = new DateTime(ano, mes, 1);
            var dataFinal = new DateTime(ano, mes, 1);

            dataFinal = dataFinal.AddMonths(1).AddDays(-1);

            var lcs = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase, dataInicial, dataFinal);
            var contaslst = new List<String>();

            foreach (var item in lcs)
            {
                foreach (var itemDet in item.LancamentoContabilDetalhes)
                {
                    contaslst.Add(itemDet.ContaContabil.codigo);
                }
            }
            contaslst = contaslst.Distinct().OrderBy(p => p).ToList();


            foreach (var itemcc in contaslst)
            {
                
                                             
                var lcsAux = lcs.
                    Where(p => p.LancamentoContabilDetalhes.Where(x => x.ContaContabil.codigo == itemcc).Count() > 0).
                    ToList();

                lcsAux = lcsAux.OrderBy(p => p.codigoLancamento).ToList();


                var unidadeidlst = new List<int?>();


                foreach (var itemUnidade in lcsAux)
                {
                    if (itemUnidade.UnidadeNegocio == null)
                    {
                        if (unidadeidlst.Where(p => p == null).Count() == 0)
                        {
                            unidadeidlst.Add(null);
                        }
                    }
                    else
                    {
                        if (unidadeidlst.Where(p => p.Value == itemUnidade.UnidadeNegocio.id).Count() == 0)
                        {
                            unidadeidlst.Add(itemUnidade.UnidadeNegocio.id);
                        }
                    }
                }

                var conta = new ContaContabil().ObterPorIdCodigo(itemcc,_paramBase);
                var dtoData = new DtoData();
                dtoData.CC = itemcc;
                
                if (conta != null)
                    dtoData.CC += " " + conta.descricao;


                dtoData.DtoCentroCustos = new List<DtoCentroCusto>();
                dtoData.SaldoInicial = 0;

                foreach (var itemCentro in unidadeidlst)
                {
                    var dtoCentroCustos = new DtoCentroCusto();
                   

                    if (itemCentro != null)
                    {
                        var unidade = new UnidadeNegocio().ObterPorId(itemCentro.Value, _paramBase);
                        dtoCentroCustos.CentroCusto = "";
                        if (unidade.Codigo == null)
                            dtoCentroCustos.CentroCusto = unidade.Codigo;

                        dtoCentroCustos.CentroCusto += unidade.unidade;
                    }


                    var listaporunidade = lcsAux.Where(p => p.UnidadeNegocio_ID == null).ToList();

                    if (itemCentro != null)
                        listaporunidade = lcsAux.Where(p => p.UnidadeNegocio_ID == itemCentro.Value).ToList();


                    dtoCentroCustos.SaldoInicial = 0;


                    dtoCentroCustos.Linha = new List<DtoDataLinha>();
                    foreach (var item in listaporunidade)
                    {

                        var dtoDataLinha = new DtoDataLinha();
                        dtoDataLinha.Historico = item.historico;
                        dtoDataLinha.NL = item.codigoLancamento.ToString();
                        dtoDataLinha.Data = item.data.ToString("dd/MM/yyyy");
                        //if (item.UnidadeNegocio != null)
                        //    dtoDataLinha..CC = item.UnidadeNegocio.Codigo + " : " + item.UnidadeNegocio.unidade;

                        foreach (var itemDet in item.LancamentoContabilDetalhes)
                        {
                            if (itemDet.ContaContabil.codigo == itemcc)
                            {
                                if (itemDet.DebitoCredito == "D")
                                {
                                    dtoDataLinha.Debito = itemDet.valor;
                                    dtoDataLinha.Saldo = dtoData.SaldoFinal - dtoDataLinha.Debito;
                                    dtoData.SaldoFinal = dtoData.SaldoFinal - dtoDataLinha.Debito;
                                }
                                else
                                {
                                    dtoDataLinha.Credito = itemDet.valor;
                                    dtoDataLinha.Saldo = dtoData.SaldoFinal + dtoDataLinha.Credito;
                                    dtoData.SaldoFinal = dtoData.SaldoFinal + dtoDataLinha.Credito;
                                }
                            }
                            else
                            {
                                dtoDataLinha.CCC = itemDet.ContaContabil.codigo + " - " + itemDet.ContaContabil.descricao;
                            }
                        }
                        dtoCentroCustos.Linha.Add(dtoDataLinha);
                    }
                    dtoData.DtoCentroCustos.Add(dtoCentroCustos);
                }
                retorno.Add(dtoData);
            }


            return Json(
                retorno, JsonRequestBehavior.AllowGet);

        }

 

    }
}