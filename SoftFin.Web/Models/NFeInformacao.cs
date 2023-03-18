using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NFeInformacao: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [MaxLength(50)]
        public string descricao { get; set; }
        
        [MaxLength(1500)]
        public string informacaoComplementar { get; set; }

        [MaxLength(1500)]
        public string informacaoComplementarFisco { get; set; }

        public bool Excluir(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var obj = ObterPorId(id, db, pb);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.NFeInformacao.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(NFeInformacao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(NFeInformacao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            new LogMudanca().Incluir(obj, "", "",db, pb);

            db.Set<NFeInformacao>().Add(obj);
            db.SaveChanges();

            return true;
            
        }

        public NFeInformacao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public NFeInformacao ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.NFeInformacao.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<NFeInformacao> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.NFeInformacao.Where(x => x.estabelecimento_id == estab).ToList();
        }
    }
}