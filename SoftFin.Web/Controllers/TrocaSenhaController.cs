using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class TrocaSenhaController : BaseController
    {
        //
        // GET: /TrocaSenha/

        public ActionResult Index()
        {
            ViewData["Usuario"] = Acesso.UsuarioLogado();
            return View();
        }
        [HttpPost]
        public ActionResult Index(string usuario, string senhaAtual, string senhaNova, string senhaNovaConfirmacao)
        {
            if (usuario != Acesso.UsuarioLogado())
            {
                return RedirectToAction("Login", "Home");
            }

            ViewData["Usuario"] = Acesso.UsuarioLogado();

            if (senhaNova != senhaNovaConfirmacao)
            {
                ViewData["MsgErro"] = "Confirmação de senha inválida.";
                return View(); ;
            }
            if (senhaNova.Length < 7 )
            {
                ViewData["MsgErro"] = "Senha muito curta.";
                return View(); ;
            }

            if (Acesso.RetornaSenhaUsuario(usuario) != senhaAtual)
            {
                ViewData["MsgErro"] = "Senha inválida.";
                return View(); ;
            }



            Acesso.TrocaSenhaLogado(senhaNovaConfirmacao);



            ViewData["MsgErro"] = "Senha alterada com sucesso";
            return View();
        }

    }
}
