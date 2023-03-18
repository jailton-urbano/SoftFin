using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class TipoBase: BaseModels
    {
        [Key]
        public int id { get; set; }

        [MaxLength(40)]
        public string descricao { get; set; }

        public bool ativo { get; set; }

        


        public TipoBase ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public TipoBase ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.TipoBase.Where(x => x.id == id).FirstOrDefault();
        }


        public List<TipoBase> ObterTodos( ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            var banco = new DbControle();

            return banco.TipoBase.ToList();
        }
    }
}