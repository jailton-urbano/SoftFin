namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuPrincipalRefactory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SistemaPrincipals",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        descricao = c.String(),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SistemaPrincipalPerfils",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        perfil_id = c.Int(nullable: false),
                        sistemaprincipal_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Perfils", t => t.perfil_id, cascadeDelete: true)
                .ForeignKey("dbo.SistemaPrincipals", t => t.sistemaprincipal_id, cascadeDelete: true)
                .Index(t => t.perfil_id)
                .Index(t => t.sistemaprincipal_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SistemaPrincipalPerfils", "sistemaprincipal_id", "dbo.SistemaPrincipals");
            DropForeignKey("dbo.SistemaPrincipalPerfils", "perfil_id", "dbo.Perfils");
            DropIndex("dbo.SistemaPrincipalPerfils", new[] { "sistemaprincipal_id" });
            DropIndex("dbo.SistemaPrincipalPerfils", new[] { "perfil_id" });
            DropTable("dbo.SistemaPrincipalPerfils");
            DropTable("dbo.SistemaPrincipals");
        }
    }
}
