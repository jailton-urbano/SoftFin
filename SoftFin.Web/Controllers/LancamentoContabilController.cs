using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class LancamentoContabilController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ObterTodos()
        {
            var objs = new LancamentoContabil().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.id,
                obj.historico,
                obj.codigoLancamento,
                data = obj.data.ToString("o"),
                obj.estabelecimento_id,
                origem = obj.OrigemMovimento.Modulo,
                totDebito = obj.LancamentoContabilDetalhes.Where(p => p.DebitoCredito == "D").Sum(p => p.valor),
                totCredito = obj.LancamentoContabilDetalhes.Where(p => p.DebitoCredito == "C").Sum(p => p.valor),
                centrocustos = obj.UnidadeNegocio.unidade,
                LancamentoContabilDetalhes = obj.LancamentoContabilDetalhes.Select(p => new
                {
                    p.contaContabil_id,
                    contaContabil_desc = string.Format("{0} - {1}", p.ContaContabil.codigo, p.ContaContabil.descricao),
                    conta = p.ContaContabil.codigo + " - " + p.ContaContabil.descricao,
                    p.valor,
                    p.DebitoCredito,
                    p.id
                })
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObterConsulta(int IdCPAG, int idNF)
        {
            var objs = new List<LancamentoContabilDetalhe>();

            if (IdCPAG != 0)
            {
                objs = new LancamentoContabilDetalhe().ObterPorCPAG(IdCPAG,_paramBase, null);
            }
            if (idNF != 0)
            {
                objs = new LancamentoContabilDetalhe().ObterPorNotaFiscal(idNF, _paramBase, null);
            }

            return Json(objs.Select(obj => new
            {
                contaContabil = string.Format("{0} - {1}", obj.ContaContabil.codigo, obj.ContaContabil.descricao),
                debito = obj.DebitoCredito == "D" ? obj.valor : 0,
                credito = obj.DebitoCredito == "C" ? obj.valor : 0,
                //unidade = obj.UnidadeNegocio.unidade
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new LancamentoContabil().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new LancamentoContabil();
                obj.estabelecimento_id = _estab;
                obj.usuarioinclusaoid = _usuarioobj.id;
                obj.data = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                obj.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                obj.origemmovimento_id = new OrigemMovimento().TipoManual(_paramBase);
            }
            else
            {
                obj.usuarioalteracaoid = _usuarioobj.id;
                obj.dataAlteracao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
            }

            return Json(new
            {
                obj.id,
                obj.codigoLancamento,
                obj.historico,
                data = obj.data.ToString("o"),
                obj.estabelecimento_id,
                obj.usuarioinclusaoid,
                obj.usuarioalteracaoid,
                dataInclusao = (obj.dataInclusao == null) ? "" : obj.dataInclusao.Value.ToString("o"),
                dataAlteracao = (obj.dataAlteracao == null) ? "" : obj.dataAlteracao.Value.ToString("o"),
                obj.origemmovimento_id,
                obj.DocumentoPagarParcela_id,
                obj.UnidadeNegocio_ID,
                LancamentoContabilDetalhes = (obj.LancamentoContabilDetalhes.Select( p => new
                {
                    p.id,
                    p.lancamentoContabil_id,
                    p.contaContabil_id,
                    
                    p.valor,
                    p.DebitoCredito
                }
                ))
                
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(LancamentoContabil obj)
        {
            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var objErros = obj.Validar(ModelState);
                //if (obj.contaContabil_id == 0)
                //{
                //    objErros.Add("Informe a conta contabil");
                //}
                //if (obj.unidadeNegocio_ID == 0)
                //{
                //    objErros.Add("Informe a unidade/centro de custos");
                //}
                
                if (string.IsNullOrWhiteSpace(obj.historico))
                {
                    objErros.Add("informe o código o histórico");
                }
                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.id == 0)
                {
                    DbControle db = new DbControle();

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(_paramBase, db);
                        obj.codigoLancamento = numeroLcto;

                        if (obj.Incluir(_paramBase,db) == true)
                        {
                            dbcxtransaction.Commit();
                            return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dbcxtransaction.Rollback();
                            return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    DbControle db = new DbControle();

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        var dets = new LancamentoContabilDetalhe().ObterPorCapa(obj.id, _paramBase, db);

                        foreach (var item in dets)
                        {
                            string erro = "";
                            item.Excluir(item.id, ref erro, _paramBase, db);
                        }
                        dbcxtransaction.Commit();
                    }

                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        var y = obj.ObterPorId(obj.id, db, _paramBase);

                        y.codigoLancamento = obj.codigoLancamento;
                        y.data = obj.data;
                        y.dataAlteracao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                        y.dataInclusao = obj.dataInclusao;
                        y.DocumentoPagarParcela_id = obj.DocumentoPagarParcela_id;
                        y.estabelecimento_id = _estab;
                        y.historico = obj.historico;
                        y.notafiscal_id = obj.notafiscal_id;
                        y.origemmovimento_id = obj.origemmovimento_id;
                        y.pagamento_id = obj.pagamento_id;
                        y.recebimento_id = obj.recebimento_id;
                        y.UsuarioAlteracao = obj.UsuarioAlteracao;
                        y.usuarioinclusaoid = obj.usuarioinclusaoid;

                        y.Alterar(y, _paramBase,db);

                        foreach (var item in obj.LancamentoContabilDetalhes)
                        {
                            //item.id = 0;
                            item.lancamentoContabil_id = obj.id;
                            //db.Entry(item).State = EntityState.Added;
                            item.Incluir(_paramBase, db);
                        }
                        dbcxtransaction.Commit();
                    }

                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(LancamentoContabil obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var obj2 = new LancamentoContabil().ObterPorId(obj.id, _paramBase);
                if (obj2 == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Lançamento não encontrado" }, JsonRequestBehavior.AllowGet);



                string erro = "";
                if (new LancamentoContabil().Excluir(obj.id, ref erro, _paramBase))
                {
                    if (erro != "")
                        throw new Exception(erro);

                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult GerarArquivoDominio(DateTime ini, DateTime fim)
        {
            StringBuilder sb = new StringBuilder();

            var objs = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase, ini, fim);

            foreach (var item in objs)
            {
                sb.AppendFormat("{0}|", "6000");
                sb.AppendFormat("{0}|", "X");
                sb.AppendFormat("{0}|", item.codigoLancamento);
                sb.AppendFormat("{0}|", "");
                sb.AppendFormat("{0}|", "");
                sb.AppendLine();
                foreach (var item2 in item.LancamentoContabilDetalhes)
                {
                    sb.AppendFormat("{0}|", "6100");
                    sb.AppendFormat("{0}|", item.data.ToString("dd/MM/yyy"));
                    if (item2.DebitoCredito == "D")
                        sb.AppendFormat("{0}|", item2.ContaContabil.codigo);
                    else
                        sb.AppendFormat("{0}|", "");
                    if (item2.DebitoCredito == "C")
                        sb.AppendFormat("{0}|", item2.ContaContabil.codigo);
                    else
                        sb.AppendFormat("{0}|", "");
                    sb.AppendFormat("{0}|", item2.valor.ToString("0.00"));
                    sb.AppendFormat("{0}|", item.codigoLancamento);
                    sb.AppendFormat("{0}|", item.historico);
                    sb.AppendFormat("{0}|", _usuarioobj.nome);
                    sb.AppendFormat("{0}|", _estabobj.codigoEstabelecimentoContabil);
                    sb.AppendFormat("{0}|", "");
                    sb.AppendLine();
                }
            }
            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "REGISTRO_6000_6100_" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".txt");
        }


        public ActionResult GerarArquivoProSoft(DateTime ini, DateTime fim)
        {
            StringBuilder sb = new StringBuilder();

            var objs = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase, ini, fim);
            int contador = 0;
            foreach (var item in objs)
            {
                contador += 1;
                sb.AppendFormat("{0,-3}", "LCI"); //Constante "LC1"
                sb.AppendFormat("{0,-5},", contador.ToString("00000"));
                sb.Append(' ', 3); //BRANCOS
                sb.AppendFormat("{0,-1}", "1");
                sb.AppendFormat("{0,-8}", item.data.ToString("ddMMyyyy"));
                sb.AppendFormat("{0,-10}", item.codigoLancamento.ToString("0000000000"));
                sb.AppendFormat("{0,-5}", item.codigoLancamento.ToString("00000"));
                sb.AppendLine();
                foreach (var item2 in item.LancamentoContabilDetalhes)
                {
                    sb.AppendFormat("{0}|", "6100");
                    sb.AppendFormat("{0}|", item.data.ToString("dd/MM/yyy"));
                    if (item2.DebitoCredito == "D")
                        sb.AppendFormat("{0}|", item2.ContaContabil.codigo);
                    else
                        sb.AppendFormat("{0}|", "");
                    if (item2.DebitoCredito == "C")
                        sb.AppendFormat("{0}|", item2.ContaContabil.codigo);
                    else
                        sb.AppendFormat("{0}|", "");
                    sb.AppendFormat("{0}|", item2.valor.ToString("0.00"));
                    sb.AppendFormat("{0}|", item.codigoLancamento);
                    sb.AppendFormat("{0}|", item.historico);
                    sb.AppendFormat("{0}|", _usuarioobj.nome);
                    sb.AppendFormat("{0}|", _estabobj.codigoEstabelecimentoContabil);
                    sb.AppendFormat("{0}|", "");
                    sb.AppendLine();
                }
            }
            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "ProSoft_LC1" + DateTime.Now.ToString("ddMMyyyy") + ".txt");
        }

        public ActionResult GerarArquivoAdvanced(DateTime ini, DateTime fim)
        {
            StringBuilder sb = new StringBuilder();

            var objs = new LancamentoContabil().ObterTodosDataIniDataFim(_paramBase, ini, fim);
            int contador = 0;
            foreach (var item in objs)
            {
                contador += 1;
                sb.Append(_estabobj.codigoEstabelecimentoContabil + ",");
                sb.Append(contador.ToString() + ",");
                sb.Append(item.data.ToString("dd/MM/yyyy") + ",");

                foreach (var item2 in item.LancamentoContabilDetalhes)
                {
                    if (item2.DebitoCredito == "D")
                        sb.Append(item2.ContaContabil.codigo + ",");
                }
                foreach (var item2 in item.LancamentoContabilDetalhes)
                {
                    if (item2.DebitoCredito == "C")
                    {
                        sb.Append(item2.ContaContabil.codigo + ",");
                        sb.Append(item2.valor.ToString("n") + ",");
                    }
                }
                sb.Append("0" + ";");
                sb.AppendLine(item.historico.ToString() + ",");

            }
            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "ProAdvc_LC1" + DateTime.Now.ToString("ddMMyyyy") + ".txt");
        }


    }
}