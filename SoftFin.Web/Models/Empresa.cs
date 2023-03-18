using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class Empresa :BaseModels
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Nome Fantasia"), Required(ErrorMessage = "Campo nome fantasia obrigatório ou tamanho informado foi superior a 50 caracteres"), StringLength(50)]
        public string apelido { get; set; }
        [Display(Name = "Código"), Required(ErrorMessage = "Campo código da empresa obrigatório ou tamanho informado foi superior a 10 caracteres"), StringLength(10)]
        public string codigo { get; set; }
        [Display(Name = "Nome"), Required(ErrorMessage = "Campo nome da empresa obrigatório ou tamanho informado foi superior a 50 caracteres"), StringLength(50)]
        public string nome { get; set; }
        [Display(Name = "Holding")]
        public int Holding_id { get; set; }

        [JsonIgnore,ForeignKey("Holding_id")]
        public virtual Holding Holding { get; set; }



        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Empresa.Remove(obj);
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

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb, db);
        }

        public bool Alterar(Empresa obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,db);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                objAux.apelido = obj.apelido;
                objAux.codigo = obj.codigo;
                objAux.Holding_id = obj.Holding_id;
                objAux.nome = obj.nome;
                db.Entry(objAux).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, Empresa obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(Empresa obj, ParamBase pb, DbControle banco = null)
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
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<Empresa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public Empresa ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Empresa ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.Empresa.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<Empresa> ObterTodos(ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.Empresa.Where(x => x.id == empresa).ToList();
        }


        public Empresa ObterPorHoldId(int id, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.Empresa.Where(x => x.Holding_id == id).ToList().FirstOrDefault();
        }
    }
}