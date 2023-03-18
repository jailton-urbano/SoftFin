namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ajustes1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContaContabils", "planoDeContas_id", "dbo.PlanoDeContas");
            DropIndex("dbo.ContaContabils", new[] { "planoDeContas_id" });
            CreateTable(
                "dbo.ContaContabilCategoriaDespesas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        contaContabil_id = c.Int(nullable: false),
                        planoDeContas_id = c.Int(nullable: false),
                        empresa_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ContaContabils", t => t.contaContabil_id, cascadeDelete: false)
                .ForeignKey("dbo.Empresas", t => t.empresa_id, cascadeDelete: false)
                .ForeignKey("dbo.PlanoDeContas", t => t.planoDeContas_id, cascadeDelete: false)
                .Index(t => t.contaContabil_id)
                .Index(t => t.planoDeContas_id)
                .Index(t => t.empresa_id);
            
            AddColumn("dbo.EmpresaContaContabils", "contaContabilNFe_id", c => c.Int(nullable: false));
            AddColumn("dbo.EmpresaContaContabils", "contaContabilNFSe_id", c => c.Int(nullable: false));
            CreateIndex("dbo.EmpresaContaContabils", "contaContabilNFe_id");
            CreateIndex("dbo.EmpresaContaContabils", "contaContabilNFSe_id");
            AddForeignKey("dbo.EmpresaContaContabils", "contaContabilNFe_id", "dbo.ContaContabils", "id", cascadeDelete: false);
            AddForeignKey("dbo.EmpresaContaContabils", "contaContabilNFSe_id", "dbo.ContaContabils", "id", cascadeDelete: false);
            DropColumn("dbo.ContaContabils", "planoDeContas_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContaContabils", "planoDeContas_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.EmpresaContaContabils", "contaContabilNFSe_id", "dbo.ContaContabils");
            DropForeignKey("dbo.EmpresaContaContabils", "contaContabilNFe_id", "dbo.ContaContabils");
            DropForeignKey("dbo.ContaContabilCategoriaDespesas", "planoDeContas_id", "dbo.PlanoDeContas");
            DropForeignKey("dbo.ContaContabilCategoriaDespesas", "empresa_id", "dbo.Empresas");
            DropForeignKey("dbo.ContaContabilCategoriaDespesas", "contaContabil_id", "dbo.ContaContabils");
            DropIndex("dbo.EmpresaContaContabils", new[] { "contaContabilNFSe_id" });
            DropIndex("dbo.EmpresaContaContabils", new[] { "contaContabilNFe_id" });
            DropIndex("dbo.ContaContabilCategoriaDespesas", new[] { "empresa_id" });
            DropIndex("dbo.ContaContabilCategoriaDespesas", new[] { "planoDeContas_id" });
            DropIndex("dbo.ContaContabilCategoriaDespesas", new[] { "contaContabil_id" });
            DropColumn("dbo.EmpresaContaContabils", "contaContabilNFSe_id");
            DropColumn("dbo.EmpresaContaContabils", "contaContabilNFe_id");
            DropTable("dbo.ContaContabilCategoriaDespesas");
            CreateIndex("dbo.ContaContabils", "planoDeContas_id");
            AddForeignKey("dbo.ContaContabils", "planoDeContas_id", "dbo.PlanoDeContas", "id", cascadeDelete: false);
        }
    }
}
