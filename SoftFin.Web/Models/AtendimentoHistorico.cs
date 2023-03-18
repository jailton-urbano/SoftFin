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
    public class AtendimentoHistorico
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Usuário"), Required(ErrorMessage = "*"), MaxLength(350)]
        public string usuario { get; set; }

        [Display(Name = "Data"), Required(ErrorMessage = "*")]
        public DateTime data { get; set; }

        [Display(Name = "Arquivo Anexo"), MaxLength(350)]
        public string NomeArquivoAnexo { get; set; }

        [Display(Name = "Arquivo Anexo"), MaxLength(100)]
        public string NomeArquivoAnexoSistema { get; set; }
        
        [Display(Name = "Descrição"), Required(ErrorMessage = "*"), MaxLength(2000)]
        public string descricao { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Atendimento"), Required(ErrorMessage = "*")]
        public int atendimento_id { get; set; }
        [JsonIgnore,ForeignKey("atendimento_id")]
        public virtual Atendimento Atendimento { get; set; }

        [Display(Name = "Tipo Historico"), Required(ErrorMessage = "*")]
        public int atendimentotipohistorico_id { get; set; }

        [JsonIgnore,ForeignKey("atendimentotipohistorico_id")]
        public virtual AtendimentoTipoHistorico AtendimentoTipoHistorico { get; set; }

        public bool Excluir(int id, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var obj = ObterPorId(id, db,pb);
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);
                db.AtendimentoHistorico.Remove(obj);
                db.SaveChanges();
                return true;
            }
        }
        public bool Alterar(AtendimentoHistorico obj, ParamBase pb)
        {
            DbControle db = new DbControle();

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
        public bool Incluir( ParamBase pb)
        {
            return Incluir(this,pb);
        }
        public bool Incluir(AtendimentoHistorico obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj,pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<AtendimentoHistorico>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle banco, AtendimentoHistorico obj, ParamBase pb)
        {
            int estab = pb.estab_id;
            var objAux = banco.AtendimentoHistorico.Where(x =>
                    x.descricao == obj.descricao
                    && x.estabelecimento_id == estab).FirstOrDefault();
            return (objAux != null);
        }
        public AtendimentoHistorico ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public AtendimentoHistorico ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.AtendimentoHistorico.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<AtendimentoHistorico> ObterTodos( ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle banco = new DbControle();
            return banco.AtendimentoHistorico.Where(x => x.estabelecimento_id == estab).ToList();
        }



        public List<AtendimentoHistorico> ObterPorAtendimento(int ID, ParamBase pb)
        {
            var banco = new DbControle();
            int estab = pb.estab_id;
            
            return banco.AtendimentoHistorico.Where(x => x.atendimento_id == ID && x.estabelecimento_id == estab).ToList();
        }
    }
}