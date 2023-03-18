namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190213Ricardo01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LancamentoContabilDetalhes", "unidadeNegocio_ID", "dbo.UnidadeNegocios");
            DropIndex("dbo.LancamentoContabilDetalhes", new[] { "unidadeNegocio_ID" });
            AddColumn("dbo.LancamentoContabils", "UnidadeNegocio_ID", c => c.Int(nullable: true));
            CreateIndex("dbo.LancamentoContabils", "UnidadeNegocio_ID");
            AddForeignKey("dbo.LancamentoContabils", "UnidadeNegocio_ID", "dbo.UnidadeNegocios", "id", cascadeDelete: false);
            DropColumn("dbo.LancamentoContabilDetalhes", "unidadeNegocio_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LancamentoContabilDetalhes", "unidadeNegocio_ID", c => c.Int(nullable: false));
            DropForeignKey("dbo.LancamentoContabils", "UnidadeNegocio_ID", "dbo.UnidadeNegocios");
            DropIndex("dbo.LancamentoContabils", new[] { "UnidadeNegocio_ID" });
            DropColumn("dbo.LancamentoContabils", "UnidadeNegocio_ID");
            CreateIndex("dbo.LancamentoContabilDetalhes", "unidadeNegocio_ID");
            AddForeignKey("dbo.LancamentoContabilDetalhes", "unidadeNegocio_ID", "dbo.UnidadeNegocios", "id", cascadeDelete: true);
        }
    }
}
