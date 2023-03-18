using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.NFe.DTO
{
    public class  DTORetorno<T>
    {
        public bool CMDAceito { get; set; }
        public string CDStatus { get; set; }
        public string DSStatus { get; set; }

        public List<DTOErro> Erros { get; set; }
        public List<DTOErro> Alertas { get; set; }
        public List<T> Objs { get; set; }
    }
}