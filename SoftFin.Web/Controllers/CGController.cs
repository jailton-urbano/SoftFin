using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CGController : BaseController
    {
        public JsonResult ObterUnidades()
        {
            var objs = new UnidadeNegocio().ObterTodos(_paramBase);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.unidade
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterContaContabil()
        {
            var objs = new ContaContabil().ObterTodos(_paramBase).OrderBy(p => p.codigo).ToList();
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = string.Format("{0} - {1}", p.codigo, p.descricao)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterContaContabilPorCodigo()
        {
            var objs = new ContaContabil().ObterTodos(_paramBase).OrderBy(p => p.codigo).ToList();
            return Json(objs.Select(p => new
            {
                Value = p.codigo,
                Text = string.Format("{0} - {1}", p.codigo, p.descricao)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPlanoContasDebito()
        {
            var objs = new PlanoDeConta().ObterTodosDebito();
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = String.Format("{0} {1} ", (p.codigo + "________").Substring(0, 8) + " - ", p.descricao)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterBancoBoleto()
        {
            var objs = new Models.Banco().CarregaBancoGeralBoleto(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterBancoAplicacao()
        {
            var objs = new Models.Banco().ObterBancoAplicacao(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterBanco()
        {
            var objs = new Models.Banco().CarregaBanco(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterEstabs()
        {

            int usuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
            var acessos = new UsuarioEstabelecimento().ObterTodosRelacionados(_paramBase).Where(p => p.usuario_id == usuario);

            var x = new List<SelectListItem>();
            foreach (var item in acessos)
            {
                x.Add(new SelectListItem() { Value = item.Estabelecimento.id.ToString(), Text = String.Format("{0} - {1}", item.Estabelecimento.NomeCompleto, item.Estabelecimento.Apelido), Selected = false });
            }

            var listret = new SelectList(x, "Value", "Text");
            return Json(listret.Select(p => new
            {
                Selecionado = false,
                Valor = p.Value,
                Descricao = p.Text
            }), JsonRequestBehavior.AllowGet);
        }



    }
}