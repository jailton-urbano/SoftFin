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
    public class GPSVLW
    {
        [Display(Name = "Código de Pagamento"), MaxLength(4)]
        public int? codigoPagamento { get; set; }
        [Display(Name = "Competência"),  MaxLength(6)]
        public string competencia { get; set; }
        [Display(Name = "Identificador"),  MaxLength(14)]
        public string identificador { get; set; }

        [Display(Name = "Valor do Tributo Inicial")]
        public decimal? valorTributoInicial { get; set; }
        [Display(Name = "Valor Outras Entidades Inicial")]
        public decimal? valorOutrasEntidadesInicial { get; set; }
        [Display(Name = "Valor Atualização Monetária Inicial")]
        public decimal? valorAtualizacaoMonetariaInicial { get; set; }
        [Display(Name = "Valor Arrecadado Inicial")]
        public decimal? valorArrecadadoInicial { get; set; }
        [Display(Name = "Data da Arrecadação Inicial")]
        public DateTime? dataArrecadacaoInicial { get; set; }

        [Display(Name = "Valor do Tributo Final")]
        public decimal? valorTributoFinal { get; set; }
        [Display(Name = "Valor Outras Entidades Final")]
        public decimal? valorOutrasEntidadesFinal { get; set; }
        [Display(Name = "Valor Atualização Monetária Final")]
        public decimal? valorAtualizacaoMonetariaFinal { get; set; }
        [Display(Name = "Valor Arrecadado Final")]
        public decimal? valorArrecadadoFinal { get; set; }
        [Display(Name = "Data da Arrecadação Final")]
        public DateTime? dataArrecadacaoFinal { get; set; }
        
        [Display(Name = "Informações Complementares"),  MaxLength(50)]
        public string informacoesComplementares { get; set; }
        [Display(Name = "Nome do Contribuinte"),  MaxLength(30)]
        public string nomeContribuinte { get; set; }
    }

    public class GPS
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Código de Pagamento"), Required(ErrorMessage = "*")]
        public int codigoPagamento { get; set; }
        [Display(Name = "Competência"), Required(ErrorMessage = "*"), MaxLength(7)]
        public string competencia { get; set; }
        [Display(Name = "Identificador"), Required(ErrorMessage = "*"), MaxLength(14)]
        public string identificador { get;   set; }
        [Display(Name = "Valor do Tributo"), Required(ErrorMessage = "*")]
        public decimal valorTributo { get; set; }
        [Display(Name = "Valor Outras Entidades"), Required(ErrorMessage = "*")]
        public decimal valorOutrasEntidades { get; set; }
        [Display(Name = "Valor Atualização Monetária"), Required(ErrorMessage = "*")]
        public decimal valorAtualizacaoMonetaria { get; set; }
        [Display(Name = "Valor Arrecadado"), Required(ErrorMessage = "*")]
        public decimal valorArrecadado { get; set; }
        [Display(Name = "Data da Arrecadação"), Required(ErrorMessage = "*")]
        public DateTime dataArrecadacao { get; set; }
        [Display(Name = "Informações Complementares"), Required(ErrorMessage = "*"), MaxLength(50)]
        public string informacoesComplementares { get; set; }
        [Display(Name = "Nome do Contribuinte"), Required(ErrorMessage = "*"), MaxLength(30)]
        public string nomeContribuinte { get; set; }
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
            
            if (banco == null)
                banco = new DbControle();

            var obj = ObterPorId(id, banco,pb);
            if (obj == null)
                return false;
            else
            {
                banco.GPS.Remove(obj);
                banco.SaveChanges();
                return true;
            }
        }
        public bool Alterar(ParamBase pb,DbControle banco = null)
        {
            return Alterar(this, pb, banco);
        }
        public bool Alterar(GPS obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
            if (objAux == null)
                return false;
            else
            {
                banco.Entry(obj).State = EntityState.Modified;
                banco.SaveChanges();

                return true;
            }
        }

        public bool Incluir(ParamBase pb,DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }

        public bool Incluir(GPS obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            if (validaExistencia(banco, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", banco, pb);

                banco.Set<GPS>().Add(obj);
                banco.SaveChanges();

                return true;
            }
        }

        private bool validaExistencia(DbControle banco, GPS obj, ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.GPS.Where(x =>
                x.codigoPagamento == obj.codigoPagamento &&
                x.competencia == obj.competencia &&
                x.dataArrecadacao == obj.dataArrecadacao
                    && x.estabelecimento_id == estab).FirstOrDefault();
            return (objAux != null);
        }

        public GPS ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }

        public GPS ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.GPS.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<GPS> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.GPS.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public SelectList CarregaGPS(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.GPS().ObterTodos(pb);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", 
                    item.codigoPagamento, 
                    item.competencia, 
                    item.dataArrecadacao, 
                    item.identificador, 
                    item.informacoesComplementares, 
                    item.nomeContribuinte, 
                    item.valorArrecadado, 
                    item.valorAtualizacaoMonetaria, 
                    item.valorOutrasEntidades, 
                    item.valorTributo) });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }


        public GPS ObterPorCPAG(int id, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.GPS.Where(x => x.DocumentoPagarMestre_id == id).FirstOrDefault();
        }
    }
}
