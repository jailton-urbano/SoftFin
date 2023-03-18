namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190107Ricardo01 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.NotaFiscals", new[] { "ordemVenda_id" });
            CreateIndex("dbo.NotaFiscals", "ordemVenda_id", unique: true, name: "IX_ORDEM_UNICO");
        }
        
        public override void Down()
        {
            DropIndex("dbo.NotaFiscals", "IX_ORDEM_UNICO");
            CreateIndex("dbo.NotaFiscals", "ordemVenda_id");
        }
    }
}
