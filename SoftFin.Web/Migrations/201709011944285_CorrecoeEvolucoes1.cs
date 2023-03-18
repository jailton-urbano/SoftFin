namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrecoeEvolucoes1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CPAGEstimativas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        estabelecimento_id = c.Int(nullable: false),
                        pessoa_id = c.Int(nullable: false),
                        valorPadrao = c.Decimal(nullable: false, precision: 18, scale: 2),
                        banco_id = c.Int(nullable: false),
                        tipodocumento_id = c.Int(nullable: false),
                        tipolancamento = c.String(nullable: false, maxLength: 1),
                        usuarioinclusaoid = c.Int(),
                        usuarioalteracaoid = c.Int(),
                        planoDeConta_id = c.Int(nullable: false),
                        VigenciaInicial = c.DateTime(nullable: false),
                        VigenciaFinal = c.DateTime(),
                        diaVencimento = c.Int(nullable: false),
                        usaUltimoValorPago = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Bancoes", t => t.banco_id, cascadeDelete: true)
                .ForeignKey("dbo.Estabelecimentoes", t => t.estabelecimento_id, cascadeDelete: true)
                .ForeignKey("dbo.Pessoas", t => t.pessoa_id, cascadeDelete: true)
                .ForeignKey("dbo.PlanoDeContas", t => t.planoDeConta_id, cascadeDelete: true)
                .ForeignKey("dbo.TipoDocumentoes", t => t.tipodocumento_id, cascadeDelete: true)
                .ForeignKey("dbo.Usuarios", t => t.usuarioalteracaoid)
                .ForeignKey("dbo.Usuarios", t => t.usuarioinclusaoid)
                .Index(t => t.estabelecimento_id)
                .Index(t => t.pessoa_id)
                .Index(t => t.banco_id)
                .Index(t => t.tipodocumento_id)
                .Index(t => t.usuarioinclusaoid)
                .Index(t => t.usuarioalteracaoid)
                .Index(t => t.planoDeConta_id);
            
            CreateTable(
                "dbo.EstabelecimentoCodigoLanctoContabils",
                c => new
                    {
                        estabelecimento_id = c.Int(nullable: false),
                        codigoLancto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.estabelecimento_id)
                .ForeignKey("dbo.Estabelecimentoes", t => t.estabelecimento_id)
                .Index(t => t.estabelecimento_id);
            
            AddColumn("dbo.Estabelecimentoes", "codigoEstabelecimentoContabil", c => c.String(maxLength: 30));
            AddColumn("dbo.NotaFiscals", "percentualCargaTributaria", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscals", "fonteCargaTributaria", c => c.String(maxLength: 10));
            AddColumn("dbo.NotaFiscals", "codigoCEI", c => c.String(maxLength: 20));
            AddColumn("dbo.NotaFiscals", "matriculaObra", c => c.String(maxLength: 30));
            AddColumn("dbo.LancamentoContabils", "codigoLancamento", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EstabelecimentoCodigoLanctoContabils", "estabelecimento_id", "dbo.Estabelecimentoes");
            DropForeignKey("dbo.CPAGEstimativas", "usuarioinclusaoid", "dbo.Usuarios");
            DropForeignKey("dbo.CPAGEstimativas", "usuarioalteracaoid", "dbo.Usuarios");
            DropForeignKey("dbo.CPAGEstimativas", "tipodocumento_id", "dbo.TipoDocumentoes");
            DropForeignKey("dbo.CPAGEstimativas", "planoDeConta_id", "dbo.PlanoDeContas");
            DropForeignKey("dbo.CPAGEstimativas", "pessoa_id", "dbo.Pessoas");
            DropForeignKey("dbo.CPAGEstimativas", "estabelecimento_id", "dbo.Estabelecimentoes");
            DropForeignKey("dbo.CPAGEstimativas", "banco_id", "dbo.Bancoes");
            DropIndex("dbo.EstabelecimentoCodigoLanctoContabils", new[] { "estabelecimento_id" });
            DropIndex("dbo.CPAGEstimativas", new[] { "planoDeConta_id" });
            DropIndex("dbo.CPAGEstimativas", new[] { "usuarioalteracaoid" });
            DropIndex("dbo.CPAGEstimativas", new[] { "usuarioinclusaoid" });
            DropIndex("dbo.CPAGEstimativas", new[] { "tipodocumento_id" });
            DropIndex("dbo.CPAGEstimativas", new[] { "banco_id" });
            DropIndex("dbo.CPAGEstimativas", new[] { "pessoa_id" });
            DropIndex("dbo.CPAGEstimativas", new[] { "estabelecimento_id" });
            DropColumn("dbo.LancamentoContabils", "codigoLancamento");
            DropColumn("dbo.NotaFiscals", "matriculaObra");
            DropColumn("dbo.NotaFiscals", "codigoCEI");
            DropColumn("dbo.NotaFiscals", "fonteCargaTributaria");
            DropColumn("dbo.NotaFiscals", "percentualCargaTributaria");
            DropColumn("dbo.Estabelecimentoes", "codigoEstabelecimentoContabil");
            DropTable("dbo.EstabelecimentoCodigoLanctoContabils");
            DropTable("dbo.CPAGEstimativas");
        }
    }
}
