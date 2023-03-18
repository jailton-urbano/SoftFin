using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{

    public class ContaAssinatura
    {
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }
        
        [Required(ErrorMessage = "Data Inicial obrigatória")]
        public DateTime dataInicial { get; set; }
        
        [Required(ErrorMessage = "Data Final obrigatória")]
        public DateTime dataFinal { get; set; }
        
        [Required(ErrorMessage = "Valor")]
        public decimal valor { get; set; }
        
        [Required(ErrorMessage = "Nota Fiscal")]
        public int notaFiscal { get; set; }  
       
        [Required(ErrorMessage = "Código Verificação")]
        public string codigoVerificacao { get; set; }  
                
        [Required(ErrorMessage = "Link Nota Fiscal")]
        public string linkNotaFiscal { get; set; }     
        
        [Required(ErrorMessage = "Data de Vencimento")]
        public DateTime dataVencimentoNotaFiscal { get; set; }        
                
        [Required(ErrorMessage = "situação")]
        public string situacao { get; set; }
        
        [Required(ErrorMessage = "Histórico")]
        public string historico { get; set; }      
        

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
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
                    db.ContaAssinatura.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Impossível excluir, registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool Alterar(ContaAssinatura obj, ParamBase pb)
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

        public bool Incluir(ParamBase pb,  DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }

        private bool validaExistencia(DbControle db, ContaAssinatura obj, ParamBase pb)
        {
            return (db.ContaAssinatura.Where(x => x.dataInicial == obj.dataInicial).Count() >= 1);
        }

        public bool Incluir(ContaAssinatura obj, ParamBase pb, DbControle banco = null)
        {
            DbControle db;
            if (banco == null)
                db = new DbControle();
            else
                db = banco;

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.Set<ContaAssinatura>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public ContaAssinatura ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        
        public ContaAssinatura ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.ContaAssinatura.Where(x => x.id == id && x.empresa_id == idempresa).FirstOrDefault();
        }
        
        public List<ContaAssinatura> ObterTodos(ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.ContaAssinatura.Where(x => x.empresa_id == idempresa).ToList();
        }
        
        public List<ContaAssinatura> ObterTodosPeriodo(DateTime dataInicial, ParamBase pb)
        {
            int idempresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.ContaAssinatura.Where(x=> x.empresa_id == idempresa && x.dataInicial == dataInicial).ToList();
        }
    }

}