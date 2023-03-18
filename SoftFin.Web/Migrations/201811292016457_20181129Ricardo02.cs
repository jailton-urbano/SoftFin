namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181129Ricardo02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bancoes", "EmissaoBoletoAposNF", c => c.Boolean(nullable: false));
            AddColumn("dbo.Bancoes", "EmissaoBoletoComNFPDF", c => c.Boolean(nullable: false));
            DropColumn("dbo.Boletoes", "EmissaoBoletoAposNF");
            DropColumn("dbo.Boletoes", "EmissaoBoletoComNFPDF");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Boletoes", "EmissaoBoletoComNFPDF", c => c.Boolean(nullable: false));
            AddColumn("dbo.Boletoes", "EmissaoBoletoAposNF", c => c.Boolean(nullable: false));
            DropColumn("dbo.Bancoes", "EmissaoBoletoComNFPDF");
            DropColumn("dbo.Bancoes", "EmissaoBoletoAposNF");
        }
    }
}
