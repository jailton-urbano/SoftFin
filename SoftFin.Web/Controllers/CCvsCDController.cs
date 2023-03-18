using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SoftFin.Web.Controllers
{
    public class CCvsCDController : BaseController
    {
        // GET: CCvsCD
        public ActionResult Index()
        {
            return View();
        }

        //[OutputCache(Duration = 0, Location = OutputCacheLocation.Server, VaryByParam = "none")]
        public JsonResult ObterTodos()
        {
            DbControle db = new DbControle();

            var objs = new ContaContabil().ObterTodos(_paramBase, db);

            return Json(objs.Select(obj => new
            {
                obj.empresa_id,
                contaContabil_id = obj.id,
                ccds = string.Format("{0} - {1}", obj.codigo, obj.descricao),
                planoDeContas_id = BuscaId(obj.id, db),
                cdds = BuscaDs(obj.id, db)
            }), JsonRequestBehavior.AllowGet);
        }

        private string BuscaDs(int id, DbControle db)
        {
            var ds = new ContaContabilCategoriaDespesa().ObterPorCC(id,_paramBase, db);

            if (ds == null)
                return "";
            else
                return string.Format("{0} - {1}", ds.PlanoDeConta.codigo, ds.PlanoDeConta.descricao);
        }

        private int BuscaId(int id, DbControle db)
        {
            var ds = new ContaContabilCategoriaDespesa().ObterPorCC(id, _paramBase, db);

            if (ds == null)
                return 0;
            else
                return ds.planoDeContas_id;
        }

        public ActionResult Salvar(List<ContaContabilCategoriaDespesa> objs)
        {
            if (objs == null)
                return Json(new { CDMessage = "NOK", DSMessage = "Erro rotina 0001" }, JsonRequestBehavior.AllowGet);

            if (objs.Count() == 0)
                return Json(new { CDMessage = "NOK", DSMessage = "Carregue a tabela de conta contabil." }, JsonRequestBehavior.AllowGet);

            if (objs.First().empresa_id != _empresa)
                return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

            try
            {
                DbControle db = new DbControle();

                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    var ccds = new ContaContabilCategoriaDespesa().ObterTodos(_paramBase);
                    foreach (var item in ccds)
                    {
                        var erro = "";
                        item.Excluir(ref erro,_paramBase);
                        if (erro != "")
                        {
                            dbcxtransaction.Rollback();
                            throw new Exception("Erro ao remover CCvsCD");
                        }
                    }

                    foreach (var item in objs)
                    {
                        if (item.planoDeContas_id != 0)
                            item.Incluir(_paramBase);
                    }
                    dbcxtransaction.Commit();
                }

                return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}