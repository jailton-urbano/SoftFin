using Newtonsoft.Json;
using SoftFin.Utils;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SoftFin.Web.Negocios
{
    public enum TipoAcesso
    {
        NENHUM = 0,
        TOTAL = 1,
        CONSULTA = 2
    }
    

    public static class Acesso
    {

        //[OutputCache(Duration = 3, VaryByParam = "none")]
        public static List<UsuarioAviso> Avisos()
        {
            var emp = EmpresaLogado2();
            var usu = RetornaIdUsuario(UsuarioLogado());
            var avisos = new UsuarioAviso().ObterTodosNaoLidos(usu, emp).ToList();
            return avisos;
        }


        public static bool isUsuarioValido(string usuario, string senha)
        {
            var y = new Usuario().ValidarUsuario(usuario, senha);
            return (y);
        }

        public static List<Funcionalidade> RetornaFuncionalidadesPrincipais(string usuario)
        {
            var listret = new List<Funcionalidade>();
            var relfunc = new Funcionalidade().ObterPaisAtivos();
            foreach (var item in relfunc)
            {
                listret.Add(item); 
            }
            return listret.OrderBy(p => p.Ordem).ToList();
        }


        public static List<SistemaMenu> RetornaMenusPrincipais(string usuario)
        {
            var listret = new List<Funcionalidade>();

            var menu = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<List<SistemaMenu>>("SistemaMenu");

            if (menu == null)
                menu = new SoftFin.Web.Models.SistemaMenu().ObterTodosAtivos();

            SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("SistemaMenu", menu);

            

            if (usuario.ToUpper() != "JARVIS")
                menu = menu.Where(p => p.Descricao.ToUpper() != "JARVIS").ToList();

            return menu;
        }

        //[OutputCache(Duration = 60, VaryByParam = "none")]
        public static List<Funcionalidade> RetornaFuncionalidadesPorId(string usuario,int id)
        {
            
            var listret = new List<Funcionalidade>();
            var usuaitem = new Usuario().ObterPorCodigoAtivo(usuario);//db.Usuarios.Where(p => p.codigo.Equals(usuario)).FirstOrDefault();

            if (usuaitem == null)
                return null;

            var relfunc = new PerfilFuncionalidade().ObterPorIdPerfilIdPai(usuaitem.idPerfil, id);


            foreach (var item in relfunc)
            {
                listret.Add(item.Funcionalidade);
            }

            return listret.OrderBy(p => p.Ordem).ToList();
        }


        
        //[OutputCache(Duration = 60, VaryByParam = "none")]
        public static List<SistemaDashBoard> RetornaMenuPorId(string usuario, int id)
        {

            var menu = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<List<SistemaDashBoard>>("SistemaDashBoard.id:" + id.ToString());

            if (menu == null)
                menu = new SistemaDashBoard().ObterTodosPorIdSistemaMenu(id);

            SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("SistemaDashBoard.id:" + id.ToString(), menu);

            return menu.OrderBy(p => p.Codigo).ToList();
        }

        //[OutputCache(Duration = 60, VaryByParam = "none")]
        public static List<Funcionalidade> RetornaFavoritos()
        {
            var relfunc = new UsuarioFavorito().ObterTodosPorUsuario(Acesso.idUsuarioLogado());
            return relfunc.OrderBy(p => p.Funcionalidade.Descricao).Select(p => p.Funcionalidade).ToList();
        }

        


        //[OutputCache(Duration = 60, VaryByParam = "none")]
        public static List<PerfilFuncionalidade> RetornaNovasFuncionalidadesFilhoPorPai(int idPai)
        {
   

            var relfunc = new Funcionalidade().ObterFilhosAtivos(idPai).OrderBy(p => p.Ordem).ToList();

            List<PerfilFuncionalidade> listret = new List<PerfilFuncionalidade>();

            foreach (var item in relfunc)
            {
                PerfilFuncionalidade x = new PerfilFuncionalidade();

                x.flgTipoAcesso = "T";
                x.Funcionalidade = item;
                listret.Add(x);
            }

            return listret;
        }

        public static List<PerfilFuncionalidade> RetornaFuncionalidadesFilhoPorPai(int idPai, int idPerfil)
        {


            var relfunc = new Funcionalidade().ObterFilhosAtivos(idPai).OrderBy(p => p.Ordem).ToList();

            List<PerfilFuncionalidade> listret = new List<PerfilFuncionalidade>();

            foreach (var item in relfunc)
            {
                //var jacadastrado = db.PerfilFuncionalidade.Where(
                //    p => p.Funcionalidade.id == item.id && p.idPerfil == idPerfil).FirstOrDefault();

                var jacadastrado = new PerfilFuncionalidade().ObterPorIdFuncionalidadeIdPerfil(item.id, idPerfil);

                if (jacadastrado == null)
                {
                    PerfilFuncionalidade x = new PerfilFuncionalidade();
                    x.flgTipoAcesso = "N";
                    x.Funcionalidade = item;
                    listret.Add(x);
                }
                else
                {
                    listret.Add(jacadastrado);
                }
            }

            return listret;
        }
        public static TipoAcesso RetornaTipoFuncionalidades(string usuario, string cdeFuncionalidade)
        {
            var db = new DbControle();
            
            var funcitem = db.Funcionalidade.Where(p => p.id.Equals(cdeFuncionalidade)).FirstOrDefault();
            var usuaitem = db.Usuarios.Where(p => p.nome.Equals(usuario)).FirstOrDefault();

            if (funcitem == null)
                return TipoAcesso.NENHUM;
            if (usuaitem == null)
                return TipoAcesso.NENHUM;

            var z = db.PerfilFuncionalidade.Where(
                p => p.idFuncionalidade.Equals(funcitem.id) 
                    && p.idPerfil.Equals(usuaitem.idPerfil)
                ).FirstOrDefault();

            if (z == null)
                return TipoAcesso.NENHUM;


            if (z.flgTipoAcesso.Equals("T"))
                return TipoAcesso.TOTAL;
            if (z.flgTipoAcesso.Equals("C"))
                return TipoAcesso.CONSULTA;
            else
                return TipoAcesso.NENHUM;
        }


        public static int RetornaIdUsuario(string usuario, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var y = db.Usuarios.Where(p => p.codigo.Equals(usuario)).FirstOrDefault();
            return y.id;
        }
        public static Usuario RetornaObjUsuario(string usuario, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            var usuarioobj = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<Usuario>("usuario.codigo:" + usuario);
            if (usuarioobj != null)
                return usuarioobj;

            usuarioobj = db.Usuarios.Where(p => p.codigo.Equals(usuario)).FirstOrDefault();
            return usuarioobj;
        }
        public static string RetornaSenhaUsuario(string usuario)
        {
            var db = new DbControle();
            var y = db.Usuarios.Where(p => p.codigo.Equals(usuario)).FirstOrDefault();

            if (y != null)
                return y.senha;
            else
                return null;
        }

        public static string UsuarioLogado()
        {
            var user = System.Web.HttpContext.Current.User.Identity.Name.Split('&');

            return user.FirstOrDefault();
        }
        public static int EstabLogado2()
        {
            var user = System.Web.HttpContext.Current.User.Identity.Name.Split('&');
            if (user.Count() <= 1)
                return 0;
            else
                return int.Parse(user[1]);
        }

        public static Estabelecimento EstabLogadoObj2()
        {
            var db = new DbControle();
            int u = EstabLogado2();
            if (u == 0)
                return null;

            return db.Estabelecimento.Include(p => p.Empresa).Where(p => p.id.Equals(u)).ToList().FirstOrDefault();
        }

        //public static string EstabLogoLogado()
        //{
        //    try{
        //        var db = new DbControle();
        //        int u = EstabLogadoX();
        //        var y = db.Estabelecimento.Where(p => p.id.Equals(u)).ToList().FirstOrDefault();
        //        if (y.Logo == null)
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return y.Logo;
        //        }

        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        public static int EmpresaLogado2()
        {
            var db = new DbControle();
            string usu = UsuarioLogado();
            if (usu == "")
                return 0;
            return db.Usuarios.Where(p => p.codigo == usu).FirstOrDefault().empresa_id;
        }
        
        public static string PerfilLogado(DbControle db = null)
        {
            string usu = UsuarioLogado();
            if (db == null)
                db = new DbControle();

            var perfilobj = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<Perfil>("perfil.usuario:" + usu);

            if (perfilobj != null)
                return perfilobj.Descricao;

            var usuaitem = SoftFin.Utils.UtilSoftFin.CacheSF.GetItem<Usuario>("usuario.codigo:" + usu);

            if (usuaitem == null)
            {
                usuaitem = db.Usuarios.Where(p => p.codigo.Equals(usu)).FirstOrDefault();
                SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("usuario.codigo:" + usu, usuaitem);
                SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("usuario.id:" + usuaitem.id.ToString(), usuaitem);
            }

            if (usuaitem == null)
                return "";
            var perfil = db.Perfil.Find(usuaitem.idPerfil);

            SoftFin.Utils.UtilSoftFin.CacheSF.AddItem("perfil.usuario:" + usu, perfil);

            return perfil.Descricao;
        }
        public static int idPerfilLogado()
        {
            string usu = UsuarioLogado();
            var db = new DbControle();
            var usuaitem = db.Usuarios.Where(p => p.codigo.Equals(usu)).FirstOrDefault();
            if (usuaitem == null)
                return 0;
            else
                return usuaitem.idPerfil;
        }

        public static int idUsuarioLogado()
        {
            string usu = UsuarioLogado();
            var db = new DbControle();
            var usuaitem = db.Usuarios.Where(p => p.codigo.Equals(usu)).FirstOrDefault();
            if (usuaitem == null)
                return 0;
            else
                return usuaitem.id;
        }

        public static bool isUsuarioLogado()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public static void Logar(string Nome)
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(Nome, false);
        }
        public static void Logar(string nome, int estab)
        {

            System.Web.Security.FormsAuthentication.SetAuthCookie(nome + "&" + estab.ToString(), false);
        }
        public static void Logar(int estab)
        {
            string nome = UsuarioLogado();
            System.Web.Security.FormsAuthentication.SetAuthCookie(nome + "&" + estab.ToString() , false);
        }
        public static void Deslogar()
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }

        public static TipoAcesso TemAcesso(String usuario, String nomeController)
        {


            var funcitem = new Funcionalidade().ObterNomeControllerAtivo(nomeController); //.Where(p => p.NomeController == nomeController).FirstOrDefault();
            var usuaitem = new Usuario().ObterPorCodigoAtivo(usuario); //db.Usuarios.Where(p => p.codigo.Equals(usuario)).FirstOrDefault();

            if (funcitem == null)
                return TipoAcesso.NENHUM;
            if (usuaitem == null)
                return TipoAcesso.NENHUM;




            var z = new PerfilFuncionalidade().ObterPorIdFuncionalidadeIdPerfil(funcitem.id, usuaitem.idPerfil);

            if (z == null)
                return TipoAcesso.NENHUM;


            if (z.flgTipoAcesso.Equals("T"))
                return TipoAcesso.TOTAL;
            if (z.flgTipoAcesso.Equals("C"))
                return TipoAcesso.CONSULTA;
            else
                return TipoAcesso.NENHUM;
        }


        public static void EnviarEmail(string email)
        {
            var senha = RetornaSenhaUsuario(email);

            if (senha != null)
            {
                new Email().EnviarMensagem(email,
                        "SoftFin esqueci a senha " ,
                        "Sua senha atual é: " + senha);
            }
        }


        public static List<Funcionalidade> RetornaFuncionalidades(string texto)
        {
            var listret = new List<Funcionalidade>();
            var usuario = UsuarioLogado();
            var usuaitem = new Usuario().ObterPorCodigoAtivo(usuario); //db.Usuarios.Where(p => p.nome == usuario).FirstOrDefault();

            if (usuaitem == null)
                return null;

            var relfunc = new PerfilFuncionalidade().ObterPorIdPerfilIdPai(usuaitem.idPerfil, null).Where(p => p.Funcionalidade.Descricao.ToUpper().Contains(texto.ToUpper()));
                //db.PerfilFuncionalidade.Where
               // p => (p.idPerfil == usuaitem.idPerfil && p.Funcionalidade.idPai != null && p.Funcionalidade.Descricao.Contains(texto))
               // ).ToList();

            foreach (var item in relfunc)
            {
                listret.Add(item.Funcionalidade);
            }

            return listret.OrderBy(p => p.Descricao).ToList();
        }

        public static List<Funcionalidade> RetornaNivelFuncionalidades()
        {
            var listret = new List<Funcionalidade>();
            var usuario = UsuarioLogado();
            var usuaitem = new Usuario().ObterPorNomeAtivo(usuario); //db.Usuarios.Where(p => p.nome == usuario).FirstOrDefault();

            if (usuaitem == null)
                return null;

            var relfunc = new PerfilFuncionalidade().ObterPorIdPerfilIdPai(usuaitem.idPerfil, null);
            //db.PerfilFuncionalidade.Where
            // p => (p.idPerfil == usuaitem.idPerfil && p.Funcionalidade.idPai != null && p.Funcionalidade.Descricao.Contains(texto))
            // ).ToList();

            foreach (var item in relfunc)
            {
                listret.Add(item.Funcionalidade);
            }

            return listret.OrderBy(p => p.Descricao).ToList();
        }

        public static int getEmpresaUsuario(string codigo)
        {
            var db = new DbControle();
            return db.Usuarios.Where(p => p.codigo == codigo).FirstOrDefault().empresa_id;
        }

        public static void TrocaSenhaLogado(string senhaNovaConfirmacao)
        {
            var db = new DbControle();
            var usuario = UsuarioLogado();
            var y = db.Usuarios.Where(p => p.codigo.Equals(usuario)).FirstOrDefault();

            y.senha = senhaNovaConfirmacao;
            db.SaveChanges();
        }
    }
}