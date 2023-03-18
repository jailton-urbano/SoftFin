using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.GestorProcessos.Helper;
using System.Data.Entity;

namespace SoftFin.GestorProcessos.Models
{
    public class Tabela: BaseModel
    {
        public Tabela()
        {
            TabelaCampos = new List<TabelaCampo>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(250)]

        public string Nome { get; set; }

        [MaxLength(250)]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public bool CadastroAuxiliar { get; set; }


        [MaxLength(300)]
        public string SQLCadastroAuxiliar { get; set; }

        public int OrdemCriacao { get; set; }

        public int IdEmpresa { get; set; }

        [JsonIgnore, ForeignKey("IdEmpresa")]

        public virtual Empresa Empresa { get; set; }

       
        public virtual ICollection<TabelaCampo> TabelaCampos { get; set; }

        private bool ValidaExistencia(DBGPControle db, Tabela obj)
        {
            return db.Tabela.Where(x => x.Nome == obj.Nome).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Tabela obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<Tabela>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar( ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(Tabela obj, ParamBase pb)
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
                    db.Tabela.Remove(obj);
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
        public Tabela ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Tabela ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.Tabela.Where(x => x.Id == id ).Include(p => p.TabelaCampos).FirstOrDefault();
        }
        public List<Tabela> ObterTodos(ParamBase pb)
        {
            var db = new DBGPControle();
            return db.Tabela.Where(x => x.IdEmpresa == pb.Empresa).Include(p => p.TabelaCampos).ToList();
        }
    }
}