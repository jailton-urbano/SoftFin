using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ContratoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Excel()
        {

            Contrato obj = new Contrato();
            List<Contrato> lista = new Contrato().ObterTodos(_paramBase);

            CsvExport myExport = new CsvExport();
            foreach (var contrato in lista)
            {
                myExport.AddRow();
                myExport["Contrato"] = contrato.contrato;
                myExport["Descricao"] = contrato.descricao;
                myExport["Emissao"] = contrato.emissao;
                myExport["Cliente"] = contrato.Pessoa.nome;
                //myExport["Pedido"] = contrato.pedido;
                myExport["Prazo"] = contrato.prazo;
                //myExport["Status"] = contrato.ParcelaContratos;
                //myExport["Tipo"] = contrato.TipoContrato.tipo;
                //myExport["Unidade"] = contrato.UnidadeNegocio.unidade;
                myExport["Valor Total"] = contrato.valortotal;

            }


            //Exporta os dados para o Excel
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();

            //Abri o arquivo de retorno no Excel
            return File(myCsvData, "application/vnd.ms-excel", "Contrato.csv");
        }

        public JsonResult ObterContratos()
        {
            var objs = new Contrato().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                p.contrato,
                p.descricao,
                emissao = p.emissao.ToString("o"),
                p.estabelecimento_id,
                p.id,
                p.prazo,
                p.valortotal,
                pessoas_ID = 0,
                pessoa_nome = p.Pessoa.nome,
                QtdContratosItems = new ContratoItem().ObterTodos(_paramBase,p.id).Count() ,
                QtdParcelaNoMes = ParcelasaNoMes(p.id)
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListaMunicipios()
        {
            return Json(new Municipio().ObterTodos(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaClientes()
        {
            return Json(new Pessoa().ObterTodosPorTipoPessoa(62,_paramBase).ToList()
                .Select( p => new
                {
                    p.agenciaConta,
                    p.agenciaDigito,
                    p.aliquotaICMS,
                    p.bairro,
                    p.bancoConta,
                    p.CategoriaPessoa_ID,
                    p.ccm,
                    p.Celular,
                    p.cep,
                    p.CFOP,
                    p.cidade,
                    p.CNAE,
                    p.cnpj,
                    p.codigo,
                    p.complemento,
                    p.contaBancaria,
                    p.CST_CSOSN,
                    p.digitoContaBancaria,
                    p.eMail,
                    p.empresa_id,
                    p.endereco,
                    p.flgSegurado,
                    p.Foto,
                    p.id,
                    p.inscricao,
                    p.nome,
                    p.numero,
                    p.observacaoFatura,
                    p.perfilFamiliar,
                    p.perfilPessoal,
                    p.perfilProfissional,
                    p.razao,
                    p.suframa,
                    p.TelefoneFixo,
                    p.TipoEndereco_ID,
                    p.TipoPessoa_ID,
                    p.uf,
                    p.UnidadeNegocio_ID
                }
                ), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaVendedores()
        {
            return Json(new Pessoa().ObterTodosPorTipoPessoa(76,_paramBase).ToList().Select(p => new
                {
                    p.agenciaConta,
                    p.agenciaDigito,
                    p.aliquotaICMS,
                    p.bairro,
                    p.bancoConta,
                    p.CategoriaPessoa_ID,
                    p.ccm,
                    p.Celular,
                    p.cep,
                    p.CFOP,
                    p.cidade,
                    p.CNAE,
                    p.cnpj,
                    p.codigo,
                    p.complemento,
                    p.contaBancaria,
                    p.CST_CSOSN,
                    p.digitoContaBancaria,
                    p.eMail,
                    p.empresa_id,
                    p.endereco,
                    p.flgSegurado,
                    p.Foto,
                    p.id,
                    p.inscricao,
                    p.nome,
                    p.numero,
                    p.observacaoFatura,
                    p.perfilFamiliar,
                    p.perfilPessoal,
                    p.perfilProfissional,
                    p.razao,
                    p.suframa,
                    p.TelefoneFixo,
                    p.TipoEndereco_ID,
                    p.TipoPessoa_ID,
                    p.uf,
                    p.UnidadeNegocio_ID
                }), JsonRequestBehavior.AllowGet);
        }

        private int ParcelasaNoMes(int p)
        {


            DateTime mes = DateTime.Now;
            DateTime primeiroDia = DateTime.Parse("01/" + mes.ToString("MM/yyyy"));
            DateTime ultimoDiaMesAtual = primeiroDia.AddMonths(1).AddDays(-1);

            return new ParcelaContrato().ObterTodosContratos(p, primeiroDia, ultimoDiaMesAtual).Count();
        }

        public JsonResult ObterContratosPorId(int id)
        {
            var obj = new Contrato().ObterPorId(id,_paramBase);
            if (obj == null)
            {
                obj = new Contrato();
                obj.estabelecimento_id = _estab;
            }
            if (obj.id == 0)
            {
                return Json(new
                {
                    obj.contrato,
                    pessoas_ID = 0,
                    obj.descricao,
                    emissao = DateTime.Now.ToString("o"),
                    obj.estabelecimento_id,
                    obj.id,
                    obj.prazo,
                    obj.valortotal,
                    dataInclusao = (obj.dataInclusao == null) ? null : obj.dataInclusao.Value.ToString("o"),
                    dataAlteracao = (obj.dataAlteracao == null) ? null : obj.dataAlteracao.Value.ToString("o"),
                    obj.usuarioinclusaoid,
                    obj.usuarioalteracaoid,
                    pessoa_nome = "",
                    obj.Vendedor_id,
                    Vendedor_nome = (obj.Vendedor == null) ? null : obj.Vendedor.nome,
                    obj.MunicipioPrestador_id,
                    MunicipioPrestador_nome = (obj.MunicipioPrestador == null) ? null : obj.MunicipioPrestador.DESC_MUNICIPIO,
                    DataInicioVigencia = (obj.DataInicioVigencia == null) ? null : obj.DataInicioVigencia.Value.ToString("o"),
                    DataFinalVigencia = (obj.DataFinalVigencia == null) ? null : obj.DataFinalVigencia.Value.ToString("o")


                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    obj.contrato,
                    obj.descricao,
                    emissao = obj.emissao.ToString("o"),
                    obj.estabelecimento_id,
                    obj.id,
                    obj.prazo,
                    obj.valortotal,
                    obj.pessoas_ID,
                    dataInclusao = (obj.dataInclusao == null) ? null : obj.dataInclusao.Value.ToString("o"),
                    dataAlteracao = (obj.dataAlteracao == null) ? null : obj.dataAlteracao.Value.ToString("o"),
                    obj.usuarioinclusaoid,
                    obj.usuarioalteracaoid,
                    pessoa_nome = obj.Pessoa.nome + ", " + obj.Pessoa.cnpj
                    ,
                    obj.Vendedor_id,
                    Vendedor_nome = (obj.Vendedor == null) ? null : obj.Vendedor.nome,
                    obj.MunicipioPrestador_id,
                    MunicipioPrestador_nome = (obj.MunicipioPrestador == null) ? null : obj.MunicipioPrestador.DESC_MUNICIPIO,
                    DataInicioVigencia = (obj.DataInicioVigencia == null) ? null : obj.DataInicioVigencia.Value.ToString("o"),
                    DataFinalVigencia = (obj.DataFinalVigencia == null) ? null : obj.DataFinalVigencia.Value.ToString("o")
                }, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult ObterContratoItems(int id)
        {
            var objs = new ContratoItem().ObterTodos(_paramBase, id);


            return Json(objs.Select(p => new
            {
                p.id,
                p.contrato_id,
                p.itemProdutoServico_ID,
                p.pedido,
                p.tabelaPreco_ID,
                p.tipoContratos_ID,
                p.unidadeNegocio_ID,
                p.valor,
                dataInclusao = (p.dataInclusao == null) ? null : p.dataInclusao.Value.ToString("o"),
                dataAlteracao = (p.dataAlteracao == null) ? null : p.dataAlteracao.Value.ToString("o"),
                p.usuarioinclusaoid,
                p.usuarioalteracaoid,
                ListaParcelas = (p.ParcelaContratos == null) ? null : p.ParcelaContratos.Select(i => new { i.contratoitem_ID, data = i.data.ToString("o"), i.descricao, i.statusParcela_ID, i.valor, i.parcela, i.id, i.statusParcela.status, DataVencimento = (i.DataVencimento == null) ? null : i.DataVencimento.Value.ToString("o"), }),
                ListaItemUnidades = (p.ContratoItemUnidadeNegocios == null) ? null : p.ContratoItemUnidadeNegocios.Select(i => new { i.Id, i.Descricao, i.ContratoItem_Id, i.UnidadeNegocio_Id, i.Valor, Unidade_Desc = i.UnidadeNegocio.unidade})


            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterContratoItemsPorId(int id)
        {
            var obj = new ContratoItem().ObterPorId(id, _paramBase);

            if (obj == null)
            {
                obj = new ContratoItem();
            }
            return Json(new
            {
                obj.id,
                obj.contrato_id,
                obj.itemProdutoServico_ID,
                obj.pedido,
                obj.tabelaPreco_ID,
                obj.tipoContratos_ID,
                obj.unidadeNegocio_ID,
                obj.valor,
                dataInclusao = (obj.dataInclusao == null) ? null : obj.dataInclusao.Value.ToString("o"),
                dataAlteracao = (obj.dataAlteracao == null) ? null : obj.dataAlteracao.Value.ToString("o"),
                
                obj.usuarioinclusaoid,
                obj.usuarioalteracaoid,
                ListaParcelas = (obj.ParcelaContratos == null) ? null : obj.ParcelaContratos.Select(i => new { i.contratoitem_ID, data = i.data.ToString("o"), i.descricao, i.statusParcela_ID, i.valor, i.parcela, i.id, i.statusParcela.status, DataVencimento = (i.DataVencimento == null) ? null : i.DataVencimento.Value.ToString("o"), })
                ,
                ListaItemUnidades = (obj.ContratoItemUnidadeNegocios == null) ? null : obj.ContratoItemUnidadeNegocios.Select(i => new { i.Id, i.Descricao, i.ContratoItem_Id, i.UnidadeNegocio_Id, i.Valor, Unidade_Desc = i.UnidadeNegocio.unidade })


            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterParcelas(int id)
        {
            var objs = new ParcelaContrato().ObterTodos(_paramBase, id);


            return Json(objs.Select(p => new
            {
                p.id,
                p.contratoitem_ID,
                recorrente = false,
                data = p.data.ToString("o"),
                p.descricao,
                p.parcela,
                p.statusParcela_ID,
                status = p.statusParcela.status,
                p.codigoServicoEstabelecimento_id,
                banco_id = p.banco_id.ToString(),
                p.operacao_id,
                p.NFAutomatico,
                p.valor,
                dataInclusao = (p.dataInclusao == null) ? null : p.dataInclusao.Value.ToString("o"),
                dataAlteracao = (p.dataAlteracao == null) ? null : p.dataAlteracao.Value.ToString("o"),
                p.usuarioinclusaoid,
                p.usuarioalteracaoid,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterParcelasEmAberto(int id)
        {
            var objs = new ParcelaContrato().ObterTodos(_paramBase, id).Where(p => p.statusParcela.status == "Pendente");


            return Json(objs.Select(p => new
            {
                p.id,
                p.TipoFaturamento,
                DataVencimento = (p.DataVencimento == null) ? "" : p.DataVencimento.Value.ToString("o"),
                p.contratoitem_ID,
                recorrente = false,
                data = p.data.ToString("o"),
                p.descricao,
                p.parcela,
                p.Codigo,
                p.statusParcela_ID,
                status = p.statusParcela.status,
                p.codigoServicoEstabelecimento_id,
                banco_id = p.banco_id.ToString(),
                p.operacao_id,
                p.NFAutomatico,
                p.valor,
                dataInclusao = (p.dataInclusao == null) ? null : p.dataInclusao.Value.ToString("o"),
                dataAlteracao = (p.dataAlteracao == null) ? null : p.dataAlteracao.Value.ToString("o"),
                p.usuarioinclusaoid,
                p.usuarioalteracaoid,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterParcelaPorId(int id)
        {
            var obj = new ParcelaContrato().ObterPorId(id, _paramBase);

            if (obj == null)
            {
                obj = new ParcelaContrato();
                obj.data = DateTime.Now;
                obj.DataVencimento = DateTime.Now;
                obj.statusParcela = new StatusParcela().ObterPorId(4);
                obj.statusParcela_ID = 4;
                obj.recorrente = null;
                obj.NFAutomatico = false;
                var bc = new Banco().ObterPrincipal(_paramBase);
                if (bc != null)
                {
                    obj.banco_id = bc.id;
                }

            }
            return Json(new
            {
                obj.id,
                obj.contratoitem_ID,
                obj.recorrente ,
                data = obj.data.ToString("o"),
                DataVencimento = (obj.DataVencimento == null) ? null : obj.DataVencimento.Value.ToString("o"),
                obj.descricao,
                obj.parcela,
                obj.statusParcela_ID,
                status = obj.statusParcela.status,
                obj.valor,
                obj.codigoServicoEstabelecimento_id,
                banco_id = obj.banco_id.ToString(),
                obj.operacao_id,
                obj.NFAutomatico,
                dataInclusao = (obj.dataInclusao == null) ? null : obj.dataInclusao.Value.ToString("o"),
                dataAlteracao = (obj.dataAlteracao == null) ? null : obj.dataAlteracao.Value.ToString("o"),
                obj.usuarioinclusaoid,
                obj.usuarioalteracaoid,
                obj.Codigo,
                obj.TipoFaturamento
            }, JsonRequestBehavior.AllowGet);
        }

       


        [HttpPost]
        public JsonResult Salvar(Contrato obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                if (obj.pessoa_nome == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Cliente não encontrado" }, JsonRequestBehavior.AllowGet);

                //if (obj.Vendedor_id == null)
                //    return Json(new { CDMessage = "NOK", DSMessage = "Vendedor não encontrado" }, JsonRequestBehavior.AllowGet);


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros= objErros});
                }
                Contrato contrato = new Contrato();

                if (obj.id == 0)
                {
                    obj.estabelecimento_id = _estab;
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInclusao = DateTime.Now;

                    if (obj.Incluir(_paramBase) == true)
                    {
                        obj = obj.ObterPorId(obj.id,_paramBase);
                        return Json(new
                        {
                            CDStatus = "OK",
                            DSMessage = "Contrato salvo com sucesso",
                            Obj = new
                            {
                                obj.contrato,
                                obj.descricao,
                                emissao = obj.emissao.ToString("o"),
                                obj.estabelecimento_id,
                                obj.id,
                                obj.prazo,
                                obj.valortotal,
                                obj.pessoas_ID,
                                dataInclusao = (obj.dataInclusao == null) ? null : obj.dataInclusao.Value.ToString("o"),
                                dataAlteracao = (obj.dataAlteracao == null) ? null : obj.dataAlteracao.Value.ToString("o"),
                                obj.usuarioinclusaoid,
                                obj.usuarioalteracaoid,
                                pessoa_nome = obj.Pessoa.nome + ", " + ((obj.Pessoa.cnpj == null) ? "" : obj.Pessoa.cnpj),
                                
                                obj.Vendedor_id,
                                Vendedor_nome = (obj.Vendedor == null) ? null :  obj.Vendedor.nome,
                                obj.MunicipioPrestador_id,
                                MunicipioPrestador_nome = (obj.MunicipioPrestador == null) ? null : obj.MunicipioPrestador.DESC_MUNICIPIO,
                                DataInicioVigencia = (obj.DataInicioVigencia == null) ? null : obj.DataInicioVigencia.Value.ToString("o"),
                                DataFinalVigencia = (obj.DataFinalVigencia == null) ? null : obj.DataFinalVigencia.Value.ToString("o")
                            }
                        });
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Contrato já incluido" });
                    }

                }
                else
                {
                    obj.estabelecimento_id = _estab;
                    obj.usuarioalteracaoid = _usuarioobj.id;
                    obj.dataAlteracao = DateTime.Now;

                    if (obj.Alterar(obj,_paramBase) == true)
                    {
                        obj = obj.ObterPorId(obj.id,_paramBase);
                        return Json(new { CDStatus = "OK", DSMessage = "Contrato alterado com sucesso",
                                          Obj = new
                                          {
                                              obj.contrato,
                                              obj.descricao,
                                              emissao = obj.emissao.ToString("o"),
                                              obj.estabelecimento_id,
                                              obj.id,
                                              obj.prazo,
                                              obj.valortotal,
                                              obj.pessoas_ID,
                                              dataInclusao = (obj.dataInclusao == null) ? null : obj.dataInclusao.Value.ToString("o"),
                                              dataAlteracao = (obj.dataAlteracao == null) ? null : obj.dataAlteracao.Value.ToString("o"),
                                              obj.usuarioinclusaoid,
                                              obj.usuarioalteracaoid,
                                              pessoa_nome = obj.Pessoa.nome + ", " + ((obj.Pessoa.cnpj == null) ? "" : obj.Pessoa.cnpj),
                                              obj.Vendedor_id,
                                              Vendedor_nome = (obj.Vendedor == null) ? null : obj.Vendedor.nome,
                                              obj.MunicipioPrestador_id,
                                              MunicipioPrestador_nome = (obj.MunicipioPrestador == null) ? null : obj.MunicipioPrestador.DESC_MUNICIPIO,
                                              DataInicioVigencia = (obj.DataInicioVigencia == null) ? null : obj.DataInicioVigencia.Value.ToString("o"),
                                              DataFinalVigencia = (obj.DataFinalVigencia == null) ? null : obj.DataFinalVigencia.Value.ToString("o")
                                          }
                        });
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage="Erro, não consegui alterar o Contrato" });
                    }

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage=ex.Message.ToString() });

            }
        }

        [HttpPost]
        public JsonResult SalvarContratoItem(ContratoItem obj, List<ContratoItemUnidadeNegocio> listaItemUnidades)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                var db = new DbControle();

                if (obj.id == 0)
                {
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInclusao = DateTime.Now;

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        if (obj.Inclui(obj, _paramBase, db) == true)
                        {
                            foreach (var item in listaItemUnidades)
                            {
                                item.ContratoItem_Id = obj.id;
                                item.Incluir(item,  db);
                            }
                            dbcxtransaction.Commit();
                            return Json(new { CDStatus = "OK", DSMessage = "Item de Contrato incluído com sucesso" });
                        }
                        else
                        {
                            dbcxtransaction.Rollback();
                            return Json(new { CDStatus = "NOK", DSMessage = "Item de Contrato já incluido" });
                        }
                    }

                }
                else
                {
                    obj.usuarioalteracaoid = _usuarioobj.id;
                    obj.dataAlteracao = DateTime.Now;
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {

                        obj.Altera(obj, _paramBase, db);

                        var listaItemUnidadesAuv = new ContratoItemUnidadeNegocio().ObterTodos(_paramBase, db, obj.id);
                        foreach (var item in listaItemUnidadesAuv)
                        {
                            item.Excluir(item.Id, db);
                        }
                        foreach (var item in listaItemUnidades)
                        {
                            item.ContratoItem_Id = obj.id;
                            item.Incluir(item, db);
                        }
                        dbcxtransaction.Commit();
                    }

                    return Json(new { CDStatus = "OK", DSMessage = "Item de Contrato alterado com sucesso" });

                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });

            }
        }

        [HttpPost]
        public JsonResult SalvarParcela(ParcelaContrato obj, List<ContratoItemPedido> pedidos)
        {
            try
            {
                if (obj.recorrente == null)
                    obj.recorrente = false;

                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros= objErros});
                }
                Contrato contrato = new Contrato();

                if (obj.id == 0)
                {
                    obj.usuarioinclusaoid = _usuarioobj.id;
                    obj.dataInclusao = DateTime.Now;
                    if (!obj.IncluiParcela(obj,(obj.recorrente.Value) ? "true" :"false",_paramBase))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage="Parcela já incluida" });
                    }

                    return Json(new { CDStatus = "OK", DSMessage="Parcela incluído com sucesso" });
                }
                else
                {
                    DbControle db = new DbControle();
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        var cIPs = new ContratoItemPedido().ObterTodos(_paramBase, db, obj.id);
                        foreach (var item in cIPs)
                        {
                            item.Excluir(item.Id,db);
                        }


                        if (pedidos != null)
                        {
                            foreach (var item in pedidos)
                            {
                                item.ParcelaContrato_Id = obj.id;
                                item.Incluir(item, db);
                            }
                        }

                        obj.usuarioalteracaoid = _usuarioobj.id;
                        obj.dataAlteracao = DateTime.Now;
                        obj.AlteraParcela(obj, _paramBase,db);
                        dbcxtransaction.Commit();
                    }
                    return Json(new { CDStatus = "OK", DSMessage="Parcela alterada com sucesso" });
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage=ex.Message.ToString() });

            }
        }


        [HttpPost]
        public JsonResult SalvarParcelas(List<ParcelaContrato> objs)
        {
            try
            {
                foreach (var obj in objs)
                {

                    obj.recorrente = false;

                    var objErros = obj.Validar(ModelState);


                    if (objErros.Count() > 0)
                    {
                        return Json(new { CDStatus = "NOK", Erros = objErros });
                    }
                }

                DbControle db = new DbControle();
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    foreach (var obj in objs)
                    {

                    Contrato contrato = new Contrato();

                    if (obj.id == 0)
                    {
                        obj.usuarioinclusaoid = _usuarioobj.id;
                        obj.dataInclusao = DateTime.Now;
                        if (!obj.IncluiParcela(obj,"false", _paramBase))
                        {
                            dbcxtransaction.Rollback();
                            return Json(new { CDStatus = "NOK", DSMessage = "Parcela já incluida" });
                        }
                    }
                    else
                    {

                            var objAux = new ParcelaContrato().ObterPorId(obj.id, db, _paramBase);
                            objAux.banco_id = obj.banco_id;
                            objAux.data = obj.data;
                            objAux.DataVencimento = obj.DataVencimento;
                            objAux.descricao = obj.descricao;
                            objAux.NFAutomatico = obj.NFAutomatico;
                            objAux.operacao_id = obj.operacao_id;
                            objAux.parcela = obj.parcela;
                            objAux.TipoFaturamento = obj.TipoFaturamento;
                            objAux.valor = obj.valor;
                            objAux.codigoServicoEstabelecimento_id = obj.codigoServicoEstabelecimento_id;
                            objAux.usuarioalteracaoid = _usuarioobj.id;
                            objAux.dataAlteracao = DateTime.Now;
                            objAux.Codigo = obj.Codigo;
                            objAux.AlteraParcela(objAux, _paramBase, db);
                            
                        }
                        
                    }
                    dbcxtransaction.Commit();
                }
                return Json(new { CDStatus = "OK", DSMessage = "Parcelas alteradas com sucesso" });

            }


            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });

            }
        }

        public JsonResult ObterContratoItemPedido(int Id)
        {
            DbControle db = new DbControle();
            var cIPs = new ContratoItemPedido().ObterTodos(_paramBase, db, Id);
            return Json(cIPs.Select(p => new { p.Id, p.Pedido, p.Descricao, p.Valor, p.unidadenegocio_id, unidadedesc = p.UnidadeNegocio.unidade }) ,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Excluir(Contrato obj)
        {
            try
            {

                Contrato contrato = new Contrato();

                var erros = "";

                obj.Excluir(obj.id, ref erros,_paramBase);

                if (erros == "")
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Contrato excluido com sucesso" });
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Erro, não consegui excluir o Contrato, " + erros});
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });

            }
        }

        [HttpPost]
        public JsonResult ExcluirContratoItem(ContratoItem obj)
        {
            try
            {
                Contrato contrato = new Contrato().ObterPorId(obj.contrato_id,_paramBase);

                if (contrato == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Contrato não encontrado" }, JsonRequestBehavior.AllowGet);

                if (contrato.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);




                obj.Exclui(obj.id, _paramBase);

                return Json(new { CDStatus = "OK", DSMessage = "Contrato excluido com sucesso" });
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });

            }
        }

        [HttpPost]
        public JsonResult ExcluirParcelaContrato(ParcelaContrato obj)
        {
            try
            {
                ContratoItem contratoItem = new ContratoItem().ObterPorId(obj.contratoitem_ID, _paramBase);

                if (contratoItem == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Item de Contrato não encontrado" }, JsonRequestBehavior.AllowGet);


                Contrato contrato = new Contrato().ObterPorId(contratoItem.contrato_id,_paramBase);

                if (contrato == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Contrato não encontrado" }, JsonRequestBehavior.AllowGet);

                if (contrato.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                var erros = "";

                obj.ExcluiParcela(obj.id, ref erros);

                if (erros == "")
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Parcela Contrato excluido com sucesso" });
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Erro, não consegui excluir a Parcela. " + erros });
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString() });

            }
        }

        public JsonResult ObterPessoas()
        {
            var objs = new Pessoa().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                p.id,
                p.nome
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTipoContratos()
        {
            var objs = new TipoContrato().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.tipo
            }), JsonRequestBehavior.AllowGet);
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

        //public JsonResult ObterTipoContratos()
        //{
        //    var objs = new TipoContrato().ObterTodos();


        //    return Json(objs.Select(p => new
        //    {
        //        p.id,
        //        p.tipo
        //    }), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ObterTabelaPrecoItemProdutoServicos()
        {
            var objs = new TabelaPrecoItemProdutoServico().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value= p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterOperacoes()
        {
            var objs = new Operacao().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterCodigoServicos()
        {
            var objs = new CodigoServicoEstabelecimento().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.CodigoServicoMunicipio.codigo + " - " + p.CodigoServicoMunicipio.descricao
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


        public JsonResult Arquivos(int id)
        {
            return Json(new ContratoArquivo().ObterPorContrato(id,_paramBase).Select(p => new {p.arquivoOriginal, p.arquivoReal, p.descricao, p.id }));
        }

        [HttpPost]
        public JsonResult Upload(int id, string descricao, FormCollection formCollection)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, extenção não permitida" });
                    }
                }
            }

            var pessoa = new Pessoa();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo =  arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);

                    AzureStorage.UploadFile(caminhoArquivo,
                                "ContratoArquivo/" + id.ToString() + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DbControle();

                    var contratoArquivo = new ContratoArquivo();
                    contratoArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "ContratoArquivo/" + id.ToString() + "/" + nomearquivonovo;
                    contratoArquivo.arquivoOriginal = nomearquivonovo;
                    contratoArquivo.contrato_id = id;
                    contratoArquivo.descricao = descricao;
                    contratoArquivo.Salvar(_paramBase);
                }
            }
            return Json(new { CDStatus = "OK", DSMessage = "Arquivo salvo com suceso" });
        }


        public JsonResult RemoveArquivo(int id)
        {
            try
            {
                var contratoArquivo = new ContratoArquivo().ObterPorId(id,_paramBase);


                AzureStorage.DeleteFile(contratoArquivo.arquivoReal,
                                    "ContratoArquivo/" + contratoArquivo.contrato_id.ToString() + "/" + contratoArquivo.arquivoOriginal,
                                    ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                new ContratoArquivo().Excluir(id,_paramBase);

                return Json(new { CDStatus = "OK", DSMessage = "Excluido com sucesso" });
            }
            catch(Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message });
            }

        }


        [HttpPost]
        public JsonResult ImportCSVPedido(HttpPostedFileBase file, int Id)
        {
            var importacaoArquivo = new ImportacaoArquivo();

            try
            {
                var arquivo = Request.Files[0];


                var uploadPath = Server.MapPath("~/OFXTemp/");
                Directory.CreateDirectory(uploadPath);

                var nomeFile = "Upload_CSV_Pedido_" + _empresa + "_Estab_" + _estab + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + "_" + arquivo.FileName;
                var path = Path.Combine(Server.MapPath("~/OFXTemp/"), nomeFile);
                arquivo.SaveAs(path);
                var utf8 = System.Text.Encoding.UTF8;

                var lines = System.IO.File.ReadAllLines(path, utf8).Select(a => a.Split(';'));

                importacaoArquivo.TotalLinhas = lines.Count();

                var linhaCont = 1;
                var db = new DbControle();

                var cIPs = new ContratoItemPedido().ObterTodos(_paramBase, db, Id);
                foreach (var item in cIPs)
                {
                    item.Excluir(item.Id, db);
                }

                foreach (var item in lines)
                {
                    if (linhaCont == 1) //Pula o cabeçalho 
                    {
                        linhaCont += 1;
                        continue;
                    }

                    var erro = "";
                    var obj = new ContratoItemPedido();
                    if (item.Length == 3)
                    {
                        Convert(item, obj);
                        if (erro == "")
                        {
                            
                            obj.ParcelaContrato_Id = Id;
                            obj.Incluir(obj,new DbControle());
                            importacaoArquivo.TotalImportadas += 1;
                        }

                    }
                    else
                    {
                        erro = "Sua planilha deveria ter 3 colunas e foi encotrada " + item.Length.ToString() + " colunas.";
                    }
                    if (erro != "")
                    {
                        importacaoArquivo.TotalErros += 1;
                        importacaoArquivo.LinhasErros.Add("Linha " + linhaCont.ToString(), erro);
                    }

                    linhaCont += 1;
                }
            }
            catch (Exception ex)
            {
                importacaoArquivo.Situacao = "Erro durante importação";
                importacaoArquivo.Descricao = ex.Message;
            }

            if (importacaoArquivo.Situacao == null)
            {
                importacaoArquivo.Situacao = "OK";
                if (importacaoArquivo.LinhasErros.Count() > 0)
                {
                    importacaoArquivo.Situacao += ", mas com alguns problemas";
                }
            }

            return Json(importacaoArquivo,JsonRequestBehavior.AllowGet);
        }

        private string Convert(string[] obj, ContratoItemPedido item)
        {
            var erro = "";


            item.Pedido = obj[0];
            if (item.Pedido == "")
                erro += "Infome o Pedido;";
            item.Descricao = obj[1];

            decimal valor = 0;
            if (decimal.TryParse(obj[2],out valor))
                item.Valor = valor;
            else
                erro += "Infome o Valor do Pedido;";

            return erro;
        }


        public ActionResult ExcelCSVPedido(int Id)
        {
            var db = new DbControle();
            var cIPs = new ContratoItemPedido().ObterTodos(_paramBase, db, Id);
            CsvExport myExport = new CsvExport();
            
            foreach (var item in cIPs)
            {
                myExport.AddRow();
                myExport["Pedido"] = item.Pedido;
                myExport["Descrição"] = item.Descricao;
                myExport["Valor"] = item.Valor;
                
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "Pedido.csv");
        }
    }
}
