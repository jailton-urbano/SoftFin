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
    public class LanctoExtratoVLW
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Data")]
        public string data { get; set; }
        [Display(Name = "Identificação Banco"), Required(ErrorMessage = "*"), MaxLength(20)]
        public string idLancto { get; set; }
        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(120)]
        public string descricao { get; set; }
        [Display(Name = "Tipo"), Required(ErrorMessage = "*"), MaxLength(1)]
        public string Tipo { get; set; }
        [Display(Name = "Valor"), Required(ErrorMessage = "*")]
        public decimal Valor { get; set; }
    }

    
    
    public class LanctoExtrato
    {

        [Key]
        public int id { get; set; }
        [Display(Name = "Data")]
        public DateTime data { get; set; }
        [Display(Name = "Identificação Banco"), Required(ErrorMessage = "*"), MaxLength(20)]
        public string idLancto { get; set; }
        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(120)]
        public string descricao { get; set; }
        [Display(Name = "Tipo"), Required(ErrorMessage = "*"), MaxLength(1)]
        public string Tipo { get; set; }
        [Display(Name = "Valor"), Required(ErrorMessage = "*")]
        public decimal Valor { get; set; }
        [Display(Name = "Banco"), Required(ErrorMessage = "*")]
        public int banco_id { get; set; }
        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco banco { get; set; }
        
        [Display(Name = "Conciliado")]
        public bool Conciliado { get; set; }
        [Display(Name = "Data Conciliado")]
        public DateTime? DataConciliado { get; set; }
        [Display(Name = "Usuario que executou a conciliação")]
        public string UsuConciliado { get; set; }

        public bool Excluir(int id, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.LanctoExtrato.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    //erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb);
        }

        public bool Alterar(LanctoExtrato obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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

        public bool Alterar(DbControle db, ParamBase pb)
        {


            var obj = ObterPorId(this.id,db);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(this, obj, "", db, pb);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        private bool validaExistencia(DbControle db, LanctoExtrato obj)
        {
            //DbControle banco = new DbControle();
            //var objAux = banco.LanctoExtrato.Where(x => x.descricao == obj.descricao).FirstOrDefault();
            //if (objAux != null)
                //return true;
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(LanctoExtrato obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<LanctoExtrato>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public LanctoExtrato ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public LanctoExtrato ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();
            var ret = db.LanctoExtrato.Where(x => x.id == id).FirstOrDefault();

            return ret;
        }




        public List<LanctoExtrato> ObterTodos()
        {

            DbControle db = new DbControle();
            return db.LanctoExtrato.ToList();
        }

        public List<LanctoExtrato> ObterTodos(int Banco, DateTime data)
        {

            DbControle db = new DbControle();
            return db.LanctoExtrato.Where(p => p.banco_id == Banco && p.data == data).ToList();
        }

        public List<LanctoExtrato> ObterTodosNaoConciliados(int Banco, DateTime data)
        {

            DbControle db = new DbControle();
            return db.LanctoExtrato.Where(p => p.banco_id == Banco && p.data == data && p.DataConciliado == null).ToList();
        }



        public List<LanctoExtrato> ObterEntreData(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {

            DbControle db = new DbControle();
            int estab = pb.estab_id;
            return db.LanctoExtrato.Where(p => p.banco.estabelecimento_id == estab && p.data >= DataInicial && p.data <= DataFinal && p.DataConciliado == null).ToList();
        }
    }
}