using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class Holding : GenericRepository<Holding>
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Código"), Required(ErrorMessage = "*"),StringLength(10)]
        public string codigo { get; set; }
        [Display(Name = "Nome"), Required(ErrorMessage = "Campo nome da holding obrigatório ou tamanho informado foi superior a 50 caracteres"),StringLength(50)]
        public string nome { get; set; }
        
        [Display(Name = "Bloqueada"), Required(ErrorMessage = "Campo Bloqueado inválido")]
        public bool bloqueada { get; set; }

        [Display(Name = "Motivo Bloqueada"), StringLength(70)]
        public string motivobloqueada { get; set; }

        [Display(Name = "Código na API de pagamento"), StringLength(100)]
        public string codigoApiPagamento { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id,pb, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db,pb);
                    db.Holding.Remove(obj);
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

        public bool Alterar(ParamBase pb, Holding obj = null, DbControle db = null)
        {
            if (obj == null)
                obj = this;

            if (db == null)
                db = new DbControle();


            var objAux = ObterPorId(obj.id, pb, db);
            if (objAux == null)
                return false;
            else
            {
                //new LogMudanca().Incluir(obj, objAux, "", db,pb);
                objAux.bloqueada = obj.bloqueada;
                objAux.codigo = obj.codigo;
                objAux.motivobloqueada = obj.motivobloqueada;
                objAux.nome = obj.nome;
                objAux.codigoApiPagamento = obj.codigoApiPagamento;
                db.Entry(objAux).State = EntityState.Modified;
                db.SaveChanges();
                Update(obj);

                return true;
            }
        }


        private bool validaExistencia(DbControle db, Holding obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb,  DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(Holding obj, ParamBase pb, DbControle banco = null)
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
                //new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Holding>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Holding ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, pb, null);
        }
        public Holding ObterPorId(int id, ParamBase pb, DbControle db)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Holding.Where( p => p.id == id).FirstOrDefault();
        }

        public List<Holding> ObterTodos(ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            List<Holding> holds = new List<Holding>();
            holds.Add(new Empresa().ObterPorId(empresa).Holding);

            return holds;
        }

        public List<Holding> ObterTodosSF()
        {
            DbControle db = new DbControle();
            var holds = db.Holding.ToList();
            return holds;
        }
    }
}