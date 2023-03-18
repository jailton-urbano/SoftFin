namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181103Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContratoItemPedidoes", "unidadenegocio_id", c => c.Int(nullable: false));
            AddColumn("dbo.NotaFiscalOutroItems", "unidadenegocio_id", c => c.Int(nullable: false));
            AlterColumn("dbo.ParcelaContratoes", "Codigo", c => c.String(maxLength: 50));
            CreateIndex("dbo.ContratoItemPedidoes", "unidadenegocio_id");
            CreateIndex("dbo.NotaFiscalOutroItems", "unidadenegocio_id");
            AddForeignKey("dbo.ContratoItemPedidoes", "unidadenegocio_id", "dbo.UnidadeNegocios", "id", cascadeDelete: false);
            AddForeignKey("dbo.NotaFiscalOutroItems", "unidadenegocio_id", "dbo.UnidadeNegocios", "id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotaFiscalOutroItems", "unidadenegocio_id", "dbo.UnidadeNegocios");
            DropForeignKey("dbo.ContratoItemPedidoes", "unidadenegocio_id", "dbo.UnidadeNegocios");
            DropIndex("dbo.NotaFiscalOutroItems", new[] { "unidadenegocio_id" });
            DropIndex("dbo.ContratoItemPedidoes", new[] { "unidadenegocio_id" });
            AlterColumn("dbo.ParcelaContratoes", "Codigo", c => c.String());
            DropColumn("dbo.NotaFiscalOutroItems", "unidadenegocio_id");
            DropColumn("dbo.ContratoItemPedidoes", "unidadenegocio_id");
        }
    }
}
