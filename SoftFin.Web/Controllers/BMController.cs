using SoftFin.Web.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SoftFin.Web.Controllers
{
    public class BMController : BaseController
    {
        #region Public
        //[HttpPost]
        //public override JsonResult TotalizadorDash(int? id)
        //{
        //    base.TotalizadorDash(id);
        //    var soma = new BancoMovimento().ObterEntreData(DataInicial, DataFinal).Sum(x => x.valor).ToString("n");
        //    return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult Index()
        {
            return View();
        }

        public class DtoFiltro
        {
            public int? banco_id { get; set; }
            public DateTime? dataIni { get; set; }
            public DateTime? dataFim { get; set; }
            public decimal? valorIni { get; set; }
            public decimal? valorFim { get; set; }
        }


        public JsonResult ObterTodos(DtoFiltro dtoFiltro)
        {

            var objs = new BancoMovimento().IQObterTodosQuery(_paramBase);

            if (dtoFiltro.banco_id != null)
                objs = objs.Where(p => p.banco_id == dtoFiltro.banco_id);

            if (dtoFiltro.dataIni != null)
            {
                DateTime dataAux = new DateTime(dtoFiltro.dataIni.Value.Year
                                                , dtoFiltro.dataIni.Value.Month
                                                , dtoFiltro.dataIni.Value.Day, 0,0,0);
                objs = objs.Where(p => p.data >= dataAux);
            }

            if (dtoFiltro.dataFim != null)
            {
                DateTime dataAux = new DateTime(dtoFiltro.dataFim.Value.Year
                                                , dtoFiltro.dataFim.Value.Month
                                                , dtoFiltro.dataFim.Value.Day, 0,0,0).AddDays(1);
                objs = objs.Where(p => p.data <= dataAux);
            }

            if (dtoFiltro.valorIni != null)
                objs = objs.Where(p => p.valor >= dtoFiltro.valorIni);

            if (dtoFiltro.valorFim != null)
                objs = objs.Where(p => p.valor <= dtoFiltro.valorFim);

            var objs2 = objs.Include(p => p.NotaFiscal.OrdemVenda).ToList();


            return Json(objs2.Select(p => new
            {
                data = p.data.ToString("o"),
                p.historico,
                descricao = p.PlanoDeConta.codigo + " " + p.PlanoDeConta.descricao,
                p.banco_id,
                banco = p.Banco.nomeBanco + " - " + p.Banco.agencia + " - " + p.Banco.contaCorrente,
                p.valor,
                p.OrigemMovimento.Modulo,
                TipoMovimento = p.TipoMovimento.Descricao,
                p.UnidadeNegocio_id,
                pessoa = "",
                NFES = "",
                CPAG = "",
                p.id,
                numeroNfse = (p.NotaFiscal == null) ? "" : p.NotaFiscal.numeroNfse.ToString(),
                numeroNfe = (p.NotaFiscal == null) ? "" : p.NotaFiscal.numeroNfe.ToString(),
                Numero = (p.NotaFiscal == null) ? "" : (p.NotaFiscal.OrdemVenda == null) ? "" : p.NotaFiscal.OrdemVenda.Numero.ToString()
            }), JsonRequestBehavior.AllowGet);


        }

        private string montaNFES(BancoMovimento item)
        {
            if (item.NotaFiscal != null)
            {
                if (item.NotaFiscal.numeroNfse != null)
                {
                    return item.NotaFiscal.numeroNfse.Value.ToString();
                    
                }
            }
            else if (item.Recebimento != null)
            {
                return item.Recebimento.notaFiscal.numeroNfse.Value.ToString();
            }
            return "";
        }

        private string montaCPAG(BancoMovimento item)
        {
            
            if (item.DocumentoPagarParcela != null)
            {
                return item.DocumentoPagarParcela.DocumentoPagarMestre.numeroDocumento.ToString();
            }
            else if (item.Pagamento != null)
            {
                return item.Pagamento.DocumentoPagarParcela.DocumentoPagarMestre.numeroDocumento.ToString();
            }
            return "";
        }

        public JsonResult DetalheBM(int id)
        {
            var item = new BancoMovimento().ObterPorId(id);
            var razaoconsultaAux = "";

            if (item.NotaFiscal != null)
            {
                if (item.NotaFiscal.numeroNfse != null)
                {
                    razaoconsultaAux = item.NotaFiscal.NotaFiscalPessoaTomador.razao;
                }
            }
            else if (item.Recebimento != null)
            {
                razaoconsultaAux = item.Recebimento.notaFiscal.NotaFiscalPessoaTomador.razao;
            }

            if (item.DocumentoPagarParcela != null)
            {
                razaoconsultaAux = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
            }
            else if (item.Pagamento != null)
            {
                razaoconsultaAux = item.Pagamento.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.nome;
            }
            return Json(new
            {
                cpagconsulta = montaCPAG(item),
                nfconsulta = montaNFES(item),
                razaoconsulta = razaoconsultaAux
            }, JsonRequestBehavior.AllowGet);

        }

        private NotaFiscal BuscaNota(BancoMovimento p)
        {
            return new NotaFiscal().ObterPorOV(p.id);
        }
        
        public JsonResult ObterPorId(int id)
        {
            var obj = new BancoMovimento().ObterPorId(id);
            if (obj == null)
            {
                obj = new BancoMovimento();
                obj.data = DateTime.Now;
            }
            if (obj.id == 0)
            {
                return Json(new
                {
                    estab = _estab,
                    data = obj.data.ToString("o"),
                    obj.historico,
                    obj.banco_id,
                    obj.planoDeConta_id,
                    obj.tipoDeMovimento_id,
                    obj.tipoDeDocumento_id,
                    obj.origemmovimento_id,
                    obj.UnidadeNegocio_id,
                    obj.valor,

                    obj.id
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    estab = _estab,
                    data = obj.data.ToString("o"),
                    obj.historico,
                    banco_id = obj.banco_id.ToString(),
                    planoDeConta_id = obj.planoDeConta_id.ToString(),
                    obj.tipoDeMovimento_id,
                    obj.tipoDeDocumento_id,
                    obj.origemmovimento_id,
                    obj.PlanoDeConta.descricao,
                    obj.UnidadeNegocio_id,
                    obj.valor,
                    obj.OrigemMovimento.Modulo,

                    obj.id
                }, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult ObterUnidadeNegocios()
        {
            var objs = new UnidadeNegocio().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.unidade
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterBanco()
        {
            var objs = new Banco().CarregaBancoGeral(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterIndicadorMovimento()
        {
            var objs = new List<SelectListItem>();
            objs.Add(new SelectListItem() { Value = "E", Text = String.Format("Entrada") });
            objs.Add(new SelectListItem() { Value = "S", Text = String.Format("Saída") });
            var listret = new SelectList(objs, "Value", "Text");
            
            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPlanoDeContas()
        {
            var objs = new PlanoDeConta().ObterTodosTipoA(); 

            return Json(objs.Select(p => new
            {
                Value = p.Value,
                Text = p.Text
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTipoDocumento()
        {
            var objs = new TipoDocumento().ObterTodos();

            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTipoMovimento()
        {
            var objs = new TipoMovimento().ObterTodos(_paramBase);

            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterOrigemMovimento()
        {
            var objs = new OrigemMovimento().ObterTodos(_paramBase).Where(x => x.Tipo == "M");

            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.Modulo
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult verificaDuplicidade(BancoMovimento obj)
        {
            var objs = new BancoMovimento().ObterTodosDataValor(obj.data, obj.valor,_paramBase);

            if (objs.Count() > 0)
            {
                if (objs.Count() ==  1)
                {
                    if (objs.FirstOrDefault().id != obj.id)
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Atenção! Já existem lançamentos neste dia com o mesmo valor, deseja continuar ?" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Atenção! Já existem lançamentos neste dia com o mesmo valor, deseja continuar ?" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { CDStatus = "OK", DSMessage = "" }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult Salvar(BancoMovimento obj)
        {
            try
            {

                if (obj.estab != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var objErros = obj.Validar(ModelState);
                if (obj.banco_id == 0)
                {
                    objErros.Add("Informe o banco");
                }
                if (obj.origemmovimento_id == 0)
                {
                    objErros.Add("Informe a origem do movimento");
                }
                if (obj.planoDeConta_id == 0)
                {
                    objErros.Add("Informe o plano de contas");
                }
                if (obj.tipoDeMovimento_id == 0)
                {
                    objErros.Add("Informe o tipo de movimento");
                }

                if (obj.tipoDeDocumento_id == 0)
                {
                    objErros.Add("Informe o tipo de documento");
                }
                if (string.IsNullOrWhiteSpace(obj.historico))
                {
                    objErros.Add("Informe a Descrição");
                }
                

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.id == 0)
                {
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInclusao = DateTime.Now;
                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Banco Movimento incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Banco Movimento já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    obj.usuarioalteracaoid = _usuarioobj.id;
                    obj.dataAlteracao = DateTime.Now;
                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Banco Movimento alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(BancoMovimento obj)
        {

            try
            {

                string Erro = "";
                if (obj.Excluir(obj.id, ref Erro,_paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Banco Movimento excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Banco Movimento" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
    }
}
