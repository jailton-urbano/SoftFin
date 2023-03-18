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
    public class LojaFechamento: BaseModels
    {
        public LojaFechamento()
        {
            LojaFechamentoCCs = new List<LojaFechamentoCC>();
        }
        
        public int id { get; set; }

        [Required(ErrorMessage = "Informe a data do fechamento")]
        public DateTime dataFechamento { get; set; }

        [Required(ErrorMessage = "Informe a sequncia do fechamento")]
        public int sequencia { get; set; }

        public decimal saldoInicial { get; set; }

        public decimal valorBruto { get; set; }

        public decimal valorLiquido { get; set; }

        public decimal valorTaxas { get; set; }

        public decimal saldoFinal { get; set; }


        [Required(ErrorMessage = "Informe a descrição do Fechamento"), MaxLength(50)]
        public string descricao { get; set; }

        [Required(ErrorMessage = "ativo")]
        public bool ativo { get; set; }

        public int LojaOperador_id { get; set; }

        [JsonIgnore,ForeignKey("LojaOperador_id")]
        public virtual LojaOperador LojaOperador { get; set; }


        public int LojaCaixa_id { get; set; }

        [JsonIgnore,ForeignKey("LojaCaixa_id")]
        public virtual LojaCaixa LojaCaixa { get; set; }


        public int Loja_id { get; set; }

        [JsonIgnore,ForeignKey("Loja_id")]
        public virtual Loja Loja { get; set; }

        public List<LojaFechamentoCC> LojaFechamentoCCs { get; set; }



        [MaxLength(1)]
        public string flgSituacao { get; set; }
        //L lançado, C conferido e F fechado

        private bool validaExistencia(DbControle db, LojaFechamento obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase, DbControle db = null)
        {
            return Incluir(this, paramBase, db);
        }
        public bool Incluir(LojaFechamento obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db,paramBase);

                db.Set<LojaFechamento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(LojaFechamento obj, ParamBase paramBase)
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

        public bool Excluir(ref string erro, ParamBase paramBase, DbControle db = null)
        {
            return Excluir(this.id, ref erro, paramBase, db);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase, DbControle db = null)
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
                    db.LojaFechamento.Remove(obj);
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


        public LojaFechamento ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public LojaFechamento ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.LojaFechamento.Where(x => x.id == id && x.LojaCaixa.Loja.estabelecimento_id == idestab).FirstOrDefault();
        }

        public List<LojaFechamento> ObterTodos(ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            var banco = new DbControle();

            return banco.LojaFechamento.Where(x => x.LojaCaixa.Loja.estabelecimento_id == idestab).ToList();
        }

        public List<LojaFechamento> ObterTodosPorData(DateTime datFechamento, int loja, ParamBase paramBase, DbControle db = null)
        {
            int idestab = paramBase.estab_id;
            if (db == null)
                db = new DbControle();
            return db.LojaFechamento.Where(x => x.LojaCaixa.Loja.estabelecimento_id == idestab
                                && x.dataFechamento == datFechamento
                                && x.Loja_id == loja).ToList();
        }

        public List<LojaFechamento> ObterTodosByidLojaDataInicialFinal(int idLoja, DateTime dataIni, DateTime dataFin, ParamBase paramBase, DbControle db = null)
        {
            int idestab = paramBase.estab_id;
            if (db == null)
                db = new DbControle();
            return db.LojaFechamento.Where(x => x.LojaCaixa.Loja.estabelecimento_id == idestab
                                && x.dataFechamento >= dataIni
                                && x.dataFechamento <= dataFin
                                && x.Loja_id == idLoja).ToList();
        }
    }
}