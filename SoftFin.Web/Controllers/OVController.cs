using SoftFin.Web.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class OVController : BaseController
    {
        #region Public
        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new OrdemVenda().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valor).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + soma }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Excel()
        {
            var obj = new OrdemVenda();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["descricao"] = item.descricao;
                myExport["valor"] = item.valor;
                myExport["data"] = item.data;
                myExport["parcelaContrato_ID"] = item.parcelaContrato_ID;
                myExport["statusParcela_ID"] = item.statusParcela_ID;
                myExport["unidadeNegocio_ID"] = item.unidadeNegocio_ID;
                myExport["ParcelaContrato"] = item.ParcelaContrato;
                myExport["statusParcela"] = item.statusParcela;
                myExport["UnidadeNegocio"] = item.UnidadeNegocio;
                myExport["pessoas_ID"] = item.pessoas_ID;
                myExport["Pessoa"] = item.Pessoa;
                myExport["estabelecimento_id"] = item.estabelecimento_id;
                myExport["Estabelecimento"] = item.Estabelecimento;
                myExport["usuarioAutorizador_id"] = item.usuarioAutorizador_id;
                myExport["usuarioAutorizador"] = item.usuarioAutorizador;
                myExport["dataAutorizacao"] = item.dataAutorizacao;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "OrdemVenda.csv");
        }
        public class Filtro
        {
            public DateTime dataIni { get; set; }
            public DateTime dataFim { get; set; }
        }
        public JsonResult ObterTodos(Filtro filtro)
        {
            var objs = new OrdemVenda().ObterEntreData(filtro.dataIni, filtro.dataFim, _paramBase);


            return Json(objs.Select(p => new
            {
               data = p.data.ToString("o"),
               dataAutorizacao = (p.dataAutorizacao == null) ? "" : p.dataAutorizacao.Value.ToString("o"),
               p.descricao,
               p.estabelecimento_id,
               p.id,
               p.itemProdutoServico_ID,
               p.Numero,
               p.parcelaContrato_ID,
               p.pessoas_ID,
               p.statusParcela_ID,
               p.tabelaPreco_ID,
               p.unidadeNegocio_ID,
               p.usuarioAutorizador_id,
               p.valor,
               parcela = p.parcelaContrato_ID == null ? null : p.ParcelaContrato.parcela.ToString(),
               descricaoparcela = p.parcelaContrato_ID == null ? null : p.ParcelaContrato.descricao,
               nome = p.Pessoa.nome,
               unidade = p.UnidadeNegocio.unidade,
               situacao = p.statusParcela.status,
               autSituacao = (p.usuarioAutorizador_id == null) ? "Pendente Aprovaçao": "Aprovado",
               autNome = (p.usuarioAutorizador == null) ? null : p.usuarioAutorizador.nome,
               autData = (p.dataAutorizacao == null) ? null : p.dataAutorizacao.Value.ToString("o"),
               nota = (BuscaNota(p) == null) ? null : BuscaNota(p).numeroNfse
            }), JsonRequestBehavior.AllowGet);
        }

        private NotaFiscal BuscaNota(OrdemVenda p)
        {
            return new NotaFiscal().ObterPorOV(p.id);
        }
        
        public JsonResult ObterPorId(int id)
        {
            var obj = new OrdemVenda().ObterPorId(id);
            if (obj == null)
            {
                obj = new OrdemVenda();
                obj.estabelecimento_id = _estab;
                obj.data = DateTime.Now;
                obj.statusParcela_ID = new StatusParcela().ObterTodos().Where(p => p.status == "Pendente").First().id;
            }
            if (obj.id == 0)
            {
                return Json(new
                {
                    data = obj.data.ToString("o"),
                    dataAutorizacao = (obj.dataAutorizacao == null) ? "" : obj.dataAutorizacao.Value.ToString("o"),
                    obj.descricao,
                    obj.estabelecimento_id,
                    obj.id,
                    obj.itemProdutoServico_ID,
                    obj.Numero,
                    obj.parcelaContrato_ID,
                    obj.pessoas_ID,
                    obj.pessoanome,
                    obj.statusParcela_ID,
                    obj.tabelaPreco_ID,
                    obj.unidadeNegocio_ID,
                    obj.usuarioAutorizador_id,
                    obj.valor,
                    obj.TipoFaturamento

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    data = obj.data.ToString("o"),
                    dataAutorizacao = (obj.dataAutorizacao == null) ? "" : obj.dataAutorizacao.Value.ToString("o"),
                    obj.descricao,
                    obj.estabelecimento_id,
                    obj.id,
                    obj.itemProdutoServico_ID,
                    obj.Numero,
                    obj.parcelaContrato_ID,
                    obj.pessoas_ID,
                    obj.statusParcela_ID,
                    obj.tabelaPreco_ID,
                    obj.unidadeNegocio_ID,
                    obj.usuarioAutorizador_id,
                    obj.valor,
                    pessoanome = obj.Pessoa.nome + ", " + obj.Pessoa.cnpj,
                    obj.TipoFaturamento
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


        public JsonResult ObterItemProdutoServicos()
        {
            var objs = new ItemProdutoServico().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterTabelaPrecoItemProdutoServicos()
        {
            var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterStatus(bool ispendente)
        {

            var status = new StatusParcela();
            var objs = new List<StatusParcela>();
            if (ispendente)
                objs = status.ObterTodos().Where(p => p.status == "Pendente").ToList();
            else
                objs = status.ObterTodos().ToList();

            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.status
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
        public JsonResult ObterPessoas()
        {
            var objs = new Pessoa().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                p.id,
                nome = p.nome + ", " + p.cnpj
            }), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Salvar(OrdemVenda obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                if (obj.pessoanome == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Cliente não encontrado" }, JsonRequestBehavior.AllowGet);

                if (obj.pessoanome.Split(',').Count() != 2)
                    return Json(new { CDMessage = "NOK", DSMessage = "Cliente não encontrado" }, JsonRequestBehavior.AllowGet);


                var nomePessoa = obj.pessoanome.Split(',')[0];
                var cnpjPessoa = obj.pessoanome.Split(',')[1];
                var pessoa = new Pessoa().ObterPorNomeCNPJ(nomePessoa, cnpjPessoa, _paramBase);


                if (pessoa == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Cliente não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.pessoas_ID = pessoa.id;


                if (obj.estabelecimento_id != _estab)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        DSMessage = "Estabelecimento inválido, saia do sistema (troca entre abas do navegador)",
                    });

                }


                OrdemVenda tp = new OrdemVenda();
                int ov = 0;

                if (obj.id == 0)
                {
                    tp.usuarioinclusaoid = _usuarioobj.id;
                    tp.dataInclusao = DateTime.Now;
                    if (tp.Incluir(obj, ref ov, _paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Ordem Venda número " + ov.ToString() + " incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Ordem Venda já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    tp.usuarioalteracaoid = _usuarioobj.id;
                    tp.dataAlteracao = DateTime.Now;
                    tp.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Ordem venda alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(OrdemVenda obj)
        {

            try
            {

                string Erro = "";
                if (obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Ordem de venda excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Ordem de Venda" }, JsonRequestBehavior.AllowGet);
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
