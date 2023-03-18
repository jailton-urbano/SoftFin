using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;

namespace SoftFin.Web.Controllers
{
    public class MapaConciliacaoController : BaseController
    {
        string erro = "";
        public ActionResult MapaConciliacao()
        {
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

        public ActionResult MapaConciliacao2()
        {
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

        [HttpPost]
        public JsonResult geraMapaConciliacao(string dataFinal, int banco)
        {
            try
            { 
            //variáveis
                decimal varSaldoInicialAno = 0;
                decimal varSaldoInicialMes = 0;
                decimal varSaldoCalculado = 0;
                decimal varDiferenca = 0;
                string varStatusSaldoReal = "";
                string varStatusSaldoCalculado = "";
                string varStatusLanctosConciliacao = "";

                string varDataInicial = "01/" + 
                                        DateTime.Parse(dataFinal).Month.ToString() + 
                                        "/" +
                                        DateTime.Parse(dataFinal).Year.ToString();

                DateTime DataInicial = new DateTime();
                DataInicial = DateTime.Parse(varDataInicial);
            
                DateTime DataFinal = new DateTime();
                DataFinal = DateTime.Parse(dataFinal);
                string varDataFinal = DateTime.DaysInMonth(DataFinal.Year, DataFinal.Month) + "/" +
                            DataFinal.Month + "/" +
                            DataFinal.Year;
                DataFinal = DateTime.Parse(varDataFinal);

                List<MapaConciliacao> listMapaConciliacao = new List<MapaConciliacao>();

                //Calcula Saldo Inicial do Mês
                DateTime varDataInicialMes = DataInicial.AddDays(-1);
                string varAno = DataInicial.Year.ToString();
                DateTime dataInicialAno = Convert.ToDateTime("01/01/" + varAno);
                varSaldoInicialAno = ObtemSaldoInicialAno(DataInicial, banco);
                varSaldoInicialMes = CalculaSaldoFinal(varSaldoInicialAno, dataInicialAno, varDataInicialMes, banco);

                while (DataInicial <= DataFinal)
                {

                    //Obtem saldo Real
                    var sdoR = new SaldoBancarioReal().ObterBancoData(banco, DataInicial, _paramBase);
                    if (sdoR != null)
                    {
                        varStatusSaldoReal = "1";
                    }
                    else
                    {
                        varStatusSaldoReal = "0";
                    }

                    //Obtem saldo Calculado
                    varSaldoCalculado = CalculaSaldoFinal(varSaldoInicialMes, DataInicial, DataInicial, banco);
                    varDiferenca = ApuraDiferencaSaldo(varSaldoCalculado, DataInicial, banco);

                    if (varDiferenca != 0)
                    {
                        varStatusSaldoCalculado = "0";
                    }
                    else
                    {
                        varStatusSaldoCalculado = "1";
                    }

                    varStatusLanctosConciliacao = "0";
                    var bancoMovs = new BancoMovimento().ObterTodos(banco, DataInicial, _paramBase);

                    var OFXs = new LanctoExtrato().ObterTodos(banco, DataInicial);
                    var totExtrato = OFXs.Count();



                    if (totExtrato > 0)
                    {
                        varStatusLanctosConciliacao = "0";
                        if (OFXs.Where(p => p.UsuConciliado != null).Count() == totExtrato)
                            varStatusLanctosConciliacao = "1";
                    }
                    else if(bancoMovs.Count() == 0)
                    {
                        varStatusLanctosConciliacao = "1";
                    }


                    listMapaConciliacao.Add(new MapaConciliacao()
                    {
                        data = Convert.ToString(DataInicial),
                        statusSaldoReal = varStatusSaldoReal,
                        statusSaldoCalculado = varStatusSaldoCalculado,
                        statusLanctosConciliacao = varStatusLanctosConciliacao
                    });


                    varSaldoInicialMes = varSaldoCalculado;
                    DataInicial = DataInicial.AddDays(1);
                }
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(listMapaConciliacao);

                if (erro == "")
                {
                    return Json(listMapaConciliacao, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception(erro);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                if (erro == "")
                {
                    return Json("OPS! - " + ex.Message, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("OPS! - " + erro, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public JsonResult geraMapaConciliacao2(string dataFinal, int banco)
        {
            try
            {
                //variáveis
                decimal varSaldoInicialAno = 0;
                decimal varSaldoInicialMes = 0;
                decimal varSaldoCalculado = 0;
                decimal varDiferenca = 0;
                Boolean varStatusSaldoReal = false;
                Boolean varStatusSaldoCalculado = false;
                Boolean varStatusLanctosConciliacao = false;

                string varDataInicial = "01/" +
                                        DateTime.Parse(dataFinal).Month.ToString() +
                                        "/" +
                                        DateTime.Parse(dataFinal).Year.ToString();

                DateTime DataInicial = new DateTime();
                DataInicial = DateTime.Parse(varDataInicial);

                DateTime DataFinal = new DateTime();
                DataFinal = DateTime.Parse(dataFinal);
                string varDataFinal = DateTime.DaysInMonth(DataFinal.Year, DataFinal.Month) + "/" +
                            DataFinal.Month + "/" +
                            DataFinal.Year;
                DataFinal = DateTime.Parse(varDataFinal);

                List<MapaConciliacao2> listMapaConciliacao = new List<MapaConciliacao2>();

                //Calcula Saldo Inicial do Mês
                DateTime varDataInicialMes = DataInicial.AddDays(-1);
                string varAno = DataInicial.Year.ToString();
                DateTime dataInicialAno = Convert.ToDateTime("01/01/" + varAno);
                varSaldoInicialAno = ObtemSaldoInicialAno(DataInicial, banco);
                varSaldoInicialMes = CalculaSaldoFinal(varSaldoInicialAno, dataInicialAno, varDataInicialMes, banco);

                while (DataInicial <= DataFinal)
                {

                    //Obtem saldo Real
                    var sdoR = new SaldoBancarioReal().ObterBancoData(banco, DataInicial, _paramBase);
                    if (sdoR != null)
                    {
                        varStatusSaldoReal = true;
                    }
                    else
                    {
                        varStatusSaldoReal = false;
                    }

                    //Obtem saldo Calculado
                    varSaldoCalculado = CalculaSaldoFinal(varSaldoInicialMes, DataInicial, DataInicial, banco);
                    varDiferenca = ApuraDiferencaSaldo(varSaldoCalculado, DataInicial, banco);

                    if (varDiferenca != 0)
                    {
                        varStatusSaldoCalculado = false;
                    }
                    else
                    {
                        varStatusSaldoCalculado = true;
                    }



                    var OFXs = new LanctoExtrato().ObterTodos(banco, DataInicial);
                    var totExtrato = OFXs.Count();

                    if (totExtrato > 0)
                    {
                        varStatusLanctosConciliacao = false;
                        if (OFXs.Where(p => p.UsuConciliado != null).Count() == totExtrato)
                            varStatusLanctosConciliacao = true;
                    }


                    listMapaConciliacao.Add(new MapaConciliacao2()
                    {
                        data = DataInicial.ToShortDateString(),
                        statusSaldoReal = varStatusSaldoReal,
                        statusSaldoCalculado = varStatusSaldoCalculado,
                        statusLanctosConciliacao = varStatusLanctosConciliacao
                    });


                    varSaldoInicialMes = varSaldoCalculado;
                    DataInicial = DataInicial.AddDays(1);
                }
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(listMapaConciliacao);

                if (erro == "")
                {
                    return Json(listMapaConciliacao, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception(erro);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                if (erro == "")
                {
                    return Json("OPS! - " + ex.Message, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("OPS! - " + erro, JsonRequestBehavior.AllowGet);
                }
            }
        }


        public decimal ApuraDiferencaSaldo(decimal varSaldoCalculado, DateTime data, int banco)
        {

            decimal varSaldoReal = 0;
            decimal varDiferenca = 0;

            //Compara com Saldo Real e testa diferença
            var sdoReal = new SaldoBancarioReal().ObterBancoData(banco, data, _paramBase);
            if (sdoReal != null)
            {
                varSaldoReal = sdoReal.saldoFinal;
                varDiferenca = sdoReal.saldoFinal - varSaldoCalculado;
            }
            else
            {
                varSaldoReal = 0;
                varDiferenca = varSaldoCalculado;
            }
            return varDiferenca;
        }
        public decimal CalculaSaldoFinal(decimal varSaldoInicial, DateTime DataInicial, DateTime DataFinal, int banco)
        {
            decimal varSaldoCalculado = varSaldoInicial;

            var bcomovAnterior = new BancoMovimento().ObterTodosDataBanco(DataInicial, DataFinal, banco,_paramBase);
            foreach (var item in bcomovAnterior)
            {
                if (item.TipoMovimento.Codigo == "ENT")
                {
                    varSaldoCalculado += item.valor;
                }
                else if (item.TipoMovimento.Codigo == "SAI")
                {
                    varSaldoCalculado -= item.valor;
                }
            }
            return varSaldoCalculado;
        }
        public decimal ObtemSaldoInicialAno(DateTime DataInicial, int banco)
        {
            

            //Calcular Saldo final
            string varAno = DataInicial.Year.ToString();
            DateTime dataInicialAno = Convert.ToDateTime("01/01/" + varAno);
            DateTime dataAnteriorMovimento = DataInicial;
            SaldoBancarioInicial saldo = new SaldoBancarioInicial();
            var aux1 = saldo.ObterTodos(_paramBase);
            var aux2 = aux1.Where(x => x.Ano == varAno && x.banco_id == banco);
            var varSaldoInicialAno = aux2.FirstOrDefault();

            if (varSaldoInicialAno == null)
            {
                erro += "Saldo Inicial não encontrado ref: " + dataInicialAno.ToShortDateString();
                throw new Exception(erro);
            }
            return varSaldoInicialAno.saldoInicial;
        }
    }
}
