using SoftFin.API.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API.DTO
{
    public class BaseDTORetorno
    {
        public BaseDTORetorno()
        {
            exceptions = new List<DTOException>();
        }
        public string status { get; set; }
        public List<DTOException> exceptions { get; set; }
 
    }
}