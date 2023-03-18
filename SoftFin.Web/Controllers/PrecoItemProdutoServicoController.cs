using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SoftFin.Web.Controllers
{
    public class PrecoItemProdutoServicoController : BaseController
    {
        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string user = Acesso.UsuarioLogado();
            int usuario = Acesso.RetornaIdUsuario(user);
            string ValorTabela = Request.QueryString["TabelaPrecoItemProdutoServico_ID"];
            string ValorItem = Request.QueryString["ItemProdutoServico_ID"];
            string ValorDescricao = Request.QueryString["descricao"];
            string ValorPreco = Request.QueryString["preco"];

            int totalRecords = 0;
            PrecoItemProdutoServico obj = new PrecoItemProdutoServico();
            var objs = new PrecoItemProdutoServico().ObterTodos(_paramBase);


            if (!String.IsNullOrEmpty(ValorTabela))
            {
                int aux;
                int.TryParse(ValorTabela, out aux);
                objs = objs.Where(p => p.TabelaPrecoItemProdutoServico_ID == aux).ToList();
            }

            if (!String.IsNullOrEmpty(ValorItem))
            {
                int aux;
                int.TryParse(ValorItem, out aux);
                objs = objs.Where(p => p.ItemProdutoServico_ID == aux).ToList();
            }

            if (!String.IsNullOrEmpty(ValorDescricao))
            {
                objs = objs.Where(p => p.descricao.Contains(ValorDescricao)).ToList();
            }

            if (!String.IsNullOrEmpty(ValorPreco))
            {
                decimal aux;
                decimal.TryParse(ValorItem, out aux);
                objs = objs.Where(p => p.preco >= aux).ToList();
            }

            objs = Organiza(request, objs);
            totalRecords = objs.Count();
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.TabelaPrecoItemProdutoServico.descricao,
                    item.ItemProdutoServico.descricao,
                    item.descricao,
                    item.preco
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        private static List<PrecoItemProdutoServico> Organiza(JqGridRequest request, List<PrecoItemProdutoServico> objs)
        {
            switch (request.SortingName)
            {
                case "TabelaPrecoItemProdutoServico_ID":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.TabelaPrecoItemProdutoServico.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.TabelaPrecoItemProdutoServico.descricao).ToList();
                    break;
                case "ItemProdutoServico_ID":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.ItemProdutoServico.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.ItemProdutoServico.descricao).ToList();
                    break;
                case "descricao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.descricao).ToList();
                    break;
                case "preco":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.preco).ToList();
                    else
                        objs = objs.OrderBy(p => p.preco).ToList();
                    break;

            }
            return objs;
        }

        [HttpPost]
        public ActionResult Create(PrecoItemProdutoServico obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CarregaViewData();
                    PrecoItemProdutoServico preco = new PrecoItemProdutoServico();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    if (preco.Incluir( obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Preço incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Preço já cadastrado";
                        return View(obj);
                    }
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
        public ActionResult Create()
        {
            try
            {
                CarregaViewData();
                var obj = new PrecoItemProdutoServico();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(PrecoItemProdutoServico obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CarregaViewData();
                    PrecoItemProdutoServico preco = new PrecoItemProdutoServico();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;
                    preco.Alterar(obj, _paramBase);
                    ViewBag.msg = "Preço alterado";
                    return View(obj);
                }
                else
                {
                    CarregaViewData();
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
        public ActionResult Edit(int ID)
        {
            try
            {
                CarregaViewData();
                PrecoItemProdutoServico obj = new PrecoItemProdutoServico();
                PrecoItemProdutoServico preco = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(preco);
                return View(preco);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(PrecoItemProdutoServico obj)
        {
            try
            {
                CarregaViewData();
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
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
        public ActionResult Delete(int ID)
        {
            try
            {
                CarregaViewData();
                PrecoItemProdutoServico obj = new PrecoItemProdutoServico();
                PrecoItemProdutoServico preco = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(preco);
                return View(preco);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }


        private void CarregaViewData()
        {
            ViewData["Tabela"] = new SelectList(new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase), "id", "descricao");
            ViewData["Item"] = new SelectList(new ItemProdutoServico().ObterTodos(_paramBase), "id", "descricao");
        }
    }
}
