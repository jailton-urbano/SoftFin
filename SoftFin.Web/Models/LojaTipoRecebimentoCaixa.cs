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
    public class LojaTipoRecebimentoCaixa: BaseModels
    {
        public LojaTipoRecebimentoCaixa()
        {
            vigencias = new List<LojaTipoRecebimentoCaixaVigencia>();
        }

        public int id { get; set; }

        [Required(ErrorMessage = "Informe o código do Tipo Recebimento"), MaxLength(25)]
        public string codigo { get; set; }

        [Required(ErrorMessage = "Informe a descrição do Tipo Recebimento"), MaxLength(50)]
        public string descricao { get; set; }

        [Required(ErrorMessage = "ativo")]
        public bool ativo { get; set; }

        public int Loja_id { get; set; }

        [JsonIgnore,ForeignKey("Loja_id")]
        public virtual Loja Loja { get; set; }

        [Required(ErrorMessage = "Informe o banco"), Display(Name = "Banco")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }

        [JsonIgnore]
        public List<LojaTipoRecebimentoCaixaVigencia> vigencias { get; set; }


        private bool validaExistencia(DbControle db, LojaTipoRecebimentoCaixa obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(LojaTipoRecebimentoCaixa obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db,paramBase);

                db.Set<LojaTipoRecebimentoCaixa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase)
        {
            return Alterar(this, paramBase);
        }
        public bool Alterar(LojaTipoRecebimentoCaixa obj, ParamBase paramBase)
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
                    db.LojaTipoRecebimentoCaixa.Remove(obj);
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


        public LojaTipoRecebimentoCaixa ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public LojaTipoRecebimentoCaixa ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idempresa = paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.LojaTipoRecebimentoCaixa.Where(x => x.id == id ).Include(p => p.vigencias).FirstOrDefault();
        }

        public List<LojaTipoRecebimentoCaixa> ObterTodos(ParamBase paramBase)
        {

            var banco = new DbControle();

            return banco.LojaTipoRecebimentoCaixa.Where(x => x.Loja.estabelecimento_id == paramBase.estab_id).ToList();
        }
        public List<LojaTipoRecebimentoCaixa> ObterTodosPorLoja(int idLoja, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            var banco = new DbControle();

            return banco.LojaTipoRecebimentoCaixa.Where(x => x.Loja.estabelecimento_id == idestab && x.Loja_id == idLoja).ToList();
        }
        






    }
}