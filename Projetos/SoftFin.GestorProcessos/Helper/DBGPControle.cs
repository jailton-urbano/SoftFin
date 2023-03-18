using SoftFin.GestorProcessos.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Helper
{
    public class DBGPControle : DbContext
    {
        public DBGPControle()
        {

        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Student>()
        //        .HasOptional<Standard>(s => s.Standard)
        //        .WithMany()
        //        .WillCascadeOnDelete(false);
        //}
        public DbSet<AtividadeTipo> AtividadeTipo { get; set; }
        public DbSet<Funcao> Funcao { get; set; }
        public DbSet<Versao> Versao { get; set; }
        public DbSet<AtividadeFuncao> AtividadeFuncao { get; set; }
        public DbSet<Atividade> Atividade { get; set; }
        public DbSet<Processo> Processo { get; set; }
        public DbSet<ProcessoExecucao> ProcessoExecucao { get; set; }
        public DbSet<ProcessoExecucaoAtividade> ProcessoExecucaoAtividade { get; set; }
        public DbSet<AtividadePlano> AtividadePlano { get; set; }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioFuncao> UsuarioFuncao { get; set; }

        public DbSet<Tabela> Tabela { get; set; }
        public DbSet<TipoCampo> TipoCampo { get; set; }
        public DbSet<TabelaCampo> TabelaCampo { get; set; }
        public DbSet<Visao> Visao { get; set; }
        public DbSet<VisaoCampo> VisaoCampo { get; set; }

        public DbSet<AtividadeVisao> AtividadeVisao { get; set; }
 
        public DbSet<ProcessoArquivo> ProcessoArquivo { get; set; }

        public DbSet<Arquivos> Arquivos { get; set; }

        public DbSet<ProcessoAnotacao> ProcessoAnotacao { get; set; }

        public DbSet<Hashcode> Hashcode { get; set; }
        public DbSet<TipoVisao> TipoVisao { get; set; }

        public DbSet<VisaoHashcode> VisaoHashcode { get; set; }
        public DbSet<EmpresaParametro> EmpresaParametro { get; set; }

    }
}