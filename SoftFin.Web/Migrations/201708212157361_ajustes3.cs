namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajustes3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pagamentoes", "contaContabilCredito_id", c => c.Int());
            AddColumn("dbo.LancamentoContabils", "recebimento_id", c => c.Int());
            AddColumn("dbo.LancamentoContabils", "pagamento_id", c => c.Int());
            CreateIndex("dbo.Pagamentoes", "contaContabilCredito_id");
            CreateIndex("dbo.LancamentoContabils", "recebimento_id");
            CreateIndex("dbo.LancamentoContabils", "pagamento_id");
            AddForeignKey("dbo.Pagamentoes", "contaContabilCredito_id", "dbo.ContaContabils", "id");
            AddForeignKey("dbo.LancamentoContabils", "pagamento_id", "dbo.Pagamentoes", "id");
            AddForeignKey("dbo.LancamentoContabils", "recebimento_id", "dbo.Recebimentoes", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LancamentoContabils", "recebimento_id", "dbo.Recebimentoes");
            DropForeignKey("dbo.LancamentoContabils", "pagamento_id", "dbo.Pagamentoes");
            DropForeignKey("dbo.Pagamentoes", "contaContabilCredito_id", "dbo.ContaContabils");
            DropIndex("dbo.LancamentoContabils", new[] { "pagamento_id" });
            DropIndex("dbo.LancamentoContabils", new[] { "recebimento_id" });
            DropIndex("dbo.Pagamentoes", new[] { "contaContabilCredito_id" });
            DropColumn("dbo.LancamentoContabils", "pagamento_id");
            DropColumn("dbo.LancamentoContabils", "recebimento_id");
            DropColumn("dbo.Pagamentoes", "contaContabilCredito_id");
        }
    }
}
