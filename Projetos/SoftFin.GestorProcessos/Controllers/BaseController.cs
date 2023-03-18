
using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SoftFin.GestorProcessos.Controllers
{
	[Authorize]
	public class BaseController : Controller
    {
        public ParamBase _paramBase = new ParamBase();
        public string _mensagemTrocaAba = "Empresa diferente da logada";
        protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
            var db = new DBGPControle();
            var usuario = Acesso.ObtemUsuario();
            var usu = db.Usuario.Where(p => p.Login.Equals(usuario) && p.Ativo == true).FirstOrDefault();
            if (usu == null)
            {
                throw new Exception("Usuário Inválido");
            }
            _paramBase.Usuario = usuario;
            _paramBase.Empresa = usu.Empresa.Id;
        }

    }


}
