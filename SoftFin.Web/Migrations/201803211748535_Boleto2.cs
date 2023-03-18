namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Boleto2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boletoes", "OrdemVenda_ID", c => c.Int());
            CreateIndex("dbo.Boletoes", "OrdemVenda_ID");
            AddForeignKey("dbo.Boletoes", "OrdemVenda_ID", "dbo.OrdemVendas", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Boletoes", "OrdemVenda_ID", "dbo.OrdemVendas");
            DropIndex("dbo.Boletoes", new[] { "OrdemVenda_ID" });
            DropColumn("dbo.Boletoes", "OrdemVenda_ID");
        }
    }
}
