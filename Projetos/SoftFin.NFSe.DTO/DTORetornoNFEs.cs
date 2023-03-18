using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.NFSe.DTO
{
    public class DTORetornoNFEs
    {
        public DTORetornoNFEs()
        {
            Cabecalho = new tpCabecalho();
            Erro = new List<TPErro>();
            Alerta = new List<tpAlerta>();
            NFe = new List<tpNFSe>();
            
        }

        public String xmlns { get; set; }
        public tpCabecalho Cabecalho { get; set; }
        public List<TPErro> Erro { get; set; }
        public List<tpAlerta> Alerta { get; set; }
        public List<tpNFSe> NFe { get; set; }
        
        public string xml { get; set; }

        public string tipo { get; set; }
    }

}
