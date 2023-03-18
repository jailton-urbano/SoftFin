namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181101Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.NotaFiscals", "codigoServico", c => c.String(maxLength: 50));
            AlterColumn("dbo.NotaFiscals", "aliquotaINSS", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.NotaFiscals", "valorINSS", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.NotaFiscals", "valorINSS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.NotaFiscals", "aliquotaINSS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.NotaFiscals", "codigoServico", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
