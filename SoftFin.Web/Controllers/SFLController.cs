using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class SFLController : BaseController
    {
        //
        // GET: /SFL/

        public class dtoCPAG
        {
            public string flag_DebCred { get; set; }

            public int idCPAG { get; set; }
            public int idCC { get; set; }
            public int idDetalhe { get; set; }
            public int idOV { get;  set; }

            public string descricao { get; set; }
            public decimal valor { get; set; }
            public string favorecido { get; set; }
            public DateTime Vencimento { get; set; }
            public DateTime? NovoVencimento { get; set; }
            public bool cancelarMovimentacao { get; set; }
            public int capa_id { get; set; }
            public string StatusPagamento { get; set; }
            public string erro { get; set; }
            public string plano { get; set; }
        }

        public class dtoSaldo
        {
            public DateTime data { get; set; }

            public decimal lactocred { get; set; }
            public decimal lactocredmov { get; set; }
            public decimal lactocredrec { get; set; }

            public decimal lactodeb { get; set; }
            public decimal lactodebmov { get; set; }
            public decimal lactodebrec { get; set; }

            public decimal lactosaldo { get; set; }
            public decimal saldofinal { get; set; }
            public string cor { get; set; }
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListaSFCapas()
        {
            try
            {
                var objs = new SFCapa().ObterTodos(_estab);
                return Json(new {
                    CDStatus = "OK",
                    objs = objs.Select(p => new
                    {
                        dataFinal = p.dataFinal.ToString("o"),
                        dataInicial = p.dataInicial.ToString("o"),
                        p.descricao,
                        p.estabelecimento_id,
                        p.id,
                        p.SaldoInicial,
                        situacao = DescricaoSituacao(p.situacao),
                        p.usuarioalteracaoid,
                        p.usuarioinclusaoid,
                        banco = (p.banco_id == 0) ? "" : String.Format("{0} - {1} - {2} - {3}", p.Banco.codigoBanco, p.Banco.nomeBanco, p.Banco.agencia, p.Banco.contaCorrente),
                        incluirOrdemVenda= (p.incluirOrdemVenda) ? "Sim": "Não"
                   
                    })
                }
                , JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return Json(new {
                    CDStatus = "NOK",
                    DSMessage = ex.ToString()
                    }
                    , JsonRequestBehavior.AllowGet);
                
            }
        }

        public string DescricaoSituacao(string item)
        {
            //S - Simulãção / A - Applicado / E- Excluido
            switch (item)
	        {
                case "S":
                    return "Simulando";
                case "A":
                    return "Aplicado";
                case "E":
                    return "Excluido";
                default:
                    return "Desconhecido";

	        }
        }

        public JsonResult ListaSFSaldos(int id)
        {
            try
            {
     
                var objcapa = new SFCapa().ObterPorId(id);
                var objdet = new SFDetalhe().ObterPorCapa(id);
                var banco = new Banco().ObterPorId(objcapa.banco_id, _paramBase);
                var valorLimite = banco.ValorLimite;

                //var objCPAGs = new DocumentoPagarParcela().ObterEntreDataVencimento(objcapa.dataInicial,objcapa.dataFinal, _paramBase,_estab).Where(p => p.DocumentoPagarMestre.banco_id == objcapa.banco_id);

                var objCC = new BancoMovimento().ObterTodosData(objcapa.dataInicial, objcapa.dataFinal, _paramBase).Where(p =>  p.banco_id == objcapa.banco_id);
                var objOV = new OrdemVenda().ObterEntreData(objcapa.dataInicial, objcapa.dataFinal, _paramBase);

                var lista = new List<dtoSaldo>();

                var graficoData = new List<string>();
                var graficoSaldo = new List<decimal>();
                var graficoValorCredito = new List<decimal>();
                var graficoValorDebito = new List<decimal>();


                var saldoInicial = objcapa.SaldoInicial;

                for (DateTime i = objcapa.dataInicial; i < objcapa.dataFinal.AddDays(1); i = i.AddDays(1))
			    {
			        dtoSaldo dtoSaldoaux = new dtoSaldo();


                    var dataInicial = new DateTime(i.Year, i.Month, i.Day);
                    var dataFinal = new DateTime(i.Year, i.Month, i.Day, 23, 59, 59);




                    dtoSaldoaux.lactocred = objCC.Where(p => p.data >= dataInicial && p.data <= dataFinal && p.PlanoDeConta.DebitoCredito == "C").Sum(p => p.valor);
                    if (objcapa.incluirOrdemVenda)
                        dtoSaldoaux.lactocred = dtoSaldoaux.lactocred + 
                            objOV.Where(p => p.data >= dataInicial && p.data <= dataFinal 
                            && (p.statusParcela_ID == 1 || p.statusParcela_ID == 4)).Sum(p => p.valor);
                    //dtoSaldoaux.lactocred -= objdet.Where(p => p.VencimentoOriginal >= dataInicial && p.VencimentoOriginal <= dataFinal && p.flag_debcred == "C").Sum(p => p.valor);



                    dtoSaldoaux.lactodeb = objCC.Where(p => p.data >= dataInicial && p.data <= dataFinal && p.PlanoDeConta.DebitoCredito == "D").Sum(p => p.valor);
                    dtoSaldoaux.data = i;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          

                    dtoSaldoaux.lactodebrec = objdet.Where(p => p.novoVencimento >= dataInicial && p.novoVencimento <= dataFinal && p.flag_debcred == "D").Sum(p => p.valor);
                    dtoSaldoaux.lactodebmov = objdet.Where(p => p.VencimentoOriginal >= dataInicial && p.VencimentoOriginal <= dataFinal && p.flag_debcred == "D").Sum(p => p.valor);
                    dtoSaldoaux.lactodeb -= dtoSaldoaux.lactodebmov;

                    dtoSaldoaux.lactocredrec = objdet.Where(p => p.novoVencimento >= dataInicial && p.novoVencimento <= dataFinal && p.flag_debcred == "C").Sum(p => p.valor);
                    dtoSaldoaux.lactocredmov = objdet.Where(p => p.VencimentoOriginal >= dataInicial && p.VencimentoOriginal <= dataFinal && p.flag_debcred == "C").Sum(p => p.valor);
                    dtoSaldoaux.lactocred -= dtoSaldoaux.lactocredmov;

                    dtoSaldoaux.lactosaldo = ((((dtoSaldoaux.lactocred - dtoSaldoaux.lactodeb) + dtoSaldoaux.lactodebmov)) - dtoSaldoaux.lactodebrec);
                    dtoSaldoaux.lactosaldo += (dtoSaldoaux.lactocredrec - dtoSaldoaux.lactocredmov);

                    dtoSaldoaux.saldofinal = dtoSaldoaux.lactosaldo + saldoInicial;   
                    saldoInicial = dtoSaldoaux.saldofinal;

                    

                    if (saldoInicial <= 0)
                    {
                        if (valorLimite == null)
                        {
                            dtoSaldoaux.cor = "red";
                        }
                        else
                        {
                            if (saldoInicial < (-1*(valorLimite)))
                            {
                                dtoSaldoaux.cor = "red";
                            }
                            else
                            {
                                dtoSaldoaux.cor = "#FFA62F";
                            }
                        }
                    }
                    else
                    {
                        dtoSaldoaux.cor = "#3EA055";
                    }


                    if ((dtoSaldoaux.lactocred != 0) 
                        || (dtoSaldoaux.lactodeb != 0)
                        || (dtoSaldoaux.lactodebrec != 0)
                        || (dtoSaldoaux.lactodebmov != 0)
                        || (dtoSaldoaux.lactocredmov != 0)
                        || (dtoSaldoaux.lactocredrec != 0)
                        )
                    {



                        graficoData.Add(dtoSaldoaux.data.ToShortDateString());
                        graficoSaldo.Add(dtoSaldoaux.saldofinal);
                        graficoValorCredito.Add(dtoSaldoaux.lactocred);
                        graficoValorDebito.Add(dtoSaldoaux.lactodeb + dtoSaldoaux.lactodebrec - dtoSaldoaux.lactodebmov);
                        lista.Add(dtoSaldoaux);
                    }
			    }

                var al = new ArrayList();
                al.Add(graficoSaldo.ToArray());
                al.Add(graficoValorCredito.ToArray());
                al.Add(graficoValorDebito.ToArray());


                return Json(new {
                    CDStatus = "OK",
                    grafico = new {
                        labels = graficoData,
                        data = al
                    },
                    objs = lista.Select(p => new
                    {
                        data = p.data.ToString("o"),
                        p.cor,
                        p.lactocred,
                        p.lactodeb ,
                        p.lactodebmov,
                        p.lactodebrec,
                        p.lactocredmov,
                        p.lactocredrec,
                        p.lactosaldo,
                        p.saldofinal
                        
                    })
                }
                , JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return Json(new {
                    CDStatus = "NOK",
                    DSMessage = ex.ToString()
                    }
                    , JsonRequestBehavior.AllowGet);
                
            }

        }

        public JsonResult ListaBanco()
        {
            var con1 = new Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return Json(listret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaSFDetalhes(int id ,DateTime data)
        {
            try
            {
                var objcapa = new SFCapa().ObterPorId(id);
                var dataInicial = new DateTime(data.Year, data.Month, data.Day);
                var dataFinal = new DateTime(data.Year, data.Month, data.Day, 23,59,59);
                var objBMV = new BancoMovimento().ObterTodosData(dataInicial, dataFinal, _paramBase);
                var objsDetalhe = new SFDetalhe().ObterTodos(id);
                var lista = new List<dtoCPAG>();

                foreach (var item in objBMV)
	            {
		            var auxdet = objsDetalhe.Where(p => p.idAfetado == item.id && p.tabela != "OV").FirstOrDefault();
                    if (auxdet == null)
                    {
                        if (item.DocumentoPagarParcela_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome + " - " + item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.cnpj,
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = "D",
                                Vencimento = item.data,
                                idCC = item.id,
                                StatusPagamento = (item.DocumentoPagarParcela.statusPagamento == 1) ? "Em Aberto" : (item.DocumentoPagarParcela.statusPagamento == 2) ? "Pago" : "Pago Parcial"
                            });
                        }
                        else if (item.notafiscal_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = item.NotaFiscal.NotaFiscalPessoaTomador.razao + " - " + (item.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf != null) ?? item.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf,
                                valor = item.valor,
                                capa_id = id,
                                flag_DebCred = "C",
                                Vencimento = item.data,
                                idCC = item.id,
                                StatusPagamento = (item.NotaFiscal.SituacaoRecebimento == 1) ? "Em Aberto" : (item.NotaFiscal.SituacaoRecebimento == 2) ? "Recebido Parcialmente" : "Recebido Integralmente"
                            });
                        }
                        else if (item.recebimento_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.razao + " - " + (item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf != null) ?? item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf,
                                valor = item.valor,
                                capa_id = id,
                                flag_DebCred = "C",
                                Vencimento = item.data,
                                idCC = item.id,
                                StatusPagamento = (item.Recebimento.notaFiscal.SituacaoRecebimento == 1) ? "Em Aberto" : (item.Recebimento.notaFiscal.SituacaoRecebimento == 2) ? "Recebido Parcialmente" : "Recebido Integralmente"
                            });
                        }
                        else
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = "",
                                valor = item.valor,
                                capa_id = id,
                                flag_DebCred = item.PlanoDeConta.DebitoCredito,
                                Vencimento = item.data,
                                idCC = item.id,
                                StatusPagamento = ""
                            });
                        }

                    }
                    else
                    {
                        if (item.DocumentoPagarParcela_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome + " - " + item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.cnpj,
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = "D",
                                Vencimento = item.data,
                                NovoVencimento = auxdet.novoVencimento,
                                idDetalhe = auxdet.id,
                                
                                StatusPagamento = (item.DocumentoPagarParcela.statusPagamento == 1) ? "Em Aberto" : (item.DocumentoPagarParcela.statusPagamento == 2) ? "Pago" : "Pago Parcial"
                            });
                        }
                        else if (item.notafiscal_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = item.NotaFiscal.NotaFiscalPessoaTomador.razao + " - " + (item.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf != null) ?? item.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf,
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = "C",
                                Vencimento = item.data,
                                NovoVencimento = auxdet.novoVencimento,
                                idDetalhe = auxdet.id,
                                StatusPagamento = (item.NotaFiscal.SituacaoRecebimento == 1) ? "Em Aberto" : (item.NotaFiscal.SituacaoRecebimento == 2) ? "Recebido Parcialmente" : "Recebido Integralmente"
                            });
                        }
                        else if (item.recebimento_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.razao + " - " + (item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf != null) ?? item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.cnpjCpf,
                                valor = item.valor,
                                capa_id = id,
                                flag_DebCred = "C",
                                Vencimento = item.data,
                                idDetalhe = auxdet.id,
                                StatusPagamento = (item.Recebimento.notaFiscal.SituacaoRecebimento == 1) ? "Em Aberto" : (item.Recebimento.notaFiscal.SituacaoRecebimento == 2) ? "Recebido Parcialmente" : "Recebido Integralmente"
                            });
                        }
                        else
                        {
                            lista.Add(new dtoCPAG
                            {
                                plano = item.PlanoDeConta.codigo + " - " + item.PlanoDeConta.descricao,
                                descricao = item.historico,
                                favorecido = "",
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = item.PlanoDeConta.DebitoCredito,
                                Vencimento = item.data,
                                NovoVencimento = auxdet.novoVencimento,
                                idDetalhe = auxdet.id,
                                StatusPagamento = ""
                            });
                        }
                    }
	            }

                var objOV = new OrdemVenda().ObterEntreData(dataInicial, dataFinal, _paramBase).Where( p =>
                            p.statusParcela_ID == 1 || p.statusParcela_ID == 4);

                foreach (var item in objOV)
                {
                    var auxdet = objsDetalhe.Where(p => p.idAfetado == item.id && p.tabela == "OV").FirstOrDefault();
                    if (auxdet == null)
                    {
                        lista.Add(new dtoCPAG
                        {
                            plano = "",
                            descricao = item.descricao,
                            favorecido = item.Pessoa.nome + " - " + (item.Pessoa.cnpj != null) ?? item.Pessoa.cnpj,
                            valor = item.valor,
                            capa_id = id,
                            idDetalhe = 0,
                            idOV = item.id,
                            flag_DebCred = "C",
                            Vencimento = item.data,
                            StatusPagamento = item.statusParcela.status 
                        });
                    }
                    else
                    {
                        lista.Add(new dtoCPAG
                        {
                            plano = "",
                            descricao = item.descricao,
                            favorecido = item.Pessoa.nome + " - " + item.Pessoa.cnpj,
                            valor = item.valor,
                            capa_id = id,
                            idDetalhe = auxdet.id,
                            idOV = item.id,
                            flag_DebCred = "C",
                            Vencimento = item.data,
                            StatusPagamento = " Ordem " + item.statusParcela.status,
                            NovoVencimento = auxdet.novoVencimento,
                            
                        });
                    }
                }


                return Json(new {
                    CDStatus = "OK",
                    objs = lista.Select(p => new
                    {
                        Vencimento = p.Vencimento.ToString("o"),
                        p.descricao,
                        p.favorecido,
                        NovoVencimento = (p.NovoVencimento == null) ? "" : p.NovoVencimento.Value.ToString("o"),
                        p.valor,
                        p.idCC,
                        p.idDetalhe,
                        p.idOV,
                        p.capa_id,
                        p.StatusPagamento,
                        p.erro,
                        p.cancelarMovimentacao,
                        p.plano,
                        p.flag_DebCred

                    })
                }
                , JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return Json(new {
                    CDStatus = "NOK",
                    DSMessage = ex.ToString()
                    }
                    , JsonRequestBehavior.AllowGet);
                
            }

        }
        public JsonResult ListaSFDebitosRecebidos(int id, DateTime data)
        {
            try
            {

                var objcapa = new SFCapa().ObterPorId(id);
                var dataInicial = new DateTime(data.Year, data.Month, data.Day);
                var dataFinal = new DateTime(data.Year, data.Month, data.Day, 23, 59, 59);
                var objsDetalhes = new SFDetalhe().ObterTodosTranferidos(dataInicial, dataFinal, id);
                var lista = new List<dtoCPAG>();

                foreach (var item in objsDetalhes.Where(p => p.tabela != "OV"))
                {

                    var objBMV = new BancoMovimento().ObterPorId(item.idAfetado);

                    if (objBMV != null)
                    {
                        if (objBMV.DocumentoPagarParcela_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                descricao = objBMV.historico,
                                favorecido = objBMV.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome + " - " + objBMV.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.cnpj,
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = "D",
                                Vencimento = item.VencimentoOriginal,
                                NovoVencimento = null,
                                idDetalhe = item.id,
                                StatusPagamento = (objBMV.DocumentoPagarParcela.DocumentoPagarMestre.StatusPagamento == 1) ? "Em Aberto" : (objBMV.DocumentoPagarParcela.DocumentoPagarMestre.StatusPagamento == 2) ? "Pago" : "Pago Parcial"
                            });
                        }
                        else if (objBMV.notafiscal_id != null)
                        {
                            lista.Add(new dtoCPAG
                            {
                                descricao = objBMV.historico,
                                favorecido = objBMV.NotaFiscal.NotaFiscalPessoaTomador.razao + " - " + objBMV.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf,
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = "C",
                                Vencimento = objBMV.data,
                                NovoVencimento = null,
                                idDetalhe = objBMV.id,
                                StatusPagamento = (objBMV.NotaFiscal.SituacaoRecebimento == 1) ? "Em Aberto" : (objBMV.DocumentoPagarParcela.DocumentoPagarMestre.StatusPagamento == 2) ? "Recebido Parcialmente" : "Recebido Integralmente"
                            });
                        }
                        else
                        {
                            lista.Add(new dtoCPAG
                            {
                                descricao = objBMV.historico,
                                favorecido = "",
                                valor = item.valor,
                                capa_id = id,
                                idCPAG = item.id,
                                flag_DebCred = objBMV.PlanoDeConta.DebitoCredito,
                                Vencimento = objBMV.data,
                                NovoVencimento = null,
                                idDetalhe = item.id,
                                StatusPagamento = ""
                            });
                        }
                    }

                }

                foreach (var item in objsDetalhes.Where(p => p.tabela == "OV"))
                {

                    var objOV = new OrdemVenda().ObterPorId(item.idAfetado);


                    lista.Add(new dtoCPAG
                    {
                        plano = "",
                        descricao = objOV.descricao,
                        favorecido = objOV.Pessoa.nome + " - " + objOV.Pessoa.cnpj,
                        valor = item.valor,
                        capa_id = id,
                        idDetalhe = item.id,
                        idOV = item.idAfetado,
                        NovoVencimento = null,
                        flag_DebCred = "C",
                        Vencimento = objOV.data,
                        StatusPagamento = objOV.statusParcela.status
                    });

                }


                return Json(new
                {
                    CDStatus = "OK",
                    objs = lista.Select(p => new
                    {
                        Vencimento = p.Vencimento.ToString("o"),
                        p.descricao,
                        p.favorecido,
                        NovoVencimento = (p.NovoVencimento == null) ? "" : p.NovoVencimento.Value.ToString("o"),
                        p.valor,
                        p.idCPAG,
                        p.idDetalhe,
                        p.idOV,
                        p.idCC,
                        p.capa_id,
                        p.StatusPagamento,
                        p.erro

                    })
                }
                , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new
                {
                    CDStatus = "NOK",
                    DSMessage = ex.ToString()
                }
                    , JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult ListaSFCapaPorId(int id)
        {
            try
            {
                var obj = new SFCapa().ObterPorId(id);

                if (obj == null)
                {
                    obj = new SFCapa();
                    obj.estabelecimento_id = _estab;
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    obj.dataFinal = obj.dataInicial.AddDays(31).AddHours(23).AddHours(59);
                    obj.situacao = "S";
                }

                return Json(new
                {
                    CDStatus = "OK",
                    item = new
                    {
                        dataFinal = obj.dataFinal.ToString("o"),
                        dataInicial = obj.dataInicial.ToString("o"),
                        obj.descricao,
                        obj.estabelecimento_id,
                        obj.id,
                        obj.SaldoInicial,
                        obj.situacao,
                        obj.usuarioalteracaoid,
                        obj.usuarioinclusaoid,
                        banco_id = obj.banco_id.ToString(),
                        obj.incluirOrdemVenda
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return Json(new {
                    CDStatus = "NOK",
                    DSMessage = ex.ToString()
                    }
                    , JsonRequestBehavior.AllowGet);
                
            }

        }

        //public JsonResult ListaSFDetalhePorId(int id)
        //{

        //}

        public JsonResult SalvarCapa(SFCapa obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

      


                if (obj.id == 0)
                {
                    if (obj.Incluir(obj, _paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Erro ao incluir" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ExcluirCapa(SFCapa obj)
        {
            try
            {

                string Erro = "";
                if (obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir " }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult   SalvarDetalhe(List<dtoCPAG> objs)
        {
            try
            {
                var sfcapa = new SFCapa();
                var erro = false;

                foreach (var item in objs)
                {
                    if (!item.cancelarMovimentacao)
                    {
                        if (item.NovoVencimento != null)
                        {
                            var capa = sfcapa.ObterPorId(item.capa_id);
                            var dataInicial = new DateTime(capa.dataInicial.Year, capa.dataInicial.Month, capa.dataInicial.Day);
                            var dataFinal = new DateTime(capa.dataFinal.Year, capa.dataFinal.Month, capa.dataFinal.Day, 23, 59, 59);

                            var dataVencimento = new DateTime(item.NovoVencimento.Value.Year, item.NovoVencimento.Value.Month, item.NovoVencimento.Value.Day);

                            if ((dataVencimento >= dataInicial)
                                && (dataVencimento <= dataFinal))
                            {
                                item.erro = "";
                            }
                            else
                            {
                                erro = true;
                                item.erro = "Período permitido é entre " + dataInicial.ToShortDateString() + " até " + dataFinal.ToShortDateString();
                            }


                            if ((item.Vencimento.Year == item.NovoVencimento.Value.Year) &&
                                (item.Vencimento.Month == item.NovoVencimento.Value.Month) &&
                                (item.Vencimento.Day == item.NovoVencimento.Value.Day))
                            {
                                erro = true;
                                item.erro = "Informe uma data diferente da data original";
                            }

                        }  

                    }
                    else
                    {
                        item.erro = "";
                    }

                }

                if  (erro)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Atenção existem erros nas informações",
                        Objs = objs.Select(p => new
                        {
                            Vencimento = p.Vencimento.ToString("o"),
                            p.descricao,
                            p.favorecido,
                            NovoVencimento = (p.NovoVencimento == null) ? "" : p.NovoVencimento.Value.ToString("o"),
                            p.valor,
                            p.idCPAG,
                            p.idDetalhe,
                            p.capa_id,
                            p.StatusPagamento,
                            p.erro

                        })
                    }, JsonRequestBehavior.AllowGet);
                }


                foreach (var item in objs)
                {
                    var db = new DbControle();
                    var capa = sfcapa.ObterPorId(item.capa_id);
                    var sfdet = new SFDetalhe().ObterPorCpagCapa(item.idDetalhe, item.capa_id, db);


                    if (sfdet != null && item.cancelarMovimentacao)
                    {
                        string erros = "";
                        sfdet.Excluir(item.idDetalhe, ref erros, _paramBase);
                        continue;
                    }


                    if (item.NovoVencimento == null)
                    {
                        if (sfdet != null)
                        {
                            string erros = "";
                            sfdet.Excluir(item.idDetalhe,ref erros, _paramBase);
                        }
                    }
                    else
                    {
                        var dataVencimento = new DateTime(item.NovoVencimento.Value.Year, item.NovoVencimento.Value.Month, item.NovoVencimento.Value.Day);

                        if (capa != null)
                        {
                            var dataInicial = new DateTime(capa.dataInicial.Year, capa.dataInicial.Month, capa.dataInicial.Day);
                            var dataFinal = new DateTime(capa.dataFinal.Year, capa.dataFinal.Month, capa.dataFinal.Day, 23, 59, 59);

                            if ((dataVencimento >= dataInicial)
                                && (dataVencimento <= dataFinal))
                            {
                                if (sfdet == null)
                                    sfdet = new SFDetalhe();
                                sfdet.novoVencimento = dataVencimento;
                                sfdet.VencimentoOriginal = item.Vencimento;
                                sfdet.SFCapa_id = item.capa_id;
                                sfdet.valor = item.valor;
                                sfdet.flag_debcred = item.flag_DebCred;

                                if (item.idCC != 0)
                                {
                                    sfdet.tabela = "CC";
                                    sfdet.idAfetado = item.idCC;

                                }
                                else
                                {
                                    sfdet.tabela = "OV";
                                    sfdet.idAfetado = item.idOV;

                                }

                                if (sfdet.id != 0)
                                {
                                    sfdet.Alterar(_paramBase, db);
                                }
                                else
                                {
                                    sfdet.Incluir(_paramBase);
                                }
                            }
                        }
                    }

                }

                return Json(new { CDStatus = "OK", DSMessage = "Salvo com sucesso" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message, Objs = "" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
