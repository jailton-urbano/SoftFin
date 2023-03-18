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
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class Atendimento
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Reclamante/Cliente"), Required(ErrorMessage = "*"), MaxLength(350)]
        public string usuario { get; set; }

        [Display(Name = "Código Atendimento"), Required(ErrorMessage = "*")]
        public int codigoAtendimento { get; set; }

        [Display(Name = "Data Abertura"), Required(ErrorMessage = "*")]
        public DateTime dataAbertura { get; set; }

        [Display(Name = "Data Fechamento") ]
        public DateTime? dataFechamento { get; set; }


        [Display(Name = "Titulo"), Required(ErrorMessage = "*"), MaxLength(200)]
        public string titulo { get; set; }


        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(2000)]
        public string descricao { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Cliente"),
        Required(ErrorMessage = "*")]
        public int pessoas_ID { get; set; }
        [JsonIgnore,ForeignKey("pessoas_ID")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Atendimento Categoria"), Required(ErrorMessage = "*")]
        public int atendimentocategoria_id { get; set; }
        [JsonIgnore,ForeignKey("atendimentocategoria_id")]
        public virtual AtendimentoCategoria AtendimentoCategoria { get; set; }

        [Display(Name = "Atendimento Status"), Required(ErrorMessage = "*")]
        public int atendimentostatus_id { get; set; }
        [JsonIgnore,ForeignKey("atendimentostatus_id")]
        public virtual AtendimentoStatus AtendimentoStatus { get; set; }

        [Display(Name = "Arquivo Anexo"), MaxLength(350)]
        public string NomeArquivoAnexo { get; set; }

        [Display(Name = "Arquivo Anexo Sistema"), MaxLength(100)]
        public string NomeArquivoAnexoSistema { get; set; }

        [Display(Name = "Contato"), Required(ErrorMessage = "*"), MaxLength(200)]
        public string Contato { get; set; }

        DbControle db = new DbControle();


        public bool Excluir( ParamBase pb)
        {
            return Excluir(this.id, pb);
        }
        public bool Excluir(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            var obj = ObterPorId(id, db,pb);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", db, pb);
                db.Atendimento.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar( ParamBase pb)
        {
            return Alterar(this,pb);
        }
        public bool Alterar(Atendimento obj, ParamBase pb)
        {

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
        public bool Save(Atendimento obj, ParamBase pb)
        {

            var objAux = new Atendimento();
            objAux = ObterPorId(obj.id,pb);
            objAux.atendimentostatus_id = obj.atendimentostatus_id;

            if (obj.atendimentostatus_id == 3)
                objAux.dataFechamento = DateTime.Now;
            var d = db.SaveChanges();
            return (d != 0);
            
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(Atendimento obj, ParamBase pb)
        {

            if (validaExistencia(db, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Atendimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle banco, Atendimento obj, ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.Atendimento.Where(x => 
                    x.descricao == obj.descricao 
                    && x.estabelecimento_id == estab).FirstOrDefault();
            return (objAux != null);
        }
        public Atendimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public Atendimento ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Atendimento.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<Atendimento> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            return db.Atendimento.Where(x => x.estabelecimento_id == estab).ToList();
        }
        public int ObterUltimoCodigo(ParamBase pb)
        {
            int estab = pb.estab_id;

            if (db.Atendimento.Where(x => x.estabelecimento_id == estab).Count() == 0)
                return 1;
            else
                return (db.Atendimento.Where(x => x.estabelecimento_id == estab).Max(p => p.codigoAtendimento) + 1);
        }

    }
}