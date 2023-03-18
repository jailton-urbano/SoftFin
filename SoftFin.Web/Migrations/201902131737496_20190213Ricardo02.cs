namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190213Ricardo02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LancamentoContabils", "UnidadeNegocio_ID", "dbo.UnidadeNegocios");
            DropIndex("dbo.LancamentoContabils", new[] { "UnidadeNegocio_ID" });
            AlterColumn("dbo.LancamentoContabils", "UnidadeNegocio_ID", c => c.Int());
            CreateIndex("dbo.LancamentoContabils", "UnidadeNegocio_ID");
            AddForeignKey("dbo.LancamentoContabils", "UnidadeNegocio_ID", "dbo.UnidadeNegocios", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LancamentoContabils", "UnidadeNegocio_ID", "dbo.UnidadeNegocios");
            DropIndex("dbo.LancamentoContabils", new[] { "UnidadeNegocio_ID" });
            AlterColumn("dbo.LancamentoContabils", "UnidadeNegocio_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.LancamentoContabils", "UnidadeNegocio_ID");
            AddForeignKey("dbo.LancamentoContabils", "UnidadeNegocio_ID", "dbo.UnidadeNegocios", "id", cascadeDelete: true);
        }
    }
}
