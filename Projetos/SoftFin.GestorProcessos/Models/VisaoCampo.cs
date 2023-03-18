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
    public class VisaoCampo: BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int IdTabelaCampo { get; set; }

        [JsonIgnore, ForeignKey("IdTabelaCampo")]
        public virtual TabelaCampo TabelaCampo { get; set; }

        public bool Ativo { get; set; }

        public bool Visivel { get; set; }

        public bool Transferivel { get; set; }



        public int IdVisao { get; set; }

        [JsonIgnore, ForeignKey("IdVisao")]
        public virtual Visao Visao { get; set; }

        [MaxLength(150)]
        public string PadraoSalva { get; set; }

        [MaxLength(150)]
        public string ReferenciaNgModel{ get; set; }

        [MaxLength(150)]
        public string ReferenciaNgChange { get; set; }


        private bool ValidaExistencia(DBGPControle db, VisaoCampo obj)
        {
            return false;
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(VisaoCampo obj, ParamBase pb)
        {
            var db = new DBGPControle();
            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                db.Set<VisaoCampo>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }
        public bool Alterar(VisaoCampo obj, ParamBase pb)
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
                    db.VisaoCampo.Remove(obj);
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


        public VisaoCampo ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public VisaoCampo ObterPorId(int id, DBGPControle db, ParamBase pb)
        {
            if (db == null)
                db = new DBGPControle();

            return db.VisaoCampo.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<VisaoCampo> ObterTodos(int idVisao,ParamBase pb)
        {
            var db = new DBGPControle();
            return db.VisaoCampo.Where(x => x.Visao.IdEmpresa == pb.Empresa & x.IdVisao == idVisao).ToList();
        }
    }
}