using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalNFEReboque
    {
        [Key()]
        public int id { get; set; }

        public int notaFiscal_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscal_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }

        [MaxLength(9)]
        public string placa { get; set; }

        [MaxLength(2)]
        public string ufplaca { get; set; }

        [MaxLength(100)]
        public string RNTC { get; set; }

        public List<NotaFiscalNFEReboque> ObterTodos(int id)
        {
            return ObterTodos(id, null);
        }
        public List<NotaFiscalNFEReboque> ObterTodos(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalNFEReboque.Where(nf => nf.notaFiscal_id == id).ToList();
            return nfe;
        }

        public void Excluir(ParamBase pb,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFEReboque.Remove(this);
            banco.SaveChanges();
        }

        public void Incluir(DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            new LogMudanca().Incluir(this, "", "", db, pb);
            db.Set<NotaFiscalNFEReboque>().Add(this);
            db.SaveChanges();
            return;
        }

    }
}