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
    public class Funcao:BaseModel
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        public int IdEmpresa { get; set; }
        [JsonIgnore, ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }

        [NotMapped]
        public bool Selecionado { get; set; }



        private bool ValidaExistencia(DBGPControle db, Funcao obj)
        {
            return false;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Funcao obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<Funcao>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(Funcao obj, ParamBase pb)
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
                    db.Funcao.Remove(obj);
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
        public Funcao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Funcao ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.Funcao.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<Funcao> ObterTodos(ParamBase pb)
        {
            var db = new DBGPControle();
            return db.Funcao.Where(p => p.Empresa.Id == pb.Empresa).ToList();
        }

    }
}