using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.ModelBinding;

namespace SoftFin.API
{
    public class BaseApi : ApiController
    {
        public string _usuario = "";
        public int _usuarioId = 0;
        public int _estabelecimento = 0;
        public ParamBase _paramBase = new ParamBase();
        

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            try
            {
                base.Initialize(controllerContext);

                var ctx = controllerContext.Request.Properties["MS_HttpContext"] as HttpContextWrapper;
                _paramBase.usuario_ip = ctx.Request.UserHostAddress;
                if (controllerContext.RouteData.Values.Where(p => p.Key == "MS_SubRoutes").Count() > 0)
                {
                    var aux2 = ((IHttpRouteData[])controllerContext.RouteData.Values["MS_SubRoutes"]);
                    _paramBase.usuario_controller = aux2[0].Route.RouteTemplate;
                    _paramBase.usuario_action = "";
                }

                HttpContext httpContext = HttpContext.Current;
                var token = controllerContext.Request.Headers.Authorization;
                
                if (token != null)
                {
                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(token.Parameter));
                    var tokenOriginal = usernamePassword.Replace(":", "").Split(';');
                        
                    ValidaToken(tokenOriginal[0]);

                    //if (tokenOriginal.Count() > 1)
                    //{
                    //    var tokenEstabelecimento = tokenOriginal[1].ToString().Split(';');
                    //    _paramBase.estab_id = int.Parse(tokenEstabelecimento[1]);
                    //}


                }
                else
                {
                    throw new HttpException(401, "Token Inválido");
                }

            }
            catch (Exception ex)
            {
                throw new HttpException(401, "Token Inválido" + ex.ToString());
            }

        }

        internal void ValidaToken(string tokenstr)
        {
            
            var tokenaux = SoftFin.Utils.Crypto.Decryption(tokenstr);
            var usuario = int.Parse(tokenaux.Split(';')[0]);
            var usuariopesquisa = new Usuario().ObterPorId(usuario);

            //if (usuariopesquisa.tokenApi == tokenstr)
            //{
                _paramBase.tokenValidado = true;
                _paramBase.usuario_id = usuariopesquisa.id;
                _paramBase.usuario_name = usuariopesquisa.codigo;
                _usuario = usuariopesquisa.codigo;
                _usuarioId = usuariopesquisa.id;
            //}
            //else
            //{
            //    throw new HttpException(401, "Token Inválido");
            //}
        }

        protected internal bool TryValidateModel(object model)
        {
            return TryValidateModel(model, null /* prefix */);
        }

        protected internal bool TryValidateModel(object model, string prefix)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            var t = new ModelBindingExecutionContext(new HttpContextWrapper(HttpContext.Current), new System.Web.ModelBinding.ModelStateDictionary());

            foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(metadata, t).Validate(null))
            {
                ModelState.AddModelError(validationResult.MemberName, validationResult.Message);
            }

            return ModelState.IsValid;
        }
    }
}