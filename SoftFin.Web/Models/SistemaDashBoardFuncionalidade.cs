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
    public class SistemaDashBoardFuncionalidade : BaseModels
    {
        [Key]
        public int id { get; set; }


        [StringLength(75)]
        public string Descricao { get; set; }
        public bool ativo { get; set; }

        public int sistemaDashBoard_id { get; set; }

        [JsonIgnore,ForeignKey("sistemaDashBoard_id")]
        public virtual SistemaDashBoard SistemaDashBoard { get; set; }

        public int Funcionalidade_id { get; set; }

        [JsonIgnore,ForeignKey("Funcionalidade_id")]
        public virtual Funcionalidade Funcionalidade { get; set; }
        public bool cadastro { get; set; }

        public bool? relatorio { get; set; }

        [StringLength(200)]
        public String Texto { get; set; }

        [StringLength(35)]
        public String Icone { get; set; }

        [StringLength(200)]
        public String Action { get; set; }

        [StringLength(200)]
        public String Controller { get; set; }


        [StringLength(35)]
        public string cor { get; set; }


        [StringLength(150)]
        public string metodoTotalizador { get; set; }


        public int? Ordem { get; set; }

        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterTodos(ParamBase pb)
        {
            DbControle db = new DbControle();

            var    objs = (from p in db.SistemaDashBoardFuncionalidade
                        
                        select p).ToList();
            return objs;

        }

        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterTodoPorPerfil(int id, ParamBase pb)
        {
            DbControle db = new DbControle();
            var idPerfil = pb.perfil_id;
            var usuario = pb.usuario_name;

            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs;
            if (usuario.ToUpper().Equals("JARVIS"))
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                        where p.ativo == true && p.sistemaDashBoard_id == id
                        select p).ToList();
            }
            else
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                        join b in db.PerfilFuncionalidade
                             on p.Funcionalidade_id equals b.idFuncionalidade
                        where b.idPerfil == idPerfil && p.ativo == true && p.sistemaDashBoard_id == id 
                        && b.Funcionalidade.Jarvis == false
                        select p).Distinct().ToList();
            }
            return objs.ToList();

        }


        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterTodosPorDashBoardId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            var idPerfil = Acesso.idPerfilLogado();
            var usuario = Acesso.UsuarioLogado();
            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs;
            if (usuario.ToUpper().Equals("JARVIS"))
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                       where p.ativo == true && p.sistemaDashBoard_id == id && p.cadastro == false && p.relatorio == false
                       select p).ToList();
            }
            else
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                       join b in db.PerfilFuncionalidade
                            on p.Funcionalidade_id equals b.idFuncionalidade
                       where b.idPerfil == idPerfil && p.ativo == true && p.sistemaDashBoard_id == id && p.cadastro == false && p.relatorio == false
                       && b.Funcionalidade.Jarvis == false
                       select p).Distinct().ToList();
            }
            return objs.ToList();

        }

        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterPorIdRelatorios(int id)
        {
            DbControle db = new DbControle();
            var idPerfil = Acesso.idPerfilLogado();
            var usuario = Acesso.UsuarioLogado();
            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs;
            if (usuario.ToUpper().Equals("JARVIS"))
            {

                objs = (from p in db.SistemaDashBoardFuncionalidade
                       join b in db.PerfilFuncionalidade
                            on p.Funcionalidade_id equals b.idFuncionalidade
                       where b.idPerfil == idPerfil && p.ativo == true && p.sistemaDashBoard_id == id && p.relatorio == true
                       select p).ToList();
            }
            else
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                       join b in db.PerfilFuncionalidade
                            on p.Funcionalidade_id equals b.idFuncionalidade
                       where b.idPerfil == idPerfil && p.ativo == true && p.sistemaDashBoard_id == id && p.relatorio == true && b.Funcionalidade.Jarvis == false
                       select p).Distinct().ToList();

            }
            return objs.ToList();
        }

        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterPorIdCadastros(int id)
        {
            DbControle db = new DbControle();

            var idPerfil = Acesso.idPerfilLogado();
            var usuario = Acesso.UsuarioLogado();
            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs;
            if (usuario.ToUpper().Equals("JARVIS"))
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                       join b in db.PerfilFuncionalidade
                            on p.Funcionalidade_id equals b.idFuncionalidade
                       where b.idPerfil == idPerfil && p.ativo == true && p.sistemaDashBoard_id == id && p.cadastro == true
                       select p).ToList();
            }
            else
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                       join b in db.PerfilFuncionalidade
                            on p.Funcionalidade_id equals b.idFuncionalidade
                       where b.idPerfil == idPerfil && p.ativo == true && p.sistemaDashBoard_id == id && p.cadastro == true && p.Funcionalidade.Jarvis == false
                       select p).Distinct().ToList();
            }
            return objs;
        }

        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterPorNomeController(string _nameController)
        {
            DbControle db = new DbControle();
            var usuario = Acesso.UsuarioLogado();
            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs;
            objs = db.SistemaDashBoardFuncionalidade.Where(
                        p => p.ativo == true).ToList();
            return objs.Where(
                p => p.Funcionalidade.NomeController.ToUpper() == _nameController.ToUpper()
                ).ToList();

        }




        public List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> ObterPorIdFuncionalidade(int id, DbControle db =null)
        {
            if (db == null)
                db = new DbControle();
            
            var idPerfil = Acesso.idPerfilLogado();
            var usuario = Acesso.UsuarioLogado();
            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs = new List<SistemaDashBoardFuncionalidade>();
            if (usuario.ToUpper().Equals("JARVIS"))
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                           join b in db.PerfilFuncionalidade
                               on p.Funcionalidade_id equals b.idFuncionalidade
                           where b.idPerfil == idPerfil && p.ativo == true && p.Funcionalidade_id == id
                           select p).ToList();
            }
            else
            {
                objs = (from p in db.SistemaDashBoardFuncionalidade
                       join b in db.PerfilFuncionalidade
                           on p.Funcionalidade_id equals b.idFuncionalidade
                       where b.idPerfil == idPerfil && p.ativo == true && p.Funcionalidade_id == id && p.Funcionalidade.Jarvis == false
                       select p).ToList();
            }



            return objs;
        }


        public List<SistemaDashBoardFuncionalidade> ObterPorIdDash(int id)
        {
            DbControle db = new DbControle();
            var usuario = Acesso.UsuarioLogado();
            List<SoftFin.Web.Models.SistemaDashBoardFuncionalidade> objs;
            objs = db.SistemaDashBoardFuncionalidade.Where(
                        p => p.sistemaDashBoard_id == id).ToList();
            return objs.ToList();
        }

        public SistemaDashBoardFuncionalidade ObterPorId(int id, DbControle db = null)
        {
            if (db == null)//
            db = new DbControle();

            var obj = db.SistemaDashBoardFuncionalidade.Where(
                        p => p.id == id).FirstOrDefault();
            return obj;
        }


        public bool Excluir(ParamBase pb, DbControle db = null)
        {
            return Excluir(this.id, pb, db);
        }
        public bool Excluir(int id, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();

                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.SistemaDashBoardFuncionalidade.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
                {
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
        public bool Alterar(SistemaDashBoardFuncionalidade obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, db);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);

                db.Entry(objAux).CurrentValues.SetValues(obj);
                db.Entry(objAux).State = EntityState.Modified;

                db.SaveChanges();

                return true;
            }
        }
        private bool ValidaExistencia(DbControle db, SistemaDashBoardFuncionalidade obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(SistemaDashBoardFuncionalidade obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<SistemaDashBoardFuncionalidade>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
    }
}