using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class EmpresaParametro
    {
        [Key]
        public int Id { get; set; }
        public int? IdEmpresa { get; set; }
        [JsonIgnore, ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }

        [MaxLength(25)]
        public string Codigo { get; set; }

        [MaxLength(4000)]
        public string Valor { get; set; }
    }
}