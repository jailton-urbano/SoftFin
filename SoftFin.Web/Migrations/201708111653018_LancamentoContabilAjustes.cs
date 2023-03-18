namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LancamentoContabilAjustes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LancamentoContabils", "origemmovimento_id", c => c.Int(nullable: false));
            AddColumn("dbo.LancamentoContabils", "DocumentoPagarParcela_id", c => c.Int());
            CreateIndex("dbo.LancamentoContabils", "origemmovimento_id");
            CreateIndex("dbo.LancamentoContabils", "DocumentoPagarParcela_id");
            AddForeignKey("dbo.LancamentoContabils", "DocumentoPagarParcela_id", "dbo.DocumentoPagarParcelas", "id");
            AddForeignKey("dbo.LancamentoContabils", "origemmovimento_id", "dbo.OrigemMovimentoes", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LancamentoContabils", "origemmovimento_id", "dbo.OrigemMovimentoes");
            DropForeignKey("dbo.LancamentoContabils", "DocumentoPagarParcela_id", "dbo.DocumentoPagarParcelas");
            DropIndex("dbo.LancamentoContabils", new[] { "DocumentoPagarParcela_id" });
            DropIndex("dbo.LancamentoContabils", new[] { "origemmovimento_id" });
            DropColumn("dbo.LancamentoContabils", "DocumentoPagarParcela_id");
            DropColumn("dbo.LancamentoContabils", "origemmovimento_id");
        }
    }
}
