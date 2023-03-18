using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using SoftFin.Web.Negocios;

namespace SoftFin.Web.Models
{
    public class StatusParcela
    {
        public int id { get; set; }


        [Required(ErrorMessage = "Campo Status obrigatório")]
        public string status { get; set; }

        public virtual IEnumerable<StatusParcela> StatusParcelas { get; set; }

        public StatusParcela()
        {
        }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
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
                    db.StatusParcela.Remove(obj);
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

        public bool Alterar(StatusParcela obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, StatusParcela obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(StatusParcela obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<StatusParcela>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public StatusParcela ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public StatusParcela ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            return db.StatusParcela.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<StatusParcela> ObterTodos()
        {

            DbControle db = new DbControle();
            return db.StatusParcela.ToList();
        }

        public static int SituacaoLiberada()
        {

            DbControle db = new DbControle();
            var situacao = db.StatusParcela.Where(x => x.status.Equals("Liberada")).FirstOrDefault();

            if (situacao == null)
                throw new Exception("Situação Liberda não encontrada.");
            
            return situacao.id;
        }

        public static int SituacaoPendente()
        {

            DbControle db = new DbControle();
            var situacao = db.StatusParcela.Where(x =>  x.status.Equals("Pendente")).FirstOrDefault();

            if (situacao == null)
                throw new Exception("Situação Liberda não encontrada.");

            return situacao.id;
        }
        

        public static int SituacaoEmitida()
        {

            DbControle db = new DbControle();
            var situacao = db.StatusParcela.Where(x => x.status.Equals("Emitida")).FirstOrDefault();

            if (situacao == null)
                throw new Exception("Situação Emitida não encontrada.");

            return situacao.id;
        }

        public int ObterStatusManual()
        {
            DbControle db = new DbControle();
            var situacao = db.StatusParcela.Where(x => x.status.Equals("Manual")).FirstOrDefault();

            if (situacao == null)
                throw new Exception("Situação Manual não encontrada.");

            return situacao.id;
        }
    }
}