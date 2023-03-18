using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Xml;
using System.Text;
using System.Net;
using SoftFin.Utils;

namespace SoftFin.Web.Controllers
{
    public class NotadeDebitoController : BaseController
    {
     
        private void CarregaViewData()
        {
            ViewData["cliente"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["DataCancelamento"] = DateTime.Now.ToString("dd/MMM/yyyy");
            ViewData["DataRecebimento"] = DateTime.Now.ToString("dd/MMM/yyyy");
            ViewData["bancos"] = new SelectList(new Banco().ObterTodos(_paramBase), "id", "nomeBanco");
            ViewData["contas"] = new PlanoDeConta().ObterNotadeDebito();
            ViewData["unidades"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
        }

        
   

        //Notas de Débito em Aberto
        public ActionResult notasDebitoEmAberto()
        {
            return View();
        }
        public JsonResult geraNotasDebitoEmAberto()
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            var banco = new DbControle();
            var notas = new List<NotasDebitoEmAberto>();

            notas = (from nd in banco.NotadeDebito
                     where (nd.situacaoNotaDebito_id == 1)
                            && nd.estabelecimento_id == estab
                     select new NotasDebitoEmAberto
                     {
                         id = nd.id,
                         numeroND = nd.numero,
                         dataEmissaoND = nd.DataEmissao,
                         dataVencimentoND = nd.DataVencimento,
                         cliente = nd.Cliente.nome,
                         valorND = nd.valor,
                         diasAtraso = 0
                     }).ToList();

            DbControle db = new DbControle();

            for (int i = 0; i < notas.Count; i++)
            {

                //Dias em atraso
                DateTime hoje = DateTime.Now;
                notas[i].diasAtraso = Math.Round((hoje - notas[i].dataVencimentoND).TotalDays);
                if (notas[i].diasAtraso < 0)
                {
                    notas[i].diasAtraso = 0;
                }

                //Datas
                notas[i].dataEmissaoNDS = notas[i].dataEmissaoND.ToString("dd/MM/yyyy");
                notas[i].dataVencimentoNDS = notas[i].dataVencimentoND.ToString("dd/MM/yyyy");

            };

            return Json(notas, JsonRequestBehavior.AllowGet);
        }

    }

}
