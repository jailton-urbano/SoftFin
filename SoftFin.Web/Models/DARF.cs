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
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{

    public class DARF
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Código da Receita"), Required(ErrorMessage = "*")]
        public int codigoReceita { get; set; }

        [Display(Name = "CNPJ"), Required(ErrorMessage = "*"), MaxLength(18)]
        public string cnpj { get; set; }
        [Display(Name = "Período de Apuração"), Required(ErrorMessage = "*")]
        public DateTime periodoApuracao { get; set; }

        [Display(Name = "Número de Referência"), MaxLength(17)]
        public string numeroReferencia { get; set; }
        [Display(Name = "Valor Principal"), Required(ErrorMessage = "*")]
        public decimal valorPrincipal { get; set; }
        [Display(Name = "Valor da Multa"), Required(ErrorMessage = "*")]
        public decimal multa { get; set; }
        [Display(Name = "Valor Juros e Encargos"), Required(ErrorMessage = "*")]
        public decimal jurosEncargos { get; set; }
        [Display(Name = "Valor Total"), Required(ErrorMessage = "*")]
        public decimal valorTotal { get; set; }
        [Display(Name = "Data de Vencimento"), Required(ErrorMessage = "*")]
        public DateTime dataVencimento { get; set; }



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





        public bool Excluir(int id, ParamBase pb, DbControle banco =null)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();
            var obj = ObterPorId(id, banco,pb);
            if (obj == null)
                return false;
            else
            {
                banco.DARF.Remove(obj);
                banco.SaveChanges();
                return true;
            }
        }

        public bool Alterar(DARF obj, DbControle db, ParamBase pb)
        {
            if (db == null)
            db = new DbControle();

            var objAux = ObterPorId(obj.ID, db,pb);
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

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb,banco);
        }

        public bool Incluir(DARF obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            if (validaExistencia(banco, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", banco, pb);

                banco.Set<DARF>().Add(obj);
                banco.SaveChanges();

                return true;
            }
        }

        private bool validaExistencia(DbControle banco, DARF obj,  ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.DARF.Where(x =>
                x.codigoReceita == obj.codigoReceita &&
                x.dataVencimento == obj.dataVencimento
                    && x.estabelecimento_id == estab).FirstOrDefault();
            return (objAux != null);
        }

        public DARF ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public DARF ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.DARF.Where(x => x.ID == id && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public DARF ObterPorCPAG(int id, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.DARF.Where(x => x.DocumentoPagarMestre_id == id).FirstOrDefault();
        }

        public List<DARF> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.DARF.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public SelectList CarregaDARF( ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.DARF().ObterTodos(pb);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem()
                {
                    Value = item.ID.ToString(),
                    Text = String.Format("{0} - {1} - {2} - {3}",
                        item.cnpj,
                        item.codigoReceita,
                        item.dataVencimento,
                        item.jurosEncargos,
                        item.multa,
                        item.nomeContribuinte,
                        item.numeroReferencia,
                        item.periodoApuracao,
                        item.valorPrincipal,
                        item.valorTotal)
                });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }
    }
}
