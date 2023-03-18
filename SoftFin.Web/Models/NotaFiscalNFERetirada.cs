using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalNFERetirada
    {
        [NotMapped]
        public int indicadorCnpjCpf { get; set; }
        public int id { get; set; }

        public int notaFiscal_id { get; set; }


        [MaxLength(14)]
        public string cnpjCPF { get; set; }

        [MaxLength(200)]
        public string endereco { get; set; }
        [MaxLength(40)]
        public string numero { get; set; }

        [MaxLength(150)]
        public string complemento { get; set; }

        [MaxLength(10)]
        public string codMunicipio{ get; set; }



        public string bairro { get; set; }

        [NotMapped]
        public string cidade { get; set; }



        public NotaFiscalNFERetirada ObterPorId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            
            return db.NotaFiscalNFERetirada.Where(x => x.id == id).FirstOrDefault();
        }

        public void Excluir(ParamBase pb,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFERetirada.Remove(this);
            banco.SaveChanges();
        }
    }
}