using Lib.Web.Mvc.JQuery.JqGrid;
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
    public class RetornoAprovacao
    {
        public int id { get; set; }
        public string data { get; set; }
        public string usuario { get; set; }
        public string projeto { get; set; }
        public string atividade { get; set; }
        public string descricao { get; set; }
        public string totalgasto { get; set; }
        public string totalemaberto { get; set; }
    }
    public class ApontamentoAprovacaoController : BaseController
    {
        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }


        private void CarregaViewData()
        {
            ViewData["usuarios"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["DataInicial"] = DateTime.Now.ToShortDateString();
            ViewData["DataFinal"] = DateTime.Now.ToShortDateString();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Pesquisa(string dataInicial, string dataFinal, string usuario)
        {

            var objs = new ApontamentoDiario().ObterTodosDataUsuario(DateTime.Parse(dataInicial), DateTime.Parse(dataFinal), int.Parse(usuario),_paramBase);
            objs = objs.Where(p => p.SituacaoAprovacao == null).OrderBy(p => p.data).ToList();

            var rets = new List<RetornoAprovacao>();

            foreach (var item in objs)
            {
                rets.Add(new RetornoAprovacao
                    {
                        projeto = item.Atividade.Projeto.codigoProjeto + 
                                    " - " + item.Atividade.Projeto.nomeProjeto +
                                    " - " + item.Atividade.Projeto.ContratoItem.Contrato.Pessoa.nome,
                        atividade = item.Atividade.descricao,
                        data = item.data.ToShortDateString(),
                        id = item.id,
                        usuario = item.apontador.nome,
                        descricao = item.historico,
                        totalemaberto = item.qtdHorasRestantes.ToString("n"),
                        totalgasto = item.qtdHoras.ToString("n")
                    }
                );

            }


            return Json(rets, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Index(FormCollection obs)
        {
            foreach (var item in obs.AllKeys)
            {
                int id = 0;
                string Situacao = null;
                string descricao = null;
                var db = new DbControle();
                if (item.Contains("Situacao_"))
                {
                    id = int.Parse(item.Replace("Situacao_", ""));
                    var auxdesc = "txtDesc_" + id.ToString();
                    Situacao = obs[item];
                    descricao = obs[auxdesc];

                    var apontamentoDiario = new ApontamentoDiario().ObterPorId(id, db, _paramBase);

                    if (Situacao != "")
                    {
                        if (string.IsNullOrEmpty(Situacao))
                            apontamentoDiario.SituacaoAprovacao = null;
                        else
                            apontamentoDiario.SituacaoAprovacao = Situacao;

                        apontamentoDiario.DescricaoAprovacao = descricao;
                        apontamentoDiario.dataAprovado = DateTime.Now;
                        apontamentoDiario.aprovador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado()); 
                    }

                    apontamentoDiario.Alterar(_paramBase,db);
                    //ontamentoDiario.aprovador_id 

                }
            }

            ViewBag.msg = "Salvo com sucesso";
            CarregaViewData();
            return View();
        }

    }
}
