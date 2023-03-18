using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ContratoItemController : BaseController
    {
        int idContrato = 0;
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            int totalRecords = 0;


            string Contrato = Request.QueryString["Contrato"];
            int estab = _paramBase.estab_id;

            if (string.IsNullOrEmpty(Contrato))
                totalRecords = new ContratoItem().ObterTodos(_paramBase).Count();
            else
                totalRecords = new ContratoItem().ObterTodos(_paramBase).Where(x => x.Contrato.contrato == Contrato).Count();

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

            var lista = new List<Models.ContratoItem>();

            if (string.IsNullOrEmpty(Contrato))
                lista = new ContratoItem().ObterTodos(_paramBase).OrderBy(p => p.Contrato.contrato).Skip(12 * request.PageIndex).Take(12).ToList();
            else
                lista = new ContratoItem().ObterTodos(_paramBase).Where(x =>  x.Contrato.contrato == Contrato).OrderBy(p => p.Contrato.contrato).Skip(12 * request.PageIndex).Take(12).ToList();

            foreach (var item in lista)
            {
                string tipoContrato = "";
                if (item.TipoContrato != null)
                    tipoContrato = item.TipoContrato.tipo;

                string nome = "";
                if (item.Contrato.Pessoa != null)
                    nome = item.Contrato.Pessoa.nome;

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Contrato.contrato,
                    item.itemProdutoServico_ID,
                    nome,
                    item.pedido,
                    item.UnidadeNegocio.unidade,
                    tipoContrato,
                    item.valor.ToString("0.00")
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ContratoExcel(int idContrato)
        {

            var obj = new ContratoItem();
            var lista = obj.lista(idContrato).ToList();

            CsvExport myExport = new CsvExport();
            foreach (var contrato in lista)
            {
                myExport.AddRow();
                myExport["Contrato"] = contrato.Contrato.contrato;

                //myExport["Emissao"] = contrato.emissao;
                myExport["Cliente"] = contrato.Contrato.Pessoa.nome;
                myExport["Pedido"] = contrato.pedido;
                //myExport["Prazo"] = contrato.prazo;
                //myExport["Status"] = contrato.ParcelaContratos;
                myExport["Tipo"] = contrato.TipoContrato.tipo;
                myExport["Unidade"] = contrato.UnidadeNegocio.unidade;
                myExport["Valor Total"] = contrato.valor;

            }


            //Exporta os dados para o Excel
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();

            //Abri o arquivo de retorno no Excel
            return File(myCsvData, "application/vnd.ms-excel", "ContratoItem.csv");
        }

        [HttpPost]
        public ActionResult Create(ContratoItem obj)
        {
            try
            {
                idContrato = obj.contrato_id;
                CarregaViewData();


                if (ModelState.IsValid)
                {
                    /*obj.Contrato.estabelecimento_id = pb.estab_id;*/
                    var contrato = new ContratoItem();


                    if (contrato.Inclui(obj, _paramBase) == true)
                    {
                        ViewBag.msg = "Item de Contrato incluído com sucesso";
                        return View(obj);
                    }
                    else
                    {
                            

                        ViewBag.msg = "Pedido já cadastrado";
                        return View(obj);
                    }
                }


                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                            .Select(v => v.ErrorMessage + " " + v.Exception));

                ModelState.AddModelError("", messages);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Create(int id)
        {
            try
            {
                idContrato = id;
                CarregaViewData();


                var obj = new ContratoItem();
                obj.contrato_id = id;

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private void CarregaViewData()
        {
            ViewData["TipoContrato"] = new SelectList(new TipoContrato().ObterTodos(_paramBase), "id", "tipo");
            ViewData["ClienteContrato"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
            ViewData["UnidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
            ViewData["Contrato"] = new SelectList(new Contrato().ObterTodos(_paramBase).Where(p => p.id == idContrato).ToList(), "id", "contrato");
            ViewData["item"] = new SelectList(new ItemProdutoServico().ObterTodos(_paramBase), "id", "descricao");
            ViewData["tabelaPreco"] = new SelectList(new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase), "id", "descricao");
        }

        [HttpPost]
        public ActionResult Edit(ContratoItem obj)
        {
            try
            {
                
                idContrato = obj.contrato_id;
                CarregaViewData();
                if (ModelState.IsValid)
                {
                    var contr = new ContratoItem();
                    contr.Altera(obj, _paramBase);
                    ViewBag.msg = "item de Contrato alterado com sucesso";
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
                ContratoItem ct = new ContratoItem().ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(ct);
                idContrato = ct.contrato_id;
                CarregaViewData();

                return View(ct);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [HttpPost]
        public ActionResult Delete(ContratoItem obj)
        {
            try
            {
                obj = obj.ObterPorId(obj.id, _paramBase);
                idContrato = obj.contrato_id;
                CarregaViewData();

                if (new ParcelaContrato().ObterTodos(_paramBase).Where(x => x.contratoitem_ID == obj.id).Count() != 0)
                {
                    ViewBag.msg = "Impossível excluir, existe uma Parcela associada a esse Item de Contrato";
                    return View(obj);
                }
                else
                {
                    ContratoItem ct = new ContratoItem();
                    ContratoItem obj2 = ct.ObterPorId(obj.id, _paramBase);
                    ct.Exclui(obj.id, _paramBase);

                    ViewBag.msg = "Item de Contrato excluído com sucesso";

                    return RedirectToAction("Edit", "ContratoNew", new { id = obj2.contrato_id });
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
                ContratoItem obj = new ContratoItem();
                ContratoItem ct = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(ct);
                idContrato = ct.contrato_id;
                CarregaViewData();

                return View(ct);
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
                CarregaViewData();
                ContratoItem obj = new ContratoItem();
                ContratoItem ct = obj.ObterPorId(ID, _paramBase);
                return View(ct);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ListasDetail(JqGridRequest request)
        {
            
            int totalRecords = 0;

            int ident = int.Parse(Request.QueryString["id"]);

            totalRecords = new ParcelaContrato().ObterTodos(_paramBase).Where(p => p.ContratoItem.contrato_id == ident).Count();

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

            List<ParcelaContrato> lista = new List<ParcelaContrato>();

            lista = new ParcelaContrato().ObterTodos(_paramBase).Where(p => p.ContratoItem.contrato_id == ident).OrderBy(p => p.data).Skip(12 * request.PageIndex).Take(12).ToList();

            foreach (var item in lista)
            {
                string numContrato = "";
                if (item.ContratoItem != null)
                    numContrato = item.ContratoItem.Contrato.contrato;

                string status = "";
                if (item.statusParcela != null)
                    status = item.statusParcela.status;

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    numContrato,
                    item.parcela,
                    item.descricao,
                    item.data.Date.ToString("dd/MM/yyyy"),
                    item.valor.ToString("0.00"),
                    status
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }


    }
}
