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
using System.Web.Script.Serialization;

namespace SoftFin.Web.Models
{
    public class SFCapa:BaseModels
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "Data Iniclal obrigatória")]
        public DateTime dataInicial { get; set; }
        [Required(ErrorMessage = "Data Final obrigatória")]
        public DateTime dataFinal { get; set; }
        [MaxLength(1), Required(ErrorMessage = "Situação obrigatória")]
        public string situacao { get; set; }
        //S - Simulãção / A - Aplicado / E- Excluido

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }
        [Display(Name = "Usuario Inclusão")]
        public int? usuarioinclusaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioinclusaoid")]
        public virtual Usuario UsuarioInclusao { get; set; }
        [Display(Name = "Usuario Alteração")]
        public int? usuarioalteracaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioalteracaoid")]
        public virtual Usuario UsuarioAlteracao { get; set; }

        public decimal SaldoInicial { get; set; }

        public string descricao { get; set; }
        [Display(Name = "Banco"),
            Required(ErrorMessage = "*")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }

        public bool incluirOrdemVenda { get; set; }

        

        public List<SFCapa> ObterTodos(int idestab)
        {
            DbControle db = new DbControle();
            return db.SFCapa.Where(x => x.estabelecimento_id == idestab).ToList();
        }

        public SFCapa ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public SFCapa ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.SFCapa.Where(x => x.id == id).FirstOrDefault();
        }

        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }

        public bool Incluir(SFCapa obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();


            new LogMudanca().Incluir(obj, "", "", db, pb);

            db.Set<SFCapa>().Add(obj);
            db.SaveChanges();
            return true;
        }
        
        public bool Alterar(SFCapa obj, ParamBase pb)
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
        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.SFCapa.Remove(obj);
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
    }
}