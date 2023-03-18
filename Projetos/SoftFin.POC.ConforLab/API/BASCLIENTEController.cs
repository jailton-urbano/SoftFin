using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using SoftFin.POC.ConforLab.Models;

namespace SoftFin.POC.ConforLab.Controllers
{
    [System.Web.Http.RoutePrefix("api/BASCLIENTE")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BASCLIENTEController : ApiController
    {
        [System.Web.Http.HttpGet, System.Web.Http.Route("ObterNovo")]
        public BAS_CLIENTE ObterNovo()
        {
            var entidade = new BAS_CLIENTE();
            entidade.BCL_PROTOCOL = Guid.NewGuid();
            return entidade;
        }

        [System.Web.Http.HttpGet, System.Web.Http.Route("ObterPorProtocolo")]
        public BAS_CLIENTE ObterPorProtocolo(string protocolo)
        {
            var db = new DBAAContext();

            var entidade = db;
            var guidProtocolo = Guid.Parse(protocolo);
            return entidade.BAS_CLIENTE.Where(p => p.BCL_PROTOCOL == guidProtocolo).FirstOrDefault();
        }


        // GET: BASCADTECNICO
        [System.Web.Http.HttpPost, System.Web.Http.Route("Salvar")]
        public DTOProximaAtividade Salvar(BAS_CLIENTE entidade)
        {

            //Vefica Prototolo Existente

            //Validação
            
            //Salva Protoocolo
            var db = new DBAAContext();
            var protocolo =  Guid.Parse(entidade.CodigoProcessoAtual);
            var cadastrado = db.BAS_CLIENTE.Where(p => p.BCL_PROTOCOL == protocolo).FirstOrDefault();

            if (cadastrado == null)
            {
                entidade.BCL_PROTOCOL = Guid.Parse(entidade.CodigoProcessoAtual);
                db.BAS_CLIENTE.Add(entidade);
            }
            else
            {
                cadastrado.BCL_RAZAO = entidade.BCL_RAZAO;
                cadastrado.BCL_NOME_FANTASIA = entidade.BCL_NOME_FANTASIA;
                //TODO Ricardo
                db.Entry(cadastrado).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();

            var param = new ParamProcesso();
            param.CodigoAtividade = entidade.CodigoAtividade;
            param.CodigoEmpresa = entidade.CodigoEmpresa;
            param.CodigoUsuario = entidade.CodigoUsuario;
            param.CodigoProcesso = entidade.CodigoProcesso;
            param.CodigoAtividadeExecucaoAtual = entidade.CodigoAtividadeExecucaoAtual;

            var proximaAtividade = new GestorProcessos().GeraProximaAtividade(param);
            proximaAtividade.MostraProximaAtividade = true;

            return proximaAtividade;
        }



    }
}