namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181101Ricardo02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NotaFiscals", "operacao_id", "dbo.Operacaos");
            DropIndex("dbo.NotaFiscals", new[] { "operacao_id" });
            AlterColumn("dbo.NotaFiscals", "operacao_id", c => c.Int());
            CreateIndex("dbo.NotaFiscals", "operacao_id");
            AddForeignKey("dbo.NotaFiscals", "operacao_id", "dbo.Operacaos", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotaFiscals", "operacao_id", "dbo.Operacaos");
            DropIndex("dbo.NotaFiscals", new[] { "operacao_id" });
            AlterColumn("dbo.NotaFiscals", "operacao_id", c => c.Int(nullable: false));
            CreateIndex("dbo.NotaFiscals", "operacao_id");
            AddForeignKey("dbo.NotaFiscals", "operacao_id", "dbo.Operacaos", "id", cascadeDelete: true);
        }
    }
}
