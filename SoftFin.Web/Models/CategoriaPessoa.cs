using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace SoftFin.Web.Models
{
    public class CategoriaPessoa
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Descrição"), Required(ErrorMessage = "Campo Descrição obrigatório"), MaxLength(50)]
        public string Descricao { get; set; }
        public virtual IEnumerable<Pessoa> Pessoas { get; set; }


        ////Exclui uma Categoria Pessoa
        //public void ExcluiCategoriaPessoa(int ID)
        //{
        //    DbControle banco = new DbControle();
        //    CategoriaPessoa Excluir = new CategoriaPessoa();
        //    Excluir = banco.CategoriaPessoas.Where(x => x.id == ID).First();
        //    banco.Set<CategoriaPessoa>().Remove(Excluir);
        //    banco.SaveChanges();

        //}
        ////Inclui uma Categoria Pessoa
        //public bool IncluiCategoriaPessoa(CategoriaPessoa obj)
        //{
        //    DbControle banco = new DbControle();
        //    //Verificar se usuário já existe no banco de dados
        //    if (banco.Perfis.Where(x => x.Descricao == obj.Descricao).Count() != 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        banco.CategoriaPessoas.Add(obj);
        //        banco.SaveChanges();
        //        return true;
        //    }
        //}
        ////Altera uma Categoria Pessoa
        //public void AlteraPerfil(CategoriaPessoa obj)
        //{
        //    DbControle banco = new DbControle();
        //    CategoriaPessoa Salvar = new CategoriaPessoa();
        //    Salvar = banco.CategoriaPessoas.Where(x => x.id == obj.id).First();
        //    Salvar.Descricao = obj.Descricao;
        //    banco.SaveChanges();
        //}
        ////Lista Perfil
        //public IEnumerable<CategoriaPessoa> listaCategoriaPessoa()
        //{
        //    DbControle banco = new DbControle();
        //    IEnumerable<CategoriaPessoa> lista = banco.CategoriaPessoas.ToList();
        //    return lista;
        //}


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
                    db.CategoriaPessoa.Remove(obj);
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

        public bool Alterar(CategoriaPessoa obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, CategoriaPessoa obj, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db.CategoriaPessoa.Where(x => x.Descricao == obj.Descricao).Count() != 0)
            {
                return true;
            }

            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(CategoriaPessoa obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<CategoriaPessoa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public CategoriaPessoa ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public CategoriaPessoa ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.CategoriaPessoa.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<CategoriaPessoa> ObterTodos(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.CategoriaPessoa.ToList();
        }
  

    }
}