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
    public class BancoMovimentoConfig
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }


        [Display(Name = "Banco NFEs")]
        public int? bancoNFEs_id { get; set; }
        [JsonIgnore,ForeignKey("bancoNFEs_id")]
        public virtual Banco BancoNFEs { get; set; }

        [Display(Name = "Plano de Contas NFEs")]
        public int? planoDeContaNFEs_id { get; set; }
        [JsonIgnore,ForeignKey("planoDeContaNFEs_id")]
        public virtual PlanoDeConta PlanoDeContaNFEs { get; set; }

        [Display(Name = "Tipo de Movimento NFEs")]
        public int? tipoDeMovimentoNFEs_id { get; set; }
        [JsonIgnore,ForeignKey("tipoDeMovimentoNFEs_id")]
        public virtual TipoMovimento TipoMovimentoNFEs { get; set; }

        [Display(Name = "Tipo de Documento NFEs")]
        public int? tipoDeDocumentoNFEs_id { get; set; }
        [JsonIgnore,ForeignKey("tipoDeDocumentoNFEs_id")]
        public virtual TipoDocumento TipoDocumentoNFEs { get; set; }

        [Display(Name = "Origem Movimento NFEs")]
        public int? origemmovimentoNFEs_id { get; set; }
        [JsonIgnore,ForeignKey("origemmovimentoNFEs_id")]
        public virtual OrigemMovimento OrigemMovimentoNFEs { get; set; }




        //public bool Excluir(int id, ref string erro, ParamBase pb)
        //{
        //    try
        //    {
        //        int estab = pb.estab_id;
        //        DbControle db = new DbControle();
        //        var obj = ObterPorId(id, db);
        //        if (obj == null)
        //        {
        //            erro = "Registro não encontrado";
        //            return false;
        //        }
        //        else
        //        {
        //            new LogMudanca().Incluir(obj, "", "",db, pb);
        //            db.BancoMovimentoConfig.Remove(obj);
        //            db.SaveChanges();
        //            return true;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
        //        {
        //            erro = "Registro esta relacionado com outro cadastro";
        //            return false;
        //        }
        //        else
        //        {
        //            throw ex;
        //        }
        //    }
        //}

        //public bool Alterar(BancoMovimentoConfig obj)
        //{
        //    DbControle banco = new DbControle();

        //    var objAux = ObterPorId(obj.id);
        //    if (objAux == null)
        //        return false;
        //    else
        //    {
        //        new LogMudanca().Incluir(obj, objAux, "",db, pb);
        //        banco.Entry(obj).State = EntityState.Modified;
        //        banco.SaveChanges();

        //        return true;
        //    }
        //}


        //public bool Incluir(BancoMovimentoConfig obj)
        //{
        //    var banco = new DbControle();
         
        //    var objAux = banco.BancoMovimentoConfig.Where(x => x.empresa_id == obj.empresa_id
        //        && x.origemmovimentoNFEs_id == obj.origemmovimentoNFEs_id).FirstOrDefault();
        //    if (objAux != null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        new LogMudanca().Incluir(obj, "", "",db, pb);

        //        banco.Set<BancoMovimentoConfig>().Add(obj);
        //        banco.SaveChanges();
        //        return true;
        //    }
        //}

        //public BancoMovimentoConfig ObterPorId(int id)
        //{
        //    return ObterPorId(id, null);
        //}

        //public BancoMovimentoConfig ObterPorId(int id, DbControle banco)
        //{
        //    int idempresa  = pb.empresa_id;
        //    if (banco == null)
        //        banco = new DbControle();

        //    return banco.BancoMovimentoConfig.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        //}
        //public List<BancoMovimentoConfig> ObterTodos()
        //{
        //    int idempresa  = pb.empresa_id;
        //    DbControle banco = new DbControle();
        //    return banco.BancoMovimentoConfig.Where(x => x.empresa_id == idempresa).ToList();
        //}


        //public BancoMovimentoConfig Obter()
        //{
        //    int idempresa  = pb.empresa_id;
        //    var banco = new DbControle();

        //    return banco.BancoMovimentoConfig.Where(x => x.empresa_id == idempresa).FirstOrDefault();
        //}

    }
}