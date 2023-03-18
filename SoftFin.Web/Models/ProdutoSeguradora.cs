using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class ProdutoSeguradora
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string descricao { get; set; }

        public System.Collections.IEnumerable ObterTodos()
        {
            DbControle db = new DbControle();
            return db.ProdutoSeguradora.ToList();
        }
    }
}