namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180529Ricardo02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContratoItemPedidoes", "ContratoItem_Id", "dbo.ContratoItems");
            DropIndex("dbo.ContratoItemPedidoes", new[] { "ContratoItem_Id" });
            AddColumn("dbo.ContratoItemPedidoes", "ParcelaContrato_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ContratoItemPedidoes", "ParcelaContrato_Id");
            AddForeignKey("dbo.ContratoItemPedidoes", "ParcelaContrato_Id", "dbo.ParcelaContratoes", "id", cascadeDelete: false);
            DropColumn("dbo.ContratoItemPedidoes", "ContratoItem_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContratoItemPedidoes", "ContratoItem_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.ContratoItemPedidoes", "ParcelaContrato_Id", "dbo.ParcelaContratoes");
            DropIndex("dbo.ContratoItemPedidoes", new[] { "ParcelaContrato_Id" });
            DropColumn("dbo.ContratoItemPedidoes", "ParcelaContrato_Id");
            CreateIndex("dbo.ContratoItemPedidoes", "ContratoItem_Id");
            AddForeignKey("dbo.ContratoItemPedidoes", "ContratoItem_Id", "dbo.ContratoItems", "id", cascadeDelete: false);
        }
    }
}
