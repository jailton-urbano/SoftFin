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
    public class Funcionalidade
    {
        public Funcionalidade()
        {
            Ativo = true;
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "Descrição"),Required(ErrorMessage = "*")]
        public string Descricao { get; set; }

        [Display(Name = "Codigo Pai")]
        public int? idPai { get; set; }

        [Display(Name = "Controller")]
        public string NomeController { get; set; }
        [Display(Name = "Action")]
        public string NomeAction { get; set; }

        [Display(Name = "Ordem"),Required(ErrorMessage = "*")]
        public int Ordem { get; set; }


        [Display(Name = "Nome Relatório")]
        public string NomeRelatorio { get; set; }

        [Display(Name = "Icone")]
        public string Icone { get; set; }

        [Display(Name = "Indexado")]
        public bool Indexado { get; set; }

        [Display(Name = "Comentario")]
        public string Comentario { get; set; }

        [Display(Name = "Ativo")]
        public bool? Ativo { get; set; }

        [Display(Name = "Jarvis")]
        public bool? Jarvis { get; set; }

        //[Display(Name = "Código"), Required(ErrorMessage = "*")]
        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Funcionalidade.Remove(obj);
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

        public bool Alterar(Funcionalidade obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
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


        private bool validaExistencia(DbControle db, Funcionalidade obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Funcionalidade obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Funcionalidade>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Funcionalidade ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Funcionalidade ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Funcionalidade.Where(x => x.id == id).FirstOrDefault();
        }
        public List<Funcionalidade> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Funcionalidade.ToList();
        }


        public List<Funcionalidade> ObterPaisAtivos()
        {
            DbControle db = new DbControle();
            return  db.Funcionalidade.Where(
                    p => p.idPai.Equals(null) && p.Ativo == true
                    ).ToList();

        }

        public List<Funcionalidade> ObterFilhosAtivos(int idPai)
        {
            DbControle db = new DbControle();
            var retorno = db.Funcionalidade.Where(
                    p => p.idPai == idPai && p.Ativo == true
                    ).ToList();
            return retorno;

        }

        public Funcionalidade ObterNomeControllerAtivo(string nomeController)
        {
            DbControle db = new DbControle();
            return db.Funcionalidade.Where(p => p.NomeController == nomeController && p.Ativo == true).FirstOrDefault();
        }
    }
}