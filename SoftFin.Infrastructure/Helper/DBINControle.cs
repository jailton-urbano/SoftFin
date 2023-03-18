
using SoftFin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.InfrastructureHelper
{
    public class DBINControle : DbContext
    {
        public DBINControle()
        {

        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Student>()
        //        .HasOptional<Standard>(s => s.Standard)
        //        .WithMany()
        //        .WillCascadeOnDelete(false);
        //}
        public DbSet<LogEvento> LogEvento { get; set; }
        public DbSet<LogImportar> LogImportacao { get; set; }

        public DbSet<LogComercial> LogComercial { get; set; }
    }
}