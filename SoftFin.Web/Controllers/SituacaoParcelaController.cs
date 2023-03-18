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
    public class SituacaoParcelaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listas(JqGridRequest request)
        {

            int totalRecords = 0;



            totalRecords = new StatusParcela().ObterTodos().Count();

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
            //Table with rows data

            List<StatusParcela> lista = new List<StatusParcela>();

            lista = new StatusParcela().ObterTodos().OrderBy(p => p.status).Skip(12 * request.PageIndex).Take(12).ToList();

            foreach (var item in lista)
            {


                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.status
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public ActionResult Create(StatusParcela obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StatusParcela sp = new StatusParcela();

                    if (sp.Incluir(obj, _paramBase) == true)
                    {

                        ViewBag.msg = "Status da Parcela incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                        ViewBag.msg = "Status da Parcela já cadastrado";
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
                //carrega varável de sessão do usuário logado
                ViewBag.usuario = Acesso.UsuarioLogado();
                ViewBag.perfil = Acesso.PerfilLogado();

                StatusParcela obj = new StatusParcela();
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Edit(StatusParcela obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StatusParcela tipo = new StatusParcela();

                    tipo.Alterar(obj, _paramBase);

                    ViewBag.msg = "Status do Contrato alterado com sucesso";
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
        public ActionResult Edit(int ID)
        {
            try
            {
                StatusParcela obj = new StatusParcela();
                StatusParcela st = obj.ObterPorId(ID);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(st);
                return View(st);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(StatusParcela obj)
        {
            try
            {
                //CarregaViewData();
                string Erro = "";
                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    ViewBag.msg = Erro;
                    obj = obj.ObterPorId(obj.id);
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
                StatusParcela obj = new StatusParcela();
                StatusParcela st = obj.ObterPorId(ID);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(st);
                return View(st);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }


    }
}
