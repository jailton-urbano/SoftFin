using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ItemProdutoServicoController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            string Valorcodigo = Request.QueryString["codigo"];
            string Valordescricao = Request.QueryString["descricao"];


            var objs = new ItemProdutoServico().ObterTodos(_paramBase).ToList();

            if (!String.IsNullOrEmpty(Valorcodigo))
            {
                objs = objs.Where(p => p.codigo.Contains(Valorcodigo)).ToList();
            }
            if (!String.IsNullOrEmpty(Valordescricao))
            {
                objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
            }

            int totalRecords = objs.Count();

            //Fix for grouping, because it adds column name instead of index to SortingName
            //string sortingName = "cdg_perfil";
            objs = Organiza(request, objs);
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
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.codigo,
                    item.descricao,
                    item.unidadeMedida,
                    item.ncm
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        private static List<ItemProdutoServico> Organiza(JqGridRequest request, List<ItemProdutoServico> objs)
        {
            switch (request.SortingName)
            {

                case "codigo":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.codigo).ToList();
                    else
                        objs = objs.OrderBy(p => p.codigo).ToList();
                    break;
                case "descricao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.descricao).ToList();
                    break;
            }
            return objs;
        }

        public ActionResult ConsultaItem(string item)
        {
            try
            {
                ItemProdutoServico ps = new ItemProdutoServico();
                ItemProdutoServico obj = ps.ObterTodos(_paramBase).Where(x => x.codigo == item).FirstOrDefault();

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        [HttpPost]
        public ActionResult Create(ItemProdutoServico obj)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    ItemProdutoServico item = new ItemProdutoServico();
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;


                    if (item.Incluir(obj, _paramBase) == true)
                    {
                        CarregaViewData();

                        ViewBag.msg = "Item incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        CarregaViewData();

                        ViewBag.msg = "Item já cadastrada";
                        return View(obj);
                    }
                }
                else
                {
                    CarregaViewData();
                    String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                               .Select(v => v.ErrorMessage + " " + v.Exception));

                    ModelState.AddModelError("", messages);
                    return View(obj);

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        private void CarregaViewData()
        {
            ViewData["CategoriaItem"] = new SelectList(new CategoriaItemProdutoServico().ObterTodos(_paramBase), "id", "Descricao");

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

        [HttpPost]
        public ActionResult Edit(ItemProdutoServico obj)
        {
            try
            {
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    int idempresa  = _paramBase.empresa_id;
                    obj.empresa_id = idempresa;

                    ItemProdutoServico item = new ItemProdutoServico();
                    item.Alterar(obj, _paramBase);
                    ViewBag.msg = "Item alterado com sucesso";
                    return View(obj);
                }
                else
                {
                    String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(v => v.ErrorMessage + " " + v.Exception));
                    ModelState.AddModelError("", messages);
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
                ItemProdutoServico item = new ItemProdutoServico();
                item = new ItemProdutoServico().ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(item);
                return View(item);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        public ActionResult Detail(int ID)
        {
            try
            {
                return Delete(ID);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }


        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
                try
                {
                    CarregaViewData();
                    ItemProdutoServico item = new ItemProdutoServico().ObterPorId(id, _paramBase);
                    string Erro = "";
                    if (!item.Excluir(item.id, ref Erro, _paramBase))
                    {
                        ViewBag.msg = Erro;
                        item = item.ObterPorId(item.id, _paramBase);
                        return View(item);
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
                ItemProdutoServico obj = new ItemProdutoServico();
                ItemProdutoServico item = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(item);
                return View(item);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

    }
}
