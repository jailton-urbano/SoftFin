using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymousAttribute]
        [HttpPost]
        public ActionResult Index(string usuario, string senha)
        {
            try
            {
                string logout = Request.Form["Logout"];
                if (logout != null)
                {
                    Acesso.Deslogar();
                    
                    return RedirectToAction("/Index", "Home");
                }
                else
                {

                    if (Acesso.isUsuarioValido(usuario, senha))
                    {
                        Acesso.Logar(usuario);
                        return RedirectToAction("/Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Usuário ou senha inválido");
                        ViewData["ERRO"] = "Usuário ou senha inválido";
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}