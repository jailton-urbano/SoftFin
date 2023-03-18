using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    [Table("PerfilFuncionalidade")]
    public class PerfilFuncionalidade : GenericRepository<PerfilFuncionalidade>
    {
        [Key]
        public int id { get; set; }
        public int idPerfil { get; set; }
        public int idFuncionalidade { get; set; }
        public string flgTipoAcesso { get; set; }

        [JsonIgnore,ForeignKey("idFuncionalidade")]
        public virtual Funcionalidade Funcionalidade { get; set; }


        public List<PerfilFuncionalidade> ObterPorIdPerfil(int id)
        {
            return ObterPorIdPerfil(id, null);
        }
        public List<PerfilFuncionalidade> ObterPorIdPerfil(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.PerfilFuncionalidade.Where(x => x.idPerfil == id).ToList();
        }


        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }

        public bool Incluir(PerfilFuncionalidade obj, ParamBase pb, DbControle banco = null)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
            {
                db = banco;
            }
            if (validaExistencia(db, obj))
                return false;
            else
            {
                //new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<PerfilFuncionalidade>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        private bool validaExistencia(DbControle db, PerfilFuncionalidade obj)
        {
            return (false);
        }




        public List<PerfilFuncionalidade> ObterPorIdPerfilIdPai(int idPerfil, int? id)
        {
            DbControle db = new DbControle();
            if (id == null)
                return db.PerfilFuncionalidade.Where(p => (p.idPerfil == idPerfil && p.Funcionalidade.Ativo == true)).ToList();
            else
                return db.PerfilFuncionalidade.Where(p => (p.idPerfil == idPerfil && p.Funcionalidade.idPai == id && p.Funcionalidade.Ativo == true)).ToList();
        }

        public PerfilFuncionalidade ObterPorIdFuncionalidadeIdPerfil(int IdFuncionalidade, int IdPerfil)
        {
            DbControle db = new DbControle();
            return db.PerfilFuncionalidade.Where(
                p => p.idFuncionalidade.Equals(IdFuncionalidade)
                    && p.idPerfil.Equals(IdPerfil)
                    && p.Funcionalidade.Ativo == true
                ).FirstOrDefault();        }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = db.PerfilFuncionalidade.Find(id);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    //new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.PerfilFuncionalidade.Remove(obj);
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
    }
}