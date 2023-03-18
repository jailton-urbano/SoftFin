using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace SoftFin.Web.Models
{
    public class NotadeDebito:BaseModels
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe o estabelecimento.")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "numero"),
        Required(ErrorMessage = "Informe o número")]
        public int numero { get; set; }

        [DisplayName("Data Emissão"),
        Required(ErrorMessage = "Informe a Data de Emissão")]
        [DataType(DataType.Date)]
        public DateTime DataEmissao { get; set; }

        [DisplayName("Data Vencimento"),
        Required(ErrorMessage = "Data de Vencimento")]
        [DataType(DataType.Date)]
        public DateTime DataVencimento { get; set; }
            
        [DisplayName("Data Recebimento")]
        [DataType(DataType.Date)]
        public DateTime? DataRecebimento { get; set; }

        [DisplayName("Data Cancelamento")]
        [DataType(DataType.Date)]
        public DateTime? DataCancelamento { get; set; }

        [Display(Name = "Cliente"), Required(ErrorMessage = "Informe o cliente")]
        public int cliente_id { get; set; }
        [JsonIgnore,ForeignKey("cliente_id")]
        public virtual Pessoa Cliente { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "Informe os comentário"), MaxLength(1500)]
        public string descricao { get; set; }

        [Display(Name = "Comentários"),
        MaxLength(1500)]
        public string comentario { get; set; }

        [Display(Name = "Valor"),
        Required(ErrorMessage = "Informe o valor")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valor { get; set; }

        [Display(Name = "Situação Nota de Débito"), Required(ErrorMessage = "*")]
        public int situacaoNotaDebito_id { get; set; }
        [JsonIgnore,ForeignKey("situacaoNotaDebito_id")]
        public virtual SituacaoNotaDebito SituacaoNotaDebito { get; set; }


        [Display(Name = "Banco Movimento")]
        public int? bancoMovimento_id { get; set; }
        [JsonIgnore,ForeignKey("bancoMovimento_id")]
        public virtual BancoMovimento BancoMovimento { get; set; }


        [NotMapped]
        public int banco_id { get; set; }

        [NotMapped]
        public int planoconta_id { get; set; }
        


        public bool Excluir(int id, ref string erro, ParamBase pb,DbControle db = null )
        {
            try
            {
                int estab = pb.estab_id;
                if (db == null)
                    db = new DbControle();
                
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db,pb);
                    db.NotadeDebito.Remove(obj);
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

        public bool Alterar(NotadeDebito obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db,pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, NotadeDebito obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(NotadeDebito obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb); ;

                db.Set<NotadeDebito>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public NotadeDebito ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public NotadeDebito ObterPorId(int id, DbControle NotadeDebito, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (NotadeDebito == null)
                NotadeDebito = new DbControle();

            return NotadeDebito.NotadeDebito.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<NotadeDebito> ObterTodos(ParamBase pb)
        {
            DbControle NotadeDebito = new DbControle();
            return NotadeDebito.NotadeDebito.Where(x => x.estabelecimento_id == pb.estab_id).ToList();
        }

        public int ObterUltima(ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            if (db.NotadeDebito.Where(x => x.estabelecimento_id == estab).Count() > 0)
            {
                return db.NotadeDebito.Where(x => x.estabelecimento_id == estab).Max(m => m.numero);
            }
            else
            {
                return 0;
            }
        }

        public DateTime? dataInclusao { get; set; }

        [MaxLength(250)]
        public string usuarioRecebimento { get; set; }

        public int usuarioRecebimento_id { get; set; }

        public NotadeDebito ObterPorBM(int idBM, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.NotadeDebito.Where(x => x.estabelecimento_id == estab && x.bancoMovimento_id == idBM ).FirstOrDefault();

        }
    }
}