namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajusteRelacionamentoCategoria2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContaContabils", "planoDeContas_id", c => c.Int(nullable: false));
            CreateIndex("dbo.ContaContabils", "planoDeContas_id");
            AddForeignKey("dbo.ContaContabils", "planoDeContas_id", "dbo.PlanoDeContas", "id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContaContabils", "planoDeContas_id", "dbo.PlanoDeContas");
            DropIndex("dbo.ContaContabils", new[] { "planoDeContas_id" });
            DropColumn("dbo.ContaContabils", "planoDeContas_id");
        }
    }
}
