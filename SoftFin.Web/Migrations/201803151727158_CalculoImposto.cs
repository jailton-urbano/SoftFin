namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalculoImposto : DbMigration
    {
        public override void Up()
        {
            Sql("delete from calculoImpostoes where estabelecimento_id is null");
            DropForeignKey("dbo.calculoImpostoes", "empresa_id", "dbo.Empresas");
            DropForeignKey("dbo.calculoImpostoes", "estabelecimento_id", "dbo.Estabelecimentoes");
            DropIndex("dbo.calculoImpostoes", new[] { "empresa_id" });
            DropIndex("dbo.calculoImpostoes", new[] { "estabelecimento_id" });
            AlterColumn("dbo.calculoImpostoes", "estabelecimento_id", c => c.Int(nullable: false));
            CreateIndex("dbo.calculoImpostoes", "estabelecimento_id");
            AddForeignKey("dbo.calculoImpostoes", "estabelecimento_id", "dbo.Estabelecimentoes", "id", cascadeDelete: true);
            DropColumn("dbo.calculoImpostoes", "empresa_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.calculoImpostoes", "empresa_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.calculoImpostoes", "estabelecimento_id", "dbo.Estabelecimentoes");
            DropIndex("dbo.calculoImpostoes", new[] { "estabelecimento_id" });
            AlterColumn("dbo.calculoImpostoes", "estabelecimento_id", c => c.Int());
            CreateIndex("dbo.calculoImpostoes", "estabelecimento_id");
            CreateIndex("dbo.calculoImpostoes", "empresa_id");
            AddForeignKey("dbo.calculoImpostoes", "estabelecimento_id", "dbo.Estabelecimentoes", "id");
            AddForeignKey("dbo.calculoImpostoes", "empresa_id", "dbo.Empresas", "id", cascadeDelete: true);
        }
    }
}
