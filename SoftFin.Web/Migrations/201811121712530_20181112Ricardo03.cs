namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181112Ricardo03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoletoArquivoHistoricoes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        DataGeracao = c.DateTime(nullable: false),
                        Caminho = c.String(),
                        estabelecimento_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Estabelecimentoes", t => t.estabelecimento_id, cascadeDelete: false)
                .Index(t => t.estabelecimento_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoletoArquivoHistoricoes", "estabelecimento_id", "dbo.Estabelecimentoes");
            DropIndex("dbo.BoletoArquivoHistoricoes", new[] { "estabelecimento_id" });
            DropTable("dbo.BoletoArquivoHistoricoes");
        }
    }
}
