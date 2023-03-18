using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class DTOMapaDet
    {
        public DTOMapaDet()
        {
            Execs = new List<DTOMapaDetExecucao>();
        }


        public string Atividade { get; set; }
        public string Funcao { get; set; }

        public List<DTOMapaDetExecucao> Execs { get; set; }
        public int IdPlano { get; set; }
        public string CondicaoEntrada { get; set; }
    }
}
