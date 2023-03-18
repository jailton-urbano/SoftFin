namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LCNovo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LancamentoContabils", "contaContabil_id", "dbo.ContaContabils");
            DropForeignKey("dbo.LancamentoContabils", "unidadeNegocio_ID", "dbo.UnidadeNegocios");
            DropIndex("dbo.LancamentoContabils", new[] { "contaContabil_id" });
            DropIndex("dbo.LancamentoContabils", new[] { "unidadeNegocio_ID" });
            CreateTable(
                "dbo.LancamentoContabilDetalhes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        lancamentoContabil_id = c.Int(nullable: false),
                        contaContabil_id = c.Int(nullable: false),
                        DebitoCredito = c.String(maxLength: 1),
                        valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        unidadeNegocio_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ContaContabils", t => t.contaContabil_id, cascadeDelete: false)
                .ForeignKey("dbo.LancamentoContabils", t => t.lancamentoContabil_id, cascadeDelete: false)
                .ForeignKey("dbo.UnidadeNegocios", t => t.unidadeNegocio_ID, cascadeDelete: false)
                .Index(t => t.lancamentoContabil_id)
                .Index(t => t.contaContabil_id)
                .Index(t => t.unidadeNegocio_ID);
            
            DropColumn("dbo.LancamentoContabils", "contaContabil_id");
            DropColumn("dbo.LancamentoContabils", "DebitoCredito");
            DropColumn("dbo.LancamentoContabils", "valor");
            DropColumn("dbo.LancamentoContabils", "unidadeNegocio_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LancamentoContabils", "unidadeNegocio_ID", c => c.Int(nullable: false));
            AddColumn("dbo.LancamentoContabils", "valor", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.LancamentoContabils", "DebitoCredito", c => c.String(maxLength: 1));
            AddColumn("dbo.LancamentoContabils", "contaContabil_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.LancamentoContabilDetalhes", "unidadeNegocio_ID", "dbo.UnidadeNegocios");
            DropForeignKey("dbo.LancamentoContabilDetalhes", "lancamentoContabil_id", "dbo.LancamentoContabils");
            DropForeignKey("dbo.LancamentoContabilDetalhes", "contaContabil_id", "dbo.ContaContabils");
            DropIndex("dbo.LancamentoContabilDetalhes", new[] { "unidadeNegocio_ID" });
            DropIndex("dbo.LancamentoContabilDetalhes", new[] { "contaContabil_id" });
            DropIndex("dbo.LancamentoContabilDetalhes", new[] { "lancamentoContabil_id" });
            DropTable("dbo.LancamentoContabilDetalhes");
            CreateIndex("dbo.LancamentoContabils", "unidadeNegocio_ID");
            CreateIndex("dbo.LancamentoContabils", "contaContabil_id");
            AddForeignKey("dbo.LancamentoContabils", "unidadeNegocio_ID", "dbo.UnidadeNegocios", "id", cascadeDelete: false);
            AddForeignKey("dbo.LancamentoContabils", "contaContabil_id", "dbo.ContaContabils", "id", cascadeDelete: false);
        }
    }
}
