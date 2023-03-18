using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class AtividadeController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new Atividade();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["Projeto"] = item.Projeto.nomeProjeto;
                myExport["descricao"] = item.descricao;
                myExport["sequencia"] = item.sequencia;
                //if (item.Predescessora != null)
                //    myExport["predescessora"] = item.Predescessora.descricao;
                //if (item.Sucessora != null)
                //    myExport["sucessora"] = item.Sucessora.descricao;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "Atividade.csv");
        }
        public ActionResult Index()
        {
            CarregaViewData(0);
            var obj = new Atividade();
/*            obj.estabelecimento_id = _estab;
            obj.projeto_id = id;*/
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
            string Valordescricao = Request.QueryString["descricao"];
            string Valorprojeto_id = Request.QueryString["projeto_id"];
            int totalRecords = 0;
            Atividade obj = new Atividade();
            var objs = new Atividade().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valordescricao))
            {
                objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorprojeto_id))
            {
                int aux;
                int.TryParse(Valorprojeto_id, out aux);
                objs = objs.Where(p => p.projeto_id == aux).ToList();
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
                string predescessora = "";
                if (item.predescessora_id != null)
                {
                    predescessora = new Atividade().ObterPorId(item.predescessora_id.Value, _paramBase).descricao;
                }

                string sucessora = "";
                if (item.sucessora_id != null)
                {
                    sucessora = new Atividade().ObterPorId(item.sucessora_id.Value, _paramBase).descricao;
                }

                var dataIni = "";
                if (item.DataInicial != null)
                    dataIni = item.DataInicial.Value.ToShortDateString();

                var dataFin = "";
                if (item.DataFinal != null)
                    dataFin = item.DataFinal.Value.ToShortDateString();
                
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.Projeto.codigoProjeto + " - " + item.Projeto.nomeProjeto,
                    item.descricao,
                    predescessora,
                    sucessora,
                    item.sequencia,
                    dataIni,
                    dataFin,
                    item.qtdHoras
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        private static List<Atividade> Organiza(JqGridRequest request, List<Atividade> objs)
        {
            switch (request.SortingName)
            {
                case "estabelecimento_id":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.estabelecimento_id).ToList();
                    else
                        objs = objs.OrderBy(p => p.estabelecimento_id).ToList();
                    break;
                case "Estabelecimento":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Estabelecimento).ToList();
                    else
                        objs = objs.OrderBy(p => p.Estabelecimento).ToList();
                    break;
                case "descricao":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.descricao).ToList();
                    else
                        objs = objs.OrderBy(p => p.descricao).ToList();
                    break;
                case "sequencia":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.sequencia).ToList();
                    else
                        objs = objs.OrderBy(p => p.sequencia).ToList();
                    break;

                case "projeto_id":
                    if (request.SortingOrder == JqGridSortingOrders.Desc)
                        objs = objs.OrderByDescending(p => p.Projeto.nomeProjeto).ToList();
                    else
                        objs = objs.OrderBy(p => p.Projeto.nomeProjeto).ToList();
                    break;
             }
             return objs;
          }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AtividadeUsuarioView obj)
        {
            try
            {
                CarregaViewData(obj.projeto_id, obj); 

                obj.estabelecimento_id = _estab;

                if (ModelState.IsValid)
                {
                    var db = new DbControle();
                    var atividade = new Atividade();

                    atividade.DataInicial = obj.DataInicial;
                    atividade.DataFinal = obj.DataFinal;
                    atividade.descricao = obj.descricao;
                    atividade.predescessora_id = obj.predescessora_id;
                    atividade.projeto_id = obj.projeto_id;
                    atividade.qtdHoras = obj.qtdHoras;
                    atividade.sequencia = obj.sequencia;
                    atividade.sucessora_id = obj.sucessora_id;
                    atividade.estabelecimento_id = _estab;

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        if (atividade.Incluir(_paramBase, db))
                        {
                            SalvaItens(obj, db, atividade);
                            dbcxtransaction.Commit();
                            CarregaViewData(obj.projeto_id, obj, atividade.id); 

                            ViewBag.msg = "Incluído com sucesso";
                            return View(obj);
                        }
                        else
                        {
                            dbcxtransaction.Rollback();
                            ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                            return View(obj);
                        }
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
        public ActionResult Create(int id)
        {
            try
            {
                var obj = new AtividadeUsuarioView();
                obj.estabelecimento_id = _estab;
                obj.Projeto = new Projeto().ObterPorId(id, _paramBase);
                obj.projeto_id = obj.Projeto.id;
                CarregaViewData(obj.projeto_id, obj);





                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AtividadeUsuarioView obj)
        {
            try
            {
                CarregaViewData(obj.projeto_id);
                if (ModelState.IsValid)
                {
                    var db = new DbControle();
                    var atividade = new Atividade();

                    obj.estabelecimento_id = _estab;
                    atividade.DataInicial = obj.DataInicial;
                    atividade.DataFinal = obj.DataFinal;
                    atividade.descricao = obj.descricao;
                    atividade.predescessora_id = obj.predescessora_id;
                    atividade.projeto_id = obj.projeto_id;
                    atividade.qtdHoras = obj.qtdHoras;
                    atividade.sequencia = obj.sequencia;
                    atividade.sucessora_id = obj.sucessora_id;
                    atividade.id = obj.id;
                    atividade.estabelecimento_id = _estab;

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        if (atividade.Alterar(_paramBase,db))
                        {
                            DeletaItens(obj, db, atividade);
                            SalvaItens(obj, db, atividade);
                            dbcxtransaction.Commit();
                            CarregaViewData(obj.projeto_id, obj, atividade.id);

                            ViewBag.msg = "Alterado com sucesso";
                            return View(obj);
                        }
                        else
                        {
                            ViewBag.msg = "Impossivel alterar, registro excluído";
                            return View(obj);
                        }
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


        private void DeletaItens(AtividadeUsuarioView obj, DbControle db, Atividade atividade)
        {
            var atividadeUsuario = new AtividadeUsuario().ObterTodosPorAtividade(atividade.id, _paramBase);
            foreach (var item in atividadeUsuario )
            {
                new AtividadeUsuario().Excluir(item.id, _paramBase, db);
            }
        }


        private void SalvaItens(AtividadeUsuarioView obj, DbControle db, Atividade atividade)
        {
            if (obj.ListaUsuarioVal != null)
            {
                foreach (string item in obj.ListaUsuarioVal)
                {
                    var usuario = new Usuario().ObterTodosUsuariosAtivos(_paramBase).Where(p => p.codigo == item).FirstOrDefault();

                    if (usuario != null)
                    {
                        new AtividadeUsuario()
                        .Incluir(new AtividadeUsuario
                        {
                            atividade_id = atividade.id,
                            usuario_id = usuario.id
                        }, _paramBase, db);
                    }
                }
            }
        }
        public ActionResult Edit(int ID)
        {
            try
            {
                
                Atividade obj = new Atividade();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                AtividadeUsuarioView atividade = new AtividadeUsuarioView();

                atividade.DataInicial = obj.DataInicial;
                atividade.DataFinal = obj.DataFinal;
                atividade.descricao = obj.descricao;
                atividade.predescessora_id = obj.predescessora_id;
                atividade.projeto_id = obj.projeto_id;
                atividade.qtdHoras = obj.qtdHoras;
                atividade.sequencia = obj.sequencia;
                atividade.sucessora_id = obj.sucessora_id;
                atividade.id = obj.id;
                atividade.estabelecimento_id = _estab;



                CarregaViewData(obj.projeto_id,atividade, ID);
                return View(atividade);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AtividadeUsuarioView obj)
        {
            try
            {
                    var db = new DbControle();
                    var atividade = new Atividade();

                    obj.estabelecimento_id = _estab;
                    atividade.DataInicial = obj.DataInicial;
                    atividade.DataFinal = obj.DataFinal;
                    atividade.descricao = obj.descricao;
                    atividade.predescessora_id = obj.predescessora_id;
                    atividade.projeto_id = obj.projeto_id;
                    atividade.qtdHoras = obj.qtdHoras;
                    atividade.sequencia = obj.sequencia;
                    atividade.sucessora_id = obj.sucessora_id;
                    atividade.id = obj.id;
                    atividade.estabelecimento_id = _estab;

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        DeletaItens(obj, db, atividade);
                        if (atividade.Excluir(obj.id, _paramBase, db))
                        {
                            dbcxtransaction.Commit();
                            return RedirectToAction("/Index");
                        }
                        else
                        {
                            dbcxtransaction.Rollback();

                            ViewBag.msg = "Impossivel excluir";
                            return View(obj);
                        }
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

                Atividade obj = new Atividade();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                AtividadeUsuarioView atividade = new AtividadeUsuarioView();

                atividade.DataInicial = obj.DataInicial;
                atividade.DataFinal = obj.DataFinal;
                atividade.descricao = obj.descricao;
                atividade.predescessora_id = obj.predescessora_id;
                atividade.projeto_id = obj.projeto_id;
                atividade.qtdHoras = obj.qtdHoras;
                atividade.sequencia = obj.sequencia;
                atividade.sucessora_id = obj.sucessora_id;
                atividade.id = obj.id;
                atividade.estabelecimento_id = _estab;



                CarregaViewData(obj.projeto_id, atividade, ID);
                return View(atividade);
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

                Atividade obj = new Atividade();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                AtividadeUsuarioView atividade = new AtividadeUsuarioView();

                atividade.DataInicial = obj.DataInicial;
                atividade.DataFinal = obj.DataFinal;
                atividade.descricao = obj.descricao;
                atividade.predescessora_id = obj.predescessora_id;
                atividade.projeto_id = obj.projeto_id;
                atividade.qtdHoras = obj.qtdHoras;
                atividade.sequencia = obj.sequencia;
                atividade.sucessora_id = obj.sucessora_id;
                atividade.id = obj.id;
                atividade.estabelecimento_id = _estab;



                CarregaViewData(obj.projeto_id, atividade, ID);
                return View(atividade);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        private void CarregaViewData(int projetoid, AtividadeUsuarioView obj = null, int? atividadeid = null)
        {
            ViewData["predescessora"] = new SelectList(new Atividade().ObterTodos(_paramBase).Where(z => z.projeto_id == projetoid), "id", "descricao");
            ViewData["sucessora"] = new SelectList(new Atividade().ObterTodos(_paramBase).Where(z => z.projeto_id == projetoid), "id", "descricao");
            ViewData["projeto"] = new Projeto().CarregaProjeto(_paramBase);

            var atividadeUsuario = new AtividadeUsuario();

            if (atividadeid != null)
            {
                if (obj != null)
                {
                    var usus = atividadeUsuario.ObterTodosPorAtividade(atividadeid.Value, _paramBase);

                    obj.ListaUsuario = new Dictionary<string, bool>();
                    var objusus = new Usuario().ObterTodosUsuariosAtivos(_paramBase);
                    var projetosusu = new ProjetoUsuario().ObterTodosPorProjeto(projetoid, _paramBase).ToList();
  
                    objusus = (from a in objusus  
                                  join p in projetosusu on a.id equals p.usuario_id
                                  select a).ToList();
                                                
                    foreach (var item in objusus)
                    {
                        var existe = usus.Where(p => p.Usuario.codigo == item.codigo).FirstOrDefault();

                        obj.ListaUsuario.Add(item.codigo, existe != null);
                    }
                }
            }
            else
            {
                if (obj != null)
                {
                    obj.ListaUsuario = new Dictionary<string, bool>();
                    var objusus = new Usuario().ObterTodosUsuariosAtivos(_paramBase);
                    var projetosusu = new ProjetoUsuario().ObterTodosPorProjeto(projetoid, _paramBase, null);

                    objusus = (from a in objusus
                               join p in projetosusu on a.id equals p.usuario_id
                               select a).ToList();

                    foreach (var item in objusus)
                    {
                        obj.ListaUsuario.Add(item.codigo, false);
                    }
                }

            }

        }

        
    }
}
