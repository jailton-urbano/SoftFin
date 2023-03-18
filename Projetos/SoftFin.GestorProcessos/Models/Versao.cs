using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class Versao
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int? IdEmpresa { get; set; }
        [JsonIgnore, ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
    }
}