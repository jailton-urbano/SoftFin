using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SoftFin.Utils;
using System.Drawing;
using ImageProcessor;
using System.Data.Entity;

namespace SoftFin.Web.Controllers
{
    public class CPAGController : BaseController
    {
        #region Novo Contas a Pagar

        #region JsonResult
        [HttpGet]
        public JsonResult GerarHistoricoProjeto(int? id)
        {

            try
            {
                DbControle db = new DbControle();

                if (id == null)
                {
                    GeraTodosHitoricoProjeto(db);
                }
                else
                {
                    GerarPorCpagHistoricoProjeto(id, db);
                }

                return Json("OK",JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
     

        }

        private void GerarPorCpagHistoricoProjeto(int? id, DbControle db)
        {
            var cpag = new DocumentoPagarMestre().ObterPorId(id.Value, db, _paramBase);
            cpag.ReferenciaProjeto = TrataHistoricoProjeto(cpag.id, db);
            cpag.Alterar(_paramBase, db);
        }

        private void GeraTodosHitoricoProjeto(DbControle db)
        {
            var cpags = new DocumentoPagarMestre().ObterTodos(_paramBase, db);
            foreach (var item in cpags)
            {
                item.ReferenciaProjeto = TrataHistoricoProjeto(item.id, db);
                item.Alterar(_paramBase, db);
            }
        }

        private string TrataHistoricoProjeto(int id, DbControle db)
        {
            var retorno = "";
            var participacaoRateio = new DocumentoPagarProjeto().ObterPorCPAG(id,db);

            foreach (var item in participacaoRateio)
            {
                var projeto = participacaoRateio.Where(p => p.Projeto_Id == item.Projeto_Id).FirstOrDefault();
                var porcentagem = Math.Round((projeto.Valor / item.DocumentoPagarMestre.valorBruto) * 100, 2);
                var valorrateioprojeto = item.Valor;
                if (retorno != "")
                    retorno += " - ";

                retorno += item.Projeto.nomeProjeto + " (" + porcentagem.ToString("n") + "%) " + valorrateioprojeto.ToString("n");
            }

            return retorno;
        }

        public JsonResult obtemFiltro()
        {

            var filtro = new {
                dataVencimentoIni = DateTime.Parse(DateTime.Now.ToShortDateString()).AddDays(-30).ToString("o"),
                dataVencimentoFim = DateTime.Parse(DateTime.Now.ToShortDateString()).ToString("o"),
                valorBrutoIni = 0,
                valorBrutoFim = 999999999.99,
                dataDocumentoIni = "",
                dataDocumentoFim = ""
            };
            return Json(filtro, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterProjetos()
        {
            var objs = new Projeto().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nomeProjeto + ((p.ContratoItem == null) ? "" : "(" + p.ContratoItem.Contrato.Pessoa.nome + ")")
            }), JsonRequestBehavior.AllowGet);
        }

        public class dtoFiltro
        {
            public DateTime? dataLancamentoIni { get; set; }
            public DateTime? dataLancamentoFim { get; set; }
            public DateTime? dataVencimentoIni { get; set; }
            public DateTime? dataVencimentoFim { get; set; }
            public Decimal? valorBrutoIni { get; set; }

            public Decimal? valorBrutoFim { get; set; }

        }

        [HttpPost]
        public JsonResult CalculaParcelas(DateTime vencimento, DateTime vencimentoPrevisto, int parcelas, decimal valorBruto, string historico, string sobra)
        {
            try
            {
                if (vencimento == null)
                    return Json(new { CDStatus = "NOK", DSMessage = "Informe o vencimento" }, JsonRequestBehavior.AllowGet);

                if (vencimentoPrevisto == null)
                    return Json(new { CDStatus = "NOK", DSMessage = "Informe o vencimento previsto" }, JsonRequestBehavior.AllowGet);

                if (parcelas == 0)
                    parcelas = 1;

                var returno = new List<DocumentoPagarParcela>();
                for (int i = 1; i <= parcelas; i++)
                {
                    var item = new DocumentoPagarParcela();
                    item.valor = (valorBruto / parcelas);
                    item.valor = Math.Round(item.valor, 2);
                    item.vencimento = vencimento;
                    item.vencimentoPrevisto = vencimentoPrevisto;
                    item.historico = historico;
                    item.parcela = i;
                    returno.Add(item);

                    vencimento = vencimento.AddMonths(1);
                    vencimentoPrevisto = vencimentoPrevisto.AddMonths(1);

                }
                var sobravalor = valorBruto - returno.Sum(p => p.valor);
                if (sobra == "U")
                    returno.Last().valor += sobravalor;
                else
                    returno.First().valor += sobravalor;

                return Json(new
                {
                    CDStatus = "OK",
                    CPAGPARCELAS = returno.Select(p => new {
                        p.DocumentoPagarMestre_id,
                        p.historico, p.id,
                        p.lotePagamentoBanco,
                        p.parcela,
                        p.usuarioAutorizador_id,
                        p.valor,
                        vencimento = p.vencimento.ToString("o"),
                        vencimentoPrevisto = p.vencimentoPrevisto.ToString("o")
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult ObtemTodos(dtoFiltro data)
        {
            DocumentoPagarMestre obj = new DocumentoPagarMestre();
            var objs = new DocumentoPagarMestre().ObterTodos2(_paramBase);
            _eventos.Info("Pesquisa Contas a Pagar", data);

            if ((data.dataVencimentoIni != null) && (data.dataVencimentoFim != null))
            {
                objs = objs.Where(p => p.DocumentoPagarParcelas.
                    Where(y => DbFunctions.TruncateTime(y.vencimentoPrevisto) >= DbFunctions.TruncateTime(data.dataVencimentoIni.Value)
                    && DbFunctions.TruncateTime(y.vencimentoPrevisto) <= DbFunctions.TruncateTime(data.dataVencimentoFim.Value)).Count() > 0);
            }
            else
            {
                if ((data.dataVencimentoIni != null))
                {
                    objs = objs.Where(p => p.DocumentoPagarParcelas.Where(y => DbFunctions.TruncateTime(y.vencimentoPrevisto) >= DbFunctions.TruncateTime(data.dataVencimentoIni.Value)).Count() > 0);
                }
                if ((data.dataVencimentoFim != null))
                {
                    objs = objs.Where(p => p.DocumentoPagarParcelas.Where(y => DbFunctions.TruncateTime(y.vencimentoPrevisto) <= DbFunctions.TruncateTime(data.dataVencimentoFim.Value)).Count() > 0);
                }
            }
            if (data.valorBrutoIni != null)
            {
                objs = objs.Where(p => p.valorBruto >= data.valorBrutoIni);
            }
            if (data.valorBrutoFim != null)
            {
                objs = objs.Where(p => p.valorBruto <= data.valorBrutoFim);
            }
            if (data.dataLancamentoIni != null)
            {
                objs = objs.Where(p => DbFunctions.TruncateTime(p.dataDocumento) >= DbFunctions.TruncateTime(data.dataLancamentoIni));
            }
            if (data.dataLancamentoFim != null)
            {
                objs = objs.Where(p => DbFunctions.TruncateTime(p.dataDocumento) <= DbFunctions.TruncateTime(data.dataLancamentoFim));
            }

            var objAuxs = objs.ToList().Select(item => new
            {
                situacao = ClassificaSituacao(item.StatusPagamento),
                banco = item.Banco.nomeBanco + " - " + item.Banco.agencia + "-" + item.Banco.contaCorrente,

                pessoa = item.Pessoa.nome,
                Agencia = item.Pessoa.agenciaConta,
                Conta = item.Pessoa.contaBancaria,
                DigitoConta = item.Pessoa.digitoContaBancaria,
                descricao = item.tipoDocumento.descricao,
                dataLancamento = item.dataLancamento.ToString("o"),
                planoContas = item.PlanoDeConta.codigo + "-" + item.PlanoDeConta.descricao,
                valorBruto = item.valorBruto,
                dataDocumento = item.dataDocumento.ToShortDateString(),
                numeroDocumento = item.numeroDocumento.ToString(),
                id = item.id,
                qtdParcelas = item.qtdParcelas,
                
                ProjetoNome = item.ReferenciaProjeto, //(item.Projeto == null) ? "" : item.Projeto.nomeProjeto + ((item.Projeto.ContratoItem == null) ? "" : "(" + item.Projeto.ContratoItem.Contrato.Pessoa.nome) + ")",
                ultParcelaPaga = (item.DocumentoPagarParcelas.Where(p => p.statusPagamento != 1).Count() > 0) ? item.DocumentoPagarParcelas.Where(p => p.statusPagamento != 1).Last() : null,
                ultParcelaAvencer = (item.DocumentoPagarParcelas.Where(p => p.statusPagamento == 1).Count() > 0) ? item.DocumentoPagarParcelas.Where(p => p.statusPagamento == 1).First() : null,
                qtdImagens = item.QtdArquivosUpload, //new DocumentoPagarArquivo().ObterPorCPAG(item.id, _paramBase).Count(),
                projetos = item.DocumentoPagarProjetos
            });


            return Json(

                objAuxs.Select(p => new
                {
                    p.ProjetoNome,
                    p.valorBruto,
                    p.banco,
                    p.dataDocumento,
                    p.dataLancamento,
                    p.id,
                    p.numeroDocumento,
                    numeroDocumento2 = "ND" + p.numeroDocumento,
                    p.pessoa,
                    p.planoContas,
                    p.qtdImagens,
                    p.qtdParcelas,
                    p.situacao,
                    projetos = p.projetos.Select(x => new { x.Historico, x.Id, x.Projeto_Id, x.Valor, x.Projeto.nomeProjeto }),
                    ultParcelaAvencer = (p.ultParcelaAvencer) == null ? null : new
                    {
                        p.ultParcelaAvencer.DocumentoPagarMestre_id,
                        p.ultParcelaAvencer.historico,
                        p.ultParcelaAvencer.id,
                        p.ultParcelaAvencer.lotePagamentoBanco,
                        p.ultParcelaAvencer.parcela,
                        vencimentoPrevisto = p.ultParcelaAvencer.vencimentoPrevisto.ToString("o"),
                        p.ultParcelaAvencer.valor,
                        situacao = ClassificaSituacao(p.ultParcelaAvencer.statusPagamento),
                        aprovado = ClassificaAutorizado(p.ultParcelaAvencer.usuarioAutorizador_id)
                    },

                    ultParcelaPaga = (p.ultParcelaPaga) == null ? null : new
                    {
                        p.ultParcelaPaga.DocumentoPagarMestre_id,
                        p.ultParcelaPaga.historico,
                        p.ultParcelaPaga.id,
                        p.ultParcelaPaga.lotePagamentoBanco,
                        p.ultParcelaPaga.parcela,
                        vencimentoPrevisto = p.ultParcelaPaga.vencimentoPrevisto.ToString("o"),
                        p.ultParcelaPaga.valor,
                        situacao = ClassificaSituacao(p.ultParcelaPaga.statusPagamento),
                        aprovado = ClassificaAutorizado(p.ultParcelaPaga.usuarioAutorizador_id)
                    }
                })
                , JsonRequestBehavior.AllowGet);
        }

        private string ClassificaSituacao(int? situacao)
        {

            switch (situacao)
            {
                case null:
                    return "1 - Em Aberto";
                case DocumentoPagarMestre.DOCEMABERTO:
                    return "1 - Em Aberto";
                case DocumentoPagarMestre.DOCPAGOTOTAL:
                    return "3 - Pago Total";
                case DocumentoPagarMestre.DOCPAGOPARC:
                    return "2 - Pago Parcialmente";
                default:
                    return "Outro";
            }
        }


        private static string ClassificaAutorizado(object situacao)
        {
            if (situacao == null)
                return "Não";
            else
                return "Sim";
        }

        private class dtoAux
        {
            public string numeroDocumento { get; set; }
            public string dataLancamento { get; set; }
            public string descricao { get; set; }
            public string banco { get; set; }
            public string nome { get; set; }
            public string pessoa { get; set; }

            public string tipoDocumento { get; set; }

            public string dataVencimento { get; set; }
            public string planoContas { get; set; }

            public decimal valorBruto { get; set; }
            public string dataDocumento { get; set; }

            public string situacao { get; set; }
            public string aprovado { get; set; }


            public int id { get; set; }

            public int qtdImagens { get; set; }

            public int qtdParcelas { get; set; }

            public DocumentoPagarParcela ultParcelaPaga { get; set; }

            public DocumentoPagarParcela ultParcelaAvencer { get; set; }
            public int? Projeto_Id { get; set; }
            public string ProjetoNome { get;  set; }
            public string Agencia { get;  set; }
            public string Conta { get;  set; }
            public string DigitoConta { get;  set; }
        }


        public JsonResult ObtemEntidade(int id, Boolean copiar = false)
        {

            var obj = new DocumentoPagarMestre().ObterPorId(id, _paramBase);

            var itens = new List<DocumentoPagarDetalhe>();
            GPS gps = null;
            DARF darf = null;
            FGTS fgts = null;


            if (id != 0)
            {

                if (obj != null)
                {
                    if (copiar)
                    {
                        obj.id = 0;
                        obj.dataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        obj.dataLancamento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        obj.dataDocumento = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                        obj.dataCompetencia = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia().ToString("MM/yyyy");
                        obj.tipolancamento = "P";

                        var bcaux = new Banco().ObterPrincipal(_paramBase);
                        if (bcaux != null)
                            obj.banco_id = bcaux.id;
                    }

                    itens = new DocumentoPagarDetalhe().ObterPorCPAG(id);
                    if (obj.tipodocumento_id == 66)
                    {
                        gps = new GPS().ObterPorCPAG(id);

                        if ((copiar) && (gps != null))
                        {
                            obj.id = 0;
                        }
                    }
                    if (obj.tipodocumento_id == 67)
                    {
                        darf = new DARF().ObterPorCPAG(id);
                        if ((copiar) && (darf != null))
                        {
                            obj.id = 0;
                        }
                    }
                    if (obj.tipodocumento_id == 68)
                    {
                        fgts = new FGTS().ObterPorCPAG(id);
                        if ((copiar) && (fgts != null))
                        {
                            obj.id = 0;
                        }
                    }
                }
            }
            else
            {

                obj = new DocumentoPagarMestre();
                obj.dataLancamento = DateTime.Now;
                obj.dataDocumento = DateTime.Now;
                //obj.dataVencimento = DateTime.Now;

                var bcaux = new Banco().ObterPrincipal(_paramBase);
                if (bcaux != null)
                    obj.banco_id = bcaux.id;

                //obj.dataVencimentoOriginal = DateTime.Now;
                obj.dataCompetencia = DateTime.Now.ToString("MM/yyyy");
                obj.estabelecimento_id = _estab;
                obj.tipolancamento = "P";

            }

            return Json(new
            {
                CPAG = new
                {
                    banco_id = obj.banco_id.ToString(),
                    obj.codigoPagamento,
                    obj.CodigoVerificacao,
                    dataAlteracao = (obj.dataAlteracao == null) ? "" : obj.dataAlteracao.Value.ToString("o"),
                    obj.dataCompetencia,
                    dataDocumento = obj.dataDocumento.ToString("o"),
                    dataInclusao = (obj.dataInclusao == null) ? "" : obj.dataInclusao.Value.ToString("o"),
                    dataLancamento = obj.dataLancamento.ToString("o"),
                    //dataPagamanto = (obj.dataPagamanto == null) ? "" : obj.dataPagamanto.Value.ToString("o"),
                    //dataVencimento = obj.dataVencimento.ToString("o"),
                    //dataVencimentoOriginal = obj.dataVencimentoOriginal.ToString("o"),
                    obj.documentopagaraprovacao_id,
                    obj.estabelecimento_id,
                    obj.id,
                    obj.LinhaDigitavel,
                    //obj.lotePagamentoBanco,
                    obj.numeroDocumento,
                    obj.pessoa_id,
                    planoDeConta_id = obj.planoDeConta_id.ToString(),
                    obj.situacaoPagamento,
                    obj.StatusPagamento,
                    tipodocumento_id = obj.tipodocumento_id.ToString(),
                    obj.tipolancamento,
                    obj.usuarioalteracaoid,
                    //obj.usuarioAutorizador_id,
                    obj.usuarioinclusaoid,
                    obj.valorBruto,
                    pessoa_desc = (obj.pessoa_id == 0) ? "" : obj.Pessoa.nome + ", " + obj.Pessoa.cnpj
                },
                CPAGPARCELA = new
                {
                    id = 0,
                    vencimento = DateTime.Now.ToString("o"),
                    vencimentoPrevisto = DateTime.Now.ToString("o"),
                    parcelas = 1,
                    historico = "",
                    sobra = "U"
                },
                CPAGITENS = itens.Select
                (
                    p => new
                    {
                        p.historico,
                        p.estabelecimento_id,
                        p.id,
                        p.unidadenegocio_id,
                        p.valor,
                        UnidadeNegocio_desc = p.UnidadeNegocio.unidade
                    }
                ),
                GPS = (gps == null) ? null : (new
                {
                    gps.codigoPagamento,
                    gps.competencia,
                    dataArrecadacao = gps.dataArrecadacao.ToString("o"),
                    gps.identificador,
                    gps.informacoesComplementares,
                    gps.nomeContribuinte,
                    gps.valorArrecadado,
                    gps.valorAtualizacaoMonetaria,
                    gps.valorOutrasEntidades,
                    gps.valorTributo
                }),
                DARF = (darf == null) ? null : (new
                {
                    darf.cnpj,
                    darf.codigoReceita,
                    dataVencimento = darf.dataVencimento.ToString("o"),
                    darf.DocumentoPagarMestre_id,
                    darf.estabelecimento_id,
                    darf.ID,
                    darf.jurosEncargos,
                    darf.multa,
                    darf.nomeContribuinte,
                    darf.numeroReferencia,
                    periodoApuracao = darf.periodoApuracao.ToString("o"),
                    darf.valorPrincipal,
                    darf.valorTotal
                }),
                FGTS = (fgts == null) ? null : (new
                {
                    fgts.cnpj,
                    fgts.codigoBarras,
                    fgts.codigoReceita,
                    dataPagamento = fgts.dataPagamento.ToString("o"),
                    fgts.digitoLacre,
                    fgts.DocumentoPagarMestre_id,
                    fgts.estabelecimento_id,
                    fgts.ID,
                    fgts.identificadorFgts,
                    fgts.lacreConectividadeSocial,
                    fgts.nomeContribuinte,
                    fgts.tipoInscricao,
                    fgts.valorPagamento
                }),
                CPAGPARCELAS = (copiar) ? null : (
                    obj.DocumentoPagarParcelas.Select(
                     p => new {
                         p.DocumentoPagarMestre_id,
                         p.historico,
                         p.id,
                         p.lotePagamentoBanco,
                         p.parcela,
                         p.usuarioAutorizador_id,
                         p.valor,
                         vencimento = p.vencimento.ToString("o"),
                         vencimentoPrevisto = p.vencimentoPrevisto.ToString("o"),
                         status = ClassificaSituacao(p.statusPagamento)
                     }
                    )
                ),
                PROJETOSITEMS = (copiar) ? null : (
                    obj.DocumentoPagarProjetos.Select(
                     p => new {
                         p.DocumentoPagarMestre_id ,
                         p.Id,
                         p.Historico,
                         p.Projeto_Id,
                         p.Valor,
                         ProjetoNome = p.Projeto.nomeProjeto
                     }
                    )
                )
            }
            , JsonRequestBehavior.AllowGet);

        }
        public JsonResult ListaTipoDocumento()
        {
            var data = new SelectList(new TipoDocumento().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaTipoLancamento()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "P", Text = "Provisório", Selected = true });
            items.Add(new SelectListItem() { Value = "R", Text = "Real", Selected = true });
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaContaContabil()
        {
            var pc = new PlanoDeConta().ObterTodosTipoA();
            return Json(pc, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaUnidadeNegocio()
        {
            return Json(new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaCliente()
        {
            return Json(new Pessoa().ObterClienteComCNPJ(_paramBase), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaPessoas()
        {
            return Json(new Pessoa().ObterTodosComCNPJ(_paramBase), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaFornecedor()
        {
            //return Json(new Pessoa().ObterFornecedorComCNPJ(), JsonRequestBehavior.AllowGet);
            return Json(new Pessoa().ObterTodosComCNPJ(_paramBase), JsonRequestBehavior.AllowGet);
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


        public JsonResult Salvar(DocumentoPagarMestre cpag, List<DocumentoPagarDetalhe> cpagItems, List<DocumentoPagarParcela> cpagParcelas, DARF darf, GPS gps, FGTS fgts, List<DocumentoPagarProjeto> projetos)
        {
            try
            {
                if (cpag.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                if (cpagItems == null)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Erro! Informe o detalhe do lançamento" }, JsonRequestBehavior.AllowGet);
                }


                if (cpagItems.Count() == 0)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Erro! Informe o detalhe do lançamento" }, JsonRequestBehavior.AllowGet);
                }
                decimal tot = cpagItems.Sum(p => p.valor);
                if (cpag.valorBruto != tot)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Erro! valor bruto diferente da somatoria das parcelas" }, JsonRequestBehavior.AllowGet);
                }


                if (projetos != null)
                {
                    if (projetos.Count() > 0)
                    {
                        decimal totprojeto = projetos.Sum(p => p.Valor);
                        if (cpag.valorBruto != totprojeto)
                        {
                            return Json(new { CDMessage = "NOK", DSErroReturn = "Erro! valor bruto diferente do projeto" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }

                if (cpagParcelas == null)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe as parcelas" }, JsonRequestBehavior.AllowGet);
                }


                if (cpagParcelas.Count() == 0)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe as parcelas" }, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in cpagParcelas)
                {
                    if (item.vencimentoPrevisto.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de Vencimento Prevista inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }

                    if (item.vencimento.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de Vencimento inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }
                }


                //if (cpagParcelas.Sum(p=> p.valor) != cpag.valorBruto)
                //{
                //    return Json(new { CDMessage = "NOK", DSErroReturn = "Valor Bruto diferente das parcelas" }, JsonRequestBehavior.AllowGet);
                //}    


                if (cpag.dataDocumento.Year < 2000)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Data do Documento inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                }

                if (cpag.planoDeConta_id == 0)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe o plano de contas" }, JsonRequestBehavior.AllowGet);
                }
                if (cpag.banco_id == 0)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe o banco" }, JsonRequestBehavior.AllowGet);
                }
                if (cpag.tipodocumento_id == 0)
                {
                    return Json(new { CDMessage = "NOK", DSErroReturn = "Informe o tipo de documento" }, JsonRequestBehavior.AllowGet);
                }

                if (cpag.tipodocumento_id == 62)
                {
                    if (String.IsNullOrEmpty(cpag.LinhaDigitavel))
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Boleno CNAB é necessário o codigo de barras" }, JsonRequestBehavior.AllowGet);
                    }
                    var linhaDigitavel = cpag.LinhaDigitavel.Replace(".", "").Replace(" ", "");

                    if (cpag.LinhaDigitavel.Length != 54 )
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Boleno CNAB é necessário o codigo de barras válido" }, JsonRequestBehavior.AllowGet);
                    }


                    var varCodigoBarras = linhaDigitavel.Substring(0, 3) + //Código do Banco favorecido (0, 3)
                              linhaDigitavel.Substring(3, 1) + //Código da Moeda (3, 1)
                              linhaDigitavel.Substring(32, 1) + //DV do Código de Barras (32, 1)
                              linhaDigitavel.Substring(33, 4) + //Fator de Vencimento - ex.: 01/05/2002 (33, 4) - Data Base Febraban 
                              linhaDigitavel.Substring(37, 10) +  //Valor do Título (37, 10)
                              linhaDigitavel.Substring(4, 5) + //Campo Livre - Parte I (4, 5)
                              linhaDigitavel.Substring(10, 10) + //Campo Livre - Parte II (10, 10)
                              linhaDigitavel.Substring(21, 10); //Campo Livre - Parte III (21, 10)

                    if (calculaDV(varCodigoBarras) == false)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Boleno CNAB é necessário o codigo de barras válido" }, JsonRequestBehavior.AllowGet);
                    }
                }


                var db = new DbControle();

                if (gps != null)
                {
                    var objAux = db.GPS.Where(x =>
                        x.codigoPagamento == gps.codigoPagamento &&
                        x.competencia == gps.competencia &&
                        x.dataArrecadacao == gps.dataArrecadacao
                            && x.estabelecimento_id == _estab
                            && x.DocumentoPagarMestre_id != cpag.id);
                    if (objAux.Count() > 0)
                        return Json(new { CDMessage = "NOK", DSErroReturn = "GPS Já lançado." }, JsonRequestBehavior.AllowGet);


                    if (cpag.dataDocumento.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de arrecadação do GPS inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }

                }

                if (darf != null)
                {
                    if (darf.dataVencimento.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de vencimento do DARF inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }
                    if (darf.periodoApuracao.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de apuração do DARF inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (gps != null)
                {
                    if (gps.dataArrecadacao.Year < 2000)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Data de arrecadação do GPS inferior ao ano 2000" }, JsonRequestBehavior.AllowGet);
                    }
                }

                var documento = new DocumentoPagarMestre();
                if (cpag.id != 0)
                {
                    documento = new DocumentoPagarMestre().ObterPorId(cpag.id, db, _paramBase);
                    if (documento == null)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Documento não encontrado para a alteração" }, JsonRequestBehavior.AllowGet);
                    }
                    //if (documento.dataPagamanto != null)
                    //{
                    //    return Json(new { CDMessage = "NOK", DSErroReturn = "Documento não pode sofrer alterações por já estar baixado" }, JsonRequestBehavior.AllowGet);
                    //}
                    if (documento.StatusPagamento != 1)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Documento não pode sofrer alterações por estar parcialmente ou totalmente pago" }, JsonRequestBehavior.AllowGet);
                    }
                }


                var CPAGItens = new List<DocumentoPagarDetalhe>();
                foreach (var item in cpagItems)
                {

                    var unidadeaux = new UnidadeNegocio().ObterTodos(_paramBase).Where(p => p.unidade == item.UnidadeNegocio_desc);
                    if (unidadeaux.Count() == 0)
                    {
                        return Json(new { CDMessage = "NOK", DSErroReturn = "Unidade não encontrado" }, JsonRequestBehavior.AllowGet);
                    }

                    item.unidadenegocio_id = unidadeaux.First().id;
                    item.estabelecimento_id = _estab;
                }


                var nomePessoa = cpag.pessoa_desc.Split(',')[0];
                var cnpjPessoa = cpag.pessoa_desc.Split(',')[1];
                var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa, _paramBase);



                if (cpag.id != 0)
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        ConvertDocumento(cpag, documento, pessoa);
                        documento.usuarioinclusaoid = _usuarioobj.id;
                        documento.dataAlteracao = DateTime.Now;
                        documento.StatusPagamento = new DocumentoPagarMestre().ObterPorId(cpag.id, _paramBase).StatusPagamento;
                        AlterarCPAG(documento, cpagItems, cpagParcelas, gps, darf, fgts, projetos, db);
                        GerarPorCpagHistoricoProjeto(documento.id, db);
                        dbcxtransaction.Commit();

                    }
                }
                else
                {
                    if (cpag.RepetirLancamento == 0)
                        cpag.RepetirLancamento = 1;


                    //var ldataVencimento = cpag.dataVencimento;
                    //var ldataVencimentoOriginal = cpag.dataVencimentoOriginal;
                    var ldataDocumento = cpag.dataDocumento;
                    var lnumeroDocumento = cpag.numeroDocumento;
                    DateTime ldataCompetencia;

                    if (cpag.dataCompetencia.Length == 7)
                        ldataCompetencia = DateTime.Parse("01/" + cpag.dataCompetencia);
                    else
                        ldataCompetencia = DateTime.Parse(cpag.dataCompetencia);


                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        for (int i = 0; i < cpag.RepetirLancamento; i++)
                        {
                            var DocumentoPagarMestreInclusao = new DocumentoPagarMestre();
                            ConvertDocumento(cpag, DocumentoPagarMestreInclusao, pessoa);
                            DocumentoPagarMestreInclusao.StatusPagamento = 1;
                            DocumentoPagarMestreInclusao.situacaoPagamento = "A";
                            DocumentoPagarMestreInclusao.dataDocumento = ldataDocumento;
                            DocumentoPagarMestreInclusao.numeroDocumento = lnumeroDocumento;
                            DocumentoPagarMestreInclusao.id = 0;
                            DocumentoPagarMestreInclusao.dataCompetencia = ldataCompetencia.ToString("MM/yyyy");
                            DocumentoPagarMestreInclusao.usuarioinclusaoid = _usuarioobj.id;
                            DocumentoPagarMestreInclusao.dataInclusao = DateTime.Now;
                            DocumentoPagarMestreInclusao.qtdParcelas = cpagParcelas.Count();
                            
                            InclurCPAG(DocumentoPagarMestreInclusao, cpagItems, cpagParcelas, gps, darf, fgts, projetos, db);

                            ldataDocumento = ldataDocumento.AddMonths(1);
                            lnumeroDocumento = lnumeroDocumento + 1;
                            ldataCompetencia = ldataCompetencia.AddMonths(1);

                            if (DocumentoPagarMestreInclusao.urlNomeimagem != null)
                            {
                                var nomearquivonovo = DocumentoPagarMestreInclusao.urlNomeimagem.Replace("%", " ");
                                var uploadPath = Server.MapPath("~/TXTTemp/");
                                Directory.CreateDirectory(uploadPath);
                                string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                                AzureStorage.DownloadFile(caminhoArquivo, "CPAGTEMP/" + nomearquivonovo, ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                                AzureStorage.UploadFile(caminhoArquivo,
                                            "CPAG/" + DocumentoPagarMestreInclusao.id.ToString() + "/" + nomearquivonovo,
                                            ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());


                                var documentoPagarArquivo = new DocumentoPagarArquivo();
                                documentoPagarArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                            "CPAG/" + DocumentoPagarMestreInclusao.id.ToString() + "/" + nomearquivonovo;
                                documentoPagarArquivo.arquivoOriginal = nomearquivonovo;
                                documentoPagarArquivo.descricao = nomearquivonovo;
                                documentoPagarArquivo.documentoPagarMestre_id = DocumentoPagarMestreInclusao.id;

                                documentoPagarArquivo.Salvar(_paramBase, db);
                            }
                            GerarPorCpagHistoricoProjeto(DocumentoPagarMestreInclusao.id, db);

                        }
                        dbcxtransaction.Commit();
                    }

                }
                return Json(new { CDMessage = "OK", Success = cpag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDMessage = "NOK", DSErroReturn = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public void ConvertDocumento(DocumentoPagarMestre obj, DocumentoPagarMestre documento, Pessoa pessoa)
        {
            documento.estabelecimento_id = _estab;
            documento.pessoa_id = pessoa.id;
            documento.banco_id = obj.banco_id;
            documento.codigoPagamento = obj.codigoPagamento;
            documento.dataCompetencia = obj.dataCompetencia;
            documento.dataDocumento = obj.dataDocumento;
            //documento.dataVencimentoOriginal = obj.dataVencimento;
            //documento.dataVencimento = obj.dataVencimento;
            documento.numeroDocumento = obj.numeroDocumento;
            documento.tipodocumento_id = obj.tipodocumento_id;
            documento.tipolancamento = obj.tipolancamento;
            documento.valorBruto = obj.valorBruto;
            documento.LinhaDigitavel = obj.LinhaDigitavel;
            documento.CodigoVerificacao = obj.CodigoVerificacao;
            documento.planoDeConta_id = obj.planoDeConta_id;
            documento.urlNomeimagem = obj.urlNomeimagem;


        }

        [HttpPost]
        public JsonResult Excluir(int id)
        {

            try
            {
                var db = new DbControle();

                var pagamentos = new Pagamento().ObterPorDocumentoPagarMestreId(id, _paramBase);

                var documento = new DocumentoPagarMestre().ObterPorId(id, db, _paramBase);
                if (documento == null)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Documento não encontrado para a alteração" }, JsonRequestBehavior.AllowGet);
                }
                if (pagamentos != null)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Documento não pode sofrer alterações por possuir pagamentos" }, JsonRequestBehavior.AllowGet);
                }
                //if (documento != null)
                //{
                //    return Json(new { CDMessage = "NOK", DSErroReturn = "Documento não pode sofrer auterações por já estar autorizado" }, JsonRequestBehavior.AllowGet);
                //}



                using (var dbcxtransaction = db.Database.BeginTransaction())
                {



                    var gps = new GPS().ObterPorCPAG(id, db);

                    if (gps != null)
                        new GPS().Excluir(gps.id, _paramBase, db);


                    var darf = new DARF().ObterPorCPAG(id, db);
                    if (darf != null)
                        new DARF().Excluir(darf.ID, _paramBase, db);

                    var fgts = new FGTS().ObterPorCPAG(id, db);
                    if (fgts != null)
                        new FGTS().Excluir(fgts.ID, _paramBase, db);

                    var documentoPagarItems = new DocumentoPagarDetalhe().ObterPorCPAG(id, db);

                    foreach (var item in documentoPagarItems)
                    {
                        string erro = "";
                        new DocumentoPagarDetalhe().Excluir(item.id, ref erro, _paramBase, db);
                        if (erro != "")
                            throw new Exception(erro);
                    }

                    var docimgs = new DocumentoPagarArquivo().ObterPorCPAG(id, _paramBase, db);

                    foreach (var item in docimgs)
                    {
                        item.Excluir(item.id, _paramBase, db);
                    }

                    var doccpags = new DocumentoPagarParcela().ObterPorCPAG(id, db);

                    foreach (var item in doccpags)
                    {

                        var bancoMovimentos = new BancoMovimento().ObterPorCPAGParcela(item.id, _paramBase, db);

                        foreach (var itembv in bancoMovimentos)
                        {
                            string erro = "";
                            new BancoMovimento().Excluir(itembv.id, ref erro, _paramBase, db);
                            if (erro != "")
                                throw new Exception(erro);
                        }

                        var objLcs = new LancamentoContabil().ObterPorCPAGParcela(item.id, _paramBase, db);

                        foreach (var itemLC in objLcs)
                        {
                            var errolc = "";
                            itemLC.Excluir(itemLC.id, ref errolc, _paramBase, db);
                            if (errolc != "")
                                throw new Exception(errolc);
                        }

                        var erropag = "";
                        item.Excluir(item.id, ref erropag, _paramBase, db);
                        if (erropag != "")
                            throw new Exception(erropag);




                    }

                    var cs = new DocumentoPagarMestre();
                    cs.Excluir(id, _paramBase, db);
                    dbcxtransaction.Commit();
                }

                return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }


        #endregion

        #region Rotas
        //

        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var notaDebito = new DocumentoPagarParcela().ObterEntreData(DataInicial, DataFinal, _paramBase).Where(p => p.DocumentoPagarMestre.StatusPagamento == DocumentoPagarMestre.DOCEMABERTO).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + notaDebito }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Manut(int? id, bool? delete, bool? detail)
        {
            ViewData["btnOK"] = "OK";
            ViewData["delete"] = false;
            ViewData["detail"] = false;

            ViewData["opcao"] = "Novo";
            if (id != null)
            {
                ViewData["opcao"] = "Editar";

            }
            if (delete != null)
            {
                ViewData["opcao"] = "Exclusão";
                detail = true;
                ViewData["delete"] = true;
                ViewData["detail"] = true;
            }
            else
            {
                if (detail != null)
                {
                    ViewData["Titulo"] = "Detalhe";
                    ViewData["btnOK"] = "Detalhe";
                    detail = true;
                    ViewData["detail"] = true;
                }
            }

            ViewData["id"] = id;


            return View();
        }


        #endregion

        #region Privates

        public void InclurCPAG(DocumentoPagarMestre documento, List<DocumentoPagarDetalhe> detalhes, List<DocumentoPagarParcela> parcelas, GPS gpsvm, DARF darfvm, FGTS fgtsvm, List<DocumentoPagarProjeto> projetos,  DbControle db)
        {
            var tipoDoc = new TipoDocumento().ObterPorId(documento.tipodocumento_id, db).codigo;
            documento.Incluir(documento, detalhes, parcelas, projetos, _paramBase, db);

            if (tipoDoc == "GPS")
            {
                SalvaGPS(documento, gpsvm, db);
            }
            if (tipoDoc == "DARF")
            {

                SalvaDARF(documento, darfvm, db);
            }
            if (tipoDoc == "FGTS")
            {
                SalvaFGTS(documento, fgtsvm, db);
            }
        }
        private void SalvaFGTS(DocumentoPagarMestre documento, FGTS obj, DbControle db)
        {
            var fgts = new FGTS().ObterPorCPAG(documento.id);

            if (fgts == null)
            {
                fgts = new FGTS();
                fgts.DocumentoPagarMestre_id = documento.id;
                fgts.cnpj = obj.cnpj;
                fgts.codigoBarras = obj.codigoBarras;
                fgts.codigoReceita = obj.codigoReceita;
                fgts.dataPagamento = obj.dataPagamento;
                fgts.digitoLacre = obj.digitoLacre;
                fgts.estabelecimento_id = _estab;
                fgts.identificadorFgts = obj.identificadorFgts;
                fgts.nomeContribuinte = obj.nomeContribuinte;
                fgts.tipoInscricao = obj.tipoInscricao;
                fgts.valorPagamento = obj.valorPagamento;
                fgts.lacreConectividadeSocial = obj.lacreConectividadeSocial;
                if (fgts.Incluir(_paramBase, db) == false)
                    throw new Exception("FGTS já Lançado na base");
            }
            else
            {

                fgts.cnpj = obj.cnpj;
                fgts.codigoBarras = obj.codigoBarras;
                fgts.codigoReceita = obj.codigoReceita;
                fgts.dataPagamento = obj.dataPagamento;
                fgts.digitoLacre = obj.digitoLacre;
                fgts.estabelecimento_id = _estab;
                fgts.identificadorFgts = obj.identificadorFgts;
                fgts.nomeContribuinte = obj.nomeContribuinte;
                fgts.tipoInscricao = obj.tipoInscricao;
                fgts.valorPagamento = obj.valorPagamento;
                fgts.lacreConectividadeSocial = obj.lacreConectividadeSocial;
                fgts.Alterar(_paramBase, db);
            }
        }
        private void SalvaDARF(DocumentoPagarMestre documento, DARF darfvm, DbControle db)
        {
            var darf = new DARF().ObterPorCPAG(documento.id, db);

            if (darf == null)
            {
                darf = new DARF();

                darf.DocumentoPagarMestre_id = documento.id;
                darf.cnpj = darfvm.cnpj;
                darf.codigoReceita = darfvm.codigoReceita;

                darf.dataVencimento = darfvm.dataVencimento;
                darf.estabelecimento_id = _estab;
                darf.jurosEncargos = darfvm.jurosEncargos;
                darf.multa = darfvm.multa;
                darf.nomeContribuinte = darfvm.nomeContribuinte;
                darf.numeroReferencia = darfvm.numeroReferencia;
                darf.periodoApuracao = darfvm.periodoApuracao;
                darf.valorPrincipal = darfvm.valorPrincipal;
                darf.valorTotal = darfvm.valorTotal;
                darf.Incluir(_paramBase, db);
            }
            else
            {
                darf.DocumentoPagarMestre_id = documento.id;
                darf.cnpj = darfvm.cnpj;
                darf.codigoReceita = darfvm.codigoReceita;

                darf.estabelecimento_id = _estab;
                darf.jurosEncargos = darfvm.jurosEncargos;
                darf.multa = darfvm.multa;
                darf.nomeContribuinte = darfvm.nomeContribuinte;
                darf.numeroReferencia = darfvm.numeroReferencia;
                darf.periodoApuracao = darfvm.periodoApuracao;
                darf.valorPrincipal = darfvm.valorPrincipal;
                darf.valorTotal = darfvm.valorTotal;
                darf.Alterar(darf, db, _paramBase);
            }
        }
        private void SalvaGPS(DocumentoPagarMestre documento, GPS gpsvm, DbControle db)
        {
            var gps = new GPS().ObterPorCPAG(documento.id, db);

            if (gps == null)
            {
                gps = new GPS();
                gps.DocumentoPagarMestre_id = documento.id;
                gps.codigoPagamento = gpsvm.codigoPagamento;
                gps.competencia = gpsvm.competencia;
                gps.estabelecimento_id = _estab;
                gps.identificador = gpsvm.identificador;
                gps.informacoesComplementares = gpsvm.informacoesComplementares;
                gps.nomeContribuinte = gpsvm.nomeContribuinte;
                gps.valorArrecadado = gpsvm.valorArrecadado;
                gps.valorAtualizacaoMonetaria = gpsvm.valorAtualizacaoMonetaria;
                gps.valorOutrasEntidades = gpsvm.valorOutrasEntidades;
                gps.valorTributo = gpsvm.valorTributo;
                gps.dataArrecadacao = gpsvm.dataArrecadacao;
                gps.Incluir(_paramBase, db);
            }
            else
            {
                gps.codigoPagamento = gpsvm.codigoPagamento;
                gps.competencia = gpsvm.competencia;
                gps.identificador = gpsvm.identificador;
                gps.informacoesComplementares = gpsvm.informacoesComplementares;
                gps.nomeContribuinte = gpsvm.nomeContribuinte;
                gps.valorArrecadado = gpsvm.valorArrecadado;
                gps.valorAtualizacaoMonetaria = gpsvm.valorAtualizacaoMonetaria;
                gps.valorOutrasEntidades = gpsvm.valorOutrasEntidades;
                gps.valorTributo = gpsvm.valorTributo;
                gps.dataArrecadacao = gpsvm.dataArrecadacao;
                gps.Alterar(_paramBase, db);
            }
        }
        public void AlterarCPAG(DocumentoPagarMestre documento, List<DocumentoPagarDetalhe> detalhes, List<DocumentoPagarParcela> parcelas, GPS gps, DARF darf, FGTS fgts, List<DocumentoPagarProjeto> projetos,  DbControle db)
        {
            var tipoDoc = new TipoDocumento().ObterPorId(documento.tipodocumento_id, db).codigo;
            documento.Alterar(documento, detalhes, parcelas,projetos, db, _paramBase);

            if (tipoDoc == "GPS")
            {
                SalvaGPS(documento, gps, db);
            }
            if (tipoDoc == "DARF")
            {
                SalvaDARF(documento, darf, db);
            }
            if (tipoDoc == "FGTS")
            {
                SalvaFGTS(documento, fgts, db);
            }

        }
        #endregion

        #endregion
        public ActionResult Excel()
        {
            var obj = new DocumentoPagarMestre();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["estabelecimento_id"] = item.estabelecimento_id;
                myExport["Estabelecimento"] = item.Estabelecimento.NomeCompleto;
                myExport["pessoa_id"] = item.pessoa_id;
                myExport["dataLancamento"] = item.dataLancamento;
                myExport["dataCompetencia"] = item.dataCompetencia;
                //myExport["dataVencimentoOriginal"] = item.dataVencimentoOriginal;
                //myExport["dataVencimento"] = item.dataVencimento;
                myExport["valorBruto"] = item.valorBruto;
                myExport["tipodocumento_id"] = item.tipodocumento_id;
                myExport["tipolancamento"] = item.tipolancamento;
                myExport["numeroDocumento"] = item.numeroDocumento;
                myExport["dataDocumento"] = item.dataDocumento;
                myExport["situacaoPagamento"] = item.situacaoPagamento;
                //myExport["dataPagamanto"] = item.dataPagamanto;
                myExport["codigoPagamento"] = item.codigoPagamento;
                //myExport["lotePagamentoBanco"] = item.lotePagamentoBanco;
                myExport["Pessoa"] = item.Pessoa;
                myExport["tipoDocumento"] = item.tipoDocumento;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "DocumentoPagarMestre.csv");
        }
        public ActionResult cnpj(int id)
        {
            var pes = new Pessoa().ObterPorId(id, _paramBase);

            if (pes != null)
                if (pes.cnpj != null)
                    return Content(pes.cnpj.ToString());

            return Content("");
        }
        [HttpPost]
        public ActionResult codigoBarras(string LinhaDigitavel)
        {
            DateTime dataInicial = Convert.ToDateTime("07/10/1997");
            string varLinhaDigitavel = "";
            string varCodigoBarras = "";
            string auxLinhaDigitavel = "";
            string auxVencimento = "";
            string auxValor = "";
            string auxErro = "";

            decimal valorcalc = 0;

            varLinhaDigitavel = LinhaDigitavel.Replace(".", "").Replace(" ", "");

            if (varLinhaDigitavel.Length == 36 && varLinhaDigitavel.Substring(33, 3) == "000")
            {
                //341917531421323592044001730900026000
                varLinhaDigitavel = varLinhaDigitavel + "00000000000";
                LinhaDigitavel = varLinhaDigitavel;
            }

            if (varLinhaDigitavel.Length == 47 || varLinhaDigitavel.Length == 48)
            {
                if (varLinhaDigitavel.Substring(0, 1) == "8") //Concessionárias e IPTU
                {
                    varCodigoBarras = varLinhaDigitavel.Substring(0, 11) +
                                varLinhaDigitavel.Substring(12, 11) +
                                varLinhaDigitavel.Substring(24, 11) +
                                varLinhaDigitavel.Substring(36, 11);

                    if (varLinhaDigitavel.Length == 48 && calculaDV2(varCodigoBarras) == true)
                    {
                        auxLinhaDigitavel = varLinhaDigitavel.Substring(0, 12) + " " +
                                                    varLinhaDigitavel.Substring(12, 12) + " " +
                                                    varLinhaDigitavel.Substring(24, 12) + " " +
                                                    varLinhaDigitavel.Substring(36, 12);

                        auxVencimento = "";
                        auxValor = (Decimal.Parse(varCodigoBarras.Substring(4, 11)) / 100).ToString("n");
                    }
                    else
                    {
                        auxErro = "Linha do Boleto inválida!";
                    }
                }
                else //Bloqueto de Cobrança
                {
                    varCodigoBarras = varLinhaDigitavel.Substring(0, 3) + //Código do Banco favorecido (0, 3)
                              varLinhaDigitavel.Substring(3, 1) + //Código da Moeda (3, 1)
                              varLinhaDigitavel.Substring(32, 1) + //DV do Código de Barras (32, 1)
                              varLinhaDigitavel.Substring(33, 4) + //Fator de Vencimento - ex.: 01/05/2002 (33, 4) - Data Base Febraban 
                              varLinhaDigitavel.Substring(37, 10) +  //Valor do Título (37, 10)
                              varLinhaDigitavel.Substring(4, 5) + //Campo Livre - Parte I (4, 5)
                              varLinhaDigitavel.Substring(10, 10) + //Campo Livre - Parte II (10, 10)
                              varLinhaDigitavel.Substring(21, 10); //Campo Livre - Parte III (21, 10)

                    if (varLinhaDigitavel.Length == 47 && calculaDV(varCodigoBarras) == true)
                    {
                        auxLinhaDigitavel = varLinhaDigitavel.Substring(0, 5) + "." +
                                                  varLinhaDigitavel.Substring(5, 5) + " " +
                                                  varLinhaDigitavel.Substring(10, 5) + "." +
                                                  varLinhaDigitavel.Substring(15, 6) + " " +
                                                  varLinhaDigitavel.Substring(21, 5) + "." +
                                                  varLinhaDigitavel.Substring(26, 6) + " " +
                                                  varLinhaDigitavel.Substring(32, 1) + " " +
                                                  varLinhaDigitavel.Substring(33, 14);

                        auxVencimento = dataInicial.AddDays(Convert.ToInt32(varLinhaDigitavel.Substring(33, 4))).ToString("o");
                        auxValor = varLinhaDigitavel.Substring(37, 10);
                        valorcalc = Math.Round( Decimal.Parse(auxValor) / 100,2);
                    }
                    else
                    {
                        auxErro = "Linha do Boleto inválida!";
                    }
                }
            }
            else
            {
                auxErro = "Linha do Boleto inválida!";
            }

            return Json(new { Erro = auxErro, LinhaDigitavel = auxLinhaDigitavel, Vencimento = auxVencimento, Valor = valorcalc }, JsonRequestBehavior.AllowGet);
        }


        public void CodigoBarrasInterno(string linhaDigitavel,
            ref string linhaFormatada,
            DateTime? vencimento,
            ref decimal? valor,
            ref string referencial)
        {
            if (linhaDigitavel == "")
                return;

            DateTime dataInicial = Convert.ToDateTime("07/10/1997");
            string varLinhaDigitavel = "";
            string varCodigoBarras = "";

            string auxValor = "";

            linhaFormatada = linhaDigitavel;
            varLinhaDigitavel = linhaFormatada.Replace(".", "").Replace(" ", "");

            if (varLinhaDigitavel.Length == 32 && varLinhaDigitavel.Substring(29, 3) == "000")
            {
                //341917531421323592044001730900026000
                varLinhaDigitavel = varLinhaDigitavel + "00000000000";
                linhaFormatada = varLinhaDigitavel;
            }

            if (varLinhaDigitavel.Substring(0, 1) == "8") //Concessionárias e IPTU
            {
                varCodigoBarras = varLinhaDigitavel;
                valor = (Decimal.Parse(varCodigoBarras.Substring(4, 11)) / 100);

                var parte01 = varLinhaDigitavel.Substring(0, 11);
                var parte02 = varLinhaDigitavel.Substring(11, 11);
                var parte03 = varLinhaDigitavel.Substring(22, 11);
                var parte04 = varLinhaDigitavel.Substring(33, 11);

                var digparte01 = DigitoM11(parte01);
                var digparte02 = DigitoM11(parte02);
                var digparte03 = DigitoM11(parte03);
                var digparte04 = DigitoM11(parte04);

                referencial = parte01 + digparte01 +
                                    parte02 + digparte02 +
                                    parte03 + digparte03 +
                                    parte04 + digparte04;
            }
            else //Bloqueto de Cobrança
            {
                vencimento = dataInicial.AddDays(Convert.ToInt32(varLinhaDigitavel.Substring(29, 4)));
                auxValor = varLinhaDigitavel.Substring(35, 10);
                valor = Decimal.Parse(auxValor) / 100;
            }

            //return Json(new { Erro = auxErro, LinhaDigitavel = auxLinhaDigitavel, Vencimento = auxVencimento, Valor = auxValor }, JsonRequestBehavior.AllowGet);
        }
        public bool calculaDV(string codigoBarras) //Valida Código de Barras do Boleto
        {
            string codigoSemDv = codigoBarras.Substring(0, 4) + codigoBarras.Substring(5, 39);

            Int32 DvCodigoBarras = Convert.ToInt32(codigoBarras.Substring(4, 1));
            Int32 DvCalculado = 0;
            Int32 Acumulador = 0;
            Int32 mult = 2;
            for (int i = 42; i >= 0; i--)
            {
                Acumulador += (Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult);
                if (mult < 9)
                {
                    mult += 1;
                }
                else
                {
                    mult = 2;
                }
            }

            DvCalculado = 11 - (Acumulador % 11);
            if (DvCalculado == 10)
            {
                DvCalculado = 0;
            }
            if (DvCalculado == 11)
            {
                DvCalculado = 1;
            }
            if (DvCalculado == DvCodigoBarras)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool calculaDV2(string codigoBarras) //Valida Código de Barras Concessionárias e IPTU/ISS
        {
            string codigoSemDv = codigoBarras.Substring(0, 3) + codigoBarras.Substring(4, 40);

            Int32 DvCodigoBarras = Convert.ToInt32(codigoBarras.Substring(3, 1));
            Int32 DvCalculado = 0;
            Int32 Acumulador = 0;
            Int32 mult = 2;
            for (int i = 42; i >= 0; i--)
            {

                if ((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult) > 9)
                {
                    Acumulador += Convert.ToInt32((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult).ToString().Substring(0, 1));
                    Acumulador += Convert.ToInt32((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult).ToString().Substring(0, 2));
                }
                else
                {
                    Acumulador += (Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult);
                }
                if (mult == 2)
                {
                    mult -= 1;
                }
                else
                {
                    mult = 2;
                }
            }

            DvCalculado = 10 - (Acumulador % 10);
            if (DvCalculado == 10)
            {
                DvCalculado = 0;
            }

            if (DvCalculado == DvCodigoBarras)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void CarregaViewData()
        {

            ViewData["Pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["TipoDocumento"] = new SelectList(new TipoDocumento().ObterTodos(), "id", "descricao");

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "P", Text = "Provisório", Selected = true });
            //items.Add(new SelectListItem() { Value = "D", Text = "Definitivo" });
            ViewData["TipoLancamento"] = new SelectList(items, "Value", "Text");
            var pc = new PlanoDeConta().ObterTodos().Where(p => p.TipoConta == "A");
            ViewData["PlanoDeContas"] = new SelectList(pc, "id", "descricao");
            ViewData["UnidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");

            CarregaBanco();
            //items.Add(new SelectListItem() { Value = "A", Text = "Em Aberto", Selected = true });
            //items.Add(new SelectListItem() { Value = "P", Text = "Pago" });
            //items.Add(new SelectListItem() { Value = "C", Text = "Cancelador" });
            //ViewData["SituacaoPagamento"] = new SelectList(items, "Value", "Text");
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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string Valorpessoa_id = Request.QueryString["pessoa_id"];
            string Valorbanco = Request.QueryString["banco"];
            string ValordataLancamentoIni = Request.QueryString["dataLancamentoIni"];
            string ValordataLancamentoFim = Request.QueryString["dataLancamentoFim"];
            string ValordataCompetencia = Request.QueryString["dataCompetencia"];
            string ValordataVencimentoIni = Request.QueryString["dataVencimentoIni"];
            string ValordataVencimentoFim = Request.QueryString["dataVencimentoFim"];
            string ValorvalorBrutoIni = Request.QueryString["valorBrutoIni"];
            string ValorvalorBrutoFim = Request.QueryString["valorBrutoFim"];
            string Valortipodocumento_id = Request.QueryString["tipodocumento_id"];
            string ValornumeroDocumento = Request.QueryString["numeroDocumento"];
            string ValordataDocumentoIni = Request.QueryString["dataDocumentoIni"];
            string ValordataDocumentoFim = Request.QueryString["dataDocumentoFim"];
            string ValorsituacaoPagamento = Request.QueryString["situacaoPagamento"];
            string ValorcodigoPagamento = Request.QueryString["codigoPagamento"];
            string ValorlotePagamentoBanco = Request.QueryString["lotePagamentoBanco"];
            string Valordocumentopagaraprovacao_id = Request.QueryString["documentopagaraprovacao_id"];

            int totalRecords = 0;
            DocumentoPagarMestre obj = new DocumentoPagarMestre();
            var objs = new DocumentoPagarMestre().ObterTodos(_paramBase);

            if (!String.IsNullOrEmpty(Valorpessoa_id))
            {
                int aux;
                int.TryParse(Valorpessoa_id, out aux);
                objs = objs.Where(p => p.pessoa_id == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valorbanco))
            {
                int aux;
                int.TryParse(Valorbanco, out aux);
                objs = objs.Where(p => p.banco_id == aux).ToList();
            }

            if (!String.IsNullOrEmpty(ValordataLancamentoIni))
            {
                DateTime aux;
                DateTime.TryParse(ValordataLancamentoIni, out aux);
                objs = objs.Where(p => p.dataLancamento >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataLancamentoFim))
            {
                DateTime aux;
                DateTime.TryParse(ValordataLancamentoFim, out aux);
                objs = objs.Where(p => p.dataLancamento <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataCompetencia))
            {
                objs = objs.Where(p => p.dataCompetencia == ValordataCompetencia).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataVencimentoIni))
            {
                DateTime aux;
                DateTime.TryParse(ValordataVencimentoIni, out aux);
                objs = objs.Where(p => p.DocumentoPagarParcelas.Where(y => y.vencimento >= aux).Count() > 0).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataVencimentoFim))
            {
                DateTime aux;
                DateTime.TryParse(ValordataVencimentoFim, out aux);
                objs = objs.Where(p => p.DocumentoPagarParcelas.Where(y => y.vencimento <= aux).Count() > 0).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorBrutoIni))
            {
                decimal aux;
                decimal.TryParse(ValorvalorBrutoIni, out aux);
                objs = objs.Where(p => p.valorBruto >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorvalorBrutoFim))
            {
                decimal aux;
                decimal.TryParse(ValorvalorBrutoFim, out aux);
                objs = objs.Where(p => p.valorBruto <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valortipodocumento_id))
            {
                int aux;
                int.TryParse(Valortipodocumento_id, out aux);
                objs = objs.Where(p => p.tipodocumento_id == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValornumeroDocumento))
            {
                int aux;
                int.TryParse(ValornumeroDocumento, out aux);
                objs = objs.Where(p => p.numeroDocumento == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataDocumentoIni))
            {
                DateTime aux;
                DateTime.TryParse(ValordataDocumentoIni, out aux);
                objs = objs.Where(p => p.dataDocumento >= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataDocumentoFim))
            {
                DateTime aux;
                DateTime.TryParse(ValordataDocumentoFim, out aux);
                objs = objs.Where(p => p.dataDocumento <= aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorsituacaoPagamento))
            {
                objs = objs.Where(p => p.situacaoPagamento.Contains(ValorsituacaoPagamento)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorcodigoPagamento))
            {
                int aux;
                int.TryParse(ValorcodigoPagamento, out aux);
                objs = objs.Where(p => p.codigoPagamento == aux).ToList();
            }
            //if (!String.IsNullOrEmpty(ValorlotePagamentoBanco))
            //{
            //    objs = objs.Where(p => p.lotePagamentoBanco.Contains(ValorlotePagamentoBanco)).ToList();
            //}


            totalRecords = objs.Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                string Aprovado = "";
                //if (item.usuarioAutorizador_id == null)
                //    Aprovado = "Não";
                //else
                //    Aprovado = "Sim";

                string auxsituacao = "1 - Em Aberto";

                switch (item.StatusPagamento)
                {
                    case DocumentoPagarMestre.DOCEMABERTO:
                        break;
                    case DocumentoPagarMestre.DOCPAGOTOTAL:
                        auxsituacao = "3 - Pago Total";
                        break;
                    case DocumentoPagarMestre.DOCPAGOPARC:
                        auxsituacao = "2 - Pago Parcialmente";
                        break;
                    default:
                        break;
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Banco.nomeBanco + " - " + item.Banco.agencia + " - " + item.Banco.contaCorrente,
                    item.Pessoa.nome,
                    item.tipoDocumento.descricao,
                    item.dataLancamento.ToShortDateString(),
                    //item.dataVencimento.ToShortDateString(),
                    item.PlanoDeConta.descricao,
                    item.valorBruto,
                    item.dataDocumento.ToShortDateString(),
                    item.numeroDocumento.ToString(),
                    auxsituacao,
                    Aprovado

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public JsonResult Arquivos(int id)
        {
            return Json(new DocumentoPagarArquivo().ObterPorCPAG(id, _paramBase).Select(p => new { p.arquivoOriginal, p.arquivoReal, p.descricao, p.id }));
        }
        [HttpPost]
        public JsonResult Upload(int id, string descricao, FormCollection formCollection)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".xlxs", ".xlx", ".pdf", ".txt", ".jpeg", ".jpg", ".png" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, extenção não permitida" });

                    }
                    else
                    {
                        var documentoPagarArquivo = new DocumentoPagarArquivo().ObterPorCPAGArquivo(id, arquivo.FileName, _paramBase);
                        if (documentoPagarArquivo.Count() >= 1)
                        {
                            return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, arquivo já gravado" });
                        }
                    }
                }
            }

            var pessoa = new Pessoa();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo = arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);

                    AzureStorage.UploadFile(caminhoArquivo,
                                "CPAG/" + id.ToString() + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DbControle();

                    var documentoPagarArquivo = new DocumentoPagarArquivo();



                    documentoPagarArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "CPAG/" + id.ToString() + "/" + nomearquivonovo;
                    documentoPagarArquivo.arquivoOriginal = nomearquivonovo;
                    documentoPagarArquivo.descricao = descricao;
                    documentoPagarArquivo.documentoPagarMestre_id = id;

                    documentoPagarArquivo.Salvar(_paramBase);

                    new DocumentoPagarMestre().AtualizaQtdArquivos(id, _paramBase, db);


                }
            }

            return Json(new { CDStatus = "OK", DSMessage = "Arquivo salvo com suceso" });
        }
        public enum TipoDespesas
        {
            DARF,
            GPS,
            FGTS,
            AGUA,
            LUZ,
            NAOIDENTIFICADO
        }


        public static int DigitoM10(long intNumero)
        {
            int[] intPesos = { 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 };
            string strText = intNumero.ToString();

            if (strText.Length > 16)
                throw new Exception("Número não suportado pela função!");

            int intSoma = 0;
            int intIdx = 0;
            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
            }

            intSoma = intSoma % 10;
            intSoma = 10 - intSoma;
            if (intSoma == 10)
            {
                intSoma = 0;
            }

            return intSoma;
        }

        public static int DigitoM11(string intNumero)
        {
            return DigitoM11(long.Parse(intNumero));
        }
        /// <summary>
        ///  Calculo de digito Modulo 11
        /// </summary>
        /// <param name="intNumero">Informar o numero para calculo digito</param>
        /// <returns>Retorna o digito</returns>
        public static int DigitoM11(long intNumero)
        {
            int[] intPesos = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5, 6, 7, 8, 9 };
            string strText = intNumero.ToString();

            if (strText.Length > 16)
                throw new Exception("Número não suportado pela função!");

            int intSoma = 0;
            int intIdx = 0;
            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
            }
            int intResto = (intSoma * 10) % 11;
            int intDigito = intResto;
            if (intDigito >= 10)
                intDigito = 0;

            return intDigito;
        }

        [HttpPost]
        public async Task<JsonResult> UploadLacto(string descricao, FormCollection formCollection)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".jpeg", ".jpg", ".png", ".pdf" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, extenção não permitida" });

                    }
                }
            }

            var pessoa = new Pessoa();
            var retorno = new List<string>(); ;
            TipoDespesas tipoConta = TipoDespesas.NAOIDENTIFICADO;
            var darf = new DARF();
            var gps = new GPS();
            var fgts = new FGTS();
            var nomearquivo = "";
            var codigoBarras = "";
            var descricaoExtra = "";

            DocumentoPagarMestre obj = new DocumentoPagarMestre();
            obj.dataLancamento = DateTime.Now;
            obj.dataDocumento = DateTime.Now;
            var dataVencimento = DateTime.Now;
            var dataVencimentoOriginal = DateTime.Now;
            obj.dataCompetencia = DateTime.Now.ToString("MM/yyyy");
            obj.estabelecimento_id = _estab;
            obj.tipolancamento = "P";
            obj.LinhaDigitavel = codigoBarras;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var MGCS = new SoftFin.Cognitive.MicrosoftCognitiveCaptionService();

                    nomearquivo = "templact" + Guid.NewGuid().ToString() + "_" + arquivo.FileName;
                    nomearquivo = nomearquivo.ToLower();
                    var caminhoarquivo = SalvaTemp(arquivo, nomearquivo);

                    var codigoBarrasAux = Spire.Barcode.BarcodeScanner.Scan(caminhoarquivo.Replace(".pdf", ".png"), Spire.Barcode.BarCodeType.Interleaved25);

                    if (codigoBarrasAux.Count() != 0)
                        codigoBarras = codigoBarrasAux[0];

                    if (nomearquivo.ToLower().Contains(".pdf"))
                    {
                        var f = System.IO.File.Open(caminhoarquivo.Replace(".pdf", ".png"), FileMode.Open, FileAccess.Read);
                        retorno = await MGCS.GetTextAsyncToList(f);
                        f.Close();
                    }
                    else
                    {
                        retorno = await MGCS.GetTextAsyncToList(arquivo.InputStream);

                    }

                    if (retorno.Count() > 0)
                    {



                        if ((retorno.Where(p => p.ToUpper().Contains("DAMSP")).Count() >= 1))
                        {
                            tipoConta = TipoDespesas.NAOIDENTIFICADO;
                        }
                        else if ((retorno.Where(p => p.ToUpper().Contains("FGTS")).Count() >= 1))
                        {
                            tipoConta = TipoDespesas.FGTS;
                        }
                        else if ((retorno.Where(p => p.ToUpper().Contains("GPS")).Count() >= 1))
                        {
                            tipoConta = TipoDespesas.GPS;
                        }
                        else if ((retorno.Where(p => p.ToUpper().Contains("DARF")).Count() >= 1))
                        {
                            tipoConta = TipoDespesas.DARF;
                        }
                        else if ((retorno.Where(p => p.ToUpper().Contains("ARRECADAÇÃO")).Count() >= 1) &&
                            (retorno.Where(p => p.ToUpper().Contains("RECEITA")).Count() >= 1))
                        {
                            tipoConta = TipoDespesas.DARF;
                        } else if ((retorno.Where(p => p.ToUpper().Contains("SIMPLES")).Count() >= 1) &&
                             (retorno.Where(p => p.ToUpper().Contains("NACIONAL")).Count() >= 1))
                        {
                            tipoConta = TipoDespesas.NAOIDENTIFICADO;
                        }


                        if (tipoConta == TipoDespesas.DARF)
                        {
                            //Recorte
                            CortaImagem(caminhoarquivo);

                            var f = System.IO.File.Open(caminhoarquivo
                                                            .Replace(".png", "_corte1.png")
                                                            .Replace(".pdf", "_corte1.png"), FileMode.Open, FileAccess.Read);

                            retorno = await MGCS.GetTextAsyncToList(f);
                            f.Close();



                            PreenchDARF(retorno, darf);

                            obj.dataDocumento = darf.dataVencimento;
                            dataVencimento = darf.dataVencimento;
                            dataVencimentoOriginal = darf.dataVencimento;
                            obj.dataCompetencia = darf.dataVencimento.ToString("MM/yyyy");
                            obj.valorBruto = darf.valorTotal;
                            descricaoExtra = "DARF";

                        }
                        else if (tipoConta == TipoDespesas.GPS)
                        {
                            CortaImagem(caminhoarquivo);

                            var f = System.IO.File.Open(caminhoarquivo
                                                            .Replace(".png", "_corte1.png")
                                                            .Replace(".pdf", "_corte1.png"), FileMode.Open, FileAccess.Read);

                            var retorno2 = await MGCS.GetTextAsyncToList(f);
                            f.Close();

                            PreenchGPS(retorno, retorno2, gps);

                            obj.dataDocumento = gps.dataArrecadacao;
                            dataVencimento = gps.dataArrecadacao;
                            dataVencimentoOriginal = gps.dataArrecadacao;
                            obj.dataCompetencia = gps.competencia;
                            obj.valorBruto = gps.valorTributo;

                            descricaoExtra = "GPS";
                        }
                        else if (tipoConta == TipoDespesas.FGTS)
                        {
                            PreenchFGTS(retorno, fgts);
                            descricaoExtra = "FGTS";
                        }

                    }
                }
            }


            string linhaFormatadaAux = "";
            DateTime? vencimentoAux = null;
            decimal? valorAux = null;
            string extra = null;


            CodigoBarrasInterno(codigoBarras, ref linhaFormatadaAux, vencimentoAux, ref valorAux, ref extra);

            if (!string.IsNullOrEmpty(linhaFormatadaAux))
            {
                obj.LinhaDigitavel = linhaFormatadaAux;
            }

            if (vencimentoAux != null)
            {
                dataVencimento = vencimentoAux.Value;
                dataVencimentoOriginal = vencimentoAux.Value;
                //dataCompetencia = vencimentoAux.Value.ToString("MM/yyyy");
            }

            if (valorAux != null)
            {
                obj.valorBruto = valorAux.Value;
            }


            var bcaux = new Banco().ObterPrincipal(_paramBase);
            if (bcaux != null)
                obj.banco_id = bcaux.id;


            dynamic darfaux = null;
            dynamic gpsaux = null;
            dynamic fgtsaux = null;
            switch (tipoConta)
            {
                case TipoDespesas.DARF:

                    if (darf.cnpj != null)
                    {
                        var pessoaaux = new Pessoa().ObterPorCNPJ(darf.cnpj, _paramBase);
                        if (pessoaaux != null)
                        {
                            darf.nomeContribuinte = pessoaaux.razao;
                            obj.Pessoa = pessoaaux;
                            obj.pessoa_id = pessoaaux.id;
                        }
                        else
                        {
                            var pessoaestab = new Estabelecimento().ObterTodos(_paramBase).
                                Where(p =>
                                    p.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "") == darf.cnpj.Replace(".", "").Replace("/", "").Replace("-", ""));
                            if (pessoaestab.Count() > 0)
                            {
                                darf.nomeContribuinte = pessoaestab.First().NomeCompleto;
                            }
                        }


                    }
                    dataVencimento = darf.dataVencimento;
                    dataVencimentoOriginal = darf.dataVencimento;


                    obj.tipodocumento_id = 67;
                    darfaux = (new
                    {
                        darf.cnpj,
                        darf.codigoReceita,

                        dataVencimento = darf.dataVencimento.ToString("o"),
                        darf.DocumentoPagarMestre_id,
                        darf.estabelecimento_id,
                        darf.ID,
                        darf.jurosEncargos,
                        darf.multa,
                        darf.nomeContribuinte,
                        darf.numeroReferencia,
                        periodoApuracao = darf.periodoApuracao.ToString("o"),
                        darf.valorPrincipal,
                        darf.valorTotal
                    });
                    break;

                case TipoDespesas.GPS:

                    if (gps.identificador != null)
                    {
                        var pessoaaux = new Pessoa().ObterPorCNPJ(gps.identificador, _paramBase);

                        if (pessoaaux != null)
                        {
                            gps.nomeContribuinte = pessoaaux.razao;
                            obj.Pessoa = pessoaaux;
                            obj.pessoa_id = pessoaaux.id;
                        }
                    }

                    obj.tipodocumento_id = 66;
                    obj.valorBruto = gps.valorTributo;


                    gpsaux = (new
                    {
                        gps.codigoPagamento,
                        gps.competencia,
                        dataArrecadacao = gps.dataArrecadacao.ToString("o"),
                        gps.DocumentoPagarMestre_id,
                        gps.estabelecimento_id,
                        gps.id,
                        gps.identificador,
                        gps.informacoesComplementares,
                        gps.nomeContribuinte,
                        gps.valorArrecadado,
                        gps.valorAtualizacaoMonetaria,
                        gps.valorOutrasEntidades,
                        gps.valorTributo
                    });

                    break;
                case TipoDespesas.FGTS:
                    if (fgts.cnpj != null)
                    {
                        var pessoaaux = new Pessoa().ObterPorCNPJ(fgts.cnpj, _paramBase);

                        if (pessoaaux != null)
                        {
                            fgts.nomeContribuinte = pessoaaux.razao;
                            obj.Pessoa = pessoaaux;
                            obj.pessoa_id = pessoaaux.id;
                        }

                    }

                    fgts.codigoBarras = extra;
                    obj.LinhaDigitavel = extra;
                    obj.tipodocumento_id = 68;
                    fgtsaux = (new
                    {
                        fgts.cnpj,
                        fgts.codigoBarras,
                        fgts.codigoReceita,
                        dataPagamento = fgts.dataPagamento.ToString("o"),
                        fgts.digitoLacre,
                        fgts.DocumentoPagarMestre_id,
                        fgts.estabelecimento_id,
                        fgts.ID,
                        fgts.lacreConectividadeSocial,
                        fgts.nomeContribuinte,
                        fgts.tipoInscricao,
                        fgts.valorPagamento
                    });
                    break;
                case TipoDespesas.AGUA:
                    break;
                case TipoDespesas.LUZ:
                    break;
                case TipoDespesas.NAOIDENTIFICADO:
                    break;
                default:
                    break;
            }


            return Json(new
            {
                CDStatus = "OK",
                DSMessage = "OK",
                DARF = darfaux,
                GPS = gpsaux,
                FGTS = fgtsaux,
                CPAG = new
                {
                    urlimg = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "CPAGTEMP/" + nomearquivo.Replace(".pdf", ".png"),
                    urlNomeimagem = nomearquivo.Replace(".pdf", ".png").Replace(" ", "%"),
                    banco_id = obj.banco_id.ToString(),
                    obj.codigoPagamento,
                    obj.CodigoVerificacao,
                    dataAlteracao = (obj.dataAlteracao == null) ? "" : obj.dataAlteracao.Value.ToString("o"),
                    obj.dataCompetencia,
                    dataDocumento = obj.dataDocumento.ToString("o"),
                    dataInclusao = (obj.dataInclusao == null) ? "" : obj.dataInclusao.Value.ToString("o"),
                    dataLancamento = obj.dataLancamento.ToString("o"),
                    //dataPagamanto = (obj.dataPagamanto == null) ? "" : obj.dataPagamanto.Value.ToString("o"),
                    //dataVencimento = obj.dataVencimento.ToString("o"),
                    //dataVencimentoOriginal = obj.dataVencimentoOriginal.ToString("o"),
                    obj.documentopagaraprovacao_id,
                    obj.estabelecimento_id,
                    obj.id,
                    obj.LinhaDigitavel,
                    //.lotePagamentoBanco,
                    obj.numeroDocumento,
                    obj.pessoa_id,
                    planoDeConta_id = "",
                    obj.situacaoPagamento,
                    obj.StatusPagamento,
                    tipodocumento_id = obj.tipodocumento_id.ToString(),
                    obj.tipolancamento,
                    obj.usuarioalteracaoid,
                    //obj.usuarioAutorizador_id,
                    obj.usuarioinclusaoid,
                    obj.valorBruto,
                    descricaoExtra,
                    pessoa_desc = (obj.pessoa_id == 0) ? "" : obj.Pessoa.nome + ", " + obj.Pessoa.cnpj
                },
                CPAGPARCELA = new
                {
                    id = 0,
                    vencimento = dataVencimentoOriginal.ToString("o"),
                    vencimentoPrevisto = dataVencimento.ToString("o"),
                    parcelas = 1,
                    historico = "",
                    sobra = "U"
                },
                CPAGPARCELAS = (
                    obj.DocumentoPagarParcelas.Select(
                        p => new
                        {
                            p.DocumentoPagarMestre_id,
                            p.historico,
                            p.id,
                            p.lotePagamentoBanco,
                            p.parcela,
                            p.usuarioAutorizador_id,
                            p.valor,
                            vencimento = p.vencimento.ToString("o"),
                            vencimentoPrevisto = p.vencimentoPrevisto.ToString("o")
                        }
                    )
                )

            });

        }

        private static void VerificaDatas(List<string> retorno, List<DateTime> dataValidas)
        {
            var datasaux = retorno.Where(p => p.Contains("/"));
            datasaux = datasaux.Where(p => !p.Contains("-"));
            datasaux = datasaux.Where(p => !p.Contains("."));
            datasaux = datasaux.Where(p => p.ToUpper() == p.ToLower()); //so numeros
            datasaux = datasaux.Where(p => p.Length != 7);
            var data3 = "";
            foreach (var item in datasaux)
            {
                data3 += item;
                var data4 = new DateTime();
                if (data3.Count(p => p == '/') == 2)
                {
                    if (DateTime.TryParse(data3, out data4))
                    {
                        if (data4.Year <= 2000)
                        {
                            dataValidas.Add(new DateTime(DateTime.Now.Year, data4.Month, data4.Day, 0, 0, 0, 0));
                            data3 = "";
                        }
                        else
                        {
                            dataValidas.Add(data4);
                            data3 = "";
                        }
                    }
                    else
                    {
                        data3 = "";
                    }
                }

                if (data3.Count(p => p == '/') > 2)
                {
                    data3 = "";
                }
                if (data3.Length > 10)
                {
                    data3 = "";
                }

            }
        }

        private static void CortaImagem(string caminhoarquivo)
        {
            byte[] photoBytes = System.IO.File.ReadAllBytes(caminhoarquivo.Replace(".pdf", ".png"));


            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory())
                    {
                        var crop = new ImageProcessor.Imaging.CropLayer(
                            73,
                            0,
                            100,
                            100,
                            ImageProcessor.Imaging.CropMode.Percentage
                        )
                        ;

                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Crop(crop)
                                    .Save(outStream);
                    }

                    using (FileStream file = new FileStream(caminhoarquivo
                                                                .Replace(".png", "_corte1.png")
                                                                .Replace(".pdf", "_corte1.png"), FileMode.Create, FileAccess.Write))
                    {
                        outStream.WriteTo(file);
                    }


                    // Do something with the stream.
                }
            }
        }
        private void PreenchFGTS(List<string> retorno, FGTS fgts)
        {
            var cnpj = retorno.Where(p => p.Contains("/")).ToList();
            cnpj = cnpj.Where(p => p.Contains(".")).ToList();
            cnpj = cnpj.Where(p => p.Contains("-")).ToList();
            if (cnpj.Count() > 0)
                fgts.cnpj = cnpj.First().Replace(".", "").Replace("-", "").Replace("/", "");


            var datasaux = retorno.Where(p => p.Contains("/"));
            datasaux = datasaux.Where(p => !p.Contains("-"));

            var dats = new List<DateTime>();
            VerificaDatas(retorno, dats);

            if (dats.Count() > 0)
                fgts.dataPagamento = dats.Last();

            var numerosaux = retorno.Where(p => p.Length == 4).Where(p => !p.Contains(",")).Where(p => !p.Contains("."));
            foreach (var item in numerosaux)
            {
                var numero = 0;
                if (int.TryParse(item, out numero))
                {
                    fgts.codigoReceita = numero;
                    break;
                }
            }

            var numeros = retorno.Where(p => p.Contains(","));


            foreach (var item in numeros)
            {
                decimal numero;
                if (decimal.TryParse(item, out numero))
                {
                    fgts.valorPagamento = numero;

                }
            }
        }


        private string VerificaCNPJ(List<string> retorno)
        {

            string cnpjSecundario = "";

            foreach (var item in retorno)
            {
                string cnpjPrincipal = cnpjSecundario + item;


                if (cnpjPrincipal.Count(p => p == '.') == 2)
                {
                    if (cnpjPrincipal.Count(p => p == '-') == 1)
                    {
                        if (cnpjPrincipal.Count(p => p == '/') == 1)
                        {
                            if (cnpjPrincipal.Length == 18)
                                return cnpjPrincipal;
                        }
                    }
                }

                cnpjSecundario = item;

            }

            return "";
        }

        private void PreenchGPS(List<string> retorno, List<string> retorno2, GPS gps)
        {
            var cnpj = retorno2.Where(p => p.Contains("/")).ToList();
            cnpj = cnpj.Where(p => p.Contains(".")).ToList();
            cnpj = cnpj.Where(p => p.Contains("-")).ToList();
            if (cnpj.Count() > 0)
                gps.identificador = cnpj.First().Replace(".", "").Replace("-", "").Replace("/", "");

            if (gps.identificador == null)
                gps.identificador = VerificaCNPJ(retorno2);




            List<DateTime> datasaux = new List<System.DateTime>();
            VerificaDatas(retorno, datasaux);

            foreach (var item in datasaux)
            {
                gps.dataArrecadacao = datasaux.First();
            }

            var numerosaux = retorno2.Where(p => p.Length == 4).Where(p => !p.Contains(",")).Where(p => !p.Contains("."));
            foreach (var item in numerosaux)
            {
                var numero = 0;
                if (int.TryParse(item, out numero))
                {
                    gps.codigoPagamento = numero;
                    break;
                }
            }

            var numerosaux2 = retorno2.Where(p => p.Length == 7).Where(p => p.Contains("/"));
            foreach (var item in numerosaux2)
            {
                DateTime datateste;
                if (DateTime.TryParse("01/" + item, out datateste))
                {
                    gps.competencia = item;
                    break;
                }
            }

            var numeros = VerificaValores(retorno2);

            var valorArrecadado = true;
            var valorOutrasEntidades = true;
            var valorAtualizacaoMonetaria = true;

            foreach (var item in numeros)
            {



                if (valorArrecadado)
                {
                    gps.valorArrecadado = item;
                    valorArrecadado = false;
                }
                else if (valorAtualizacaoMonetaria)
                {
                    gps.valorAtualizacaoMonetaria = item;
                    valorAtualizacaoMonetaria = false;
                }
                else if (valorOutrasEntidades)
                {
                    gps.valorOutrasEntidades = item;
                    valorOutrasEntidades = false;
                }

                else
                {
                    gps.valorTributo = item;
                    break;
                }
            }
        }

        private static List<decimal> VerificaValores(List<string> retorno2, int quantidadeMaxima = 4)
        {
            var numeros = retorno2.Where(p => p.Contains(","));
            var retorno = new List<decimal>();
            var retornook = new List<decimal>();
            var quantidade = 0;

            foreach (var item in numeros)
            {
                var numero = new decimal();
                if (decimal.TryParse(item, out numero))
                {
                    retorno.Add(numero);
                    quantidade += 1;
                    if (quantidade == quantidadeMaxima)
                        break;
                }
            }

            if (retorno.Count == 4)
            {
                //Sem Multa e Juros
                if (retorno.Where(p => p == 0).Count() == 2)
                {
                    var retornoaux = retorno.Where(p => p != 0).ToArray();

                    if (retornoaux[0] >= retornoaux[1])
                        retornook.Add(retornoaux[0]);
                    retornook.Add(0);
                    retornook.Add(0);

                    if (retornoaux[0] <= retornoaux[1])
                        retornook.Add(retornoaux[1]);

                }
                else
                {
                    //Boleto com 2 vias sem multa e em juros informados (campo vazio)
                    if (retorno.Where(p => p == retorno.First()).Count() == 4)
                    {
                        retornook.Add(retorno.First());
                        retornook.Add(0);
                        retornook.Add(0);
                        retornook.Add(retorno.First());
                    }
                    else
                    {
                        retornook = retorno;
                    }
                }
            }
            else if (retorno.Count == 2)
            {

                //Boleto com 1 vias sem multa e em juros informados (campo vazio)
                if (retorno.Where(p => p == retorno.First()).Count() == 2)
                {
                    retornook.Add(retorno.First());
                    retornook.Add(0);
                    retornook.Add(0);
                    retornook.Add(retorno.First());
                }
                else
                {
                    retornook = retorno;
                }
            }
            else
            {
                retornook = retorno;
            }


            return retornook;
        }


        private static void PreenchDARF(List<string> retorno, DARF darf)
        {

            var cnpj = retorno.Where(p => p.Contains("/")).ToList();
            //cnpj = cnpj.Where(p => p.Contains(".")).ToList();
            cnpj = cnpj.Where(p => p.Contains("-")).ToList();
            if (cnpj.Count() > 0)
                darf.cnpj = cnpj.First().Replace(".", "").Replace("-", "").Replace("/", "");



            List<DateTime> datasaux = new List<System.DateTime>();
            VerificaDatas(retorno, datasaux);


            if (datasaux.Count() == 1)
            {
                var data = new DateTime();
                darf.dataVencimento = data;
            }
            else
            {
                var periodo = true;
                foreach (var item in datasaux)
                {

                    if (periodo)
                    {
                        darf.periodoApuracao = item;
                        periodo = false;
                    }
                    else
                    {
                        darf.dataVencimento = item;
                        break;
                    }
                }
            }



            var numerosaux = retorno.Where(p => p.Length == 4).Where(p => !p.Contains(",")).Where(p => !p.Contains("."));
            foreach (var item in numerosaux)
            {
                var numero = 0;
                if (int.TryParse(item, out numero))
                {
                    darf.codigoReceita = numero;
                    break;
                }
            }

            var numeros = retorno.Where(p => p.Contains(",")).Where(p => !p.Contains("10,00"));

            var valorPrincipal = true;
            var multa = true;
            var jurosEncargos = true;

            foreach (var item in numeros)
            {
                var numero = new decimal();


                if (valorPrincipal)
                {
                    if (decimal.TryParse(item, out numero))
                    {
                        darf.valorPrincipal = numero;
                        valorPrincipal = false;
                    }
                }
                else if (multa)
                {
                    if (decimal.TryParse(item, out numero))
                    {
                        darf.multa = numero;
                        multa = false;
                    }
                }
                else if (jurosEncargos)
                {
                    if (decimal.TryParse(item, out numero))
                    {
                        darf.jurosEncargos = numero;
                        jurosEncargos = false;
                    }
                }
                else
                {
                    if (decimal.TryParse(item, out numero))
                    {
                        darf.valorTotal = numero;
                        break;
                    }
                }
            }

            if (darf.valorTotal == darf.valorPrincipal)
            {
                darf.jurosEncargos = 0;
                darf.multa = 0;
            }
        }
        private string SalvaTemp(HttpPostedFileBase arquivo, string nomearquivo)
        {
            var uploadPath = Server.MapPath("~/TXTTemp/");
            Directory.CreateDirectory(uploadPath);
            nomearquivo = nomearquivo.ToLower();
            string caminhoArquivo = Path.Combine(@uploadPath, nomearquivo);
            arquivo.SaveAs(caminhoArquivo);


            if (nomearquivo.ToLower().Contains(".pdf"))
            {
                PDFToImage(caminhoArquivo, caminhoArquivo.Replace(".pdf", ".png"), 300);
                //PdfReader pdfdocument = new PdfReader(caminhoArquivo);
                //System.Diagnostics.Process process1 = new System.Diagnostics.Process();
                //process1.StartInfo.RedirectStandardOutput = false; 
                //process1.StartInfo.UseShellExecute = false;
                //process1.StartInfo.WorkingDirectory = Request.MapPath("~/");
                //process1.StartInfo.FileName = Request.MapPath("ClientBin/gswin64.exe").Replace("\\CPAG","");
                //process1.StartInfo.Arguments = "-dSAFER -dBATCH -dNOPAUSE -sDEVICE=png16m -r150 -dTextAlphaBits=4 -dGraphicsAlphaBits=4 -dMaxStripSize=8192 -sOutputFile=\"" + caminhoArquivo.Replace(".pdf", ".png") + "\" \"" + caminhoArquivo + "\"";
                //process1.Start();
                //process1.WaitForExit(2000);


                AzureStorage.UploadFile(caminhoArquivo.Replace(".pdf", ".png"),
                    "CPAGTEMP/" + nomearquivo.Replace(".pdf", ".png"),
                    ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());
            }


            AzureStorage.UploadFile(caminhoArquivo,
                        "CPAGTEMP/" + nomearquivo,
                        ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());


            return caminhoArquivo;
        }
        public void PDFToImage(string file, string outputPath, int dpi)
        {
            string path = Request.MapPath("ClientBin").Replace("\\CPAG", ""); //Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Ghostscript.NET.Rasterizer.GhostscriptRasterizer rasterizer = null;
            Ghostscript.NET.GhostscriptVersionInfo vesion = new Ghostscript.NET.GhostscriptVersionInfo(new Version(0, 0, 0), path + @"\gsdll32.dll", string.Empty, Ghostscript.NET.GhostscriptLicense.GPL);

            using (rasterizer = new Ghostscript.NET.Rasterizer.GhostscriptRasterizer())
            {
                rasterizer.Open(file, vesion, false);

                string pageFilePath = outputPath;

                Image img = rasterizer.GetPage(dpi, dpi, 1);
                img.Save(pageFilePath, System.Drawing.Imaging.ImageFormat.Png);


                rasterizer.Close();
            }
        }
        public JsonResult RemoveArquivo(int id)
        {
            try
            {
                var documentoPagarArquivo = new DocumentoPagarArquivo().ObterPorId(id, _paramBase);


                AzureStorage.DeleteFile(documentoPagarArquivo.arquivoReal,
                                    "CPAG/" + documentoPagarArquivo.documentoPagarMestre_id.ToString() + "/" + documentoPagarArquivo.arquivoOriginal,
                                    ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                new DocumentoPagarArquivo().Excluir(id, _paramBase);
                new DocumentoPagarMestre().AtualizaQtdArquivos(id, _paramBase, null);

                return Json(new { CDStatus = "OK", DSMessage = "Excluido com sucesso" });
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message });
            }

        }
        //private DocumentoPagarMestreVlw2 CarregaProrrocacaoBaixaManual(int ID)
        //{
        //    DocumentoPagarMestreVlw2 obj = new DocumentoPagarMestreVlw2();
        //    CarregaViewData();

        //    var documento = new DocumentoPagarMestre().ObterPorId(ID);

        //    obj.banco_id = documento.banco_id;
        //    obj.codigoPagamento = documento.codigoPagamento;
        //    obj.dataCompetencia = documento.dataCompetencia;
        //    obj.dataDocumento = documento.dataDocumento;
        //    obj.dataLancamento = documento.dataLancamento;
        //    obj.dataPagamanto = documento.dataPagamanto;
        //    obj.dataVencimentoOriginal = documento.dataVencimentoOriginal;
        //    obj.dataVencimento = documento.dataVencimento;
        //    obj.documentopagaraprovacao_id = documento.documentopagaraprovacao_id;
        //    obj.estabelecimento_id = documento.estabelecimento_id;
        //    obj.lotePagamentoBanco = documento.lotePagamentoBanco;
        //    obj.numeroDocumento = documento.numeroDocumento;
        //    obj.pessoa_id = documento.pessoa_id;
        //    obj.situacaoPagamento = documento.situacaoPagamento;
        //    obj.tipodocumento_id = documento.tipodocumento_id;
        //    obj.tipolancamento = documento.tipolancamento;
        //    obj.valorBruto = documento.valorBruto;
        //    obj.linhadigitavel = documento.LinhaDigitavel;

        //    ViewData["DocumentoPagarDetalhe"] = new DocumentoPagarDetalhe().ObterPorCPAG(documento.id);
        //    ViewData["SemBarras"] = "S";

        //    var tipoDoc = new TipoDocumento().ObterPorId(documento.tipodocumento_id).codigo;

        //    if (tipoDoc == "GPS")
        //    {
        //        var gps = new GPS().ObterPorCPAG(documento.id);
        //        if (gps != null)
        //        {
        //            obj.GPScodigoPagamento = gps.codigoPagamento;
        //            obj.GPScompetencia = gps.competencia;
        //            obj.GPSidentificador = gps.identificador;
        //            obj.GPSinformacoesComplementares = gps.informacoesComplementares;
        //            obj.GPSnomeContribuinte = gps.nomeContribuinte;
        //            obj.GPSvalorArrecadado = gps.valorArrecadado;
        //            obj.GPSvalorAtualizacaoMonetaria = gps.valorAtualizacaoMonetaria;
        //            obj.GPSvalorOutrasEntidades = gps.valorOutrasEntidades;
        //            obj.GPSvalorTributo = gps.valorTributo;
        //            obj.GPSdataArrecadacao = gps.dataArrecadacao;
        //        }
        //    }


        //    if (tipoDoc == "DARF")
        //    {
        //        var darf = new DARF().ObterPorCPAG(documento.id);

        //        if (darf != null)
        //        {
        //            obj.DARFcnpj = darf.cnpj;
        //            obj.DARFcodigoReceita = darf.codigoReceita;
        //            obj.DARFdataPagamento = darf.DocumentoPagarMestre.dataPagamanto;
        //            obj.DARFdataVencimento = darf.dataVencimento;
        //            obj.estabelecimento_id = darf.estabelecimento_id;
        //            obj.DARFjurosEncargos = darf.jurosEncargos;
        //            obj.DARFmulta = darf.multa;
        //            obj.DARFnomeContribuinte = darf.nomeContribuinte;
        //            obj.DARFnumeroReferencia = darf.numeroReferencia;
        //            obj.DARFperiodoApuracao = darf.periodoApuracao;
        //            obj.DARFvalorPrincipal = darf.valorPrincipal;
        //            obj.DARFvalorTotal = darf.valorTotal;
        //        }
        //    }

        //    if (tipoDoc == "FGTS")
        //    {

        //        var fgts = new FGTS().ObterPorCPAG(documento.id);

        //        if (fgts != null)
        //        {
        //            obj.FGTScnpj = fgts.cnpj;
        //            obj.FGTScodigoBarras = fgts.codigoBarras;
        //            obj.FGTScodigoReceita = fgts.codigoReceita;
        //            obj.FGTSdataPagamento = fgts.dataPagamento;
        //            obj.FGTSdigitoLacre = fgts.digitoLacre;
        //            obj.estabelecimento_id = fgts.estabelecimento_id;
        //            obj.FGTSidentificadorFgts = fgts.identificadorFgts;
        //            obj.FGTSnomeContribuinte = fgts.nomeContribuinte;
        //            obj.FGTStipoInscricao = fgts.tipoInscricao;
        //            obj.FGTSvalorPagamento = fgts.valorPagamento;
        //        }
        //    }
        //    return obj;
        //}
        //public ActionResult BaixaManual(int ID)
        //{
        //    try
        //    {
        //        DocumentoPagarMestreVlw2 obj = CarregaProrrocacaoBaixaManual(ID);

        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}
        //[HttpPost]
        //public ActionResult BaixaManual(int id, DateTime dataPagamanto)
        //{
        //    try
        //    {
        //        DbControle banco = new DbControle();

        //        using (var dbcxtransaction = banco.Database.BeginTransaction())
        //        {
        //            var documento = new DocumentoPagarMestre().ObterPorId(id, banco);
        //            documento.dataPagamanto = dataPagamanto;

        //            documento.Alterar(documento, null, banco);

        //            dbcxtransaction.Commit();
        //        }

        //        ViewBag.msg = "Documento alterado com sucesso "
        //            + id.ToString() + " - "
        //            + dataPagamanto.ToShortDateString();
        //        //return View();
        //        return Prorrogar(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}
        //public ActionResult Prorrogar(int ID)
        //{
        //    try
        //    {
        //        DocumentoPagarMestreVlw2 obj = CarregaProrrocacaoBaixaManual(ID);
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventos.Error(ex);
        //        return RedirectToAction("/Index", "Erros");
        //    }
        //}
        public ActionResult Autorizacao()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
            return View();
        }
        [HttpPost]
        public ActionResult Autorizacao(string dataInicial, string dataFinal)
        {
            @ViewData["DataInicial"] = dataInicial;
            @ViewData["DataFinal"] = dataFinal;
            PerfilPagarAprovacao PF = new PerfilPagarAprovacao();

            PF = new PerfilPagarAprovacao().ObterTodos(_paramBase).Where(p => p.usuarioAutorizador.codigo == Acesso.UsuarioLogado()).FirstOrDefault();

            if (PF == null)
            {
                ViewBag.msg = "Usuário não tem acesso a autorização.";
                return View();
            }


            List<int> cpgs = new List<int>();
            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 3) == "cpg")
                {
                    cpgs.Add(int.Parse(item.ToString().Substring(3)));
                }

            }

            int idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
            int estab = _paramBase.estab_id;
            string msgextra = "";
            var db = new DbControle();

            foreach (var item in cpgs)
            {
                var ov = db.DocumentoPagarParcela.Where(x => x.id == item && x.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();

                if (ov != null)
                {
                    if (ov.valor <= decimal.Parse(PF.valorLimiteCPAG.ToString()))
                    {
                        ov.usuarioAutorizador_id = idusuario;
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
        public JsonResult ConsultaAutorizacao(string dataInicial, string dataFinal)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);


            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal).AddDays(1);
            DataFinal = new DateTime(DataFinal.Year, DataFinal.Month, DataFinal.Day, 23, 59, 00);

            var cpgs = new DocumentoPagarParcela().ObterTodosNaoAutorizado(DataInicial, DataFinal, _paramBase);
            List<DocumentoPagarMestreAutorizacao> rets = new List<DocumentoPagarMestreAutorizacao>();

            foreach (var item in cpgs)
            {
                string auxbanco = "";
                if (item.DocumentoPagarMestre.Banco != null)
                    auxbanco = item.DocumentoPagarMestre.Banco.nomeBanco;

                rets.Add(new DocumentoPagarMestreAutorizacao()
                {
                    banco = auxbanco,
                    dataCompetencia = item.DocumentoPagarMestre.dataCompetencia.ToString(),
                    dataLancamento = item.DocumentoPagarMestre.dataLancamento.ToShortDateString(),
                    dataVencimento = item.vencimento.ToShortDateString(),
                    id = item.id,
                    pessoa = item.DocumentoPagarMestre.Pessoa.nome,
                    tipodocumento = item.DocumentoPagarMestre.tipoDocumento.descricao,
                    valorBruto = item.valor.ToString("n"),
                    numerodocumento = item.DocumentoPagarMestre.numeroDocumento.ToString()
                });
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

            return Json(rets, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SalvaVenctos(List<CPAGItemVenctos> objs)
        {
            bool erro = false;
            foreach (var item in objs.Where(p => p.statusPagamento == 1))
            {
                if (item.vencimentoPrevisto == null)
                {
                    item.erro = "Data Vencimento Previsto não pode ser vazio";
                    erro = true;
                    break;
                }

                if (item.vencimentoPrevisto < SoftFin.Utils.UtilSoftFin.DateBrasilia())
                {
                    item.erro = "Data Vencimento Previsto não pode ser menor do dia atual";
                    erro = true;
                    break;
                }

            }

            if (erro)
            {
                return Json(new
                {
                    CDStatus = "NOK",
                    objs = objs.Select(p => new
                    {
                        p.id,
                        p.desc,
                        p.status,
                        vencimento = p.vencimento.ToString("o"),
                        vencimentoPrevisto = p.vencimentoPrevisto.ToString("o"),
                        p.statusPagamento,
                        p.erro,
                        p.valor
                    })
                }
            , JsonRequestBehavior.AllowGet);
            }

            foreach (var item in objs.Where(p => p.statusPagamento == 1))
            {
                var db =new  DbControle();
                var vencimentoobj = new DocumentoPagarParcela().ObterPorId(item.id, db,_paramBase);
                vencimentoobj.vencimentoPrevisto = item.vencimentoPrevisto;
                vencimentoobj.Alterar(vencimentoobj, db, _paramBase);
                var itens = new BancoMovimento().ObterPorCPAG(vencimentoobj.id, _paramBase, db);
                foreach (var itemBC in itens)
                {
                    if (itemBC.pagamento_id == null)
                    {
                        itemBC.data = item.vencimentoPrevisto;
                    }
                    itemBC.Alterar(_paramBase,db);
                }
            }
            return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObtemVenctos(int idDocumento)
        {
            var objs = new List<CPAGItemVenctos>();
            var doctos = new DocumentoPagarParcela().ObterPorCapa(idDocumento, _paramBase);

            foreach (var item in doctos)
            {
                var obj = new CPAGItemVenctos();
                obj.id = item.id;
                obj.desc = item.DocumentoPagarMestre.numeroDocumento.ToString();
                obj.vencimento = item.vencimento;
                obj.vencimentoPrevisto = item.vencimentoPrevisto;
                obj.status = ClassificaSituacao(item.statusPagamento);
                obj.statusPagamento = item.statusPagamento;
                obj.valor = item.valor;
                objs.Add(obj);
                  
            }


            return Json(new
            {
                objs = objs.Select(p => new
                {
                    p.id,
                    p.desc,
                    p.status,
                    vencimento = p.vencimento.ToString("o"),
                    vencimentoPrevisto = p.vencimentoPrevisto.ToString("o"),
                    p.statusPagamento,
                    p.valor
                })
            }
            , JsonRequestBehavior.AllowGet);
        }

        public class CPAGItemVenctos
        {
            public int id { get; set; }
            public DateTime vencimento { get; set; }
            public DateTime vencimentoPrevisto { get; set; }
            public decimal valor { get; set; }
            public string desc { get; set; }
            public string erro { get; set; }
            public int? statusPagamento { get; set; }
            public string status { get; set; }
        }

    }
}
