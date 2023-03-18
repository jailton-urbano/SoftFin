using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using SoftFin.GestorProcessos.Models;
using System.Web.Http.Cors;

namespace SoftFin.GestorProcessos.API.Controllers
{
    [RoutePrefix("api/AtividadeExecucao")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AtividadeExecucaoController : ApiController
    {
        [System.Web.Http.HttpPost, Route("DetalharProcesso")]
        public string DetalharProcesso(ParamProcesso paramProcesso)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            var codigoProcessoExecucao = Guid.Parse(paramProcesso.CodigoProcessoAtual);
            var processoExecucao = db.ProcessoExecucao.Where(p => p.Codigo == codigoProcessoExecucao).FirstOrDefault();
            processoExecucao.FimProcesso = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
            processoExecucao.MotivoCancelado = paramProcesso.Motivo;
            processoExecucao.IdUsuarioCancelamento = db.Usuario.Where(p => p.Login == paramProcesso.CodigoUsuario).FirstOrDefault().Id;
            db.SaveChanges();
            return "OK";
        }
        [System.Web.Http.HttpPost, Route("CancelarProcesso")]
        public string CancelarProcesso(ParamProcesso paramProcesso)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            var codigoProcessoExecucao = Guid.Parse(paramProcesso.CodigoProcessoAtual);
            var processoExecucao = db.ProcessoExecucao.Where(p => p.Codigo == codigoProcessoExecucao).FirstOrDefault();
            processoExecucao.FimProcesso = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
            processoExecucao.MotivoCancelado = paramProcesso.Motivo;
            processoExecucao.IdUsuarioCancelamento = db.Usuario.Where(p => p.Login == paramProcesso.CodigoUsuario).FirstOrDefault().Id;
            db.SaveChanges();



            return "OK";
        }
        [System.Web.Http.HttpPost, Route("Obter")]
        public DTOGenericoRetorno<DTOAtividadeExecucao> Obter(ParamProcesso paramProcesso)
        {
            var retorno = new DTOGenericoRetorno<DTOAtividadeExecucao>();
            retorno.status = "NOK";

            try
            {

                var lista = new List<DTOListaProcessos>();
                var db = new DBGPControle();
                var listaAux = db.ProcessoExecucaoAtividade.Include(x => x.Usuario.UsuarioFuncaos)
                    .Where(p => p.ProcessoExecucao.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa
                        && p.ProcessoExecucao.Processo.Ativo == true
                        && p.ProcessoExecucao.FimProcesso == null
                        && p.FimAtividade == null
                        && p.Usuario.Login == paramProcesso.CodigoUsuario
                        && p.ProcessoExecucao.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa
                        && p.Situacao == "Em Aberto"
                    );

                if (paramProcesso.CodigoProcesso != null)
                {
                    listaAux = listaAux.Where(p => p.ProcessoExecucao.Processo.Codigo == paramProcesso.CodigoProcesso);
                }

                retorno.status = "OK";
                retorno.Objs = ToDTO(listaAux,paramProcesso);
            }
            catch (Exception ex)
            {
                retorno.Exceptions.Add(new DTOException { Codigo = "EX02", Descricao = ex.Message, Tipo = "Exception" });
            }

            return retorno;
        }

        [System.Web.Http.HttpPost, Route("ObterHistorico")]
        public DTOGenericoRetorno<DTOAtividadeExecucao> ObterHistorico(ParamProcesso paramProcesso)
        {
            var retorno = new DTOGenericoRetorno<DTOAtividadeExecucao>();
            retorno.status = "NOK";

            try
            {

                var lista = new List<DTOListaProcessos>();
                var db = new DBGPControle();
                var listaAux = db.ProcessoExecucao.Include(x => x.Usuario.UsuarioFuncaos)
                    .Where(p => p.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa
                        && p.Processo.Ativo == true
                        && p.FimProcesso != null
                        && p.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa
                    );

                if (paramProcesso.CodigoProcesso != null)
                {
                    listaAux = listaAux.Where(p => p.Processo.Codigo == paramProcesso.CodigoProcesso);
                }

                retorno.status = "OK";
                retorno.Objs = ToDTO(listaAux, paramProcesso);
            }
            catch (Exception ex)
            {
                retorno.Exceptions.Add(new DTOException { Codigo = "EX02", Descricao = ex.Message, Tipo = "Exception" });
            }

            return retorno;
        }

        [System.Web.Http.HttpPost, Route("ObterOutros")]
        public DTOGenericoRetorno<DTOAtividadeExecucao> ObterOutros(ParamProcesso paramProcesso)
        {
            var retorno = new DTOGenericoRetorno<DTOAtividadeExecucao>();
            retorno.status = "NOK";

            try
            {

                var lista = new List<DTOListaProcessos>();
                var db = new DBGPControle();
                var listaAux = db.ProcessoExecucaoAtividade.Include(x => x.Usuario.UsuarioFuncaos)
                    .Where(p => p.ProcessoExecucao.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa
                        && p.ProcessoExecucao.Processo.Ativo == true
                        && p.ProcessoExecucao.FimProcesso == null
                        && p.FimAtividade == null
                        && p.Usuario.Login != paramProcesso.CodigoUsuario
                        && p.ProcessoExecucao.Processo.Empresa.Codigo == paramProcesso.CodigoEmpresa
                        && p.Situacao == "Em Aberto"
                    );


                if (paramProcesso.CodigoProcesso != null)
                {
                    listaAux = listaAux.Where(p => p.ProcessoExecucao.Processo.Codigo == paramProcesso.CodigoProcesso);
                }

                retorno.status = "OK";
                retorno.Objs = ToDTO(listaAux, paramProcesso);
            }
            catch (Exception ex)
            {
                retorno.Exceptions.Add(new DTOException { Codigo = "EX01", Descricao = ex.Message, Tipo = "Exception" });
            }

            return retorno;
        }

        private List<DTOAtividadeExecucao> ToDTO(IQueryable<Models.ProcessoExecucao> listaAux, ParamProcesso paramProcesso)
        {

            var retorno = new List<DTOAtividadeExecucao>();
            var lista = listaAux.ToList();
            foreach (var item in lista)
            {
                var aux = new DTOAtividadeExecucao();
                aux.CodigoProcesso = item.Processo.Codigo.ToString();
                aux.CodigoProcessoAtual = item.Codigo.ToString();
                aux.NumeroProtocolo = item.Protocolo;
                aux.CodigoProtocolo = item.Codigo.ToString();
                aux.DataInicio =item.InicioProcesso.ToString("o");
                aux.DataFinal = (item.FimProcesso == null) ? "" : item.FimProcesso.Value.ToString("o");
                aux.Responsavel = item.Usuario.Nome;
                aux.CodigoEmpresa = paramProcesso.CodigoEmpresa;
                aux.CodigoUsuario = paramProcesso.CodigoUsuario;
                aux.MotivoCancelado = item.MotivoCancelado;
                aux.UsuarioCancelamento = (item.UsuarioCancelamento == null) ? "" : item.UsuarioCancelamento.Nome;
                var funcao = item.Usuario.UsuarioFuncaos.FirstOrDefault();
                if (funcao != null)
                    aux.Funcao = funcao.Funcao.Descricao;
                if (item.InicioProcesso != null)
                {
                    TimeSpan ts =  item.FimProcesso.Value - item.InicioProcesso;
                    aux.Tempo = ts.ToString("dd") + " dias " + ts.ToString("hh") + ":" + ts.ToString("mm");
                }
                else
                {
                    aux.Tempo = "";
                }
                retorno.Add(aux);
            }
            return retorno;

        }

        private List<DTOAtividadeExecucao> ToDTO(IQueryable<Models.ProcessoExecucaoAtividade> listaAux, ParamProcesso paramProcesso)
        {

            var retorno = new List<DTOAtividadeExecucao>();
            var lista = listaAux.ToList();
            foreach (var item in lista)
            {
                var aux = new DTOAtividadeExecucao();
                aux.CodigoProcesso = item.ProcessoExecucao.Processo.Codigo.ToString();
                aux.CodigoProcessoAtual = item.ProcessoExecucao.Codigo.ToString();
                aux.NumeroProtocolo = item.ProcessoExecucao.Protocolo;
                aux.CodigoProtocolo = item.ProcessoExecucao.Codigo.ToString();
                aux.DataInicio = (item.InicioAtividade == null) ? "" : item.InicioAtividade.Value.ToString("o");
                aux.Responsavel = item.Usuario.Nome;
                aux.Atividade = item.Atividade.Descricao;
                aux.CodigoAtividade = item.Atividade.Codigo.ToString();
                aux.CodigoAtividadeExecucaoAtual = item.Codigo.ToString();
                aux.GuidProtocolo = item.ProcessoExecucao.Codigo.ToString();
                aux.CodigoEmpresa = paramProcesso.CodigoEmpresa;
                aux.CodigoUsuario = paramProcesso.CodigoUsuario;
                aux.Action = item.Atividade.AtividadeTipo.Codigo;


                var funcao = item.Usuario.UsuarioFuncaos.FirstOrDefault();
                if (funcao != null)
                    aux.Funcao = funcao.Funcao.Descricao;
                if (item.InicioAtividade != null)
                {
                    TimeSpan ts = DateTime.Now - item.InicioAtividade.Value;
                    aux.Tempo = ts.ToString("dd") + " dias " + ts.ToString("hh") + ":" + ts.ToString("mm");
                }
                else
                {
                    aux.Tempo = "";
                }
                retorno.Add(aux);
            }
            return retorno;

        }





        [System.Web.Http.HttpPost]
        public DTOProximaAtividade GerarProximaAtividade(ParamProcesso paramProcesso)
        {
            var entidade = new DTOProximaAtividade();

            var db = new DBGPControle();
            var aux = paramProcesso.CodigoAtividade;
            var atividadeAtual = db.Atividade.Where(p => p.Codigo == aux).FirstOrDefault();
            var codigoProcesso = paramProcesso.CodigoProcesso;
            var processoAtual = db.Processo.Where(p => p.Codigo == codigoProcesso).FirstOrDefault();
            var usuarioAtual = db.Usuario.Where(p => p.Login == paramProcesso.CodigoUsuario).FirstOrDefault();
            var aux2 = paramProcesso.CodigoAtividadeExecucaoAtual;
            var usuarioProcessoAtualExecucao = db.ProcessoExecucaoAtividade.Where(p => p.Codigo == aux2).FirstOrDefault();

            var codigoProcessoExecucao = Guid.Parse(paramProcesso.CodigoProcessoAtual);
            var processoExecucao = db.ProcessoExecucao.Where(p => p.Codigo == codigoProcessoExecucao).FirstOrDefault();

            if (usuarioProcessoAtualExecucao.Situacao == "Executado")
                throw new Exception("Atividade já encerrada");


            if (!usuarioProcessoAtualExecucao.Usuario.Login.Equals(paramProcesso.CodigoUsuario))
                throw new Exception("Esta atividade não pertece a este usuário");

            usuarioProcessoAtualExecucao.FimAtividade = DateTime.Now;
            usuarioProcessoAtualExecucao.ResultadoFinal = paramProcesso.Resultado;
            usuarioProcessoAtualExecucao.IdUsuarioExecucao = usuarioAtual.Id;
            usuarioProcessoAtualExecucao.Situacao = "Executado";
            usuarioProcessoAtualExecucao.Motivo = paramProcesso.Motivo;
            //db.SaveChanges();

            IQueryable<AtividadePlano> proximoAtividade;

            proximoAtividade = VerificaProximaAtividadeNoPlano(paramProcesso, db, atividadeAtual, processoAtual);

            //Se não tem proxima atividade encerra processo
            if (proximoAtividade.Count() == 0)
            {
                FinalizaProcesso(entidade, db, processoExecucao);
                return entidade;
            }


            var agendamentoConcluido = false;
            agendamentoConcluido = VerificaSeMantemUsuarioChamador(db, usuarioAtual, processoExecucao, proximoAtividade, agendamentoConcluido);

            if (!agendamentoConcluido)
            {
                VerificaProximoUsuarioPorRankeamento(db, processoExecucao, proximoAtividade);
            }
            entidade.CDStatus = "OK";

            db.SaveChanges();
            return entidade;
        }

        private static IQueryable<AtividadePlano> VerificaProximaAtividadeNoPlano(ParamProcesso paramProcesso, DBGPControle db, Atividade atividadeAtual, Processo processoAtual)
        {
            IQueryable<AtividadePlano> proximoAtividade;
            if (paramProcesso.Resultado == null)
            {
                proximoAtividade = db.AtividadePlano.Where(p => p.AtividadeIdEntrada == atividadeAtual.Id && p.Processo.Id == processoAtual.Id);
            }
            else
            {
                proximoAtividade = db.AtividadePlano.Where(p => p.AtividadeIdEntrada == atividadeAtual.Id && p.Processo.Id == processoAtual.Id
                            && p.CondicaoEntrada.ToLower().Equals(paramProcesso.Resultado.ToLower()));
            }

            return proximoAtividade;
        }

        private static void FinalizaProcesso(DTOProximaAtividade entidade, DBGPControle db, ProcessoExecucao processoExecucao)
        {
            processoExecucao.FimProcesso = DateTime.Now;

            db.SaveChanges();
            entidade.CDStatus = "OK";
            entidade.MostraProximaAtividade = false;
            entidade.FinalAtividade = true;
        }

        private static void VerificaProximoUsuarioPorRankeamento(DBGPControle db, ProcessoExecucao processoExecucao, IQueryable<AtividadePlano> proximoAtividade)
        {
            foreach (var item in proximoAtividade)
            {

                var atividadeFuncaos = db.AtividadeFuncao.Where(p => p.IdAtividade == item.AtividadeId).ToList();
                var rank = new Dictionary<int, int>();
                foreach (var itemF in atividadeFuncaos)
                {
                    var usuarioFuncaos = db.UsuarioFuncao.Where(p => p.IdFuncao == itemF.IdFuncao).ToList();

                    foreach (var itemU in usuarioFuncaos)
                    {
                        var conta = db.ProcessoExecucaoAtividade.Where(p => p.IdUsuario == itemU.IdUsuario).Count();
                        if (rank.Where(p => p.Key == itemU.IdUsuario).Count() == 0)
                            rank.Add(itemU.IdUsuario, conta);
                        else
                            rank[itemU.IdUsuario] += conta;

                    }
                }

                var rankOrdem = rank.OrderBy(p => p.Value).FirstOrDefault();

                var processoExecucaoAtividadeGuid = Guid.NewGuid().ToString();
                var processoExecucaoAtividade = new ProcessoExecucaoAtividade
                {
                    Codigo = processoExecucaoAtividadeGuid,
                    IdAtividade = item.AtividadeId,
                    IdUsuario = rankOrdem.Key,
                    InicioAtividade = DateTime.Now,
                    ProcessoExecucaoId = processoExecucao.Id,

                    Situacao = "Em Aberto"

                };

                db.ProcessoExecucaoAtividade.Add(processoExecucaoAtividade);

            }
        }

        private static bool VerificaSeMantemUsuarioChamador(DBGPControle db, Usuario usuarioAtual, ProcessoExecucao processoExecucao, IQueryable<AtividadePlano> proximoAtividade, bool agendamentoConcluido)
        {
            foreach (var item in proximoAtividade)
            {
                var atividadeFuncaos = db.AtividadeFuncao.Where(p => p.IdAtividade == item.AtividadeId).ToList();

                foreach (var itemF in atividadeFuncaos)
                {
                    var usuarioFuncao = db.UsuarioFuncao.Where(p => p.IdUsuario == usuarioAtual.Id && p.IdFuncao == itemF.IdFuncao).FirstOrDefault();
                    if (usuarioFuncao != null)
                    {
                        var processoExecucaoAtividadeGuid = Guid.NewGuid().ToString();
                        var processoExecucaoAtividade = new ProcessoExecucaoAtividade
                        {
                            Codigo = processoExecucaoAtividadeGuid,
                            IdAtividade = item.AtividadeId,
                            IdUsuario = usuarioAtual.Id,
                            InicioAtividade = DateTime.Now,
                            ProcessoExecucaoId = processoExecucao.Id,

                            Situacao = "Em Aberto"

                        };

                        db.ProcessoExecucaoAtividade.Add(processoExecucaoAtividade);
                        agendamentoConcluido = true;

                        break;
                    }
                }
            }

            return agendamentoConcluido;
        }
    }
}