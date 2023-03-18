namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180508Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentoPagarMestres", "Projeto_Id", c => c.Int());
            CreateIndex("dbo.DocumentoPagarMestres", "Projeto_Id");
            AddForeignKey("dbo.DocumentoPagarMestres", "Projeto_Id", "dbo.Projetoes", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentoPagarMestres", "Projeto_Id", "dbo.Projetoes");
            DropIndex("dbo.DocumentoPagarMestres", new[] { "Projeto_Id" });
            DropColumn("dbo.DocumentoPagarMestres", "Projeto_Id");
        }
    }
}
