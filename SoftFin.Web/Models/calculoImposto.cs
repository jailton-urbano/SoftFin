using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class calculoImposto: BaseModels
    {
        public calculoImposto()
        {
            CalculoImpostoTipoImpostos = new List<CalculoImpostoTipoImposto>();
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "Aliquota"), Required(ErrorMessage = "Informe Aliquota")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal aliquota { get; set; }
        [Display(Name = "Arrecador"), Required(ErrorMessage = "Informe o arrecadador")]
        public string arrecadador { get; set; }
        [Display(Name = "Retido"), Required(ErrorMessage = "Informe se é Retido")]
        public bool retido { get; set; } //S-Sim ou N-Não

        [Display(Name = "Operação"), Required(ErrorMessage = "Informe a Operação")]
        public int operacao_id { get; set; }

        [Display(Name = "Imposto"), Required(ErrorMessage = "Informe o Imposto*")]
        public int imposto_id { get; set; }

        [JsonIgnore,ForeignKey("operacao_id")]
        public virtual Operacao operacao { get; set; }

        [JsonIgnore,ForeignKey("imposto_id")]
        public virtual Imposto imposto { get; set; }



        [Display(Name = "Estabelecimento")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        public int? idAux { get; set; }

        public decimal? baseCalculo { get; set; }

        [MaxLength(40)]
        public string modalidade { get; set; }

        public decimal? margemValorAgregado { get; set; }

        public List<CalculoImpostoTipoImposto> CalculoImpostoTipoImpostos { get; set; }

        [MaxLength(4)]
        public string CST { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
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
                    db.calculoImposto.Remove(obj);
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

        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;


                if (db == null)
                    db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.calculoImposto.Remove(obj);
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

        public bool Alterar(calculoImposto obj,ParamBase pb)
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


        private bool validaExistencia(DbControle db, calculoImposto obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(calculoImposto obj,  ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<calculoImposto>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public calculoImposto ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public calculoImposto ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int empresa = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.calculoImposto.Where(x => x.id == id && x.estabelecimento_id == pb.estab_id).FirstOrDefault();
        }
        public List<calculoImposto> ObterPorOrdem(int id, DbControle db, ParamBase pb)
        {
            int empresa = pb.empresa_id;
            if (db == null)
                db = new DbControle();

            return db.calculoImposto.Where(x => x.id == id && x.operacao_id == id).ToList();
        }

        public List<calculoImposto> ObterTodos(ParamBase pb, DbControle db = null)
        {

            
            if (db == null)
                db = new DbControle();
            
            return db.calculoImposto.Where(x => x.estabelecimento_id == pb.estab_id).ToList();
        }

        public List<calculoImposto> ObterIssRetido(int operacao, ParamBase pb)
        {
            int empresa  = pb.empresa_id;
            DbControle db = new DbControle();
            return db.calculoImposto.Where(x => x.estabelecimento_id == pb.estab_id && x.operacao_id == operacao && x.imposto.codigo == "ISS").ToList();
        }
  


    }
}