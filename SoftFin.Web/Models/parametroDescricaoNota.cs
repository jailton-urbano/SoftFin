using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations.Schema;


namespace SoftFin.Web.Models
{
    public class parametroDescricaoNota: BaseModels
    {

        [Key]
        public int id { get; set; }



        [Display(Name = "Nome"), Required(ErrorMessage = "Informe o Nome")]
        public string nome { get; set; }
        [Display(Name = "Nome Model"), Required(ErrorMessage = "Informe a Model")]
        public string nomemodel { get; set; }
        [Display(Name = "Campo"), Required(ErrorMessage = "Informe o Campo")]
        public string campo { get; set; }
        [Display(Name = "HashTag"), Required(ErrorMessage = "Informe a HashTag")]
        public string hashtag { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase paramBase)
        {
            try
            {
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.parametroDescricaoNota.Remove(obj);
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

        public bool Alterar(parametroDescricaoNota obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", null, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, parametroDescricaoNota obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase)
        {
            return Incluir(this, paramBase);
        }
        public bool Incluir(parametroDescricaoNota obj, ParamBase paramBase)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, paramBase);

                db.Set<parametroDescricaoNota>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public parametroDescricaoNota ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public parametroDescricaoNota ObterPorId(int id, DbControle db)
        {
            
            if (db == null)
                db = new DbControle();

            return db.parametroDescricaoNota.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<parametroDescricaoNota> ObterTodos()
        {
            
            DbControle db = new DbControle();
            return db.parametroDescricaoNota.ToList();
        }
  



        //ID	Nome	            Model	    campo	            Hashtag
        //1 	Vencimento	        NotaFiscal	dataVencimentoNfse	#datadevencimento#
        //2 	Pedido de Compras	parametroDescricaoNota	pedido	            #pedidodecompras#
        //3 	Base de Cálculo	    NotaFiscal	basedeCalculo	    #basedecalculo#
        //4 	Aliquota ISS	    NotaFiscal	aliquotaISS     	#aliquotaiss#
        //5 	Valor do ISS	    NotaFiscal	valorISS	        #valoriss#
        //6 	aliquota PIS	    NotaFiscal	aliquotaPis	        #aliquotapis#
        //7 	Valor PIS	        NotaFiscal	pisRetido	        #pisretido#
        //8 	Aliquota COFINS	    NotaFiscal	aliquotaCofins	    #aliquotacofins#
        //9 	Valor COFINS	    NotaFiscal	cofinsRetida	    #cofinsretida#
        //10 	Aliquota CSLL	    NotaFiscal	aliquotaCsll	    #aliquotacsll#
        //11 	Valor CSLL	        NotaFiscal	csllRetida	        #csllretida#
        //12 	Aliquota IRRF	    NotaFiscal	aliquotaIrrf	    #aliquotairrf#
        //13 	Valor IRRF	        NotaFiscal	irrf	            #irrf#
        //14 	parametroDescricaoNota	        parametroDescricaoNota	parametroDescricaoNota	        #parametroDescricaoNota#
        //15 	Nome do Banco	    Banco	    nomeBanco	        #nomebanco#
        //16 	Banco Depósito	    Banco	    codigoBanco	        #codigobanco#
        //17 	Agência Depósito	Banco	    agencia	            #agencia#
        //18 	Conta Depósito	    Banco	    contaCorrente	    #contacorrente#
        //19 	Valor Líquido	    NotaFiscal	valorLiquido	    #valorliquido#

    }
}