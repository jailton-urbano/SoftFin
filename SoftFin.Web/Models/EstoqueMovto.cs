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
    public class EstoqueMovto
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }
        
        [Display(Name = "Pessoa"),Required(ErrorMessage = "*")]
        public int pessoas_id { get; set; }
        [JsonIgnore,ForeignKey("pessoas_id")]
        public virtual Pessoa Pessoa { get; set; }

        public decimal ValorTotal { get; set; }

        [MaxLength(1),Required]
        public string TipoMovto { get; set; }
        //E-Entrada S-Saida
 
        [Display(Name = "Usuario Inclusão")]
        public int? usuarioinclusaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioinclusaoid")]
        public virtual Usuario UsuarioInclusao { get; set; }
        [Display(Name = "Usuario Alteração")]
        public int? usuarioalteracaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioalteracaoid")]
        public virtual Usuario UsuarioAlteracao { get; set; }

        public DateTime? dataInclusao { get; set; }

        public DateTime? dataAlteracao { get; set; }

        [MaxLength(70)]
        public string descricao { get; set; }


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
                    db.EstoqueMovto.Remove(obj);
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

        public bool Alterar(EstoqueMovto obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, EstoqueMovto obj)
        {
            return (false);
        }

        public bool Incluir(EstoqueMovto obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                var estab = pb.estab_id;
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<EstoqueMovto>().Add(obj);
                db.SaveChanges();
                return true;
            }
        }


        public EstoqueMovto ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public EstoqueMovto ObterPorId(int id, DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            return db.EstoqueMovto.Where(x => x.id == id).FirstOrDefault();
        }
        public List<EstoqueMovto> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.EstoqueMovto.Where(x => x.estabelecimento_id == estab).ToList();
        }



        public List<EstoqueMovto> ObterEntreData(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var situacaoliberada = StatusParcela.SituacaoLiberada();
            return db.EstoqueMovto.Where(x => x.estabelecimento_id == estab && x.dataInclusao >= DataInicial && x.dataInclusao <= DataFinal).ToList();
        }


    }
}