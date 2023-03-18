using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class UsuarioEstabelecimentoController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            var regs = new UsuarioEstabelecimento().ObterTodos(_paramBase);
            int totalRecords = regs.Count();

            //Fix for grouping, because it adds column name instead of index to SortingName
            //string sortingName = "cdg_perfil";

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            regs = regs.OrderBy(p => p.Usuario.nome).Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                regs)
            {
                var usuario = "";
                var holding = "";
                var empresa = "";
                var estab = "";

                if (item.Usuario != null)
                    usuario = item.Usuario.nome;

                if (item.Estabelecimento.Empresa.Holding != null)
                    holding = item.Estabelecimento.Empresa.Holding.nome;

                if (item.Estabelecimento.Empresa != null)
                    empresa = item.Estabelecimento.Empresa.nome;

                if (item.Estabelecimento != null)
                    estab = item.Estabelecimento.NomeCompleto;

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    usuario,
                    holding,
                    empresa,
                    estab
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            try
            {
                CarregaViewData();
                return View();
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        private void CarregaViewData()
        {
            ViewData["Usuarios"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["Estabelecimentos"] = new SelectList(new Estabelecimento().ObterTodos(_paramBase), "id", "NomeCompleto");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioEstabelecimento obj)
        {
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {

                    if (obj.Incluir(_paramBase) == true)
                    {
                        ViewBag.msg = "Incluído com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "Relacionamento já criado sistema.";
                    }
                    return View(obj);
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        public ActionResult Edit(int id = 0)
        {
            try
            {
                UsuarioEstabelecimento usuarioEstabelecimento = new UsuarioEstabelecimento().ObterPorId(id, _paramBase);

                if (usuarioEstabelecimento == null)
                {
                    return HttpNotFound();
                }


                CarregaViewData();
                return View(usuarioEstabelecimento);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioEstabelecimento obj)
        {
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    int estab = _paramBase.estab_id;

                    if (obj.Alterar(_paramBase) == true)
                    {
                        ViewBag.msg = "Alterado com sucesso";
                    }
                    else
                    {
                        ViewBag.msg = "Não foi possivel atualizar o registro";
                    }
                    return View(obj);
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        public ActionResult Delete(int id = 0)
        {
            try
            {
                CarregaViewData();
                UsuarioEstabelecimento usuarioEstabelecimento = new UsuarioEstabelecimento().ObterPorId(id, _paramBase);
                if (usuarioEstabelecimento == null)
                {
                    return HttpNotFound();
                }
                return View(usuarioEstabelecimento);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CarregaViewData();
                UsuarioEstabelecimento obj = new UsuarioEstabelecimento();
                string Erro = "";
                if (!obj.Excluir(id, ref Erro,_paramBase))
                {
                    ViewBag.msg = Erro;

                    obj = obj.ObterPorId(obj.id, _paramBase);
                    return View(obj);
                }
                else
                {
                    return RedirectToAction("/Index");
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }


        public ActionResult SelecaoEstab()
        {
            int? empresa  = _paramBase.empresa_id;
            int usuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
            var acessos =new UsuarioEstabelecimento().ObterTodosRelacionados(_paramBase).Where(p => p.usuario_id == usuario);

            var x = new List<SelectListItem>();
            foreach (var item in acessos)
	        {
                x.Add(new SelectListItem() { Value = item.Estabelecimento.id.ToString(), Text = String.Format("{0} - {1}", item.Estabelecimento.NomeCompleto, item.Estabelecimento.Apelido), Selected = false });
	        }            

            var listret = new SelectList(x, "Value", "Text");
            ViewData["Estabelecimentos"] = listret;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelecaoEstab(FormCollection itens)
        {
            Acesso.Logar(int.Parse(itens["estabelecimento_id"]));
            
            return RedirectToAction("/Index","Home");
        }

    }
}
