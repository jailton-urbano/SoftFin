using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class Arquivos
    {
        public Arquivos()
        {
            DataInclucao = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }

        public DateTime DataInclucao { get; set; }

        [MaxLength(50)]
        public String Descricao { get; set; }

        [Display(Name = "Arquivo Real"),
        Required(ErrorMessage = "*"), MaxLength(750)]
        public string ArquivoReal { get; set; }
        [Display(Name = "Arquivo Original"),
        Required(ErrorMessage = "*"), MaxLength(750)]
        public string ArquivoOriginal { get; set; }

        public int Tamanho { get; set; }

        [MaxLength(10)]
        public string ArquivoExtensao { get; set; }

        [MaxLength(75)]
        public string RotinaOwner { get; set; }


        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public string Empresa { get; set; }
        public string Codigo { get; set; }
        [MaxLength(200)]
        public string Usuario { get;  set; }
        [MaxLength(50)]
        public string Processo { get;  set; }

        public Arquivos ObterPorId(int id, string codigoEmpresa, DBGPControle db = null)
        {
            
            if (db == null)
                db = new DBGPControle();

            return db.Arquivos.Where(x => x.Id == id && x.Empresa == codigoEmpresa).FirstOrDefault();
        }

        public void Excluir(int id, string codigoEmpresa, DBGPControle db = null)
        {
            
            if (db == null)
                db = new DBGPControle();
            var x = ObterPorId(id, codigoEmpresa, db);
            db.Arquivos.Remove(x);
            db.SaveChanges();
        }

        public void Salvar()
        {
            DBGPControle db = new DBGPControle();
            db.Set<Arquivos>().Add(this);
            db.SaveChanges();
        }

        public List<Arquivos> ObterPorNomeDeArquivo(string owner, string nomearquivo,  string processo, string codigoEmpresa, DBGPControle db = null )
        {
            
            if (db == null)
                db = new DBGPControle();

            return db.Arquivos.Where(x => x.RotinaOwner == owner && x.Empresa == codigoEmpresa && x.ArquivoOriginal == nomearquivo && x.Processo == processo).ToList();
        }

        public List<Arquivos> ObterPorCodigo(string owner, string empresa, string codigo, DBGPControle db = null)
        {

            if (db == null)
                db = new DBGPControle();

            return db.Arquivos.Where(x => x.RotinaOwner == owner && x.Empresa == empresa && x.Codigo == codigo).ToList();
        }
    }

}