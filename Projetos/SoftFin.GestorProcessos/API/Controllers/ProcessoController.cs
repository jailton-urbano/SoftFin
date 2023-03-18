using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SoftFin.GestorProcessos.API.Controllers
{
    public class ProcessoController : ApiController
    {
        [System.Web.Http.HttpPost,ActionName("ObterProcessosIniciaisPorUsuario")]
        public List<DTOComum> ObterProcessosIniciaisPorUsuario(GestorProcessos.Comum.Param.ParamProcesso param)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();

            lista = db.Usuario.Join(db.UsuarioFuncao, u => u.Id, uf => uf.IdUsuario, (u, uf) => new { u, uf })
                .Where(x => x.u.Login == param.CodigoUsuario && x.u.Ativo == true)
                .Join(db.AtividadeFuncao, uf => uf.uf.IdFuncao, af => af.IdFuncao, (uf, af) => new { uf, af })
                .Join(db.AtividadePlano, af => af.af.IdAtividade, ap => ap.AtividadeId, (af, ap) => new { af, ap })
                .Join(db.Processo, ap => ap.ap.ProcessoId, p => p.Id, (ap, p) => new { ap, p })
                .Where(x => x.p.Ativo == true)
                .Select(x => new DTOComum { Codigo = x.p.Codigo, Descricao = x.p.Descricao })
                .Distinct().
                ToList();
            return lista;
        }

        [System.Web.Http.HttpPost, ActionName("Obter")]
        public List<DTOComum> Obter(GestorProcessos.Comum.Param.ParamProcesso param)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            lista = db.Processo.Where(p => p.Empresa.Codigo == param.CodigoEmpresa
                    && p.Ativo == true).
                Select(p => new DTOComum { Codigo = p.Codigo.ToString(), Descricao = p.Descricao }).
                ToList();

            return lista;
        }






    }
}