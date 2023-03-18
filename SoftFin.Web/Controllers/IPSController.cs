using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class IPSController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos()
        {
            var objs = new ItemProdutoServico().ObterTodos(_paramBase);
            return Json(objs.Select(ips => new
            {
                ips.id,
                ips.empresa_id,
                ips.codigo,
                ips.descricao,
                ips.unidadeMedida,
                ips.ncm,
                ips.CategoriaItemProdutoServico_ID,
                ips.codigoBarrasEAN,
                ips.marca,
                ips.origem,
                ips.estoque,
                ips.custo,
                ips.margem,
                ips.precoVenda,
                ips.informacoesComplementares,
                ips.EXTIPI,
                ips.CEST,
                ips.pesoLiquido,
                ips.pesoBruto,
                ips.Ativo,
                ips.dataAlteracao,
                ips.dataInclusao,
                ips.usuarioinclusaoid,
                ips.usuarioalteracaoid
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterTodosTabela()
        {
            var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);
            return Json(objs.Select(ips => new
            {
                ips.id,
                ips.empresa_id,
                ips.descricao,
                ips.porcentagemlucropadrao,
                ativo = (ips.ativo == null) ? "SIM" : (ips.ativo == false) ? "NÃO" : "SIM" 

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterCategoria()
        {
            var objs = new CategoriaItemProdutoServico().ObterTodos(_paramBase);

            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterPorId(int id)
        {
            var ips = new ItemProdutoServico().ObterPorId(id, _paramBase);
            if (ips == null)
            {
                ips = new ItemProdutoServico();
                ips.empresa_id = _empresa;
                ips.dataInclusao = DateTime.Now;
            }

            return Json(new
            {
                ips.id,
                ips.empresa_id,
                ips.codigo,
                ips.descricao,
                ips.unidadeMedida,
                ips.ncm,
                ips.CategoriaItemProdutoServico_ID,
                ips.codigoBarrasEAN,
                ips.marca,
                ips.origem,
                ips.estoque,
                ips.custo,
                ips.margem,
                ips.precoVenda,
                ips.informacoesComplementares,
                ips.EXTIPI,
                ips.CEST,
                ips.pesoLiquido,
                ips.pesoBruto,
                ips.Ativo,
                dataAlteracao = (ips.dataAlteracao == null) ? null : ips.dataAlteracao.Value.ToString("o"),
                dataInclusao = (ips.dataInclusao == null) ? null : ips.dataInclusao.Value.ToString("o"),
                ips.usuarioinclusaoid,
                ips.usuarioalteracaoid,
                ips.valorTributos
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(ItemProdutoServico obj)
        {
            try
            {
                 
                var objErros = obj.Validar(ModelState);
                
                if (obj.CategoriaItemProdutoServico_ID == 0)
                {
                    objErros.Add("Informe a categória do Produto Serviço");
                }
                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                if (obj.id == 0)
                {
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInclusao = DateTime.Now;
                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Produto Serviço incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Produto Serviço já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    obj.usuarioalteracaoid = _usuarioobj.id;
                    obj.dataAlteracao = DateTime.Now;
                    obj.Alterar(obj,_paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Produto Serviço alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(int id)
        {

            try
            {

                string Erro = "";
                if (new ItemProdutoServico().Excluir(id, ref Erro, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Produto Serviço excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Produto Serviço" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }



        public JsonResult ObterTabelaPorId(int id)
        {
            var obs = new TabelaPrecoItemProdutoServico().ObterPorId(id, _paramBase);
            if (obs == null)
            {
                obs = new TabelaPrecoItemProdutoServico();
                obs.empresa_id = _empresa;

            }

            return Json(new
            {
                obs.id,
                obs.empresa_id,
                obs.porcentagemlucropadrao,
                obs.descricao,
                obs.usuarioinclusaoid,
                obs.usuarioalteracaoid,
                dataAlteracao = (obs.dataAlteracao == null) ? null : obs.dataAlteracao.Value.ToString("o"),
                dataInclusao = (obs.dataInclusao == null) ? null : obs.dataInclusao.Value.ToString("o"),
                obs.ativo


            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SalvarTabela(TabelaPrecoItemProdutoServico obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                if (obj.id == 0)
                {
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInclusao = DateTime.Now;
                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Tabela incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Tabela já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    obj.usuarioalteracaoid = _usuarioobj.id;
                    obj.dataAlteracao = DateTime.Now;
                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Produto Serviço alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ExcluirTabela(TabelaPrecoItemProdutoServico obj)
        {

            try
            {
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                string Erro = "";
                if (obj.Excluir(obj.id, ref Erro, _paramBase))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Tabela excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir a Tabela" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GerarRelacionamento(ItemProdutoServico itemProdutoServico)
        {
            var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);
            var retornos = new List<DtoRelaciona>();
            foreach (var item in objs)
            {
                bool relac = false;
                decimal porc = 0;
                string descricao = "";

                var ver = new PrecoItemProdutoServico().ObterPorTabelaEProduto(item.id, itemProdutoServico.id, _paramBase);

                if (ver  != null)
                {
                    relac = true;
                    porc = ver.preco;
                    descricao = ver.descricao;
                }
                else
                {
                    porc = item.porcentagemlucropadrao;
                    descricao = itemProdutoServico.descricao;
                }


                retornos.Add(new DtoRelaciona { 
                    idproduto = itemProdutoServico.id, 
                    idtabela = item.id, 
                    relaciona = relac, 
                    porcentagem = porc, 
                    tabela = item.descricao,
                    descricao = descricao,
                    custo = itemProdutoServico.custo
                });
            }
            return Json(retornos.Select(p => new { p.idproduto, p.idtabela, p.porcentagem, p.relaciona,p.descricao, p.tabela, p.custo }), JsonRequestBehavior.AllowGet);
        }



        public JsonResult SalvarRelacionamento(List<DtoRelaciona> dtoRelaciona, int idproduto)
        {
            try
            {
                var relacionamentos = new PrecoItemProdutoServico().ObterPorProduto(idproduto, _paramBase);
                foreach (var item in relacionamentos)
                {
                    string erro = "";
                    item.Excluir(item.id, ref erro, _paramBase);
                }
                if (dtoRelaciona != null)
                    foreach (var item in dtoRelaciona)
                    {
                        if (item.relaciona)
                        {
                            new PrecoItemProdutoServico().Incluir(new PrecoItemProdutoServico
                            {
                                empresa_id = _empresa,
                                descricao = item.descricao,
                                ItemProdutoServico_ID = item.idproduto,
                                preco = item.porcentagem,
                                TabelaPrecoItemProdutoServico_ID = item.idtabela
                            }, _paramBase);
                        }
                    }
                return Json(new { CDStatus = "OK", DSMessage = "Produto Serviço relacionado com sucesso" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }

        public class DtoRelaciona
        {
            public int idtabela { get; set; }
            public int idproduto { get; set; }
            public bool relaciona { get; set; }
            public decimal porcentagem { get; set; }
            public string tabela { get; set; }


            public string descricao { get; set; }

            public decimal? custo { get; set; }
        }

    }
}
