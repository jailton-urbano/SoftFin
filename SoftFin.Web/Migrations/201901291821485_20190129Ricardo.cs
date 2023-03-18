namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190129Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bancoes", "ContaContabilMovimentacaoDC_id", c => c.Int());
            AddColumn("dbo.EmpresaContaContabils", "ContaContabilRecebimentoDC_id", c => c.Int());
            CreateIndex("dbo.Bancoes", "ContaContabilMovimentacaoDC_id");
            CreateIndex("dbo.EmpresaContaContabils", "ContaContabilRecebimentoDC_id");
            AddForeignKey("dbo.Bancoes", "ContaContabilMovimentacaoDC_id", "dbo.ContaContabils", "id");
            AddForeignKey("dbo.EmpresaContaContabils", "ContaContabilRecebimentoDC_id", "dbo.ContaContabils", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmpresaContaContabils", "ContaContabilRecebimentoDC_id", "dbo.ContaContabils");
            DropForeignKey("dbo.Bancoes", "ContaContabilMovimentacaoDC_id", "dbo.ContaContabils");
            DropIndex("dbo.EmpresaContaContabils", new[] { "ContaContabilRecebimentoDC_id" });
            DropIndex("dbo.Bancoes", new[] { "ContaContabilMovimentacaoDC_id" });
            DropColumn("dbo.EmpresaContaContabils", "ContaContabilRecebimentoDC_id");
            DropColumn("dbo.Bancoes", "ContaContabilMovimentacaoDC_id");
        }
    }
}
