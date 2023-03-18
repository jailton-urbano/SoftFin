using System;
using System.Collections.Generic;
using System.Linq;




namespace SoftFin.Infrastructure.DTO
{
    public class DTOLogComercial
    {
        public int Id { get; set; }
        public string CodigoSistema { get; set; }
        public string Estabelecimento { get; set; }
        public DateTime Data { get; set; }

        public string Nome { get; set; }
        public string NomeEmpresa { get; set; }
        public string Email { get; set; }

        public string Telefone { get; set; }

        public string Senha { get; set; }

        public string TemEmpresa { get; set; }

        public string IP { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public string Local { get; set; }
    }
}