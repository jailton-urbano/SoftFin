using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class DeParaItem : BaseModels
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "De"), Required(ErrorMessage = "Informe De"), MaxLength(300)]
        public string De { get; set; }
        [Display(Name = "Para"), Required(ErrorMessage = "Informe Para"), MaxLength(300)]
        public string Para { get; set; }
        [Display(Name = "Contrato"), Required(ErrorMessage = "Informe o contrato relacionado")]
        public int DePara_Id { get; set; }
        [JsonIgnore, ForeignKey("DePara_Id")]
        public virtual DeParaMestre DeParaMestre { get; set; }

        [NotMapped]
        public bool deleted { get; set; }

        public List<DeParaItem> ObterPorIdMestre(int id, ParamBase pb, DbControle banco = null)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.DeParaItem.Where(x => x.DePara_Id == id && x.DeParaMestre.Estabelecimento_id == estab).ToList();

        }


        public bool Excluir(ParamBase pb, DbControle db = null)
        {
            return Excluir(this.Id, pb, db);
        }

        public bool Excluir(int id, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();

                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.DeParaItem.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
                {
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb, db);
        }
        public bool Alterar(DeParaItem obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.Id, db, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);

                db.Entry(objAux).CurrentValues.SetValues(obj);
                db.Entry(objAux).State = EntityState.Modified;

                db.SaveChanges();

                return true;
            }
        }


        private bool ValidaExistencia(DbControle db, DeParaItem obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(DeParaItem obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<DeParaItem>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public DeParaItem ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public DeParaItem ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DeParaItem.Where(x => x.Id == id && x.DeParaMestre.Estabelecimento_id == estab).FirstOrDefault();
        }
        public List<DeParaItem> ObterTodos(int idmestre, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DeParaItem.Where(x => x.DePara_Id == idmestre ).ToList();
        }

    }
}