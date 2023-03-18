using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.POC.AnaliseAmostra.Models
{
    public class DBAAContext: DbContext
    {
        public DbSet<BAS_CLIENTE> BAS_CLIENTE { get; set; }
        public DbSet<BAS_CAD_TECNICO> BAS_CAD_TECNICO { get; set; }
        public DbSet<BAS_GP_ANALISE> BAS_GP_ANALISE { get; set; }
        public DbSet<BAS_PROCEDIMENTO> BAS_PROCEDIMENTO { get; set; }
        public DbSet<BAS_TEMPERATURA> BAS_TEMPERATURA { get; set; }
        public DbSet<POC_RETIRADA_ANALISE_AMOSTRA_CAB> POC_RETIRADA_ANALISE_AMOSTRA_CAB { get; set; }
        public DbSet<POC_RETIRADA_ANALISE_AMOSTRA_ITEM> POC_RETIRADA_ANALISE_AMOSTRA_ITEM { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }

    }
}