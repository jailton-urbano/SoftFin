using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class TipoStatusPropostaSeguro
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Codigo"),
        Required(ErrorMessage = "*"),
        StringLength(20)]
        public string codigo { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "*"),
        StringLength(35)]
        public string descricao { get; set; }

        public System.Collections.IEnumerable ObterTodos()
        {
            DbControle db = new DbControle();
            return db.TipoStatusPropostaSeguro.ToList();
        }

        public TipoStatusPropostaSeguro ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }

        public TipoStatusPropostaSeguro ObterPorId(int id, DbControle banco)
        {
            //int idEstab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.TipoStatusPropostaSeguro.Where(x => x.id == id).FirstOrDefault();
        }
    }
}