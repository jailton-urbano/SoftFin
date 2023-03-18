using System;
using System.Collections.Generic;
using System.Linq;


namespace SoftFin.Infrastructure.DTO
{
    public class DTOLogEvento
    {
        public int Id { get; set; }
        public string CodigoSistema { get; set; }
        public string Estabelecimento { get; set; }
        public DateTime Data { get; set; }
        public string Usuario { get; set; }
        public Guid Agrupador { get; set; }
        public string Tipo { get; set; }
        public string Ip { get; set; }
        public string Rotina { get; set; }
        public string Descricao { get; set; }
        public string Exception { get; set; }
        public string Json { get; set; }
 
    }
}