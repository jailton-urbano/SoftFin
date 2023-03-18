using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class SaldoContabil : BaseModels
    {
        public SaldoContabil()
        {
            SaldoContabilDetalhe = new List<SaldoContabilDetalhe>();
        }
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a data do fechamento")]
        public DateTime DataBase { get; set; }

        public string Usuario { get; set; }

        public DateTime DataFechamento { get; set; }

        [MaxLength(50)]
        public String HashCode{ get; set; }

        public int Situacao { get; set; }
        // 0-Em Aberto 1-Cancelado 9-Fechado

        [Display(Name = "Estabelecimento"),
        Required(ErrorMessage = "Informe o Estabelecimento do Contrato")]

        public int Estabelecimento_id { get; set; }
        [JsonIgnore, ForeignKey("Estabelecimento_id")]

        public virtual Estabelecimento Estabelecimento { get; set; }

        public List<SaldoContabilDetalhe> SaldoContabilDetalhe { get; set; }





        private bool ValidaExistencia(DbControle db, SaldoContabil obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase paramBase, DbControle db = null)
        {
            return Incluir(this, paramBase, db);
        }
        public bool Incluir(SaldoContabil obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (ValidaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db,paramBase);

                db.Set<SaldoContabil>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public bool Alterar(ParamBase paramBase, DbControle db)
        {
            return Alterar(this, paramBase, db);
        }

        public List<SaldoContabil> ObterPorDatabase(DateTime data, ParamBase paramBase, DbControle db = null)
        {
            int idestab = paramBase.estab_id;
            if (db == null)
                db = new DbControle();

            return db.SaldoContabil.Where(x => x.DataBase == data
                                    && x.Estabelecimento_id == idestab).ToList();
        }

        public bool Alterar(SaldoContabil obj, ParamBase paramBase, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.Id, paramBase);
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

        public bool Excluir(ref string erro, ParamBase paramBase, DbControle db = null)
        {
            return Excluir(this.Id, ref erro, paramBase, db);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase, DbControle db = null)
        {
            try
            {
                if (db == null)
                    db = new DbControle();
                
                var obj = ObterPorId(id, db, paramBase);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.SaldoContabil.Remove(obj);
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


        public SaldoContabil ObterPorId(int id, ParamBase paramBase)
        {
            return ObterPorId(id, null, paramBase);
        }
        public SaldoContabil ObterPorId(int id, DbControle banco, ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.SaldoContabil.Where(x => x.Id == id && x.Estabelecimento_id == idestab).FirstOrDefault();
        }

        public List<SaldoContabil> ObterTodos(ParamBase paramBase)
        {
            int idestab = paramBase.estab_id;
            var banco = new DbControle();

            return banco.SaldoContabil.Where(x => x.Estabelecimento_id == idestab).ToList();
        }


    }
}