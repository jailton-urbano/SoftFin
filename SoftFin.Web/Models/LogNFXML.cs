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
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class LogNFXMLPrincipal : BaseModels
    {
        public LogNFXMLPrincipal()
        {
            logNFXMLAlertas = new List<LogNFXMLAlerta>();
            logNFXMLErros = new List<LogNFXMLErro>();
   
        }



        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "XML é obrigatório")]
        public string xml { get; set; }

        [Required(ErrorMessage = "Data da inclusão é obrigatório")]
        public DateTime dataInsert { get; set; }

        [Required(ErrorMessage = "Usuário criador obrigatório"),MaxLength(400)]
        public string usuarioInsert { get; set; }

        [Required(ErrorMessage = "Comando aceito na Prefeitura")]
        public bool aceito { get; set; }
        //true Aceito

        [Required(ErrorMessage = "Tipo de Envio")]
        public string tipo { get; set; }
        //Envio Cancelamento Consulta ConsultaEmitidas

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscal NotaFiscal { get; set; }

        public int? notaFiscal_id { get; set; }

        public virtual ICollection<LogNFXMLAlerta> logNFXMLAlertas { get; set; }
        public virtual ICollection<LogNFXMLErro> logNFXMLErros { get; set; }



        public bool Inclui(LogNFXMLPrincipal obj)
        {
            var banco = new DbControle();
            var contratoPesquisar = new LogNFXMLPrincipal();
            banco.LogNFXML.Add(obj);
            banco.SaveChanges();
            return true;

        }

        public void Altera(LogNFXMLPrincipal obj)
        {
            var banco = new DbControle();
            banco.Entry(obj).State = EntityState.Modified;
            banco.SaveChanges();
        }

        public IEnumerable<LogNFXMLPrincipal> ObterTodosPorNota(int idNota, DbControle db = null)
        {
            if (db == null)
                db = new DbControle(); 
            var lista = db.LogNFXML.Where(p => p.notaFiscal_id == idNota).ToList();
            return lista;
        }

        public LogNFXMLPrincipal ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public LogNFXMLPrincipal ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.LogNFXML.Where(x => x.id == id).FirstOrDefault();
        }
        public List<LogNFXMLPrincipal> ObterTodos(DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            var lista = banco.LogNFXML.ToList();
            return lista;
        }

        public void Excluir(DbControle banco, ParamBase pb)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.LogNFXML.Remove(this);
            banco.SaveChanges();
        }

        public string xmlRetorno { get; set; }
    }


    public class LogNFXMLAlerta : BaseModels
    {
        public int id { get; set; }

        [Required(ErrorMessage = "código é obrigatório"),MaxLength(50)]
        public string codigo { get; set; }

        [Required(ErrorMessage = "descrição é obrigatório"), MaxLength(500)]
        public string descricao { get; set; }


        [JsonIgnore,ForeignKey("logNFXML_id")]
        public virtual LogNFXMLPrincipal LogNFXML { get; set; }

        [Required(ErrorMessage = "Informe a que log pertence este alerta")]
        public int logNFXML_id { get; set; }

        public List<LogNFXMLAlerta> ObterTodosPorCapa(int idCapa, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            var lista = banco.LogNFXMLAlerta.Where(p => p.logNFXML_id == idCapa).ToList();
            return lista;
        }

        public LogNFXMLPrincipal ObterTodosId(int id, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.LogNFXML.Where(p => p.id == id).ToList().First();
        }
        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.LogNFXMLAlerta.Remove(this);
            banco.SaveChanges();
        }
        
    }


    public class LogNFXMLErro : BaseModels
    {
        public int id { get; set; }

        [Required(ErrorMessage = "código é obrigatório"), MaxLength(50)]
        public string codigo { get; set; }

        [Required(ErrorMessage = "descrição é obrigatório"), MaxLength(500)]
        public string descricao { get; set; }


        [JsonIgnore,ForeignKey("logNFXML_id")]
        public virtual LogNFXMLPrincipal LogNFXML { get; set; }

        [Required(ErrorMessage = "Informe a que log pertence este erro")]
        public int logNFXML_id { get; set; }
        //true Aceito

        public List<LogNFXMLErro> ObterTodosPorCapa(int idCapa, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            var lista = banco.LogNFXMLErro.Where(p => p.logNFXML_id == idCapa).ToList();
            return lista;
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "",banco, pb);

            var lista = banco.LogNFXMLErro.Remove(this);
            banco.SaveChanges();
        }
    }
}
