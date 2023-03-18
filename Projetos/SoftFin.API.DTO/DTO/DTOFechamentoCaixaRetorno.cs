using SoftFin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class DTOFechamentoCaixaRetorno : BaseDTORetorno
    {
        public List<DTOFechamentoCaixa> objs { get; set; }
    }
}