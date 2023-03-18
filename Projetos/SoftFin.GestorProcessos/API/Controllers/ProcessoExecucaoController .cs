using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using SoftFin.GestorProcessos.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SoftFin.GestorProcessos.API.Controllers
{
    [RoutePrefix("api/ProcessoExecucao")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProcessoExecucaoController : ApiController
    {
        [System.Web.Http.HttpPost, Route("Obter")]
        public DTOGenericoRetorno<DTOAtividadeExecucao> Obter(ParamProcesso paramProcesso)
        {
            var retorno = new DTOGenericoRetorno<DTOAtividadeExecucao>();
            retorno.status = "NOK";

            try
            {

                var lista = new List<DTOListaProcessos>();
                var db = new DBGPControle();
                var listaAux = db.ProcessoExecucao
                    .Where(p => p.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa && p.Processo.Ativo == true && p.FimProcesso == null);
                var listaRetorno = listaAux.Select(p => new DTOAtividadeExecucao { CodigoProcesso = p.Processo.Codigo.ToString(), NumeroProtocolo = p.Protocolo, DataInicio = p.InicioProcesso.ToString("o") });


                retorno.status = "OK";
                retorno.Objs = listaRetorno.ToList();
            }catch(Exception ex)
            {
                retorno.Exceptions.Add(new DTOException { Codigo = "EX01", Descricao = ex.Message, Tipo = "Exception" });
            }

            return retorno;
        }

        private bool UsuarioTemAcesso(int idusuario, int idAtividade, DBGPControle db)
        {
            var atividadeFuncaos = db.AtividadeFuncao.Where(p => p.IdAtividade == idAtividade).ToList();

            foreach (var itemF in atividadeFuncaos)
            {
                var usuarioFuncao = db.UsuarioFuncao.Where(p => p.IdUsuario == idusuario && p.IdFuncao == itemF.IdFuncao).FirstOrDefault();
                if (usuarioFuncao != null)
                {
                    return true;
                }
            }
            return false;
        }


        [System.Web.Http.HttpPost, Route("CriarNovaExecucao")]
        public DTOGenericoRetorno<DTOProximaAtividade> CriarNovaExecucao(ParamProcesso paramProcesso)
        {
            var entidade = new DTOGenericoRetorno<DTOProximaAtividade>();
            entidade.status = "NOK";
            try
            {
                var db = new DBGPControle();
                var codigoProcesso = paramProcesso.CodigoProcesso;
                var atividadeAtual = db.AtividadePlano.Where(p => p.Processo.Codigo == codigoProcesso 
                && p.AtividadeIdEntrada == null
                && p.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa).FirstOrDefault();

                var processoAtual = db.Processo.Where(p => p.Codigo == codigoProcesso).FirstOrDefault();
                if (processoAtual == null)
                    throw new Exception("Processo não encontrado");

                var usuarioAtual = db.Usuario.Where(p => p.Login == paramProcesso.CodigoUsuario).FirstOrDefault();
                if (usuarioAtual == null)
                    throw new Exception("Usuário não encontrado");

                if (!UsuarioTemAcesso(usuarioAtual.Id, atividadeAtual.AtividadeId, db))
                    throw new Exception("Usuário sem acesso");

                var processoExecucao = new ProcessoExecucao();

                processoAtual.Contador += 1;
                var protocolo = string.Format(processoAtual.CodigoProcessoTemplate, processoAtual.Contador);
                db.Entry(processoAtual).State = EntityState.Modified;
                processoExecucao = new ProcessoExecucao
                {
                    Codigo = Guid.NewGuid(),
                    IdUsuario = usuarioAtual.Id,
                    InicioProcesso = DateTime.Now,
                    ProcessoId = processoAtual.Id,
                    Protocolo = protocolo
                };
                db.ProcessoExecucao.Add(processoExecucao);

                db.SaveChanges();
                var processoExecucaoAtividadeGuid = Guid.NewGuid().ToString();
                var processoExecucaoAtividade = new ProcessoExecucaoAtividade
                {
                    Codigo = processoExecucaoAtividadeGuid,
                    IdAtividade = atividadeAtual.AtividadeId,
                    IdUsuario = usuarioAtual.Id,
                    ResultadoFinal = paramProcesso.Resultado,
                    InicioAtividade = DateTime.Now,
                    FimAtividade = null,
                    IdUsuarioExecucao = usuarioAtual.Id,
                    ProcessoExecucaoId = processoExecucao.Id,
                    Situacao = "Não Confirmado"

                };

                db.ProcessoExecucaoAtividade.Add(processoExecucaoAtividade);
                db.SaveChanges();

                entidade.Obj = new DTOProximaAtividade {
                    CodigoAtividadeAtual = processoExecucaoAtividadeGuid.ToString(),
                    CodigoAtividade = atividadeAtual.Atividade.Codigo.ToString(),
                    NumeroProcesso = protocolo,
                    CodigoProcessoAtual = processoExecucao.Codigo.ToString(),
                    CodigoAtividadeExecucaoAtual = processoExecucaoAtividadeGuid.ToString(),
                    TipoAtividade = atividadeAtual.Atividade.AtividadeTipo.Codigo

                };
                entidade.status = "OK";
            }
            catch (Exception ex)
            {
                //TODO Grava Log
            }



            return entidade;
        }


        //[System.Web.Http.HttpPost, Route("Cancelar")]
        //public string Cancelar(ParamProcesso paramProcesso)
        //{
        //    var db = new DBGPControle();
        //    var codigoProcesso = paramProcesso.CodigoProcessoAtual;
        //    var processoExecucao = db.ProcessoExecucao.Where(p => p.Processo.Codigo == codigoProcesso).FirstOrDefault();
            
        //    var usuarioAtual = db.Usuario.Where(p => p.Login == paramProcesso.CodigoUsuario).FirstOrDefault();
        //    processoExecucao.FimProcesso = DateTime.Now;
        //    processoExecucao.MotivoCancelado = paramProcesso.Resultado;

        //    processoExecucao.IdUsuarioCancelamento = usuarioAtual.Id;
        //    //processoExecucao.Situacao = "Cancelado";
        //    db.SaveChanges();

        //    return "OK";
        //}

        [System.Web.Http.HttpPost, Route("Mapa")]
        public DTOGenericoRetorno<DTOMapa> Mapa(ParamProcesso paramProcesso)
        {
            DTOGenericoRetorno<DTOMapa> retorno = new DTOGenericoRetorno<DTOMapa>();
            var db = new DBGPControle();
            var codigoAtividadeExecucaoAtual = "";
            Guid codigoProcessoAtual;
            var idProcessoAtual = 0;
            var idProcesso = 0;
            var codigoProcesso = "";
            var processoExecucaoAtividade = new List<ProcessoExecucaoAtividade>();

            if (paramProcesso.CodigoAtividadeExecucaoAtual != null)
            {
                codigoAtividadeExecucaoAtual = paramProcesso.CodigoAtividadeExecucaoAtual;
                processoExecucaoAtividade = db.ProcessoExecucaoAtividade.Where(p => p.Codigo == codigoAtividadeExecucaoAtual).ToList();
                codigoProcessoAtual = processoExecucaoAtividade.FirstOrDefault().ProcessoExecucao.Codigo;
                idProcessoAtual = processoExecucaoAtividade.FirstOrDefault().ProcessoExecucao.Id;
                codigoProcesso = processoExecucaoAtividade.FirstOrDefault().ProcessoExecucao.Processo.Codigo;
                idProcesso = processoExecucaoAtividade.FirstOrDefault().ProcessoExecucao.Processo.Id;
            }

            if (paramProcesso.CodigoProcessoAtual != null)
            {
                codigoProcessoAtual = Guid.Parse(paramProcesso.CodigoProcessoAtual);
                processoExecucaoAtividade = db.ProcessoExecucaoAtividade.Where(p => p.ProcessoExecucao.Codigo == codigoProcessoAtual).ToList();

                var processoExecucaos = db.ProcessoExecucao.Where(p => p.Codigo == codigoProcessoAtual).ToList();
                codigoProcessoAtual = processoExecucaos.FirstOrDefault().Codigo;
                idProcessoAtual = processoExecucaos.FirstOrDefault().Processo.Id;
                codigoProcesso = processoExecucaos.FirstOrDefault().Processo.Codigo;
                idProcesso = processoExecucaos.FirstOrDefault().Processo.Id;
            }

            var processoExecucao = db.Processo.Where(p => p.Codigo == codigoProcesso).FirstOrDefault();
            var atividadePlano = db.AtividadePlano.Where(p => p.Processo.Codigo == codigoProcesso).ToList();
            var pe = processoExecucaoAtividade.FirstOrDefault().ProcessoExecucao;


            retorno.Obj = new DTOMapa();

            retorno.Obj.Processo = processoExecucao.Descricao;
            retorno.Obj.InicioProcesso = pe.InicioProcesso.ToString("o");
            retorno.Obj.FimProcesso = (pe.FimProcesso == null)? "": pe.FimProcesso.Value.ToString("o");
            retorno.Obj.Cancelado = (pe.MotivoCancelado != null) ? "Sim" : "Não";
            retorno.Obj.Motivo = pe.MotivoCancelado;
            retorno.Obj.UsuarioCancelado = (pe.UsuarioCancelamento == null) ? "" : pe.Usuario.Nome;

            var atividadeFuncao = db.AtividadeFuncao.
                Join(db.AtividadePlano, af => af.IdAtividade, ap => ap.AtividadeId, (af, ap) => new { af, ap }).
                Where(p => p.ap.ProcessoId == idProcesso).
                Select(af =>  af.af ).ToList() ;

            MontaAtividadeExecucao(retorno, db, atividadePlano, processoExecucaoAtividade, atividadeFuncao, null, null);
            retorno.status = "OK";
            return retorno;
        }

        private static void MontaAtividadeExecucao(DTOGenericoRetorno<DTOMapa> retorno, 
            DBGPControle db, 
            List<AtividadePlano> atividadePlano, 
            List<ProcessoExecucaoAtividade> processoExecucaoAtividade,
            List<AtividadeFuncao> atividadeFuncao,
            int? idAtividadeInicial,
            string condicaoEntreda)
        {
            var atividadePlanos = atividadePlano.Where(p => p.AtividadeIdEntrada == idAtividadeInicial && p.CondicaoEntrada == condicaoEntreda).ToList();
            foreach (var itemAP in atividadePlanos)
            { 
                var jaMapaDet = retorno.Obj.Dets.Where(p => p.IdPlano == itemAP.Id).Count();
                if (jaMapaDet > 0)
                {
                    return;
                }
                if (retorno.Obj.Dets.Where(p => p.Atividade == itemAP.Atividade.Descricao).Count() > 0)
                {
                    break;
                }


                var funcao = atividadeFuncao.Where(p => p.IdAtividade == itemAP.Atividade.Id).FirstOrDefault();
                var det = new DTOMapaDet
                {
                    Atividade = itemAP.Atividade.Descricao,
                    Funcao = (funcao != null) ? funcao.Funcao.Descricao : "",
                    IdPlano = itemAP.Id,
                    CondicaoEntrada = condicaoEntreda
                };
                var processoExecucaoAtividadeAux = processoExecucaoAtividade.Where(p => p.IdAtividade == itemAP.AtividadeId).OrderByDescending(p => p.InicioAtividade).ToList();
                var execs = new List<DTOMapaDetExecucao>();
                foreach (var item in processoExecucaoAtividadeAux)
                {
                    execs.Add(new DTOMapaDetExecucao
                    {
                        Codigo = item.Codigo,
                        DataInicio = (item.InicioAtividade == null) ? "" : item.InicioAtividade.Value.ToString("o"),
                        DataFim = (item.FimAtividade == null) ? "" : item.FimAtividade.Value.ToString("o"),
                        Status = item.Situacao,
                        Usuario = item.Usuario.Nome,
                        UsuarioExecucao = (item.UsuarioExecucao == null) ? "" : item.UsuarioExecucao.Login,
                        Motivo = item.Motivo
                    });
                }
                det.Execs.AddRange(execs);
                retorno.Obj.Dets.Add(det);

                if (processoExecucaoAtividadeAux.Count() > 0)
                {
                    if (processoExecucaoAtividadeAux.FirstOrDefault().Atividade.AtividadeTipo.Codigo != "Question")
                    {
                        MontaAtividadeExecucao(retorno, db, atividadePlano, processoExecucaoAtividade, atividadeFuncao, itemAP.AtividadeId, null);
                    }
                    else
                    {
                        MontaAtividadeExecucao(retorno, db, atividadePlano, processoExecucaoAtividade, atividadeFuncao, itemAP.AtividadeId, "Sim");
                        MontaAtividadeExecucao(retorno, db, atividadePlano, processoExecucaoAtividade, atividadeFuncao, itemAP.AtividadeId, "Não");
                    }
                }
            }
        }
    }
}
