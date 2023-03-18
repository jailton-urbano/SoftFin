using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class DTOMapaDetExecucao
    {
        public string Codigo { get; set; }
        public string Usuario { get; set; }
        public string UsuarioExecucao { get; set; }
        public string DataInicio { get; set; }
        public string DataFim{ get; set; }
        public string Status { get; set; }
        public string Motivo { get; set; }
    }
}
