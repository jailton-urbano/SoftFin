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
    public class Banco: BaseModels
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Código Identificação"), Required(ErrorMessage = "Informe o Código Identificação"), MaxLength(20)]
        public string codigo { get; set; }
        [Display(Name = "Nome Banco"), Required(ErrorMessage = "Informe o Nome Banco"), MaxLength(50)]
        public string nomeBanco { get; set; }
        [Display(Name = "Código Banco"), Required(ErrorMessage = "Informe o Código Banco"), MaxLength(3)]
        public string codigoBanco { get; set; }
        [Display(Name = "Agencia"), Required(ErrorMessage = "Informe a Agência"), MaxLength(10)]
        public string agencia { get; set; }
        [Display(Name = "Digito"), MaxLength(1)]
        public string agenciaDigito { get; set; }
        
        [Display(Name = "Conta Corrente"), Required(ErrorMessage = "Informe a Conta Corrente"), MaxLength(20)]
        public string contaCorrente { get; set; }

        [Display(Name = "Digito"),  MaxLength(1)]
        public string contaCorrenteDigito { get; set; }
        
        [Display(Name = "Nosso Numero"),  MaxLength(20)]
        public string nossoNumero { get; set; }

        [Display(Name = "Numero Documento")]
        public int numeroDocumento { get; set; }

        [Display(Name = "Carteira")]
        public int carteira { get; set; }

        [Display(Name = "Texto Boleto 01"), MaxLength(40)]
        public string TextoBoleto01 { get; set; }

        [Display(Name = "Texto Boleto 02"), MaxLength(40)]
        public string TextoBoleto02 { get; set; }

        [Display(Name = "Texto Boleto 03"), MaxLength(40)]
        public string TextoBoleto03 { get; set; }

        [Display(Name = "Observação"),  MaxLength(30)]
        public string observacao { get; set; }

        [Display(Name = "Principal")]
        public bool principal { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o Extabelecimeno")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }
        
        [Display(Name = "Emite Boleto")]
        public bool EmiteBoleto { get; set; }

        [Display(Name = "Tipo Conta")]
        public int TipoConta { get; set; }
        //0 - Conta Corrente , 1 - Cartão, 2 - Aplicação, 3 - Caixinha



        [Display(Name = "Valor Limite")]
        public decimal? ValorLimite { get; set; }

        public string NumeroConvenio { get; set; }

        public int NumeroArquivoRemessa { get; set; }


        public int? AplicacaoAutomatica { get; set; }
        
        public int DiaFechamentoFatura { get; set; }

        [MaxLength(20)]
        public string ReferenciaIntegracao { get; set; }
                
        public decimal? JurosDia { get; set; }

        public decimal? Multa { get; set; }

        public int SequencialLoteCNAB { get; set; }

        [MaxLength(30)]
        public string CodigoTransmissao { get; set; }


        public bool EmissaoBoletoAposNF { get; set; }
        public bool EmissaoBoletoComNFPDF { get; set; }

        public int? ContaContabilMovimentacaoDC_id { get; set; }
        [JsonIgnore, ForeignKey("ContaContabilMovimentacaoDC_id")]
        public virtual ContaContabil ContaContabilMovimentacaoDC { get; set; }

        //Altera Banco
        public void DefineBancoPrincipal(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            IEnumerable<Banco> lista = banco.Bancos.Where(p => p.estabelecimento_id == estab).ToList();

            foreach (var item in lista)
            {
                item.principal = (item.id == id);
            }
            banco.SaveChanges();
        }
        public bool Excluir(int id, ParamBase pb)
        {
            int estab =pb.estab_id;
            DbControle db = new DbControle();
            var obj = ObterPorId(id, db,pb);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.Bancos.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(Banco obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
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
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(Banco obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Banco>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle banco, Banco obj, ParamBase pb)
        {
            int estab =pb.estab_id;
            var objAux = banco.Bancos.Where(x => 
                x.contaCorrente == obj.contaCorrente && 
                x.codigoBanco == obj.codigoBanco &&
                x.agencia == obj.agencia 
                    && x.estabelecimento_id == estab).FirstOrDefault();
            return (objAux != null);
        }
        public Banco ObterPorId(int id,ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Banco ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab =pb.estab_id;
            if (banco ==null)
                banco = new DbControle();

            return banco.Bancos.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }


        public Banco ObterPorReferenciaIntegracao(string referencia, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Bancos.Where(x => x.ReferenciaIntegracao == referencia && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<Banco> ObterTodos(ParamBase pb)
        {
            int estab =pb.estab_id;
            DbControle banco = new DbControle();
            return banco.Bancos.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<Banco> ObterTodosHoldind(ParamBase pb)
        {
            DbControle banco = new DbControle();
            return banco.Bancos.Where(x => x.Estabelecimento.Empresa.Holding_id == pb.holding_id).ToList();
        }
        public List<Banco> ObterTodosEstab( int estab)
        {
            DbControle banco = new DbControle();
            return banco.Bancos.Where(x => x.estabelecimento_id == estab).ToList();
        }
        public SelectList CarregaBanco(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.Banco().ObterTodos(pb).OrderBy(p => p.principal);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }
        public SelectList CarregaBancoGeral(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.Banco().ObterTodos(pb).OrderBy(p => p.principal);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }

        public SelectList CarregaBancoGeralBoleto(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.Banco().ObterTodos(pb).Where(p=>p.EmiteBoleto == true).OrderBy(p => p.principal);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }


        public SelectList ObterBancoAplicacao(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.Banco().ObterTodos(pb).Where(p => p.TipoConta == 2).OrderBy(p => p.principal);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }
        public Banco ObterPrincipal(ParamBase pb)
        {
            return new SoftFin.Web.Models.Banco().ObterTodos(pb).Where(p => p.principal == true).FirstOrDefault();
        }
    }
}