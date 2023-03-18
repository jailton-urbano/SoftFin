using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SoftFin.Web.Controllers
{
    public class DashController : BaseController
    {
        //
        // GET: /Dash/
        //[OutputCache(Duration = 6000, VaryByParam = "id", Location = OutputCacheLocation.Client, NoStore = false)]
        public ActionResult Index(int id)
        {
            var difcache = _paramBase.perfil_id.ToString() + "." + id.ToString();
            var Dash = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<SistemaDashBoard>("Dash.id:" + difcache);

            ViewBag.MigalhaC = "Dash";
            if (Dash == null)
                Dash = new SoftFin.Web.Models.SistemaDashBoard().ObterAtivoPorId(id);


            var menu = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<List<SistemaMenu>>("SistemaMenu");

            if (menu == null)
                menu = new SoftFin.Web.Models.SistemaMenu().ObterTodosAtivos();

            SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("SistemaMenu", menu);

            if (Dash != null)
            {
                SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("Dash.id:" + difcache, Dash);

                ViewBag.MigalhaB = Dash.Descricao;
                var manuA = menu.Where(p => p.id == Dash.sistemaMenu_id);
                ViewBag.MigalhaA = menu.First().Descricao;
                
            }

            ViewBag.Title = ViewBag.MigalhaB;

            var funcionalidades = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<List<SoftFin.Web.Models.
                SistemaDashBoardFuncionalidade>>("SistemaDashBoardFuncionalidade.id:" + difcache);

            if (funcionalidades == null)
                funcionalidades = new SoftFin.Web.Models.SistemaDashBoardFuncionalidade().ObterTodoPorPerfil(id, _paramBase).OrderBy(p => p.Ordem).ToList();

            if (funcionalidades.Count() == 0)
                funcionalidades = new SoftFin.Web.Models.SistemaDashBoardFuncionalidade().ObterTodoPorPerfil(id, _paramBase).OrderBy(p => p.Ordem).ToList();

            SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("SistemaDashBoardFuncionalidade.id:" + difcache, funcionalidades);

            ViewData["relatorios"] = funcionalidades.Where(p => p.relatorio == true).OrderBy(p => p.Ordem).ToList().ToList();  
            ViewData["cadastros"] = funcionalidades.Where(p => p.cadastro == true).OrderBy(p => p.Ordem).ToList().ToList();

            return View(funcionalidades.Where(p => p.cadastro == false && p.relatorio == false).ToList());
        }


        public JsonResult Favorito(int id)
        {
            var usuario = new UsuarioFavorito();

            string message = usuario.SalvaFavorito(id, _paramBase);

            return Json(new { CDStatus = "OK", Message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}
