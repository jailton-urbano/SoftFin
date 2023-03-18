using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SoftFin.GestorProcessos.API.Controllers
{
    [RoutePrefix("api/Usuario")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsuarioController : ApiController
    {
        [System.Web.Http.HttpPost, Route("ObterPorRoleAtividade")]
        public List<DTOComum> ObterPorRoleAtividade(ParamProcesso paramProcesso)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            var auxAtividade = paramProcesso.CodigoAtividade;

            //var atividade = db.Atividade.Where(p => p.Codigo == auxAtividade)
            //var x = db.AtividadeFuncao.Where(p => p.c)

            lista = (from u in db.UsuarioFuncao
                     join a in db.AtividadeFuncao on u.IdFuncao equals a.IdFuncao
                     where a.Atividade.Codigo == auxAtividade && u.Usuario.Ativo == true
                     select new DTOComum { Codigo = u.Usuario.Login, Descricao = u.Usuario.Login }).ToList();



            return lista;
        }


        [System.Web.Http.HttpPost, Route("Obter")]
        public List<DTOComum> Obter(ParamProcesso paramProcesso)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            var auxAtividade = paramProcesso.CodigoAtividade;

            //var atividade = db.Atividade.Where(p => p.Codigo == auxAtividade)
            //var x = db.AtividadeFuncao.Where(p => p.c)

            lista = (from u in db.Usuario
                     
                     select new DTOComum { Codigo = u.Login, Descricao = u.Nome }).ToList();



            return lista;
        }

        [System.Web.Http.HttpPost, Route("DelegarAtividade")]
        public string DelegarAtividade(ParamProcesso paramProcesso)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            var auxAtividade = paramProcesso.CodigoAtividadeExecucaoAtual;

            var at = db.ProcessoExecucaoAtividade.Where(p => p.Codigo == auxAtividade).First();
            at.IdUsuario = db.Usuario.Where(p => p.Login == paramProcesso.CodigoUsuario).First().Id;
            db.SaveChanges();



            return "OK";
        }
    }
}