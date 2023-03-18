using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class ContatoSite
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Email"), EmailAddress, Required(ErrorMessage = "*"), MaxLength(200)]
        public string Email { get; set; }

        [Display(Name = "Telefone"),  Required(ErrorMessage = "*"), MaxLength(50)]
        public string Telefone { get; set; }

        [Display(Name = "Nome"), Required(ErrorMessage = "*"), MaxLength(70)]
        public string Nome { get; set; }

        [Display(Name = "Assunto"), Required(ErrorMessage = "*"), MaxLength(70)]
        public string Assunto { get; set; }

        [Display(Name = "Mensagem"), Required(ErrorMessage = "*"), MaxLength(1500)]
        public string Mensagem { get; set; }

        [Display(Name = "Data"), ]
        public DateTime Data { get; set; }
        

        public bool Excluir(int id, ParamBase pb)
        {

            DbControle db = new DbControle();
            var obj = ObterPorId(id, db);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.ContatoSite.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ContatoSite obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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
        public bool Incluir()
        {
            return Incluir(this);
        }
        public bool Incluir(ContatoSite obj)
        {
            DbControle banco = new DbControle();

            if (validaExistencia(banco, obj))
                return false;
            else
            {

                banco.Set<ContatoSite>().Add(obj);
                banco.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle banco, ContatoSite obj)
        {
            return (false);
        }
        public ContatoSite ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public ContatoSite ObterPorId(int id, DbControle banco)
        {
            
            if (banco ==null)
                banco = new DbControle();

            return banco.ContatoSite.Where(x => x.id == id).FirstOrDefault();
        }
        public List<ContatoSite> ObterTodos()
        {
            
            DbControle banco = new DbControle();
            return banco.ContatoSite.ToList();
        }


    }
}