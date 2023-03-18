namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PessoaContaContabilTirandoObrigacao : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id", "dbo.ContaContabils");
            DropForeignKey("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id", "dbo.ContaContabils");
            DropForeignKey("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id", "dbo.ContaContabils");
            DropIndex("dbo.PessoaContaContabils", new[] { "contaContabilDespesaPadrao_id" });
            DropIndex("dbo.PessoaContaContabils", new[] { "contaContabilPagarPadrao_id" });
            DropIndex("dbo.PessoaContaContabils", new[] { "contaContabilReceberPadrao_id" });
            AlterColumn("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id", c => c.Int());
            AlterColumn("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id", c => c.Int());
            AlterColumn("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id", c => c.Int());
            CreateIndex("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id");
            CreateIndex("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id");
            CreateIndex("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id");
            AddForeignKey("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id", "dbo.ContaContabils", "id");
            AddForeignKey("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id", "dbo.ContaContabils", "id");
            AddForeignKey("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id", "dbo.ContaContabils", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id", "dbo.ContaContabils");
            DropForeignKey("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id", "dbo.ContaContabils");
            DropForeignKey("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id", "dbo.ContaContabils");
            DropIndex("dbo.PessoaContaContabils", new[] { "contaContabilReceberPadrao_id" });
            DropIndex("dbo.PessoaContaContabils", new[] { "contaContabilPagarPadrao_id" });
            DropIndex("dbo.PessoaContaContabils", new[] { "contaContabilDespesaPadrao_id" });
            AlterColumn("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id", c => c.Int(nullable: false));
            AlterColumn("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id", c => c.Int(nullable: false));
            AlterColumn("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id", c => c.Int(nullable: false));
            CreateIndex("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id");
            CreateIndex("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id");
            CreateIndex("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id");
            AddForeignKey("dbo.PessoaContaContabils", "contaContabilReceberPadrao_id", "dbo.ContaContabils", "id", cascadeDelete: true);
            AddForeignKey("dbo.PessoaContaContabils", "contaContabilPagarPadrao_id", "dbo.ContaContabils", "id", cascadeDelete: true);
            AddForeignKey("dbo.PessoaContaContabils", "contaContabilDespesaPadrao_id", "dbo.ContaContabils", "id", cascadeDelete: true);
        }
    }
}
