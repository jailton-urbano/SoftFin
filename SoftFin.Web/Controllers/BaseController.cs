using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Web.Regras;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SoftFin.Web.Controllers
{
	[Authorize]
	public class BaseController : Controller
	{
		public Eventos _eventos = new Eventos();
		public ParamBase _paramBase = new ParamBase();
		public int _estab;
		public int _empresa;
        public string _codigoEmpresa;
		public SoftFin.Web.Models.Estabelecimento _estabobj;
		public SoftFin.Web.Models.Usuario _usuarioobj;
		public string _usuario;

		RequestContext _requestContext;
		string _actionName = "";
		string _nameController ="";
        bool _isAjaxCall = false;
        bool _isPermission = false;

        public string _mensagemTrocaAba = "Não é possivel concluir a opção. Empresa ou Estabelecimento trocado em outra aba";


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);



            if (_isAjaxCall)
                return;

            if (_nameController == "Home")
                return;

            if (_nameController == "Site")
                return;

            if (_paramBase.usuario_name.ToUpper().Equals("JARVIS"))
                return;

            if (_nameController == "UsuarioEstabelecimento")
                return;
#if DEBUG
            return;
#endif

            if (_isPermission == false)
                filterContext.Result = RedirectToAction("Index", new RouteValueDictionary(
                    new { controller = "Home", action = "Index" }));
        }

        protected override void Initialize(RequestContext requestContext)
		{
            _isPermission = true;

            base.Initialize(requestContext);
            

            bool _isAjaxCall = (Request.AcceptTypes.Where(p => p == "application/json").Count() > 0);
            

            _requestContext = requestContext;
			_actionName = _requestContext.RouteData.Values["action"].ToString();
			_nameController = _requestContext.RouteData.Values["Controller"].ToString();

            _paramBase.usuario_ip = Request.UserHostAddress;
            _paramBase.usuario_action = _actionName;
            _paramBase.usuario_controller = _nameController;
            
            _eventos.paramBase = _paramBase;

            if (_isAjaxCall || _actionName.ToUpper().Contains("TOTALIZADOR"))
                return;

            if (_nameController == "Dash")
            {
                ViewBag.MigalhaD = "Painel";
                return;
            }

            _isPermission = false;

            ViewData["NomeController"] = _nameController;
			ViewData["NomeActionName"] = _actionName;

            


            ViewBag.MigalhaA = "";
			ViewBag.MigalhaB = "";
			ViewBag.MigalhaC = "";
			var db = new DbControle();

            IQueryable<Funcionalidade> principals;


			if (_nameController.ToUpper() == "DRE")
			{
			   principals = db.Funcionalidade.Where(p => p.NomeController == _nameController && p.NomeAction == _actionName);
			}
			else
			{
			   principals = db.Funcionalidade.Where(p => p.NomeController == _nameController);
			}

			


			if (principals.ToList().Count() > 0)
			{
				ViewData["idFuncionalidade"] = principals.First().id.ToString();

				var obj = new SoftFin.Web.Models.UsuarioFavorito().ObterPorIdFuncionalida(principals.First().id,db, _paramBase);
				if (obj != null)
					ViewData["paginapreferida"] = "paginapreferida";


			}


			switch (_actionName)
			{
				case "Index":
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.Index;
					break;
				case "Edit":
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.Edit;
					break;
				case "Create":
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.Create;
					break;
				case "Detail":
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.Detail;
					break;
				case "Delete":
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.Delete;
					break;
				case "listaContasAPagar":
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.listaContasAPagar;
					break;
				default:
					ViewBag.MigalhaD = SoftFin.Web.Regras.Dicionario.Index;
					break;
			}

            if (_nameController.ToUpper() != "DASH")
            {
                foreach (var item in principals)
                {
                    

                    var Func = new SoftFin.Web.Models.SistemaDashBoardFuncionalidade().ObterPorIdFuncionalidade(item.id);

                    

                    if (Func.Count() > 0)
                    {
                        _isPermission = true;
                        var descricao = Func.First().Descricao;
                        _eventos.Info("Acesso " + descricao);

                        ViewBag.MigalhaC = descricao;
                        var Dash = new SoftFin.Web.Models.SistemaDashBoard().ObterAtivoPorId(Func.First().sistemaDashBoard_id);
                        ViewBag.MigalhaB = Dash.Descricao;
                        ViewBag.MigalhaBUrl = "../../Dash/Index/" + Dash.id;
                        ViewBag.MigalhaCUrl = ViewBag.MigalhaB;
                        if (Dash != null)
                        {
                            var Menu = new SoftFin.Web.Models.SistemaMenu().ObterAtivoPorId(Dash.sistemaMenu_id);
                            ViewBag.MigalhaA = Menu.Descricao;
                        }
                    }


                }


            }


            ViewBag.Title = ViewBag.MigalhaC;



			//MenuCadastroGerais();
		
		}

        public JsonResult AcessoEdicao()
        {
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            var perfil = new PerfilFuncionalidade().ObterPorIdPerfil(_usuarioobj.idPerfil).Where(p => p.Funcionalidade.NomeController == controller);
            if (perfil.Count() == 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else 
                return Json((perfil.First().flgTipoAcesso == "T"), JsonRequestBehavior.AllowGet);
        }

        public void MenuCadastroGerais()
		{
			int id = 2;
			var cad = new SoftFin.Web.Models.SistemaDashBoardFuncionalidade().ObterPorIdCadastros(id);
			foreach (var item in cad)
			{
				item.Controller = item.Funcionalidade.NomeController;
				item.Action = item.Funcionalidade.NomeAction;
			}
			ViewData["cadastros"] = cad;
		}


		public BaseController()
		{
			try
			{
                //bool isAjaxCall = (Request.AcceptTypes.Where(p => p == "application/json").Count() > 0);
                int us = Acesso.EstabLogado2();
                if (us == 0)
                {
                    _estabobj = null;
                }
                else
                {
                    _estabobj = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<Estabelecimento>("_estabobj.usuario:" + us);
                    if (_estabobj == null)
                    {
                        _estabobj = Acesso.EstabLogadoObj2();
                        SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("_estabobj.usuario:" + us, _estabobj);
                    }
                        
                }


       


                if (_estabobj != null)
                {
                    var db = new DbControle();

                    ViewBag.usuario = Acesso.UsuarioLogado();
                    ViewBag.usu = ViewBag.usuario;
                    ViewBag.perfil = Acesso.PerfilLogado(db);
                    ViewBag.estabelecimento = _estabobj.NomeCompleto;
                    ViewBag.nomeController = _nameController;
                    ViewBag.nomeAction = _actionName;

                    _usuario = ViewBag.usuario;
                    _usuarioobj = Acesso.RetornaObjUsuario(_usuario, db);
                    _estab = _estabobj.id;
                    _empresa = _estabobj.Empresa_id;
                    _codigoEmpresa = _estabobj.Empresa.codigo;

                    _paramBase.estab_id = _estabobj.id;
                    _paramBase.empresa_id = _estabobj.Empresa_id;
                    _paramBase.holding_id = _estabobj.Empresa.Holding_id;
                    _paramBase.usuario_name = ViewBag.usuario;
                    _paramBase.usuario_id = _usuarioobj.id;
                    _paramBase.perfil_id = _usuarioobj.idPerfil;
                    _paramBase.municipio_id = _estabobj.Municipio_id;
                }
                else
                {
                    _paramBase.usuario_name = "";
                    _paramBase.usuario_id = 0;
                }


                
				
			}
			catch 
			{
			}
            _eventos.paramBase = _paramBase;
			

		}
		public ActionResult InvalidUser()
		{
			return PartialView("Error.cshtml");
		}

		public bool isValidaEstabUsuario(int estab )
		{
			if (estab == _estab)
			{
				return true;
			}

			return false;
		}
		public void ValidaEstabUsuarioException(int estab)
		{
			if (!isValidaEstabUsuario(estab))
			{
				throw new Exception(_mensagemTrocaAba);
			}
		}
		public string ValidaEstabUsuarioString(int estab)
		{
			if (!isValidaEstabUsuario(estab))
			{
				return _mensagemTrocaAba;
			}
			return "";
		}



		public bool isValidaEmpresaUsuario(int empresa)
		{
			if (empresa == _empresa)
			{
				return true;
			}

			return false;
		}
		public void ValidaEmpresaUsuarioException(int Empresa)
		{
			if (!isValidaEmpresaUsuario(Empresa))
			{
				throw new Exception(_mensagemTrocaAba);
			}
		}
		public string ValidaEmpresaUsuarioString(int Empresa)
		{
			if (!isValidaEmpresaUsuario(Empresa))
			{
				return _mensagemTrocaAba;
			}
			return "";
		}
		public ActionResult UnAuthorizedUser()
		{
			return PartialView("Error.cshtml");
		}


		[HttpPost]
		public virtual JsonResult TotalizadorDash(int? id)
		{
			if (id == null)
			{
				id = 1;
			}
				List<object> data = new List<object>();

				//var texto = "Mês";
				if (id == 1)
				{
					DataInicial = DateTime.Parse("01" + DateTime.Now.ToString("/MM/yyyy"));
					DataFinal = DataInicial.AddMonths(1);
					DataFinal = DataFinal.AddDays(-1);
				}
				if (id == 2)
				{
					//texto = "Semana";
					int numeroMenor = 1, numeroMaior = 7;
					DataInicial = DateTime.Now.AddDays(numeroMenor - DateTime.Now.DayOfWeek.GetHashCode());
					DataFinal = DateTime.Now.AddDays(numeroMaior - DateTime.Now.DayOfWeek.GetHashCode());
				}
				if (id == 3)
				{
					//texto = "Dia";
					DataInicial = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00");
					DataFinal = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59");
				}

			
			return Json(new { CDStatus = "OK", Result = ""},JsonRequestBehavior.AllowGet );
		}


		public DateTime DataInicial { get; set; }

		public DateTime DataFinal { get; set; }
	}
}
