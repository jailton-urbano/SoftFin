using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class Operacao: BaseModels
    {
        public Operacao() 
        {
            CalculoImposto = new List<calculoImposto>();
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "Informe a empresa")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }

        [Display(Name = "Codigo"), Required(ErrorMessage = "Informe o código")]
        public string codigo { get; set; }
        [Display(Name = "Descrição"), Required(ErrorMessage = "Informe a descrição")]
        public string descricao { get; set; }
       
        [Display(Name = "Descrição Nota")]
        public string descricaoNota { get; set; }

        [Display(Name = "Tipo RPS")]
        public int? tipoRPS_id { get; set; }

        [JsonIgnore,ForeignKey("tipoRPS_id")]
        public virtual TipoRPS tipoRPS { get; set; }

        [Display(Name = "Situação Tributária Nota")]
        public int? situacaoTributariaNota_id { get; set; }

        [JsonIgnore,ForeignKey("situacaoTributariaNota_id")]
        public virtual SituacaoTributariaNota situacaoTributariaNota { get; set; }


        [Display(Name = "Entrada ou Saida (E ou S)"), MaxLength(1)]
        public string entradaSaida { get; set; }


        [Display(Name = "Estabelecimento")]
        public int? estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        public int? idAux { get; set; }

        [MaxLength(5)]
        public string CFOP { get; set; }

        [MaxLength(3)]
        public string CSOSN { get; set; }
        
        [MaxLength(1)]
        public string produtoServico { get; set; }


        public List<calculoImposto> CalculoImposto { get; set; }


        public decimal percentualCargaTributaria { get; set; }

        [MaxLength(4)]
        public string fonteCargaTributaria { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Operacao.Remove(obj);
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

        public bool Excluir(int id, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            var obj = banco.Operacao.Where(x => x.id == id).First();
            if (obj == null)
                return false;
            else
            {
                banco.Set<Operacao>().Remove(obj);
                banco.SaveChanges();
                return true;
            }
        }

        public bool Alterar(Operacao obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

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


        private bool validaExistencia(DbControle db, Operacao obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(Operacao obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Operacao>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public void Incluir(Operacao operacao, List<calculoImposto> calculo, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            
            int estab = pb.estab_id;

            var objAux = banco.Operacao.Where(x =>
                       x.estabelecimento_id == estab
                    && x.codigo == operacao.codigo).FirstOrDefault();


            operacao.estabelecimento_id = estab;

            if (objAux != null)
            {
                throw new Exception("Operação já cadastrada");
            }

            banco.Operacao.Add(operacao);
            banco.SaveChanges();
            SalvaCalculoImposto(operacao, calculo, banco, false,pb);
        }

        public void Alterar(Operacao operacao, List<calculoImposto> calculo, DbControle banco, ParamBase pb)
        {
            if (banco == null)
                banco = new DbControle();
            int estab = pb.estab_id;
            operacao.estabelecimento_id = estab;
            banco.Entry(operacao).State = EntityState.Modified;
            banco.SaveChanges();
            if (calculo != null)
            {
                SalvaCalculoImposto(operacao, calculo, banco, true,pb);
            }
        }


        private void SalvaCalculoImposto(Operacao obj, List<calculoImposto> calculo, DbControle banco, bool atualiza, ParamBase pb)
        {
            try
            {
                if (atualiza)
                {
                    string Erro = "";

                    var objitem = new calculoImposto().ObterTodos(pb).Where(x=> x.operacao_id == obj.id).ToList();

                    foreach (var item in objitem)
                    {
                        item.Excluir(item.id, ref Erro, pb, banco);
                    }
                }

                //Cria Cálculo do Imposto
                foreach (var item in calculo)
                {
                    var objitem = new calculoImposto();
                    objitem.estabelecimento_id  = pb.estab_id;
                    objitem.aliquota = item.aliquota;
                    objitem.arrecadador = item.arrecadador;
                    objitem.retido = item.retido;
                    objitem.operacao_id = obj.id;
                    objitem.imposto_id = item.imposto_id;
                    
                    banco.calculoImposto.Add(objitem);
                }

                banco.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Operacao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public Operacao ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idestab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Operacao.Include(x => x.CalculoImposto).Where(x => x.id == id && x.estabelecimento_id == idestab).FirstOrDefault();
        }
        public List<Operacao> ObterTodos( ParamBase pb)
        {
            int idestab = pb.estab_id;
            DbControle db = new DbControle();
            var objs = db.Operacao.Where(x => x.estabelecimento_id == idestab).ToList();
            return objs;

        }




        public SelectList CarregaOperacao(ParamBase pb)
        {
            var con1 = new SoftFin.Web.Models.Operacao().ObterTodos(pb).OrderBy(p => p.codigo);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1}", item.codigo, item.descricao)});
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }
  
    }
}