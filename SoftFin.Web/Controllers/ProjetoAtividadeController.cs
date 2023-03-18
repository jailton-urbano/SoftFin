using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ProjetoAtividadeController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodos(Filtro filtro)
        {
            
            var objs = new Atividade().ObterTodos(_paramBase);

            objs = objs.Where(p => p.projeto_id == filtro.Projeto_id).ToList();

            return Json(objs.Select(p => new
            {
                p.id,
                DataFinal = (p.DataFinal == null) ? "": p.DataFinal.Value.ToString("o"),
                DataInicial = (p.DataInicial == null) ? "" : p.DataInicial.Value.ToString("o"),
                p.descricao,
                p.sequencia,
                p.projeto_id,
                p.predescessora_id,
                p.sucessora_id,
                p.qtdHoras,
                p.estabelecimento_id
            }), JsonRequestBehavior.AllowGet);
        }

        public class Filtro
        {
            public int Projeto_id { get; set; }
        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new Atividade().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new Atividade();
                //obj.empresa_id = _empresa ;
                obj.estabelecimento_id = _estab;
            }

            return Json(new
            {
                obj.id,
                DataFinal = (obj.DataFinal == null) ? "" : obj.DataFinal.Value.ToString("o"),
                DataInicial = (obj.DataInicial == null) ? "" : obj.DataInicial.Value.ToString("o"),
                obj.descricao,
                obj.sequencia,
                obj.projeto_id,
                obj.predescessora_id,
                obj.sucessora_id,
                obj.qtdHoras,
                obj.estabelecimento_id
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(Atividade obj)
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                if (obj.projeto_id == 0)
                {
                    objErros.Add("Informe o Projeto");
                }

                if (obj.DataInicial == null)
                {
                    objErros.Add("Informe a data inicial");
                }

                if (obj.descricao == null)
                {
                    objErros.Add("Informe a descrição");
                }

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Errors = objErros,
                        DSMessage = "Verifique os dados informados" });
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


        public JsonResult Excluir(Atividade obj)
        {

            try
            {
                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                if (new Atividade().Excluir(obj.id, _paramBase))
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