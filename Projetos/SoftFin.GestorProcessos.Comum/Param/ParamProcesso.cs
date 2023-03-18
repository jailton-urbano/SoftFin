using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Comum.Param
{
    public class ParamProcesso
    {
        public string Resultado { get; set; }
        public string Motivo { get; set; }

        public string CodigoUsuario { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string CodigoProcesso { get; set; }
        public string CodigoProtocolo { get; set; }
        public string GuidProtocolo { get; set; }
        public string CodigoAtividade { get; set; }
        public string NumeroProcesso { get; set; }
        public string NumeroProtocolo { get; set; }
        public string CodigoAtividadeExecucaoAtual { get; set; }
        public string CodigoProcessoAtual { get; set; }
        public string Tabela { get; set; }
    }
}