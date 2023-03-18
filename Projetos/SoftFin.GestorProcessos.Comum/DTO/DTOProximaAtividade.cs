using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class DTOProximaAtividade
    {

        public string Url { get; set; }
        public string CodigoAtividadeAtual { get; set; }
        public string CodigoAtividade { get; set; }
        public string CodigoProximaAtividade { get; set; }
        public string ProximaAtividade { get; set; }
        public string CDStatus { get; set; }
        public bool MostraProximaAtividade { get; set; }
        public Dictionary<string,string> Erros { get; set; }
        public string DSMessage { get; set; }
        public object NumeroProcesso { get; set; }
        public string CodigoAtividadeExecucaoAtual { get; set; }
        public string CodigoProcessoAtual { get; set; }
        public bool FinalAtividade { get; set; }
        public string TipoAtividade { get; set; }
    }
}