namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180529Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContratoItemPedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContratoItem_Id = c.Int(nullable: false),
                        Pedido = c.String(nullable: false, maxLength: 30),
                        Descricao = c.String(maxLength: 50),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContratoItems", t => t.ContratoItem_Id, cascadeDelete: false)
                .Index(t => t.ContratoItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContratoItemPedidoes", "ContratoItem_Id", "dbo.ContratoItems");
            DropIndex("dbo.ContratoItemPedidoes", new[] { "ContratoItem_Id" });
            DropTable("dbo.ContratoItemPedidoes");
        }
    }
}
