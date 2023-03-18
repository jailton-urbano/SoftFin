namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181219Ricardo03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "notaFiscalIntermediario_id", c => c.Int());
            AddColumn("dbo.NotaFiscals", "ValAliqCOFINS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.NotaFiscals", "notaFiscalIntermediario_id");
            AddForeignKey("dbo.NotaFiscals", "notaFiscalIntermediario_id", "dbo.NotaFiscalPessoas", "id");
            DropColumn("dbo.NotaFiscals", "ValBCCOFINS");
            DropColumn("dbo.NotaFiscals", "ValBCINSS");
            DropColumn("dbo.NotaFiscals", "ValBCCSLL");
            DropColumn("dbo.NotaFiscals", "ISSRetido");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NotaFiscals", "ISSRetido", c => c.Boolean(nullable: false));
            AddColumn("dbo.NotaFiscals", "ValBCCSLL", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ValBCINSS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "ValBCCOFINS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.NotaFiscals", "notaFiscalIntermediario_id", "dbo.NotaFiscalPessoas");
            DropIndex("dbo.NotaFiscals", new[] { "notaFiscalIntermediario_id" });
            DropColumn("dbo.NotaFiscals", "ValAliqCOFINS");
            DropColumn("dbo.NotaFiscals", "notaFiscalIntermediario_id");
        }
    }
}
