using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalNFEEntrega
    {
        public int id { get; set; }


        [MaxLength(14)]
        public string cnpjCPF { get; set; }

        [MaxLength(200)]
        public string endereco { get; set; }
        [MaxLength(40)]
        public string numero { get; set; }

        [MaxLength(150)]
        public string complemento { get; set; }

        [MaxLength(10)]
        public string codMunicipio { get; set; }

        [MaxLength(60)]
        public string bairro { get; set; }


        [NotMapped]
        public string cidade { get; set; }





        public NotaFiscalNFEEntrega ObterPorId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            
            return db.NotaFiscalNFEEntrega.Where(x => x.id == id).FirstOrDefault();
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco,pb);

            var lista = banco.NotaFiscalNFEEntrega.Remove(this);
            banco.SaveChanges();
        }

        [NotMapped]
        public int indicadorCnpjCpf { get; set; }
    }
}