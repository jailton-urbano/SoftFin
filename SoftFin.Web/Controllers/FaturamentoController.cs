using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Regras;

namespace SoftFin.Web.Controllers
{
    public class FaturamentoController : BaseController
    {
        
        [HttpPost]
        public JsonResult TotalizadorEmissaoNFSe(int? id)
        {
            base.TotalizadorDash(id);
            var situacaoliberada = StatusParcela.SituacaoLiberada();
            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase);
            var soma2  = soma.Where(p => p.usuarioAutorizador != null && p.statusParcela_ID == situacaoliberada).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma2 }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult TotalizadorPrevFaturamento(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult PrevisaoFaturamento()
        {
            try
            {
                var ano = string.Empty;
                var mes = string.Empty;
                var unidade = string.Empty;

                if (Request.QueryString["AnoID"] != null)
                {
                    ano = Request.QueryString["AnoID"].ToString();
                }
                if (Request.QueryString["MesID"] != null)
                {
                    mes = Request.QueryString["MesID"].ToString();
                }
                if (Request.QueryString["UnidadeID"] != null)
                {
                    unidade = Request.QueryString["UnidadeID"].ToString();
                }
                var Listas = Pesquisa(ano, mes, unidade);
                return View();
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        private List<ParcelasView> Pesquisa(string AnoID, string MesID, string UnidadeID)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            var banco = new DbControle();

            var con1 = from parcela in new OrdemVenda().ObterTodos(_paramBase)
                       group parcela.data.Year by new { parcela.data.Year }
                           into g
                           select new AnoParcelaView { ano = g.Key.Year };

            ViewData["Ano"] = new SelectList(con1, "ano", "ano");

            var con2 = from parcela in new OrdemVenda().ObterTodos(_paramBase)
                       group parcela.data.Month by new { parcela.data.Month }
                           into g
                           select new MesParcelaView { mes = g.Key.Month };
            ViewData["Mes"] = new SelectList(con2, "mes", "mes");

            var con3 = from c in new UnidadeNegocio().ObterTodos(_paramBase)
                       select c;
            ViewData["UnidadeNegocio"] = new SelectList(con3, "id", "unidade");

            if ((UnidadeID == null || UnidadeID == string.Empty) &
                (AnoID == null || AnoID == string.Empty) &
                 (MesID == null || MesID == string.Empty))
            {
                var parcelas = new List<ParcelasView>();

                parcelas = (from parcela in banco.OrdemVenda
                            where parcela.estabelecimento_id == estab
                            group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                            into g
                            select new ParcelasView
                            {
                                mes = g.Key.mes,
                                ano = g.Key.ano,
                                unidade = g.Key.unidade,
                                valor = g.Sum(x => x.valor)
                            }).OrderBy(p => p.ano).ToList();
 

                ViewData["Parcelas"] = parcelas;
                return parcelas;
            }
            else
            {
                if ((UnidadeID != null || UnidadeID != string.Empty) &
                    (AnoID == null || AnoID == string.Empty) &
                     (MesID == null || MesID == string.Empty))
                {
                    var UN = Convert.ToInt32(UnidadeID);
                    var parcelas = new List<ParcelasView>();
                    parcelas = (from parcela in banco.OrdemVenda
                                where parcela.estabelecimento_id == estab
                                & parcela.unidadeNegocio_ID == UN
                                group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                                    into g
                                    select new ParcelasView
                                    {
                                        mes = g.Key.mes,
                                        ano = g.Key.ano,
                                        unidade = g.Key.unidade,
                                        valor = g.Sum(x => x.valor)
                                    }).OrderBy(p => p.ano).ToList();

                    ViewData["Parcelas"] = parcelas;
                    return parcelas;
                }
                else
                {
                    if ((UnidadeID == null || UnidadeID == string.Empty) &
                        (AnoID != null || AnoID != string.Empty) &
                         (MesID == null || MesID == string.Empty))
                    {
                        var ano = Convert.ToInt32(AnoID);
                        var parcelas = new List<ParcelasView>();

                        parcelas = (from parcela in banco.OrdemVenda
                                    where parcela.estabelecimento_id == estab
                                    & parcela.data.Year == ano
                                    group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                                        into g
                                        select new ParcelasView
                                        {
                                            mes = g.Key.mes,
                                            ano = g.Key.ano,
                                            unidade = g.Key.unidade,
                                            valor = g.Sum(x => x.valor)
                                        }).OrderBy(p => p.ano).ToList();


                        ViewData["Parcelas"] = parcelas;
                        return parcelas;
                    }
                    else
                    {
                        if ((UnidadeID == null || UnidadeID == string.Empty) &
                             (AnoID == null || AnoID == string.Empty) &
                             (MesID != null || MesID != string.Empty))
                        {
                            var mes = Convert.ToInt32(MesID);
                            var parcelas = new List<ParcelasView>();
                            parcelas = (from parcela in banco.OrdemVenda
                                        where parcela.estabelecimento_id == estab
                                        & parcela.data.Month == mes
                                        group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                                            into g
                                            select new ParcelasView
                                            {
                                                mes = g.Key.mes,
                                                ano = g.Key.ano,
                                                unidade = g.Key.unidade,
                                                valor = g.Sum(x => x.valor)
                                            }).OrderBy(p => p.ano).ToList();
       

                            ViewData["Parcelas"] = parcelas;
                            return parcelas;
                        }
                        else
                        {
                            if ((UnidadeID == null || UnidadeID == string.Empty) &
                                (AnoID != null || AnoID != string.Empty) &
                                (MesID != null || MesID != string.Empty))
                            {
                                var ano = Convert.ToInt32(AnoID);
                                var mes = Convert.ToInt32(MesID);
                                var parcelas = new List<ParcelasView>();
                                parcelas = (from parcela in banco.OrdemVenda
                                            where parcela.estabelecimento_id == estab
                                            & parcela.data.Year == ano & parcela.data.Month == mes
                                            group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                                                into g
                                                select new ParcelasView
                                                {
                                                    mes = g.Key.mes,
                                                    ano = g.Key.ano,
                                                    unidade = g.Key.unidade,
                                                    valor = g.Sum(x => x.valor)
                                                }).OrderBy(p => p.ano).ToList();
                                                            //from unidade in banco.UnidadeNegocio
                                                            //join contrato in banco.Contrato
                                                            //on unidade.id equals contrato.UnidadeNegocio.id
                                                            //join parcela in banco.ParcelaContrato
                                                            //on contrato.id equals parcela.ContratoItem.contrato_id
                                                            //where parcela.data.Year == ano & parcela.data.Month == mes
                                                            //group parcela by new { parcela.data, unidade.unidade }
                                                            //    into g
                                                            //    select new ParcelasView
                                                            //    {
                                                            //        mes = g.Key.data.Month,
                                                            //        ano = g.Key.data.Year,
                                                            //        unidade = g.Key.unidade,
                                                            //        valor = g.Sum(x => x.valor)
                                                                //};
                                ViewData["Parcelas"] = parcelas;
                                return parcelas;
                            }
                            else
                            {
                                if ((UnidadeID != null || UnidadeID != string.Empty) &
                                    (AnoID == null || AnoID == string.Empty) &
                                    (MesID != null || MesID != string.Empty))
                                {
                                    var UN = Convert.ToInt32(UnidadeID);
                                    var mes = Convert.ToInt32(MesID);
                                    var parcelas = new List<ParcelasView>();
                                    parcelas = (from parcela in banco.OrdemVenda
                                                where parcela.estabelecimento_id == estab
                                                & parcela.unidadeNegocio_ID == UN & parcela.data.Month == mes
                                                group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                                                    into g
                                                    select new ParcelasView
                                                    {
                                                        mes = g.Key.mes,
                                                        ano = g.Key.ano,
                                                        unidade = g.Key.unidade,
                                                        valor = g.Sum(x => x.valor)
                                                    }).OrderBy(p => p.ano).ToList();
                                                                //from unidade in banco.UnidadeNegocio
                                                                //join contrato in banco.Contrato
                                                                //on unidade.id equals contrato.UnidadeNegocio.id
                                                                //join parcela in banco.ParcelaContrato
                                                                //on contrato.id equals parcela.ContratoItem.contrato_id
                                                                //where unidade.id == UN & parcela.data.Month == mes
                                                                //group parcela by new { parcela.data, unidade.unidade }
                                                                //    into g
                                                                //    select new ParcelasView
                                                                //    {
                                                                //        mes = g.Key.data.Month,
                                                                //        ano = g.Key.data.Year,
                                                                //        unidade = g.Key.unidade,
                                                                //        valor = g.Sum(x => x.valor)
                                                                //    };
                                    ViewData["Parcelas"] = parcelas;
                                    return parcelas;
                                }
                                else
                                {
                                    var UN = Convert.ToInt32(UnidadeID);
                                    var ano = Convert.ToInt32(AnoID);
                                    var parcelas = new List<ParcelasView>();
                                    parcelas = (from parcela in banco.OrdemVenda
                                                where parcela.estabelecimento_id == estab
                                                & parcela.data.Year == ano & parcela.unidadeNegocio_ID == UN
                                                group parcela by new { ano = parcela.data.Year, mes = parcela.data.Month, parcela.UnidadeNegocio.unidade }
                                                    into g
                                                    select new ParcelasView
                                                    {
                                                        mes = g.Key.mes,
                                                        ano = g.Key.ano,
                                                        unidade = g.Key.unidade,
                                                        valor = g.Sum(x => x.valor)
                                                    }).OrderBy(p => p.ano).ToList();

                                    ViewData["Parcelas"] = parcelas;
                                    return parcelas;
                                }
                            }
                        }
                    }
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            try
            {
                var ano = string.Empty;
                var mes = string.Empty;
                var unidade = string.Empty;

                if (Request.QueryString["AnoID"] != null)
                {
                    ano = Request.QueryString["AnoID"].ToString();
                }
                if (Request.QueryString["MesID"] != null)
                {
                    mes = Request.QueryString["MesID"].ToString();
                }
                if (Request.QueryString["UnidadeID"] != null)
                {
                    unidade = Request.QueryString["UnidadeID"].ToString();
                }
                var Listas = Pesquisa(ano, mes, unidade);

                var totalRecords = Listas.Count();

                var response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                foreach (var item in
                    Listas.OrderBy(p => p.ano & p.mes).Skip(12 * request.PageIndex).Take(12).ToList())
                {
                    response.Records.Add(new JqGridRecord(Convert.ToString(item.mes), new List<object>()
                {
                    item.ano,
                    item.mes,
                    item.unidade,
                    item.valor.ToString("n")
                }));
                }


                return new JqGridJsonResult() { Data = response };
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ListasEmissaoNfe(JqGridRequest request)
        {
            var data = string.Empty;
            var contrato = string.Empty;


            if (Request.QueryString["data"] != null)
            {
                data = Request.QueryString["Data"].ToString();
            }
            if (Request.QueryString["contrato"] != null)
            {
                contrato = Request.QueryString["Contrato"].ToString();
            }
            var Listas = PesquisaNFse(contrato, data);


            var totalRecords = Listas.Count();

            var response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };



            foreach (var item in
                Listas.OrderBy(p => p.data).Skip(12 * request.PageIndex).Take(12).ToList())
            {
                string contratogrid = "";
                string parcela = "";

                if (item.ParcelaContrato != null)
                {
                    contratogrid = item.ParcelaContrato.ContratoItem.Contrato.contrato;
                    parcela = item.ParcelaContrato.parcela.ToString();
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Numero.ToString(),
                    contratogrid,
                    parcela,
                    item.descricao,
                    item.Pessoa.nome,
                    item.data.ToString("dd/MM/yyyy"),
                    item.valor,
                    item.statusParcela.status
                }));
            }


            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index(string contrato, string data)
        {
            ViewData["data"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
            return View();
        }

        private IEnumerable<OrdemVenda> PesquisaNFse(string contrato, string data)
        {
            var obj = new OrdemVenda();
            IEnumerable<OrdemVenda> lista;

            if ((contrato == null || contrato == string.Empty) & (Request.QueryString["data"] == null))
            {
                var date = DateTime.Now.AddMonths(1);
                lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.data <= date);
                @ViewData["data"] = date.ToString("dd/MM/yyyy");
                @ViewData["contrato"] = contrato;
            }
            else
            {
                if (data == null || data == string.Empty)
                {
                    if (contrato == null || contrato == string.Empty)
                    {
                        lista = obj.ObterTodosPendentesAutorizadas(_paramBase);
                        @ViewData["data"] = string.Empty;
                        @ViewData["contrato"] = string.Empty;
                    }
                    else
                    {
                        lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.ParcelaContrato.ContratoItem.Contrato.contrato == contrato);
                        @ViewData["data"] = string.Empty;
                        @ViewData["contrato"] = contrato;
                    }
                }
                else
                {
                    if (contrato == null || contrato == string.Empty)
                    {
                        var date = Convert.ToDateTime(data);
                        date = DateTime.Now.AddMonths(1);
                        lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.data <= date);
                        @ViewData["data"] = date.ToString("dd/MM/yyyy");
                        @ViewData["contrato"] = contrato;
                    }
                    else
                    {
                        var date = Convert.ToDateTime(data);
                        date = DateTime.Now.AddMonths(1);
                        lista = obj.ObterTodosPendentesAutorizadas(_paramBase).Where(x => x.ParcelaContrato.ContratoItem.Contrato.contrato == contrato & x.data <= date);
                        @ViewData["data"] = date.ToString("dd/MM/yyyy");
                        @ViewData["contrato"] = contrato;
                    }
                }
            }
            return lista;
        }

        //public ActionResult Create(int ID)
        //{
        //    ViewBag.usuario = Acesso.UsuarioLogado();
        //    ViewBag.perfil = Acesso.PerfilLogado();

        //    CarregaCodigoServico();
        //    CarregaBanco();
        //    CarregaOperacao();

        //    var objNF = NotaFiscalController.CriaNota(ID, new DbControle());
        //    return View(objNF);
        //}



        //[HttpPost]
        //public ActionResult Create(NotaFiscal obj, string dataVencimento)
        //{
        //    string erros = "";
        //    try
        //    {
        //        CarregaCodigoServico();
        //        CarregaBanco();
        //        CarregaOperacao();



        //        var CodigoServicoID = obj.codigoServico;

        //        //var codigoservico = new CodigoServicoEstabelecimento().ObterPorId(CodigoServicoID);
        //        //if (obj.operacao_id == null)
        //        //{
        //        //    ModelState.AddModelError("", "Operação não configurada");
        //        //    return View(obj);
        //        //}

                
        //        OrdemVenda ov = new OrdemVenda().ObterPorId(obj.ordemVenda_id.Value);

        //        DateTime data = Convert.ToDateTime(dataVencimento);
        //        var nota = NotaFiscalCalculos.CalculaNota(obj, ov, new DbControle(), CodigoServicoID, obj.operacao_id, data,_estabobj);

        //        if (erros.Length != 0)
        //        {
        //            var textoNota = new NotaFiscalCalculos().MontaTextoNota(nota, new DbControle(), ov, ref erros);
        //            @ViewData["textoNota"] = textoNota;
        //            nota.discriminacaoServico = textoNota;
        //            obj = NotaFiscalController.CriaNota(ov.id, new DbControle());
        //            obj.banco_id = nota.banco_id;
        //            obj.valorNfse = nota.valorNfse;
        //            obj.operacao_id = nota.operacao_id;
        //            obj.dataEmissaoNfse = nota.dataEmissaoNfse;
        //            ModelState.AddModelError("", erros);
        //            ViewBag.msg = erros;
        //            return View(obj);
        //        }
        //        else
        //        {
        //            return RedirectToAction("Create", "NotaFiscal", new { @OrdemVenda = ov.id,  @CodigoServicoID = CodigoServicoID, @data = data, @idBanco = nota.banco_id, @idOperacao = nota.operacao_id, @valorNfse = nota.valorNfse });
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        ModelState.AddModelError("", ex.ToString());
        //        ViewBag.msg = ex.ToString();
        //        return View(obj);
        //    }
        //}



        private void CarregaCodigoServico()
        {
            var con1 = new CodigoServicoEstabelecimento().ObterTodos(_paramBase, _estabobj.Municipio_id);

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.CodigoServicoMunicipio.codigo, Text = String.Format("{0} - {1}", item.CodigoServicoMunicipio.codigo, item.CodigoServicoMunicipio.descricao), Selected = false });
            }

            var listret = new SelectList(items, "Value", "Text");



            ViewData["CodigoServico"] = listret;
        }
        private void CarregaBanco()
        {

            var con1 = new Banco().ObterTodos(_paramBase);

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }

            var listret = new SelectList(items, "Value", "Text");



            ViewData["banco"] = listret;
        }
        private void CarregaOperacao()
        {

            var con1 = new Operacao().ObterTodos(_paramBase);

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}", item.descricao), Selected = false });
            }

            var listret = new SelectList(items, "Value", "Text");



            ViewData["operacao"] = listret;
        }
    }
}
