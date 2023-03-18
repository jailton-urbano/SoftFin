using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ProjetoDespesaController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos(Filtro filtro)
        {

            var objs = new Despesa().ObterTodos(_paramBase);

            if (filtro.Projeto_id != null)
                objs = objs.Where(p => p.projeto_id == filtro.Projeto_id).ToList();

            if (filtro.Colaborador_id != null)
                objs = objs.Where(p => p.colaborador_id == filtro.Colaborador_id).ToList();

            if (filtro.Cliente_id != null)
                objs = objs.Where(p => p.cliente_id == filtro.Cliente_id).ToList();

            if (filtro.TipoDespesa_id != null)
                objs = objs.Where(p => p.tipoDespesa_id == filtro.TipoDespesa_id).ToList();

            if (filtro.DataInicial != null)
                objs = objs.Where(p => p.Data >= filtro.DataInicial).ToList();

            if (filtro.DataFinal != null)
                objs = objs.Where(p => p.Data <= filtro.DataFinal).ToList();

            if (filtro.ValorInicial != null)
                objs = objs.Where(p => p.valor >= filtro.ValorInicial).ToList();

            if (filtro.ValorFinal != null)
                objs = objs.Where(p => p.valor <= filtro.ValorFinal).ToList();


            return Json(objs.Select(p => new
            {
                p.id,
                p.estabelecimento_id,
                p.colaborador_id,
                p.cliente_id,
                p.projeto_id,
                p.tipoDespesa_id,
                Data = p.Data.ToString("o"),
                dataAprovado = (p.dataAprovado == null) ? "" : p.dataAprovado.Value.ToString("o"),
                p.descricao,
                p.valor,
                p.aprovador_id,
                p.DescricaoAprovacao,
                p.SituacaoAprovacao,
                p.loteCobranca_id,
                p.loteReembolso_id,
                p.loteAdiantamento_id,
                colaborador_nome = p.Colaborador.nome,
                projeto_nome = p.Projeto.nomeProjeto,
                cliente_nome = p.Cliente.nome,
                tipodespesa_nome = p.TipoDespesa.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public class Filtro
        {
            public int? Colaborador_id { get; set; }
            public int? Cliente_id { get; set; }
            public int? Projeto_id { get; set; }
            public int? TipoDespesa_id { get; set; }
            public DateTime? DataInicial { get; set; }
            public DateTime? DataFinal { get; set; }
            public Decimal? ValorInicial { get; set; }
            public Decimal? ValorFinal { get; set; }
            public String Descricao { get; set; }
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new Despesa().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new Despesa();
                //obj.empresa_id = _empresa ;
                obj.estabelecimento_id = _estab;
            }

            return Json(new
            {
                obj.id,
                obj.estabelecimento_id,
                obj.colaborador_id,
                obj.cliente_id,
                obj.projeto_id,
                obj.tipoDespesa_id,
                Data = obj.Data.ToString("o"),
                dataAprovado = (obj.dataAprovado == null) ? "" : obj.dataAprovado.Value.ToString("o"),
                obj.descricao,
                obj.valor,
                obj.aprovador_id,
                obj.DescricaoAprovacao,
                obj.SituacaoAprovacao,
                obj.loteCobranca_id,
                obj.loteReembolso_id,
                obj.loteAdiantamento_id
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(Despesa obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.projeto_id == 0)
                {
                    objErros.Add("Informe o Projeto");
                }


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Errors = objErros,
                        DSMessage = "Verifique os campos digitados" });
                    }
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);





                if (obj.id == 0)
                {

                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(Despesa obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                if (new Banco().Excluir(obj.id, _paramBase))
                {
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


        public JsonResult ObterProjetos()
        {
            var objs = new Projeto().ObterTodos(_paramBase).Where(p => p.ativo == true).OrderBy(p => p.nomeProjeto);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nomeProjeto
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterColaboradores()
        {
            var objs = new Usuario().ObterTodosUsuariosAtivos(_paramBase).OrderBy(p => p.nome);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nome
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterClientes()
        {
            var objs = new Pessoa().ObterCliente(_paramBase).OrderBy(p => p.nome);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nome
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTipoDespesas()
        {
            var objs = new TipoDespesa().ObterTodos(_paramBase).OrderBy(p => p.descricao);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterAprovadores()
        {
            var objs = new Usuario().ObterTodosUsuariosAtivos(_paramBase).OrderBy(p => p.nome);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nome
            }), JsonRequestBehavior.AllowGet);
        }

    }
}