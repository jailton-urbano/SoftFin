using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class DTOAtividadeExecucao
    {
        public string Tempo { get; set; }

  
        public string DataInicio { get; set; }
        public string Atividade { get; set; }
        public string Responsavel { get; set; }
        public string CodigoProcesso { get; set; }
        public string CodigoProtocolo { get; set; }
        public string GuidProtocolo { get; set; }
        public string CodigoAtividade { get; set; }
        public string NumeroProcesso { get; set; }
        public string NumeroProtocolo { get; set; }
        public string CodigoAtividadeExecucaoAtual { get; set; }

        
        
        public string CodigoProcessoAtual { get; set; }
        public string Funcao { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CodigoUsuario { get; set; }
        public string Action { get; set; }
        public string DataFinal { get; set; }
        public string MotivoCancelado { get; set; }
        public string UsuarioCancelamento { get; set; }
    }
}
