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
    public class DeParaMestre : BaseModels
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Código"), Required(ErrorMessage = "Informe o Código Identificação"), MaxLength(20)]
        public string Codigo { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "Informe a Descrição Identificação"), MaxLength(70)]
        public string Descricao { get; set; }

        public int Estabelecimento_id { get; set; }

        [JsonIgnore, ForeignKey("Estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [JsonIgnore]
        public List<DeParaItem> DeParaItems { get; set; }

        public DeParaMestre ObterPorCodigo(string codigoDePara, ParamBase pb, DbControle banco = null)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();
            var x = banco.DeParaMestre.ToList();
            return x.Where(p => p.Codigo == codigoDePara).FirstOrDefault();

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
                    db.DeParaMestre.Remove(obj);
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
        public bool Alterar(DeParaMestre obj, ParamBase pb, DbControle db = null)
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


        private bool ValidaExistencia(DbControle db, DeParaMestre obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(DeParaMestre obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<DeParaMestre>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public DeParaMestre ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public DeParaMestre ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DeParaMestre.Where(x => x.Id == id && x.Estabelecimento_id == estab).FirstOrDefault();
        }
        public List<DeParaMestre> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DeParaMestre.Where(x => x.Estabelecimento_id == estab).ToList();
        }
    }
}