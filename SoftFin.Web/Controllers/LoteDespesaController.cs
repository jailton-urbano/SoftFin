using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LoteDespesaController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listas(JqGridRequest request)
        {
            int totalRecords = 0;
            LoteDespesa obj = new LoteDespesa();
            var objs = new LoteDespesa().ObterTodos(_paramBase);
  
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

                string ND = "";
                if (item.NotadeDebito != null)
                    ND = item.NotadeDebito.numero.ToString();

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.codigo,
                    item.Data.ToShortDateString(),
                    item.TipoLoteDespesa.descricao,
                    item.Historico,
                    item.ValorLote,
                    item.SituacaoLoteDespesa.descricao,
                    ND
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            CarregaViewData();
            return View();
        }

        public class RetornoLoteDespesa
        {
            public int id { get; set; }
            public string usuario { get; set; }
            public DateTime data { get; set; }
            public string dataS { get; set; }
            public string projeto { get; set; }
            public string tipodespesa { get; set; }
            public int tipoLote { get; set; }
            public string descricao { get; set; }
            public Decimal valor { get; set; }
            public string valorS { get; set; }
            public int idProjeto { get; set; }
            public int idTipoDespesa { get; set; }
            public string cliente { get; set; }
            public int? loteCobranca { get; set; }
            public int? loteReembolso { get; set; }
            public int? loteAdiantamento { get; set; }

        }

        private void CarregaViewData()
        {
            ViewData["usuario"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["tipoLote"] = new SelectList(new TipoLoteDespesa().ObterTodos(_paramBase), "id", "descricao");
            ViewData["DataInicial"] = DateTime.Now.ToShortDateString();
            ViewData["DataFinal"] = DateTime.Now.ToShortDateString();
            ViewData["colaborador"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["cliente"] = new SelectList(new Pessoa().ObterCliente(_paramBase), "id", "nome");
            ViewData["projeto"] = new SelectList(new Projeto().ObterTodos(_paramBase), "id", "nomeProjeto");
            ViewData["tipoDespesa"] = new SelectList(new TipoDespesa().ObterTodos(_paramBase), "id", "descricao");
            ViewData["aprovador"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase), "id", "nome");
            ViewData["lote"] = new SelectList(new LoteDespesa().ObterTodos(_paramBase), "id", "lote");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Pesquisa(string dataInicial, string dataFinal, int tipoLote)
        {
            var banco = new DbControle();
            var user = Acesso.UsuarioLogado();
            int estab = _paramBase.estab_id;

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);
            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var loteDespesa = new List<RetornoLoteDespesa>();
            loteDespesa = (from despesa in banco.Despesa
                            join t in banco.TipoDespesa on despesa.tipoDespesa_id equals t.id
                            where despesa.Data >= DataInicial && despesa.Data <= DataFinal &&
                            despesa.estabelecimento_id == estab
                                select new RetornoLoteDespesa
                                {
                                    id = despesa.id,
                                    usuario = despesa.Colaborador.nome,
                                    data = despesa.Data,
                                    projeto = despesa.Projeto.nomeProjeto,
                                    tipodespesa = despesa.TipoDespesa.descricao,
                                    tipoLote = tipoLote,
                                    descricao = despesa.descricao,
                                    valor = despesa.valor,
                                    idProjeto = despesa.projeto_id,
                                    idTipoDespesa = despesa.tipoDespesa_id,
                                    loteAdiantamento = despesa.loteAdiantamento_id,
                                    loteReembolso = despesa.loteReembolso_id,
                                    loteCobranca = despesa.loteCobranca_id

                                }).OrderBy(s => s.usuario).ToList();

            var permitida = new DespesaPermitida();

            if (tipoLote == 1) //Lote de Cobrança
            {
                for (int i = 0; i < loteDespesa.Count; i++)
                {
                    //Converte DateTime to String
                    loteDespesa[i].dataS = loteDespesa[i].data.ToString("dd/MM/yyyy");
                    //Converte Decimal to String
                    loteDespesa[i].valorS = loteDespesa[i].valor.ToString("n");

                    //Identificar se a despesa é Cobrável ou não
                    if (permitida.ValidaDespesaCobravel(loteDespesa[i].idProjeto, loteDespesa[i].idTipoDespesa, _paramBase) == true)
                    {
                        //Marca despesa como Cobrável
                        if (loteDespesa[i].loteCobranca == null)
                        {
                            loteDespesa[i].tipoLote = tipoLote;
                        }
                        else
                        {
                            loteDespesa[i].tipoLote = 0;
                        }
                    }
                    else
                    {
                        loteDespesa[i].tipoLote = 0;
                    }
                };
            }

            if (tipoLote == 2) //Lote de Reembolso
            {
                for (int i = 0; i < loteDespesa.Count; i++)
                {
                    //Converte DateTime to String
                    loteDespesa[i].dataS = loteDespesa[i].data.ToString("dd/MM/yyyy");
                    //Converte Decimal to String
                    loteDespesa[i].valorS = loteDespesa[i].valor.ToString("n");

                    //Identificar se a despesa é reembolsável ou não
                    if (permitida.ValidaDespesaReembolsavel(loteDespesa[i].idProjeto, loteDespesa[i].idTipoDespesa, _paramBase) == true)
                    {
                        //Marca despesa como Reembolsável
                        if (loteDespesa[i].loteReembolso == null)
                        {
                            loteDespesa[i].tipoLote = tipoLote;
                        }
                        else
                        {
                            loteDespesa[i].tipoLote = 0;
                        }
                    }
                    else
                    {
                        loteDespesa[i].tipoLote = 0;
                    }
                };
            }

            if (tipoLote == 3) //Lote de Adiantamento
            {
                for (int i = 0; i < loteDespesa.Count; i++)
                {
                    //Converte DateTime to String
                    loteDespesa[i].dataS = loteDespesa[i].data.ToString("dd/MM/yyyy");
                    //Converte Decimal to String
                    loteDespesa[i].valorS = loteDespesa[i].valor.ToString("n");

                    //Identificar se a despesa é adiantamento ou não
                    if (permitida.ValidaDespesaAdiantamento(loteDespesa[i].idProjeto, loteDespesa[i].idTipoDespesa, _paramBase) == true)
                    {
                        //Marca despesa como Adiantamento
                        if (loteDespesa[i].loteAdiantamento == null)
                        {
                            loteDespesa[i].tipoLote = tipoLote;
                        }
                        else
                        {
                            loteDespesa[i].tipoLote = 0;
                        }
                    }
                    else
                    {
                        loteDespesa[i].tipoLote = 0;
                    }
                };
            }

            //Remove tipoLote = 0
            loteDespesa.RemoveAll(x => x.tipoLote == 0);

            ViewData["tipoLote"] = new SelectList(new TipoLoteDespesa().ObterTodos(_paramBase), "id", "descricao");

            return Json(loteDespesa, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(FormCollection obs, int tipoLoteDespesa_id)
        {
                int estab = _paramBase.estab_id;
                int id = 0;
                int count = 0;
                int idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                string msgextra = "";
                decimal valorLote = 0;

                DbControle db = new DbControle(); //Contexto para o Lote
                //Cria lote de reembolso
                var lote = new LoteDespesa();
                var ultimoLote = lote.ObterUltimo(_paramBase);
                lote.codigo = ultimoLote + 1;
                lote.Data = DateTime.Now;
                lote.estabelecimento_id = estab;
                lote.situacaoLoteDespesa_id = 1;
                if (tipoLoteDespesa_id == 1)
                {
                    lote.Historico = "Lote Cobrança";
                }
                if (tipoLoteDespesa_id == 2)
                {
                    lote.Historico = "Lote Reembolso";
                }
                if (tipoLoteDespesa_id == 3)
                {
                    lote.Historico = "Lote Adiantamento";
                }
                lote.tipoLoteDespesa_id = tipoLoteDespesa_id;
                lote.nd_id = null;
                db.Set<LoteDespesa>().Add(lote);
                db.SaveChanges(); //Salva o Lote

                DbControle db2 = new DbControle(); //Contexto para as Despesas que serão atreladas ao Lote

                using (var dbcxtransaction = db2.Database.BeginTransaction())
                {
                    var novoLote = db2.LoteDespesa.Where(x => x.codigo == lote.codigo).FirstOrDefault();
                    foreach (var item in obs.AllKeys)
                    {
                        if (item.Contains("despesaID"))
                        {
                            id = int.Parse(item.Replace("despesaID", ""));
                            var despesa = db2.Despesa.Where(x => x.id == id).ToList().FirstOrDefault();
                            if (tipoLoteDespesa_id == 1)
                            {
                                despesa.loteCobranca_id = novoLote.id;
                            }
                            if (tipoLoteDespesa_id == 2)
                            {
                                despesa.loteReembolso_id = novoLote.id;
                            }
                            if (tipoLoteDespesa_id == 3)
                            {
                                despesa.loteAdiantamento_id = novoLote.id;
                            }
                            count = +1;
                            valorLote = valorLote + despesa.valor;
                        }
                    }
                    if (count > 0)
                    {
                        novoLote.ValorLote = valorLote;
                        db2.SaveChanges();
                        dbcxtransaction.Commit();
                        ViewBag.msg = "Lote gerado com sucesso." + msgextra;
                        CarregaViewData();
                        return View();
                    }
                    else
                    {
                        //Remove o Lote caso não tenha despesas para gerar
                        var novoLote2 = db2.LoteDespesa.Where(x => x.codigo == lote.codigo).FirstOrDefault();
                        db2.Set<LoteDespesa>().Remove(novoLote2);
                        db2.SaveChanges();
                        dbcxtransaction.Commit();
                        ViewBag.msg = "Nenhuma despesa selecionada." + msgextra;
                        CarregaViewData();
                        return View();
                    }
                }
        }

        public ActionResult Detail(int ID)
        {
            var despesa = new LoteDespesa().ObterPorId(ID, _paramBase);
            return View(despesa);
        }

        [HttpPost]
        public ActionResult getDespesaDetalhes(int ID)
        {
            ViewBag.usuario = Acesso.UsuarioLogado();
            ViewBag.perfil = Acesso.PerfilLogado();
            int estab = _paramBase.estab_id;

            var loteDespesa = new LoteDespesa().ObterPorId(ID, _paramBase);

            var banco = new DbControle();
            var despesas = new List<RetornoLoteDespesa>();

            despesas = (from despesa in banco.Despesa
                           join t in banco.TipoDespesa on despesa.tipoDespesa_id equals t.id
                           where (  despesa.loteAdiantamento_id ==  loteDespesa.id || 
                                    despesa.loteCobranca_id ==  loteDespesa.id || 
                                    despesa.loteReembolso_id ==  loteDespesa.id )
                            &&  despesa.estabelecimento_id == estab

                           select new RetornoLoteDespesa
                           {
                               id = despesa.id,
                               usuario = despesa.Colaborador.nome,
                               data = despesa.Data,
                               projeto = despesa.Projeto.nomeProjeto,
                               tipodespesa = despesa.TipoDespesa.descricao,
                               tipoLote = despesa.tipoDespesa_id,
                               descricao = despesa.descricao,
                               valor = despesa.valor,
                               idProjeto = despesa.projeto_id,
                               idTipoDespesa = despesa.tipoDespesa_id,
                               cliente = despesa.Cliente.nome
                           }).OrderBy(s => s.usuario).ToList();

            for (int i = 0; i < despesas.Count; i++)
            {
                despesas[i].dataS = despesas[i].data.ToString("dd/MM/yyyy");
                despesas[i].valorS = despesas[i].valor.ToString("n");
            };

            return Json(despesas, JsonRequestBehavior.AllowGet);
        }

    }
}
