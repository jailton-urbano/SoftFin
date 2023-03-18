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
    public class SistemaPrincipalPerfil
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Perfil"), Required(ErrorMessage = "Informe o Perfil")]
        public int perfil_id { get; set; }

        [JsonIgnore, ForeignKey("perfil_id")]
        public virtual Perfil Perfil { get; set; }

        [Display(Name = "Sistema Principal"), Required(ErrorMessage = "Informe o Item do Menu Principal")]
        public int sistemaprincipal_id { get; set; }

        [JsonIgnore, ForeignKey("sistemaprincipal_id")]
        public virtual SistemaPrincipal SistemaPrincipal { get; set; }


        [Display(Name = "Json"), Required(ErrorMessage = "Informe o Json Principal"), MaxLength(4000)]
        public string Json { get; set; }


        private bool validaExistencia(DbControle db, SistemaPrincipalPerfil obj)
        {
            return (false);
        }
        internal bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        internal bool Incluir(SistemaPrincipalPerfil obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", null, paramBase);

                db.Set<SistemaPrincipalPerfil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        internal bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        internal bool Alterar(SistemaPrincipalPerfil obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", null, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        internal bool Excluir(ref string erro, ParamBase paramBase)
        {
            return Excluir(this.id, ref erro, paramBase);
        }
        internal bool Excluir(int id, ref string erro, ParamBase paramBase)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", null, paramBase);
                    db.SistemaPrincipalPerfil.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
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


        public SistemaPrincipalPerfil ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public SistemaPrincipalPerfil ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.SistemaPrincipalPerfil.Where(x => x.id == id).FirstOrDefault();
        }

        internal List<SistemaPrincipalPerfil> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            var banco = new DbControle();

            return banco.SistemaPrincipalPerfil.Where(x => x.perfil_id == paramBase.perfil_id).ToList();
        }
        internal List<SistemaPrincipalPerfil> ObterPorIdPerfil(int idPerfil)
        {
            var banco = new DbControle();
            return banco.SistemaPrincipalPerfil.Where(x => x.perfil_id == idPerfil).ToList();
        }
        internal SistemaPrincipalPerfil ObterPorIdPerfilSistemaPrincipal(int idPerfil, int idSistemaPrincipal)
        {
            var banco = new DbControle();
            return banco.SistemaPrincipalPerfil.Where(x => x.perfil_id == idPerfil 
                        && x.sistemaprincipal_id == idSistemaPrincipal).FirstOrDefault();
        }
    }
}