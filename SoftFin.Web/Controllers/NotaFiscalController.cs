using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using System.Web.Helpers;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.Web.Mvc.Html;
using System.Globalization;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Reflection;
using System.IO;
using System.Text;

using SoftFin.Utils;
using SoftFin.NFSe.DTO;
using SoftFin.Web.Regras;

namespace SoftFin.Web.Controllers
{




    public class NotaFiscalController : BaseController
    {
        DbControle _banco = new DbControle();
        int _contaLinha = 0;
        decimal _ValorServicos = 0;
        decimal _ValorDeducoes = 0;


        [HttpPost]
        public JsonResult TotalizadorAutorizacao(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.usuarioAutorizador_id == null).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new NotaFiscal().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valorNfse).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }

        //Grid de notas fiscais
        public ActionResult Index()
        {
            var hoje = DateTime.Now;
            ViewData["DataInicial"] = new DateTime(hoje.Year, hoje.Month, 1).ToShortDateString();
            ViewData["DataFinal"] = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();


            var con1 = new CodigoServicoEstabelecimento().CarregaCombo(_paramBase);
            ViewData["CodigoServico"] = con1;

            ViewData["situacaoPrefeitura"] = Models.NotaFiscal.ListaDropDrow();

            ViewData["unidadeNegocio"] = new SelectList(new Models.UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");

            var obj = new NotaFiscalFiltro();


            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            NotaFiscal obj = new NotaFiscal();


            var objs = obj.ObterTodos(_paramBase);

            string ValorrazaoTomador = Request.QueryString["razaoTomador"];
            string Valorcnpj = Request.QueryString["cnpj"];
            string Valorcpf = Request.QueryString["cpf"];
            string ValorsituacaoPrefeitura_id = Request.QueryString["situacaoPrefeitura_id"];
            string ValorcodigoServico_id = Request.QueryString["codigoServico_id"];
            string ValornumeroRpsIni = Request.QueryString["numeroRpsIni"];
            string ValornumeroRpsFim = Request.QueryString["numeroRpsFim"];
            string ValordataEmissaoRpsIni = Request.QueryString["dataEmissaoRpsIni"];
            string ValordataEmissaoRpsFim = Request.QueryString["dataEmissaoRpsFim"];
            string ValornumeroNfseIni = Request.QueryString["numeroNfseIni"];
            string ValornumeroNfseFim = Request.QueryString["numeroNfseFim"];
            string ValordataEmissaoNfseIni = Request.QueryString["dataEmissaoNfseIni"];
            string ValordataEmissaoNfseFim = Request.QueryString["dataEmissaoNfseFim"];
            string ValordataVencimentoNfseIni = Request.QueryString["dataVencimentoNfseIni"];
            string ValordataVencimentoNfseFim = Request.QueryString["dataVencimentoNfseFim"];
            string ValorcodigoServico = Request.QueryString["codigoServico"];
            string unidadeNegocio_id = Request.QueryString["unidadeNegocio_id"];
            if (!String.IsNullOrEmpty(ValorrazaoTomador))
            {
                objs = objs.Where(p => p.NotaFiscalPessoaTomador.razao.Contains(ValorrazaoTomador)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorcnpj))
            {
                objs = objs.Where(p => p.NotaFiscalPessoaTomador.cnpjCpf.Contains(Valorcnpj)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorcpf))
            {
                objs = objs.Where(p => p.NotaFiscalPessoaTomador.cnpjCpf.Contains(Valorcpf)).ToList();
            }

            int ValornumeroNfseIniAux;
            if (int.TryParse(ValornumeroNfseIni, out ValornumeroNfseIniAux))
                objs = objs.Where(p => p.numeroNfse.Value >= ValornumeroNfseIniAux).ToList();

            int ValornumeroNfseFimAux;
            if (int.TryParse(ValornumeroNfseFim, out ValornumeroNfseFimAux))
                objs = objs.Where(p => p.numeroNfse.Value >= ValornumeroNfseFimAux).ToList();

            DateTime ValordataEmissaoNfseIniAux;
            if (DateTime.TryParse(ValordataEmissaoNfseIni, out ValordataEmissaoNfseIniAux))
                objs = objs.Where(p => p.dataEmissaoNfse >= ValordataEmissaoNfseIniAux).ToList();

            DateTime ValordataEmissaoNfseFimAux;
            if (DateTime.TryParse(ValordataEmissaoNfseFim, out ValordataEmissaoNfseFimAux))
                objs = objs.Where(p => p.dataEmissaoNfse <= ValordataEmissaoNfseFimAux).ToList();

            DateTime ValordataVencimentoNfseIniAux;
            if (DateTime.TryParse(ValordataVencimentoNfseIni, out ValordataVencimentoNfseIniAux))
                objs = objs.Where(p => p.dataVencimentoNfse >= ValordataVencimentoNfseIniAux).ToList();

            DateTime ValordataVencimentoNfseFimAux;
            if (DateTime.TryParse(ValordataVencimentoNfseFim, out ValordataVencimentoNfseFimAux))
                objs = objs.Where(p => p.dataVencimentoNfse <= ValordataVencimentoNfseFimAux).ToList();

            DateTime ValordataEmissaoRpsIniAux;
            if (DateTime.TryParse(ValordataEmissaoRpsIni, out ValordataEmissaoRpsIniAux))
                objs = objs.Where(p => p.dataEmissaoRps >= ValordataEmissaoRpsIniAux).ToList();

            DateTime ValordataEmissaoRpsFimAux;
            if (DateTime.TryParse(ValordataEmissaoRpsFim, out ValordataEmissaoRpsFimAux))
                objs = objs.Where(p => p.dataEmissaoRps <= ValordataEmissaoRpsFimAux).ToList();

            int ValorsituacaoPrefeitura_idAux;
            if (int.TryParse(ValorsituacaoPrefeitura_id, out ValorsituacaoPrefeitura_idAux))
                objs = objs.Where(p => p.situacaoPrefeitura_id == ValorsituacaoPrefeitura_idAux).ToList();

            //int ValorcodigoServico_idAux;
            //if (int.TryParse(ValorcodigoServico_id, out ValorcodigoServico_idAux))
            //    objs = objs.Where(p => p.codigoServico == ValorcodigoServico_idAux).ToList();

            int unidadeNegocio_idAux;
            if (int.TryParse(unidadeNegocio_id, out unidadeNegocio_idAux))
                objs = objs.Where(p => p.OrdemVenda.unidadeNegocio_ID == unidadeNegocio_idAux).ToList();

            int totalRecords = objs.Count();

            //Fix for grouping, because it adds column name instead of index to SortingName
            //string sortingName = "cdg_perfil";

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            objs = objs.OrderBy(p => p.numeroNfse).Skip(12 * request.PageIndex).Take(12).ToList();
            var estab = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);


            //Table with rows data
            foreach (var item in
                objs)
            {

                var nfps = "";

                if (item.numeroNfse != null)
                {
                    string url = "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?inscricao={inscricao}&nf={numeronf}&verificacao={codigoverificacao}";

                    url = url.Replace("{codigoverificacao}", item.codigoVerificacao);
                    url = url.Replace("{numeronf}", item.numeroNfse.ToString());
                    url = url.Replace("{inscricao}", estab.InscricaoMunicipal.ToString().Replace("-", "").Replace(".", "").Replace("/", ""));

                    nfps = "<a href='{url}' target='_blank' >{valor}</a>";
                    nfps = nfps.Replace("{url}", url);
                    nfps = nfps.Replace("{valor}", item.numeroNfse.ToString());
                }

                var situacao = "";
                var situacao_id = item.situacaoPrefeitura_id;
                situacao = Models.NotaFiscal.CarregaSituacao(situacao_id);

                var ov = "";
                var Numero = 0;
                if (item.OrdemVenda != null)
                {
                    ov = item.OrdemVenda.UnidadeNegocio.unidade;
                    Numero = item.OrdemVenda.Numero;
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.tipoRps,
                    item.serieRps,
                    item.numeroRps,
                    item.dataEmissaoRps.ToShortDateString(),
                    nfps,
                    item.dataEmissaoNfse.ToShortDateString(),
                    item.dataVencimentoNfse.ToShortDateString(),
                    item.NotaFiscalPessoaTomador.razao,
                    item.valorNfse,
                    item.codigoServico,
                    situacao,
                    ov,
                    Numero
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }




        //Inclusão de NFS-e
        [HttpPost]
        public ActionResult Create(NotaFiscal obj, string dataEmissaoRps, string dataVencimentoNfse)
        {
            try
            {
                int estab = _paramBase.estab_id;
                
                DbControle banco = new DbControle();
                var ov = _banco.OrdemVenda.Where(p => p.id == obj.ordemVenda_id && p.estabelecimento_id == estab).FirstOrDefault();


                if (ov.ParcelaContrato != null)
                {
                    @ViewData["ParcelaID"] = ov.ParcelaContrato.parcela;
                    @ViewData["ContratoID"] = ov.ParcelaContrato.ContratoItem.Contrato.contrato;
                }
                
                CarregaCodigoServico(banco);

                if (ModelState.IsValid)
                {
                    obj.valorNfse = obj.valorNfse / 100;
                    var emissaoRps = Convert.ToDateTime(dataEmissaoRps);
                    var vencimentoNfse = Convert.ToDateTime(dataVencimentoNfse);
                    //obj.discriminacaoServico = discriminacao;
                    obj.dataEmissaoNfse = emissaoRps;
                    obj.dataVencimentoNfse = vencimentoNfse;


                    //var con1 = new CodigoServico().ObterTodos();
                    //ViewData["CodigoServico"] = new SelectList(con1, "id", "codigo");


                    //if (ModelState.IsValid)
                    //{
                    NotaFiscal nota = new NotaFiscal();
                    obj.situacaoPrefeitura_id = Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA;
                    obj.municipio_id = _estabobj.Municipio_id;

                    NotaFiscal rps = new NotaFiscal();
                    rps = banco.NotaFiscal.Where(p => p.estabelecimento_id == _estab).ToList().OrderByDescending(r => r.numeroRps).FirstOrDefault();

                    //Dados do RPS

                    if (rps == null)
                        obj.numeroRps = 1;
                    else
                        obj.numeroRps = (rps.numeroRps + 1);
                    obj.situacaoRps = "1";

                    //var ov = _banco.OrdemVenda.Where(p => p.id == obj.ordemVenda_id && p.estabelecimento_id == estab).FirstOrDefault();

                    if (nota.Incluir(obj, _paramBase) == true)
                    {

                        if (ov.parcelaContrato_ID != null)
                        {
                            ParcelaContrato parcela = new ParcelaContrato();
                            parcela = _banco.ParcelaContrato.ToList().Where(p => p.id == ov.parcelaContrato_ID).FirstOrDefault();
                            parcela.statusParcela_ID = StatusParcela.SituacaoEmitida();

                        }
                        ov.statusParcela_ID = StatusParcela.SituacaoEmitida();
                        _banco.SaveChanges();


                        ViewBag.msg = "Nota Fiscal incluída com sucesso";
                        return RedirectToAction("/Index", "Faturamento");
                        
                    }
                    else
                    {

                        //Seleciona Código de Serviço
                        obj.Estabelecimento = new Estabelecimento().ObterPorId(estab, _paramBase);
                        ViewBag.msg = "Nota Fiscal já cadastrado";
                        return View(obj);
                    }
                }
                else
                {

                //    //Seleciona Código de Serviço
                    obj.Estabelecimento = new Estabelecimento().ObterPorId(estab, _paramBase);
                    ModelState.AddModelError("", "Dados Invalidos");
                    ViewBag.msg = "Erro";
                    return View(obj);

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        public ActionResult ConsultaPFSP(int id)
        {
            DbControle banco = new DbControle();
            NotaFiscal notaFiscal = banco.NotaFiscal.ToList().Where(c => c.id == id).FirstOrDefault();

            if (notaFiscal.numeroNfse != null)
            {
                string url = "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?inscricao={inscricao}&nf={numeronf}&verificacao={codigoverificacao}";

                var estab = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);


                url = url.Replace("{codigoverificacao}", notaFiscal.codigoVerificacao);
                url = url.Replace("{numeronf}", notaFiscal.numeroNfse.ToString());
                url = url.Replace("{inscricao}", estab.InscricaoMunicipal.ToString().Replace("-", "").Replace(".", "").Replace("/", ""));

                Response.Redirect(url, true);
            }

            return View();
        }

        public ActionResult Detail(int id)
        {
            
            try
            {
                return DetailPadrao(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Erro";
                return View(new NotaFiscal());
            }
        }






        private ActionResult DetailPadrao(int id)
        {
            DbControle banco = new DbControle();
            ContratoItem contrato = banco.ContratoItem.ToList().Where(c => c.id == id).FirstOrDefault();
            CarregaCodigoServico(banco);

            var nota = new NotaFiscal().ObterPorId(id);

            @ViewData["textoNota"] = nota.discriminacaoServico;
            @ViewData["dataVencimento"] = nota.dataVencimentoNfse.ToString("dd/MM/yyyy");
            @ViewData["dataEmissaoRPS"] = nota.dataEmissaoRps.ToString("dd/MM/yyyy");


            if (nota.OrdemVenda != null)
            {
                ViewData["OV"] = nota.OrdemVenda.Numero.ToString();
                if (nota.OrdemVenda.ParcelaContrato != null)
                {
                    @ViewData["ParcelaID"] = nota.OrdemVenda.ParcelaContrato.parcela;
                    @ViewData["ContratoID"] = nota.OrdemVenda.ParcelaContrato.ContratoItem.Contrato.contrato;
                }
            }
            else
            {
                @ViewData["ParcelaID"] = "";
                @ViewData["ContratoID"] = "";
                ViewData["OV"] = new OrdemVenda();
            }
            nota.discriminacaoServico = nota.discriminacaoServico;
            return View(nota);
        }




        public ActionResult Cancelamento(int id)
        {
            try
            {
                DbControle banco = new DbControle();
                ContratoItem contrato = banco.ContratoItem.ToList().Where(c => c.id == id).FirstOrDefault();
                CarregaCodigoServico(banco);

                var nota = new NotaFiscal().ObterPorId(id);

                @ViewData["textoNota"] = nota.discriminacaoServico;
                @ViewData["dataVencimento"] = nota.dataVencimentoNfse.ToString("dd/MM/yyyy");
                @ViewData["dataEmissaoRPS"] = nota.dataEmissaoRps.ToString("dd/MM/yyyy");


                if (nota.OrdemVenda != null)
                {
                    ViewData["OV"] = nota.OrdemVenda.Numero.ToString();
                    if (nota.OrdemVenda.ParcelaContrato != null)
                    {
                        @ViewData["ParcelaID"] = nota.OrdemVenda.ParcelaContrato.parcela;
                        @ViewData["ContratoID"] = nota.OrdemVenda.ParcelaContrato.ContratoItem.Contrato.contrato;
                    }
                }
                else
                {
                    @ViewData["ParcelaID"] = "";
                    @ViewData["ContratoID"] = "";
                    ViewData["OV"] = new OrdemVenda();
                }
                nota.discriminacaoServico = nota.discriminacaoServico;
                return View(nota);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Erro";
                return View(new NotaFiscal());
            }
        }

        [HttpPost]
        public ActionResult Cancelamento(NotaFiscal obj, string dataEmissaoRps, string dataVencimentoNfse)
        {
            try
            {
                if (obj.estabelecimento_id != _estab)
                {
                    ModelState.AddModelError("", "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)");
                    ViewBag.msg = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)";
                    return View(Cancelamento(obj.id));
                }

                var db = new DbControle();
                var nfe = db.NotaFiscal.Where(nf => nf.id == obj.id && nf.estabelecimento_id == _estab).FirstOrDefault();

                if (nfe.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA)
                {
                    db.NotaFiscal.Remove(nfe);

                    var ov = db.OrdemVenda.Where(p => p.id == obj.ordemVenda_id && p.estabelecimento_id == _estab).FirstOrDefault();

                    if (ov.ParcelaContrato != null)
                    {
                        var contratoParcela = db.ParcelaContrato.Where(pf => pf.id == ov.parcelaContrato_ID
                            && pf.ContratoItem.Contrato.estabelecimento_id == _estab).FirstOrDefault();
                        contratoParcela.statusParcela_ID = Models.StatusParcela.SituacaoLiberada();
                    }
                    ov.statusParcela_ID = Models.StatusParcela.SituacaoLiberada();
                }
                else if (nfe.situacaoPrefeitura_id == Models.NotaFiscal.NFGERADAENVIADA)
                {
                    nfe.situacaoPrefeitura_id = Models.NotaFiscal.NFCANCELADAEMCONF;
                }

                //Exclui Banco Movimento quando cancelada a nota fiscal

                var bcoMovs = db.BancoMovimento.Where(nf => nf.notafiscal_id == obj.id);
                foreach (var item in bcoMovs)
                {
                    db.BancoMovimento.Remove(item);
                }
                db.SaveChanges();

                
                return RedirectToAction("/Index", "NotaFiscal");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Não foi possivel cancelar a nota";
                return View(Cancelamento(obj.id));
            }
        }

        public ActionResult BaixaPerda(int id)
        {
            try
            {
                DbControle banco = new DbControle();
                ContratoItem contrato = banco.ContratoItem.ToList().Where(c => c.id == id).FirstOrDefault();
                CarregaCodigoServico(banco);

                var nota = new NotaFiscal().ObterPorId(id);

                @ViewData["textoNota"] = nota.discriminacaoServico;
                @ViewData["dataVencimento"] = nota.dataVencimentoNfse.ToString("dd/MM/yyyy");
                @ViewData["dataEmissaoRPS"] = nota.dataEmissaoRps.ToString("dd/MM/yyyy");


                if (nota.OrdemVenda != null)
                {
                    ViewData["OV"] = nota.OrdemVenda.Numero.ToString();
                    if (nota.OrdemVenda.ParcelaContrato != null)
                    {
                        @ViewData["ParcelaID"] = nota.OrdemVenda.ParcelaContrato.parcela;
                        @ViewData["ContratoID"] = nota.OrdemVenda.ParcelaContrato.ContratoItem.Contrato.contrato;
                    }
                }
                else
                {
                    @ViewData["ParcelaID"] = "";
                    @ViewData["ContratoID"] = "";
                    ViewData["OV"] = new OrdemVenda();
                }
                nota.discriminacaoServico = nota.discriminacaoServico;
                return View(nota);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Erro";
                return View(new NotaFiscal());
            }
        }

        [HttpPost]
        public ActionResult BaixaPerda(NotaFiscal obj, string dataEmissaoRps, string dataVencimentoNfse)
        {
            try
            {
                var db = new DbControle();
                var nfe = db.NotaFiscal.Where(nf => nf.id == obj.id && nf.estabelecimento_id == _estab).FirstOrDefault();

                nfe.situacaoPrefeitura_id = Models.NotaFiscal.NFBAIXA;
                nfe.SituacaoRecebimento = 4;

                //Exclui Banco Movimento quando baixada a nota fiscal
                var bcoMov = db.BancoMovimento.Where(nf => nf.notafiscal_id == obj.id && nf.Banco.estabelecimento_id == _estab).FirstOrDefault();
                if (bcoMov != null)
                {
                    db.BancoMovimento.Remove(bcoMov);
                }
                db.SaveChanges();

                return RedirectToAction("/Index", "NotaFiscal");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Não foi possivel baixar a nota";
                return View(Cancelamento(obj.id));
            }
        }

        public ActionResult Create2()
        {
            var notaFiscal = new NotaFiscalOrdemVenda();
            notaFiscal.dataEmissaoNfse = DateTime.Now;
            notaFiscal.dataEmissaoRps = DateTime.Now;
            notaFiscal.dataVencimentoNfse = DateTime.Now;
            notaFiscal.data = DateTime.Now;
            notaFiscal.tipoRps = 2;
            CarregaViewDataOV2();
            return View(notaFiscal);
        }

        private void CarregaViewDataOV2()
        {
            ViewData["parcelaContrato"] = new SelectList(new ParcelaContrato().ObterTodos(_paramBase, 0), "id", "descricao");
            ViewData["statusParcela"] = new SelectList(new StatusParcela().ObterTodos().Where(p => p.status == "Emitida").ToList(), "id", "status"); 
            ViewData["unidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
            ViewData["pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = Models.NotaFiscal.NFGERADAENVIADA.ToString(), Text = Models.NotaFiscal.NFSEGERADA_TEXTO });
            var listret = new SelectList(items, "Value", "Text");

            ViewData["situacaoPrefeitura"] = listret;
            ViewData["codigoServico"] = new CodigoServicoEstabelecimento().CarregaCombo(_paramBase);
            ViewData["banco"] = new Banco().CarregaBancoGeral(_paramBase);
            ViewData["operacao"] = new SelectList(new Operacao().ObterTodos(_paramBase), "id", "descricao");
            ViewData["item"] = new SelectList(new ItemProdutoServico().ObterTodos(_paramBase), "id", "descricao");
            ViewData["tabelaPreco"] = new SelectList(new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase), "id", "descricao");
        }
        
        private void CarregaViewDataOV(bool ispendente)
        {
            var status = new StatusParcela();
            var lista = new List<StatusParcela>();
            if (ispendente) 
                lista = status.ObterTodos().Where(p => p.status == "Pendente").ToList();
            else
                lista = status.ObterTodos().ToList();

            ViewData["parcelaContrato"] = new SelectList(new ParcelaContrato().ObterTodos(_paramBase, 0), "id", "descricao");
            ViewData["statusParcela"] = new SelectList(lista, "id", "status");
            ViewData["unidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
            ViewData["pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(NotaFiscalOrdemVenda obj)
        {
            try
            {
                var ov = new OrdemVenda();
                var nf = new  NotaFiscal();

                obj.tipoRps = 1;
                CarregaViewDataOV2();

                obj.estabelecimento_id = _estab;
                
                var plano = new PlanoDeConta().ObterTodos().Where(p => p.codigo.Equals("01")).FirstOrDefault();

                if (plano == null)
                {
                    ViewBag.msg = "Plano de contas 01 não configurado";
                    return View(obj);
                }

                

                var tipoManual = new OrigemMovimento().TipoManual(_paramBase);
                var tipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
                var tipoNotaPromissoria = new TipoDocumento().TipoNotaPromissoria();

                if (ModelState.IsValid)
                {
                    int numov = 0;
                    ov.data = obj.data;
                    ov.descricao = obj.descricao;
                    ov.estabelecimento_id = _estab;
                    ov.Numero = obj.Numero;
                    ov.unidadeNegocio_ID = obj.unidadeNegocio_ID;
                    ov.statusParcela_ID = obj.statusParcela_ID;
                    ov.valor=  obj.valor;
                    ov.pessoas_ID  = obj.pessoas_ID;
                    ov.itemProdutoServico_ID = obj.itemProdutoServico_ID;
                    ov.tabelaPreco_ID = obj.tabelaPreco_ID;
                    ov.usuarioinclusaoid = _usuarioobj.id;
                    ov.dataInclusao = DateTime.Now;

                    DbControle db = new DbControle();

                    // Inicio Lançamento Contabil
                    var idCredito = 0;
                    var idDebito = 0;
                    var ccLC = new LancamentoContabil();
                    var ccDebito = new LancamentoContabilDetalhe();
                    var ccCredito = new LancamentoContabilDetalhe();

                    var ecf = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);
                    if (ecf != null)
                    {
                        idCredito = ecf.ContaContabilNFMercadoria_id;
                        idDebito = ecf.ContaContabilRecebimento_id;
                    }

                    if (idCredito != 0 && idDebito != 0)
                    {
                        ccLC.data = nf.dataVencimentoNfse;
                        ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                        ccLC.estabelecimento_id = _paramBase.estab_id;
                        ccLC.historico = ov.descricao;
                        ccLC.usuarioinclusaoid = _paramBase.usuario_id;
                        ccLC.origemmovimento_id = new OrigemMovimento().TipoFaturamento(_paramBase);
                        ccLC.UnidadeNegocio_ID = ov.unidadeNegocio_ID;

                        ccDebito.contaContabil_id = idDebito;
                        ccDebito.DebitoCredito = "D";
                        ccDebito.valor = ov.valor;

                        ccDebito.contaContabil_id = idCredito;
                        ccDebito.DebitoCredito = "C";
                        ccDebito.valor = ov.valor;
                    }
                    //Fim Lançamento Contabil

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {                        
                        if (!ov.Incluir(ov, ref numov, _paramBase, db))
                        {
                            dbcxtransaction.Rollback();
                            ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                            return View(obj);
                        }
                        
                        ConverteObj(obj, ov, nf);


                        if (nf.Incluir(_paramBase, db))
                        {
                            //Inicio Lançamento Contabil
                            if (idCredito != 0 && idDebito != 0)
                            {
                                var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                                ccLC.codigoLancamento = numeroLcto;
                                ccLC.notafiscal_id = nf.id;
                                ccLC.Incluir(_paramBase, db);
                                ccDebito.lancamentoContabil_id = ccLC.id;
                                ccCredito.lancamentoContabil_id = ccLC.id;
                                ccDebito.Incluir(_paramBase, db);
                                ccCredito.Incluir(_paramBase, db);
                            }
                            //Fim Lançamento Contabil

                            ViewBag.msg = "Incluído com sucesso";
                            //return View(obj);
                        }
                        else
                        {
                            ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                            dbcxtransaction.Rollback();
                            return View(obj);
                        }

                        var bm = new BancoMovimento();

                        bm.banco_id = obj.banco_id;
                        bm.data = obj.dataVencimentoNfse;
                        //bm.empresa_id  = pb.empresa_id;
                        bm.historico = obj.descricao;
                        bm.notafiscal_id = nf.id;
                        bm.valor = nf.valorLiquido;
                        bm.origemmovimento_id = tipoManual;
                        bm.tipoDeMovimento_id = tipoEntrada;
                        bm.tipoDeDocumento_id = tipoNotaPromissoria;
                        //bm.unidadeDeNegocio_id = obj.unidadeNegocio_ID;
                        bm.planoDeConta_id = plano.id ;
                        bm.usuarioinclusaoid = _usuarioobj.id;
                        bm.dataInclusao = DateTime.Now;
                        if (bm.Incluir(bm, _paramBase, db))
                        {
                            ViewBag.msg = "Incluído com sucesso";

                        }
                        else
                        {
                            ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                            dbcxtransaction.Rollback();
                            return View(obj);
                        }
                        dbcxtransaction.Commit();

                        return View(obj);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private void ConverteObj(NotaFiscalOrdemVenda obj, OrdemVenda ov, NotaFiscal nf)
        {
            nf.municipio_id = _estabobj.Municipio_id;
            nf.ordemVenda_id = ov.id;
            nf.estabelecimento_id = _estab;
            nf.aliquotaIrrf = obj.aliquotaIrrf;
            nf.aliquotaISS = obj.aliquotaISS;
            nf.NotaFiscalPessoaTomador.bairro = obj.bairroTomador;
            nf.banco = obj.banco; ;
            nf.banco_id = obj.banco_id;
            nf.basedeCalculo = obj.basedeCalculo;
            nf.NotaFiscalPessoaTomador.cep = obj.cepTomador;
            nf.NotaFiscalPessoaTomador.cidade = obj.cidadeTomador;
            nf.NotaFiscalPessoaTomador.cnpjCpf = obj.cnpjCpf;
            nf.codigoServico = obj.codigoServico;

            nf.codigoVerificacao = obj.codigoVerificacao;
            nf.cofinsRetida = obj.cofinsRetida;
            nf.NotaFiscalPessoaTomador.complemento = obj.complementoTomador;
            nf.creditoImposto = obj.creditoImposto;
            nf.csllRetida = obj.csllRetida;
            nf.dataEmissaoNfse = obj.dataEmissaoNfse;
            nf.dataEmissaoRps = obj.dataEmissaoRps;
            nf.dataVencimentoNfse = obj.dataVencimentoNfse;
            nf.discriminacaoServico = obj.discriminacaoServico;
            nf.NotaFiscalPessoaTomador.email = obj.emailTomador;
            nf.NotaFiscalPessoaTomador.endereco = obj.enderecoTomador;
            nf.NotaFiscalPessoaTomador.indicadorCnpjCpf = obj.indicadorCnpjCpf;
            nf.NotaFiscalPessoaTomador.inscricaoEstadual = obj.inscricaoEstadual;
            nf.NotaFiscalPessoaTomador.inscricaoMunicipal = obj.inscricaoMunicipal;
            nf.irrf = obj.irrf;
            nf.numeroNfse = obj.numeroNfse;
            nf.numeroRps = obj.numeroRps;
            nf.NotaFiscalPessoaTomador.numero = obj.numeroTomador;
            nf.Operacao = obj.Operacao;
            nf.operacao_id = obj.operacao_id;
            nf.pisRetido = obj.pisRetido;
            nf.NotaFiscalPessoaTomador.razao = obj.razaoTomador;
            nf.Recebimentos = obj.Recebimentos.ToList();
            nf.serieRps = obj.serieRps;
            nf.situacaoPrefeitura_id = obj.situacaoPrefeitura_id;
            nf.situacaoRps = obj.situacaoRps;
            nf.NotaFiscalPessoaTomador.tipoEndereco = obj.tipoEndereco;
            nf.NotaFiscalPessoaTomador.uf = obj.ufTomador;
            nf.valorDeducoes = obj.valorDeducoes;
            nf.valorISS = obj.valorISS;
            nf.valorLiquido = obj.valorLiquido;
            nf.valorNfse = obj.valor;
            nf.dataEmissaoNfse = obj.data;
            nf.valorINSS = obj.valorINSS;
            nf.aliquotaINSS = obj.aliquotaINSS;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NotaFiscal obj)
        {
            CarregaViewDataOV2();
            try
            {
                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {

                    var nf = new NotaFiscal().ObterPorId(obj.id,db);
                    nf.dataVencimentoNfse = obj.dataVencimentoNfse;

                    //Atualiza Banco Movimento
                    var banco = db.BancoMovimento.Where(x => x.notafiscal_id == obj.id).FirstOrDefault();
                    if (banco != null)
                    {
                        banco.data = obj.dataVencimentoNfse;
                    }

                    int estab = _paramBase.estab_id;
                    nf.estabelecimento_id = estab;
                    if (nf.Alterar(_paramBase, db))
                    {
                        dbcxtransaction.Commit();
                        ViewBag.msg = "Alterado com sucesso";
                        return Edit(obj.id);

                    }
                    else
                    {
                        dbcxtransaction.Rollback();
                        ViewBag.msg = "Impossivel alterar, registro excluído";
                        return View(obj);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Edit(int id)
        {
            
            try
            {
                DbControle banco = new DbControle();
                ContratoItem contrato = banco.ContratoItem.ToList().Where(c => c.id == id).FirstOrDefault();
                CarregaCodigoServico(banco);

                var nota = new NotaFiscal().ObterPorId(id);

                @ViewData["textoNota"] = nota.discriminacaoServico;
                @ViewData["dataVencimento"] = nota.dataVencimentoNfse.ToString("dd/MM/yyyy");
                @ViewData["dataEmissaoRPS"] = nota.dataEmissaoRps.ToString("dd/MM/yyyy");


                if (nota.OrdemVenda != null)
                {
                    ViewData["OV"] = nota.OrdemVenda.Numero.ToString();
                    if (nota.OrdemVenda.ParcelaContrato != null)
                    {
                        @ViewData["ParcelaID"] = nota.OrdemVenda.ParcelaContrato.parcela;
                        @ViewData["ContratoID"] = nota.OrdemVenda.ParcelaContrato.ContratoItem.Contrato.contrato;
                    }
                }
                else
                {
                    @ViewData["ParcelaID"] = "";
                    @ViewData["ContratoID"] = "";
                    ViewData["OV"] = new OrdemVenda();
                }
                nota.discriminacaoServico = nota.discriminacaoServico;
                return View(nota);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Erro";
                return View(new NotaFiscal());
            }
        }
        public ActionResult Create(int OrdemVenda, string CodigoServicoID, DateTime data, int idBanco, int idOperacao, Decimal valorNfse)
        {
            string erros = "";
            try
            {

                int estab = _paramBase.estab_id;

                DbControle banco = new DbControle();
                var ov = _banco.OrdemVenda.Where(p => p.id == OrdemVenda && p.estabelecimento_id == estab).FirstOrDefault();

                CarregaCodigoServico(banco, CodigoServicoID);

                var nota = CriaNota(OrdemVenda, banco);
                nota = NotaFiscalCalculos.CalculaNota(nota, ov, banco, CodigoServicoID, idOperacao, data, _estabobj, _paramBase);
                nota.banco_id = idBanco;
                nota.valorNfse = valorNfse;
                nota.operacao_id = idOperacao;
                nota.dataEmissaoNfse = data;
                nota.codigoServico = CodigoServicoID;
                nota.ordemVenda_id = OrdemVenda;
                nota.dataVencimentoNfse = data;
                nota.dataEmissaoNfse = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                var textoNota = new NotaFiscalCalculos().MontaTextoNota(nota, banco, ov, ref erros);
                if (ov.ParcelaContrato != null)
                {
                    @ViewData["ParcelaID"] = ov.ParcelaContrato.parcela;
                    @ViewData["ContratoID"] = ov.ParcelaContrato.ContratoItem.Contrato.contrato;
                }

                ViewData["OV"] = ov.Numero.ToString();
                nota.discriminacaoServico = textoNota;
                return View(nota);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Erro";
                return View(new NotaFiscal());
            }
        }

        [HttpPost]
        public JsonResult CalculaNotaTela(
            string strCodigoServico, 
            string strdata, 
            string stridBanco, 
            string stridOperacao, 
            string strvalor, 
            string strunidadeNegocio, 
            string strpessoa)
        {
            
            try
            {
                
                DateTime data = DateTime.Parse(strdata);
                int idBanco = int.Parse(stridBanco);
                int idOperacao = 0;
                if (stridOperacao != "")
                    idOperacao = int.Parse(stridOperacao);
                decimal valor = decimal.Parse(strvalor.Replace(".",","))/100;
                int unidadeNegocio = int.Parse(strunidadeNegocio);
                int pessoa = int.Parse(strpessoa);

                var nf = new NotaFiscalCalculos().Calcula(strCodigoServico,
                                                           data,
                                                           idBanco,
                                                           idOperacao,
                                                           valor,
                                                           unidadeNegocio,
                                                           pessoa,
                                                           0,
                                                           _estabobj,
                                                           new NotaFiscal(),
                                                           _paramBase
                                                           );

                return Json(nf, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
                ViewBag.msg = "Erro";
                return null;
            }
        }

        public NotaFiscal CriaNota(int ID, DbControle banco)
        {
            var objNF = new NotaFiscal();
            OrdemVenda ov = new OrdemVenda().ObterPorId(ID);

            objNF.OrdemVenda = ov;
            objNF.ordemVenda_id = ov.id;
            objNF.dataVencimentoNfse = ov.data.Date;
            objNF.valorNfse = ov.valor;

            var objy = new Banco().ObterTodos(_paramBase).Where(p => p.principal == true).FirstOrDefault();
            if (objy != null)
                objNF.banco_id = objy.id;

            return objNF;
        }



        private void CarregaCodigoServico(DbControle banco,String codigoservico =null)
        {
            var con1 = new CodigoServicoEstabelecimento().ObterTodos(_paramBase);

            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                var slect = false;
                if (codigoservico != null)
                    slect = (item.CodigoServicoMunicipio.codigo == codigoservico);

                items.Add(new SelectListItem() { Value = item.CodigoServicoMunicipio.codigo, Text = String.Format("{0} - {1}", item.CodigoServicoMunicipio.codigo, item.CodigoServicoMunicipio.descricao), Selected = slect });
            }

            var listret = new SelectList(items, "Value", "Text");



            ViewData["CodigoServico"] = listret;
        }


        public ActionResult GeracaoArquivo()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");

            return View();
        }


        [HttpPost]
        public FileStreamResult GeracaoArquivo(string dataInicial, string dataFinal)
        {
            StringBuilder sb = new StringBuilder();

            List<int> nfs = new List<int>();

            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 2) == "nf")
                {
                    nfs.Add(int.Parse(item.ToString().Substring(2)));
                }

            }

            //Carrega apelido do estabelecimento para incluir no arquivo de RPS
            int idestab = _paramBase.estab_id;
            Estabelecimento emp = new Estabelecimento();
            var apelidoEstabelecimento = emp.ObterPorId(idestab, _paramBase).Apelido;

            Cabecario(sb, dataInicial, dataFinal);
            Detalhe(sb, nfs);
            rodape(sb);

            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "RPS_" + apelidoEstabelecimento + "_" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".txt");
        }



        private void Cabecario(StringBuilder sb, string DataInicia, string DataFinal)
        {
            sb.Append("1001");
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
            //var prestador = new Pessoa().ObterTodos().Where(p => p.TipoPessoa.Descricao == "Prestador").First();
            sb.Append(prestador.InscricaoMunicipal.ToString("D8"));
            sb.Append(DateTime.Parse(DataInicia).ToString("yyyyMMdd"));
            sb.AppendLine(DateTime.Parse(DataFinal).ToString("yyyyMMdd"));
        }


        private void Detalhe(StringBuilder sb, List<int> nfsid)
        {
            int estab = _paramBase.estab_id;

            var db = new DbControle();



            List<NotaFiscal> listaAux = new List<NotaFiscal>();
            foreach (var item in nfsid)
            {
                var nfe = db.NotaFiscal.Where(nf => nf.id == item && nf.estabelecimento_id == estab).FirstOrDefault();
                if (nfe != null)
                    listaAux.Add(nfe);
            }


            foreach (var item in listaAux)
            {
                _ValorDeducoes += item.valorDeducoes;
                _ValorServicos += item.valorNfse;
                _contaLinha += 1;
                item.valorDeducoes = 0;

                sb.Append("2"); //1) Tipo de registro
                sb.Append("RPS  "); //2) Tipo do RPS
                sb.AppendFormat("{0,-5}", item.serieRps); //3) Série do RPS
                sb.AppendFormat("{0,-12}", item.numeroRps.ToString().PadLeft(12, '0'));//4) Número do RPS
                sb.AppendFormat("{0,-8:yyyyMMdd}", item.dataEmissaoRps);//5) Data de Emissão do RPS

                //Situação da Nota Fiscal com 01 posição:
                //T - Tributado em São Paulo
                //I - Operação isenta ou não tributável, executadas no Município de São Paulo.
                //F - Tributado fora de São Paulo
                //C - Cancelada
                //E - Extraviada
                //J - ISS Suspenso por Decisão Judicial
                //A - Tributado em São Paulo, porém Isento
                //B - Tributado fora de São Paulo, porém Isento
                //M - Tributado em São Paulo, porém Imune
                //N - Tributado Fora de São Paulo, porém Imune
                //X - Tributado em São Paulo, porém Exigibilidade Suspensa
                //V - Tributado Fora de São Paulo, porém Exigibilidade Suspensa
                //P - Exportação de Serviços
                //S - NFS-e substituída

                if (item.situacaoPrefeitura_id == Models.NotaFiscal.NFCANCELADAEMCONF)
                    sb.AppendFormat("{0}", "C");//6) Situação do RPS
                else
                    sb.AppendFormat("{0}", item.Operacao.situacaoTributariaNota.codigo);//6) Situação do RPS

                sb.AppendFormat("{0,-15}", item.valorNfse.ToString("0.00").Replace(",", "").PadLeft(15, '0'));//7) Valor dos Serviços
                sb.AppendFormat("{0,-15}", item.valorDeducoes.ToString("0.00").Replace(",", "").PadLeft(15, '0'));//8) Valor das Deduções
                sb.AppendFormat("{0,-5}", item.codigoServico);//9) Código do Serviço Prestado
                sb.AppendFormat("{0,-4}", item.aliquotaISS.ToString("0.00").Replace(",", "").PadLeft(4, '0'));//10) Alíquota

                string IssRetidoStatus = "";
                var calc = new calculoImposto().ObterIssRetido(item.operacao_id.Value, _paramBase).FirstOrDefault();

                if (calc.retido == true)
                {                    
                    IssRetidoStatus = "1";
                }
                else
                {
                    IssRetidoStatus = "2";
                }

                sb.Append(IssRetidoStatus); //11) ISS Retido

                sb.AppendFormat("{0,-1}", item.NotaFiscalPessoaTomador.indicadorCnpjCpf); //12) Indicador de CPF/CNPJ do Tomador
                sb.AppendFormat("{0,-14}", item.NotaFiscalPessoaTomador.cnpjCpf.Replace(".", "").Replace("-", "").Replace("/", ""));//13) CPF ou CNPJ do Tomador
                sb.AppendFormat("{0,-8}", item.NotaFiscalPessoaTomador.inscricaoMunicipal);//14) Inscrição Municipal do Tomador

                if (string.IsNullOrEmpty(item.NotaFiscalPessoaTomador.inscricaoMunicipal))
                {
                    if (item.NotaFiscalPessoaTomador.inscricaoEstadual == null)
                    {
                        throw new Exception("Regere o RPS com inscrição estadual"); 
                    }

                    if (item.NotaFiscalPessoaTomador.inscricaoEstadual.ToUpper().Equals("ISENTO") || item.NotaFiscalPessoaTomador.inscricaoEstadual.ToUpper().Equals("ISENTA"))
                    {
                        sb.AppendFormat("{0,-12}", "");//15) Inscrição Estadual do Tomador
                    }
                    else
                    {
                        sb.AppendFormat("{0,-12}", item.NotaFiscalPessoaTomador.inscricaoEstadual);//15) Inscrição Estadual do Tomador
                    }
                }
                else
                {
                    sb.AppendFormat("{0,-12}", "");//15) Inscrição Estadual do Tomador
                }
                sb.AppendFormat("{0,-75}", item.NotaFiscalPessoaTomador.razao);//16) Nome/ Razão Social do Tomador

                if (item.NotaFiscalPessoaTomador.tipoEndereco.Length < 3)
                    sb.AppendFormat("{0,-3}", item.NotaFiscalPessoaTomador.tipoEndereco);//17) Tipo do Endereço do Tomador (Rua, Av, ...)
                else
                    sb.AppendFormat("{0,-3}", item.NotaFiscalPessoaTomador.tipoEndereco.Substring(0, 3));//17) Tipo do Endereço do Tomador (Rua, Av, ...)

                sb.AppendFormat("{0,-50}", item.NotaFiscalPessoaTomador.endereco);//18) Endereço do Tomador
                sb.AppendFormat("{0,-10}", item.NotaFiscalPessoaTomador.numero);//19) Número do Endereço do Tomador
                sb.AppendFormat("{0,-30}", item.NotaFiscalPessoaTomador.complemento);//20) Complemento do Endereço do Tomador
                sb.AppendFormat("{0,-30}", item.NotaFiscalPessoaTomador.bairro);//21) Bairro do Tomador
                sb.AppendFormat("{0,-50}", item.NotaFiscalPessoaTomador.cidade);//22) Cidade do Tomador
                sb.AppendFormat("{0,-2}", item.NotaFiscalPessoaTomador.uf);//23) UF do Tomador
                sb.AppendFormat("{0,-8}", item.NotaFiscalPessoaTomador.cep).Replace("-", "");//24) CEP do Tomador
                sb.AppendFormat("{0,-75}", item.NotaFiscalPessoaTomador.email);//25) Email do Tomador
                sb.AppendFormat("{0,-1000}", item.discriminacaoServico.Replace("\n", "|").Replace("\r", ""));//26) Discriminação dos Serviços
                sb.AppendLine();
            }
        }


        private void rodape(StringBuilder sb)
        {
            _ValorDeducoes = 0;
            sb.Append("9");
            sb.AppendFormat("{0,-7}", _contaLinha.ToString().PadLeft(7, '0'));//8) Valor das Deduções
            sb.AppendFormat("{0,-15}", _ValorServicos.ToString("0.00").Replace(",", "").PadLeft(15, '0'));//8) Valor dos serviços
            sb.AppendFormat("{0,-15}", _ValorDeducoes.ToString("0.00").Replace(",", "").PadLeft(15, '0'));//8) Valor das Deduções
            sb.AppendLine();
        }



        [HttpPost]
        public JsonResult ConsultaNota(string dataInicial, string dataFinal)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);


            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);



            var nfs = new NotaFiscal().ObterTodos(DataInicial, DataFinal, _paramBase).Where(p => p.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA
                                        || p.situacaoPrefeitura_id == Models.NotaFiscal.NFCANCELADAEMCONF);
            List<RetornoNota> rets = new List<RetornoNota>();

            foreach (var item in nfs)
            {
                rets.Add(new RetornoNota
                {
                    Cliente = item.NotaFiscalPessoaTomador.razao,
                    id = item.id,
                    RPS = item.numeroRps.ToString(),
                    Valor = item.valorNfse.ToString("0.00"),
                    Data = item.dataEmissaoRps.ToString("dd/MM/yyyy"),
                    Situacao = Models.NotaFiscal.CarregaSituacao(item.situacaoPrefeitura_id)
                });
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

            return Json(rets, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Autorizacao()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
            return View();
        }

        [HttpPost]
        public ActionResult Autorizacao(string dataInicial, string dataFinal, int opc)
        {
            @ViewData["DataInicial"] = dataInicial;
            @ViewData["DataFinal"] = dataFinal;
            PerfilPagarAprovacao PF = new PerfilPagarAprovacao();

            PF = new PerfilPagarAprovacao().ObterTodos(_paramBase).Where(p => p.usuarioAutorizador_id == _paramBase.usuario_id).FirstOrDefault();

            if (PF == null)
            {
                ViewBag.msg = "Usuário não tem acesso a autorização.";
                return View();
            }


            List<int> nfs = new List<int>();
            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 2) == "nf")
                {
                    nfs.Add(int.Parse(item.ToString().Substring(2)));
                }

            }

            if (nfs.Count() == 0)
            {
                ViewBag.msg = "Não foram selecionadas notas.";
                return View();
            }

            int idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
            int estab = _paramBase.estab_id;
            string msgextra = "";
            var db = new DbControle();

            foreach (var item in nfs)
            {
                var ov = db.OrdemVenda.Where(x => x.id == item && x.estabelecimento_id == estab).FirstOrDefault();

                if (ov != null)
                {
                    if (ov.valor <= decimal.Parse(PF.valorLimiteNFSE.ToString()))
                    {
                        if (opc == 1)
                        {
                            ov.dataAutorizacao = DateTime.Now;
                            ov.usuarioAutorizador_id = idusuario;
                            ov.statusParcela_ID = StatusParcela.SituacaoLiberada();

                            if (ov.ParcelaContrato != null)
                            {
                                ov.ParcelaContrato.statusParcela_ID = StatusParcela.SituacaoLiberada();
                            }
                        }
                        else
                        {
                            ov.dataAutorizacao = null;
                            ov.usuarioAutorizador_id = null;
                            ov.statusParcela_ID = StatusParcela.SituacaoPendente();

                            if (ov.ParcelaContrato != null)
                            {
                                ov.ParcelaContrato.statusParcela_ID = StatusParcela.SituacaoPendente();
                            }
                        }
                    }
                    else
                    {
                        msgextra = ", 1 ou mais notas não autorizadas por exceder o limite de valores";
                    }

                }
            }

            db.SaveChanges();
            ViewBag.msg = "Autorizado com sucesso." + msgextra;

            return View();
        }



        [HttpPost]
        public JsonResult ConsultaAutorizacao(string dataInicial, string dataFinal, int opc)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);


            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            List<OrdemVenda> nfs = new List<OrdemVenda>();
            if (opc == 1)
                nfs = new OrdemVenda().ObterTodosNaoAutorizado(DataInicial, DataFinal, _paramBase);
            else
                nfs = new OrdemVenda().ObterTodosAutorizado(DataInicial, DataFinal, _paramBase);

            List<RetornoNota> rets = new List<RetornoNota>();

            foreach (var item in nfs)
            {
                string ParcelaAux = "";
                string ContratoItemAux = "";
                string ContratoAux = "";

                if (item.ParcelaContrato != null)
                {
                    ParcelaAux = item.ParcelaContrato.parcela.ToString();
                    if (item.ParcelaContrato.ContratoItem != null)
                    {
                        ContratoItemAux = item.ParcelaContrato.ContratoItem.ItemProdutoServico.descricao;
                        if (item.ParcelaContrato.ContratoItem.Contrato != null)
                        {
                            ContratoAux = item.ParcelaContrato.ContratoItem.Contrato.descricao;
                        }
                    }
                }

                rets.Add(new RetornoNota
                {
                    Cliente = item.Pessoa.nome,
                    OV = item.Numero.ToString(),
                    id = item.id,
                    RPS = "",
                    Valor = item.valor.ToString("0.00"),
                    Data = item.data.ToString("dd/MM/yyyy"),
                    Parcela = (item.ParcelaContrato != null) ? item.ParcelaContrato.parcela.ToString() : "",
                    Situacao = (item.statusParcela_ID == StatusParcela.SituacaoPendente() ? "Pendente" : "Liberada"),
                    ContratoItem = ContratoItemAux,
                    Contrato = ContratoAux
                });
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

            return Json(rets, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult EnviaNFService(int id)
        {
            try
            {
                var resultado = EnviaNFBussiness(id);

                if (resultado.Cabecalho.Sucesso.ToLower() == "false")
                    return Json(new { CDStatus = "NOK", DSMessage = "Comando não aceito na prefeitura, verifique os erros", Erros = resultado.Erro }, JsonRequestBehavior.AllowGet);

                return Json(new { CDStatus = "OK", Alerts = resultado.Alerta }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private DTORetornoNFEs EnviaNFBussiness(int id)
        {
            var objPedidoEnvioLoteRPS = new DTONotaFiscal();

            var objEstab = _paramBase.estab_id;
            var objNF = new NotaFiscal().ObterPorId(id);
            var resultado = new DTORetornoNFEs();

            AtualizaNFServiceValidar(_estabobj, objNF);

            var listaNF = new List<NotaFiscal>();
            listaNF.Add(objNF);

            string caminhoArquivo;
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert;
            ObtemCertificadoX509(_estabobj, out caminhoArquivo, out cert);

            if (objNF.situacaoPrefeitura_id == Models.NotaFiscal.NFCANCELADAEMCONF)
            {
                var objCancelamentoNFe = new DTONotaFiscal();

                new Conversao().ConverterNFEs(objCancelamentoNFe, _estabobj, listaNF);
                
                var service = new SoftFin.NFSe.Business.SFCancelamentoNFSe();
                resultado = service.Execute(objCancelamentoNFe, cert, "");
                new Conversao().ConverteRetornoGravaLog(resultado,objNF.id,_usuario);
                
            }
            else if (objNF.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA) 
            {

                new Conversao().ConverterNFEs(objPedidoEnvioLoteRPS, _estabobj, listaNF);

                var arquivoxml = CriaPastaeNomeXML();

                var service = new SoftFin.NFSe.Business.SFEnvioLoteRPS();

                resultado = service.Execute(objPedidoEnvioLoteRPS, cert, arquivoxml);
                new Conversao().ConverteRetornoGravaLog(resultado, objNF.id, _usuario);
            }
            else
            {
                throw new Exception("Situação de Nota Fiscal inválida.");
            }





            try
            {
                cert = null;
                System.IO.File.Delete(caminhoArquivo);
            }
            catch
            {
            }

            return resultado;
        }

        private string CriaPastaeNomeXML()
        {
            var uploadPath = Server.MapPath("~/OFXTemp/");
            Directory.CreateDirectory(uploadPath);
            var arquivoxml = Path.Combine(Server.MapPath("~/OFXTemp/"), "NFEnvio" + _estabobj.id + ".xml");

            try
            {
                System.IO.File.Delete(arquivoxml);
            }
            catch
            {
            }
            return arquivoxml;
        }

        private void ObtemCertificadoX509(Estabelecimento objEstab, out string caminhoArquivo, out System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            var uploadPath = Server.MapPath("~/CertTMP/");
            Directory.CreateDirectory(uploadPath);

            var nomearquivonovo = Guid.NewGuid().ToString();

            caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

            cert = UtilSoftFin.BuscaCert(_estab, objEstab.senhaCertificado, caminhoArquivo, objEstab.CNPJ);
        }





        //private void ConverterCancelamentoNF(
        //        DTOPedidoCancelamentoNFe obj, 
        //        Estabelecimento objEstab,
        //        List<NotaFiscal> objNFs)
        //{

        //    var FormatoData = "yyyy-MM-dd";

        //    foreach (var objNF in objNFs)
        //    {
        //        var chaveRps = new tpChaveNFe();
        //        chaveRps.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();
        //        chaveRps.NumeroNFe = objNF.numeroNfse.ToString();
        //        obj.Detalhe.ChaveNFe = chaveRps;
        //    }
            
        //    obj.Cabecalho.cPFCNPJRemetente.CNPJ = UtilSoftFin.ReplaceCNPJCPF(objEstab.CNPJ);
        //}



        [HttpGet]
        public JsonResult AtualizaNFService(int id)
        {
            try
            {
                DbControle db;
                NotaFiscal objNF;
                DTORetornoNFEs resultado;
                
                AtualizaNFBussines(id, out db, out objNF, out resultado);

                if (resultado.Cabecalho.Sucesso.ToLower() == "false")
                    return Json(new { CDStatus = "NOK", DSMessage = "Comando não aceito na prefeitura, verifique os erros", Alertas = resultado.Alerta, Erros = resultado.Erro, NFes = resultado.NFe }, JsonRequestBehavior.AllowGet);


                
                objNF.numeroNfse = int.Parse(resultado.NFe.First().ChaveNFe.NumeroNFe);
                objNF.codigoVerificacao = resultado.NFe.First().ChaveNFe.CodigoVerificacao;
                objNF.situacaoPrefeitura_id = 2;
                objNF.Alterar(_paramBase, db);


                return Json(new { CDStatus = "OK", Alertas = resultado.Alerta, Erros = resultado.Erro, NFes = resultado.NFe }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private void AtualizaNFBussines(int id, out DbControle db, out NotaFiscal objNF, out DTORetornoNFEs resultado)
        {
            //var service = new SoftFin.NFSe.SaoPaulo.Bussiness.ConsultaNFe();
            var obj = new DTORetornoNFEs();

            db = new DbControle();
            var objEstab = _paramBase.estab_id;
            objNF = new NotaFiscal().ObterPorId(id, db);


            //AtualizaNFServiceValidar(objEstab, objNF);

            var dTONF = new DTONotaFiscal();
            var listaNFs = new List<NotaFiscal>();
            listaNFs.Add(objNF);
            new Conversao().ConverterNFEs(dTONF, _estabobj, listaNFs);
            var arquivoxml = CriaPastaeNomeXML();


            //obj.NFe.First().CnpjPrestador.CNPJ = UtilSoftFin.ReplaceCNPJCPF(objEstab.CNPJ);
            ////obj.Detalhe.ChaveNFe.NumeroNFe = "1";
            //obj.Detalhe.ChaveNFe.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();
            //obj.Detalhe.ChaveRPS.NumeroRPS = objNF.numeroRps.ToString();
            //obj.Detalhe.ChaveRPS.SerieRPS = objNF.serieRps;
            //obj.Detalhe.ChaveRPS.InscricaoPrestador = objEstab.InscricaoMunicipal.ToString();// UtilSoftFin.ReplaceCNPJCPF(objEstab.CNPJ);


            var uploadPath = Server.MapPath("~/CertTMP/");
            Directory.CreateDirectory(uploadPath);
            var nomearquivonovo = Guid.NewGuid().ToString();
            string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
            var cert = UtilSoftFin.BuscaCert(_estab, _estabobj.senhaCertificado, caminhoArquivo, _estabobj.CNPJ);

            //resultado = service.Execute(objNF, cert);

            var regra = new SoftFin.NFSe.Business.SFConsultaNFSe();

            var result = regra.Execute(dTONF, cert);

            resultado = result;

            try
            {
                cert = null;
                System.IO.File.Delete(caminhoArquivo);
            }
            catch
            {
            }
        }




        //[HttpGet]




        private static void AtualizaNFServiceValidar(Estabelecimento objEstab, NotaFiscal objNF)
        {
            if (objNF == null)
                throw new Exception("Nota não encotrada.");

            if (objNF.estabelecimento_id != objEstab.id)
                throw new Exception("Estabelecimento não compativel.");

            if (objNF.numeroRps == 0)
                throw new Exception("RPS não emitido.");

            if (objNF.serieRps == null)
                throw new Exception("RPS não emitido.");
        }


        //[HttpPost]
        //public ActionResult Create(Operacao obj)
        //{
        //    CarregaViewData();
        //    if (ModelState.IsValid)
        //    {
        //        Operacao operacao = new Operacao();
        //        if (operacao.Incluir(obj) == true)
        //        {
        //            ViewBag.msg = "Nota Fiscal incluído com sucesso";
        //            return View(obj);
        //        }
        //        else
        //        {
        //            ViewBag.msg = "Nota Fiscal já cadastrado";
        //            return View(obj);
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Dados Invalidos");
        //        return View(obj);
        //    }
        //}

        //public ActionResult Create()
        //{
        //    CarregaViewData();

        //    return View(new Recebimento());
        //}

        //[HttpPost]
        //public ActionResult Edit(Operacao obj)
        //{
        //    CarregaViewData();
        //    if (ModelState.IsValid)
        //    {
        //        Operacao operacao = new Operacao();
        //        operacao.Alterar(obj);

        //        ViewBag.msg = "Nota Fiscal alterado com sucesso";
        //        return View(obj);

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Dados Invalidos");
        //        return View(obj);

        //    }
        //}

        //public ActionResult Edit(int ID)
        //{
        //    CarregaViewData();
        //    Operacao obj = _banco.Operacao.Where(p => p.id == ID).FirstOrDefault();
        //    return View(obj);
        //}

        //[HttpPost]
        //public ActionResult Delete(Operacao obj)
        //{
        //    CarregaViewData();

        //    Operacao usu = new Operacao();
        //    usu.Excluir(obj.id);

        //    ViewBag.msg = "Nota Fiscal excluído com sucesso";
        //    return RedirectToAction("/Index", "Operacao");

        //}
        //public ActionResult Delete(int ID)
        //{
        //    CarregaViewData();
        //    Operacao obj = _banco.Operacao.Where(p => p.id == ID).FirstOrDefault();
        //    return View(obj);
        //}
        //private void CarregaViewData()
        //{
        //    DbControle banco = new DbControle();


        //    var notasFiscais = banco.NotasFiscais.ToList();
        //    ViewData["notafiscal"] = new SelectList(notasFiscais, "id", "Descricao");

        //    var bancos = banco.Bancos.ToList();
        //    ViewData["banco"] = new SelectList(bancos, "id", "Descricao");
        //}


    }

    public class RetornoNota
    {
        public int id { get; set; }
        public string RPS { get; set; }
        public string Cliente { get; set; }
        public string Data { get; set; }
        public string Valor { get; set; }
        public string Situacao { get; set; }
        public string OV { get; set; }
        public string Contrato { get; set; }
        public string ContratoItem { get; set; }
        public string Parcela { get; set; }
    }


    public class ModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var ModelType = bindingContext.ModelType;
            var Instance = Activator.CreateInstance(ModelType);
            var Form = bindingContext.ValueProvider;

            foreach (var Property in ModelType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                Type PropertyType = Property.PropertyType;

                // I'm using an ORM so this checks whether or not the property is a
                // reference to another object
                if (!(PropertyType.GetGenericArguments().Count() > 0))
                {
                    // This is the not so generic part.  It really just checks whether or 
                    // not it is a custom object.  Also the .Load() method is specific
                    // to the ORM
                    if (PropertyType.FullName.StartsWith("Objects.Models"))
                    {
                        var Load = PropertyType.GetMethod("Load", BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static, null, new Type[] { typeof(long) }, null);
                        var Value = Load.Invoke(new object(), new object[] { long.Parse(Form.GetValue(Property.Name + ".id").AttemptedValue) });
                        Property.SetValue(Instance, Value, null);
                    }
                    // checkboxes are weird and require a special case
                    else if (PropertyType.Equals(typeof(bool)))
                    {
                        if (Form.GetValue(Property.Name) == null)
                        {
                            Property.SetValue(Instance, false, null);
                        }
                        else if (Form.GetValue(Property.Name).Equals("on"))
                        {
                            Property.SetValue(Instance, true, null);
                        }
                    }
                    else
                    {
                        Property.SetValue(Instance, Convert.ChangeType(bindingContext.ValueProvider.GetValue(Property.Name).AttemptedValue, PropertyType), null);
                    }
                }
            }

            return Instance;
        }
    }
}
