using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class NotaFiscalPessoa: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Razão "), Required(ErrorMessage = "Preencha a Razão do "), StringLength(150)]
        public string razao { get; set; }

        [Display(Name = "Indicador CNPJ CPF"), Required(ErrorMessage = "Preencha o Indicador CPF ou CNPJ do ")]
        public int indicadorCnpjCpf { get; set; }

        [Display(Name = "CNPJ CPF"), Required(ErrorMessage = "Preencha o CPF ou CNPJ do "), StringLength(18)]
        public string cnpjCpf { get; set; }

        [Display(Name = "Inscrição Municipal"), StringLength(8)]
        public string inscricaoMunicipal { get; set; }

        [Display(Name = "Inscrição Estadual")]
        public string inscricaoEstadual { get; set; }

        [Display(Name = "Tipo Endereço"), Required(ErrorMessage = "Preencha o Tipo de Endereço"), StringLength(35)]
        public string tipoEndereco { get; set; }

        [Display(Name = "Endereço "), Required(ErrorMessage = "Preencha o Endereço do "), StringLength(35)]
        public string endereco { get; set; }

        [Display(Name = "Número "), Required, StringLength(15)]
        public string numero { get; set; }

        [Display(Name = "Complemento "), StringLength(50)]
        public string complemento { get; set; }

        [Display(Name = "Bairro "), Required, StringLength(50)]
        public string bairro { get; set; }

        [Display(Name = "Cidade "), Required, StringLength(50)]
        public string cidade { get; set; }

        [Display(Name = "UF "), Required, StringLength(20)]
        public string uf { get; set; }

        [Display(Name = "CEP "), Required, StringLength(10)]
        public string cep { get; set; }

        [Display(Name = "Email "), Required, StringLength(300)]
        public string email { get; set; }

        public int? notaFiscalOriginal { get; set; }

        [MaxLength(15)]
        public string fone { get; set; }

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb, db);
        }
        public bool Alterar(NotaFiscalPessoa obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, db);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle db, NotaFiscalPessoa obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db);
        }
        public bool Incluir(NotaFiscalPessoa obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", null, db, pb);

                db.Set<NotaFiscalPessoa>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }

        public void Excluir(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalPessoa.Remove(this);
            banco.SaveChanges();
        }
        public void ExcluirNS(ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            new LogMudanca().Incluir(this, "", "", banco, pb);

            var lista = banco.NotaFiscalPessoa.Remove(this);
        }

        public NotaFiscalPessoa ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public NotaFiscalPessoa ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();

            var nfe = db.NotaFiscalPessoa.Where(nf => nf.id == id).FirstOrDefault();
            //db.Entry(nfe).State = EntityState.Detached;

            return nfe;
        }

        public bool Excluir(ref string erro, ParamBase paramBase, DbControle db = null)
        {
            return Excluir(this.id, ref erro, paramBase, db);
        }
        public bool Excluir(int id, ref string erro, ParamBase paramBase, DbControle db = null)
        {
            try
            {
                if (db == null)
                    db = new DbControle();

                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, paramBase);
                    db.NotaFiscalPessoa.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
                {
                    erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }


    }
}