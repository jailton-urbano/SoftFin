using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class DPController : BaseController
    {
        // GET: DP
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos(Filtro filtro)
        {

            var objs = new DespesaPermitida().ObterTodos(_paramBase);

            if (filtro.Projeto_id != null)
                objs = objs.Where(p => p.projeto_id == filtro.Projeto_id).ToList();


            return Json(objs.Select(p => new
            {
                p.id,
                p.aprovador_id,
                p.cobravel, // Grid
                p.descricao, // Grid
                p.estabelecimento_id,
                p.projeto_id,
                p.reembolsavel, // Grid Formato Sim/Não
                p.tipodespesa_id,
                p.usarpadrao, // Grid Formato Sim/Não
                p.valorLimite, // Grid Formato 0,00
                p.valorPadrao // Grid Formato 0,00
            }), JsonRequestBehavior.AllowGet);
        }

        public class Filtro
        {
            public int? Projeto_id { get; set; }
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new DespesaPermitida().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new DespesaPermitida();
                //obj.empresa_id = _empresa ;
                obj.estabelecimento_id = _estab;
            }

            return Json(new
            {
                obj.id,
                obj.aprovador_id, // Select
                obj.cobravel, // Grid Select Sim/Não
                obj.descricao, // Grid Text
                obj.estabelecimento_id,
                obj.projeto_id, // Select
                obj.reembolsavel, // Grid  Select Formato Sim/Não
                obj.tipodespesa_id, // Select
                obj.usarpadrao, // Grid Select  Formato Sim/Não
                obj.valorLimite, // Grid Text Formato 0,00
                obj.valorPadrao // Grid Text  ormato 0,00
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(DespesaPermitida obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.projeto_id == 0)
                {
                    objErros.Add("Informe o Projeto");
                }
                if (obj.aprovador_id == 0)
                {
                    objErros.Add("Informe o Aprovador");
                }

                if (obj.tipodespesa_id == 0)
                {
                    objErros.Add("Informe o Tipo Despesa");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        Errors = objErros,
                        DSMessage = "Verifique os campos digitados"
                    });
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


        public JsonResult Excluir(DespesaPermitida obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                var erro = "";
                var funcionou = new DespesaPermitida().Excluir(obj.id, ref erro, _paramBase);

                if ((funcionou) && (erro == ""))
                {
                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (erro == "")
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = erro }, JsonRequestBehavior.AllowGet);
                    }
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