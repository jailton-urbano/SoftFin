namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ricardo17082019_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotaFiscalNFEFormaPagamentoes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        notaFiscal_id = c.Int(nullable: false),
                        indPag = c.Byte(),
                        tPag = c.Byte(),
                        vPag = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tpIntegra = c.Byte(),
                        CNPJ = c.String(maxLength: 14),
                        tBand = c.Byte(),
                        cAut = c.String(maxLength: 20),
                        vTroco = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.NotaFiscalNFEs", t => t.notaFiscal_id, cascadeDelete: false)
                .Index(t => t.notaFiscal_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotaFiscalNFEFormaPagamentoes", "notaFiscal_id", "dbo.NotaFiscalNFEs");
            DropIndex("dbo.NotaFiscalNFEFormaPagamentoes", new[] { "notaFiscal_id" });
            DropTable("dbo.NotaFiscalNFEFormaPagamentoes");
        }
    }
}
