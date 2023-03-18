namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nfcorrecao01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "valorCargaTributaria", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotaFiscals", "valorCargaTributaria");
        }
    }
}
