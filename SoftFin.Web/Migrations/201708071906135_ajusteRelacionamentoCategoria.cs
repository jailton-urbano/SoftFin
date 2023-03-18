namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajusteRelacionamentoCategoria : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContaContabilCategoriaDespesas", "contaContabil_id", "dbo.ContaContabils");
            DropForeignKey("dbo.ContaContabilCategoriaDespesas", "planoDeContas_id", "dbo.PlanoDeContas");
            DropIndex("dbo.ContaContabilCategoriaDespesas", new[] { "contaContabil_id" });
            DropIndex("dbo.ContaContabilCategoriaDespesas", new[] { "planoDeContas_id" });
            DropTable("dbo.ContaContabilCategoriaDespesas");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ContaContabilCategoriaDespesas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        contaContabil_id = c.Int(nullable: false),
                        planoDeContas_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateIndex("dbo.ContaContabilCategoriaDespesas", "planoDeContas_id");
            CreateIndex("dbo.ContaContabilCategoriaDespesas", "contaContabil_id");
            AddForeignKey("dbo.ContaContabilCategoriaDespesas", "planoDeContas_id", "dbo.PlanoDeContas", "id", cascadeDelete: true);
            AddForeignKey("dbo.ContaContabilCategoriaDespesas", "contaContabil_id", "dbo.ContaContabils", "id", cascadeDelete: true);
        }
    }
}
