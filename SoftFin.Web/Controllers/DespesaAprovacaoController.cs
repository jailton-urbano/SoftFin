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
    public class DespesaAprovacaoController : BaseController
    {
        public class RetornoAprovacao
        {
            public int id { get; set; }
            public string data { get; set; }
            public string projeto { get; set; }
            public string tipodespesa { get; set; }
            public string descricao { get; set; }
            public string valor { get; set; }
        }

        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }


        private void CarregaViewData()
        {
            ViewData["usuario"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["DataInicial"] = DateTime.Now.ToShortDateString();
            ViewData["DataFinal"] = DateTime.Now.ToShortDateString();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Pesquisa(string dataInicial, string dataFinal, string usuario)
        {

            var objs = new Despesa().ObterTodosDataUsuario(DateTime.Parse(dataInicial), DateTime.Parse(dataFinal), int.Parse(usuario), _paramBase);
            objs = objs.Where(p => p.SituacaoAprovacao == null).OrderBy(p => p.Data).ToList();

            var rets = new List<RetornoAprovacao>();
            var user = Acesso.UsuarioLogado();

            foreach (var item in objs)
            {
                rets.Add(new RetornoAprovacao
                {
                    id = item.id,
                    data = item.Data.ToShortDateString(),
                    projeto = item.Projeto.nomeProjeto,
                    tipodespesa = item.TipoDespesa.descricao,
                    descricao = item.descricao,
                    valor = item.valor.ToString("n")
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

                    var Despesa = new Despesa().ObterPorId(id, db,_paramBase);

                    if (Situacao != "")
                    {
                        if (string.IsNullOrEmpty(Situacao))
                            Despesa.SituacaoAprovacao = null;
                        else
                            Despesa.SituacaoAprovacao = Situacao;

                        Despesa.DescricaoAprovacao = descricao;
                        Despesa.dataAprovado = DateTime.Now;
                        Despesa.aprovador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                    }

                    Despesa.Alterar(_paramBase, db);

                }
            }

            ViewBag.msg = "Salvo com sucesso";
            CarregaViewData();
            return View();
        }

    }

}
