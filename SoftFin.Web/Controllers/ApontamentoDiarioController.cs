using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ApontamentoDiarioController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos(Filtro filtro)
        {

            var objs = new ApontamentoDiario().ObterTodosUsuario(_paramBase.usuario_id,_paramBase);

            filtro.Data = SoftFin.Utils.UtilSoftFin.TiraHora(filtro.Data);
            objs = objs.Where(p => p.data == filtro.Data).ToList();

            return Json(objs.Select(p => new
            {
                p.id,
                p.apontador_id,
                p.aprovador_id,
                p.atividade_id,
                data = p.data.ToString("o"),
                dataAprovado = (p.dataAprovado != null) ? p.dataAprovado.Value.ToString("o") : "",
                p.DescricaoAprovacao,
                p.historico,
                p.qtdHoras,
                p.qtdHorasRestantes,
                p.SituacaoAprovacao,
                p.Atividade.Projeto.nomeProjeto,
                p.Atividade.descricao
            }), JsonRequestBehavior.AllowGet);
        }

        public class Filtro
        {
            public DateTime Data { get; set; }
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new ApontamentoDiario().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new ApontamentoDiario();
                obj.apontador_id = _paramBase.usuario_id;
                obj.data = DateTime.Now;
                //obj.empresa_id = _empresa ;
                //obj.estabelecimento_id = _estab;
            }

            return Json(new
            {
                obj.id,
                obj.apontador_id,
                obj.aprovador_id,
                obj.atividade_id,
                data = obj.data.ToString("o"),
                dataAprovado = (obj.dataAprovado != null) ? obj.dataAprovado.Value.ToString("o") : "",
                obj.DescricaoAprovacao,
                obj.historico,
                obj.qtdHoras,
                obj.qtdHorasRestantes,
                obj.SituacaoAprovacao,
                Projeto_id = (obj.Atividade == null) ? 0 : obj.Atividade.projeto_id}, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(ApontamentoDiario obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.atividade_id == 0)
                {
                    objErros.Add("Informe a atividade");
                }

                if (obj.data == null)
                {
                    objErros.Add("Informe a data inicial");
                }
                else
                {
                    obj.data = SoftFin.Utils.UtilSoftFin.TiraHora(obj.data);
                }
                if (obj.apontador_id == 0)
                {
                    objErros.Add("Informe o  usuário");
                }
                
                if (obj.qtdHoras == 0)
                {
                    objErros.Add("Informe o a quantidade de horas");
                }

                if (obj.historico == "")
                {
                    objErros.Add("Informe o historico");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new
                    {
                        CDStatus = "NOK",
                        Errors = objErros,
                        DSMessage = "Verifique os dados informados"
                    });
                }

                var atividade = new Atividade().ObterPorId(obj.atividade_id,_paramBase);


                if (atividade.Projeto.estabelecimento_id != _estab)
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


        public JsonResult Excluir(ApontamentoDiario obj)
        {

            try
            {
                var atividade = new Atividade().ObterPorId(obj.atividade_id, _paramBase);


                if (atividade.Projeto.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);


                var erro = "";
                if (new ApontamentoDiario().Excluir(obj.id, ref erro, _paramBase))
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



        public JsonResult ObterAtividades(int projeto_id)
        {
            var objs = new Atividade().ObterTodos(_paramBase).
                Where(p => p.projeto_id == projeto_id).OrderBy(p => p.descricao);
            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao
            }), JsonRequestBehavior.AllowGet);
        }
    }
}