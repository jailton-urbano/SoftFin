using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(250)]
        public string Descricao { get; set; }
        [MaxLength(25)]
        public string Codigo { get; set; }
        public bool Ativo { get; set; }
        [MaxLength(500)]
        public string ConectionString { get; set; }
    }
}