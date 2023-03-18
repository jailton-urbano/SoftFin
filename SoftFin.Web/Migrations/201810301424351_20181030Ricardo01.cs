namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181030Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotaFiscalOutroItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        notafical_id = c.Int(nullable: false),
                        Codigo = c.String(maxLength: 20),
                        Descricao = c.String(maxLength: 70),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotaFiscals", t => t.notafical_id, cascadeDelete: false)
                .Index(t => t.notafical_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotaFiscalOutroItems", "notafical_id", "dbo.NotaFiscals");
            DropIndex("dbo.NotaFiscalOutroItems", new[] { "notafical_id" });
            DropTable("dbo.NotaFiscalOutroItems");
        }
    }
}
