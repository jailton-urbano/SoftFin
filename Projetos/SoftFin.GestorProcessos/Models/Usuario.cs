using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class Usuario
    {
        public Usuario()
        {
            UsuarioFuncaos = new List<UsuarioFuncao>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(250)]
        public string Login { get; set; }
        [MaxLength(250)]
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public List<UsuarioFuncao> UsuarioFuncaos { get; set; }
        [MaxLength(250)]
        public string Senha { get; set; }
        public int? IdEmpresa { get; set; }
        [JsonIgnore, ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }

        public bool ValidarUsuario(string usuario, string senha)
        {
            var db = new DBGPControle();
            var usu = db.Usuario.Where(p => p.Login.Equals(usuario) 
            && p.Senha.Equals(senha) 
            && p.Ativo == true ).ToList();

            if (usu.Count() > 1)
            {
                throw new Exception("Usuário duplicado na base de dados.");
            }
            if (usu.Count() == 1)
            {
                if (!usu.First().Ativo)
                    return false;
                else
                    return true;
            }
            return false;
        }



    }
}