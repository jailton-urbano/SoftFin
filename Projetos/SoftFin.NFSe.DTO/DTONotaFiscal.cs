using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SoftFin.NFSe.DTO
{
    public class DTONotaFiscal
    {
        public DTONotaFiscal()
        {
            NFSe = new List<tpNFSe>();
            Cabecalho = new tpCabecalho();
        }
        public tpCabecalho Cabecalho { get; set; }
        public List<tpNFSe> NFSe { get; set; }
        public string municipio_desc { get; set; }
    }

}