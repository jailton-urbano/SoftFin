using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class DTOMapa
    {
        public DTOMapa()
        {
            Dets = new List<DTOMapaDet>();
        }
        public string Processo { get; set; }

        public string InicioProcesso { get; set; }

        public string FimProcesso { get; set; }

        public string Cancelado { get; set; }
        public string Motivo { get; set; }
        public string UsuarioCancelado { get; set; }
        public List<DTOMapaDet> Dets { get; set; }

    }
}
