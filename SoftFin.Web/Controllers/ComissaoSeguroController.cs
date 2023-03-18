
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ComissaoSeguroController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }


        private static string ExtraiString(Dictionary<string, string> parameters, string key)
        {
            try
            {
                return parameters[key];
            }
            catch
            {
                return null;
            }

        }

        private List<ComissaoSeguro> Organiza(JqGridRequest request, List<ComissaoSeguro> objs)
        {

            return new SoftFin.Utils.UtilSoftFin.GenericSorter<ComissaoSeguro>().Sort(objs.AsQueryable(), request.SortingName, request.SortingOrder).ToList();
        }


        //[OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Listas(JqGridRequest request)
        {
            Dictionary<string, string> parameters = HttpContext.Request.QueryString.Keys.Cast<string>()
                .ToDictionary(k => k, v => HttpContext.Request.QueryString[v]);

            int totalRecords = 0;
            var objs = new ComissaoSeguro().ObterTodos();

            if (!String.IsNullOrEmpty(ExtraiString(parameters, "descr")))
            {
                var auxstring = parameters["descricao"];
                objs = objs.Where(p => p.descricao == auxstring).ToList();
            }

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            objs = Organiza(request, objs);
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Seguradora.nome, 
                    item.ProdutoSeguradora.descricao,
                    item.descricao,
                    item.PercentualSeguradora.ToString("0.00"),
                    item.PercentualVendedor.ToString("0.00")
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        public JsonResult Salvar(ComissaoSeguro entidade)
        {
            try
            {
                if (entidade.SeguradoraDescricao != null)
                {
                    var nomePessoa = entidade.SeguradoraDescricao.Split(',')[0];
                    var cnpjPessoa = entidade.SeguradoraDescricao.Split(',')[1];
                    var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa,_paramBase);
                    entidade.Seguradora_id = pessoa.id;
                }
                if (entidade.id == 0)
                {
                    entidade.dataInclusao = DateTime.Now;
                    entidade.dataAlteracao = DateTime.Now;
                }

                var validacao = entidade.Validar(ModelState);
                if (validacao.Count() > 0)
                    return Json(new { CDStatus = "NOK", Erros = validacao }, JsonRequestBehavior.AllowGet);

                if (entidade.ObterPorId(entidade.id) == null)
                {
                    entidade.Incluir(_paramBase);
                }
                else
                {
                    entidade.dataAlteracao = DateTime.Now;
                    entidade.Alterar(entidade,_paramBase);
                }
                return Json(new
                {
                    CDStatus = "OK",
                    apolice = (entidade == null) ? null : new
                    {
                        descricao = entidade.descricao
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Excluir(ComissaoSeguro entidade)
        {
            try
            {
                if (entidade.ObterPorId(entidade.id) != null)
                {
                    string auxErro = "";
                    if (!entidade.Excluir(entidade.id, ref auxErro, _paramBase))
                    {
                        return Json(new { CDStatus = "NOK", Exception = auxErro }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { CDStatus = "NOK", Exception = "Registro não encontrado" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    CDStatus = "OK",
                    apolice = (entidade == null) ? null : new
                    {
                        descricao = entidade.descricao
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObterId(int id)
        {
            try
            {
                var obj = new ComissaoSeguro().ObterPorId(id);
                return Json(new
                {
                    CDStatus = "OK",
                    entidade = (obj == null) ? new ComissaoSeguroVM
                    {
                        dataAlteracao = DateTime.Now.ToString("o"),
                        dataInclusao = DateTime.Now.ToString("o"),
                        id = 0
                    }
                    : new ComissaoSeguroVM
                    {
                        dataAlteracao = obj.dataAlteracao.ToString("o"),
                        dataInclusao = obj.dataInclusao.ToString("o"),
                        descricao = obj.descricao,
                        id = obj.id,
                        PercentualSeguradora = obj.PercentualSeguradora,
                        PercentualVendedor = obj.PercentualVendedor,
                        ProdutoSeguradora_id = obj.ProdutoSeguradora_id,
                        Seguradora_id = obj.Seguradora_id,
                        SeguradoraDescricao = obj.Seguradora.nome + ", " +obj.Seguradora.cnpj
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", Exception = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListaPessoas()
        {
            var seguradoras = new Pessoa().ObterSeguradoras(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in seguradoras)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0}, {1} ", item.nome, item.cnpj) });
            }
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListaProdutoSeguradora()
        {
            return Json(new ProdutoSeguradora().ObterTodos(), JsonRequestBehavior.AllowGet);
        }

    }
}
