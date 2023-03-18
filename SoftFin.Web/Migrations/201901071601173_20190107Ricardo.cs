namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190107Ricardo : DbMigration
    {
        public override void Up()
        {
            Sql("delete from dbo.EmpresaContaContabils;");
            AddColumn("dbo.EmpresaContaContabils", "ContaContabilOutro_id", c => c.Int(nullable: false));
            CreateIndex("dbo.EmpresaContaContabils", "ContaContabilOutro_id");
            AddForeignKey("dbo.EmpresaContaContabils", "ContaContabilOutro_id", "dbo.ContaContabils", "id", cascadeDelete: false);

            RenameColumn(table: "dbo.EmpresaContaContabils", name: "contaContabilDespesaPadrao_id", newName: "ContaContabilTitulo_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "contaContabilNFe_id" , newName: "ContaContabilNFMercadoria_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "contaContabilNFSe_id", newName: "ContaContabilNFServico_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "contaContabilPagarPadrao_id", newName: "ContaContabilPagamento_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "contaContabilReceberPadrao_id", newName: "ContaContabilRecebimento_id");

            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_contaContabilDespesaPadrao_id", newName: "IX_ContaContabilTitulo_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_contaContabilNFe_id", newName: "IX_ContaContabilNFMercadoria_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_contaContabilNFSe_id", newName: "IX_ContaContabilNFServico_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_contaContabilPagarPadrao_id", newName: "IX_ContaContabilPagamento_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_contaContabilReceberPadrao_id", newName: "IX_ContaContabilRecebimento_id");
            

            


        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmpresaContaContabils", "ContaContabilOutro_id", "dbo.ContaContabils");
            DropIndex("dbo.EmpresaContaContabils", new[] { "ContaContabilOutro_id" });
            DropColumn("dbo.EmpresaContaContabils", "ContaContabilOutro_id");


            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_ContaContabilTitulo_id", newName: "IX_contaContabilDespesaPadrao_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_ContaContabilNFMercadoria_id", newName: "IX_contaContabilNFe_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_ContaContabilNFServico_id", newName: "IX_contaContabilNFSe_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_ContaContabilPagamento_id", newName: "IX_contaContabilPagarPadrao_id");
            RenameIndex(table: "dbo.EmpresaContaContabils", name: "IX_ContaContabilRecebimento_id", newName: "IX_contaContabilReceberPadrao_id");

            RenameColumn(table: "dbo.EmpresaContaContabils", name: "ContaContabilTitulo_id", newName: "contaContabilDespesaPadrao_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "ContaContabilNFMercadoria_id", newName: "contaContabilNFe_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "ContaContabilNFServico_id", newName: "contaContabilNFSe_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "ContaContabilPagamento_id", newName: "contaContabilPagarPadrao_id");
            RenameColumn(table: "dbo.EmpresaContaContabils", name: "ContaContabilRecebimento_id", newName: "contaContabilReceberPadrao_id");
        }
    }
}
