using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class Processo: BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        [MaxLength(50)]
        public string CodigoProcessoTemplate { get; set; }
        [MaxLength(50)]
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int IdEmpresa { get; set; }
        [JsonIgnore, ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
        public int Contador { get;  set; }

        private bool ValidaExistencia(DBGPControle db, Processo obj)
        {
            return db.Processo.Where(x => x.Codigo == obj.Codigo).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Processo obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<Processo>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(Processo obj, ParamBase pb)
        {
            var db = new DBGPControle();

            var objAux = ObterPorId(obj.Id, pb);
            if (objAux == null)
                return false;
            else
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                var db = new DBGPControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    db.Processo.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }
        public Processo ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Processo ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.Processo.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<Processo> ObterTodos(ParamBase pb)
        {
            var db = new DBGPControle();
            return db.Processo.Where(x => x.IdEmpresa == pb.Empresa).ToList();
        }
    }
}