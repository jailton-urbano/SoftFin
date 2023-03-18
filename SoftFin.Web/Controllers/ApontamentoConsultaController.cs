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
    public class RetornoAprovacaoConsulta
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
    public class ApontamentoConsultaController : BaseController
    {
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

            var objs = new ApontamentoDiario().ObterTodosDataUsuario(DateTime.Parse(dataInicial), DateTime.Parse(dataFinal), int.Parse(usuario), _paramBase);
            objs = objs.OrderBy(p => p.data).ToList();

            var rets = new List<RetornoAprovacaoConsulta>();

            foreach (var item in objs)
            {
                rets.Add(new RetornoAprovacaoConsulta
                    {
                        projeto = item.Atividade.Projeto.nomeProjeto,
                        atividade = item.Atividade.descricao,
                        data = item.data.ToShortDateString(),
                        id = item.id,
                        usuario = "",
                        descricao = item.historico,
                        totalemaberto = item.qtdHorasRestantes.ToString("n"),
                        totalgasto = item.qtdHoras.ToString("n")
                    }
                );

            }


            return Json(rets, JsonRequestBehavior.AllowGet);

        }
    }
}
