using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOInfNFe
    {
        public  DTOInfNFe()
        {
            Det = new List<DTODet>();
            Ide = new DTOIde();
            Emi = new DTOEmit();
            Avulsa = new DTOAvulsa();
            Dest = new DTODest();
            Retirada = new DTORetirada();
            Entrega = new DTOEntrega();
            Total = new DTOTotal();
            Transp = new DTOTransp();
            InfAdic = new DTOInfAdic();
            Cobr = new DTOCobr();
            Pagamento = new List<DTOPagamento>();
            
        }
        public string versao { get; set; }
        public string pk_nItem { get; set; }
        public DTOIde Ide { get; set; }
        public DTOEmit Emi { get; set; }
        public DTOAvulsa Avulsa { get; set; }
        public DTODest Dest { get; set; }
        public DTORetirada Retirada { get; set; }
        public DTOEntrega Entrega { get; set; }
        public List<DTODet> Det { get; set; }
        public List<DTOAutXML> AutXML { get; set; }

        public DTOTotal Total { get; set; }
        public DTOTransp Transp { get; set; }
        public List<DTOPagamento> Pagamento { get; set; }
        public DTOInfAdic InfAdic { get; set; }

        public DTOCobr Cobr { get; set; }



        public string Id { get; set; }
    }
}
