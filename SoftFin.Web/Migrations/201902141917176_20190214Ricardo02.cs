namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190214Ricardo02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SaldoContabils",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataBase = c.DateTime(nullable: false),
                        Usuario = c.String(),
                        DataFechamento = c.DateTime(nullable: false),
                        HashCode = c.String(maxLength: 50),
                        Situacao = c.Int(nullable: false),
                        Estabelecimento_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estabelecimentoes", t => t.Estabelecimento_id, cascadeDelete: false)
                .Index(t => t.Estabelecimento_id);
            
            CreateTable(
                "dbo.SaldoContabilDetalhes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoConta = c.String(nullable: false, maxLength: 20),
                        CodigoCentroCusto = c.String(nullable: false, maxLength: 20),
                        DescricaoConta = c.String(nullable: false, maxLength: 150),
                        DescricaoCentroCusto = c.String(nullable: false, maxLength: 150),
                        SaldoInicial = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SaldoFinal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCredito = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDebito = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SaldoContabil_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SaldoContabils", t => t.SaldoContabil_id, cascadeDelete: false)
                .Index(t => t.SaldoContabil_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaldoContabilDetalhes", "SaldoContabil_id", "dbo.SaldoContabils");
            DropForeignKey("dbo.SaldoContabils", "Estabelecimento_id", "dbo.Estabelecimentoes");
            DropIndex("dbo.SaldoContabilDetalhes", new[] { "SaldoContabil_id" });
            DropIndex("dbo.SaldoContabils", new[] { "Estabelecimento_id" });
            DropTable("dbo.SaldoContabilDetalhes");
            DropTable("dbo.SaldoContabils");
        }
    }
}
