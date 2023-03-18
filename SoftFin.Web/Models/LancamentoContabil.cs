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
    public class LancamentoContabil: BaseModels
    {
        public LancamentoContabil()
        {
            LancamentoContabilDetalhes = new List<LancamentoContabilDetalhe>();
        }

        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o estabelecimento")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore, ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Data"), Required(ErrorMessage = "Informe a data")]
        public DateTime data { get; set; }

        [Display(Name = "Historico"),StringLength(500)]
        public string historico { get; set; }


        [Display(Name = "Usuario Inclusão")]
        public int? usuarioinclusaoid { get; set; }

        [JsonIgnore, ForeignKey("usuarioinclusaoid")]
        public virtual Usuario UsuarioInclusao { get; set; }

        [Display(Name = "Usuario Alteração")]
        public int? usuarioalteracaoid { get; set; }

        [JsonIgnore, ForeignKey("usuarioalteracaoid")]
        public virtual Usuario UsuarioAlteracao { get; set; }

        public DateTime? dataInclusao { get; set; }

        public DateTime? dataAlteracao { get; set; }

        [Display(Name = "Origem Movimento"),
                Required(ErrorMessage = "Informe a Origem do Movimento")]
        public int origemmovimento_id { get; set; }
        [JsonIgnore, ForeignKey("origemmovimento_id")]
        public virtual OrigemMovimento OrigemMovimento { get; set; }


        [Display(Name = "Documento Contas a Pagar")]
        public int? DocumentoPagarParcela_id { get; set; }
        [JsonIgnore, ForeignKey("DocumentoPagarParcela_id")]
        public virtual DocumentoPagarParcela DocumentoPagarParcela
        {
            get; set;
        }
        [Display(Name = "Nota Fiscal")]
        public int? notafiscal_id { get; set; }
        [JsonIgnore, ForeignKey("notafiscal_id")]
        public virtual NotaFiscal NotaFiscal { get; set; }
        
        [Display(Name = "Recebimento")]
        public int? recebimento_id { get; set; }
        [JsonIgnore, ForeignKey("recebimento_id")]
        public virtual Recebimento Recebimento { get; set; }

        [Display(Name = "Pagamento")]
        public int? pagamento_id { get; set; }
        [JsonIgnore, ForeignKey("pagamento_id")]
        public virtual Pagamento Pagamento { get; set; }

        [Display(Name = "Código Lançamento")]
        public int codigoLancamento { get; set; }

        public List<LancamentoContabilDetalhe> LancamentoContabilDetalhes { get; set; }


        [Display(Name = "Unidade de Negócio")]
        public int? UnidadeNegocio_ID { get; set; }

        [JsonIgnore, ForeignKey("UnidadeNegocio_ID")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }


        private bool validaExistencia(DbControle db, LancamentoContabil obj = null)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase, DbControle db = null)
        {
            return Incluir(this, paramBase, db);
        }
        public bool Incluir(LancamentoContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<LancamentoContabil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }
        public bool Alterar(LancamentoContabil obj, ParamBase paramBase, DbControle db = null)
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
                    db.LancamentoContabil.Remove(obj);
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


        public LancamentoContabil ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public LancamentoContabil ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.LancamentoContabil.Where(x => x.id == id && x.estabelecimento_id == paramBase.estab_id ).Include(p => p.LancamentoContabilDetalhes).FirstOrDefault();
        }

        public List<LancamentoContabil> ObterTodos(ParamBase paramBase)
        {

            var banco = new DbControle();

            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == paramBase.estab_id).Include(p => p.LancamentoContabilDetalhes).ToList();
        }

        public List<LancamentoContabil> ObterTodosDataIniDataFim(ParamBase paramBase, DateTime ini, DateTime fim)
        {

            var banco = new DbControle();

            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == paramBase.estab_id 
            && DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(ini) 
            && DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(fim)).Include(p => p.LancamentoContabilDetalhes).ToList();
        }


        public List<LancamentoContabil> ObterPorCPAGParcela(int id, ParamBase pb, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();


            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == pb.estab_id && x.DocumentoPagarParcela_id == id).ToList();
        }

        public List<LancamentoContabil> ObterPorCPAG(int id, ParamBase pb, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();


            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == pb.estab_id && x.DocumentoPagarParcela.DocumentoPagarMestre_id == id).ToList();
        }

        public List<LancamentoContabil> ObterPorCPAGParcelaPagamento(int id, ParamBase pb, DbControle banco)
        {
            if (banco == null)
                banco = new DbControle();
            

            return banco.LancamentoContabil.Where(x => x.estabelecimento_id == pb.estab_id && x.pagamento_id == id).ToList();
        }
        public List<LancamentoContabil> ObterPorNotaFiscal(int id, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();


            return db.LancamentoContabil.Where(x => x.estabelecimento_id == pb.estab_id && x.notafiscal_id == id).ToList();
        }

        public List<LancamentoContabil> ObterPorNotaFiscalRecebimento(int id, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            

            return db.LancamentoContabil.Where(x => x.estabelecimento_id == pb.estab_id && x.recebimento_id == id ).ToList();
        }
    }
}