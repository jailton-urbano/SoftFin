using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class PaginaDinamicaController : BaseController
    {
        // GET: PaginaDinamica
        public ActionResult Manut(string tabela)
        {

            ViewData["Tabela"] = tabela;
            ViewData["CodigoUsuario"] = _usuario;
            ViewData["CodigoEmpresa"] = _codigoEmpresa;
            ViewData["CodigoEstabelecimento"] = _codigoEmpresa;
            ViewData["urlSPG"] = ConfigurationManager.AppSettings["urlSPG"].ToString();
            return View();
        }

        public ActionResult Index(string Id)
        {
            var novaAtividade = new CallApi.GestorProcessos().
                CriarNovaExecucao(new
                GestorProcessos.Comum.Param.ParamProcesso
                { CodigoEmpresa = _codigoEmpresa, CodigoUsuario = _usuario, CodigoProcesso = Id });

            if (novaAtividade.status == "OK")
            {
                ViewData["CodigoProcesso"] = Id;
                ViewData["CodigoAtividadeAtual"] = novaAtividade.Obj.CodigoAtividadeAtual.ToString();
                ViewData["CodigoAtividade"] = novaAtividade.Obj.CodigoAtividade.ToString();
                ViewData["CodigoUsuario"] = _usuario;
                ViewData["CodigoEmpresa"] = _codigoEmpresa;
                ViewData["NumeroProtocolo"] = novaAtividade.Obj.NumeroProcesso;
                ViewData["CodigoAtividadeExecucaoAtual"] = novaAtividade.Obj.CodigoAtividadeExecucaoAtual;
                ViewData["CodigoProcessoAtual"] = novaAtividade.Obj.CodigoProcessoAtual;
                ViewData["urlSPG"] = ConfigurationManager.AppSettings["urlSPG"].ToString();
                ViewData["TipoAtividade"] = novaAtividade.Obj.TipoAtividade;

            }
            else
            {
                throw new Exception("Atividade não pode ser criada");
            }

            return View();
        }

    }
}