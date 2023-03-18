using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalNFETransportadora
    {
        public int id { get; set; }

        [MaxLength(500)]
        public string nomeRazao { get; set; }

        [MaxLength(14)]
        public string cnpjCPF { get; set; }

        [MaxLength(200)]
        public string cidade { get; set; }

        [MaxLength(2)]
        public string uf { get; set; }

        [MaxLength(9)]
        public string placa { get; set; }

        [MaxLength(2)]
        public string ufplaca { get; set; }

        [MaxLength(100)]
        public string RNTC { get; set; }


        [MaxLength(1)]
        public string modalidadeFrete { get; set; }




        public decimal baseCalculo { get; set; }

        public decimal aliquota { get; set; }

        public decimal valorServico { get; set; }

        public decimal ICMSRetido { get; set; }

        [MaxLength(4)]
        public string  CFOP { get; set; }
        
        [MaxLength(14)]
        public string IE { get; set; }

        [MaxLength(60)]
        public string EnderecoCompleto { get; set; }

        [MaxLength(7)]
        public string codigoMunicipioOcorrencia { get; set; }

        [NotMapped]
        public int indicadorCnpjCpf { get; set; }


        public NotaFiscalNFETransportadora ObterPorId(int id, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            
            return db.NotaFiscalNFETransportadora.Where(x => x.id == id).FirstOrDefault();
        }

        public void Excluir(ParamBase pb,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalNFETransportadora.Remove(this);
            banco.SaveChanges();
        }
    }
}