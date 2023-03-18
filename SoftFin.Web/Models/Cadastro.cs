using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class CadastroCliente
    {
        public CadastroCliente()
        {
            tipoassinatura = "Ver cadastro de cartão";
        }
        [Key]
        public int id { get; set; }

        [Display(Name = "Data Cadastro")]
        public DateTime datacadastro { get; set; }

        [Display(Name = "Tipo Assinatura"), MaxLength(35), Required(ErrorMessage = "Informe  o Tipo de Plano")]
        public String tipoassinatura { get; set; }

        [Display(Name = "Nome Usuário ADM"), MaxLength(70), Required(ErrorMessage = "Informe Usuário Responsável")]
        public String usuarioADM { get; set; }

        [Display(Name = "Email Usuário ADM"), MaxLength(70), Required(ErrorMessage = "Informe Email de Acesso")]
        public String emailADM { get; set; }

        [Display(Name = "Senha ADM"), 
        Required(ErrorMessage = "Informe a Senha*")]
        public String senhaADM { get; set; }

        [Display(Name = "Usuário Confirmado")]
        public bool usuarioConfimado { get; set; }

        [Display(Name = "Nome Pessoa"),
        MaxLength(70),
        Required(ErrorMessage = "Informe o Nome da Empresa")]
        public string nome { get; set; }

        [Display(Name = "Razão Social"),
        MaxLength(50),
        Required(ErrorMessage = "Informe a Razão Social")]
        public string razao { get; set; }

        [Display(Name = "CNPJ/CPF"),
        MaxLength(50),
        Required(ErrorMessage = "Informe o  CNPJ")]
        public string cnpj { get; set; }

 
     
        [Display(Name = "Telefone Fixo"), StringLength(20)]
        public string TelefoneFixo { get; set; }

        [Display(Name = "Celular"), StringLength(20)]
        public string Celular { get; set; }

        [Display(Name = "token"), StringLength(50)]
        public string Token { get; set; }

        DbControle _banco = new DbControle();

        public bool Incluir(DbControle banco = null)
        {
            return Incluir(this);
        }
        public bool Incluir(CadastroCliente obj, DbControle banco = null)
        {
            if (banco == null)
            {
                banco = new DbControle();
                banco.Cadastro.Add(obj);
                banco.SaveChanges();
            }
            else
            {
                _banco.Cadastro.Add(obj);
                _banco.SaveChanges();
            }
            return true;
        }


        public bool Alterar(DbControle banco = null)
        {
            return Alterar(this,banco);
        }
        public bool Alterar(CadastroCliente obj, DbControle banco = null)
        {
            if (banco != null)
            {
                banco.Entry(obj).State = EntityState.Modified;
                banco.SaveChanges();
            }
            else
            {
                _banco.Entry(obj).State = EntityState.Modified;
                _banco.SaveChanges();
            }
            return true;
        }

        public CadastroCliente ObterToken(string token, DbControle banco = null)
        {
            if (banco != null)
                return banco.Cadastro.Where(p => p.Token == token).FirstOrDefault();
            else
                return _banco.Cadastro.Where(p => p.Token == token).FirstOrDefault();
        }

    }
}