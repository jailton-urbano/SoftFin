using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Comum.DTO
{
    public class  DTOGenericoRetorno<T> : BaseDTORetorno
    {
        public List<T> Objs { get; set; }
        public T Obj { get; set; }
    }
}