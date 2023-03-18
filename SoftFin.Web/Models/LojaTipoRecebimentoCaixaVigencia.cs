using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class LojaTipoRecebimentoCaixaVigencia : BaseModels
    {
        public int id { get; set; }

        [MaxLength(150)]
        public string historico { get; set; }

        [Required(ErrorMessage = "Prazo em Dias")]
        public int prazoDias { get; set; }

        [Required(ErrorMessage = "Texa")]
        public decimal taxa { get; set; }

        public DateTime? dataFimVigencia { get; set; }

        public int LojaTipoRecebimentoCaixa_id { get; set; }

        [NotMapped]
        public bool deleted { get; set; }

        [JsonIgnore, ForeignKey("LojaTipoRecebimentoCaixa_id")]
        public virtual LojaTipoRecebimentoCaixa LojaTipoRecebimentoCaixa { get; set; }

        private bool validaExistencia(DbControle db, LojaTipoRecebimentoCaixaVigencia obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(LojaTipoRecebimentoCaixaVigencia obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db,paramBase);

                db.Set<LojaTipoRecebimentoCaixaVigencia>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(LojaTipoRecebimentoCaixaVigencia obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool Excluir(ref string erro, ParamBase paramBase)
        {
            return Excluir(this.id, ref erro, paramBase);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.LojaTipoRecebimentoCaixaVigencia.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
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


        public LojaTipoRecebimentoCaixaVigencia ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public LojaTipoRecebimentoCaixaVigencia ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.LojaTipoRecebimentoCaixaVigencia.Where(x => x.id == id ).FirstOrDefault();
        }

        //public List<LojaTipoRecebimentoCaixaVigencia> ObterTodos(ParamBase paramBase)
        //{

        //    var banco = new DbControle();

        //    return banco.LojaTipoRecebimentoCaixaVigencia.Where(x => x.Loja.estabelecimento_id == paramBase.estab_id).ToList();
        //}
        //public List<LojaTipoRecebimentoCaixaVigencia> ObterTodosPorLoja(int idLoja, ParamBase paramBase)
        //{
        //    int idestab = paramBase.estab_id;
        //    var banco = new DbControle();

        //    return banco.LojaTipoRecebimentoCaixaVigencia.Where(x => x.Loja.estabelecimento_id == idestab && x.Loja_id == idLoja).ToList();
        //}


        public LojaTipoRecebimentoCaixaVigencia ObterTipoVigente(int idTipoRecebimento, DateTime date, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            var banco = new DbControle();
            //var date = SoftFin.Utils.UtilSoftFin.DateBrasilia();

            var objs = banco.LojaTipoRecebimentoCaixaVigencia.Where(x => 
                x.LojaTipoRecebimentoCaixa_id == idTipoRecebimento && (x.dataFimVigencia >= date || x.dataFimVigencia == null)).OrderBy(p => p.dataFimVigencia).ToList();
                
            if (objs.Where(p => p.dataFimVigencia >= date).Count() > 0)
            {
                return objs.Where(p => p.dataFimVigencia >= date).First();
            }

            if (objs.Where(p => p.dataFimVigencia == null).Count() > 0)
            {
                return objs.First();
            }

            throw new Exception("Vigência de data não configurada");            

        }



    }
}