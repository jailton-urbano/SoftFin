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
    public class LojaFechamentoCC: BaseModels
    {
        public int id { get; set; }


        public decimal valorBruto { get; set; }

        public decimal valorLiquido { get; set; }

        public decimal valorTaxa { get; set; }


        [Required(ErrorMessage = "Informe a descrição do Fechamento"), MaxLength(50)]
        public string descricao { get; set; }

        [Required(ErrorMessage = "Tipo Movimento"), MaxLength(1)]
        public string tipoMovimento { get; set; }

        [Required(ErrorMessage = "Tipo Venda"), MaxLength(1)]
        public string tipoVenda { get; set; }
        //A Avista Prazo

        public int LojaFechamento_id { get; set; }

        [JsonIgnore,ForeignKey("LojaFechamento_id")]
        public virtual LojaFechamento LojaFechamento { get; set; }

        public int LojaTipoRecebimentoCaixa_id { get; set; }

        [JsonIgnore,ForeignKey("LojaTipoRecebimentoCaixa_id")]
        public virtual LojaTipoRecebimentoCaixa LojaTipoRecebimentoCaixa { get; set; }


        private bool validaExistencia(DbControle db, LojaFechamentoCC obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(LojaFechamentoCC obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",null,paramBase);

                db.Set<LojaFechamentoCC>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(LojaFechamentoCC obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", null, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool Excluir(ref string erro, ParamBase paramBase, DbControle db= null)
        {
            return Excluir(this.id, ref erro, paramBase, db);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase, DbControle db=null)
        {
            try
            {
                if (db == null)
                    db = new DbControle();
                
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.LojaFechamentoCC.Remove(obj);
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


        public LojaFechamentoCC ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public LojaFechamentoCC ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;

            if (banco == null)
                banco = new DbControle();

            return banco.LojaFechamentoCC.Where(x => x.id == id && x.LojaFechamento.LojaCaixa.Loja.estabelecimento_id == idestab).FirstOrDefault();
        }

        public List<LojaFechamentoCC> ObterTodos(int lojaFechamento_id, ParamBase paramBase, DbControle banco = null)
        {
            int idempresa = paramBase.empresa_id;
            
            if (banco == null)
                banco = new DbControle();

            return banco.LojaFechamentoCC.Where(x => x.LojaFechamento_id == lojaFechamento_id).ToList();
        }

        public int prazoDias { get; set; }

        public decimal taxa { get; set; }

        public DateTime dataPagamentoPrevisto { get; set; }

        public int banco_id { get; set; }
    }
}