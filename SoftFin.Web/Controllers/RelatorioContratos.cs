using SoftFin.Utils;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioContratosController : BaseController
    {
        // GET: RelatorioOV
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult  ObterRelatorio(DateTime dataIni, DateTime dataFim, bool excel)
        {
            var contratos = new ParcelaContrato().ObterEntreData(dataIni, dataFim, _paramBase);
            var unidades = GetUnidadesBancoDados();


            if (!excel)
            {
                var retorno = contratos.Select(p => new
                {
                    Codigo = p.ContratoItem.Contrato.contrato,
                    Nome = p.ContratoItem.Contrato.Pessoa.nome,
                    CI = p.ContratoItem.pedido,
                    ValorCI = p.ContratoItem.valor,
                    DataEmissao = p.data.ToString("o"),
                    Situacao = p.statusParcela.status,
                    Parcela = p.parcela,
                    TipoContrato = p.ContratoItem.TipoContrato.tipo,
                    ValorParcela = p.valor,
                    Unidades = DistribuicaoPorUnidade(unidades, p.ContratoItem.ContratoItemUnidadeNegocios, p.ContratoItem, p)
                }).ToList();
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var retorno = contratos.Select(p => new
                {
                    Codigo = p.ContratoItem.Contrato.contrato,
                    Nome = p.ContratoItem.Contrato.Pessoa.nome,
                    CI = p.ContratoItem.pedido,
                    ValorCI = p.ContratoItem.valor,
                    DataEmissao = p.data.ToString("dd/MM/yyyy"),
                    Situacao = p.statusParcela.status,
                    Parcela = p.parcela,
                    TipoContrato = p.ContratoItem.TipoContrato.tipo,
                    ValorParcela = p.valor,
                    Unidades = DistribuicaoPorUnidade(unidades, p.ContratoItem.ContratoItemUnidadeNegocios, p.ContratoItem, p)
                }).ToList();
                CsvExport myExport = new CsvExport();

                foreach (var item in retorno)
                {
                    myExport.AddRow();
                    myExport["Codigo"] = item.Codigo;
                    myExport["Nome"] = item.Nome;
                    myExport["Contrato Item"] = item.CI;
                    myExport["Valor Contrato Item"] = item.ValorCI;
                    myExport["Data Emissao"] = item.DataEmissao;
                    myExport["Situacao"] = item.Situacao;
                    myExport["Parcela"] = item.Parcela;
                    myExport["Valor Parcela"] = item.ValorParcela;
                    myExport["Tipo Contrato"] = item.TipoContrato;

                    for (int i = 0; i < item.Unidades.Count; i++)
                    {
                        myExport[unidades[i].ToString()] = item.Unidades[i];
                    }


                }
                string myCsv = myExport.Export();
                byte[] myCsvData = myExport.ExportToBytes();
                return File(myCsvData, "application/vnd.ms-excel", "Contratos.csv");
            }

        }

        private static List<decimal> DistribuicaoPorUnidade(List<string> unidades, 
            ICollection<ContratoItemUnidadeNegocio> unidadesCI,
            ContratoItem contratoItem,
            ParcelaContrato pc
            )
        {
            var unidadesCIaux = unidadesCI.ToList();

            var valores = new List<decimal>();
            foreach (var item in unidades)
            {
                var unidadavez = unidadesCIaux.Where(p => p.UnidadeNegocio.unidade == item).FirstOrDefault();

                if (unidadavez != null)
                {
                    decimal valorUnidade = (contratoItem.valor * 100) / unidadavez.Valor;
                    valorUnidade = decimal.Round((pc.valor / 100) * valorUnidade, 2);
                    valores.Add(valorUnidade);
                }
                else
                {
                    valores.Add( 0);
                }
            }
            return valores;
        }

        [HttpGet]
        public JsonResult ObterNomesUnidades()
        {
            List<string> unidades = GetUnidadesBancoDados();
            return Json(unidades, JsonRequestBehavior.AllowGet);
        }

        private List<string> GetUnidadesBancoDados()
        {
            return new UnidadeNegocio().ObterTodos(_paramBase).Select(p => p.unidade).OrderBy(p => p).ToList();
        }
    }
}