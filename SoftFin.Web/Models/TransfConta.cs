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
    public class TransfConta: BaseModels
    {
        [Key]
        public int Id { get; set; }

        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public int BancoSaida_Id { get; set; }
        [JsonIgnore, ForeignKey("BancoSaida_Id")]
        public virtual Banco BancoSaida { get; set; }

        public int BancoEntrada_Id { get; set; }
        [JsonIgnore, ForeignKey("BancoEntrada_Id")]
        public virtual Banco BancoEntrada { get; set; }


        public int UnidadeSaida_Id { get; set; }
        [JsonIgnore, ForeignKey("UnidadeSaida_Id")]
        public virtual UnidadeNegocio UnidadeSaida { get; set; }


        public int UnidadeEntrada_Id { get; set; }
        [JsonIgnore, ForeignKey("UnidadeEntrada_Id")]
        public virtual UnidadeNegocio UnidadeEntrada { get; set; }




        public int BancoMovimentoSaida_Id { get; set; }
        [JsonIgnore, ForeignKey("BancoMovimentoSaida_Id")]
        public virtual BancoMovimento BancoMovimentoSaida { get; set; }


        public int BancoMovimentoEntrada_Id { get; set; }
        [JsonIgnore, ForeignKey("BancoMovimentoEntrada_Id")]
        public virtual BancoMovimento BancoMovimentoEntrada { get; set; }
        public DateTime Data { get; set; }

        [NotMapped]
        public int Estab { get; set; }
        public int UsuarioInclusao_Id { get; internal set; }
        public DateTime DataInclusao { get; internal set; }
        public int UsuarioAlteracao_Id { get; internal set; }
        public DateTime DataAlteracao { get; internal set; }

        public IQueryable<TransfConta> ObterTodos(ParamBase paramBase)
        {
            DbControle banco = new DbControle();
            return banco.TransfConta.Where(x => x.BancoEntrada.estabelecimento_id == paramBase.estab_id);
        }

        public bool Alterar(ParamBase pb, DbControle db)
        {
            return Alterar(this, pb, db);
        }


        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb, null);
        }

        public bool Alterar(TransfConta obj, ParamBase pb)
        {
            return Alterar(obj, pb, null);
        }

        public bool Alterar(TransfConta obj, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.Id);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(TransfConta obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();


            new LogMudanca().Incluir(obj, "", "", banco, pb);
            banco.Set<TransfConta>().Add(obj);
            banco.SaveChanges();
            return true;
        }

        public TransfConta ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }

        public TransfConta ObterPorId(int id, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.TransfConta.Where(x => x.Id == id).FirstOrDefault();
        }


        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();

                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {

                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.TransfConta.Remove(obj);
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
    }
}