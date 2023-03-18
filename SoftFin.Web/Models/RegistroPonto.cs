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
    public class RegistroPonto
    {
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*")]
        public DateTime data { get; set; }

        [Display(Name = "Ponto1")]
        public DateTime ponto1 { get; set; }
        [Display(Name = "Ponto2")]
        public DateTime ponto2 { get; set; }
        [Display(Name = "Ponto3")]
        public DateTime ponto3 { get; set; }
        [Display(Name = "Ponto4")]
        public DateTime ponto4 { get; set; }
        [Display(Name = "Ponto5")]
        public DateTime ponto5 { get; set; }
        [Display(Name = "Ponto6")]
        public DateTime ponto6 { get; set; }
        [Display(Name = "Ponto7")]
        public DateTime ponto7 { get; set; }
        [Display(Name = "Ponto8")]
        public DateTime ponto8 { get; set; }

        [Display(Name = "Comentários"), MaxLength(400)]
        public string comentarios { get; set; }

        [Display(Name = "Aprovador")]
        public int? aprovador_id { get; set; }

        [JsonIgnore,ForeignKey("aprovador_id")]
        public virtual Usuario aprovador { get; set; }

        [Display(Name = "Data Aprovação")]
        public DateTime? dataAprovado { get; set; }

        [Display(Name = "Descrição Aprovação"), MaxLength(250)]
        public string DescricaoAprovacao { get; set; }

        [Display(Name = "Situacao Aprovação"), MaxLength(10)]
        public string SituacaoAprovacao { get; set; }

        [Display(Name = "apontador")]
        public int? apontador_id { get; set; }

        [JsonIgnore,ForeignKey("apontador_id")]
        public virtual Usuario apontador { get; set; }

        public bool Excluir(ref string erro, ParamBase pb)
        {
            return Excluir(this.id, ref erro,pb);
        }

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
                    db.RegistroPonto.Remove(obj);
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

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb, db);
        }

        public bool Alterar(RegistroPonto obj, ParamBase pb, DbControle db = null)
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


        private bool validaExistencia(DbControle db, RegistroPonto obj, ParamBase pb)
        {
            if (obj.ObterTodos(pb).Where(x=> x.apontador_id == obj.apontador_id && x.data == obj.data).Count() > 0)
            {
                return (true);
            }
            else
            {
                return (false);
            }

        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(RegistroPonto obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<RegistroPonto>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public RegistroPonto ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public RegistroPonto ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.RegistroPonto.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }

        public List<RegistroPonto> ObterTodos( ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.RegistroPonto.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<RegistroPonto> ObterTodosUsuario(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.RegistroPonto.Where(x => x.estabelecimento_id == estab
                                                && x.apontador_id == id).ToList();
        }

        public List<RegistroPonto> ObterTodosDataUsuario(DateTime dataInicial, DateTime dataFinal, int usuario, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.RegistroPonto.Where(x => x.estabelecimento_id == estab
                                                        && x.data >= dataInicial
                                                        && x.data <= dataFinal
                                                        && x.apontador_id == usuario).ToList();
        }


        public List<RegistroPonto> ObterTodosEntreData(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.RegistroPonto.Where(x => x.estabelecimento_id == estab
                                                        && x.data >= DataInicial
                                                        && x.data <= DataFinal).ToList();
        }
    }
}