using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Runtime.Caching;

namespace SoftFin.Web.Models
{
    public class Perfil : BaseModels
    {
        public int id { get; set; }


        [Display(Name = "Estabelecimento"), MaxLength(15)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Campo Descrição obrigatório"), MaxLength(150)]
        public string Descricao { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Usuario> Usuarios { get; set; }
        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Perfil.Remove(obj);
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

        public bool Alterar(Perfil obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, Perfil obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(Perfil obj, ParamBase pb, DbControle banco = null)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
                db = banco;

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Perfil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Perfil ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Perfil ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int emp  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.Perfil.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<Perfil> ObterTodos(ParamBase pb)
        {
            int emp  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.Perfil.Where(x => x.empresa_id == emp).ToList();
        }

        public List<Perfil> ObterPadrao(string padrao)
        {

            DbControle db = new DbControle();
            return db.Perfil.Where(x => x.Descricao.Contains(padrao)
                ).ToList();
        }


        public List<Perfil> ObterPorEmpresa(int id)
        {
            DbControle db = new DbControle();
            return db.Perfil.Where(x => x.empresa_id == id 
                ).ToList();
        }


    }
}