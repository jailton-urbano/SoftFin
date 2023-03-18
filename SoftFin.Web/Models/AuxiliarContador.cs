using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class AuxiliarContador
    {
        [Key]
        public int id { get; set; }


        public int estab { get; set; }

        [Display(Name = "Codigo"),
        Required(ErrorMessage = "*"),
        StringLength(20)]
        public string codigo { get; set; }

        [Display(Name = "Valor"),
        Required(ErrorMessage = "*")]
        public int valor { get; set; }

        public string GeraNovoContador(string chave, int _estab)
        {
            DbControle banco = new DbControle();

            var existeaChave = banco.AuxiliarContador.Where(p => p.codigo == chave && p.estab == _estab).ToList();

            AuxiliarContador item;

            if (existeaChave.Count() == 0)
            {
                item = new AuxiliarContador { codigo = chave, estab = _estab, valor = 1 };
                banco.Set<AuxiliarContador>().Add(item);
            }
            else
            {
                item = existeaChave.First();
                item.valor += 1;
                banco.Entry(item).State = EntityState.Modified;

            }
            banco.SaveChanges();

            return item.valor.ToString();
        }
    }
}