using Newtonsoft.Json;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SoftFin.Web.CallApi
{
    
    public class GestorProcessos: BaseCallApi
    {
        string _uri = ConfigurationManager.AppSettings["urlGestorProcessos"].ToString();

        internal object ObterManutencao(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<List<DTOComum>>(_uri + "Manutencao/Obter", jsonstr);
            return ret;
        }

        //string uri = "http://localhost:50714/api/";
        public List<DTOComum> ObterProcessos(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<List<DTOComum>>(_uri + "Processo/Obter", jsonstr);
            return ret;
        }

        public List<DTOComum> ObterProcessosIniciaisPorUsuario(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<List<DTOComum>>(_uri + "Processo/ObterProcessosIniciaisPorUsuario", jsonstr);
            return ret;
        }
        
        public DTOGenericoRetorno<DTOAtividadeExecucao> ObterAtividadeExecucao(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<DTOGenericoRetorno<DTOAtividadeExecucao>>(_uri + "AtividadeExecucao/Obter", jsonstr);
            return ret;
        }

        public DTOGenericoRetorno<DTOAtividadeExecucao> ObterAtividadeExecucaoHistorico(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<DTOGenericoRetorno<DTOAtividadeExecucao>>(_uri + "AtividadeExecucao/ObterHistorico", jsonstr);
            return ret;
        }

        internal List<DTOComum> ObterUsuarios(ParamProcesso param)
        {
            var jsonstr = JsonConvert.SerializeObject(param);
            var ret = base.PostSync<List<DTOComum>>(_uri + "Usuario/Obter", jsonstr);
            return ret;
        }

        public DTOGenericoRetorno<DTOAtividadeExecucao> ObterAtividadeExecucaoOutros(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<DTOGenericoRetorno<DTOAtividadeExecucao>>(_uri + "AtividadeExecucao/ObterOutros", jsonstr);
            return ret;
        }

        internal string DelegarAtividade(ParamProcesso param, string novousuario, string codigoAtividadeExecucaoAtual)
        {
            param.CodigoUsuario = novousuario;
            param.CodigoAtividadeExecucaoAtual = codigoAtividadeExecucaoAtual;

            var jsonstr = JsonConvert.SerializeObject(param);
            var ret = base.PostSync<string>(_uri + "Usuario/DelegarAtividade", jsonstr);
            return ret;
        }

        public DTOGenericoRetorno<DTOProximaAtividade> CriarNovaExecucao(ParamProcesso paramProcesso)
        {
            var jsonstr = JsonConvert.SerializeObject(paramProcesso);
            var ret = base.PostSync<DTOGenericoRetorno<DTOProximaAtividade>>(_uri + "ProcessoExecucao/CriarNovaExecucao", jsonstr);
            return ret;
        }

        internal DTOGenericoRetorno<DTOMapa> DetalharProcesso(ParamProcesso param, string codigoProcessoAtual)
        {

            param.CodigoProcessoAtual = codigoProcessoAtual;
            var jsonstr = JsonConvert.SerializeObject(param);
            var ret = base.PostSync<DTOGenericoRetorno<DTOMapa>>(_uri + "ProcessoExecucao/Mapa", jsonstr);
            return ret;
        }

        internal string CancelarProcesso(ParamProcesso param, string motivo, string codigoProcessoAtual)
        {
            param.Motivo = motivo;
            param.CodigoProcessoAtual = codigoProcessoAtual;
            var jsonstr = JsonConvert.SerializeObject(param);
            var ret = base.PostSync<string>(_uri + "AtividadeExecucao/CancelarProcesso", jsonstr);
            return ret;
        }
    }
}
