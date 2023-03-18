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
    public class TabelaCampo:BaseModel
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(250)]
        public string Campo { get; set; }

        [MaxLength(250)]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public int Tabela_Id { get; set; }

        [JsonIgnore, ForeignKey("Tabela_Id")]
        [InverseProperty("TabelaCampos")]
        public virtual Tabela Tabela { get; set; }

        public int IdTipoCampo { get; set; }

        [JsonIgnore, ForeignKey("IdTipoCampo")]
        public virtual TipoCampo TipoCampo { get; set; }


        [MaxLength(12)]
        public string TamanhoColuna { get; set; }

        [MaxLength(10)]
        public string TamanhoCampo { get; set; }

        public int Precisao { get; set; }

        public int Ordem { get; set; }

        public bool Obrigatorio { get; set; }

        [MaxLength(500)]
        public string SQLDefault { get; set; }

        public int? IdChaveEstrageira { get; set; }

        [JsonIgnore, ForeignKey("IdChaveEstrageira")]
        public virtual Tabela ChaveEstrageira { get; set; }


        private bool ValidaExistencia(DBGPControle db, TabelaCampo obj)
        {
            return db.TabelaCampo.Where(x => x.Campo == obj.Campo).FirstOrDefault() != null;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(TabelaCampo obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<TabelaCampo>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(TabelaCampo obj, ParamBase pb)
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
                    db.TabelaCampo.Remove(obj);
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


        public TabelaCampo ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public TabelaCampo ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.TabelaCampo.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<TabelaCampo> ObterTodos(int tabela_id)
        {
            var db = new DBGPControle();
            return db.TabelaCampo.Where(x => x.Tabela_Id == tabela_id).ToList();
        }

    }
}