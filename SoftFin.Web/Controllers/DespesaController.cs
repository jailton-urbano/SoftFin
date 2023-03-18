using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SoftFin.Web.Controllers
{
    public class DespesaController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new Despesa();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["estabelecimento_id"] = item.estabelecimento_id;
                myExport["Estabelecimento"] = item.Estabelecimento.Apelido;
                myExport["colaborador_id"] = item.colaborador_id;
                myExport["Colaborador"] = item.Colaborador.nome;
                myExport["cliente_id"] = item.cliente_id;
                myExport["Cliente"] = item.Cliente.nome;
                myExport["projeto_id"] = item.projeto_id;
                myExport["Projeto"] = item.Projeto.nomeProjeto;
                myExport["tipoDespesa_id"] = item.tipoDespesa_id;
                myExport["TipoDespesa"] = item.TipoDespesa.descricao;
                myExport["Data"] = item.Data;
                myExport["valor"] = item.valor;
                myExport["descricao"] = item.descricao;
                myExport["aprovador_id"] = item.aprovador_id;
                myExport["Aprovador"] = item.Aprovador.nome;
                myExport["aprovado"] = item.SituacaoAprovacao.ToString();
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "despesa.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new Despesa();

            return View();
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string Valorid = Request.QueryString["id"];
            string Valorestabelecimento_id = Request.QueryString["estabelecimento_id"];
            string Valorcolaborador_id = Request.QueryString["colaborador_id"];
            string Valorcliente_id = Request.QueryString["cliente_id"];
            string Valorprojeto_id = Request.QueryString["projeto_id"];
            string ValortipoDespesa_id = Request.QueryString["tipoDespesa_id"];
            string ValorData = Request.QueryString["Data"];
            string Valorvalor = Request.QueryString["valor"];
            string Valordescricao = Request.QueryString["descricao"];
            string Valoraprovador_id = Request.QueryString["aprovador_id"];
            string Valoraprovado = Request.QueryString["aprovado"];
            string Valorlote_id = Request.QueryString["lote_id"];
            string ValorLoteDespesa = Request.QueryString["LoteDespesa"];
            int totalRecords = 0;
            Despesa obj = new Despesa();
            var objs = new Despesa().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valorid))
            {
                int aux;
                int.TryParse(Valorid, out aux);
                objs = objs.Where(p => p.id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorestabelecimento_id))
            {
                int aux;
                int.TryParse(Valorestabelecimento_id, out aux);
                objs = objs.Where(p => p.estabelecimento_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorcolaborador_id))
            {
                int aux;
                int.TryParse(Valorcolaborador_id, out aux);
                objs = objs.Where(p => p.colaborador_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorcliente_id))
            {
                int aux;
                int.TryParse(Valorcliente_id, out aux);
                objs = objs.Where(p => p.cliente_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorprojeto_id))
            {
                int aux;
                int.TryParse(Valorprojeto_id, out aux);
                objs = objs.Where(p => p.projeto_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(ValortipoDespesa_id))
            {
                int aux;
                int.TryParse(ValortipoDespesa_id, out aux);
                objs = objs.Where(p => p.tipoDespesa_id.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(Valordescricao))
            {
                objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
            }
            if (!String.IsNullOrEmpty(Valoraprovador_id))
            {
                int aux;
                int.TryParse(Valoraprovador_id, out aux);
                objs = objs.Where(p => p.aprovador_id.Equals(aux)).ToList();
            }
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
                string Aprovador = "";
                if (item.Aprovador != null)
                    Aprovador = item.Aprovador.nome;

                string LoteCobranca = "";
                if (item.LoteCobranca != null)
                    LoteCobranca = item.LoteCobranca.codigo.ToString();

                string LoteReembolso = "";
                if (item.LoteReembolsoDespesa != null)
                    LoteReembolso = item.LoteReembolsoDespesa.codigo.ToString();

                string LoteAdiantamento = "";
                if (item.LoteAdiantamento != null)
                    LoteAdiantamento = item.LoteAdiantamento.codigo.ToString();


                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Data.ToShortDateString(),
                    item.valor.ToString("n"),
                    item.Cliente.nome,
                    item.Colaborador.nome,
                    item.TipoDespesa.descricao,
                    LoteCobranca,
                    LoteReembolso,
                    LoteAdiantamento,
                    Aprovador,
                    item.SituacaoAprovacao
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Despesa obj)
        {
            try
            {
                bool validaLimite = true;

                var despesaPermitida = new DespesaPermitida().ObterPorProjeto(obj.projeto_id, _paramBase).Where(x=> x.tipodespesa_id == obj.tipoDespesa_id).FirstOrDefault();
                if (despesaPermitida != null)
                {
                    if (obj.valor > despesaPermitida.valorLimite)
                    {
                        validaLimite = false;
                    }
                }
                    CarregaViewData();
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;

                    if (validaLimite == true)
                    {

                    if (ModelState.IsValid)
                    {
                        if (obj.Incluir(_paramBase))
                        {
                            ViewBag.msg = "Incluído com sucesso";
                            return View(obj);
                        }
                        else
                        {
                            ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                            return View(obj);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dados Invalidos");
                        return View(obj);
                    }
                }
                else
                {
                    ViewBag.msg = "Valor da despesa excede o limite";
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();
                var obj = new Despesa();
                obj.Data = DateTime.Now;
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Despesa obj)
        {
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    int estab = _paramBase.estab_id;
                    obj.estabelecimento_id = estab;
                    if (obj.Alterar(_paramBase))
                    {
                        ViewBag.msg = "Alterado com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Impossivel alterar, registro excluído";
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
        public ActionResult Edit(int ID)
        {
            try
            {
                CarregaViewData();
                Despesa obj = new Despesa();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);


                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        public ActionResult Delete(Despesa obj)
        {
            try
            {
                CarregaViewData();
                string erro = "";
                if (obj.Excluir(ref erro, _paramBase))
                {
                    ViewBag.msg = "Excluido com sucesso";
                    return RedirectToAction("/Index");
                }
                else
                {
                    ViewBag.msg = erro;
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            try
            {
                CarregaViewData();
                Despesa obj = new Despesa();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        private void CarregaViewData()
        {
            ViewData["colaborador"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["cliente"] = new SelectList(new Pessoa().ObterCliente(_paramBase), "id", "nome");
            ViewData["projeto"] = new SelectList(new Projeto().ObterTodos(_paramBase), "id", "nomeProjeto");
            ViewData["tipoDespesa"] = new SelectList(new TipoDespesa().ObterTodos(_paramBase), "id", "descricao");
            ViewData["aprovador"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["lote"] = new SelectList(new LoteDespesa().ObterTodos(_paramBase), "id", "lote");
        }

        public JsonResult getProjetos(int cliente)
        {
            var estab = _paramBase.estab_id;
            var banco = new DbControle();
            var projetos = new List<comboProjeto>();

            projetos = (from projeto in banco.Projeto
                        where (projeto.ContratoItem.Contrato.pessoas_ID == cliente && projeto.estabelecimento_id == estab)
                        select new comboProjeto
                        {
                            id = projeto.id,
                            nomeProjeto = projeto.nomeProjeto
                        }).ToList();

            return Json(projetos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getTipoDespesa(int projeto)
        {
            var estab = _paramBase.estab_id;
            var banco = new DbControle();
            var tipoDespesa = new List<comboTipoDespesa>();

            tipoDespesa = (from tipo in banco.TipoDespesa
                           join desp in banco.DespesaPermitida
                           on tipo.id equals desp.tipodespesa_id
                           where (desp.projeto_id == projeto && desp.estabelecimento_id == estab)
                           select new comboTipoDespesa
                           {
                               id = tipo.id,
                               descricao = tipo.descricao
                           }).ToList();

            return Json(tipoDespesa, JsonRequestBehavior.AllowGet);
        }
    }
}
