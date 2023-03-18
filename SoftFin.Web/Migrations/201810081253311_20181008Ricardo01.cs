namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181008Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeParaItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        De = c.String(nullable: false, maxLength: 300),
                        Para = c.String(nullable: false, maxLength: 300),
                        DePara_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DeParaMestres", t => t.DePara_Id, cascadeDelete: false)
                .Index(t => t.DePara_Id);
            
            CreateTable(
                "dbo.DeParaMestres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(nullable: false, maxLength: 20),
                        Descricao = c.String(nullable: false, maxLength: 70),
                        Estabelecimento_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estabelecimentoes", t => t.Estabelecimento_id, cascadeDelete: false)
                .Index(t => t.Estabelecimento_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeParaItems", "DePara_Id", "dbo.DeParaMestres");
            DropForeignKey("dbo.DeParaMestres", "Estabelecimento_id", "dbo.Estabelecimentoes");
            DropIndex("dbo.DeParaMestres", new[] { "Estabelecimento_id" });
            DropIndex("dbo.DeParaItems", new[] { "DePara_Id" });
            DropTable("dbo.DeParaMestres");
            DropTable("dbo.DeParaItems");
        }
    }
}
