namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181917Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentoPagarProjetoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentoPagarMestre_id = c.Int(nullable: false),
                        Historico = c.String(maxLength: 50),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Projeto_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentoPagarMestres", t => t.DocumentoPagarMestre_id, cascadeDelete: false)
                .ForeignKey("dbo.Projetoes", t => t.Projeto_Id)
                .Index(t => t.DocumentoPagarMestre_id)
                .Index(t => t.Projeto_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentoPagarProjetoes", "Projeto_Id", "dbo.Projetoes");
            DropForeignKey("dbo.DocumentoPagarProjetoes", "DocumentoPagarMestre_id", "dbo.DocumentoPagarMestres");
            DropIndex("dbo.DocumentoPagarProjetoes", new[] { "Projeto_Id" });
            DropIndex("dbo.DocumentoPagarProjetoes", new[] { "DocumentoPagarMestre_id" });
            DropTable("dbo.DocumentoPagarProjetoes");
        }
    }
}
