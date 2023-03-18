namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181218Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Estabelecimentoes", "MigrateCode", c => c.String());
            AddColumn("dbo.Estabelecimentoes", "CNAE", c => c.String());
            AddColumn("dbo.NotaFiscalNFEs", "TipoOperacao", c => c.Int(nullable: false));
            AddColumn("dbo.NotaFiscalNFEs", "IndicadorDestino", c => c.String());
            AddColumn("dbo.NotaFiscalNFEs", "IndicadorFinal", c => c.String());
            AddColumn("dbo.NotaFiscalNFEs", "IndicadorPresencial", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "cBenef", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "pRedBC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscalNFEItems", "valorFrete", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscalNFEItems", "EXTIPI", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "valorSeguro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NotaFiscalNFEItems", "nItemPed", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "dProd", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "nRECOPI", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "indEscala", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "CNPJFab", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "CST", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "aliquotaICMS", c => c.String());
            AddColumn("dbo.NotaFiscalNFEItems", "modBC", c => c.String());
            DropColumn("dbo.Estabelecimentoes", "MigateCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Estabelecimentoes", "MigateCode", c => c.String());
            DropColumn("dbo.NotaFiscalNFEItems", "modBC");
            DropColumn("dbo.NotaFiscalNFEItems", "aliquotaICMS");
            DropColumn("dbo.NotaFiscalNFEItems", "CST");
            DropColumn("dbo.NotaFiscalNFEItems", "CNPJFab");
            DropColumn("dbo.NotaFiscalNFEItems", "indEscala");
            DropColumn("dbo.NotaFiscalNFEItems", "nRECOPI");
            DropColumn("dbo.NotaFiscalNFEItems", "dProd");
            DropColumn("dbo.NotaFiscalNFEItems", "nItemPed");
            DropColumn("dbo.NotaFiscalNFEItems", "valorSeguro");
            DropColumn("dbo.NotaFiscalNFEItems", "EXTIPI");
            DropColumn("dbo.NotaFiscalNFEItems", "valorFrete");
            DropColumn("dbo.NotaFiscalNFEItems", "pRedBC");
            DropColumn("dbo.NotaFiscalNFEItems", "cBenef");
            DropColumn("dbo.NotaFiscalNFEs", "IndicadorPresencial");
            DropColumn("dbo.NotaFiscalNFEs", "IndicadorFinal");
            DropColumn("dbo.NotaFiscalNFEs", "IndicadorDestino");
            DropColumn("dbo.NotaFiscalNFEs", "TipoOperacao");
            DropColumn("dbo.Estabelecimentoes", "CNAE");
            DropColumn("dbo.Estabelecimentoes", "MigrateCode");
        }
    }
}
