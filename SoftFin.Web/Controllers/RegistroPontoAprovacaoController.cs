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
    public class RetornoPontoAprovacao
    {
        public int id { get; set; }
        public string usuario { get; set; }
        public string data { get; set; }
        public string ponto1 { get; set; }
        public string ponto2 { get; set; }
        public string ponto3 { get; set; }
        public string ponto4 { get; set; }
        public string ponto5 { get; set; }
        public string ponto6 { get; set; }
        public string ponto7 { get; set; }
        public string ponto8 { get; set; }
        public string comentarios { get; set; }

    }
    public class RegistroPontoAprovacaoController : BaseController
    {
        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }

        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new RegistroPonto().ObterTodosEntreData(DataInicial, DataFinal, _paramBase).Where(p=> p.aprovador_id == null).Count().ToString();
            return Json(new { CDStatus = "OK", Result = soma }, JsonRequestBehavior.AllowGet);

        }

        private void CarregaViewData()
        {
            ViewData["usuarios"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase).OrderBy(x => x.nome), "id", "nome");
            ViewData["DataInicial"] = DateTime.Now.ToShortDateString();
            ViewData["DataFinal"] = DateTime.Now.ToShortDateString();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Pesquisa(string dataInicial, string dataFinal, string usuario)
        {

            var objs = new RegistroPonto().ObterTodosDataUsuario(DateTime.Parse(dataInicial), DateTime.Parse(dataFinal), int.Parse(usuario), _paramBase);
            objs = objs.Where(p => p.SituacaoAprovacao == null).OrderBy(p => p.data).ToList();

            var rets = new List<RetornoPontoAprovacao>();

            foreach (var item in objs)
            {
                var auxComentarios = "";
                if (item.comentarios != null)
                    auxComentarios = item.comentarios;

                rets.Add(new RetornoPontoAprovacao
                {
                    id = item.id,
                    usuario = item.apontador.nome,
                    data = item.data.ToShortDateString(),
                    ponto1 = item.ponto1.ToShortTimeString(),
                    ponto2 = item.ponto2.ToShortTimeString(),
                    ponto3 = item.ponto3.ToShortTimeString(),
                    ponto4 = item.ponto4.ToShortTimeString(),
                    ponto5 = item.ponto5.ToShortTimeString(),
                    ponto6 = item.ponto6.ToShortTimeString(),
                    ponto7 = item.ponto7.ToShortTimeString(),
                    ponto8 = item.ponto8.ToShortTimeString(),
                    comentarios = auxComentarios
                }
                );
            }
            return Json(rets, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ObterId(int id)
        {
            try
            {
                var obj = new RegistroPonto().ObterPorId(id, _paramBase);


                return Json(new
                {
                    CDMessage = "OK",
                    obj.id,
                    data = obj.data.ToShortDateString(),
                    obj.apontador.nome,
                    ponto1 = obj.ponto1.ToString("HH:mm"),
                    ponto2 = obj.ponto2.ToString("HH:mm"),
                    ponto3 = obj.ponto3.ToString("HH:mm"),
                    ponto4 = obj.ponto4.ToString("HH:mm"),
                    ponto5 = obj.ponto5.ToString("HH:mm"),
                    ponto6 = obj.ponto6.ToString("HH:mm"),
                    ponto7 = obj.ponto7.ToString("HH:mm"),
                    ponto8 = obj.ponto8.ToString("HH:mm")
                }
                , JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDMessage = "NOK", DSMessage = ex.Message },JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Salvar(int id, string ponto1, string ponto2, string ponto3, string ponto4, string ponto5, string ponto6, string ponto7, string ponto8)
        {
            try
            {
                var db = new DbControle();
                var obj = new RegistroPonto().ObterPorId(id, db, _paramBase);


                obj.ponto1 = validadeFormataHora(obj.ponto1, ponto1,1);
                obj.ponto2 = validadeFormataHora(obj.ponto2, ponto2,2);
                obj.ponto3 = validadeFormataHora(obj.ponto3, ponto3,3);
                obj.ponto4 = validadeFormataHora(obj.ponto4, ponto4,4);
                obj.ponto5 = validadeFormataHora(obj.ponto5, ponto5,5);
                obj.ponto6 = validadeFormataHora(obj.ponto6, ponto6,6);
                obj.ponto7 = validadeFormataHora(obj.ponto7, ponto7,7);
                obj.ponto8 = validadeFormataHora(obj.ponto8, ponto8,8);
                obj.Alterar(obj, _paramBase, db);
                
                return Json(new
                {
                    CDStatus = "OK"
                }
                , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private DateTime validadeFormataHora(DateTime dateTime, string ponto, int numeroponto)
        {
            try
            {
                if (ponto.Length != 5)
                    throw new Exception("Formato de data inválido do ponto " + numeroponto.ToString() + " - Tamanho");

                if (ponto.Substring(2, 1) != ":")
                    throw new Exception("Formato de data inválido do ponto " + numeroponto.ToString() + " - 2 pontos");

                int hora = int.Parse(ponto.Substring(0, 2));
                int minuto = int.Parse(ponto.Substring(3, 2));

                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hora, minuto, 0);

            }
            catch 
            {
                throw new Exception("Formato de data inválido do ponto " + numeroponto.ToString() + " - Geral");
            }
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

                    var registroPonto = new RegistroPonto().ObterPorId(id, db, _paramBase);

                    if (Situacao != "")
                    {
                        if (string.IsNullOrEmpty(Situacao))
                            registroPonto.SituacaoAprovacao = null;
                        else
                            registroPonto.SituacaoAprovacao = Situacao;

                        registroPonto.DescricaoAprovacao = descricao;
                        registroPonto.dataAprovado = DateTime.Now;
                        registroPonto.aprovador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                    }

                    registroPonto.Alterar(_paramBase, db);
                }
            }

            ViewBag.msg = "Salvo com sucesso";
            CarregaViewData();
            return View();
        }

    }
}
