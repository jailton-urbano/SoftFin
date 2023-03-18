using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace SoftFin.GestorProcessos.API.Controllers
{
    public class ManutencaoController : ApiController
    {
        [System.Web.Http.HttpPost, ActionName("Obter")]
        public List<DTOComum> Obter(GestorProcessos.Comum.Param.ParamProcesso param)
        {
            var lista = new List<DTOComum>();
            var db = new DBGPControle();
            lista = db.Tabela.Where(p => p.Empresa.Codigo == param.CodigoEmpresa && p.Ativo == true && p.CadastroAuxiliar == true).
                Select(p => new DTOComum { Codigo = p.Nome, Descricao = p.Descricao }).
                ToList();

            return lista;
        }

    }
}