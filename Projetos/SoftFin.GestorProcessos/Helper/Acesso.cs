using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Helper
{
    public static class Acesso
    {
        public static bool isUsuarioLogado()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public static void Logar(string Nome)
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(Nome, false);
        }

        public static void Deslogar()
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }

        internal static string ObtemUsuario()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public static bool isUsuarioValido(string usuario, string senha)
        {
            var y = new Usuario().ValidarUsuario(usuario, senha);
            return (y);
        }
    }
}