namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181129Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boletoes", "EmissaoBoletoAposNF", c => c.Boolean(nullable: false));
            AddColumn("dbo.Boletoes", "EmissaoBoletoComNFPDF", c => c.Boolean(nullable: false));
            AddColumn("dbo.PessoaContatoes", "RecebeCobranca", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PessoaContatoes", "RecebeCobranca");
            DropColumn("dbo.Boletoes", "EmissaoBoletoComNFPDF");
            DropColumn("dbo.Boletoes", "EmissaoBoletoAposNF");
        }
    }
}
