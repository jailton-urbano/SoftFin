using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Runtime.Caching;

namespace SoftFin.Web.Models
{

    public class Usuario : BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }
        
        [Required(ErrorMessage = "Código obrigatório")]
        public string codigo { get; set; }

        [Required(ErrorMessage = "Nome obrigatório")]
        public string nome { get; set; }

        [Required(ErrorMessage = "Senha obrigatória")]
        public string senha { get; set; }

        [Required(ErrorMessage = "Código obrigatório")]
        public int logado { get; set; }

        [Required(ErrorMessage = "Código obrigatório")]
        public int idPerfil { get; set; }

        [JsonIgnore,ForeignKey("idPerfil")]
        public virtual Perfil Perfil { get; set; }

        [Display(Name = "Usuário Bloqueado")]
        public bool usuarioBloqueado { get; set; }
        
        [NotMapped]
        public bool alterarsenha { get; set; }

        [MaxLength(150)]
        public string tokenApi { get; set; }



        


        public override List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState = null)
        {
            DbControle db;
            db = new DbControle();
            var erros = base.Validar(ModelState);
            if (validaExistencia(db, this))
            {
                erros.Add("Este usuário já existe no sistema, por favor escolha outro");
            }
            return erros;
        }
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
                    db.Usuarios.Remove(obj);
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

        public bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }

        public bool Alterar(Usuario obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,db);
            if (objAux == null)
                return false;
            else
            {
                if (paramBase != null)
                    new LogMudanca().Incluir(obj, objAux,"Alteração de Usuário" , db, paramBase );

                if (obj.alterarsenha )
                {
                    objAux.senha = obj.senha;
                }
                objAux.codigo = obj.codigo;
                objAux.empresa_id = obj.empresa_id;
                objAux.idPerfil = obj.idPerfil;
                objAux.logado = obj.logado;
                objAux.nome = obj.nome;
                objAux.id = obj.id;
                objAux.usuarioBloqueado = obj.usuarioBloqueado;
                objAux.tokenApi = obj.tokenApi;
                db.Entry(objAux).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, Usuario obj)
        {
            return (db.Usuarios.Where(x => x.codigo.ToUpper() == obj.codigo.ToUpper() && x.id != obj.id).Count() >= 1);
        }
        public bool Incluir(ParamBase paramBase,  DbControle banco = null)
        {
            return Incluir(this,paramBase, banco);
        }
        public bool Incluir(Usuario obj, ParamBase pb, DbControle banco = null)
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

                db.Set<Usuario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Usuario ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public Usuario ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.Usuarios.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<Usuario> ObterTodosUsuariosAtivos(ParamBase paramBase)
        {
            DbControle db = new DbControle();
            return db.Usuarios.Where(x => x.empresa_id == paramBase.empresa_id && x.usuarioBloqueado == false).ToList();
        }
        public List<Usuario> ObterTodos(ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            DbControle db = new DbControle();
            return db.Usuarios.Where(x => x.empresa_id == idempresa).ToList();
        }
        public Usuario ObterPorCodigo(string codigo)
        {
            DbControle db = new DbControle();
            return db.Usuarios.Where(x => x.codigo == codigo).FirstOrDefault();
        }

        public List<Usuario> ObterTodosEmail()
        {
            DbControle db = new DbControle();
            return db.Usuarios.ToList();
        }

        public bool ValidarUsuario(string usuario, string senha)
        {
            DbControle db = new DbControle();
            var usu = db.Usuarios.Where(p => p.codigo.Equals(usuario) && p.senha.Equals(senha) && p.usuarioBloqueado == false && p.Empresa.Holding.bloqueada == false).ToList();

            if (usu.Count() > 1)
            {
                throw new Exception("Usuário duplicado na base de dados.");
            }
            if (usu.Count() == 1)
            {
                if (usu.First().usuarioBloqueado)
                    return false;
                else
                    return true;
            }
            return false;
        }

        public Usuario ObterPorCodigoAtivo(string usuario, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            return db.Usuarios.Where(p => p.codigo.Equals(usuario) && p.usuarioBloqueado == false).FirstOrDefault();
        }

        public Usuario ObterPorNomeAtivo(string nome, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            return db.Usuarios.Where(p => p.nome.Equals(nome) && p.usuarioBloqueado == false).FirstOrDefault();
        }

        public object ObterTodosPorUsuario(bool p)
        {
            throw new NotImplementedException();
        }

        public List<Usuario> ObterPorEmpresa(int id)
        {
            DbControle db = new DbControle();
            return db.Usuarios.Where(p => p.empresa_id == id).ToList();
        }
    }

}