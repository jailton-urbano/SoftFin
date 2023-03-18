namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190215Guilherme01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaldoContabils", "CodigoCentroCusto", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.SaldoContabils", "DescricaoCentroCusto", c => c.String(nullable: false, maxLength: 150));
            DropColumn("dbo.SaldoContabilDetalhes", "CodigoCentroCusto");
            DropColumn("dbo.SaldoContabilDetalhes", "DescricaoCentroCusto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SaldoContabilDetalhes", "DescricaoCentroCusto", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.SaldoContabilDetalhes", "CodigoCentroCusto", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.SaldoContabils", "DescricaoCentroCusto");
            DropColumn("dbo.SaldoContabils", "CodigoCentroCusto");
        }
    }
}
