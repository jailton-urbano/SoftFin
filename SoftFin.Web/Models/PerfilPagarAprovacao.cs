using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SoftFin.Web.Models
{
    public class PerfilPagarAprovacao
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Usuário Autorizador:")]
        public int? usuarioAutorizador_id { get; set; }
        [JsonIgnore,ForeignKey("usuarioAutorizador_id")]
        public virtual Usuario usuarioAutorizador { get; set; }

        [Display(Name = "Valor Limite Contas a Pagar"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valorLimiteCPAG { get; set; }

        [Display(Name = "Valor Limite NFS-e"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valorLimiteNFSE { get; set; }

        [Display(Name = "Inativo")]
        public bool Inativo { get; set; }

        [NotMapped]
        public bool valor { get; set; }

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
                    db.PerfilPagarAprovacao.Remove(obj);
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

        public bool Alterar(ParamBase pb, PerfilPagarAprovacao obj)
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


        private bool validaExistencia(DbControle db, PerfilPagarAprovacao obj, ParamBase pb)
        {
            int estab = pb.estab_id;
            return (ObterTodos(pb).Where(p => p.estabelecimento_id == estab && p.usuarioAutorizador_id == obj.usuarioAutorizador_id).Count() > 0);

        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(PerfilPagarAprovacao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<PerfilPagarAprovacao>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public PerfilPagarAprovacao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public PerfilPagarAprovacao ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.PerfilPagarAprovacao.Where(x => x.id == id).FirstOrDefault();
        }
        public List<PerfilPagarAprovacao> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var ppa = db.PerfilPagarAprovacao.Where(x => x.estabelecimento_id == estab).ToList();
            return ppa;
        }

        public PerfilPagarAprovacao ObterPorEstabUsuario(int estabid, int usuarioid, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            return db.PerfilPagarAprovacao.Where(x => x.usuarioAutorizador_id == usuarioid && x.estabelecimento_id == estabid).FirstOrDefault();
        }


        public List<PerfilPagarAprovacao> ObterTodosPorIdUsuario(int usuarioid)
        {
            
            DbControle db = new DbControle();
            return db.PerfilPagarAprovacao.Where(x => x.usuarioAutorizador_id == usuarioid).ToList();
        }

    }
}
