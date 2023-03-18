using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class        BancoMovimentoLanctoExtratoUsuario
    {
        [Key]
        public int id { get; set; }

        [DisplayName("Data Emissão"),
        Required(ErrorMessage = "*")]
        [DataType(DataType.Date)]
        public DateTime emissao { get; set; }

        [Display(Name = "Usuario"), MaxLength(50)]
        public string Codigo { get; set; }

        [Display(Name = "Usuario"), MaxLength(50)]
        public string Tipo { get; set; }

        [Display(Name = "Usuario"), MaxLength(250)]
        public string UsuarioLogado { get; set; }

        [Display(Name = "Valor")]
        public decimal Valor { get; set; }



        public bool Excluir(ParamBase pb)
        {
            string Erro = "";
            return Excluir(this.id, ref Erro, pb);
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
                    db.BancoMovimentoLanctoExtratoUsuario.Remove(obj);
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

        public bool Alterar(BancoMovimentoLanctoExtratoUsuario obj, ParamBase pb)
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

        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }

        public bool Incluir(BancoMovimentoLanctoExtratoUsuario obj, ParamBase pb)
        {
            var db = new DbControle();

            var objAux = db.BancoMovimentoLanctoExtratoUsuario.Where(x => x.id == obj.id).FirstOrDefault();
            if (objAux != null)
            {
                return false;
            }
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<BancoMovimentoLanctoExtratoUsuario>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }


        public BancoMovimentoLanctoExtratoUsuario ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }

        public BancoMovimentoLanctoExtratoUsuario ObterPorId(int id, DbControle banco)
        {

            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimentoLanctoExtratoUsuario.Where(x => x.id == id).FirstOrDefault();
        }

        public List<BancoMovimentoLanctoExtratoUsuario> ObterTodosUsuario()
        {
            DbControle banco = new DbControle();
            var usu = Acesso.UsuarioLogado();
            return banco.BancoMovimentoLanctoExtratoUsuario.Where(x => x.UsuarioLogado == usu).ToList();
        }
        public List<BancoMovimentoLanctoExtratoUsuario> ObterTodosUsuario(DbControle banco)
        {
            var usu = Acesso.UsuarioLogado();
            return banco.BancoMovimentoLanctoExtratoUsuario.Where(x => x.UsuarioLogado == usu).ToList();
        }

        public BancoMovimentoLanctoExtratoUsuario ObterUsuarioCodigo(string codigo)
        {
            DbControle banco = new DbControle();
            var usu = Acesso.UsuarioLogado();
            return banco.BancoMovimentoLanctoExtratoUsuario.Where(x => x.UsuarioLogado == usu && x.Codigo == codigo).FirstOrDefault();
        }

    }
}