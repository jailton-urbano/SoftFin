using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class EstoqueController : BaseController
    {
        //
        // GET: /Estoque/

        public ActionResult Index()
        {
            return View();
        }

        public class dtoListaProduto
        {
            public int id { get; set; }
            public string descricao { get; set; }
            public decimal saldo { get; set; }
            public decimal valor { get; set; }
            public string unidadeMedida { get; set; }
        }

        //Busca Produto e calcula saldo
        public JsonResult ObterTodos()
        {
            try
            {

                var objs = new ItemProdutoServico().ObterTodos(_paramBase);
                var itensReturno = new List<dtoListaProduto>();

                foreach (var item in objs)
                {
                    var itemAux = new dtoListaProduto();

                    itemAux.id = item.id;
                    itemAux.descricao = item.descricao;
                    itemAux.unidadeMedida = item.unidadeMedida;

                    var movtos = new EstoqueMovtoItem().ObterPorIdProduto(item.id, _paramBase);

                    var entradas = movtos.Where(p => p.EstoqueMovto.TipoMovto == "E").Sum(p => p.quantidade);
                    var saidas = movtos.Where(p => p.EstoqueMovto.TipoMovto == "S").Sum(p => p.quantidade);

                    var valorentradas = movtos.Where(p => p.EstoqueMovto.TipoMovto == "E").Sum(p => p.valorTotal);
                    var valorsaidas = movtos.Where(p => p.EstoqueMovto.TipoMovto == "S").Sum(p => p.valorTotal);


                    itemAux.saldo = entradas - saidas;
                    itemAux.valor = valorentradas - valorsaidas; 

                    itensReturno.Add(itemAux);
                }


                return Json(new { CDStatus = "OK", DSMessage = "", objs = itensReturno }, JsonRequestBehavior.AllowGet); 

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message },JsonRequestBehavior.AllowGet ); 
            }
        }

        //Lista de Pessoas
        public JsonResult ListaPessoas()
        {



            try
            {

                var objs = new Pessoa().ObterTodos(_paramBase);

                return Json(new
                {
                    CDStatus = "OK",
                    DSMessage = "",
                    objs = objs.Select(p => new
                    {
                        Value = p.id.ToString(),
                        Text = p.nome
                    })
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ListaCategorias()
        {
            try
            {
                return Json(new { CDStatus = "OK", DSMessage = "", objs = new CategoriaItemProdutoServico().ObterTodos(_paramBase) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ListaTabelas()
        {
            try
            {

                var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);

                return Json(new
                {
                    CDStatus = "OK",
                    DSMessage = "",
                    objs = objs.Select(p => new
                        {
                            Value = p.id.ToString(),
                            Text = p.descricao
                        })
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Lista Produto Combo
        public JsonResult ListaProdutos(int idTabela)
        {
            try
            {

                var objs = new PrecoItemProdutoServico().ObterTodos(_paramBase).Where(p => p.TabelaPrecoItemProdutoServico_ID == idTabela).ToList();
                
                return Json(new
                {
                    CDStatus = "OK",
                    DSMessage = "",
                    objs = objs.Select(p => new
                    {
                        Value = p.ItemProdutoServico.id,
                        Text = p.ItemProdutoServico.descricao,
                        Price = p.preco,
                        Unit = p.ItemProdutoServico.unidadeMedida
                    })
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Carrega Preco
        public JsonResult CarregaPreco()
        {
            try
            {
                return Json(new { CDStatus = "OK", DSMessage = "", objs = new List<object>() }, JsonRequestBehavior.AllowGet); 

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message },JsonRequestBehavior.AllowGet ); 
            }
        }
        //Adiciona Lancamento
        public JsonResult Salvar(dtoSalva entidade)
        {
            EstoqueMovto estoqueMovto = new EstoqueMovto();
            EstoqueMovtoItem estoqueMovtoItem = new EstoqueMovtoItem();



            try
            {
                estoqueMovto.estabelecimento_id = _estab;
                estoqueMovto.pessoas_id = new Pessoa().ObterTodos(_paramBase).Where(p => p.nome == entidade.pessoa).First().id;
                estoqueMovto.TipoMovto = entidade.tipomov;
                estoqueMovto.ValorTotal = Math.Round(entidade.quantidade * entidade.valorUnitario,2);
                estoqueMovto.usuarioinclusaoid = _usuarioobj.id;
                estoqueMovto.descricao = entidade.descricao;
                estoqueMovto.dataInclusao = DateTime.Now;

                estoqueMovto.Incluir(estoqueMovto, _paramBase);

                estoqueMovtoItem.estoqueMovto_id = estoqueMovto.id;
                estoqueMovtoItem.itemProdutoServico_id = entidade.produtoid;
                estoqueMovtoItem.quantidade = entidade.quantidade;
                estoqueMovtoItem.valorUnitario = entidade.valorUnitario;
                estoqueMovtoItem.valorTotal = Math.Round(entidade.quantidade * entidade.valorUnitario, 2);
                estoqueMovtoItem.Incluir(estoqueMovtoItem, _paramBase);


                return ObterTodos(); 

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message },JsonRequestBehavior.AllowGet ); 
            }
        }

        public class dtoSalva
        {
            public string pessoa { get; set; }
            public string tipomov { get; set; }
            public int tabelaid { get; set; }
            public int produtoid { get; set; }
            public int quantidade { get; set; }
            public decimal valorUnitario { get; set; }
            public string descricao { get; set; }

        }

        public JsonResult Detalhe(int produtoid)
        {
            try
            {

                var objs = new EstoqueMovtoItem().ObterPorIdProduto(produtoid, _paramBase).ToList().OrderBy(p => p.EstoqueMovto.dataInclusao);

                return Json(new
                {
                    CDStatus = "OK",
                    DSMessage = "",
                    objs = objs.Select(p => new
                    {
                        pessoa = p.EstoqueMovto.Pessoa.nome,
                        data = p.EstoqueMovto.dataInclusao.Value.ToString("o"),
                        tipomov = p.EstoqueMovto.TipoMovto,
                        quantidade = p.quantidade,
                        valorUnitario = p.valorUnitario,
                        valorTotal = p.valorTotal,
                        descricao = p.EstoqueMovto.descricao
                    })
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
