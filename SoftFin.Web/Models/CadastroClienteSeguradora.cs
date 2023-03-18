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
    public class CadastroClienteSeguradora
    {
        public CadastroClienteSeguradora()
        {
            dataCadastro = DateTime.Now;
        }
        [Key]
        public int id { get; set; }

        [Display(Name = "Data Cadastro")]
        public DateTime dataCadastro { get; set; }

        [Display(Name = "Nome Pessoa"),
        MaxLength(300),
        Required(ErrorMessage = "Informe o Nome da Empresa")]
        public string nome { get; set; }


        [Display(Name = "TIpo Seguro"),
        MaxLength(100),
        Required(ErrorMessage = "Informe o Tipo de Seguro")]
        public string tipoSeguro { get; set; }

        [Display(Name = "Email"),
        MaxLength(300),
        Required(ErrorMessage = "Informe o Email")]
        public string email { get; set; }

        [Display(Name = "Telefone Fixo")]
        public string telefone { get; set; }


        [Display(Name = "Celular")]
        public string celular { get; set; }



        DbControle _banco = new DbControle();

        public bool Incluir(DbControle banco = null)
        {
            return Incluir(this);
        }
        public bool Incluir(CadastroClienteSeguradora obj, DbControle banco = null)
        {
            if (banco == null)
            {
                banco = new DbControle();
                banco.CadastroClienteSeguradora.Add(obj);
                banco.SaveChanges();
            }
            else
            {
                _banco.CadastroClienteSeguradora.Add(obj);
                _banco.SaveChanges();
            }
            return true;
        }


        public bool Alterar(DbControle banco = null)
        {
            return Alterar(this,banco);
        }
        public bool Alterar(CadastroClienteSeguradora obj, DbControle banco = null)
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


    }
}