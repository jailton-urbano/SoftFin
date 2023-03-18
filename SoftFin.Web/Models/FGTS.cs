using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class FGTSVLW
    {
        [Display(Name = "Código da Receita")]
        public int? codigoReceita { get; set; }

        [Display(Name = "Tipo de Inscrição")]
        public int? tipoInscricao { get; set; }

        [Display(Name = "CNPJ"),  MaxLength(14)]
        public string cnpj { get; set; }
        
        [Display(Name = "Código de Barras"),  MaxLength(48)]
        public string codigoBarras { get; set; }
        
        [Display(Name = "Identificador FGTS"),  MaxLength(16)]
        public string identificadorFgts { get; set; }
        
        [Display(Name = "Lacre Conectividade Social")]
        public int? lacreConectividadeSocial { get; set; }
        
        [Display(Name = "Dígita do Lacre")]
        public int? digitoLacre { get; set; }
        
        [Display(Name = "Nome do Contribuinte"),  MaxLength(30)]
        public string nomeContribuinte { get; set; }
        
        [Display(Name = "Data de Pagamento Inicial"), Required(ErrorMessage = "*")]
        public DateTime? dataPagamentoInicial { get; set; }
        
        [Display(Name = "Valor Pagamento Inicial"), Required(ErrorMessage = "*")]
        public Decimal? valorPagamentoInicial { get; set; }
        
        [Display(Name = "Data de Pagamento Final"), Required(ErrorMessage = "*")]
        public DateTime? dataPagamentoFinal { get; set; }
        
        [Display(Name = "Valor Pagamento Final"), Required(ErrorMessage = "*")]
        public Decimal? valorPagamentoFinal { get; set; }
    }
    public class FGTS
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Código da Receita"), Required(ErrorMessage = "*")]
        public int codigoReceita { get; set; }
        [Display(Name = "Tipo de Inscrição"), Required(ErrorMessage = "*")]
        public int tipoInscricao { get; set; }
        [Display(Name = "CNPJ"), Required(ErrorMessage = "*"), MaxLength(18)]
        public string cnpj { get; set; }
        [Display(Name = "Código de Barras"), Required(ErrorMessage = "*"), MaxLength(48)]
        public string codigoBarras { get; set; }
        [Display(Name = "Identificador FGTS"), Required(ErrorMessage = "*"), MaxLength(16)]
        public string identificadorFgts { get; set; }
        [Display(Name = "Lacre Conectividade Social"), Required(ErrorMessage = "*")]
        public int lacreConectividadeSocial { get; set; }
        [Display(Name = "Dígita do Lacre"), Required(ErrorMessage = "*")]
        public int digitoLacre { get; set; }
        [Display(Name = "Nome do Contribuinte"), Required(ErrorMessage = "*"), MaxLength(30)]
        public string nomeContribuinte { get; set; }
        [Display(Name = "Data de Pagamento"), Required(ErrorMessage = "*")]
        public DateTime dataPagamento { get; set; }
        [Display(Name = "Valor Pagamento"), Required(ErrorMessage = "*")]
        public Decimal valorPagamento { get; set; }
        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Documento Contas a Pagar")]
        public int? DocumentoPagarMestre_id { get; set; }
        [JsonIgnore,ForeignKey("DocumentoPagarMestre_id")]
        public virtual DocumentoPagarMestre DocumentoPagarMestre { get; set; }

        public bool Excluir(int id, ParamBase pb, DbControle banco = null)
        {
            int estab = pb.estab_id;
            
            if (banco  == null)
                banco = new DbControle();
            
            var obj = ObterPorId(id, banco,pb);
            if (obj == null)
                return false;
            else
            {
                banco.FGTS.Remove(obj);
                banco.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb, DbControle banco = null)
        {
            return Alterar(this, pb, banco);
        }
        public bool Alterar(FGTS obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            var objAux = ObterPorId(obj.ID,pb);
            if (objAux == null)
                return false;
            else
            {
                banco.Entry(obj).State = EntityState.Modified;
                banco.SaveChanges();

                return true;
            }
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }

        public bool Incluir(FGTS obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            if (validaExistencia(banco, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", banco, pb);

                banco.Set<FGTS>().Add(obj);
                banco.SaveChanges();

                return true;
            }
        }

        private bool validaExistencia(DbControle banco, FGTS obj,ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.FGTS.Where(x =>
                x.codigoReceita == obj.codigoReceita &&
                x.dataPagamento == obj.dataPagamento &&
                x.codigoBarras == obj.codigoBarras
                    && x.estabelecimento_id == estab).FirstOrDefault();
            return (objAux != null);
        }

        public FGTS ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }

        public FGTS ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.FGTS.Where(x => x.ID == id && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<FGTS> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.FGTS.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public SelectList CarregaFGTS(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.FGTS().ObterTodos(pb);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem()
                {
                    Value = item.ID.ToString(),
                    Text = String.Format("{0} - {1} - {2} - {3}",
                        item.cnpj,
                        item.codigoBarras,
                        item.codigoReceita,
                        item.dataPagamento,
                        item.digitoLacre,
                        item.identificadorFgts,
                        item.lacreConectividadeSocial,
                        item.nomeContribuinte,
                        item.tipoInscricao,
                        item.valorPagamento)
                });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }


        public FGTS ObterPorCPAG(int id, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            return banco.FGTS.Where(x => x.DocumentoPagarMestre_id == id).FirstOrDefault();

        }
    }
}
