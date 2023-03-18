namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Boleto3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Boletoes", "NotaFiscal_ID", "dbo.NotaFiscals");
            DropForeignKey("dbo.Boletoes", "OrdemVenda_ID", "dbo.OrdemVendas");
            DropIndex("dbo.Boletoes", new[] { "NotaFiscal_ID" });
            DropIndex("dbo.Boletoes", new[] { "OrdemVenda_ID" });
            AddColumn("dbo.Boletoes", "NomeArquivo", c => c.String());
            AlterColumn("dbo.Boletoes", "OrdemVenda_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Boletoes", "OrdemVenda_ID");
            AddForeignKey("dbo.Boletoes", "OrdemVenda_ID", "dbo.OrdemVendas", "id", cascadeDelete: false);
            DropColumn("dbo.Boletoes", "NotaFiscal_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Boletoes", "NotaFiscal_ID", c => c.Int());
            DropForeignKey("dbo.Boletoes", "OrdemVenda_ID", "dbo.OrdemVendas");
            DropIndex("dbo.Boletoes", new[] { "OrdemVenda_ID" });
            AlterColumn("dbo.Boletoes", "OrdemVenda_ID", c => c.Int());
            DropColumn("dbo.Boletoes", "NomeArquivo");
            CreateIndex("dbo.Boletoes", "OrdemVenda_ID");
            CreateIndex("dbo.Boletoes", "NotaFiscal_ID");
            AddForeignKey("dbo.Boletoes", "OrdemVenda_ID", "dbo.OrdemVendas", "id");
            AddForeignKey("dbo.Boletoes", "NotaFiscal_ID", "dbo.NotaFiscals", "id");
        }
    }
}
