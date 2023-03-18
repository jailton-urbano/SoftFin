using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.DTO
{
    public class DTOLogEvento
    {
        public string chNFe { get; set; }
        public string cOrgao { get; set; }
        public string cnpj { get; set; }
        public string cpf { get; set; }
        public string dhEvento { get; set; }
        public string tpEvento { get; set; }
        public string nSeqEvento { get; set; }
        public string verEvento { get; set; }
        public string detEvento { get; set; }
        public string versao { get; set; }
        public string descEvento { get; set; }

        public string nProt { get; set; }
        
        public string xJust { get; set; }

        public string xCorrecao { get; set; }
        public string xCondUso { get; set; }
    }
}
