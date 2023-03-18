namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ajustes2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LancamentoContabils", "notafiscal_id", c => c.Int());
            CreateIndex("dbo.LancamentoContabils", "notafiscal_id");
            AddForeignKey("dbo.LancamentoContabils", "notafiscal_id", "dbo.NotaFiscals", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LancamentoContabils", "notafiscal_id", "dbo.NotaFiscals");
            DropIndex("dbo.LancamentoContabils", new[] { "notafiscal_id" });
            DropColumn("dbo.LancamentoContabils", "notafiscal_id");
        }
    }
}
