namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Operacao : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Operacaos", "percentualCargaTributaria", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Operacaos", "fonteCargaTributaria", c => c.String(maxLength: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Operacaos", "fonteCargaTributaria");
            DropColumn("dbo.Operacaos", "percentualCargaTributaria");
        }
    }
}
