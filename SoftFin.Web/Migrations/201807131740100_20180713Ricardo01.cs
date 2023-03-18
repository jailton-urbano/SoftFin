namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180713Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContratoItemUnidadeNegocios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContratoItem_Id = c.Int(nullable: false),
                        UnidadeNegocio_Id = c.Int(nullable: false),
                        Descricao = c.String(maxLength: 50),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContratoItems", t => t.ContratoItem_Id, cascadeDelete: false)
                .ForeignKey("dbo.UnidadeNegocios", t => t.UnidadeNegocio_Id, cascadeDelete: false)
                .Index(t => t.ContratoItem_Id)
                .Index(t => t.UnidadeNegocio_Id);
            
            AddColumn("dbo.ParcelaContratoes", "Codigo", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContratoItemUnidadeNegocios", "UnidadeNegocio_Id", "dbo.UnidadeNegocios");
            DropForeignKey("dbo.ContratoItemUnidadeNegocios", "ContratoItem_Id", "dbo.ContratoItems");
            DropIndex("dbo.ContratoItemUnidadeNegocios", new[] { "UnidadeNegocio_Id" });
            DropIndex("dbo.ContratoItemUnidadeNegocios", new[] { "ContratoItem_Id" });
            DropColumn("dbo.ParcelaContratoes", "Codigo");
            DropTable("dbo.ContratoItemUnidadeNegocios");
        }
    }
}
