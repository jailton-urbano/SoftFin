using SoftFin.InfrastructureHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Infrastructure.Models
{
    public class LogComercial
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(15)]
        public string CodigoSistema { get; set; }
        [MaxLength(50)]
        public string Estabelecimento { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataRecebida { get; set; }
        [MaxLength(300)]
        public string Nome { get; set; }
        [MaxLength(300)]
        public string NomeEmpresa { get; set; }
        [MaxLength(300)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Telefone { get; set; }

        [MaxLength(500)]
        public string Senha { get; set; }

        [MaxLength(50)]
        public string TemEmpresa { get; set; }

        [MaxLength(50)]
        public string IP { get; set; }

        [MaxLength(50)]
        public string IPLogado { get; set; }
        [MaxLength(1000)]
        public string Assunto { get;  set; }
        [MaxLength(8000)]
        public string Mensagem { get;  set; }
        [MaxLength(150)]
        public string Local { get;  set; }

        public void Incluir()
        {
            Incluir(this);
        }
        public void Incluir(LogComercial obj)
        {
            var db = new DBINControle();
            db.Set<LogComercial>().Add(obj);
            db.SaveChanges();
        }
    }
}