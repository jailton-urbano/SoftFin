using OFX;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ConciliacaoVLW
    {
        public List<LanctoExtratoVLW> LanctoExtratos { get; set; }
        public List<BancoMovimentoConciliacaoVLW3> BancoMovimentoConciliacaoVLWs { get; set; }
        public List<Conciliado> Conciliados { get; set; }
        public List<Conciliado> ConciliadosAuto { get; set; }
        public List<ConsultaConciliado> ConsultaConciliados { get; set; }
        public int ExtratoPaginaAtual { get; set; }
        public int ExtratoPaginaQtd { get; set; }
        public int ExtratoTotalRegistros { get; set; }
        public int BancoPaginaAtual { get; set; }
        public int BancoPaginaQtd { get; set; }
        public int BancoTotalRegistros { get; set; }
    }

    public class ConciliacaoVLWTotal
    {
        public string ExtratoTotal { get; set; }
        public string BancoTotal { get; set; }
    }

    public class Conciliado
    {
        public int id { get; set; }
        public int idCC { get; set; }
        public int idCPAG { get; set; }
        public int idNFES { get; set; }

        public string descricaoOFX { get; set; }
        public string valorOFX { get; set; }
        public string descricaoCC { get; set; }
        public string valorCC { get; set; }

        public List<ConciliadoDet> ConciliadoDet { get; set; }
        public List<ConciliadoDet> ConciliadoCC { get; set; }

        public bool selecionado { get; set; }
    }

    public class ConciliadoDet
    {
        public string descricao { get; set; }
    }


    public class ConsultaConciliado
    {
        public string lancamentos { get; set; }
        public string totnaoconc { get; set; }
        public string data { get; set; }
        public string porc { get; set; }
    }
    public class ConciliacaoController : BaseController
    {
        #region Metodos Antigos
        //
        // GET: /Conciliacao/
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new LanctoExtrato().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(p => p.Valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Create()
        {
            ViewData["banco"] = new Banco().CarregaBancoGeral(_paramBase);
            return View();
        }

        public ActionResult Create2()
        {
            ViewData["planoDeConta"] = new PlanoDeConta().ObterTodosTipoA();
            ViewData["tipoDeDocumento"] = new SelectList(new TipoDocumento().ObterTodos(), "id", "descricao");
            ViewData["unidadeDeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "Unidade");
            ViewData["banco"] = new Banco().CarregaBancoGeral(_paramBase);
            return View();
        }




        [HttpPost]
        public JsonResult Pesquisa(string txtBanco, string txtData)
        {

            ExcluirTodosUsuario(_paramBase);
            ConciliacaoVLW retorno = GeraPesquisa(txtBanco, txtData);


            return Json(retorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Pesquisa2(string txtBanco, string txtData)
        {
            ConciliacaoVLW retorno = GeraPesquisa(txtBanco, txtData);


            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        private ConciliacaoVLW GeraPesquisa(string txtBanco, string txtData)
        {
            ConciliacaoVLW retorno = new ConciliacaoVLW();
            int banco = int.Parse(txtBanco);
            DateTime data = DateTime.Parse(txtData);

            var OFXs = new LanctoExtrato().ObterTodos(banco, data);

            List<LanctoExtratoVLW> OFXVLWS = new List<LanctoExtratoVLW>();

            ConvertLanctoExtratoVW(OFXs, OFXVLWS);


            retorno.LanctoExtratos = OFXVLWS;
            retorno.BancoMovimentoConciliacaoVLWs = new BancoMovimento().ObterAgrupado(banco, data, _paramBase);
            retorno.Conciliados = MontaConciliados(banco, data);
            retorno.ConciliadosAuto = MontaConciliadosAuto(banco, data);
            retorno.ConsultaConciliados = MontaConsultaConciliadosAuto(banco, data);

            CalculaTotais(retorno);

            return retorno;
        }

        private void SalvaConciliadosAuto(int banco, DateTime data, List<ConsultaConciliado> objs)
        {

            return;
        }

        private List<ConsultaConciliado> MontaConsultaConciliadosAuto(int banco, DateTime data)
        {
            var rets = new List<ConsultaConciliado>();
            var datfin = data.AddDays(21);
            for (var datini = data.AddDays(-21); datini < datfin; datini = datini.AddDays(1))
            {
                var item = new ConsultaConciliado();
                var OFXs = new LanctoExtrato().ObterTodos(banco, datini);
                item.data = datini.ToShortDateString();
                Double lancto = OFXs.Count();
                if (lancto == 0)
                    continue;

                Double naoconc = OFXs.Where(p => p.UsuConciliado != null).Count();
                item.lancamentos = lancto.ToString();
                item.totnaoconc = naoconc.ToString();


                item.porc = "0";

                if (lancto != 0)
                {
                    if (naoconc != 0)
                    {
                        Double calculo = (naoconc / lancto) * 100;
                        item.porc = Math.Round(calculo).ToString("").Replace(",", ".");
                    }
                }
                rets.Add(item);
            }
            return rets.ToList();
        }



        private List<Conciliado> MontaConciliadosAuto(int banco, DateTime data)
        {
            var rets = new List<Conciliado>();
            var OFXs = new LanctoExtrato().ObterTodos(banco, data);


            var bmsaux = new BancoMovimento().ObterAgrupado(banco, data, _paramBase);


            foreach (var item in OFXs)
            {
                var conciliado = new BancoMovimentoLanctoExtrato().ObterLanctoExtrato_id(item.id).FirstOrDefault();
                if (conciliado == null)
                {
                    var valoraux = item.Valor.ToString("n");
                    var bmaux2 = bmsaux.Where(p => p.Valor == valoraux);

                    if (bmaux2.Count() != 0)
                    {
                        BancoMovimentoConciliacaoVLW3 BMs = new BancoMovimentoConciliacaoVLW3();
                        BMs = bmaux2.FirstOrDefault();

                        if (bmaux2.Where(p => p.ContaBancaria != null).Count() > 1)
                        {
                            foreach (var item2 in bmaux2.Where(p => p.ContaBancaria != null))
                            {
                                if (item2.Descricao.Contains(item2.ContaBancaria))
                                {
                                    BMs = item2;
                                }
                            }
                        }


                        if (BMs != null)
                        {
                            var obj = new Conciliado();
                            obj.descricaoOFX = item.descricao;
                            obj.valorOFX = item.Valor.ToString("n");
                            obj.id = item.id;

                            obj.descricaoCC = BMs.Descricao;
                            obj.valorCC = BMs.Valor;

                            if (BMs.notafiscal_id != null)
                                obj.idNFES = BMs.notafiscal_id.Value;

                            if (BMs.DocumentoPagarMestre_id != null)
                                obj.idCPAG = BMs.DocumentoPagarMestre_id.Value;

                            if (BMs.notafiscal_id == null && BMs.DocumentoPagarMestre_id == null)
                                obj.idCC = int.Parse(BMs.id);

                            rets.Add(obj);
                        }
                    }
                }
            }
            return rets;

        }


        [HttpPost]
        public JsonResult ConciliaBM(int planoDeConta_id, int tipoDeDocumento_id, int unidadeDeNegocio_id, string historico)
        {

            SalvaConcBM(planoDeConta_id, tipoDeDocumento_id, unidadeDeNegocio_id, historico);
            return Json("OK", JsonRequestBehavior.AllowGet);

        }



        private List<Conciliado> MontaConciliados(int banco, DateTime data)
        {
            var rets = new List<Conciliado>();

            var objs = new BancoMovimentoLanctoExtratoMestre().ObterTodos(banco, data);

            var verCC = new List<int>();
            var verOFX = new List<int>();

            foreach (var item in objs)
            {
                var conciliado = new Conciliado();
                conciliado.id = item.id;
                var objs2 = new BancoMovimentoLanctoExtrato().ObterBancoMovimentoLanctoExtratoMestre_id(item.id,null);
                decimal valorAcumuladoOFX = 0;
                decimal valorAcumuladoBM = 0;
                conciliado.ConciliadoDet = new List<ConciliadoDet>();
                conciliado.ConciliadoCC = new List<ConciliadoDet>();

                foreach (var item2 in objs2)
                {
                    if (verOFX.Where(p => p == item2.LanctoExtrato_id).Count() == 0)
                    {
                        verOFX.Add(item2.LanctoExtrato_id);
                        conciliado.descricaoOFX += item2.LanctoExtrato.descricao + " Valor " + item2.LanctoExtrato.Valor.ToString("n");
                        conciliado.ConciliadoDet.Add(new ConciliadoDet { descricao = item2.LanctoExtrato.descricao + " Valor " + item2.LanctoExtrato.Valor.ToString("n") });
                        valorAcumuladoOFX += item2.LanctoExtrato.Valor;

                    }
                    if (verCC.Where(p => p == item2.BancoMovimento_id).Count() == 0)
                    {
                        verCC.Add(item2.BancoMovimento_id);

                        if (item2.BancoMovimento.TipoMovimento.id == item2.BancoMovimento.TipoMovimento.TipoSaida(_paramBase))
                        {
                            conciliado.descricaoCC += item2.BancoMovimento.data.ToShortDateString() + "-" + item2.BancoMovimento.historico + " Valor -" + item2.BancoMovimento.valor.ToString("n");
                            conciliado.ConciliadoCC.Add(new ConciliadoDet { descricao = item2.BancoMovimento.data.ToShortDateString() + "-" + item2.BancoMovimento.historico + " Valor -" + item2.BancoMovimento.valor.ToString("n") });
                            valorAcumuladoBM += -(item2.BancoMovimento.valor);
                        }
                        else
                        {
                            conciliado.descricaoCC += item2.BancoMovimento.data.ToShortDateString() + "-" + item2.BancoMovimento.historico + " Valor " + item2.BancoMovimento.valor.ToString("n");
                            conciliado.ConciliadoCC.Add(new ConciliadoDet { descricao = item2.BancoMovimento.data.ToShortDateString() + "-" + item2.BancoMovimento.historico + " Valor " + item2.BancoMovimento.valor.ToString("n") });
                            valorAcumuladoBM += (item2.BancoMovimento.valor);

                        }
                    }
                }
                conciliado.valorOFX = valorAcumuladoOFX.ToString("n");
                conciliado.valorCC = valorAcumuladoBM.ToString("n");

                rets.Add(conciliado);

            }
            return rets;

        }

        private static void ConvertLanctoExtratoVW(List<LanctoExtrato> OFXs, List<LanctoExtratoVLW> OFXVLWS)
        {
            foreach (var item in OFXs)
            {
                var existe = (new BancoMovimentoLanctoExtrato().ObterLanctoExtrato_id(item.id, null).Count() > 0);

                if (!existe)
                {
                    OFXVLWS.Add(new LanctoExtratoVLW
                    {
                        data = item.data.ToShortDateString(),
                        descricao = item.descricao,
                        id = item.id,
                        idLancto = item.idLancto,
                        Tipo = item.Tipo,
                        Valor = item.Valor
                    });
                }
            }
        }

        private static void CalculaTotais(ConciliacaoVLW retorno)
        {
            retorno.ExtratoTotalRegistros = retorno.LanctoExtratos.Count();
            retorno.BancoTotalRegistros = retorno.BancoMovimentoConciliacaoVLWs.Count();

            if (retorno.ExtratoTotalRegistros != 0)
                retorno.ExtratoPaginaQtd = retorno.LanctoExtratos.Count() / 5;
            else
                retorno.ExtratoPaginaQtd = 1;

            if (retorno.BancoTotalRegistros != 0)
                retorno.BancoPaginaQtd = retorno.BancoMovimentoConciliacaoVLWs.Count() / 5;
            else
                retorno.BancoPaginaQtd = 1;

            retorno.BancoPaginaAtual = 1;
            retorno.ExtratoPaginaAtual = 1;
        }

        [HttpPost]
        public JsonResult Marca(string id, string valor)
        {
            if (id != "")
            {
                var x = new BancoMovimentoLanctoExtratoUsuario();
                var y = x.ObterUsuarioCodigo(id);

                if (y == null)
                {

                    x.Codigo = id;
                    x.emissao = DateTime.Now;
                    var i = 0;
                    if (int.TryParse(id, out i) == true)
                        x.Tipo = "OFX";
                    else
                        x.Tipo = "CC";
                    x.UsuarioLogado = Acesso.UsuarioLogado();
                    x.Valor = decimal.Parse(valor.Replace(".", ","));
                    x.Incluir(_paramBase);
                }
                else
                {
                    y.Excluir(_paramBase);
                }
            }

            var retorno = new ConciliacaoVLWTotal();

            retorno.ExtratoTotal = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario().
                        Where(p => p.Tipo == "OFX").Sum(p => p.Valor).ToString("n");
            retorno.BancoTotal = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario().
                        Where(p => p.Tipo == "CC").Sum(p => p.Valor).ToString("n");



            return Json(retorno, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Conciliar()
        {
            string retorno = "";
            var obj = new ConciliacaoVLWTotal();

            obj.ExtratoTotal = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario().
                        Where(p => p.Tipo == "OFX").Sum(p => p.Valor).ToString("n");
            obj.BancoTotal = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario().
                        Where(p => p.Tipo == "CC").Sum(p => p.Valor).ToString("n");


            if (obj.BancoTotal != obj.ExtratoTotal)
            {
                retorno = "Valor inválido, Total de Extrato não é igual a valor do Banco Movimento";
            }
            else
            {
                SalvaConc();
                ExcluirTodosUsuario(_paramBase);
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ConciliarAuto(int idOFX, int idCC, int idCPAG, int idNFES)
        {

            var db = new DbControle();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                var objOFX = new LanctoExtrato().ObterPorId(idOFX, db);
                var objCCs = new List<BancoMovimento>();

                if (idCC != 0)
                    objCCs.Add(new BancoMovimento().ObterPorId(idCC, db));

                if (idCPAG != 0)
                    objCCs = new BancoMovimento().ObterPorCPAG(idCPAG, _paramBase, db);

                if (idNFES != 0)
                    objCCs = new BancoMovimento().ObterPorNFES(idNFES, db, _paramBase);



                var bancoMovimentoLanctoExtrato = new BancoMovimentoLanctoExtrato();
                var bancoMovimentoLanctoExtratoMestre = new BancoMovimentoLanctoExtratoMestre();

                bancoMovimentoLanctoExtratoMestre.banco_id = objOFX.banco_id;
                bancoMovimentoLanctoExtratoMestre.dataConciliacao = DateTime.Now;
                bancoMovimentoLanctoExtratoMestre.Usuario = Acesso.UsuarioLogado();
                bancoMovimentoLanctoExtratoMestre.banco_id = objOFX.banco_id;
                bancoMovimentoLanctoExtratoMestre.dataConciliacao = objOFX.data;

                bancoMovimentoLanctoExtratoMestre.Incluir(db, _paramBase);

                foreach (var item in objCCs)
                {
                    bancoMovimentoLanctoExtrato.LanctoExtrato_id = objOFX.id;
                    bancoMovimentoLanctoExtrato.BancoMovimento_id = item.id;
                    bancoMovimentoLanctoExtrato.BancoMovimentoLanctoExtratoMestre_id = bancoMovimentoLanctoExtratoMestre.id;
                    bancoMovimentoLanctoExtrato.LanctoExtrato_id = objOFX.id;
                    bancoMovimentoLanctoExtrato.Incluir(db, _paramBase);

                    var y = new LanctoExtrato().ObterPorId(objOFX.id, db);
                    y.DataConciliado = DateTime.Now;
                    y.Conciliado = true;
                    y.UsuConciliado = Acesso.UsuarioLogado();
                    y.Alterar(db, _paramBase);

                }
                dbcxtransaction.Commit();
            }
            return Json("OK", JsonRequestBehavior.AllowGet);

        }


        private void ExcluirTodosUsuario(ParamBase pb)
        {
            var objs = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario();
            foreach (var item in objs)
            {
                item.Excluir(_paramBase);
            }


        }

        private void SalvaConc()
        {

            var ofxs = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario().
                                Where(p => p.Tipo == "OFX");

            int banco = 0;
            DateTime data = DateTime.Now;

            bool fez = false;
            var bancoMovimentoLanctoExtratoMestre = new BancoMovimentoLanctoExtratoMestre();

            var db = new DbControle();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                foreach (var item in ofxs)
                {
                    int idlancto = int.Parse(item.Codigo);
                    var y = new LanctoExtrato().ObterPorId(idlancto, db);
                    y.DataConciliado = DateTime.Now;
                    y.Conciliado = true;
                    banco = y.banco_id;
                    data = y.data;
                    y.UsuConciliado = Acesso.UsuarioLogado();
                    y.Alterar(db, _paramBase);

                    if (!fez)
                    {
                        fez = true;

                        bancoMovimentoLanctoExtratoMestre.Usuario = Acesso.UsuarioLogado();
                        bancoMovimentoLanctoExtratoMestre.banco_id = banco;
                        bancoMovimentoLanctoExtratoMestre.dataConciliacao = data;
                        bancoMovimentoLanctoExtratoMestre.Incluir(db, _paramBase);
                    }
                    var bvs = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario(db).
                                       Where(p => p.Tipo == "CC");
                    foreach (var itembv in bvs)
                    {
                        var bms = new List<BancoMovimento>();
                        if (itembv.Codigo.StartsWith("CPAG"))
                        {
                            int x = int.Parse(itembv.Codigo.Substring(4));
                            bms = new BancoMovimento().ObterTodos(db, _paramBase).Where(p => p.DocumentoPagarParcela_id == x).ToList();
                        }
                        else if (itembv.Codigo.StartsWith("NFES"))
                        {
                            int x = int.Parse(itembv.Codigo.Substring(4));
                            bms = new BancoMovimento().ObterTodos(db, _paramBase).Where(p => p.id == x).ToList();
                        }
                        else
                        {
                            int x = int.Parse(itembv.Codigo.Substring(2));
                            bms = new BancoMovimento().ObterTodos(db, _paramBase).Where(p => p.id == x).ToList();
                        }
                        foreach (var itembm in bms)
                        {
                            var bvrel = new BancoMovimentoLanctoExtrato();
                            bvrel.BancoMovimento_id = itembm.id;
                            bvrel.LanctoExtrato_id = int.Parse(item.Codigo);
                            bvrel.BancoMovimentoLanctoExtratoMestre_id = bancoMovimentoLanctoExtratoMestre.id;
                            bvrel.Incluir(db, _paramBase);
                        }
                    }
                }
                dbcxtransaction.Commit();
            }
        }

        private void SalvaConcBM(int planoDeConta_id, int tipoDeDocumento_id, int unidadeDeNegocio_id, string historico)
        {
            DbControle db = new DbControle();
            var ofxs = new BancoMovimentoLanctoExtratoUsuario().ObterTodosUsuario().
                                Where(p => p.Tipo == "OFX");

            var OrigemMovimentoTipoManual = new OrigemMovimento().TipoManual(_paramBase);
            var TipoMovimentoTipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
            var TipoMovimentoTipoSaida = new TipoMovimento().TipoSaida(_paramBase);

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {

                foreach (var item in ofxs)
                {
                    var ofx = new LanctoExtrato().ObterPorId(int.Parse(item.Codigo), db);
                    ofx.DataConciliado = DateTime.Now;
                    ofx.Conciliado = true;
                    ofx.UsuConciliado = Acesso.UsuarioLogado();
                    ofx.Alterar(db, _paramBase);


                    var bm = new BancoMovimento();
                    bm.banco_id = ofx.banco_id;
                    bm.data = ofx.data;
                    //bm.empresa_id  = pb.empresa_id;
                    bm.historico = historico;
                    bm.origemmovimento_id = OrigemMovimentoTipoManual;
                    bm.planoDeConta_id = planoDeConta_id;
                    bm.tipoDeDocumento_id = tipoDeDocumento_id;

                    if (ofx.Valor >= 0)
                    {
                        bm.tipoDeMovimento_id = TipoMovimentoTipoEntrada;
                        bm.valor = ofx.Valor;
                    }
                    else
                    {
                        bm.tipoDeMovimento_id = TipoMovimentoTipoSaida;
                        bm.valor = ofx.Valor * -1;
                    }
                    //bm.unidadeDeNegocio_id = unidadeDeNegocio_id;
                    bm.Incluir(bm, _paramBase);

                    var bancoMovimentoLanctoExtratoMestre = new BancoMovimentoLanctoExtratoMestre();
                    bancoMovimentoLanctoExtratoMestre.Usuario = Acesso.UsuarioLogado();
                    bancoMovimentoLanctoExtratoMestre.banco_id = ofx.banco_id;
                    bancoMovimentoLanctoExtratoMestre.dataConciliacao = ofx.data;
                    bancoMovimentoLanctoExtratoMestre.Incluir(db, _paramBase);


                    var bancoMovimentoLanctoExtrato = new BancoMovimentoLanctoExtrato();
                    bancoMovimentoLanctoExtrato.BancoMovimento_id = bm.id;
                    bancoMovimentoLanctoExtrato.BancoMovimentoLanctoExtratoMestre_id = bancoMovimentoLanctoExtratoMestre.id;
                    bancoMovimentoLanctoExtrato.LanctoExtrato_id = ofx.id;
                    bancoMovimentoLanctoExtrato.Incluir(db, _paramBase);

                }
                dbcxtransaction.Commit();
            }
        }

        [HttpPost]
        public JsonResult ExcluiItemConciliado(int id)
        {
            DbControle db = new DbControle();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                var objs = new BancoMovimentoLanctoExtrato().ObterBancoMovimentoLanctoExtratoMestre_id(id, db);
                foreach (var item in objs)
                {
                    new BancoMovimentoLanctoExtrato().Excluir(item.id, _paramBase, db);

                    var y = new LanctoExtrato().ObterPorId(item.LanctoExtrato_id, db);
                    if (y != null)
                    {
                        y.DataConciliado = null;
                        y.Conciliado = false;
                        y.UsuConciliado = null;
                        y.Alterar(db, _paramBase);
                    }
                }

                new BancoMovimentoLanctoExtratoMestre().Excluir(id, db, _paramBase);
                dbcxtransaction.Commit();


            }

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        #endregion


        public ActionResult Create3()
        {
            ViewData["banco"] = new Banco().CarregaBancoGeral(_paramBase);
            return View();
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

        public JsonResult ListaContaContabil()
        {
            var pc = new PlanoDeConta().ObterTodosTipoA();
            return Json(pc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaTipoDocumento()
        {
            var data = new SelectList(new TipoDocumento().ObterTodos(), "id", "descricao");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaUnidadeNegocio()
        {
            return Json(new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult PesquisarAjs(string data, string banco_id)
        {
            ConciliacaoVLW retorno = new ConciliacaoVLW();
            int banco = int.Parse(banco_id);
            DateTime dataAux = DateTime.Parse(data.Substring(0, 10));

            var OFXs = new LanctoExtrato().ObterTodos(banco, dataAux);

            List<LanctoExtratoVLW> OFXVLWS = new List<LanctoExtratoVLW>();

            ConvertLanctoExtratoVW(OFXs, OFXVLWS);


            retorno.LanctoExtratos = OFXVLWS;
            retorno.BancoMovimentoConciliacaoVLWs = new BancoMovimento().ObterAgrupado(banco, dataAux, _paramBase);
            retorno.Conciliados = MontaConciliados(banco, dataAux);
            retorno.ConciliadosAuto = MontaConciliadosAuto(banco, dataAux);
            retorno.ConsultaConciliados = MontaConsultaConciliadosAuto(banco, dataAux);

            CalculaTotais(retorno);


            return Json(retorno, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PesquisarBMAjs(DateTime data, string banco_id, int tipo, decimal valor)
        {
            ConciliacaoVLW retorno = new ConciliacaoVLW();
            int banco = int.Parse(banco_id);
            DateTime dataAux = SoftFin.Utils.UtilSoftFin.TiraHora(data); 

            if (tipo == 1)
            {
                retorno.BancoMovimentoConciliacaoVLWs = new BancoMovimento().ObterAgrupado(banco, dataAux, _paramBase);
            }
            else if (tipo == 2)
            {
                dataAux = new BancoMovimento().ObterDataSuperior(dataAux, valor, _paramBase);
                retorno.BancoMovimentoConciliacaoVLWs = new BancoMovimento().ObterAgrupado(banco, dataAux, _paramBase);
            }
            else
            {
                dataAux = new BancoMovimento().ObterDataInferior(dataAux, valor, _paramBase);
                retorno.BancoMovimentoConciliacaoVLWs = new BancoMovimento().ObterAgrupado(banco, dataAux, _paramBase);
            }



            return Json(new { data = dataAux.ToString("o"), BancoMovimentoConciliacaoVLWs = retorno.BancoMovimentoConciliacaoVLWs }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExcluirAjs(int id)
        {
            ExcluirConciliado(id);

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        private void ExcluirConciliado(int id)
        {
            DbControle db = new DbControle();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                var objs = new BancoMovimentoLanctoExtrato().ObterBancoMovimentoLanctoExtratoMestre_id(id, db);
                foreach (var item in objs)
                {
                    new BancoMovimentoLanctoExtrato().Excluir(item.id, _paramBase, db);
                }

                foreach (var item in objs.Select(p => p.LanctoExtrato_id).Distinct())
                {
                    var y = new LanctoExtrato().ObterPorId(item, db);
                    if (y != null)
                    {
                        y.DataConciliado = null;
                        y.Conciliado = false;
                        y.UsuConciliado = null;
                        y.Alterar(db, _paramBase);
                    }

                }

                new BancoMovimentoLanctoExtratoMestre().Excluir(id, db, _paramBase);
                dbcxtransaction.Commit();


            }
        }

        public JsonResult ConciliarAutoAjsLista(List<Conciliado> objs)
        {

            
            foreach (var obj in objs.Where(p => p.selecionado == true))
            {
                using (var db = new DbControle())
                {
                    var dbcxtransaction = db.Database.BeginTransaction();
                    SalvaAuto(obj, db);
                    dbcxtransaction.Commit();
                }
               
            }

            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        public JsonResult ConciliarAutoAjs(Conciliado obj)
        {

            var db = new DbControle();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                SalvaAuto(obj, db);
                dbcxtransaction.Commit();
            }
            return Json("OK", JsonRequestBehavior.AllowGet);

        }

        private void SalvaAuto(Conciliado obj, DbControle db)
        {
            var objOFX = new LanctoExtrato().ObterPorId(obj.id, db);
            var objCCs = new List<BancoMovimento>();

            if (obj.idCC != 0)
                objCCs.Add(new BancoMovimento().ObterPorId(obj.idCC, db));

            if (obj.idCPAG != 0)
                objCCs = new BancoMovimento().ObterPorCPAG(obj.idCPAG, _paramBase, db);

            if (obj.idNFES != 0)
                objCCs = new BancoMovimento().ObterPorNFES(obj.idNFES, db, _paramBase);



            var bancoMovimentoLanctoExtrato = new BancoMovimentoLanctoExtrato();
            var bancoMovimentoLanctoExtratoMestre = new BancoMovimentoLanctoExtratoMestre();

            bancoMovimentoLanctoExtratoMestre.banco_id = objOFX.banco_id;
            bancoMovimentoLanctoExtratoMestre.dataConciliacao = DateTime.Now;
            bancoMovimentoLanctoExtratoMestre.Usuario = Acesso.UsuarioLogado();
            bancoMovimentoLanctoExtratoMestre.banco_id = objOFX.banco_id;
            bancoMovimentoLanctoExtratoMestre.dataConciliacao = objOFX.data;

            bancoMovimentoLanctoExtratoMestre.Incluir(db, _paramBase);

            foreach (var item in objCCs)
            {
                bancoMovimentoLanctoExtrato.LanctoExtrato_id = objOFX.id;
                bancoMovimentoLanctoExtrato.BancoMovimento_id = item.id;
                bancoMovimentoLanctoExtrato.BancoMovimentoLanctoExtratoMestre_id = bancoMovimentoLanctoExtratoMestre.id;
                bancoMovimentoLanctoExtrato.LanctoExtrato_id = objOFX.id;
                bancoMovimentoLanctoExtrato.Incluir(db, _paramBase);

                var y = new LanctoExtrato().ObterPorId(objOFX.id, db);
                y.DataConciliado = DateTime.Now;
                y.Conciliado = true;
                y.UsuConciliado = _paramBase.usuario_name;
                    y.Alterar(db, _paramBase);

            }
        }
            




        public JsonResult SalvaBMAjs(int planoDeConta_id,
                            int tipoDeDocumento_id,
                            int unidadeDeNegocio_id,
                            string historico,
                            List<int> Ofxs)
        {
            DbControle db = new DbControle();

            var OrigemMovimentoTipoManual = new OrigemMovimento().TipoManual(_paramBase);
            var TipoMovimentoTipoEntrada = new TipoMovimento().TipoEntrada(_paramBase);
            var TipoMovimentoTipoSaida = new TipoMovimento().TipoSaida(_paramBase);

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                //var ofxConciliar = from a in db.LanctoExtrato join b in Ofxs on a.id equals b select a;

                foreach (var item in Ofxs)
                {
                    var ofx = new LanctoExtrato().ObterPorId(item, db);

                    ofx.DataConciliado = DateTime.Now;
                    ofx.Conciliado = true;
                    ofx.UsuConciliado = Acesso.UsuarioLogado();
                    ofx.Alterar(db, _paramBase);

                    var bm = new BancoMovimento();
                    bm.banco_id = ofx.banco_id;
                    bm.data = ofx.data;
                    bm.historico = historico;
                    bm.origemmovimento_id = OrigemMovimentoTipoManual;
                    bm.planoDeConta_id = planoDeConta_id;
                    bm.tipoDeDocumento_id = tipoDeDocumento_id;

                    if (ofx.Valor >= 0)
                    {
                        bm.tipoDeMovimento_id = TipoMovimentoTipoEntrada;
                        bm.valor = ofx.Valor;
                    }
                    else
                    {
                        bm.tipoDeMovimento_id = TipoMovimentoTipoSaida;
                        bm.valor = ofx.Valor * -1;
                    }
                    bm.Incluir(bm, _paramBase);

                    var bancoMovimentoLanctoExtratoMestre = new BancoMovimentoLanctoExtratoMestre();
                    bancoMovimentoLanctoExtratoMestre.Usuario = Acesso.UsuarioLogado();
                    bancoMovimentoLanctoExtratoMestre.banco_id = ofx.banco_id;
                    bancoMovimentoLanctoExtratoMestre.dataConciliacao = ofx.data;
                    bancoMovimentoLanctoExtratoMestre.Incluir(db, _paramBase);


                    var bancoMovimentoLanctoExtrato = new BancoMovimentoLanctoExtrato();
                    bancoMovimentoLanctoExtrato.BancoMovimento_id = bm.id;
                    bancoMovimentoLanctoExtrato.BancoMovimentoLanctoExtratoMestre_id = bancoMovimentoLanctoExtratoMestre.id;
                    bancoMovimentoLanctoExtrato.LanctoExtrato_id = ofx.id;
                    bancoMovimentoLanctoExtrato.Incluir(db, _paramBase);
                }

                dbcxtransaction.Commit();
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConciliarAjs(
                            List<int> Ofxs, List<int> Exts, int idBanco, DateTime dataConciliacao)
        {
            try
            {
                if (Ofxs == null || Exts == null)
                    return Json("Selecione pelo menus 1 lançamento de extrato e 1 movimento bancário.", JsonRequestBehavior.AllowGet);

                if (Ofxs.Count() == 0 || Exts.Count() == 0)
                    return Json("Selecione pelo menus 1 lançamento de extrato e 1 movimento bancário.", JsonRequestBehavior.AllowGet);

                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    //var ofxs = from a in db.LanctoExtrato join b in Ofxs on a.id equals b select a;

                    var bancoMovimentoLanctoExtratoMestre = new BancoMovimentoLanctoExtratoMestre();

                    bancoMovimentoLanctoExtratoMestre.Usuario = Acesso.UsuarioLogado();
                    bancoMovimentoLanctoExtratoMestre.banco_id = idBanco;
                    bancoMovimentoLanctoExtratoMestre.dataConciliacao =SoftFin.Utils.UtilSoftFin.TiraHora(dataConciliacao);
                    bancoMovimentoLanctoExtratoMestre.Incluir(db, _paramBase);


                    foreach (var item in Ofxs)
                    {
                        var lanctoExtrato = new LanctoExtrato().ObterPorId(item, db);
                        lanctoExtrato.DataConciliado = DateTime.Now;
                        lanctoExtrato.Conciliado = true;
                        lanctoExtrato.UsuConciliado = Acesso.UsuarioLogado();
                        lanctoExtrato.Alterar(db, _paramBase);

                        foreach (var itembv in Exts)
                        {
                            var bvrel = new BancoMovimentoLanctoExtrato();
                            bvrel.BancoMovimento_id = itembv;
                            bvrel.LanctoExtrato_id = item;
                            bvrel.BancoMovimentoLanctoExtratoMestre_id = bancoMovimentoLanctoExtratoMestre.id;
                            bvrel.Incluir(db, _paramBase);
                        }
                    }
                    dbcxtransaction.Commit();
                }
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json("Sistema Indisponivel", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ExcluirOFX(int id)
        {
            try
            {
                new LanctoExtrato().Excluir(id, _paramBase);

                return Json(new { CDStatus = "OK", CDMessage = "Excluido com sucesso." });
            }
            catch(Exception ex)
            {
                return Json(new { CDStatus = "NOK", CDMessage = ex.Message });
            }
        }

        public JsonResult ExcluirOFXLista(List<int> objs)
        {
            try
            {
                foreach (var item in objs)
                {
                    new LanctoExtrato().Excluir(item, _paramBase);
                }

                return Json(new { CDStatus = "OK", CDMessage = "Excluido com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", CDMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UploadOFX()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".txt", ".ofx" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Impossivel carregar o OFX! extensão não permitida, são permitidas somente as extensões (OFX e TXT)" });

                    }
                }
            }

            var pessoa = new Pessoa();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                var uploadPath = Server.MapPath("~/OFXTemp/");
                Directory.CreateDirectory(uploadPath);

                var path = Path.Combine(Server.MapPath("~/OFXTemp/"), arquivo.FileName);
                arquivo.SaveAs(path);


                FileStream SourceStream = System.IO.File.Open(path, FileMode.Open);
                var ofx = new OfxDocument();

                var extrato = ofx.CarregaOFX(SourceStream);

                var codBanco = "";
                var codAgencia = "";
                var codConta = "";
                var codDigito = "";
                DbControle db = new DbControle();

                int estab = _paramBase.estab_id;
                var banco = new Banco();

                if (ofx.BankID == "0237")
                {
                    codBanco = int.Parse(ofx.BankID).ToString();
                    codConta = ofx.AccountID.ToString();
                    banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.codigoBanco == codBanco &&
                            x.contaCorrente == codConta).FirstOrDefault();

                }
                else if (ofx.BankID.Substring(0, 3) == "033")
                {
                    codBanco = ofx.BankID.Substring(0, 3);
                    codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                    codConta = ofx.AccountID.Substring(4, 8).ToString();
                    codDigito = ofx.AccountID.Substring(12, 1).ToString();
                    banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.agencia == codAgencia &&
                            x.contaCorrenteDigito == codDigito &&
                            x.contaCorrente == codConta &&
                            x.codigoBanco == codBanco).FirstOrDefault();

                }
                else if (ofx.BankID.IndexOf("104") >= 0)
                {
                    codBanco = int.Parse(ofx.BankID).ToString();
                    //codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                    codConta = ofx.AccountID.Substring(4, 4).ToString();
                    codDigito = ofx.AccountID.Substring(8, 1).ToString();
                    banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.contaCorrenteDigito == codDigito &&
                            x.contaCorrente == codConta &&
                            x.codigoBanco == codBanco).FirstOrDefault();

                }
                else
                {

                    codBanco = int.Parse(ofx.BankID).ToString();
                    if (codBanco == "1") //Banco do Brasil
                    {
                        //codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                        codConta = ofx.AccountID.Substring(0, 4).ToString();
                        codDigito = ofx.AccountID.Substring(5, 1).ToString();
                        banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                                x.contaCorrenteDigito == codDigito &&
                                x.contaCorrente == codConta &&
                                x.codigoBanco == "001").FirstOrDefault();
                    }
                    else
                    {
                        codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                        codConta = ofx.AccountID.Substring(4, 5).ToString();
                        codDigito = ofx.AccountID.Substring(9, 1).ToString();
                        banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.agencia == codAgencia &&
                            x.contaCorrenteDigito == codDigito &&
                            x.contaCorrente == codConta &&
                            x.codigoBanco == codBanco).FirstOrDefault();

                    }


                }





                if (banco == null)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Este arquivo não tem um banco configurado, favor acesse o cadastro de bancos e contas e cadastre, caso tenha dificuldade ligue para a softfin" });
                }


                foreach (var item in extrato)
                {
                    var vData = DateTime.Parse(item.DatePosted.Substring(0, 4) + "-" + item.DatePosted.Substring(4, 2) + "-" + item.DatePosted.Substring(6, 2));
                    var vValor = decimal.Parse(item.TransAmount.Replace(".", ","));

                    LanctoExtrato lanctoExtrato = db.LanctoExtrato.Where(p => p.banco_id == banco.id
                                                                           && p.idLancto == item.FITID
                                                                           && p.data == vData
                                                                           && p.Valor == vValor).FirstOrDefault();

                    if (lanctoExtrato == null)
                    {

                        lanctoExtrato = new LanctoExtrato();
                        db.LanctoExtrato.Add(lanctoExtrato);
                    }

                    lanctoExtrato.idLancto = item.FITID.Trim();
                    lanctoExtrato.banco_id = banco.id;
                    lanctoExtrato.data = vData;
                    lanctoExtrato.descricao = item.Memo.Trim();
                    if (item.TransType == TransTypes.Credit)
                        lanctoExtrato.Tipo = "C";
                    else
                        lanctoExtrato.Tipo = "D";

                    lanctoExtrato.Valor = vValor;

                    db.SaveChanges();
                }
               

            }



            return Json(new { CDStatus = "OK", DSMessage = "Importação Finalizada com sucesso" });
        }

        public JsonResult CarregaNaoConciliado(DateTime data, string banco_id)
        {
            ConciliacaoVLW retorno = new ConciliacaoVLW();
            int banco = int.Parse(banco_id);
            DateTime dataAux = SoftFin.Utils.UtilSoftFin.TiraHora(data); ;

            var OFXs = new LanctoExtrato().ObterTodos(banco, dataAux);

            List<LanctoExtratoVLW> OFXVLWS = new List<LanctoExtratoVLW>();

            ConvertLanctoExtratoVW(OFXs, OFXVLWS);


            retorno.LanctoExtratos = OFXVLWS;
            //retorno.BancoMovimentoConciliacaoVLWs = new BancoMovimento().ObterAgrupado(banco, dataAux, _paramBase);
            //CalculaTotais(retorno);
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }



        public JsonResult CarregaAutoConciliados(string data, string banco_id)
        {
            DateTime dataAux = DateTime.Parse(data.Substring(0, 10));
            int banco = int.Parse(banco_id);
            var ConciliadosAuto = MontaConciliadosAuto(banco, dataAux);
            return Json(ConciliadosAuto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConsultaConciliadosPeriodo(string data, string banco_id)
        {
            DateTime dataAux = DateTime.Parse(data.Substring(0, 10));
            int banco = int.Parse(banco_id);
            var ConsultaConciliados = MontaConsultaConciliadosAuto(banco, dataAux);
            return Json(ConsultaConciliados, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConsultaConciliados(string data, string banco_id)
        {
            DateTime dataAux = DateTime.Parse(data.Substring(0, 10));
            int banco = int.Parse(banco_id);
            var ConsultaConciliados = MontaConciliados(banco, dataAux);
            return Json(ConsultaConciliados, JsonRequestBehavior.AllowGet);
        }

    }
}
