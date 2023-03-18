using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.Controllers
{
    public class PRController : BaseController
    {
        // GET: TB
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterTodosProcessos()
        {
            var objs = new Processo().ObterTodos(base._paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.IdEmpresa,
                obj.Ativo,
                obj.Codigo,
                obj.CodigoProcessoTemplate,
                obj.Contador,
                obj.Descricao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosAtividades(int idProcesso)
        {
            var objs = new Atividade().ObterTodos(idProcesso, base._paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.Ativo,
                obj.Codigo,
                obj.Descricao,
                obj.IdAtividadeTipo,
                AtividadeTipo_Nome = obj.AtividadeTipo.Descricao, 
                obj.IdProcesso,
                obj.Url

            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterTodosAtividadesCombo(int idProcesso)
        {
            var objs = new Atividade().ObterTodos(idProcesso, base._paramBase);
            return Json(objs.Select(obj => new
            {
                Value = obj.Id,
              
                Text = obj.Codigo + " - " + obj.Descricao
   

            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosPlanos(int idProcesso)
        {
            var objs = new AtividadePlano().ObterTodos(idProcesso, base._paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                Atividade_Desc = obj.Atividade.Codigo + " - " + obj.Atividade.Descricao,
                AtividadeEntrada_Desc = (obj.AtividadeEntrada == null) ? " " : obj.AtividadeEntrada.Codigo + " - " + obj.AtividadeEntrada.Descricao,
                obj.CondicaoEntrada,
                obj.ProcessoId,
                obj.AtividadeId,
                obj.AtividadeIdEntrada,
                obj.Inicial,
                obj.Final
                

            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosAtividadesFuncoes(int idAtividade)
        {
            var objs = new AtividadeFuncao().ObterTodos(idAtividade, _paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.IdFuncao,
                obj.IdAtividade,
                Funcao_Nome = obj.Funcao.Descricao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosAtividadeTipo()
        {
            var objs = new AtividadeTipo().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.Descricao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterTodosAtividadeVisao(int idAtividade)
        {
            var objs = new AtividadeVisao().ObterTodos(idAtividade, _paramBase);
            return Json(objs.Select(obj => new
            {
                obj.Id,
                obj.IdAtividade,
                obj.IdVisao,
                obj.Ordem,
                obj.Titulo,
                obj.Visao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterProcessosPorId(int id)
        {
            var obj = new Processo().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new Processo();
                obj.IdEmpresa = _paramBase.Empresa;
            }

            return Json(new
            {
                obj.Id,
                obj.IdEmpresa,
                obj.Ativo,
                obj.Codigo,
                obj.CodigoProcessoTemplate,
                obj.Contador,
                obj.Descricao
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SalvarProcesso(Processo obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.IdEmpresa != _paramBase.Empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                if (obj.Id == 0)
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ExcluirProcessos(Processo obj)
        {

            try
            {
                if (obj.IdEmpresa != _paramBase.Empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);
                string erro = "";

                if (new Processo().Excluir(obj.Id, ref erro, _paramBase))
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult ObterAtividadesPorId(int id)
        {
            var obj = new Atividade().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new Atividade();

            }

            return Json(new
            {
                obj.Id,
                obj.Ativo,
                obj.Codigo,
                obj.Descricao,
                obj.IdAtividadeTipo,
                AtividadeTipo_Nome = obj.AtividadeTipo.Descricao,
                obj.IdProcesso,
                obj.Url
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SalvarAtividade(Atividade obj, List<Funcao> funcoes, List<Visao> visaos )
        {
            try
            {

                var objErros = obj.Validar(ModelState);

                //if (objErros.Count() > 0)
                //{
                //    return Json(new { CDStatus = "NOK", Erros = objErros });
                //}

                if (obj.Id == 0)
                {
                    if (obj.Incluir(_paramBase) == true)
                    {
                        foreach (var item in funcoes)
                        {
                            new AtividadeFuncao { IdAtividade = obj.Id, IdFuncao = item.Id }.Incluir(_paramBase);
                        }

                        foreach (var item in visaos)
                        {
                            new AtividadeVisao { IdAtividade = obj.Id, IdVisao = item.Id }.Incluir(_paramBase);
                        }
                        return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var objFuncoes = new AtividadeFuncao().ObterTodos(obj.Id, _paramBase);
                    
                    foreach (var item in objFuncoes)
                    {
                        string erros = "";
                        item.Excluir(item.Id, ref erros, _paramBase);
                    }
                    foreach (var item in funcoes)
                    {
                        new AtividadeFuncao { IdAtividade = obj.Id, IdFuncao = item.Id }.Incluir(_paramBase);
                    }

                    var objVisaoes = new AtividadeVisao().ObterTodos(obj.Id, _paramBase);
                    foreach (var item in objVisaoes)
                    {
                        string erros = "";
                        item.Excluir(item.Id, ref erros, _paramBase);
                    }

                    foreach (var item in visaos)
                    {
                        new AtividadeVisao { IdAtividade = obj.Id, IdVisao = item.Id }.Incluir(_paramBase);
                    }

                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ExcluirAtividade(Atividade obj)
        {

            try
            {
                string erro = "";

                var objFuncoes = new AtividadeFuncao().ObterTodos(obj.Id, _paramBase);

                foreach (var item in objFuncoes)
                {
                    string erros = "";
                    item.Excluir(item.Id, ref erros, _paramBase);
                }

                var objVisaoes = new AtividadeVisao().ObterTodos(obj.Id, _paramBase);
                foreach (var item in objVisaoes)
                {
                    string erros = "";
                    item.Excluir(item.Id, ref erros, _paramBase);
                }
                if (new Atividade().Excluir(obj.Id, ref erro, _paramBase))
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult ObterAtividadeFuncaosPorId(int id)
        {
            var obj = new AtividadeFuncao().ObterPorId(id,null, _paramBase);
            if (obj == null)
            {
                obj = new AtividadeFuncao();
            }

            return Json(new
            {
                obj.Id,
                obj.IdFuncao,
                obj.IdAtividade,
                Funcao_Nome = obj.Funcao.Descricao
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SalvarAtividadeFuncaos(AtividadeFuncao obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.Id == 0)
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ExcluirAtividadeFuncaos(AtividadeFuncao obj)
        {

            try
            {

                string erro = "";

                if (new AtividadeFuncao().Excluir(obj.Id, ref erro, _paramBase))
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult ObterAtividadeVisaosPorId(int id)
        {
            var obj = new AtividadeVisao().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new AtividadeVisao();
                
            }

            return Json(new
            {
                obj.Id,
                obj.IdAtividade,
                obj.IdVisao,
                obj.Ordem,
                obj.Titulo,
                obj.Visao
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SalvarAtividadeVisaos(AtividadeVisao obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.Id == 0)
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ExcluirAtividadeVisaos(AtividadeVisao obj)
        {

            try
            {

                string erro = "";

                if (new AtividadeVisao().Excluir(obj.Id, ref erro, _paramBase))
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult ObterAtividadePlanosPorId(int id)
        {
            var obj = new AtividadePlano().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new AtividadePlano();
 
            }

            return Json(new
            {
                obj.Id,
                obj.AtividadeId,
                obj.AtividadeIdEntrada,
                obj.CondicaoEntrada,
                obj.ProcessoId

            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ObterMapaAtividadePlanosPorId(int idProcesso)
        {
            var obj = new AtividadePlano().ObterPorId(idProcesso, _paramBase);
            if (obj == null)
            {
                obj = new AtividadePlano();
            }
            return Json(new
            {
                obj.Id,
                obj.AtividadeId,
                obj.AtividadeIdEntrada,
                obj.CondicaoEntrada,
                obj.ProcessoId,
                obj.Inicial,
                obj.Final

            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SalvarAtividadePlanos(AtividadePlano obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }
                if (obj.Id == 0)
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ExcluirAtividadePlanos(AtividadePlano obj)
        {

            try
            {

                string erro = "";

                if (new AtividadePlano().Excluir(obj.Id, ref erro, _paramBase))
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
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult ObterAtividadeTipo()
        {
            var objs = new AtividadeTipo().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            return Json(objs.Select(p => new
            {
                Value = p.Id,
                Text = p.Descricao
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterFuncao(int idAtividade)
        {
            var objs = new Funcao().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            var obj2s = new AtividadeFuncao().ObterTodos(idAtividade, _paramBase);

            foreach (var item in obj2s)
            {
                var obj3 = objs.Where(p => p.Id == item.IdFuncao).FirstOrDefault();
                if (obj3 != null)
                    obj3.Selecionado = true;
            }


            return Json(objs.Select(p => new
            {
                p.Id,
                p.Descricao,
                p.Selecionado,
                p.Ativo,
                p.IdEmpresa
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObterListaVisao(int idAtividade)
        {
            var objs = new Visao().ObterTodos(_paramBase).Where(p => p.Ativo == true).OrderBy(p => p.Descricao);
            var obj2s = new AtividadeVisao().ObterTodos(idAtividade, _paramBase);

            foreach (var item in obj2s)
            {
                var obj3 = objs.Where(p => p.Id == item.IdVisao).FirstOrDefault();
                if (obj3 != null)
                    obj3.Selecionado = true;
            }


            return Json(objs.Select(p => new
            {
                p.Id,
                p.Descricao,
                p.Selecionado,
                p.Ativo,
                p.IdEmpresa,
                p.IdTabela,
                p.IdTipoVisao,
                
                Tabela = p.Tabela.Descricao
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}