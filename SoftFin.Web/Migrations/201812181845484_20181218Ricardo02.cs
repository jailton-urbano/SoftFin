namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181218Ricardo02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "ValAliqPIS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ValBCCOFINS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ValBCINSS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ValBCCSLL", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ISSRetido", c => c.Boolean(nullable: false));
            AddColumn("dbo.NotaFiscals", "ValISSRetido", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "RespRetencao", c => c.String());
            AddColumn("dbo.NotaFiscals", "ValAliqCSLL", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ValAliqISSRetido", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotaFiscals", "ValAliqISSRetido");
            DropColumn("dbo.NotaFiscals", "ValAliqCSLL");
            DropColumn("dbo.NotaFiscals", "RespRetencao");
            DropColumn("dbo.NotaFiscals", "ValISSRetido");
            DropColumn("dbo.NotaFiscals", "ISSRetido");
            DropColumn("dbo.NotaFiscals", "ValBCCSLL");
            DropColumn("dbo.NotaFiscals", "ValBCINSS");
            DropColumn("dbo.NotaFiscals", "ValBCCOFINS");
            DropColumn("dbo.NotaFiscals", "ValAliqPIS");
        }
    }
}
