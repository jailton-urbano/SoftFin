using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class SistemaPrincipal
    {
        [Key]
        public int id { get; set; }
        [MaxLength(40)]
        public string descricao { get; set; }
        public bool ativo { get; set; }
        [MaxLength(12)]
        public string Codigo { get; set; }

        internal SistemaPrincipal ObterPorCodigo(string codigo)
        {
            var banco = new Classes.DbControle();
            return banco.SistemaPrincipal.Where(x => x.Codigo == codigo).FirstOrDefault();
        }
    }
}