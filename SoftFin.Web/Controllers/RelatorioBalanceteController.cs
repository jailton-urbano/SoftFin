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
    public class RelatorioBalanceteController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public  class UnidadesRelatorio
        {
            public int Id { get; set; }
            public String Nome { get; set; }
            public String Codigo { get; set; }
        }


        public ActionResult ObterRelatorio(int mes, int ano, bool excel)
        {
            var retorno = new List<DtoData>();
            var dataInicial = new DateTime(ano, mes, 1 );
            var dataFinal = new DateTime( ano, mes, 1 );

            dataFinal = dataFinal.AddMonths(1).AddDays(-1);

            var contaContabils = new ContaContabil().ObterTodos(_paramBase).OrderBy(p => p.codigo);
            var lancamentoContabils = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase, dataInicial, dataFinal);
            var unidadesRelatorio = new List<UnidadesRelatorio>();
            var unidades = lancamentoContabils.Select(p => p.UnidadeNegocio).Distinct();
            foreach (var itemUnidade in unidades)
            {
                if (itemUnidade == null)
                {
                    if (unidadesRelatorio.Where(p => p == null).Count() == 0)
                    {
                        unidadesRelatorio.Add(new UnidadesRelatorio { Id = 0, Codigo = "", Nome = "" });
                    }
                }
                else
                {
                    if (unidadesRelatorio.Where(p => p.Id == itemUnidade.id).Count() == 0)
                    {
                        unidadesRelatorio.Add(new UnidadesRelatorio { Id = itemUnidade.id, Codigo = itemUnidade.Codigo, Nome = itemUnidade.unidade });
                    }
                }
            }

            var saldoContabil = new SaldoContabil().ObterPorDatabase(dataInicial, _paramBase);

            foreach (var item in contaContabils)
            {
                foreach (var itemUnidade in unidadesRelatorio)
                {
                    var aux = new DtoData();
                    aux.ContaContabil = item.codigo;
                    aux.DescricaoConta = item.descricao;

                    var capas = saldoContabil.ToList();
                    var codigoCentroCusto = "";

                    if (itemUnidade.Id == 0)
                    {
                        codigoCentroCusto = null;
                        aux.CentroCusto = "";
                    }
                    else
                    {
                        codigoCentroCusto = itemUnidade.Codigo;
                        aux.CentroCusto = codigoCentroCusto + "-" + itemUnidade.Nome;
                    }

                    

                    foreach (var capa in capas)
                    {
                        foreach (var detalhe in 
                            capa.SaldoContabilDetalhe.Where(p => p.CodigoConta == item.codigo 
                            && p.CodigoCentroCusto == codigoCentroCusto)
                            )
                        {
                            aux.SaldoAnterior += detalhe.SaldoInicial;
                            break;
                        }
                    }
                    var lancamentoporUnidade = new List<LancamentoContabil>();

                    if (codigoCentroCusto != null)
                    {
                        lancamentoporUnidade = lancamentoContabils.
                            Where(p => p.UnidadeNegocio.Codigo == codigoCentroCusto).ToList();
                    }
                    else
                    {
                        lancamentoporUnidade = lancamentoContabils.Where(p => p.UnidadeNegocio == null).ToList();
                    }

                    foreach (var itemLC in lancamentoporUnidade)
                    {
                        var listaLancto = itemLC.LancamentoContabilDetalhes.
                            Where(p => p.ContaContabil.codigo.StartsWith(item.codigo));

                        foreach (var itemLCD in listaLancto)
                        {
                            if (itemLCD.DebitoCredito == "C")
                            {
                                aux.Credito += itemLCD.valor;
                            }
                            else
                            {
                                aux.Debito += itemLCD.valor;
                            }
                        }
                    }
                    aux.SaldoFinal = aux.SaldoAnterior + (aux.Credito - aux.Debito);
                    retorno.Add(aux);
                }
                
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public class DtoData
        {
            public string ContaContabil { get; set; }
            public string CentroCusto { get; set; }
            public string DescricaoConta { get; set; }
            public decimal SaldoAnterior { get; set; }
            public decimal Debito { get; set; }
            public decimal Credito { get; set; }
            public decimal SaldoFinal { get; set; }
        }

    }
}