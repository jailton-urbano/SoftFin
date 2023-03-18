namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190226Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaldoContabilDetalhes", "CodigoCentroCusto", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.SaldoContabilDetalhes", "DescricaoCentroCusto", c => c.String(nullable: false, maxLength: 150));
            DropColumn("dbo.SaldoContabils", "CodigoCentroCusto");
            DropColumn("dbo.SaldoContabils", "DescricaoCentroCusto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SaldoContabils", "DescricaoCentroCusto", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.SaldoContabils", "CodigoCentroCusto", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.SaldoContabilDetalhes", "DescricaoCentroCusto");
            DropColumn("dbo.SaldoContabilDetalhes", "CodigoCentroCusto");
        }
    }
}
