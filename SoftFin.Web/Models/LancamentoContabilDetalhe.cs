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
    public class LancamentoContabilDetalhe: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "LancamentoContabil"), Required(ErrorMessage = "Informe o estabelecimento")]
        public int lancamentoContabil_id { get; set; }

        [JsonIgnore, ForeignKey("lancamentoContabil_id")]
        public virtual LancamentoContabil LancamentoContabil { get; set; }

        [Display(Name = "Conta Contabil"), Required(ErrorMessage = "Informe a Conta Contabil")]
        public int contaContabil_id { get; set; }

        [JsonIgnore, ForeignKey("contaContabil_id")]
        public virtual ContaContabil ContaContabil { get; set; }

        [Display(Name = "Débito ou Crédito(D/C)"), StringLength(1)]
        public string DebitoCredito { get; set; }

        [Display(Name = "Valor"), Required(ErrorMessage = "informe o valor")]
        public decimal valor { get; set; }


        

        private bool validaExistencia(DbControle db, LancamentoContabilDetalhe obj = null)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase, DbControle db = null)
        {
            return Incluir(this, paramBase, db);
        }
        public bool Incluir(LancamentoContabilDetalhe obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<LancamentoContabilDetalhe>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }
        public bool Alterar(LancamentoContabilDetalhe obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

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
                    db.LancamentoContabilDetalhe.Remove(obj);
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


        public LancamentoContabilDetalhe ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public LancamentoContabilDetalhe ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.LancamentoContabilDetalhe.Where(x => x.id == id && x.LancamentoContabil.estabelecimento_id == paramBase.estab_id ).FirstOrDefault();
        }

        public List<LancamentoContabil> ObterTodos(ParamBase paramBase)
        {

            var banco = new DbControle();

            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == paramBase.estab_id).ToList();
        }

        public List<LancamentoContabil> ObterTodosDataIniDataFim(ParamBase paramBase, DateTime ini, DateTime fim)
        {

            var banco = new DbControle();

            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == paramBase.estab_id 
            && DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(ini) 
            && DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(fim)).ToList();
        }


        public List<LancamentoContabilDetalhe> ObterPorCPAGParcela(int id, ParamBase pb, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();


            return banco.LancamentoContabilDetalhe.Where(x => x.LancamentoContabil.estabelecimento_id 
            == pb.estab_id && x.LancamentoContabil.DocumentoPagarParcela_id == id).ToList();
        }

        public List<LancamentoContabilDetalhe> ObterPorCPAG(int id, ParamBase pb, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();


            return banco.LancamentoContabilDetalhe.Where(x => 
            x.LancamentoContabil.estabelecimento_id == pb.estab_id 
            && x.LancamentoContabil.DocumentoPagarParcela.DocumentoPagarMestre_id == id).ToList();
        }

        public List<LancamentoContabilDetalhe> ObterPorCPAGParcelaPagamento(int id, ParamBase pb, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();
            

            return banco.LancamentoContabilDetalhe.Where(x => x.LancamentoContabil.estabelecimento_id == pb.estab_id && x.LancamentoContabil.pagamento_id == id).ToList();
        }

        public List<LancamentoContabilDetalhe> ObterPorCapa(int id, ParamBase pb, DbControle db)
        {
            return db.LancamentoContabilDetalhe.Where(x => x.LancamentoContabil.estabelecimento_id == pb.estab_id && x.lancamentoContabil_id == id).ToList();
        }

        public List<LancamentoContabilDetalhe> ObterPorNotaFiscal(int id, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();


            return db.LancamentoContabilDetalhe.Where(x => x.LancamentoContabil.estabelecimento_id == pb.estab_id && x.LancamentoContabil.notafiscal_id == id).ToList();
        }

        public List<LancamentoContabilDetalhe> ObterPorNotaFiscalRecebimento(int id, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            

            return db.LancamentoContabilDetalhe.Where(x => x.LancamentoContabil.estabelecimento_id == pb.estab_id && x.LancamentoContabil.recebimento_id == id ).ToList();
        }
    }
}